import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { PlotSeries } from "../model";
import { environment } from "environments/environment";
import { AggregationSettings, AxisRange, StateOfCharge, AggregationType, PlotTemplate, PlotParameters, PlotsData, Chart, ChartFilter, ChartUoMSettings, ChartSettings } from "../model";

@Injectable()
export class ChartService {
    constructor(private http: HttpClient) {
    }

    get(projectIds: number[], plotType: number, plotTemplateId: string, viewId: number, stateOfCharge: StateOfCharge): Observable<Chart> {
        switch (plotType) {
            case -1:
                {
                    const url = this.makeByTemplateUrl(projectIds, plotTemplateId);
                    return this.http.get<Chart>(url, { withCredentials: true });                    
                }
            case -2:
                {
                    const url = this.makeByViewUrl(viewId);
                    return this.http.get<Chart>(url, { withCredentials: true });
                }
            case -3:
                {
                    const url = this.makeByViewUrl(viewId);
                    return this.setStateOfCharge(projectIds, stateOfCharge);
                }
            default: {
                const url = this.makeUrl(projectIds, plotType);
                return this.http.get<Chart>(url, { withCredentials: true });
            }
        }             
    }

    getbytemplate(projectIds: number[], plotTemplateId: string): Observable<Chart> {
        const url = this.makeByTemplateUrl(projectIds, plotTemplateId);
        return this.http.get<Chart>(url, { withCredentials: true });
    }


    getbyview(viewId: number): Observable<Chart> {
        const url = this.makeByViewUrl(viewId);
        return this.http.get<Chart>(url, { withCredentials: true });
    }

    export(projectIds: number[], plotType: number, cycleFilter: ChartFilter,
        aggregationSettings: AggregationSettings, uomSettings: ChartUoMSettings, pointSize: number, templateIdValue?: string, stateOfCharge?: StateOfCharge, data?: any): Observable<Blob> {
        const url = this.makeExportUrl(projectIds, plotType, cycleFilter, aggregationSettings, uomSettings, pointSize, templateIdValue, stateOfCharge, "xlsx");
        const formData: FormData = new FormData();
        formData.append("file", data);

        return this.http.post(url, formData, { responseType: "blob", withCredentials: true });
    }

    exportAll(projectIds: number[], plotType: number, uomSettings: ChartUoMSettings, pointSize: number, templateIdValue?: string, stateOfCharge?: StateOfCharge, data?: any): Observable<Blob> {
        const url = this.makeExportUrl(projectIds, plotType, null, null, uomSettings, pointSize, templateIdValue, stateOfCharge, "xlsx", false);
        const formData: FormData = new FormData();
        formData.append("file", data);

        return this.http.post(url, formData, { responseType: "blob", withCredentials: true });
    }

    savetemplate(projectIds: number[], plotTemplate: PlotTemplate): Observable<PlotsData> {
        const url = environment.serverBaseUrl + "api/plots";
        const templateModel = {
            newTemplate: plotTemplate,
            projectIds: projectIds
        };

        return this.http.post<PlotsData>(url, { templateModel: templateModel }, { withCredentials: true });
    }

    deletetemplate(projectIds: number[], plotTemplate: PlotTemplate): Observable<any> {
        const url = environment.serverBaseUrl + "api/plots/" + plotTemplate.id;
        return this.http.delete(url, { withCredentials: true });
    }

    getPlots(): Observable<PlotTemplate[]> {
        const url = environment.serverBaseUrl + "api/plots";

        return this.http.get<PlotTemplate[]>(url, { withCredentials: true });
    }

    saveParameters(cycleFilter: ChartFilter,
        aggregationSettings: AggregationSettings, uomSettings: ChartUoMSettings, legendVisible: boolean | null
        , plotParameters: PlotParameters): Observable<any> {
        const url = environment.serverBaseUrl + "api/setParameters";

        const parametersModel = {
            maxCycles: environment.maxCycles,
            simplification: 1,
            maxPointsPerSeries: environment.maxPointsPerSeries,
            fromCycle: null,
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
            isInitialized: false,
            normalizeBy: null,
            pointSize: null,
            xLineVisible: null,
            yLineVisible: null,
            legendShowen: legendVisible,
            xAxisText: null,
            yAxisText: [],
            chartTitle: null,
            axisRange: null,
        };

        if (plotParameters) {
            parametersModel.pointSize = plotParameters.pointSize;
            parametersModel.isInitialized = plotParameters.isInitialized;
            parametersModel.xLineVisible = plotParameters.xLineVisible;
            parametersModel.yLineVisible = plotParameters.yLineVisible;
            if (plotParameters.xAxisText != "") {
                parametersModel.xAxisText = plotParameters.xAxisText;
            }
            if (plotParameters.chartTitle != "") {
                parametersModel.chartTitle = plotParameters.chartTitle;
            }

            if (plotParameters.yAxisText.length > 0) {
                parametersModel.yAxisText = plotParameters.yAxisText;
            }
            if (plotParameters.axisRange) {
                parametersModel.axisRange = plotParameters.axisRange;
            }
        }

        
        if (aggregationSettings) {
            parametersModel.simplification = aggregationSettings.algorithm;
            if (aggregationSettings.algorithm === AggregationType.None) {
                parametersModel.maxPointsPerSeries = null;
            }
        }

        if (cycleFilter) {
            parametersModel.fromCycle = cycleFilter.from;
            parametersModel.toCycle = cycleFilter.to;
            parametersModel.everyNthCycle = cycleFilter.everyNth;
            if (cycleFilter.custom) {
                parametersModel.customCycleFilter = encodeURIComponent(cycleFilter.custom);
            }
            parametersModel.disableCharge = cycleFilter.disableCharge;
            parametersModel.disableDischarge = cycleFilter.disableDischarge;
            parametersModel.threshold = cycleFilter.threshold;
            parametersModel.minY = cycleFilter.minY;
            parametersModel.maxY = cycleFilter.maxY;
        }

        if (uomSettings) {
            if (uomSettings.currentUoM) {
                parametersModel.currentUoM = uomSettings.currentUoM;
            }

            if (uomSettings.capacityUoM) {
                parametersModel.capacityUoM = uomSettings.capacityUoM;
            }

            if (uomSettings.timeUoM) {
                parametersModel.timeUoM = uomSettings.timeUoM;
            }

            if (uomSettings.powerUoM) {
                parametersModel.powerUoM = uomSettings.powerUoM;
            }

            if (uomSettings.energyUoM) {
                parametersModel.energyUoM = uomSettings.energyUoM;
            }

            if (uomSettings.resistanceUoM) {
                parametersModel.resistanceUoM = uomSettings.resistanceUoM;
            }

            if (uomSettings.normalizeBy) {
                parametersModel.normalizeBy = uomSettings.normalizeBy;
            }
        }

        return this.http.post<any>(url, { parametersModel: parametersModel }, { withCredentials: true });
    }

    shareTemplate(objectIds: number[], email: string, plotType: number): Observable<any> {

        switch (plotType) {
            case -1:
                {
                    const url = environment.serverBaseUrl + "api/shareTemplate";

                    const parametersModel = {
                        objectIds: objectIds,
                        email: email
                    };

                    return this.http.post<any>(url, { parametersModel: parametersModel }, { withCredentials: true });
                }
            case -2:
                {
                    const url = environment.serverBaseUrl + "api/shareView";

                    const parametersModel = {
                        objectIds: objectIds,
                        email: email
                    };

                    return this.http.post<any>(url, { parametersModel: parametersModel }, { withCredentials: true });
                }
            default: {              
                return null;
            }
        }           
    }

    shareProject(objectIds: number[], email: string): Observable<any> {
        const url = environment.serverBaseUrl + "api/shareProject";

        const parametersModel = {
            objectIds: objectIds,
            email: email
        };

        return this.http.post<any>(url, { parametersModel: parametersModel }, { withCredentials: true });
    }

    setDefaultParameters(): Observable<any> {
        const url = environment.serverBaseUrl + "api/setDefaultParameters";

        const parametersModel = {
            maxCycles: environment.maxCycles,
            maxPointsPerSeries: environment.maxPointsPerSeries,
            fromCycle: null,
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
            isInitialized: true,
            simplification: 1,
            normalizeBy: null,
            pointSize: null,
            xLineVisible: null,
            yLineVisible: null,
            xAxisText: null,
            yAxisText: [],
            chartTitle: null
        };

        return this.http.post<any>(url, { parametersModel: parametersModel }, { withCredentials: true });
    }

    setStateOfCharge(projectIds: number[],
        stateOfCharge: StateOfCharge): Observable<Chart> {
        const url = this.makeSOCUrl(projectIds, stateOfCharge);
        return this.http.get<Chart>(url, { withCredentials: true });
    }

    private makeSOCUrl(projectIds: number[], stateOfCharge: StateOfCharge): string {
        let url = environment.serverBaseUrl + "api/setStateOfCharge?";

        for (const projectId of projectIds) {
            url += `&projectId=${projectId}`;
        }

        if (stateOfCharge) {
            if (stateOfCharge.chargeFrom) {
                url += `&chargeFrom=${stateOfCharge.chargeFrom}`;
            }

            if (stateOfCharge.chargeTo) {
                url += `&chargeTo=${stateOfCharge.chargeTo}`;
            }
        }
        return url;
    }

    private makeUrl(projectIds: number[], plotType: number): string {
        let url = environment.serverBaseUrl + "api/chart?plotType=" + plotType;       

        for (const projectId of projectIds) {
            url += `&projectId=${projectId}`;
        }             
        return url;
    }

    private makeExportUrl(projectIds: number[], plotType: number,
        cycleFilter: ChartFilter, aggregationSettings: AggregationSettings,
        uomSettings: ChartUoMSettings, pointSize: number = 1, templateIdValue?: string, stateOfCharge?: StateOfCharge,
        format?: string, useMaxCycles: boolean = true): string {
        let url = environment.serverBaseUrl + "api/exportchart?plotType=" + plotType;

        
        if (templateIdValue) {
            url += `&templateIdValue=${templateIdValue}`;
        }

        if (format) {
            url += `&format=${encodeURIComponent(format)}`;
        }

        for (const projectId of projectIds) {
            url += `&projectId=${projectId}`;
        }

        if (useMaxCycles && environment.maxCycles) {
            url += `&maxCycles=${environment.maxCycles}`;
        }

        if (aggregationSettings) {
            if (aggregationSettings.algorithm != AggregationType.None) {
                url += `&maxPointsPerSeries=${environment.maxPointsPerSeries}`;
            }
        }

        if (cycleFilter) {
            if (cycleFilter.from) {
                url += `&fromCycle=${cycleFilter.from}`;
            }

            if (cycleFilter.to) {
                url += `&toCycle=${cycleFilter.to}`;
            }

            if (cycleFilter.everyNth) {
                url += `&everyNthCycle=${cycleFilter.everyNth}`;
            }

            if (cycleFilter.custom) {
                url += `&customCycleFilter=${encodeURIComponent(cycleFilter.custom)}`;
            }

            if (cycleFilter.disableCharge) {
                url += `&disableCharge=${cycleFilter.disableCharge}`;
            }

            if (cycleFilter.disableDischarge) {
                url += `&disableDischarge=${cycleFilter.disableDischarge}`;
            }

            if (cycleFilter.threshold) {
                url += `&threshold=${cycleFilter.threshold}`;
            }

            if (cycleFilter.minY) {
                url += `&minY=${cycleFilter.minY}`;
            }

            if (cycleFilter.maxY) {
                url += `&maxY=${cycleFilter.maxY}`;
            }
        }
        if (uomSettings) {
            if (uomSettings.currentUoM) {
                url += `&currentUoM=${uomSettings.currentUoM}`;
            }

            if (uomSettings.capacityUoM) {
                url += `&capacityUoM=${uomSettings.capacityUoM}`;
            }

            if (uomSettings.timeUoM) {
                url += `&timeUoM=${uomSettings.timeUoM}`;
            }

            if (uomSettings.powerUoM) {
                url += `&powerUoM=${uomSettings.powerUoM}`;
            }

            if (uomSettings.energyUoM) {
                url += `&energyUoM=${uomSettings.energyUoM}`;
            }

            if (uomSettings.resistanceUoM) {
                url += `&resistanceUoM=${uomSettings.resistanceUoM}`;
            }

            if (uomSettings.normalizeBy) {
                url += `&normalizeBy=${uomSettings.normalizeBy}`;
            }
        }
        if (stateOfCharge) {
            if (stateOfCharge.chargeFrom) {
                url += `&chargeFrom=${stateOfCharge.chargeFrom}`;
            }

            if (stateOfCharge.chargeTo) {
                url += `&chargeTo=${stateOfCharge.chargeTo}`;
            }
        }
        url += `&pointSize=${pointSize}`;
        return url;
    }

    private makeByTemplateUrl(projectIds: number[], templateIdValue: string, format?: string): string {
        let url = environment.serverBaseUrl + "api/plots/" + templateIdValue + "?";

        if (format) {
            url += `format=${encodeURIComponent(format)}&`;
        }

        for (const projectId of projectIds) {
            url += `projectId=${projectId}&`;
        }
        return url;
    }

    private makeByViewUrl(viewIdValue: number, format?: string): string {
        let url = `${environment.serverBaseUrl}api/views/${viewIdValue}`;

        if (format) {
            url += `&format=${encodeURIComponent(format)}`;
        }
        return url;
    }
}
