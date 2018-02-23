import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { environment } from "environments/environment";
import { UserPreferences } from "../model/preferences.model";

@Injectable()
export class UserPreferencesService {
    constructor(private http: HttpClient) {
    }

    getPreferences(): Observable<UserPreferences> {
        const url = environment.serverBaseUrl + "api/preferences";
        return this.http.get<UserPreferences>(url, { withCredentials: true });
    }

    savePrefernces(preferences: UserPreferences): Observable<any>  {
        const url = environment.serverBaseUrl + "api/preferences";
        return this.http.post<UserPreferences>(url, preferences, { withCredentials: true });
    }
}