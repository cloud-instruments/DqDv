import { Component, EventEmitter, Input, Output, OnInit, SimpleChanges, OnChanges, AfterViewInit } from "@angular/core";
import { BehaviorSubject } from "rxjs/BehaviorSubject";
import "rxjs/add/operator/skipWhile";
import { PlotTemplate, PlotParameters, SeriesTemplate } from "../model/chart-plot-settings";
import { ChartFilter, AggregationSettings, AggregationType } from "../model";
import { environment } from "../../../environments/environment";
import { PagerService } from "../service/pager-service";

@Component({
    selector: "app-custom-template-editor",
    templateUrl: "custom-template-editor.component.html",
    styleUrls: ["custom-template-editor.component.css"]
})

export class CustomTemplateEditorComponent implements AfterViewInit, OnInit, OnChanges {
    algorithms = [
        { key: AggregationType.Default, value: "Enabled" },
        { key: AggregationType.VisvalingamWhyatt, value: "Visvalingam-Whyatt" },
        { key: AggregationType.None, value: "None" }
    ];

    private templates$ = new BehaviorSubject<PlotTemplate[]>(null);

    @Input()
    value: ChartFilter;

    @Input()
    set plotTemplates(value: PlotTemplate[]) {
        this.templates$.next(value);
    };
    get plotTemplates(): PlotTemplate[] {
        return this.templates$.getValue();
    }

    @Output()
    cancel = new EventEmitter();

    @Output()
    save = new EventEmitter<PlotTemplate>();

    @Output()
    apply = new EventEmitter<PlotTemplate>();

    @Output()
    delete = new EventEmitter<PlotTemplate>();

    @Input()
    selectedAxisItems = [];

    @Input()
    dropdownListAxis = [];

    @Input()
    valueAxisItems = [];

    @Input()
    selectedTemplateItems = [];

    //@Input()
    dropdownListTemplates = [];

    //@Input()
    userPlotTemplates: PlotTemplate[];

    @Input()
    plotTemplate: PlotTemplate;

    @Input()
    plotParameters: PlotParameters;

    @Input()
    dropdownSettings = {};

    @Input()
    showActionButtons: boolean;

    @Input()
    showYAxisDropDown: boolean;

    @Input()
    templateEditPageVisible: boolean;

    argumentAxisItems = [];
    selectedArgumentItem: any;

    constructor(private pagerService: PagerService) {
    }

    ngOnChanges(changes: SimpleChanges): void {
        //if (changes["plotTemplates"]) {
        //    this.pagedItems = changes["plotTemplates"].currentValue.slice(0, 5);           
        //}
    }
    
    pager: any = {};

    //plotItems: PlotTemplate[];
    pagedItems: PlotTemplate[];

    ngOnInit() {
        //this.pagedItems = [];
        //this.userPlotTemplates = [];

        this.selectedArgumentItem = "None";
        this.templates$
            .skipWhile(data => !data)
            .subscribe(data => {
                this.userPlotTemplates = data;

                this.fillyTemplatesData("CycleNumber");
                this.setPage(1);
            });
    }

    ngAfterViewInit(): void {       
        this.showActionButtons = false;
        this.templateEditPageVisible = false;
        this.showYAxisDropDown = false;
        //this.userPlotTemplates = [];
        this.selectedAxisItems = [];
        this.selectedTemplateItems = [];
        this.dropdownListAxis = [];
        //for (const template of this.plotTemplates) {
        //    this.userPlotTemplates.push(template);
        //}

        this.argumentAxisItems = [
            { id: "CycleNumber", itemName: "Cycle Number", valueAxisType: "cycle" },
            { id: "CRate", itemName: "Charge C-Rate", valueAxisType: "c-rate" },
            { id: "DischargeCRate", itemName: "Discharge C-Rate", valueAxisType: "discharge-c-rate" },
            { id: "ChargeEndCurrent", itemName: "Charge Final Current", valueAxisType: "cycle" },
            { id: "DischargeEndCurrent", itemName: "Discharge Final Current", valueAxisType: "cycle" },
            { id: "MidVoltage", itemName: "Medium Voltage", valueAxisType: "cycle" },
            { id: "ChargeEndVoltage", itemName: "Charge Final Voltage", valueAxisType: "cycle" },
            { id: "DischargeEndVoltage", itemName: "Discharge Final Voltage", valueAxisType: "cycle" },
            { id: "Time", itemName: "Time", valueAxisType: "data" },
            { id: "Current", itemName: "Current", valueAxisType: "data" },
            { id: "Voltage", itemName: "Voltage", valueAxisType: "data" },
            { id: "Energy", itemName: "Energy", valueAxisType: "data" },
            { id: "Power", itemName: "Power", valueAxisType: "data" },
            { id: "Temperature", itemName: "Temperature", valueAxisType: "cycle" },
            { id: "Capacity", itemName: "Capacity", valueAxisType: "data" }
        ];

        this.valueAxisItems = [
            { id: "CycleNumber", itemName: "Cycle Number", type: "cycle", useCycleData: true },
            { id: "CoulombicEfficiency", itemName: "Coulombic Efficiency", type: "cycle" },
            { id: "EnergyEfficiency", itemName: "Energy Efficiency", type: "cycle" },
            { id: "AreaSpecificImpedance", itemName: "Charge Area-Specific Impedance (ASI)", type: "cycle", useCycleData: true, combinedId: "AreaSpecificImpedance:DischargeAreaSpecificImpedance" },
            { id: "DischargeAreaSpecificImpedance", itemName: "Discharge Area-Specific Impedance (ASI)", type: "cycle", useCycleData: true, combinedId: "AreaSpecificImpedance:DischargeAreaSpecificImpedance" },
            { id: "AreaSpecificImpedance:DischargeAreaSpecificImpedance", itemName: "Area-Specific Impedance (ASI)", type: "cycle", useCycleData: true },

            { id: "ChargeEndCurrent", itemName: "Charge Final Current", type: "cycle", useCycleData: true, combinedId: "ChargeEndCurrent:DischargeEndCurrent", multiplierType: 1 },
            { id: "DischargeEndCurrent", itemName: "Discharge Final Current", type: "cycle", useCycleData: true, combinedId: "ChargeEndCurrent:DischargeEndCurrent", multiplierType: 1 },
            { id: "ChargeEndCurrent:DischargeEndCurrent", itemName: "Final Current", type: "cycle", useCycleData: true, multiplierType: 1 },

            { id: "MidVoltage", itemName: "Medium Voltage", type: "cycle", useCycleData: true, multiplierType: 4 },

            { id: "ChargeEndVoltage", itemName: "Charge Final Voltage", type: "cycle", useCycleData: true, combinedId: "ChargeEndVoltage:DischargeEndVoltage", multiplierType: 4 },
            { id: "DischargeEndVoltage", itemName: "Discharge Final Voltage", type: "cycle", useCycleData: true, combinedId: "ChargeEndVoltage:DischargeEndVoltage", multiplierType: 4 },
            { id: "ChargeEndVoltage:DischargeEndVoltage", itemName: "Final Voltage", type: "cycle", useCycleData: true, multiplierType: 4 },

            //{ id: "ChargeCapacity", itemName: "Charge Capacity", type: "cycle", useCycleData: true, combinedId: "ChargeCapacity:DischargeCapacity" },
            //{ id: "DischargeCapacity", itemName: "Discharge Capacity", type: "cycle", useCycleData: true, combinedId: "ChargeCapacity:DischargeCapacity" },
            { id: "ChargeCapacity:DischargeCapacity", itemName: "Capacity", type: "cycle", useCycleData: true, multiplierType: 2 },

            //{ id: "ChargeCapacityRetention", itemName: "Charge Capacity Retention", type: "cycle", useCycleData: true, combinedId: "ChargeCapacityRetention:DischargeCapacityRetention" },
            //{ id: "DischargeCapacityRetention", itemName: "Discharge Capacity Retention", type: "cycle", useCycleData: true, combinedId: "ChargeCapacityRetention:DischargeCapacityRetention" },
            { id: "ChargeCapacityRetention:DischargeCapacityRetention", itemName: "Capacity Retention", type: "cycle", useCycleData: true },
            
            //{ id: "Power", itemName: "Charge Power", type: "cycle", useCycleData: true, combinedId: "Power:DischargePower" },
            //{ id: "DischargePower", itemName: "Discharge Power", type: "cycle", useCycleData: true, combinedId: "Power:DischargePower" },
            { id: "Power:DischargePower", itemName: "Power", type: "cycle", useCycleData: true },

            { id: "Temperature", itemName: "Temperature", type: "cycle", useCycleData: true },
            //{ id: "ChargeEnergy", itemName: "ChargeEnergy", type: "cycle", useCycleData: true },
            //{ id: "DischargeEnergy", itemName: "DischargeEnergy", type: "cycle", useCycleData: true },
            { id: "ChargeEnergy:DischargeEnergy", itemName: "Energy", type: "cycle", useCycleData: true },

            //{ id: "ResistanceOhms", itemName: "Resistance(Ohms)", type: "cycle", useCycleData: true },
            //{ id: "DischargeResistance", itemName: "DischargeResistance", type: "cycle", useCycleData: true },

            { id: "ResistanceOhms:DischargeResistance", itemName: "Resistance", type: "cycle", useCycleData: true },
            
            { id: "Time", itemName: "Time", type: "data", useCycleData: false },
            { id: "Current", itemName: "Current", type: "data", useCycleData: false, multiplierType: 1 },
            { id: "Voltage", itemName: "Voltage", type: "data", useCycleData: false, multiplierType: 4 },
            { id: "Capacity", itemName: "Capacity", type: "data", useCycleData: false, multiplierType: 2 },
            { id: "Energy", itemName: "Energy", type: "data", useCycleData: false },
            { id: "Power", itemName: "Power", type: "data", useCycleData: false },
            { id: "Temperature", itemName: "Temperature", type: "data", useCycleData: false },

            { id: "CRate", itemName: "C-Rate", type: "c-rate" },
            { id: "DischargeCapacity", itemName: "Discharge Capacity", type: "c-rate", useCycleData: true, checkChargeDischarge: true },
            { id: "ChargeCapacity", itemName: "Charge Capacity", type: "c-rate", useCycleData: true, checkChargeDischarge: true },
            { id: "DischargeCRate", itemName: "Discharge C-Rate", type: "discharge-c-rate" },
            { id: "DischargeCapacity", itemName: "Discharge Capacity", type: "discharge-c-rate", useCycleData: true, checkChargeDischarge: true },
            { id: "ChargeCapacity", itemName: "Charge Capacity", type: "discharge-c-rate", useCycleData: true, checkChargeDischarge: true }
        ];

        //this.dropdownListTemplates = [];
        //this.fillyTemplatesData("CycleNumber");

        this.dropdownSettings = {
            singleSelection: false,
            text: "Select yAxis",
            enableSearchFilter: true,
            classes: "myclass custom-class",
            limitSelection: 2,
        };

        if (this.plotParameters == null) {
            this.plotParameters = {
                maxCycles: environment.maxCycles,
                maxPointsPerSeries: environment.maxPointsPerSeries,
                fromCycle: null,
                legendShowen: false,
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
                xLineVisible: false,
                yLineVisible: true,
                fontFamilyName: "Verdana",
                fontSize: 20,
                simplification: 1,
                xAxisText: null,
                yAxisText: [],
                chartTitle: null,
                chartPalette: [],
                axisRange: {
                    xAxis: null,
                    yAxis: null,
                    y2Axis: null,
                },
                pointSize: 1};
        }
        //this.setPage(1);
    }

    setPage(page: number) {
        if (page < 1 || page > this.pager.totalPages) {
            return;
        }
        this.pager = this.pagerService.getPager(this.userPlotTemplates.length, page);
        this.pagedItems = this.userPlotTemplates.slice(this.pager.startIndex, this.pager.endIndex + 1);
    }
   
    onCancel(): void {
        this.cancel.emit();
    }

    onSave() {
        if (this.plotTemplate.name === "") {
            alert("Template name can not be empty");
            return;
        }
        if (this.plotTemplate.xAxis.valueField === "") {
            alert("Template xAxis should be specified");
            return;
        }
        if (this.selectedAxisItems.length === 0 && this.plotTemplate.yAxis.length === 0) {
            alert("Template yAxis values should be specified");
            return;
        }
        if (this.selectedAxisItems.length > 0) {
            const yValue = this.selectedAxisItems[0];
            this.plotTemplate.yAxis = [];
            this.plotTemplate.yAxis.push({
                axis: "y",
                color: "",
                denominator: null,
                name: yValue.id,
                numerator: {
                    action: 0,
                    arg1: {
                        arg: yValue.id,
                        isConstValue: false,
                        multiplierType: 0,
                        type: ""
                    },
                    arg2: null
                },
                title: this.selectedAxisItems[0].itemName,
                valueField: "y"
            });

            this.plotTemplate.yAxis[0].numerator.arg1.multiplierType = yValue.multiplierType || 0;

            if (this.selectedAxisItems.length === 2) {
                const zValue = this.selectedAxisItems[1];
                this.plotTemplate.yAxis.push({
                    axis: "z",
                    color: "",
                    denominator: null,
                    name: zValue.id,
                    numerator: {
                        action: 0,
                        arg1: {
                            arg: zValue.id,
                            isConstValue: false,
                            multiplierType: 0,
                            type: ""
                        },
                        arg2: null
                    },
                    title: this.selectedAxisItems[1].itemName,
                    valueField: "z"
                });

                this.plotTemplate.yAxis[1].numerator.arg1.multiplierType = zValue.multiplierType || 0;
            }            
        }

        if (this.plotTemplate.useFirstCycle || (this.plotParameters.toCycle - this.plotParameters.fromCycle) < 2) {
            this.plotParameters.pointSize = 1;
        }
        this.plotTemplate.plotParameters = Object.assign({}, this.plotParameters);
        const valueAxisType = this.argumentAxisItems.find(t => t.id === this.selectedArgumentItem).valueAxisType;

        this.plotTemplate.useCycleData = valueAxisType === "cycle" || valueAxisType === "c-rate" || valueAxisType === "discharge-c-rate";
        this.plotTemplate.useCRateCalculation = valueAxisType === "c-rate";
        this.plotTemplate.useDischargeCRateCalculation = valueAxisType === "discharge-c-rate";

        //todo: not clear why we need checkChargeDischarge
        if (this.plotTemplate.useCycleData) {

            if (this.plotTemplate.xAxis.name.indexOf("ChargeCapacity") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("DischargeCapacity") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("ChargeCapacityRetention") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("CoulombicEfficiency") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("EnergyEfficiency") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("Area Specific Impedance") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("Discharge Area Specific Impedance") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("DischargeCapacityRetention") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("ChargeEnergy") >= 0 ||
                this.plotTemplate.xAxis.name.indexOf("DischargeEnergy") >= 0) {
                this.plotTemplate.checkChargeDischarge = true;
            }

            for (const templateSeriesItem of this.plotTemplate.yAxis) {
                if (templateSeriesItem.name.indexOf("ChargeCapacity") >= 0 ||
                    templateSeriesItem.name.indexOf("DischargeCapacity") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("CoulombicEfficiency") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("EnergyEfficiency") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("Area Specific Impedance") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("Discharge Area Specific Impedance") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("ChargeCapacityRetention") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("DischargeCapacityRetention") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("ChargeEnergy") >= 0 ||
                    this.plotTemplate.xAxis.name.indexOf("DischargeEnergy") >= 0) {
                    this.plotTemplate.checkChargeDischarge = true;
                }
            }
        }       

        this.save.emit(this.plotTemplate);
    }

    onApplyTemplate(template) {      
        this.apply.emit(template);
    }

    onDeleteTemplate(template) {
        this.delete.emit(template);
    }

    fillyTemplatesData(xName): void {
        this.dropdownListTemplates = [];
        for (const template of this.userPlotTemplates) {
            if (template.xAxis.numerator.arg1.arg === xName) {
                this.dropdownListTemplates.push({
                    id: template.xAxis.name,
                    itemName: template.xAxis.title
                });
            }
        }
    }

    checkPlotTeplateExists() {
        if (this.plotTemplate == null) {
            this.plotTemplate = {
                plotParameters: this.plotParameters,
                checkChargeDischarge: false,
                name: "",
                useCRateCalculation: false,
                useDischargeCRateCalculation: false,
                useAgregateData: true,
                useCycleData: false,
                useFirstCycle: false,
                canEdit:true,
                xAxis: {
                    axis: "x",
                    color: "",
                    denominator: null,
                    name: "",
                    numerator: null,
                    title: "",
                    valueField: "x"
                },
                yAxis: [],
                id: "",
                userId: ""
            };
        }
    }

    onChangeAlgorithm(e): void {
        this.checkPlotTeplateExists();
        this.plotTemplate.plotParameters.simplification = e.target.value;
    }  

    onChangeAgregateData(e): void {
        this.checkPlotTeplateExists();
        this.plotTemplate.useAgregateData = e.target.checked;
    }  

    onChangeDisableCharge(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.disableCharge = e.target.checked;
    }

    onChangeDisableDischarge(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.disableDischarge = e.target.checked;
    }

    onChangeLegendVisible(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.legendShowen = e.target.checked;
    }

    onChangeXLineVisible(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.xLineVisible = e.target.checked;
    }

    onChangeYLineVisible(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.yLineVisible = e.target.checked;
    }

    onChangeMinY(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.minY = e.element[0].children[0].value;
    }

    onChangeMaxY(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.maxY = e.element[0].children[0].value;
    }

    onChangeThreshold(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.threshold = e.element[0].children[0].value;
    }

    onChangeCustomFilter(e): void {

        this.checkPlotTeplateExists();
        this.plotParameters.customCycleFilter = e.srcElement.value;
    }

    onChangeFromCycle(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.fromCycle = e.element[0].children[0].value;
    }

    onChangeToCycle(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.toCycle = e.element[0].children[0].value;
    }

    onChangeNthCycle(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.everyNthCycle = e.element[0].children[0].value;
    }

    onChangeFontSize(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.fontSize = e.element[0].children[0].value;
    }

    onChangePointSize(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.pointSize = e.element[0].children[0].value;
    }

    onChangeTemplateName(e): void {
        this.checkPlotTeplateExists();
        this.plotTemplate.name = e.target.value;
    }

    onChangeFontName(e): void {
        this.checkPlotTeplateExists();
        this.plotParameters.fontFamilyName = e.target.value;       
    }

    onChangeSimpleAxis(e): void {
        this.checkPlotTeplateExists();
        this.selectedAxisItems = [];
        if (this.selectedArgumentItem === "None") {
            this.showYAxisDropDown = false;
            this.plotTemplate.xAxis = {
                axis: "x",
                color: "",
                denominator: null,
                name: "",
                numerator: null,
                title: "",
                valueField: "x"
            };
                this.plotTemplate.yAxis = [];
        } else {
            this.showYAxisDropDown = true;
            this.plotTemplate.xAxis.title = this.selectedArgumentItem;
            this.plotTemplate.xAxis.name = this.selectedArgumentItem;
            this.plotTemplate.xAxis.numerator = {
                action: 0,                
                arg1: {
                    arg: this.selectedArgumentItem,
                    isConstValue: false,
                    multiplierType: 0,
                    type:""
                },
                arg2:null
            }
            if (this.selectedArgumentItem === "Capacity") {
                this.plotTemplate.xAxis.numerator.arg1.multiplierType = 2;
            } else
                if (this.selectedArgumentItem === "Current" || this.selectedArgumentItem === "ChargeEndCurrent") {
                    this.plotTemplate.xAxis.numerator.arg1.multiplierType = 1;
                } else {
                    this.plotTemplate.xAxis.numerator.arg1.multiplierType = 0;
                }

            this.fillValueAxisItems();
        }
    }

    onChangeXAxis(e): void {
        this.fillyTemplatesData(e.target.value);
    }

    onTabChange(e): void {
        if (e.tabTitle === "Templates") {
            this.showActionButtons = false;         
        }
        else {
            this.selectedArgumentItem = "None";
            this.showActionButtons = true;    
            this.plotParameters = {
                maxCycles: environment.maxCycles,
                maxPointsPerSeries: environment.maxPointsPerSeries,
                fromCycle: null,
                legendShowen: false,
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
                xLineVisible: false,
                yLineVisible: true,
                fontFamilyName: "Verdana",
                fontSize: 20,
                simplification: 1,
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
            };
            this.plotTemplate = {
                plotParameters: this.plotParameters,
                checkChargeDischarge: false,
                name: "",
                useCRateCalculation: false,
                useDischargeCRateCalculation: false,
                useAgregateData: true,
                useCycleData: false,
                useFirstCycle: false,
                canEdit: true,
                xAxis: {
                    axis: "x",
                    color: "",
                    denominator: null,
                    name: "",
                    numerator: null,
                    title: "",
                    valueField: "x"
                },
                yAxis: [],
                id: "",
                userId: ""
            };
            this.selectedAxisItems = [];
            this.showYAxisDropDown = false;
        }
    }

    onSubTabFormulaChange(e): void {
      
    }

    onSubTabSimpleChange(e): void {
      
    }

    onEditTemplate(template, tabsComponent): void {
        this.showActionButtons = true;
        this.showYAxisDropDown = true;
        const argumentAxis = this.argumentAxisItems.find(item => item.id === template.xAxis.name);
        this.selectedArgumentItem = argumentAxis && argumentAxis.id || "None";
        this.fillValueAxisItems();
        this.selectedAxisItems = [];
        for (const axis of template.yAxis) {         
                this.selectedAxisItems.push({
                    id: axis.name,
                    itemName: this.valueAxisItems.find(t => t.id === axis.name).itemName
                });           
        }
        tabsComponent.tabs[0].active = false;
        tabsComponent.tabs[1].active = true;
        this.plotTemplate = template;
        this.plotParameters = template.plotParameters;
    }

    fillValueAxisItems(): void {
        this.dropdownListAxis = [];
        const temptype = this.argumentAxisItems.find(t => t.id === this.selectedArgumentItem).valueAxisType;
        const valueAxisPair = this.valueAxisItems.find(item => item.id === this.selectedArgumentItem);
        const combinedId = valueAxisPair && valueAxisPair.combinedId;

        // select all all possible values for value-axis based on argument-asxis type
        for (const template of this.valueAxisItems.filter(item => item.type === temptype)) {
            // do not allow display the same data on value & argument axis 
            // do not display combined properties if one of selected as argument (display splitted Charge/Discharge)
            if (template.id === this.selectedArgumentItem || template.id === combinedId)
                continue;
            // do not display splitted Charge/Discharge properties if combined may used instead
            if (template.combinedId && (!combinedId || template.combinedId !== combinedId))
                continue;

            this.dropdownListAxis.push({
                id: template.id,
                itemName: template.itemName
            });
        }    
    }

    onItemSelect(item: any) {     
    }

    OnItemDeSelect(item: any) {     
    }

    onSelectAll(items: any) {
        console.log(items);
    }

    onDeSelectAll(items: any) {
        console.log(items);
    }
  }
