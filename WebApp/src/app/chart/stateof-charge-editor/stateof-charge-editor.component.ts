import { Component, EventEmitter, Input, Output } from "@angular/core";
import { DxNumberBoxComponent } from "devextreme-angular";

import { StateOfCharge, Chart } from "../model";

@Component({
    selector: "app-stateof-charge-editor",
    templateUrl: "./stateof-charge-editor.component.html",
    styleUrls: ["./stateof-charge-editor.component.css"]
})
export class StateOfChargeEditorComponent {
    @Input()
    value: StateOfCharge;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<StateOfCharge>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(chargeFrom: DxNumberBoxComponent, chargeTo: DxNumberBoxComponent): void {
        this.save.emit({
            chargeFrom: chargeFrom.value,
            chargeTo: chargeTo.value
        });
    }
}
