import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Actions, Effect } from "@ngrx/effects";
import { Observable } from "rxjs/Observable";
import "rxjs/add/observable/empty";
import "rxjs/add/observable/of";
import "rxjs/add/operator/map";
import "rxjs/add/operator/switchMap";

import * as forRoot from "./auth.actions";
import { AuthService } from "./auth.service";

@Injectable()
export class AuthEffects {
    @Effect()
    onLogin = this.actions$
        .ofType(forRoot.LOGIN)
        .switchMap((action: forRoot.Login) =>
            this.authService
                .login(action.username, action.password, action.rememberMe)
                .map(response => response.success
                    ? new forRoot.LoginSucceeded(response.username)
                    : new forRoot.LoginFailed(response.message || "Invalid username and/or password"))
                .catch(error => Observable.of(new forRoot.LoginFailed("Network error")))
        );

    @Effect()
    onSignup = this.actions$
        .ofType(forRoot.SIGNUP)
        .switchMap((action: forRoot.Signup) =>
            this.authService
                .signup(action.username, action.email, action.password)
                .map(response => response.success
                    ? new forRoot.SignupSucceeded()
                    : new forRoot.SignupFailed(response.message || "Unknown error"))
                .catch(error => Observable.of(new forRoot.SignupFailed("Network error")))
    );

    @Effect({ dispatch: false })
    onLoginSucceeded = this.actions$
        .ofType(forRoot.LOGIN_SUCCEEDED)
        .do(() => this.router.navigate(["/projects"]));

    @Effect({ dispatch: false })
    onSignupSucceeded = this.actions$
        .ofType(forRoot.SIGNUP_SUCCEEDED)
        .do(() => this.router.navigate(["/signup-success"]));

    @Effect()
    onLogoff = this.actions$
        .ofType(forRoot.LOGOFF)
        .switchMap(() => this.authService.logoff()
            .map(() => new forRoot.LogoffSucceeded())
            .catch(error => Observable.of(new forRoot.LogoffFailed(error))));

    @Effect({ dispatch: false })
    onUnauthorized = this.actions$
        .ofType(forRoot.UNAUTHORIZED, forRoot.LOGOFF_SUCCEEDED, forRoot.LOGOFF_FAILED)
        .do(() => this.router.navigate(["/login"]));

    @Effect()
    onAcquirePasswordReset = this.actions$
        .ofType(forRoot.ACQUIRE_PASSWORD_RESET)
        .switchMap((action: forRoot.AcquirePasswordReset) =>
            this.authService
                .acquirePasswordReset(action.username)
                .map(response => response.success
                    ? new forRoot.AcquirePasswordResetSucceeded()
                    : new forRoot.AcquirePasswordResetFailed(response.message || "Unknown error"))
                .catch(error => Observable.of(new forRoot.AcquirePasswordResetFailed("Network error")))
    );

    @Effect({ dispatch: false })
    onAcquirePasswordResetSucceeded = this.actions$
        .ofType(forRoot.ACQUIRE_PASSWORD_RESET_SUCCEEDED)
        .do(() => this.router.navigate(["/password-reset-acquired"]));

    @Effect()
    onResetPassword = this.actions$
        .ofType(forRoot.RESET_PASSWORD)
        .switchMap((action: forRoot.ResetPassword) =>
            this.authService
                .resetPassword(action.id, action.code, action.password)
                .map(response => response.success
                    ? new forRoot.ResetPasswordSucceeded()
                    : new forRoot.ResetPasswordFailed(response.message || "Unknown error"))
                .catch(error => Observable.of(new forRoot.ResetPasswordFailed("Network error")))
        );

    @Effect({ dispatch: false })
    onResetPasswordSucceeded = this.actions$
        .ofType(forRoot.RESET_PASSWORD_SUCCEEDED)
        .do(() => this.router.navigate(["/reset-password-success"]));

    constructor(private actions$: Actions, private authService: AuthService, private router: Router) {
    }
}
