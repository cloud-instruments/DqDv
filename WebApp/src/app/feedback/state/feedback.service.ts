import { HttpClient, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";

import { environment } from "environments/environment";

@Injectable()
export class FeedbackService {
    constructor(private http: HttpClient) {
    }

    sendFeedback(comment: string, file: File): Observable<any> {
        const form = new FormData();
        this.appendFormField(form, "comment", comment);
        this.appendFormField(form, "file", file);

        const request = new HttpRequest("POST",
            environment.serverBaseUrl + "api/feedback",
            form,
            {
                reportProgress: true,
                responseType: "text",
                withCredentials: true
            });

        return this.http.request(request);
    }

    private appendFormField(form: FormData, name: string, value: any): void {
        if (value !== undefined && value !== null) {
            form.append(name, value);
        }
    }
}
