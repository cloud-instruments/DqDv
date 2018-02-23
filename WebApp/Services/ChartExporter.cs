/*
Copyright(c) <2018> <University of Washington>
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Plotting;

namespace WebApp.Services
{
    public class ChartExporter
    {
        #region Public methods

        public byte[] ToExcel(Chart chart)
        {
            var workbook = new XSSFWorkbook();
            CreateDataSheet(chart, workbook);
            return ToByteArray(workbook);
        }

        public byte[] ToExcel(Chart chart, Stream chartImageStream)
        {
            var workbook = new XSSFWorkbook();
            if (chartImageStream != null)
                CreateChartDataSheet(chartImageStream, workbook);
            CreateDataSheet(chart, workbook);
            return ToByteArray(workbook);
        }

        #endregion

        #region Private methods

        private static byte[] ToByteArray(IWorkbook workbook)
        {
            using (var stream = new MemoryStream())
            {
                workbook.Write(stream);
                return stream.ToArray();
            }
        }

        private static void CreateChartDataSheet(Stream stream, IWorkbook workbook)
        {
            var sheet = workbook.CreateSheet("Chart");
            var drawing = sheet.CreateDrawingPatriarch();

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);

            var picInd = workbook.AddPicture(data, PictureType.PNG);

            XSSFCreationHelper helper = workbook.GetCreationHelper() as XSSFCreationHelper;
            XSSFClientAnchor anchor = helper?.CreateClientAnchor() as XSSFClientAnchor;

            if (anchor != null)
            {
                anchor.Col1 = 0;
                anchor.Row1 = 0;

                var pict = drawing.CreatePicture(anchor, picInd) as XSSFPicture;
                pict?.Resize();
            }
        }

        private static void CreateDataSheet(Chart chart, IWorkbook workbook)
        {
            var sheet = workbook.CreateSheet("Data");

            var offset = 0;
            if (string.IsNullOrEmpty(chart.SelectedTemplateName))
            {
                foreach (var series in chart.Series)
                {
                    RenderSeries(chart, series, sheet, offset);
                    offset += 2;
                }
            }
            else
            {
                RenderSeries2Y(chart, sheet, offset);
            }
        }

        private static void RenderSeries2Y(Chart chart, ISheet sheet, int offset)
        {
            RenderHeader2Y(chart, sheet, offset);
            RenderData(chart.Series, sheet, offset, chart.YAxisText.Length > 1); 
        }

        private static void RenderSeries(Chart chart, Series series, ISheet sheet, int offset)
        {
            RenderHeader(chart, series, sheet, offset);
            RenderData(series.Points, sheet, offset);            
        }

        private static void RenderHeader2Y(Chart chart, ISheet sheet, int offset)
        {
            var header = GetRow(sheet, 0);
            header.CreateCell(offset++, CellType.String).SetCellValue($"{chart.XAxisText}");

            foreach (var serie in chart.Series)
            {
                header.CreateCell(offset ++, CellType.String).SetCellValue($"{serie.DisplayName}");
            }
        }

        private static void RenderHeader(Chart chart, Series series, ISheet sheet, int offset)
        {
            var header = GetRow(sheet, 0);
            header.CreateCell(offset, CellType.String).SetCellValue($"{series.DisplayName} [{chart.XAxisText}]");

            foreach (var title in chart.YAxisText)
            {
                header.CreateCell(offset + 1, CellType.String).SetCellValue($"{series.DisplayName} [{title}]");
            }
        }

        private static void RenderData(List<Series> series, ISheet sheet, int offset, bool is2Y)
        {
            if (series == null)
                return;

            var rowsCount = series.Select(s => s.Points.Count).Concat(new[] {0}).Max();
            var rowIndex = 1;
            var xValueExists = false;

            for (var rowIndexValue = 0; rowIndexValue < rowsCount; rowIndexValue++)
            {
                var row = GetRow(sheet, rowIndex++);
                if (rowIndexValue < series[0].Points.Count)
                {
                    xValueExists = true;
                    row.CreateCell(offset, CellType.Numeric).SetCellValue(series[0].Points[rowIndexValue].Discrete.GetValueOrDefault(series[0].Points[rowIndexValue].X));
                    if (!double.IsNaN(series[0].Points[rowIndexValue].Y))
                        row.CreateCell(offset + 1, CellType.Numeric).SetCellValue(series[0].Points[rowIndexValue].Y);
                }

                for (var serieIndexValue = 1; serieIndexValue < series.Count; serieIndexValue++)
                {
                    if (rowIndexValue < series[serieIndexValue].Points.Count)
                    {
                        if (!xValueExists)
                        {
                            row.CreateCell(offset, CellType.Numeric).SetCellValue(series[serieIndexValue].Points[rowIndexValue].Discrete.GetValueOrDefault(series[serieIndexValue].Points[rowIndexValue].X));
                        }
                        if (!double.IsNaN(series[serieIndexValue].Points[rowIndexValue].Y))
                            row.CreateCell(offset + 1 + serieIndexValue, CellType.Numeric).SetCellValue(series[serieIndexValue].Points[rowIndexValue].Y);
                    }
                }               
            }           
        }

        private static void RenderData(List<Point> points, ISheet sheet, int offset)
        {
            if (points == null)
                return;

            var rowIndex = 1;
            foreach (var point in points)
            {
                var row = GetRow(sheet, rowIndex++);
                row.CreateCell(offset, CellType.Numeric).SetCellValue(point.Discrete.GetValueOrDefault(point.X));
                row.CreateCell(offset + 1, CellType.Numeric).SetCellValue(point.Y);
            }
        }

        private static void RenderData(List<Point2Y> points, ISheet sheet, int offset, bool is2Y)
        {
            if (points == null)
                return;

            var rowIndex = 1;
            foreach (var point in points)
            {
                var row = GetRow(sheet, rowIndex++);
                row.CreateCell(offset, CellType.Numeric).SetCellValue(point.X);
                row.CreateCell(offset + 1, CellType.Numeric).SetCellValue(point.Y1);
               if(is2Y)
                row.CreateCell(offset + 2, CellType.Numeric).SetCellValue(point.Y2);
            }
        }

        private static IRow GetRow(ISheet sheet, int index)
        {
            return sheet.GetRow(index) ?? sheet.CreateRow(index);
        }
        #endregion
    }
}
