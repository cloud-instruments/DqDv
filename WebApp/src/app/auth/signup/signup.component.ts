import { Component, OnInit, OnDestroy } from "@angular/core";
import { AbstractControl, FormControl, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { Subject } from "rxjs/Subject";

import { createEqualsValidator } from "../shared/equals.validator";
import { passwordValidator } from "../shared/password.validator";
import { AppState, AuthState, Signup } from "../../state";

@Component({
    templateUrl: "./signup.component.html",
    styleUrls: ["./signup.component.css"]
})
export class SignupComponent implements OnInit, OnDestroy {
    form: FormGroup;
    username: FormControl;
    email: FormControl;
    password: FormControl;
    password2: FormControl;
    state$: Observable<AuthState>;
    private dead$ = new Subject();

    constructor(private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.auth);
    }

    ngOnInit(): void {
        this.username = new FormControl(null, Validators.required);
        this.email = new FormControl(null, [Validators.required, Validators.email]);
        this.password = new FormControl(null, passwordValidator);
        this.password2 = new FormControl(null, createEqualsValidator(this.password, this.dead$));

        this.form = new FormGroup({
            username: this.username,
            email: this.email,
            password: this.password,
            password2: this.password2
        });
    }

    ngOnDestroy(): void {
        this.dead$.next();
        this.dead$.complete();
    }

    signup(): void {
        if (this.form.valid) {
            this.store.dispatch(new Signup(this.username.value, this.email.value, this.password.value));
        }
    }
}
