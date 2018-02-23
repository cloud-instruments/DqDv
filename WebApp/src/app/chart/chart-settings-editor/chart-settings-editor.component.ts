import { Component, EventEmitter, OnDestroy, Input, Output, OnInit } from "@angular/core";
import { DxNumberBoxComponent } from "devextreme-angular";
import { Chart, Label } from "../model/chart";
import { ChartSettings } from "../model/chart-settings";

@Component({
    selector: "app-chart-settings-editor",
    templateUrl: "./chart-settings-editor.component.html",
    styleUrls: ["./chart-settings-editor.component.css"]
})

export class ChartSettingsEditorComponent implements OnInit {
    @Input()
    yAxisText1: string;

    @Input()
    yAxisText2: string;

    @Input()
    value: ChartSettings;

    @Input()
    chart: Chart;

    @Input()
    label: Label;

    @Input()
    legendVisible: boolean;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<ChartSettings>();

    ngOnInit() {
        this.yAxisText1 = this.chart.yAxisText[0];
        if (this.chart.yAxisText.length > 1)
        {
            this.yAxisText2 = this.chart.yAxisText[1];
        }
        else
        {
            this.yAxisText2 = null;
        }

        this.value.xLineVisible = this.chart == null || this.chart.plotParameters.xLineVisible == null ?
            this.value.xLineVisible :
            this.chart.plotParameters.xLineVisible;

        this.value.yLineVisible = this.chart == null || this.chart.plotParameters.yLineVisible == null ?
            this.value.yLineVisible :
            this.chart.plotParameters.yLineVisible;
    }

    onyAxisText1(e): void {
        this.yAxisText1 = e.target.value;
    }

    onyAxisText2(e): void {
        this.yAxisText2  = e.target.value;
    }

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(chartTitle: HTMLInputElement, xAxisText: HTMLInputElement,
        pointSize: DxNumberBoxComponent, fontFamilyName: string, fontSize: DxNumberBoxComponent,
        xLineVisible: HTMLInputElement, yLineVisible: HTMLInputElement): void 
    {
        this.save.emit({
            chartTitle: chartTitle.value,
            fontFamilyName: fontFamilyName,
            pointSize: pointSize.value,
            fontSize: fontSize.value,
            xLineVisible: xLineVisible.checked,
            yLineVisible: yLineVisible.checked,
            xAxisText: xAxisText.value,
            yAxisText: [ this.yAxisText1, this.yAxisText2 ]
        });
    }
}
