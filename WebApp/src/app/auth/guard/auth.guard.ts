import { Injectable } from "@angular/core";
import { CanActivate, Router } from "@angular/router";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";

import { AppState } from "../../state";

@Injectable()
export class AuthGuard implements CanActivate {
    constructor(private store: Store<AppState>, private router: Router) {
    }

    canActivate(): Observable<boolean> {
        return this.store
            .select(s => s.auth.loggedIn)
            .do(val => {
                if (!val) {
                    this.router.navigate(["/login"]);
                }
            });
    }
}
