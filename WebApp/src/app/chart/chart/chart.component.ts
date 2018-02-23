import { AfterViewInit, Component, Input, OnChanges, SimpleChanges, ViewChild } from "@angular/core";
import { Actions } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { AppState } from "../../state";
import { BrowserModule } from "@angular/platform-browser";
import { platformBrowserDynamic } from "@angular/platform-browser-dynamic";
import { DxChartModule, DxChartComponent, DxRangeSelectorModule } from "devextreme-angular";
import DxChart from "devextreme/viz/chart";
import { PagerService } from "../service/pager-service";
import { registerPalette, currentPalette, getPalette } from "devextreme/viz/palette";

import { Chart, Label, AxisRange } from "../model";

@Component({
    selector: "app-chart",
    templateUrl: "./chart.component.html",
    styleUrls: ["./chart.component.css"]
})
export class ChartComponent implements AfterViewInit {
    @ViewChild(DxChartComponent) chartComponent: DxChartComponent;
    
    @Input()
    label: Label;

    @Input()
    axisRange: AxisRange;

    @Input()
    legendVisible: boolean;

    @Input()
    selectorVisible: boolean;

    @Input()
    title: string;

    @Input()
    useAggregation: boolean;

    @Input()
    refreshing: boolean;

    @Input()
    chart: Chart;

    constructor(private pagerService: PagerService) {
        this.discreteItems = {};
        this.customizeText = this.customizeText.bind(this);
    }

    pager: any = {};
    pagedItems: any[];  
    private discreteItems: any = {};
      
    setPage(page: number) {
        if (page < 1 || page > this.pager.totalPages) {
            return;
        }
        this.pager = this.pagerService.getPager(this.chartComponent.instance.getAllSeries().length, page);
        this.pagedItems = this.chartComponent.instance.getAllSeries().slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    get totalSeries(): number {
        return this.chartComponent && this.chartComponent.instance && this.chartComponent.instance.getAllSeries().length || 0;
    }
    
    onShowHideSeries(e): void {
        for (const series of this.chartComponent.instance.getAllSeries()) {           
            if (e.target.checked === false) {
                series.hide();
            } else {
                series.show();
            }
        }
    }

    onSeriesClick(series): void {
       var i_series =  this.chartComponent.instance.getSeriesByName(series.name);
       if (i_series.isVisible() === true) {
           i_series.hide();
        } else {
           i_series.show();
        }
    }

    ngAfterViewInit(): void {
        if (this.chart) {
            this.onChartChanged(this.chartComponent.instance, this.chart);            
        }
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes["chart"]) {
            const control = this.chartComponent.instance;
            if (control && this.chart) {
                this.onChartChanged(control, this.chart);       
                this.onChartAxisChanged(control, this.chart);   
            }
        }
    }

    valueChanged(arg: any) {
        this.chartComponent.instance.zoomArgument(arg.value[0], arg.value[1]);
    }

    formatCrosshairLabel(value: number): string {
        return +value.toFixed(3) + "";
    }

    formatTooltip(point: { seriesName: string }): any {
        return {
            text: point.seriesName
        };
    }

    private onChartChanged(control: DxChart, chart: Chart): void {
        this.discreteItems = {};
        control.beginUpdate();
        control.option("series", chart.series);
        if (chart.series) {
            for (let i = 0; i < chart.series.length; i++) {
                var projectId = chart.series[i].projectId;
                const project = chart.projects.find(p => p.id === projectId);
                if (project && project.isAveragePlot) {
                    control.option("series[" + i + "].valueErrorBar", { highValueField: "highError", lowValueField: "lowError", opacity: 0.8, lineWidth: 1 });
                }
            }
        }

        control.option("dataSource", chart.points);
        control.option("title", chart.title);
        control.option("argumentAxis.type", "continuous");
        control.option("argumentAxis.label.customizeText", undefined);
        control.option("argumentAxis.tickInterval", chart.xAxisIsInteger ? 1 : undefined);
        control.option("argumentAxis.grid.visible", chart.plotParameters.xLineVisible);
        control.option("argumentAxis.title.text", chart.xAxisText);
        control.option("commonSeriesSettings.line.point.size", chart.plotParameters.pointSize);
        control.option("valueAxis[0].grid.visible", chart.plotParameters.yLineVisible);
        control.option("valueAxis[0].title.text", chart.yAxisText[0]);
        control.option("valueAxis[1].visible", chart.yAxisText.length > 1);
        control.option("valueAxis[1].grid.visible", chart.yAxisText.length > 1 && chart.plotParameters.yLineVisible);
        control.option("valueAxis[1].title.text", chart.yAxisText.length > 1 && chart.yAxisText[1]);

        const isCRate = chart.points && (chart.xAxisText === "CRate" || chart.xAxisText === "DischargeCRate");
        if (isCRate) {
            chart.points.map((i, index) => {
                this.discreteItems[`${i.x}`] = i.discrete;
            });

            control.option("argumentAxis.type", "discrete");
            control.option("argumentAxis.label.customizeText", this.customizeText);
        }

        control.option("legend.visible", chart.plotParameters.legendShowen);

        if (chart.plotParameters.chartPalette.length > 0) {
            registerPalette("userDefinedPalette", {
                simpleSet: chart.plotParameters.chartPalette
            });
            currentPalette("userDefinedPalette");
            control.option("palette", "userDefinedPalette");
        }
        
        control.endUpdate();
        if (this.chartComponent.instance.getAllSeries().length > 0) {
            this.setPage(1);
        }
    }

    private onChartAxisChanged(control: DxChart, chart: Chart): void {
        control.beginUpdate();
        const axisRange = chart.plotParameters.axisRange;
        if (axisRange) {
            let adjustOnZoom = false;

            //Change X first, because it does not affect zoom and value margins configurations
            if (axisRange.xAxis && (axisRange.xAxis.to ||axisRange.xAxis.from)) {
                adjustOnZoom = true;
                control.option("argumentAxis.valueMarginsEnabled", false);
                control.option("argumentAxis.max",  axisRange.xAxis.to);
                control.option("argumentAxis.min", axisRange.xAxis.from);
            } else {
                control.option("argumentAxis.valueMarginsEnabled", true);
                control.option("argumentAxis.min", undefined);
                control.option("argumentAxis.max", undefined);
            }

            if (axisRange.yAxis && (axisRange.yAxis.to || axisRange.yAxis.from)) {
                adjustOnZoom = false;
                control.option("valueAxis[0].valueMarginsEnabled", false);
                control.option("valueAxis[0].tickInterval", "0.1");

                control.option("valueAxis[0].min", axisRange.yAxis.from);
                control.option("valueAxis[0].max", axisRange.yAxis.to);
            } else {
                control.option("valueAxis[0].valueMarginsEnabled", true);
                control.option("valueAxis[0].tickInterval", undefined);
                control.option("valueAxis[0].min", undefined);
                control.option("valueAxis[0].max", undefined);
            }

            control.option("adjustOnZoom", adjustOnZoom);
        }
        control.endUpdate();       
    }

    onZoomEnd(e): void {
        e.component.render({ force: true });
    }

    private customizeText(arg: any): any {
        return this.discreteItems[`${arg.value}`];
    }
}
