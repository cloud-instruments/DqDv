import { Injectable } from "@angular/core";
import { Actions, Effect } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { UserPreferencesService } from "./preferences.service"
import { AppState } from "../../state";
import * as forRoot from "./preferences.actions";

@Injectable()
export class UserPreferencesEffects {
    constructor(private store: Store<AppState>, private actions$: Actions, private userPreferencesService: UserPreferencesService) {
    }

    @Effect()
    onGetUserPreferences = this.actions$
        .ofType(forRoot.VIEW_USER_PREFERENCES)
        .switchMap((action: forRoot.ViewUserPreferences) =>
            this.userPreferencesService.getPreferences()
                .map(preferences => new forRoot.ViewUserPreferencesSucceeded(preferences))
                .catch(error => Observable.of(new forRoot.UserPreferencesSettingsFailed("Unable to read user preferences. Please, try again later."))));

    @Effect()
    onSetUserPreferences = this.actions$
        .ofType(forRoot.SET_USER_PREFERENCES_SETTINGS)
        .withLatestFrom(this.store.select(s => s.userPreferences), (_, state) => state)
        .switchMap((state) => 
            this.userPreferencesService.savePrefernces(state.preferences)
                .map(() => new forRoot.ViewUserPreferencesSucceeded(state.preferences))
                .catch(error => Observable.of(new forRoot.UserPreferencesSettingsFailed("Unable to save user preferences. Please, try again later."))));
}