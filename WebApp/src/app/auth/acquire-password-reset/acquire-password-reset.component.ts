import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";

import { AppState, AuthState, AcquirePasswordReset } from "../../state";

@Component({
    templateUrl: "./acquire-password-reset.component.html",
    styleUrls: ["./acquire-password-reset.component.css"]
})
export class AcquirePasswordResetComponent implements OnInit {
    form: FormGroup;
    username: FormControl;
    state$: Observable<AuthState>;

    constructor(private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.auth);
    }

    ngOnInit(): void {
        this.username = new FormControl(null, Validators.required);

        this.form = new FormGroup({
            username: this.username
        });
    }

    submit(): void {
        if (this.form.valid) {
            this.store.dispatch(new AcquirePasswordReset(this.form.value.username));
        }
    }
}
