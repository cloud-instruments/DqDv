import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";

import { AppState, AuthState, Login } from "../../state";

@Component({
    templateUrl: "./login.component.html",
    styleUrls: ["./login.component.css"]
})
export class LoginComponent implements OnInit {
    form: FormGroup;
    username: FormControl;
    password: FormControl;
    rememberMe: FormControl;
    state$: Observable<AuthState>;

    constructor(private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.auth);
    }

    ngOnInit(): void {
        this.username = new FormControl(null, Validators.required);
        this.password = new FormControl(null);
        this.rememberMe = new FormControl(false);

        this.form = new FormGroup({
            username: this.username,
            password: this.password,
            rememberMe: this.rememberMe
        });
    }

    login(): void {
        if (this.form.invalid) {
            return;
        }

        this.store.dispatch(new Login(this.username.value, this.password.value, this.rememberMe.value));
        this.password.setValue(null);
    }
}
