import { Component, EventEmitter, Input, Output } from "@angular/core";
import { DxNumberBoxComponent } from "devextreme-angular";

import { AxisRange, AxisRangeItem, Chart } from "../model";

@Component({
    selector: "app-axis-range-editor",
    templateUrl: "./axis-range-editor.component.html",
    styleUrls: ["./axis-range-editor.component.css"]
})
export class AxisRangeEditorComponent {
    @Input()
    value: AxisRange;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<AxisRange>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(xAxisFrom: DxNumberBoxComponent, xAxisTo: DxNumberBoxComponent,
        yAxisFrom: DxNumberBoxComponent, yAxisTo: DxNumberBoxComponent): void {
        this.save.emit(
        {
            xAxis: {
                from: xAxisFrom.value,
                to: xAxisTo.value
            },
            yAxis: {
                from: yAxisFrom.value,
                to: yAxisTo.value
            },
            y2Axis: null,
        });
    }
}
