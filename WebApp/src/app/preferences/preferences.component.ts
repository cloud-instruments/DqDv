import { Component, OnInit } from "@angular/core";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/skipWhile";
import { FormBuilder, FormGroup, FormArray } from "@angular/forms";
import { AppState } from "../state";
import { ViewUserPreferences, SetUserPreferencesSettings } from "./state/preferences.actions"
import { UserPreferencesState } from "./state/preferences.state"
import { UserPreferences, ChartPaletteColor } from "./model/preferences.model" 

@Component({
    selector: "app-preferences",
    templateUrl: "./preferences.component.html",
    styleUrls: ["./preferences.component.css"]
})
export class PreferencesComponent implements OnInit {
    private seriesPaletteColors: any[];
    private chartFontFamilies: string[] = [
        "Helvetica Neue", "Helvetica", "Arial", "Calibri", "Tahoma", "Times New Roman", "Georgia", "Segoe", "Verdana"
    ];
    private isPreferencesLoaded: boolean;

    preferencesForm: FormGroup;
    state$: Observable<UserPreferencesState>;

    constructor(private store: Store<AppState>, private formBuilder: FormBuilder) {
        this.createForm();
        this.state$ = this.store.select(s => s.userPreferences);
    }

    ngOnInit(): void {
        this.isPreferencesLoaded = false;
        this.store.dispatch(new ViewUserPreferences());
        this.store.select(s => s.userPreferences.preferences)
            .skipWhile(p => !p)
            .subscribe(p => {
                this.isPreferencesLoaded = true;
                this.setChartPaletteColor(p.chartPreferences.paletteColors);
                this.preferencesForm.setValue({
                    chartPreferences: p.chartPreferences
                });
            });
    }

    get IsReady() {
        return this.isPreferencesLoaded;
    }

    get paletteColors(): FormArray {
        return this.preferencesForm.get("chartPreferences.paletteColors") as FormArray;
    }

    getPaletteColorGroup(fromIndex, toIndex): any[] {
        return this.paletteColors.controls.slice(fromIndex, toIndex);
    }

    save(): void {
        const userPreferences: UserPreferences = {
            chartPreferences: this.preferencesForm.value.chartPreferences
        }

        this.store.dispatch(new SetUserPreferencesSettings(userPreferences));
    }

    private setChartPaletteColor(colors: ChartPaletteColor[]) {
        const colorFGs = colors.map(color => this.formBuilder.control(color));
        const colorFormArray = this.formBuilder.array(colorFGs);
        (this.preferencesForm.get("chartPreferences") as FormGroup).setControl("paletteColors", colorFormArray);
    }

    private createForm(): void {
        this.preferencesForm = this.formBuilder.group({
            chartPreferences: this.formBuilder.group({
                pointSize: 11,
                xLineVisible: true,
                yLineVisible: true,
                showLegend: false,
                fontFamilyName: null,
                fontSize: null,
                paletteColors: this.formBuilder.array([]), 
            })
        });
    }
}