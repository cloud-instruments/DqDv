import { Component, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { DxDataGridComponent } from "devextreme-angular";
import { EdmLiteral } from "devextreme/data/odata/utils";
import "devextreme/integration/jquery";
import * as filesize from "filesize";
import { PlotTemplate } from "../chart/model/chart-plot-settings";
import { ShareSettings } from "../chart/model/share-settings";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../environments/environment";
import * as ChartActions from "../chart/state/chart.actions";
import { ProjectListItem } from "./model/project-list-item";
import { AppState, SetProjects, StartRefreshPlotTemplates, Unauthorized } from "../state";

import { ProjectService } from "../project/state/project.service"
import * as FileSaver from "file-saver";


interface ICellPreparedEvent {
    data: ProjectListItem;
    cellElement: any; // JQuery
    column: {
        command: string
    };
    rowType: string;
}

interface EditingStartEvent {
    data: ProjectListItem;
}

interface IEditorPreparingEvent {
    editorName: string;
    dataField: string;
    parentType: string;
    editorOptions: { height: number; }
}

interface IRowPreparedEvent {
    data: ProjectListItem;
    rowElement: any; // JQuery
    rowType: string;
}

interface ISelectionChangedEvent {
    component: any; // DxDataGrid;
    currentSelectedRowKeys: number[];
    selectedRowsData: ProjectListItem[];
}

interface SimpleItem {
    dataField: string;
    visible: boolean;
}

function hasData(project: ProjectListItem): boolean {
    return project.isReady && !project.failed;
}

@Component({
    templateUrl: "./project-list.component.html",
    styleUrls: ["./project-list.component.css"]
})
export class ProjectListComponent {
    @ViewChild(DxDataGridComponent)
    grid: DxDataGridComponent;   

    plotTemplates: PlotTemplate[];

    maxProjects = environment.maxProjects;
    customQuery = "";
    step = 1;
    dataSource: any;
    selected = [];
    selectedProjects = [];   
    filterRowVisible = false;
    private hasStitchedFromNames = false;

    constructor(private router: Router, private store: Store<AppState>, private http: HttpClient, private projectService: ProjectService) {
        this.customizeItem = this.customizeItem.bind(this);

        this.dataSource = {
            store: {
                type: "odata",
                key: "id",
                url: environment.serverBaseUrl + "odata/projects",
                version: 4,
                deserializeDates: false,
                withCredentials: true,
                errorHandler: (error) => this.onODataError(error),
                beforeSend: (req) => this.onODataBeforeSend(req)
            }
        };
        this.store.dispatch(new StartRefreshPlotTemplates());
        this.store.select(s => s.chart.plotTemplates).subscribe(s => this.plotTemplates = s);   
    }

    onPlot(): void {
        this.store.dispatch(new SetProjects(this.selected));
        this.step = 2;
    }

    onToggleEraseSettingsClick(): void {
        this.customQuery = "";
        this.filterRowVisible = false;
        this.grid.instance.clearFilter();
        this.grid.instance.refresh();
    }

    onAverage(): void {
        this.step = 5;
    }

    onStitch(): void {
        this.step = 3;
    }

    onView(): void {
     
        this.step = 4;
    }

    onCloseChart(): void {
        this.step = 1;
    }

    onCloseView(): void {
        this.step = 1;
    }

    onCloseStitcher(): void {
        this.step = 1;
    }

    onRefresh(): void {
        this.grid.instance.refresh();
    }

    formatFileSize(row: ProjectListItem): string {
        return filesize(row.fileSize);
    }

    formatNumCycles(row: ProjectListItem): number | undefined {
        return hasData(row) ? row.numCycles : undefined;
    }

    onCellPrepared(e: ICellPreparedEvent): void {
        if (e.rowType === "data" && e.column.command === "select" && !hasData(e.data)) {
            e.cellElement.find(".dx-select-checkbox").dxCheckBox("instance").option("disabled", true);
            e.cellElement.off();
        }
    }

    onEditingStart(e: EditingStartEvent): void {
        this.hasStitchedFromNames = !!e.data.stitchedFromNames;
    }

    onEditorPreparing(e: IEditorPreparingEvent): void {
        if (e.dataField === "comments") {
            e.editorName = "dxTextArea";
        }

        if (e.parentType === "filterRow") {
            e.editorOptions.height = undefined;
        }
    }

    customizeItem(item: SimpleItem): void {
        if (item.dataField === "stitchedFromNames" && !this.hasStitchedFromNames) {
            item.visible = false;
        }
    }

    onRowPrepared(e: IRowPreparedEvent): void {
        if (e.rowType === "data") {
            if (!e.data.isReady) {
                e.rowElement.css("color", "lightgrey");
                e.rowElement.css("font-style", "italic");
                e.rowElement.prop("title", "Project data is not ready");
            } else if (e.data.failed) {
                e.rowElement.css("color", "crimson");
                e.rowElement.css("font-style", "italic");
                e.rowElement.prop("title", "Project parse failed because of " + (e.data.error || "unknown reason"));
            }
        }
    }

    onSelectionChanged(e: ISelectionChangedEvent): void {
        const notReady = e.currentSelectedRowKeys.filter(id => !hasData(e.selectedRowsData.find(p => p.id === id)));
        e.component.deselectRows(notReady);
        this.selectedProjects = e.component.getSelectedRowsData();
    }

    onShareProjectSave(shareSettings: ShareSettings): void {
        let projects = [];
        for (const project of this.selectedProjects) {
            projects.push(project.id);
        }
        this.store.dispatch(new ChartActions.ShareProject(projects, shareSettings.email));
     }

    onToggleFilterRowClick(): void {
        this.filterRowVisible = !this.filterRowVisible;

        if (!this.filterRowVisible) {
            this.grid.instance.clearFilter();
        }
    }

    calculateDateTimeFilterExpression(filterValue: any, selectedFilterOperation: string): void {
        const filter = this["defaultCalculateFilterExpression"](filterValue, selectedFilterOperation);
        if (filter) {
            if (Array.isArray(filter[0])) {
                filter[0][2] = new EdmLiteral(filter[0][2].toISOString());
                filter[2][2] = new EdmLiteral(filter[2][2].toISOString());
            }
            else {
                filter[2] = new EdmLiteral(filter[2].toISOString());
            }
        }
        return filter;
    }

    customizeColumns(columns: any[]) {
        columns.find(col => col.dataField === "createdAt").serializeValue = (val) => val;
        columns.find(col => col.dataField === "updatedAt").serializeValue = (val) => val;
    }

    onODataError(error: any): void {
        if (error && error.httpStatus === 401) {
            this.store.dispatch(new Unauthorized());
        }
    }

    onODataBeforeSend(req: any): void {
        req.params.customQuery = this.customQuery;
    }

    onProjectDownload(projectId): void {
        const project = this.selectedProjects.find(item => item.id === projectId);
        this.projectService.download(project)
            .filter(file => !!file)
            .subscribe(file => FileSaver.saveAs(file.blob, file.name));
    }

    isAveragePlotCreationEnabled(): boolean {
        return (this.selected.length > 1 && this.selected.length < this.maxProjects) && this.selectedProjects.filter(item => item.isAveragePlot).length === 0;
    }
}
