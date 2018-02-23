import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from "@angular/core";
import { Router } from '@angular/router';
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Actions } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import { Subject } from "rxjs/Subject";
import { PlotTemplateShort } from "./model/view-projects-params";

import { AppState, ClearViewError, ViewState, ViewFailed, VIEW_FAILED, VIEW_SUCCEEDED, AddView } from "../state";
import { ProjectListItem } from "../projects/model/project-list-item";

@Component({
    selector: "app-view",
    templateUrl: "./view.component.html",
    styleUrls: ["./view.component.css"]
})
export class ViewComponent implements OnInit, OnDestroy {
    form: FormGroup;
    state$: Observable<ViewState>;

    selectedTemplateId: number;

    private dead$ = new Subject();
 
    @Input()
    plotTemplates: PlotTemplateShort[];

    @Input()
    projects: ProjectListItem[];

    @Output()
    close = new EventEmitter<any>();

    constructor(actions$: Actions, private router: Router, private store: Store<AppState>) {
        this.store.dispatch(new ClearViewError());
        this.state$ = this.store.select(s => s.viewer);

        actions$.ofType(VIEW_SUCCEEDED)
            .takeUntil(this.dead$)
            .subscribe(() => this.close.emit());
    }

    public ngOnInit(): void {
        this.selectedTemplateId = this.plotTemplates.length > 0 ? this.plotTemplates[0].id : null;
        this.form = new FormGroup({
            name: new FormControl(null, [Validators.required, Validators.maxLength(256)]),
            comments: new FormControl(null)
        });
    }

    public ngOnDestroy(): void {
        this.dead$.next();
        this.dead$.complete();
    }

    public onBack(): void {
        this.close.emit();
    }

    onChangeTemplate(e): void {       
        this.selectedTemplateId = e.target.value;       
    }

    public onSubmit(): void {
        if (this.selectedTemplateId != null) {
            const params = { name: this.form.get("name").value, comments: this.form.get("comments").value, projects: this.projects.map(p => p.id), plotTemplateId: this.selectedTemplateId };
            this.store.dispatch(new AddView(params));
          
            this.router.navigate(['/views']);
        }
    }
}
