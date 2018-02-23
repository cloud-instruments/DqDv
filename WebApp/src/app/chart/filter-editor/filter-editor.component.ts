import { Component, EventEmitter, Input, Output } from "@angular/core";
import { DxNumberBoxComponent } from "devextreme-angular";

import { ChartFilter } from "../model";

@Component({
    selector: "app-filter-editor",
    templateUrl: "./filter-editor.component.html",
    styleUrls: ["./filter-editor.component.css"]
})
export class FilterEditorComponent {
    @Input()
    value: ChartFilter;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<ChartFilter>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(from: DxNumberBoxComponent, to: DxNumberBoxComponent, everyNth: DxNumberBoxComponent, custom: HTMLInputElement,
        disableCharge: HTMLInputElement, disableDischarge: HTMLInputElement,
        threshold: DxNumberBoxComponent, minY: DxNumberBoxComponent, maxY: DxNumberBoxComponent): void {
        this.save.emit({
            from: from.value,
            to: to.value,
            everyNth: everyNth.value,
            custom: custom.value,
            disableCharge: disableCharge.checked,
            disableDischarge: disableDischarge.checked,
            threshold: threshold.value,
            minY: minY.value,
            maxY: maxY.value
        });
    }
}
