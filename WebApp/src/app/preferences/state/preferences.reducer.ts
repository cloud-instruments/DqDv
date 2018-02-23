import * as forRoot from "./preferences.actions";
import { UserPreferencesState } from "./preferences.state";

const initialState: UserPreferencesState = {
    preferences: null,
    error: null
}

export function userPreferencesReducer(state: UserPreferencesState = initialState, action: forRoot.UserPreferencesAction) : UserPreferencesState {
    switch (action.type) {
        case forRoot.VIEW_USER_PREFERENCES_SUCCEEDED:
            return {
                ...state,
                preferences: action.userPreferences,
                error: null
            };
        case forRoot.SET_USER_PREFERENCES_SETTINGS: 
            return {
                ...state,
                preferences: action.userPreferences,
                error: null
            }
        case forRoot.USER_PREFERENCES_FAILED:
            return {
                ...state,
                error: action.error
            };
        default:
            return state;
    }
}