import { Component, EventEmitter, Input, Output } from "@angular/core";
import { Router } from "@angular/router";
import { PlotTemplate, PlotSeries } from "../model/chart-plot-settings";
import { environment } from "../../../environments/environment";

import { AppState, SetProjects, StitchProjects, Unauthorized } from "../../state";

@Component({
    selector: "app-plot-editor",
    templateUrl: "plot-editor.component.html",
    styleUrls: ["plot-editor.component.css"]
})
export class PlotEditorComponent {
    @Input()
    plotSettings: PlotTemplate[];

    @Input()
    selectedTemplateName: string;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<PlotTemplate>();

    onCancel(): void {
        this.cancel.emit();
    }

    onSave(plotTemplates: HTMLInputElement) {
        this.save.emit(this.plotSettings.find(t => t.name === plotTemplates.value));
    }
}
