import { Injectable } from "@angular/core";
import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import "rxjs/add/operator/do";

import { AppState, Unauthorized } from "../../state";

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private store: Store<AppState>) {
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request)
            .do(() => { },
                (err: any) => {
                    if (err instanceof HttpErrorResponse && err.status === 401) {
                        this.store.dispatch(new Unauthorized());
                    }
                });
    }
}
