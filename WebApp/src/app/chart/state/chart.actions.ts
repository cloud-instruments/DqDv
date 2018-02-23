import { Action } from "@ngrx/store";
import { View } from "../../view/model/view-projects-params";

import { AggregationSettings, AxisRange, StateOfCharge, PlotTemplate, PlotsData, Chart, ChartFilter, ChartUoMSettings, ChartSettings, Series } from "../model";

export const SET_PROJECTS = "CHART: SET_PROJECTS";
export const SET_PLOT_TYPE = "CHART: SET_PLOT_TYPE";
export const SET_CYCLE_FILTER = "CHART: SET_CYCLE_FILTER";
export const SET_DEFAULT_CYCLE_FILTER = "CHART: SET_DEFAULT_CYCLE_FILTER";
export const SET_AGGREGATION_SETTINGS = "CHART: SET_AGGREGATION_SETTINGS";
export const SET_UOM_SETTINGS = "CHART: SET_UOM_SETTINGS";
export const START_REFRESH = "CHART: START_REFRESH";
export const END_REFRESH = "CHART: END_REFRESH";
export const REFRESH_FAILED = "CHART: REFRESH_FAILED";
export const START_EXPORT = "CHART: START_EXPORT";
export const END_EXPORT = "CHART: END_EXPORT";
export const EXPORT_FAILED = "CHART: EXPORT_FAILED";
export const START_EXPORT_ALL = "CHART: START_EXPORT_ALL";
export const TOGGLE_LEGEND = "CHART: TOGGLE_LEGEND";
export const SET_CHART_SETTINGS = "CHART: SET_CHART_SETTINGS";
export const SET_SERIES_SETTINGS = "CHART: SET_SERIES_SETTINGS";
export const TOGGLE_SELECTOR = "CHART: TOGGLE_SELECTOR";
export const SELECT_PLOT_TEMPLATE = "CHART: SELECT_PLOT_TEMPLATE";
export const DELETE_PLOT_TEMPLATE = "CHART: DELETE_PLOT_TEMPLATE";
export const SAVE_PLOT_TEMPLATE = "CHART: SAVE_PLOT_TEMPLATE";
export const GET_PLOTS_SETTINGS = "CHART: GET_PLOTS_SETTINGS";
export const PLOTS_SETTINGS = "CHART: PLOTS_SETTINGS";
export const SET_PLOTS_SETTINGS = "CHART: SET_PLOTS_SETTINGS";
export const REFRESH_PLOT_TEMPLATE = "CHART: REFRESH_PLOT_TEMPLATE";
export const REFRESH_PLOT_TEMPLATE_COMPLEATED = "CHART: REFRESH_PLOT_TEMPLATE_COMPLEATED";
export const END_PLOT_TEMPLATE_REFRESH = "CHART: END_PLOT_TEMPLATE_REFRESH";
export const START_PLOT_TEMPLATE_REFRESH = "CHART: START_PLOT_TEMPLATE_REFRESH";
export const END_PLOT_TEMPLATE_SAVE = "CHART: END_PLOT_TEMPLATE_SAVE";
export const SELECT_VIEW = "CHART: SELECT_VIEW";
export const SELECT_VIEW_COMPLEATED = "CHART: SELECT_VIEW_COMPLEATED";
export const SET_DEFAULT_PARAMS = "CHART: SET_DEFAULT_PARAMS";
export const SHARE_VIEW = "CHART: SHARE_VIEW";
export const SHARE_TEMPLATE = "CHART: SHARE_TEMPLATE";
export const SHARE_PROJECT = "CHART: SHARE_PROJECT";
export const SET_STATE_OF_CHARGE = "CHART: SET_STATE_OF_CHARGE";
export const END_SET_STATE_OF_CHARGE = "CHART: END_SET_STATE_OF_CHARGE";
export const SET_AXIS_RANGE_SETTINGS = "CHART: SET_AXIS_RANGE_SETTINGS";
export const START_DATAPOINTS_REFRESH = "CHART: START_DATAPOINTS_REFRESH";
export const END_DATAPOINTS_REFRESH = "CHART: END_DATAPOINTS_REFRESH";


export class SetProjects implements Action {
    readonly type = SET_PROJECTS;

    constructor(public projects: number[]) {
    }
}

export class SetPlotType implements Action {
    readonly type = SET_PLOT_TYPE;

    constructor(public plotType: number) {
    }
}

export class SetDefaultCycleFilter implements Action {
    readonly type = SET_DEFAULT_CYCLE_FILTER;

    constructor(public cycleFilter: ChartFilter) {
    }
}

export class SetCycleFilter implements Action {
    readonly type = SET_CYCLE_FILTER;

    constructor(public cycleFilter: ChartFilter) {
    }
}

export class SetAggregationSettings implements Action {
    readonly type = SET_AGGREGATION_SETTINGS;

    constructor(public aggregationSettings: AggregationSettings) {
    }
}

export class SetUoMSettings implements Action {
    readonly type = SET_UOM_SETTINGS;

    constructor(public uomSettings: ChartUoMSettings) {
    }
}

export class SetChartSettings implements Action {
    readonly type = SET_CHART_SETTINGS;

    constructor(public chartSettings: ChartSettings) {
    }
}

export class StartRefresh implements Action {
    readonly type = START_REFRESH;
}

export class EndRefresh implements Action {
    readonly type = END_REFRESH;

    constructor(public chart: Chart) {
    }
}

export class RefreshFailed implements Action {
    readonly type = REFRESH_FAILED;

    constructor(public error: any) {
    }
}

export class StartExport implements Action {
    readonly type = START_EXPORT;

    constructor(public chartImageBlob: any) {
    }
}

export class EndExport implements Action {
    readonly type = END_EXPORT;
}

export class ExportFailed implements Action {
    readonly type = EXPORT_FAILED;

    constructor(public error: any) {
    }
}

export class StartExportAll implements Action {
    readonly type = START_EXPORT_ALL;

    constructor(public chartImageBlob: any) {
    }
}

export class ToggleLegend implements Action {
    readonly type = TOGGLE_LEGEND;
    constructor(public legendShowen: boolean) {
    }
}

export class ToggleSelector implements Action {
    readonly type = TOGGLE_SELECTOR;
}

export class SetSeriesSettings implements Action {
    readonly type = SET_SERIES_SETTINGS;

    constructor(public series: Series[]) {
    }
}

export class DeletePlotsTemplate implements Action {
    readonly type = DELETE_PLOT_TEMPLATE;

    constructor(public plotTemplate: PlotTemplate) {
    }
}

export class SelectPlotsTemplate implements Action {
    readonly type = SELECT_PLOT_TEMPLATE;

    constructor(public plotTemplate: PlotTemplate) {
    }
}

export class SavePlotsTemplate implements Action {
    readonly type = SAVE_PLOT_TEMPLATE;

    constructor(public plotTemplate: PlotTemplate) {
    }
}

export class PlotsSettings implements Action {
    readonly type = PLOTS_SETTINGS;
}

export class RefreshPlotTemplate implements Action {
    readonly type = REFRESH_PLOT_TEMPLATE;
}

export class RefreshPlotTemplateCompleated implements Action {
    readonly type = REFRESH_PLOT_TEMPLATE_COMPLEATED;
    constructor(public chart: Chart) {
    }
}

export class StartRefreshPlotTemplates implements Action {
    readonly type = START_PLOT_TEMPLATE_REFRESH;
    constructor() {
    }
}

export class EndPlotTemplatesSave implements Action {
    readonly type = END_PLOT_TEMPLATE_SAVE;

    constructor(public plotsData: PlotsData) {
    }
}


export class EndPlotTemplatesRefresh implements Action {
    readonly type = END_PLOT_TEMPLATE_REFRESH;

    constructor(public plotTemplates: PlotTemplate[]) {
    }
}

export class SelectView implements Action {
    readonly type = SELECT_VIEW;

    constructor(public view: View) {
    }
}

export class SelectViewCompleated implements Action {
    readonly type = SELECT_VIEW_COMPLEATED;
    constructor(public chart: Chart) {
    }
}

export class SetDefaultParams implements Action {
    readonly type = SET_DEFAULT_PARAMS;
    constructor() {
    }
}

export class ShareView implements Action {
    readonly type = SHARE_VIEW;
    constructor(public objectIds: number[], public email: string) {
    }
}

export class ShareTemplate implements Action {
    readonly type = SHARE_TEMPLATE;
    constructor(public objectIds: number[], public email: string) {
    }
}

export class ShareProject implements Action {
    readonly type = SHARE_PROJECT;
    constructor(public objectIds: number[], public email: string) {
    }
}

export class SetStateOfCharge implements Action {
    readonly type = SET_STATE_OF_CHARGE;
    constructor(public stateOfCharge: StateOfCharge
        , public projects: number[]) {
    }
}

export class EndSetStateOfCharge implements Action {
    readonly type = END_SET_STATE_OF_CHARGE;
    constructor(public chart: Chart) {
    }
}

export class SetAxisRange implements Action {
    readonly type = SET_AXIS_RANGE_SETTINGS;

    constructor(public axisRange: AxisRange) {
    }
}

export class StartDataPointRefresh implements Action {
    readonly type = START_DATAPOINTS_REFRESH;
}

export class EndDataPointRefresh implements Action {
    readonly type = END_DATAPOINTS_REFRESH;
    constructor(public chart: Chart) {
    }
}


export type ChartAction
    = SetProjects
    | SetPlotType
    | SetAxisRange
    | SetStateOfCharge
    | EndSetStateOfCharge
    | SetCycleFilter
    | SetAggregationSettings
    | ShareProject
    | ShareView
    | ShareTemplate
    | SetUoMSettings
    | StartRefresh
    | EndRefresh
    | RefreshFailed
    | StartExport
    | EndExport
    | ExportFailed
    | StartExportAll
    | ToggleLegend
    | SetChartSettings
    | SetSeriesSettings
    | ToggleSelector
    | SelectPlotsTemplate    
    | DeletePlotsTemplate
    | SavePlotsTemplate
    | PlotsSettings
    | RefreshPlotTemplate
    | RefreshPlotTemplateCompleated
    | StartRefreshPlotTemplates
    | EndPlotTemplatesSave
    | EndPlotTemplatesRefresh
    | SelectView
    | SelectViewCompleated
    | SetDefaultCycleFilter
    | SetDefaultParams
    | StartDataPointRefresh
    | EndDataPointRefresh;
