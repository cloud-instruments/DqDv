export interface ArgumentTemplate {
    arg: string;
    multiplierType: number;
    type: string;
    isConstValue: boolean;
}

export interface FormulaTemplate {
    arg1: ArgumentTemplate;
    arg2: ArgumentTemplate;
    action: number;
}

export interface SeriesTemplate {
    axis: string;
    valueField: string;
    title: string;
    color: string;
    name: string;
    numerator: FormulaTemplate;
    denominator: FormulaTemplate;
}

export interface PlotTemplate {
    id: string;
    userId: string;
    name: string;
    useCRateCalculation: boolean;
    useDischargeCRateCalculation: boolean;
    useCycleData: boolean;  
    useAgregateData: boolean;  
    useFirstCycle: boolean;  
    checkChargeDischarge: boolean;
    canEdit: boolean;
    plotParameters: PlotParameters;
    xAxis: SeriesTemplate;
    yAxis: SeriesTemplate[];
}

export interface PlotSeries {
    name: string;
    title: string;
    selected: boolean;
}

export interface PlotParameters {
    maxCycles: number;
    maxPointsPerSeries: number;
    fromCycle: number;
    toCycle: number;
    everyNthCycle: number;
    customCycleFilter: number;
    disableCharge: boolean;
    disableDischarge: boolean;
    threshold: number;
    minY: number;
    maxY: number;
    currentUoM: number;
    capacityUoM: number;
    timeUoM: number;
    powerUoM: number;
    energyUoM: number;
    resistanceUoM: number;
    normalizeBy: number;
    pointSize: number;
    isInitialized: boolean;
    xLineVisible?: boolean;
    yLineVisible?: boolean;
    fontFamilyName: string;
    simplification: number;
    fontSize: number;
    legendShowen: boolean;
    xAxisText: string;
    yAxisText: string[];
    chartTitle: string;
    axisRange: AxisRange;
    chartPalette: string[];
}

export interface AxisRange {
    xAxis: AxisRangeItem;
    yAxis: AxisRangeItem;
    y2Axis: AxisRangeItem;
}

export interface AxisRangeItem {
    from: number;
    to: number;
}

