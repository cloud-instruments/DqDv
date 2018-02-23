import { AggregationSettings, AxisRange, StateOfCharge, PlotTemplate, ShareSettings, PlotParameters, Chart, ChartFilter, ChartUoMSettings, ChartSettings,Label } from "../model";

export interface ChartState {
    projects: number[];
    plotType: number;
    cycleFilter: ChartFilter;
    aggregationSettings: AggregationSettings;
    uomSettings: ChartUoMSettings;
    chartSettings: ChartSettings;
    chart: Chart;
    label: Label;
    refreshing: boolean;
    refreshingTemplate: boolean;
    exporting: boolean;
    legendVisible: boolean;
    selectorVisible: boolean;
    useAggregation: boolean;
    plotTemplateId: string;
    viewId: number;
    plotTemplate: PlotTemplate;
    plotParameters: PlotParameters;
    plotTemplates: PlotTemplate[];
    chartImageBlob: any;
    shareSettings: ShareSettings;
    stateOfCharge: StateOfCharge;
    axisRange: AxisRange;
}
