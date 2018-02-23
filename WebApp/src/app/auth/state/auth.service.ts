import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/map";

import { environment } from "environments/environment";

export interface AuthResponse {
    success: boolean;
    message: string;
    username: string;
}

@Injectable()
export class AuthService {
    constructor(private http: HttpClient) {
    }

    login(username: string, password: string, rememberMe: boolean): Observable<AuthResponse> {
        const url = environment.serverBaseUrl + "api/auth/login";
        const body = {
            username,
            password,
            rememberMe
        };

        return this.http.post<AuthResponse>(url, body, { withCredentials: true })
            .do(() => {},
                error => {
                    console.error(`Login failed. Error: `);
                    console.error(error);
                });
    }

    logoff(): Observable<Blob> {
        const url = environment.serverBaseUrl + "api/auth/logoff";

        return this.http.post(url, null, { withCredentials: true, responseType: "blob" })
            .do(() => { },
            error => {
                console.error(`Logoff failed. Error: `);
                console.error(error);
            });
    }

    signup(username: string, email: string, password: string): Observable<AuthResponse> {
        const url = environment.serverBaseUrl + "api/auth/signup";
        const body = {
            username,
            email,
            password
        };

        return this.http.post<AuthResponse>(url, body, { withCredentials: true })
            .do(() => { },
            error => {
                console.error(`Sign up failed. Error: `);
                console.error(error);
            });
    }

    acquirePasswordReset(username: string): Observable<AuthResponse> {
        const url = environment.serverBaseUrl + "api/auth/acquire-password-reset";
        const body = {
            username
        };

        return this.http.post<AuthResponse>(url, body, { withCredentials: true })
            .do(() => { },
            error => {
                console.error(`Acquire password reset failed. Error: `);
                console.error(error);
            });
    }

    resetPassword(id: string, code: string, password: string): Observable<AuthResponse> {
        const url = environment.serverBaseUrl + "api/auth/reset-password";
        const body = {
            id,
            code,
            password
        };

        return this.http.post<AuthResponse>(url, body, { withCredentials: true })
            .do(() => { },
            error => {
                console.error(`Reset password failed. Error: `);
                console.error(error);
            });
    }
}
