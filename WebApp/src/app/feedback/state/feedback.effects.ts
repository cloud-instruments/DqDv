import { HttpEventType, HttpErrorResponse, HttpProgressEvent, HttpResponse } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Actions, Effect } from "@ngrx/effects";
import { Observable } from "rxjs/Observable";
import "rxjs/add/observable/of";
import "rxjs/add/operator/filter";
import "rxjs/add/operator/map";

import * as forRoot from "./feedback.actions";
import { FeedbackService } from "./feedback.service";

@Injectable()
export class FeedbackEffects {
    @Effect()
    onSendFeedback = this.actions$
        .ofType(forRoot.SEND_FEEDBACK)
        .switchMap((action: forRoot.SendFeedback) =>
            this.feedbackService
                .sendFeedback(action.comment, action.file)
                .map((event) => {
                    if (event.type === HttpEventType.UploadProgress) {
                        const progress = event as HttpProgressEvent;
                        const percentDone = Math.round(100 * progress.loaded / progress.total);
                        return new forRoot.SendFeedbackProgress(percentDone);
                    } else if (event instanceof HttpResponse) {
                        return new forRoot.SendFeedbackSucceeded();
                    }
                })
                .filter(a => a !== undefined)
                .catch(error => {
                    if (error instanceof HttpErrorResponse && error.status === 400) {
                        const message = this.getMessage(error);
                        return Observable.of(new forRoot.SendFeedbackFailed(message || "Bad request"));
                    } else {
                        return Observable.of(new forRoot.SendFeedbackFailed("Network error"));
                    }
                })
    );

    @Effect({ dispatch: false })
    onSendFeedbackSucceeded = this.actions$
        .ofType(forRoot.SEND_FEEDBACK_SUCCEEDED)
        .do(() => this.router.navigate(["/projects"]));

    constructor(private actions$: Actions, private router: Router, private feedbackService: FeedbackService) {
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
