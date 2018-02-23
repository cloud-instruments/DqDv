import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { ReactiveFormsModule } from "@angular/forms";
import { BrowserModule } from "@angular/platform-browser";
import { ActionReducer, StoreModule } from "@ngrx/store";
import { EffectsModule } from "@ngrx/effects";
import { localStorageSync } from "ngrx-store-localstorage";
import { storeLogger } from "ngrx-store-logger";
import { AngularMultiSelectModule } from "angular2-multiselect-dropdown/angular2-multiselect-dropdown";
import { TabModule } from "angular-tabs-component";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import {
    DxChartModule,
    DxDataGridModule,
    DxFileUploaderModule,
    DxNumberBoxModule,
    DxChartComponent,
    DxRangeSelectorModule,
    DxPopupModule,
    DxSelectBoxModule,
    DxTextAreaModule,
    DevExtremeModule
} from "devextreme-angular";

import "devextreme/data/odata/store";
import { ColorPickerModule } from "ngx-color-picker";
import { AppComponent } from "./app.component";
import { AppRouteModule } from "./app-routes.module";

import { AuthInterceptor } from "./auth/interceptor/auth-interceptor";
import { AuthGuard } from "./auth/guard/auth.guard";
import { LoginComponent } from "./auth/login/login.component";
import { SignupComponent } from "./auth/signup/signup.component";
import { SignupSuccessComponent } from "./auth/signup-success/signup-success.component";
import { ConfirmSuccessComponent } from "./auth/confirm-success/confirm-success.component";
import { ConfirmFailureComponent } from "./auth/confirm-failure/confirm-failure.component";
import { AcquirePasswordResetComponent } from "./auth/acquire-password-reset/acquire-password-reset.component";
import { PasswordResetAcquiredComponent } from "./auth/password-reset-acquired/password-reset-acquired.component";
import { ResetPasswordComponent } from "./auth/reset-password/reset-password.component";
import { ResetPasswordSuccessComponent } from "./auth/reset-password-success/reset-password-success.component";
import { AuthService } from "./auth/state/auth.service";
import { ProjectService } from "./project/state/project.service"

import { AggregationSettingsEditorComponent } from "./chart/aggregation-settings-editor/aggregation-settings-editor.component";
import { ChartComponent } from "./chart/chart/chart.component";
import { StateOfChargeEditorComponent } from "./chart/stateof-charge-editor/stateof-charge-editor.component";
import { ChartViewComponent } from "./chart/chart-view.component";
import { ChartService } from "./chart/state/chart.service";
import { FilterEditorComponent } from "./chart/filter-editor/filter-editor.component";
import { LegendComponent } from "./chart/legend/legend.component";
import { PlotTypeSelectorComponent } from "./chart/plot-type-selector/plot-type-selector.component";
import { UoMSettingsEditorComponent } from "./chart/uom-settings-editor/uom-settings-editor.component";
import { ChartSettingsEditorComponent } from "./chart/chart-settings-editor/chart-settings-editor.component";
import { SeriesEditorComponent } from "./chart/series-editor/series-editor.component";
import { PagerService } from "./chart/service/pager-service";
import { AxisRangeEditorComponent } from "./chart/axis-range-editor/axis-range-editor.component";
import { PlotEditorComponent } from "./chart/plot-editor/plot-editor.component";
import { CustomTemplateEditorComponent } from "./chart/custom-template-editor/custom-template-editor.component";
import { PlotTemplateEditorComponent } from "./chart/plot-template-editor/plot-template-editor.component";
import { ShareInstanceEditorComponent } from "./chart/share-instance-editor/share-instance-editor.component";

import { UserPreferencesService } from "./preferences/state/preferences.service";
import { PreferencesComponent } from "./preferences/preferences.component";

import { FeedbackComponent } from "./feedback/feedback.component";
import { FeedbackService } from "./feedback/state/feedback.service";

import { ProjectListComponent } from "./projects/project-list.component";
import { ViewListComponent } from "./views/view-list.component";

import { StitcherService } from "./stitcher/state/stitcher.service";
import { StitcherComponent } from "./stitcher/stitcher.component";

import { StatisticStitcherService } from "./statistic-project/state/statistic-stitcher.service";
import { StatisticProjectComponent } from "./statistic-project/statistic-project.component";

import { ViewService } from "./view/state/view.service";
import { ViewComponent } from "./view/view.component";

import { UploaderComponent } from "./upload/uploader.component";
import { UploadService } from "./upload/state/upload.service";
import { DropdownBoxComponent } from "./shared/dropdown-box/dropdown-box.component";
import { ConfirmPopupComponent } from "./shared/popup/confirm-popup.component";
import { PopupService } from "./shared/popup/popup.service";
import { environment } from "../environments/environment";

import { AppState } from "./state/app.state";
import { appEffects } from "./state/app.effects";
import { appReducers } from "./state/app.reducers";

//import { ModalModule } from "ngx-bootstrap/modal";

export function logger(reducer: ActionReducer<AppState>): any {
    return storeLogger()(reducer);
}

export function localStorageSyncReducer(reducer: ActionReducer<AppState>): ActionReducer<AppState> {
    return localStorageSync({keys: [{auth: ["username", "loggedIn"] }], rehydrate: true })(reducer);
}

export const metaReducers = environment.production ? [logger, localStorageSyncReducer] : [logger, localStorageSyncReducer];

@NgModule({
    entryComponents: [ConfirmPopupComponent],
    declarations: [
        AppComponent,
        LoginComponent,
        SignupComponent,
        SignupSuccessComponent,
        ConfirmSuccessComponent,
        ConfirmFailureComponent,
        AcquirePasswordResetComponent,
        PasswordResetAcquiredComponent,
        ResetPasswordComponent,
        ResetPasswordSuccessComponent,
        ShareInstanceEditorComponent,
        AxisRangeEditorComponent,
        ChartComponent,
        PlotEditorComponent,
        PlotTemplateEditorComponent,
        ChartViewComponent,
        FilterEditorComponent,
        SeriesEditorComponent,
        LegendComponent,
        PlotTypeSelectorComponent,
        StateOfChargeEditorComponent,
        AggregationSettingsEditorComponent,
        UoMSettingsEditorComponent,
        ChartSettingsEditorComponent,
        StitcherComponent,
        StatisticProjectComponent,
        ViewComponent,
        FeedbackComponent,
        ViewListComponent,
        ProjectListComponent,
        UploaderComponent,
        DropdownBoxComponent,
        ConfirmPopupComponent,
        CustomTemplateEditorComponent,
        PreferencesComponent
    ],
    imports: [
        FormsModule,
        BrowserModule,
        HttpClientModule,
        ReactiveFormsModule,
        AppRouteModule,
        DxChartModule,
        DxDataGridModule,
        DxFileUploaderModule,
        DxRangeSelectorModule,
        DxNumberBoxModule,
        AngularMultiSelectModule,
        TabModule,
        DxPopupModule,
        DxTextAreaModule,
        ColorPickerModule,
        DevExtremeModule,
        DxSelectBoxModule,
        StoreModule.forRoot(appReducers, { metaReducers }),
        EffectsModule.forRoot(appEffects),
        NgbModule.forRoot()
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        AuthGuard,
        AuthService,
        ChartService,
        FeedbackService,
        ViewService,
        StitcherService,
        StatisticStitcherService,
        UserPreferencesService,
        UploadService,
        PagerService,
        PopupService,
        ProjectService
    ],
    bootstrap: [AppComponent]
})
export class AppModule {
}
