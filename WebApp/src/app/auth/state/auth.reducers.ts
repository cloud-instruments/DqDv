import * as forRoot from "./auth.actions";
import { AuthState } from "./auth.state";

const initialState: AuthState = {
    username: null,
    inProgress: false,
    loggedIn: false,
    loginError: null,
    signupError: null,
    acquirePasswordResetError: null,
    resetPasswordError: null
};

export function authReducer(state: AuthState = initialState, action: forRoot.AuthAction): AuthState {
    switch (action.type) {
        case forRoot.UNAUTHORIZED:
            return {
                ...state,
                username: null,
                loggedIn: false,
                loginError: null
            };

        case forRoot.LOGIN:
            return {
                ...state,
                username: null,
                loggedIn: false,
                inProgress: true,
                loginError: null
            };

        case forRoot.LOGIN_SUCCEEDED:
            return {
                ...state,
                username: action.username,
                loggedIn: true,
                inProgress: false,
                loginError: null
            };

        case forRoot.LOGIN_FAILED:
            return {
                ...state,
                username: null,
                loggedIn: false,
                inProgress: false,
                loginError: action.error
            };

        case forRoot.LOGOFF:
            return {
                ...state,
                username: null,
                loggedIn: false,
                loginError: null,
                inProgress: true
            };

        case forRoot.LOGOFF_SUCCEEDED:
            return {
                ...state,
                inProgress: false
            };

        case forRoot.LOGOFF_FAILED:
            return {
                ...state,
                inProgress: false
            };

        case forRoot.SIGNUP:
            return {
                ...state,
                username: null,
                loggedIn: false,
                inProgress: true,
                signupError: null
            };

        case forRoot.SIGNUP_SUCCEEDED:
            return {
                ...state,
                inProgress: false
            };

        case forRoot.SIGNUP_FAILED:
            return {
                ...state,
                inProgress: false,
                signupError: action.error
            };

        case forRoot.ACQUIRE_PASSWORD_RESET:
            return {
                ...state,
                inProgress: true,
                acquirePasswordResetError: null
            };

        case forRoot.ACQUIRE_PASSWORD_RESET_SUCCEEDED:
            return {
                ...state,
                inProgress: false
            };

        case forRoot.ACQUIRE_PASSWORD_RESET_FAILED:
            return {
                ...state,
                inProgress: false,
                acquirePasswordResetError: action.error
            };

        case forRoot.RESET_PASSWORD:
            return {
                ...state,
                inProgress: true,
                resetPasswordError: null
            };

        case forRoot.RESET_PASSWORD_SUCCEEDED:
            return {
                ...state,
                inProgress: false
            };

        case forRoot.RESET_PASSWORD_FAILED:
            return {
                ...state,
                inProgress: false,
                resetPasswordError: action.error
            };

        default:
            return state;
    }
}
