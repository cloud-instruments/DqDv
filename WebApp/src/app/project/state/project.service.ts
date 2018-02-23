import { Injectable } from "@angular/core";
import { HttpClient, HttpResponse } from "@angular/common/http";
import { Observable } from "rxjs/Observable";
import { environment } from "environments/environment";
import { FileBlob } from "../../shared/model/file.blob";

@Injectable()
export class ProjectService {

    constructor(private http: HttpClient) {
    }

    download(project): Observable<FileBlob> {
        const url = `${environment.serverBaseUrl}api/projects/${project.id}/download`;
        return this.http.post(url, null,
            {
                observe: "response",
                withCredentials: true,
                responseType: "blob"
            })
            .map((response: HttpResponse<any>) => {
                const blob = new Blob([response.body], { type: response.headers.get("Content-Type") });
                var contentDispositionHeader = response.headers.get("Content-Disposition");
                let fileName = "";
                if (contentDispositionHeader) {
                    const parts = contentDispositionHeader.split(";");
                    fileName = parts.length > 0
                        ? parts[1].trim().split("=")[1].slice(1,-1)
                        : project.fileName;
                }

                return {
                    blob: blob,
                    name: fileName
                }
            });
    }   
}

