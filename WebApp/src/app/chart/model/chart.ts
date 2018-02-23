import { PlotTemplate, PlotParameters } from "./chart-plot-settings";

export interface Point {
    x: number;
    [y: string]: number;   
}

export interface Series {
    id: string,
    projectId: number,
    name: string;
    color: string;
    valueField: string;    
    label: Label;
}

export interface Project {
    id: number,
    name: string;
    isAveragePlot: boolean;
}

export interface Label {
    minSpacing: number,
    font: {
        family: string,
        size: number
    }
}

export interface Chart {
    xAxisText: string;
    title: string;
    xAxisIsInteger: boolean;
    yAxisText: string[];
    projects: Project[];
    series: Series[];
    points: Point[];
    forcedEveryNthCycle?: number;
    pointSize: number;
    selectedSeries: Series;
    plotParameters: PlotParameters;
    projectIds: number[],
    selectedTemplateName: string;
}


export interface PlotsData {
    chart: Chart;
    selectedTemplate: PlotTemplate;
    plotTemplates: PlotTemplate[];
}