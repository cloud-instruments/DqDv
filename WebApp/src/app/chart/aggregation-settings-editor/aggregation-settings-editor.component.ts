import { Component, EventEmitter, Input, Output } from "@angular/core";

import { AggregationSettings, AggregationType } from "../model";

@Component({
    selector: "app-aggregation-settings-editor",
    templateUrl: "aggregation-settings-editor.component.html",
    styleUrls: [ "aggregation-settings-editor.component.css" ]
})
export class AggregationSettingsEditorComponent {
    algorithms = [
        { key: AggregationType.Default, value: "Enabled" },
        { key: AggregationType.VisvalingamWhyatt, value: "Visvalingam-Whyatt" },
        { key: AggregationType.None, value: "None" }
    ];

    @Input()
    value: AggregationSettings;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<AggregationSettings>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(algorithm: string): void {
        this.save.emit({algorithm: +algorithm});
    }
}
