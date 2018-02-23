import { Component, OnInit, OnDestroy } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { Subject } from "rxjs/Subject";

import { createEqualsValidator } from "../shared/equals.validator";
import { passwordValidator } from "../shared/password.validator";
import { AppState, AuthState, ResetPassword } from "../../state";

@Component({
    templateUrl: "./reset-password.component.html",
    styleUrls: ["./reset-password.component.css"]
})
export class ResetPasswordComponent implements OnInit, OnDestroy {
    form: FormGroup;
    password: FormControl;
    password2: FormControl;
    state$: Observable<AuthState>;
    private dead$ = new Subject();
    private id: string;
    private code: string;
    constructor(private route: ActivatedRoute, private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.auth);
    }

    ngOnInit(): void {
        this.password = new FormControl(null, passwordValidator);
        this.password2 = new FormControl(null, createEqualsValidator(this.password, this.dead$));

        this.form = new FormGroup({
            password: this.password,
            password2: this.password2
        });
    }

    ngOnDestroy(): void {
        this.dead$.next();
        this.dead$.complete();
    }

    submit(): void {
        if (this.form.valid) {
            const id = this.route.snapshot.queryParams["id"];
            const code = this.route.snapshot.queryParams["code"];
            this.store.dispatch(new ResetPassword(id, code, this.password.value));
        }
    }
}
