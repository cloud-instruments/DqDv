import { Action } from "@ngrx/store";

export const UNAUTHORIZED = "AUTH: UNAUTHORIZED";
export const LOGIN = "AUTH: LOGIN";
export const LOGIN_SUCCEEDED = "AUTH: LOGIN_SUCCEEDED";
export const LOGIN_FAILED = "AUTH: LOGIN_FAILED";
export const LOGOFF = "AUTH: LOGOFF";
export const LOGOFF_SUCCEEDED = "AUTH: LOGOFF_SUCCEEDED";
export const LOGOFF_FAILED = "AUTH: LOGOFF_FAILED";
export const SIGNUP = "AUTH: SIGNUP";
export const SIGNUP_SUCCEEDED = "AUTH: SIGNUP_SUCCEEDED";
export const SIGNUP_FAILED = "AUTH: SIGNUP_FAILED";
export const ACQUIRE_PASSWORD_RESET = "AUTH: ACQUIRE_PASSWORD_RESET";
export const ACQUIRE_PASSWORD_RESET_SUCCEEDED = "AUTH: ACQUIRE_PASSWORD_RESET_SUCCEEDED";
export const ACQUIRE_PASSWORD_RESET_FAILED = "AUTH: ACQUIRE_PASSWORD_RESET_FAILED";
export const RESET_PASSWORD = "AUTH: RESET_PASSWORD";
export const RESET_PASSWORD_SUCCEEDED = "AUTH: RESET_PASSWORD_SUCCEEDED";
export const RESET_PASSWORD_FAILED = "AUTH: RESET_PASSWORD_FAILED";

export class Unauthorized implements Action {
    readonly type = UNAUTHORIZED;
}

export class Login implements Action {
    readonly type = LOGIN;

    constructor(public username: string, public password: string, public rememberMe: boolean) {
    }
}

export class LoginSucceeded implements Action {
    readonly type = LOGIN_SUCCEEDED;

    constructor(public username: string) {
    }
}

export class LoginFailed implements Action {
    readonly type = LOGIN_FAILED;

    constructor(public error: string) {
    }
}

export class Logoff implements Action {
    readonly type = LOGOFF;
}

export class LogoffSucceeded implements Action {
    readonly type = LOGOFF_SUCCEEDED;

    constructor() {
    }
}

export class LogoffFailed implements Action {
    readonly type = LOGOFF_FAILED;

    constructor(public error: string) {
    }
}

export class Signup implements Action {
    readonly type = SIGNUP;

    constructor(public username: string, public email: string, public password: string) {
    }
}

export class SignupSucceeded implements Action {
    readonly type = SIGNUP_SUCCEEDED;

    constructor() {
    }
}

export class SignupFailed implements Action {
    readonly type = SIGNUP_FAILED;

    constructor(public error: string) {
    }
}

export class AcquirePasswordReset implements Action {
    readonly type = ACQUIRE_PASSWORD_RESET;

    constructor(public username: string) {
    }
}

export class AcquirePasswordResetSucceeded implements Action {
    readonly type = ACQUIRE_PASSWORD_RESET_SUCCEEDED;
}

export class AcquirePasswordResetFailed implements Action {
    readonly type = ACQUIRE_PASSWORD_RESET_FAILED;

    constructor(public error: string) {
    }
}

export class ResetPassword implements Action {
    readonly type = RESET_PASSWORD;

    constructor(public id: string, public code: string, public password: string) {
    }
}

export class ResetPasswordSucceeded implements Action {
    readonly type = RESET_PASSWORD_SUCCEEDED;
}

export class ResetPasswordFailed implements Action {
    readonly type = RESET_PASSWORD_FAILED;

    constructor(public error: string) {
    }
}

export type AuthAction
    = Unauthorized
    | Login
    | LoginSucceeded
    | LoginFailed
    | Logoff
    | LogoffSucceeded
    | LogoffFailed
    | Signup
    | SignupSucceeded
    | SignupFailed
    | AcquirePasswordReset
    | AcquirePasswordResetSucceeded
    | AcquirePasswordResetFailed
    | ResetPassword
    | ResetPasswordSucceeded
    | ResetPasswordFailed;
