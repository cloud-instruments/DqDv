import { Component, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { DxFileUploaderComponent } from "devextreme-angular";

import { AppState, FeedbackState, SendFeedback } from "../state";
import { environment } from "environments/environment";

@Component({
    templateUrl: "./feedback.component.html",
    styleUrls: ["./feedback.component.css"]
})
export class FeedbackComponent implements OnInit {
    @ViewChild(DxFileUploaderComponent)
    fileUploader: DxFileUploaderComponent;

    form: FormGroup;
    state$: Observable<FeedbackState>;

    constructor(private store: Store<AppState>) {
        this.state$ = this.store.select(s => s.feedback);
    }

    ngOnInit(): void {
        this.form = new FormGroup({
            comment: new FormControl(null, [Validators.required, Validators.maxLength(16000)])
        });
    }

    onSubmit(): void {
        this.store.dispatch(new SendFeedback(this.form.value.comment, this.getFile()));
    }

    onClear(): void {
        this.fileUploader.instance.reset();
    }

    isFileTooBig(): boolean {
        var file = this.getFile();
        if (!file)
            return false;

        return file.size > environment.maxFeedbackFilesize;
    }

    isFileSelected(): boolean {
        return this.getFile() != null;
    }

    private getFile(): File {
        if (!this.fileUploader.value)
            return null;

        var file = this.fileUploader.value[0] as File;
        if (!file)
            return null;

        return file;
    }
}
