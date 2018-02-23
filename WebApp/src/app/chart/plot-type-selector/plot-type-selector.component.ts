import { Component, EventEmitter, Input, Output } from "@angular/core";

@Component({
    selector: "app-plot-type-selector",
    templateUrl: "plot-type-selector.component.html",
    styleUrls: ["plot-type-selector.component.css"]
})
export class PlotTypeSelectorComponent {
    @Input()
    value: number;
    @Input()
    averagePlotsOnly: boolean;

    @Output()
    change = new EventEmitter<number>();

    setValue(value: number) {
        this.change.emit(value);
    }
}
