import * as forRoot from "./chart.actions";
import { ChartState } from "./chart.state";
import { AggregationType, AxisRange, ShareSettings } from "../model";

const initialState: ChartState = {
    projects: [],
    plotType: 0,
    cycleFilter: {},
    aggregationSettings: {
        algorithm: AggregationType.Default
    },
    uomSettings: {
        currentUoM: 0,
        capacityUoM: 0,
        timeUoM: 0,
        powerUoM: 0,
        energyUoM: 0,
        resistanceUoM: 0,
        normalizeBy: 0
    },
    chartSettings: {
        chartTitle: "",
        yAxisText: [],
        xAxisText: "",
        pointSize: null,
        fontFamilyName: "",
        fontSize: 0,
        xLineVisible: false,
        yLineVisible: true
    },
    chart: null,
    plotTemplateId: "",
    viewId: null,
    plotTemplate: null,
    plotTemplates: [],   
    plotParameters: {
        maxCycles: 100,
        maxPointsPerSeries: 1000,
        fromCycle: null,
        legendShowen: null,
        toCycle: null,
        everyNthCycle: null,
        customCycleFilter: null,
        disableCharge: null,
        disableDischarge: null,
        threshold: null,
        minY: null,
        maxY: null,
        currentUoM: null,
        capacityUoM: null,
        timeUoM: null,
        powerUoM: null,
        energyUoM: null,
        resistanceUoM: null,
        normalizeBy: null,
        isInitialized: true,
        xLineVisible: null,
        yLineVisible: null,
        fontFamilyName: "Verdana",
        fontSize: -1,
        simplification: 1,
        pointSize: null,
        xAxisText: null,
        yAxisText: [],
        chartTitle: null,
        axisRange: {
            xAxis: null,
            yAxis: null,
            y2Axis: null,
        },
        chartPalette: []
    },
    label: {
        minSpacing: 5,
        font: {
            family: "",
            size: -1
        }
    },
    refreshing: false,
    refreshingTemplate: false,
    legendVisible: false,
    exporting: false,
    selectorVisible: false,
    useAggregation: true,
    chartImageBlob: null,
    shareSettings: null,
    stateOfCharge: {
        chargeFrom: null,
        chargeTo: null,
    },    
    axisRange: {
        xAxis: null,
        yAxis: null,
        y2Axis: null,
    }
};

function getDefaultAggregationAlgorithm(plotType: number): AggregationType {
    if (plotType === 10 || plotType === 13 || plotType === 14) {
        return AggregationType.VisvalingamWhyatt;
    }
    return AggregationType.Default;
}

export function chartReducer(state: ChartState = initialState, action: forRoot.ChartAction): ChartState {
    switch (action.type) {
        case forRoot.SET_PROJECTS:
            return {
                ...state,
                chart: null,
                plotTemplateId: null,
                plotType: 0,
                viewId: null,
                shareSettings: null,
                projects: action.projects
            };
        case forRoot.SHARE_TEMPLATE:
            return {
                ...state,
                shareSettings: {
                    email: action.email,
                    objectIds: action.objectIds
                }
            };
        case forRoot.SHARE_PROJECT:
            return {
                ...state,
                shareSettings: {
                    email: action.email,
                    objectIds: action.objectIds
                }
            };
        case forRoot.SHARE_VIEW:
            return {
                ...state,
                shareSettings: {
                    email: action.email,
                    objectIds: action.objectIds
                }
            };
        case forRoot.SET_PLOT_TYPE:
            return {
                ...state,
                plotType: action.plotType,
                shareSettings: null,
                refreshing: true,
                aggregationSettings: {
                    ...state.aggregationSettings,
                    algorithm: getDefaultAggregationAlgorithm(action.plotType)
                }
            };

        case forRoot.SET_DEFAULT_CYCLE_FILTER:
            return {
                ...state,
                cycleFilter: action.cycleFilter
            };

        case forRoot.SET_CYCLE_FILTER:
            return {
                ...state,
                cycleFilter: action.cycleFilter
            };

        case forRoot.SET_AGGREGATION_SETTINGS:
            return {
                ...state,
                aggregationSettings: action.aggregationSettings
            };

        case forRoot.SET_UOM_SETTINGS:
            return {
                ...state,
                uomSettings: action.uomSettings
            };

        case forRoot.START_REFRESH:
            return {
                ...state,
                shareSettings: null,
                chart: null,
                refreshing: true
            };

        case forRoot.END_REFRESH:
            return {
                ...state,
                projects: state.projects, //action && action.chart && action.chart.projectIds,
                chart: action.chart,
                plotTemplateId: action && action.chart && action.chart.selectedTemplateName,
                plotParameters: action && action.chart && action.chart.plotParameters,
                label: {
                    minSpacing: 5,
                    font: {
                        family: !state.label.font.family && action && action.chart && action.chart.plotParameters && action.chart.plotParameters.fontFamilyName || state.label.font.family,
                        size: !state.label.font.family && action && action.chart && action.chart.plotParameters && action.chart.plotParameters.fontSize || state.label.font.size
                    }
                },
                axisRange: action && action.chart && action.chart.plotParameters.axisRange,
                cycleFilter: {
                    disableCharge: action && action.chart && action.chart.plotParameters.disableCharge,
                    disableDischarge: action && action.chart && action.chart.plotParameters.disableDischarge,
                    from: action && action.chart && action.chart.plotParameters.fromCycle,
                    to: action && action.chart && action.chart.plotParameters.toCycle,
                    everyNth: action && action.chart && action.chart.plotParameters.everyNthCycle,
                    minY: action && action.chart && action.chart.plotParameters.minY,
                    maxY: action && action.chart && action.chart.plotParameters.maxY,
                    threshold: action && action.chart && action.chart.plotParameters.threshold
                },
                legendVisible: action && action.chart && action.chart.plotParameters.legendShowen,
                refreshing: false
            };

        case forRoot.TOGGLE_LEGEND:
            return {
                ...state,
                chart: {
                    ...state.chart,
                    plotParameters: {
                        ...state.chart.plotParameters,
                        legendShowen: action.legendShowen
                    }
                },
                legendVisible: action.legendShowen
            };
        case forRoot.START_DATAPOINTS_REFRESH:
            return {
                ...state,
                refreshing: true
            };
        case forRoot.END_DATAPOINTS_REFRESH:
            return {
                ...state,
                chart: {
                    ...state.chart,
                    xAxisText: action.chart.xAxisText,
                    yAxisText: action.chart.yAxisText,
                    series: action.chart.series.map(s => {
                        var serie = (state.chart.series.find(s2 => s.id === s2.id) || { color: null, name: null });
                        s.color = serie.color;
                        s.name = serie.name || s.name;

                        return s;
                    }),
                    points: action.chart.points
                },
                cycleFilter: {
                    disableCharge: action.chart.plotParameters.disableCharge,
                    disableDischarge: action.chart.plotParameters.disableDischarge,
                    from: action.chart.plotParameters.fromCycle,
                    to: action.chart.plotParameters.toCycle,
                    everyNth: action.chart.plotParameters.everyNthCycle,
                    minY: action.chart.plotParameters.minY,
                    maxY: action.chart.plotParameters.maxY,
                    threshold: action.chart.plotParameters.threshold
                },
                legendVisible: action.chart.plotParameters.legendShowen,
                refreshing: false
            };
        case forRoot.START_EXPORT:
        case forRoot.START_EXPORT_ALL:
            return {
                ...state,
                shareSettings: null,
                chartImageBlob: action.chartImageBlob,
                exporting: true
            };

        case forRoot.END_EXPORT:
            return {
                ...state,
                exporting: false
            };

        case forRoot.SET_SERIES_SETTINGS:
            return {
                ...state,
                shareSettings: null,
                chart: {
                    ...state.chart,
                    xAxisText: state.chart.xAxisText.endsWith(" ") ?
                        state.chart.xAxisText.substring(0, state.chart.xAxisText.length - 1)
                        : state.chart.xAxisText + " ",
                    series: action.series
                }
            };

        case forRoot.SET_CHART_SETTINGS:
            return {
                ...state,
                shareSettings: null,
                plotParameters: {
                    ...state.plotParameters,
                    pointSize: action.chartSettings.pointSize,
                    xAxisText: action.chartSettings.xAxisText,
                    chartTitle: action.chartSettings.chartTitle,
                    yAxisText: action.chartSettings.yAxisText,
                    xLineVisible: action.chartSettings.xLineVisible,
                    yLineVisible: action.chartSettings.yLineVisible,
                },
                chart: {
                    ...state.chart,
                    xAxisText: action.chartSettings.xAxisText,
                    yAxisText: action.chartSettings.yAxisText,               
                    title: action.chartSettings.chartTitle,                   
                    plotParameters: {
                        ...state.chart.plotParameters,
                        pointSize: action.chartSettings.pointSize,
                        xLineVisible: action.chartSettings.xLineVisible,
                        yLineVisible: action.chartSettings.yLineVisible,
                    }
                },
                label: (action.chartSettings.fontFamilyName !== "" && action.chartSettings.fontFamilyName != null) ? {
                    minSpacing: 7,
                    font: {
                        family: action.chartSettings.fontFamilyName,
                        size: action.chartSettings.fontSize
                    }
                } : state.label
            };
        case forRoot.TOGGLE_SELECTOR:
            return {
                ...state,
                selectorVisible: !state.selectorVisible
            };
        case forRoot.REFRESH_PLOT_TEMPLATE:
            return {
                ...state,
                shareSettings: null,
                chart: null,
                refreshing: true
            };
        case forRoot.SELECT_PLOT_TEMPLATE:
            return {
                ...state,
                plotType: -1,
                chart: null,
                plotTemplateId: action.plotTemplate.id,
                aggregationSettings: {
                    ...state.aggregationSettings,
                    algorithm: action.plotTemplate.useAgregateData ?
                        getDefaultAggregationAlgorithm(10) :
                        state.aggregationSettings.algorithm
                },
                refreshing: true
            };
        case forRoot.DELETE_PLOT_TEMPLATE:
            return {
                ...state,
                shareSettings: null,
                plotType: 0,
                //chart: null,
                plotTemplate: action.plotTemplate,
                refreshing: true
            };
        case forRoot.SAVE_PLOT_TEMPLATE:
            return {
                ...state,
                chart: null,
                shareSettings: null,
                plotType: -1,
                plotTemplate: action.plotTemplate,
                aggregationSettings: {
                    ...state.aggregationSettings,
                    algorithm: action.plotTemplate.useAgregateData ?
                        getDefaultAggregationAlgorithm(10) :
                        state.aggregationSettings.algorithm
                },
                refreshing: true
            };
        case forRoot.REFRESH_PLOT_TEMPLATE_COMPLEATED:
            return {
                ...state,
                plotType: -1,
                shareSettings: null,
                projects: action.chart.projectIds,
                plotTemplateId: action.chart.selectedTemplateName,
                chart: action.chart,
                plotParameters: action.chart.plotParameters,
                label: {
                    minSpacing: 5,
                    font: {
                        family: action.chart.plotParameters.fontFamilyName,
                        size: action.chart.plotParameters.fontSize
                    }
                },
                refreshing: false
            };
        case forRoot.START_PLOT_TEMPLATE_REFRESH:
            return {
                ...state,             
                shareSettings: null,
                plotTemplates: null,
                refreshing: true
            };
        case forRoot.END_PLOT_TEMPLATE_REFRESH:
            return {
                ...state,
                shareSettings: null,
                plotTemplates: action.plotTemplates,
                refreshing: false
            };
        case forRoot.END_PLOT_TEMPLATE_SAVE:
            return {
                ...state,
                plotType: -1,
                shareSettings: null,
                projects: action.plotsData.chart.projectIds,
                plotTemplates: action.plotsData.plotTemplates,
                axisRange: action.plotsData.chart.plotParameters.axisRange,
                plotParameters: action.plotsData.chart.plotParameters,
                chart: action.plotsData.chart,
                plotTemplate: action.plotsData.selectedTemplate,
                plotTemplateId: action.plotsData.chart.selectedTemplateName,
                label: {
                    minSpacing: 5,
                    font: {
                        family: action.plotsData.chart.plotParameters.fontFamilyName,
                        size: action.plotsData.chart.plotParameters.fontSize
                    }
                },
                refreshing: true
            };
        case forRoot.SELECT_VIEW:
            return {
                ...state,
                shareSettings: null,
                projects: null,
                plotType: -2,
                viewId: action.view.id,
                chart: null,
                plotTemplate: null,
                refreshing: true
            };
        case forRoot.SELECT_VIEW_COMPLEATED:
            return {
                ...state,
                plotType: -2,
                shareSettings: null,
                projects: action.chart.projectIds,
                chart: action.chart,
                plotTemplateId: action.chart.selectedTemplateName,
                axisRange: action.chart.plotParameters.axisRange,
                plotParameters: action.chart.plotParameters,
                label: {
                    minSpacing: 5,
                    font: {
                        family: action.chart.plotParameters.fontFamilyName,
                        size: action.chart.plotParameters.fontSize
                    }
                }
            };
        case forRoot.SET_STATE_OF_CHARGE:
            return {
                ...state,
                chart: null,
                plotTemplateId: null,
                plotType: -3,
                viewId: null,
                stateOfCharge: action.stateOfCharge,
                projects: action.projects
            };
        case forRoot.END_SET_STATE_OF_CHARGE:
            return {
                ...state,
                plotType: -3,
                shareSettings: null,
                projects: action.chart.projectIds,
                plotTemplates: null,
                plotParameters: action.chart.plotParameters,
                axisRange: action.chart.plotParameters.axisRange,
                chart: action.chart,
                plotTemplate: null,
                plotTemplateId: null,
                label: {
                    minSpacing: 5,
                    font: {
                        family: action.chart.plotParameters.fontFamilyName,
                        size: action.chart.plotParameters.fontSize
                    }
                },
                refreshing: true
            };
        case forRoot.SET_AXIS_RANGE_SETTINGS:
            return {
                ...state,         
                axisRange: action.axisRange,
                plotParameters: {
                    ...state.plotParameters,
                    axisRange: action.axisRange
                },
                chart: {
                    ...state.chart,                  
                    plotParameters: {
                        ...state.chart.plotParameters,
                        axisRange: action.axisRange
                    }
                }
            };
        default:
            return state;
    }
}
