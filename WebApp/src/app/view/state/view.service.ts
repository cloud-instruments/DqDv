import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/Observable";

import { environment } from "environments/environment";
import { ViewProjectsParams, View } from "../model/view-projects-params";

@Injectable()
export class ViewService {
    constructor(private http: HttpClient) {
    }

    add(params: ViewProjectsParams): Observable<View[]> {
        const url = `${environment.serverBaseUrl}api/views`;
        return this.http.post<View[]>(url, { params: params }, { withCredentials: true });
    }

    getViews(): Observable<View[]> {
        const url = `${environment.serverBaseUrl}api/views`;
        return this.http.get<View[]>(url, { withCredentials: true });
    }

    deleteView(viewIdValue: number): Observable<any> {
        const url = `${environment.serverBaseUrl}api/views/${viewIdValue}`;
        return this.http.delete(url, { withCredentials: true });
    }
}
