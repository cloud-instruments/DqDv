import { Component, Input } from "@angular/core";

import { Project } from "../model";

@Component({
    selector: "app-legend",
    templateUrl: "./legend.component.html",
    styleUrls: ["./legend.component.css"]
})
export class LegendComponent {
    @Input()
    projects: Project[];
}
