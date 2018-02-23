import { Component, EventEmitter, Input, Output } from "@angular/core";
import { ShareSettings } from "../model/share-settings";
import { Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { AppState } from "../../state";


@Component({
    selector: "app-share-instance-editor",
    templateUrl: "share-instance-editor.component.html",
    styleUrls: [ "share-instance-editor.component.css" ]
})
export class ShareInstanceEditorComponent {   
    @Input()
    objectIdValue: any;

    email: string;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<ShareSettings>();

    constructor(private router: Router, private store: Store<AppState>) {
        this.store.select(s => s.chart).subscribe(s => s.plotType === -1 ? this.objectIdValue = s.plotTemplateId
            : s.plotType === -2 ? this.objectIdValue = s.viewId : null);
    }

    setEmail(e): void {
        this.email = e.target.value;
    }

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(): void {
        if (this.email != '')
            this.save.emit({ email: this.email, objectIds: [this.objectIdValue] });
    }
}
