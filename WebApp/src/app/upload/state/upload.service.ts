import { Injectable } from "@angular/core";
import { HttpClient, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs/Observable";

import { environment } from "environments/environment";
import { Project } from "../model/project";

@Injectable()
export class UploadService {
    constructor(private http: HttpClient) {
    }

    uniqueProjectsCheck(project: Project): Observable<any> {
        const items: Array<{ name: string, fileName: string, fileSize: number}> = [];

        project.file.forEach(item => {
            items.push({ "name": project.name, "fileName": item.name, "fileSize": item.size });
        });

        return this.http.post(
            environment.serverBaseUrl + "api/projects/unique",
            items,
            { withCredentials: true, responseType: "json" });
    }

    upload(project: Project): Observable<any> {
        const form = new FormData();

        project.file.forEach(item => {
            this.appendFormField(form, "file", item);
        });
        this.appendFormField(form, "name", project.name);
        this.appendFormField(form, "testName", project.testName);
        this.appendFormField(form, "testType", project.testType);
        this.appendFormField(form, "channel", project.channel);
        this.appendFormField(form, "tag", project.tag);
        this.appendFormField(form, "mass", project.mass);
        this.appendFormField(form, "theoreticalCapacity", project.theoreticalCapacity);
        this.appendFormField(form, "activeMaterialFraction", project.activeMaterialFraction);
        this.appendFormField(form, "area", project.area);
        this.appendFormField(form, "comments", project.comments);
        this.appendFormField(form, "overwriteExisting", true);
        
        const request = new HttpRequest("POST",
            environment.serverBaseUrl + "api/projects",
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
