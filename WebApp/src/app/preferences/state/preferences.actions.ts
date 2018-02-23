import { Action } from "@ngrx/store";
import { UserPreferences } from "../model/preferences.model";

export const VIEW_USER_PREFERENCES = "USER_PREFERENCES: VIEW_USER_PREFERENCES";
export const VIEW_USER_PREFERENCES_SUCCEEDED = "USER_PREFERENCES: VIEW_USER_PREFERENCES_SUCCEEDED";
export const SET_USER_PREFERENCES_SETTINGS = "USER_PREFERENCES: SET_USER_PREFERENCES_SETTINGS";
export const USER_PREFERENCES_FAILED = "USER_PREFERENCES: USER_PREFERENCES_FAILED";

export class ViewUserPreferences implements Action {
    readonly type = VIEW_USER_PREFERENCES;

    constructor() {
    }
}

export class ViewUserPreferencesSucceeded implements Action {
    readonly type = VIEW_USER_PREFERENCES_SUCCEEDED;
    constructor(public userPreferences: UserPreferences) {
    }
}

export class SetUserPreferencesSettings implements Action {
    readonly type = SET_USER_PREFERENCES_SETTINGS;

    constructor(public userPreferences: UserPreferences) {
    }
}

export class UserPreferencesSettingsFailed implements Action {
    readonly type = USER_PREFERENCES_FAILED;

    constructor(public error: string) {
    }
}

export type UserPreferencesAction
    = ViewUserPreferences
    | ViewUserPreferencesSucceeded
    | SetUserPreferencesSettings
    | UserPreferencesSettingsFailed