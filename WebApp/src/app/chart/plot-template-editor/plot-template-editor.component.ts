import { Component, EventEmitter, Input, Output, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { PlotTemplate, SeriesTemplate } from "../model/chart-plot-settings";
import { environment } from "../../../environments/environment";

import { AppState, SetProjects, StitchProjects, Unauthorized } from "../../state";

@Component({
    selector: "app-plot-template-editor",
    templateUrl: "plot-template-editor.component.html",
    styleUrls: ["plot-template-editor.component.css"]
})

export class PlotTemplateEditorComponent implements OnInit {
    @Input()
    plotTemplate: PlotTemplate;

    @Input()
    selectedTemplateName: string;

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<PlotTemplate>();

    ngOnInit() {
        this.checkXAxisInitialized();
    }

    checkTemplateInitialized(): void {
        if (this.plotTemplate == null) {
            this.plotTemplate = {
                id: "",
                name: "",
                userId: "",
                checkChargeDischarge: false,
                useAgregateData: true,
                useCycleData: false,
                useFirstCycle: false,
                useCRateCalculation: false,
                useDischargeCRateCalculation: false,
                canEdit: true,
                xAxis: this.createNewSeriesTemplate(),
                yAxis: [],
                plotParameters : {
                    maxCycles: environment.maxCycles,
                    maxPointsPerSeries: environment.maxPointsPerSeries,
                    fromCycle: null,
                    toCycle: null,
                    everyNthCycle: null,
                    customCycleFilter: null,
                    disableCharge: null,
                    legendShowen: null,
                    simplification: 1,
                    disableDischarge: false,
                    threshold: null,
                    minY: null,
                    maxY: null,
                    xLineVisible: false,
                    yLineVisible: true,
                    isInitialized: true,
                    currentUoM: null,
                    capacityUoM: null,
                    timeUoM: null,
                    powerUoM: null,
                    energyUoM: null,
                    resistanceUoM: null,
                    normalizeBy: null,
                    fontFamilyName: "Verdana",
                    fontSize: 20,
                    pointSize: 1,
                    xAxisText: null,
                    yAxisText: [],
                    chartTitle: null,
                    axisRange: {
                        xAxis: null,
                        yAxis: null,
                        y2Axis: null
                    },
                    chartPalette: []
                }
            };
        }
    }

    checkXAxisInitialized(): void {
        this.checkTemplateInitialized();
        if (this.plotTemplate.xAxis == null) {
            this.plotTemplate.xAxis = this.createNewSeriesTemplate();
        }
    }

    checkXAxisDenominatorInitialized(): void {
        if (this.plotTemplate.xAxis.denominator == null) {
            this.plotTemplate.xAxis.denominator = {
                arg1: {
                    arg: "",
                    multiplierType: 0,
                    type: "",
                    isConstValue: false
                },
                arg2: null,
                action: 0
            };
        }
    }

    checkYAxisInitialized(index: number): void {
        if (this.plotTemplate.yAxis == null || this.plotTemplate.yAxis.length < index + 1) {
            this.plotTemplate.yAxis = [];
            this.plotTemplate.yAxis.push(this.createNewSeriesTemplate());
        }
    }

    checkYAxisDenominatorInitialized(index: number): void {
        if (this.plotTemplate.yAxis[0].denominator == null) {
            this.plotTemplate.yAxis[0].denominator = {
                arg1: {
                    arg: "",
                    multiplierType: 0,
                    type: "",
                    isConstValue: false
                },
                arg2: null,
                action: 0
            };
        }
    }

    createNewSeriesTemplate(): SeriesTemplate {
        return {
            axis: "",
            valueField: "",
            title: "",
            color: "",
            name: "",
            numerator: {
                arg1: {
                    arg: "",
                    multiplierType: 0,
                    type: "",
                    isConstValue: false
                },
                arg2: null,
                action: 0
            },
            denominator: null
        };
    }

    onChangeTemplateName(e): void {
        this.plotTemplate.name = e.target.value;
    }

    onChangeTemplateUseCycleData(e): void {
        this.plotTemplate.useCycleData = e.target.checked;
        if (this.plotTemplate.useCycleData) {
            this.checkXAxisInitialized();
            this.plotTemplate.xAxis.axis = "x";
            this.plotTemplate.xAxis.valueField = "x";
            this.plotTemplate.xAxis.name = "Cycle";
            this.plotTemplate.xAxis.title = "Cycle";
            this.plotTemplate.xAxis.numerator.arg1.arg = "CycleNumber";
            this.plotTemplate.xAxis.numerator.arg1.isConstValue = false;
            this.plotTemplate.xAxis.numerator.arg1.type = "System.Double";
            this.plotTemplate.xAxis.numerator.arg1.multiplierType = 0;
        }
    }

    onChangeTemplateUseFirstCycle(e): void {
        this.plotTemplate.useFirstCycle = e.target.checked;
    }

    onChangeTemplateChargeDischarge(e): void {
        this.plotTemplate.useAgregateData = false;
        this.plotTemplate.checkChargeDischarge = e.target.checked;
    }

    onChangeTemplateUseAgregateData(e): void {
        this.plotTemplate.useAgregateData = e.target.checked;
        this.plotTemplate.checkChargeDischarge = false;
    }

    onChangeArg1AllPointsNumeratorX(e): void {
        this.checkXAxisInitialized();
        this.plotTemplate.xAxis.axis = "x";
        this.plotTemplate.xAxis.valueField = "x";
        this.plotTemplate.xAxis.name = e.target.value;
        this.plotTemplate.xAxis.title = e.target.value;
        this.plotTemplate.xAxis.numerator.arg1.arg = e.target.value;
        this.plotTemplate.xAxis.numerator.arg1.isConstValue = e.target.value === "1";
        this.plotTemplate.xAxis.numerator.arg1.type = "System.Double";
        if (e.target.value === "Capacity") {
            this.plotTemplate.xAxis.numerator.arg1.multiplierType = 2;
        } else
            if (e.target.value === "Current") {
                this.plotTemplate.xAxis.numerator.arg1.multiplierType = 1;
            } else {
                this.plotTemplate.xAxis.numerator.arg1.multiplierType = 0;
            }
    }

    onChangeArg1CycleNumeratorX(e): void {
        this.checkXAxisInitialized();
        this.plotTemplate.xAxis.axis = "x";
        this.plotTemplate.xAxis.valueField = "x";
        this.plotTemplate.xAxis.name = "Cycle";
        this.plotTemplate.xAxis.title = "Cycle";
        this.plotTemplate.xAxis.numerator.arg1.arg = e.target.value;
        this.plotTemplate.xAxis.numerator.arg1.isConstValue = e.target.value === "1";
        this.plotTemplate.xAxis.numerator.arg1.type = "System.Double";
        if (e.target.value === "Capacity") {
            this.plotTemplate.xAxis.numerator.arg1.multiplierType = 2;
        } else
            if (e.target.value === "ChargeEndCurrent") {
                this.plotTemplate.xAxis.numerator.arg1.multiplierType = 1;
            } else {
                this.plotTemplate.xAxis.numerator.arg1.multiplierType = 0;
            }
    }

    onChangeActionNumeratorX(e): void {
        this.checkXAxisInitialized();
        this.plotTemplate.xAxis.numerator.action = e.target.value;
    }

    onChangeArg2NumeratorX(e): void {
        this.checkXAxisInitialized();
        if (e.target.value === "None") {
            this.plotTemplate.xAxis.numerator.arg2 = null;
        } else {
            this.plotTemplate.xAxis.numerator.arg2 = {
                arg: "",
                    multiplierType: 0,
                        type: "",
                            isConstValue: false
            };
            this.plotTemplate.xAxis.numerator.arg2.arg = e.target.value;
            this.plotTemplate.xAxis.numerator.arg2.isConstValue = false;
            this.plotTemplate.xAxis.numerator.arg2.type = "System.Double";
        }
    }

    onChangeArg1DenominatorX(e): void {
        this.checkXAxisInitialized();
        this.checkXAxisDenominatorInitialized();
        this.plotTemplate.xAxis.axis = "y";
        this.plotTemplate.xAxis.valueField = "y";
        this.plotTemplate.xAxis.name = e.target.value;
        this.plotTemplate.xAxis.title = e.target.value;
        if (e.target.value === "None") {
            this.plotTemplate.xAxis.denominator.arg1 = null;
        } else {
            this.plotTemplate.xAxis.denominator.arg1.arg = e.target.value;
            this.plotTemplate.xAxis.denominator.arg1.isConstValue = e.target.value === "100";
            this.plotTemplate.xAxis.denominator.arg1.type = "System.Double";
            if (e.target.value === "Capacity") {
                this.plotTemplate.xAxis.denominator.arg1.multiplierType = 2;
            } else
                if (e.target.value === "Current" || e.target.value === "ChargeEndCurrent") {
                    this.plotTemplate.xAxis.denominator.arg1.multiplierType = 1;
                } else {
                    this.plotTemplate.xAxis.denominator.arg1.multiplierType = 0;
                }
        }
    }

    onChangeActionDenominatorX(e): void {
        this.checkXAxisInitialized();
        this.checkXAxisDenominatorInitialized();
        this.plotTemplate.xAxis.denominator.action = e.target.value;
    }

    onChangeArg2DenominatorX(e): void {
        this.checkXAxisInitialized();
        this.checkXAxisDenominatorInitialized();
        if (e.target.value === "None") {
            this.plotTemplate.xAxis.denominator.arg2 = null;
        } else {
            this.plotTemplate.xAxis.denominator.arg2 = {
                arg: "",
                multiplierType: 0,
                type: "",
                isConstValue: false
            };
            this.plotTemplate.xAxis.denominator.arg2.arg = e.target.value;
            this.plotTemplate.xAxis.denominator.arg2.isConstValue = false;
            this.plotTemplate.xAxis.denominator.arg2.type = "System.Double";
        }
    }

    onChangeArg1NumeratorY(e): void {
        this.checkYAxisInitialized(0);
        this.plotTemplate.yAxis[0].axis = "y";
        this.plotTemplate.yAxis[0].valueField = "y";
        this.plotTemplate.yAxis[0].name = e.target.value;
        this.plotTemplate.yAxis[0].title = e.target.value;
        if (e.target.value === "None") {
            this.plotTemplate.yAxis[0].numerator.arg1 = null;
        } else {
            this.plotTemplate.yAxis[0].numerator.arg1.arg = e.target.value;
            this.plotTemplate.yAxis[0].numerator.arg1.isConstValue = e.target.value === "1";
            this.plotTemplate.yAxis[0].numerator.arg1.type = "System.Double";
            if (e.target.value === "Capacity") {
                this.plotTemplate.yAxis[0].numerator.arg1.multiplierType = 2;
            } else
                if (e.target.value === "Current" || e.target.value === "ChargeEndCurrent") {
                    this.plotTemplate.yAxis[0].numerator.arg1.multiplierType = 1;
                } else {
                    this.plotTemplate.yAxis[0].numerator.arg1.multiplierType = 0;
                }
        }
    }

    onChangeActionNumeratorY(e): void {
        this.checkYAxisInitialized(0);
        this.plotTemplate.yAxis[0].numerator.action = e.target.value;
    }

    onChangeArg2NumeratorY(e): void {
        this.checkYAxisInitialized(0);
        if (e.target.value === "None") {
            this.plotTemplate.yAxis[0].numerator.arg2 = null;
        } else {
            this.plotTemplate.yAxis[0].numerator.arg2 = {
                arg: "",
                    multiplierType: 0,
                        type: "",
                            isConstValue: false
            };
            this.plotTemplate.yAxis[0].numerator.arg2.arg = e.target.value;
            this.plotTemplate.yAxis[0].numerator.arg2.isConstValue = false;
            this.plotTemplate.yAxis[0].numerator.arg2.type = "System.Double";
        }
    }

    onChangeArg1DenominatorY(e): void {
        this.checkYAxisInitialized(0);
        this.checkYAxisDenominatorInitialized(0);
        if (e.target.value === "None") {
            this.plotTemplate.yAxis[0].denominator.arg1 = null;
        } else {
            this.plotTemplate.yAxis[0].denominator.arg1.arg = e.target.value;
            this.plotTemplate.yAxis[0].denominator.arg1.isConstValue = e.target.value === "100";
            this.plotTemplate.yAxis[0].denominator.arg1.type = "System.Double";
            if (e.target.value === "Capacity") {
                this.plotTemplate.yAxis[0].denominator.arg1.multiplierType = 2;
            } else
                if (e.target.value === "Current" || e.target.value === "ChargeEndCurrent") {
                    this.plotTemplate.yAxis[0].denominator.arg1.multiplierType = 1;
                } else {
                    this.plotTemplate.yAxis[0].denominator.arg1.multiplierType = 0;
                }
        }
    }

    onChangeActionDenominatorY(e): void {
        this.checkYAxisInitialized(0);
        this.checkYAxisDenominatorInitialized(0);
        this.plotTemplate.yAxis[0].denominator.action = e.target.value;
    }

    onChangeArg2DenominatorY(e): void {
        this.checkYAxisInitialized(0);
        this.checkYAxisDenominatorInitialized(0);
        if (e.target.value === "None") {
            this.plotTemplate.yAxis[0].denominator.arg2 = null;
        } else {
            this.plotTemplate.yAxis[0].denominator.arg2 = {
                arg: "",
                multiplierType: 0,
                type: "",
                isConstValue: false
            };
            this.plotTemplate.yAxis[0].denominator.arg2.arg = e.target.value;
            this.plotTemplate.yAxis[0].denominator.arg2.isConstValue = false;
            this.plotTemplate.yAxis[0].denominator.arg2.type = "System.Double";
        }
    }

    onCancel(): void {
        this.cancel.emit();
    }

    onSave() {
        this.save.emit(this.plotTemplate);
    }
}
