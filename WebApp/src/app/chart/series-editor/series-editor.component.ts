import { Component, EventEmitter, Input, Output, OnInit} from "@angular/core";
import { PagerService } from "../service/pager-service";
import { Series } from "../model/chart";

@Component({
    selector: "app-series-editor",
    templateUrl: "./series-editor.component.html",
    styleUrls: ["./series-editor.component.css"]
})
export class SeriesEditorComponent implements OnInit {

    @Input()
    chartSeries: Series[];

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<Series[]>();

    @Output()
    leave = new EventEmitter();

    constructor(private pagerService: PagerService) { }
    pager: any = {};
    pagedItems: any[];
    ngOnInit() {
        this.setPage(1);
    }

    setPage(page: number) {
        if (page < 1 || page > this.pager.totalPages) {
            return;
        }
        this.pager = this.pagerService.getPager(this.chartSeries.length, page);
        this.pagedItems = this.chartSeries.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }

    onLeave(e): void {
        const indexSeries = this.chartSeries.findIndex(c => c.valueField === e.target.id);
        this.chartSeries[indexSeries].name = e.target.value;
    }


    onCancel(): void {
        this.cancel.emit();
    }

    onSave(series: Series[]): void {
        this.save.emit(series);
    }
}
