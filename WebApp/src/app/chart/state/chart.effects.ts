import { Injectable } from "@angular/core";
import { Actions, Effect } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { HttpErrorResponse } from "@angular/common/http";
import "rxjs/add/observable/of";
import "rxjs/add/operator/catch";
import "rxjs/add/operator/do";
import "rxjs/add/operator/map";
import "rxjs/add/operator/mergeMap";
import "rxjs/add/operator/switchMap";
import "rxjs/add/operator/withLatestFrom";
import * as FileSaver from "file-saver";

import { AppState } from "../../state";
import * as forRoot from "./chart.actions";
import { ChartService } from "./chart.service";
import * as Constants from "../../shared/errorMessages";

@Injectable()
export class ChartEffects {
    //@Effect()
    //onChangeParameters = this.actions$
    //    .ofType(forRoot.SET_PROJECTS)
    //    .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
    //    .switchMap(state =>
    //        this.chartService
    //            .saveParameters(state.cycleFilter, state.aggregationSettings, state.uomSettings
    //            , state.chart == null ? false : state.legendVisible, state.plotParameters)
    //    ).map(() => new forRoot.StartRefresh());     

    @Effect()
    onChangeParameters = this.actions$
        .ofType(forRoot.SET_PROJECTS)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state => Observable.of<forRoot.ChartAction>(new forRoot.StartRefresh()));     
    
    @Effect()
    onChangeChartSettings = this.actions$
        .ofType(forRoot.TOGGLE_LEGEND, forRoot.SET_AXIS_RANGE_SETTINGS, forRoot.SET_CYCLE_FILTER,
            forRoot.SET_AGGREGATION_SETTINGS, forRoot.SET_CHART_SETTINGS, forRoot.SET_UOM_SETTINGS)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state => 
            this.chartService.saveParameters(state.cycleFilter, state.aggregationSettings, state.uomSettings, state.chart == null ? false : state.legendVisible, state.plotParameters)
                .map(() => new forRoot.StartDataPointRefresh())
                .catch(() => Observable.of<forRoot.ChartAction>(new forRoot.EndRefresh(state.chart)))
        );     

    @Effect()
    onRefresh = this.actions$
        .ofType(forRoot.SET_PLOT_TYPE, forRoot.START_REFRESH)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state => 
            this.chartService
                .get(state.projects, state.plotType, state.plotTemplateId, state.viewId, state.stateOfCharge)
                .map(chart => new forRoot.EndRefresh(chart))
                .catch(error => {
                    return Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed( this.getFailedMessage(error)), new forRoot.EndRefresh(null));
            })
         );

    @Effect()
    onDataPointsRefresh = this.actions$
        .ofType(forRoot.START_DATAPOINTS_REFRESH)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
            .get(state.projects, state.plotType, state.plotTemplateId, state.viewId, state.stateOfCharge)
            .map(chart => new forRoot.EndDataPointRefresh(chart))
            .catch(error => {
                return Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(this.getFailedMessage(error)), new forRoot.EndRefresh(null));
            })
        );

    @Effect()
    onExport = this.actions$
        .ofType(forRoot.START_EXPORT)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .export(state.projects, state.plotType, state.cycleFilter, state.aggregationSettings, state.uomSettings, state.chart == null ? 1 : state.chart.pointSize
                , state.chart == null ? null : state.chart.selectedTemplateName, state.stateOfCharge, state.chartImageBlob)
                .do(blob => FileSaver.saveAs(blob, "Export.xlsx"))
                .map(() => new forRoot.EndExport())
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.ExportFailed(error), new forRoot.EndExport()))
    );

    @Effect()
    onExportAll = this.actions$
        .ofType(forRoot.START_EXPORT_ALL)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .exportAll(state.projects, state.plotType, state.uomSettings, state.chart == null ? 1 : state.chart.pointSize
                , state.chart == null ? null : state.chart.selectedTemplateName, state.stateOfCharge, state.chartImageBlob)
                .do(blob => FileSaver.saveAs(blob, "Export.xlsx"))
                .map(() => new forRoot.EndExport())
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.ExportFailed(error), new forRoot.EndExport()))
        );

    @Effect()
    onSaveTemplate = this.actions$
        .ofType(forRoot.SAVE_PLOT_TEMPLATE)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .savetemplate(state.projects, state.plotTemplate)
                .mergeMap(plotsData => 
                    [
                        new forRoot.SelectPlotsTemplate(plotsData.selectedTemplate),
                        new forRoot.StartRefreshPlotTemplates()
                    ])
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(error), new forRoot.EndPlotTemplatesRefresh(null)))
    );
    
    @Effect()
    onSelectView = this.actions$
        .ofType(forRoot.SELECT_VIEW)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .getbyview(state.viewId)
                .map(chart => new forRoot.SelectViewCompleated(chart))
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(this.getFailedMessage(error)), new forRoot.EndRefresh(null)))
        );

    @Effect()
    onChangePlots = this.actions$
        .ofType(forRoot.SELECT_PLOT_TEMPLATE)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .do(state => this.store.next( { type: forRoot.SET_DEFAULT_PARAMS } ))
        .switchMap(state =>
            this.chartService
                .getbytemplate(state.projects, state.plotTemplateId)
                .map(chart => new forRoot.EndRefresh(chart))
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(error), new forRoot.EndRefresh(null)))
    );

    @Effect()
    onStateOfCharge = this.actions$
        .ofType(forRoot.SET_STATE_OF_CHARGE)
        .switchMap((action: forRoot.SetStateOfCharge) =>
            this.chartService
                .setStateOfCharge(action.projects
                , action.stateOfCharge)
    ).map(chart => new forRoot.EndSetStateOfCharge(chart));

    @Effect()
    onShareTemplate = this.actions$
        .ofType(forRoot.SHARE_TEMPLATE)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .shareTemplate(state.shareSettings.objectIds, state.shareSettings.email, state.plotType)                
         );


    @Effect()
    onShareProject = this.actions$
        .ofType(forRoot.SHARE_PROJECT)
        .switchMap((action: forRoot.ShareProject) =>        
            this.chartService
                .shareProject(action.objectIds, action.email)
        );

    @Effect()
    onDeleteTemplate = this.actions$
        .ofType(forRoot.DELETE_PLOT_TEMPLATE)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .deletetemplate(state.projects, state.plotTemplate)
                .map(plotTemplates => new forRoot.StartRefreshPlotTemplates())
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(error), new forRoot.EndPlotTemplatesRefresh(null)))
    );

    @Effect()
    onRefreshPlotTemplates = this.actions$
        .ofType(forRoot.START_PLOT_TEMPLATE_REFRESH)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .getPlots()
                .map(plotTemplates => new forRoot.EndPlotTemplatesRefresh(plotTemplates))
                .catch(error => Observable.of<forRoot.ChartAction>(new forRoot.RefreshFailed(error), new forRoot.EndPlotTemplatesRefresh(null)))
        );

    @Effect()
    onSetDefaultParameters = this.actions$
        .ofType(forRoot.SET_DEFAULT_PARAMS)
        .withLatestFrom(this.store.select(s => s.chart), (_, state) => state)
        .switchMap(state =>
            this.chartService
                .setDefaultParameters()
        ).map(() => new forRoot.StartRefresh());
    constructor(private actions$: Actions, private store: Store<AppState>, private chartService: ChartService) {
    }

    private getFailedMessage(response: HttpErrorResponse): string {
        const body = response.error;
        const status = (body || {}).status;

        return Constants.errorMessages[status] || "";
    }
}
