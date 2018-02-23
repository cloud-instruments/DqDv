import { UserPreferences } from "../model/preferences.model";

export interface UserPreferencesState {
    preferences: UserPreferences;
    error: string;
}