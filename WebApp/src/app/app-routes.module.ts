import { NgModule } from "@angular/core";
import { Routes, RouterModule } from "@angular/router";

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
import { FeedbackComponent } from "./feedback/feedback.component";
import { ProjectListComponent } from "./projects/project-list.component";
import { ViewListComponent } from "./views/view-list.component";
import { UploaderComponent } from "./upload/uploader.component";
import { PreferencesComponent } from "./preferences/preferences.component";

const appRoutes: Routes = [
    {
        path: "login",
        component: LoginComponent
    },
    {
        path: "signup",
        component: SignupComponent
    },
    {
        path: "signup-success",
        component: SignupSuccessComponent
    },
    {
        path: "confirm-success",
        component: ConfirmSuccessComponent
    },
    {
        path: "confirm-failure",
        component: ConfirmFailureComponent
    },
    {
        path: "acquire-password-reset",
        component: AcquirePasswordResetComponent
    },
    {
        path: "password-reset-acquired",
        component: PasswordResetAcquiredComponent
    },
    {
        path: "reset-password",
        component: ResetPasswordComponent
    },
    {
        path: "reset-password-success",
        component: ResetPasswordSuccessComponent
    },
    {
        path: "feedback",
        component: FeedbackComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "projects",
        component: ProjectListComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "views",
        component: ViewListComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "upload",
        component: UploaderComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "preferences",
        component: PreferencesComponent,
        canActivate: [AuthGuard]
    },
    {
        path: "**",
        redirectTo: "/projects"
    }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRouteModule {
}
