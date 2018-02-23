import { HttpEventType, HttpErrorResponse, HttpProgressEvent, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Actions, Effect, toPayload } from "@ngrx/effects";
import { Observable } from "rxjs/Observable";
import { Store } from "@ngrx/store";
import "rxjs/add/observable/of";
import "rxjs/add/operator/filter";
import "rxjs/add/operator/map";
import { empty } from "rxjs/observable/empty";

import * as forRoot from "./upload.actions";
import { UploadService } from "./upload.service";
import { PopupService } from "../../shared/popup/popup.service";
import { AppState } from "../../state";


@Injectable()
export class UploadEffects {
    
    @Effect()
    onCheckProjectsUnique = this.actions$
        .ofType(forRoot.BEFORE_START_UPLOAD)
        .switchMap((action: forRoot.BeforeStartUpload) => 
            this.uploadService
                .uniqueProjectsCheck(action.project)
                .map(response => {
                    if (response.isAllUnique)
                        return new forRoot.StartUpload(action.project);
                    return new forRoot.ProjectOverwriteComfirmation(action.project);
                }));

    @Effect()
    onProjectOverwriteComfirmation = this.actions$
        .ofType(forRoot.UPLOAD_OVERWRITE_CONFIRMATION)
        .switchMap((action: forRoot.ProjectOverwriteComfirmation) => {
                this.popupService.showConfirm("Confirm Project Overwriting", "One or more projects cannot be created because they already exsit. Do you want to overwrite them?")
                    .subscribe(result => {
                        if (result === true)
                            this.store$.dispatch(new forRoot.StartUpload(action.project));;
                })
                return empty();
            }
        );
    
    @Effect()
    onStartUpload = this.actions$
        .ofType(forRoot.START_UPLOAD)
        .switchMap((action: forRoot.StartUpload) =>
            this.uploadService
                .upload(action.project)
                .map((event) => {
                    if (event.type === HttpEventType.UploadProgress) {
                        const progress = event as HttpProgressEvent;
                        const percentDone = Math.round(100 * progress.loaded / progress.total);
                        return new forRoot.UploadProgress(percentDone);
                    } else if (event instanceof HttpResponse) {
                        return new forRoot.UploadSucceeded();
                    }
                })
                .filter(a => a !== undefined)
                .catch(error => {
                    if (error instanceof HttpErrorResponse && error.status === 400) {
                        const message = this.getMessage(error);
                        return Observable.of(new forRoot.UploadFailed(message || "Bad request"));
                    } else {
                        return Observable.of(new forRoot.UploadFailed("Network error"));
                    }
                })
            );


    @Effect({ dispatch: false })
    onUploadSucceeded = this.actions$
        .ofType(forRoot.UPLOAD_SUCCEEDED)
        .do(() => this.router.navigate(["/projects"]));

    constructor(private actions$: Actions, private uploadService: UploadService, private router: Router, private popupService: PopupService, private store$: Store<AppState>) {
        
    }

    private getMessage(error: HttpErrorResponse): string {
        if (!error.error) {
            return null;
        }

        try {
            const response = JSON.parse(error.error);
            if (!response) {
                return null;
            }

            return response.message;
        } catch (e) {
            return null;
        }
    }
}
