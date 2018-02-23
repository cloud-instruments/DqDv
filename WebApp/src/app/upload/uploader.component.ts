import { Component, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { AppState, UploadState, BeforeStartUpload } from "../state";

@Component({
    templateUrl: "./uploader.component.html",
    styleUrls: ["./uploader.component.css"]
})
export class UploaderComponent implements OnInit {
    form: FormGroup;
    state$: Observable<UploadState>;

    constructor(private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.upload);
    }

    public ngOnInit(): void {
        this.form = new FormGroup({
            name: new FormControl(null, [Validators.required, Validators.maxLength(256)]),
            testName: new FormControl(null, Validators.maxLength(256)),
            testType: new FormControl(null, Validators.maxLength(256)),
            channel: new FormControl(null, Validators.maxLength(256)),
            tag: new FormControl(null, Validators.maxLength(256)),
            mass: new FormControl(null),
            activeMaterialFraction: new FormControl(null),
            theoreticalCapacity: new FormControl(null),
            area: new FormControl(null),
            comments: new FormControl(null, Validators.maxLength(256))
        });
    }

    public onSubmit(file: File[]): void {
        const project = { ...this.form.value, file: file };
        this.store.dispatch(new BeforeStartUpload(project));
        //this.store.dispatch(new StartUpload(project));
    }
}

export class CollapseExampleComponent {

    showBsCollapse() { }
    shownBsCollapse() { }
    hideBsCollapse() { }
    hiddenBsCollapse() { }
}
