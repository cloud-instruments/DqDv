import { Component, EventEmitter, Input, Output } from "@angular/core";

import { ChartUoMSettings } from "../model/chart-uom-settings";

@Component({
    selector: "app-uom-settings-editor",
    templateUrl: "./uom-settings-editor.component.html",
    styleUrls: ["./uom-settings-editor.component.css"]
})
export class UoMSettingsEditorComponent {
    @Input()
    value: ChartUoMSettings;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<ChartUoMSettings>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(currentUoM: string, capacityUoM: string, timeUoM: string, powerUoM: string, energyUoM: string, resistanceUoM: string, normalizeBy: string): void {
        this.save.emit({
            currentUoM: +currentUoM,
            capacityUoM: +capacityUoM,
            timeUoM: +timeUoM,
            powerUoM: +powerUoM,
            energyUoM: +energyUoM,
            resistanceUoM: +resistanceUoM,
            normalizeBy: +normalizeBy
        });
    }
}
