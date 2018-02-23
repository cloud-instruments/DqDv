import { Injectable } from "@angular/core";
import { Actions, Effect } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import "rxjs/add/observable/of";

import { AppState } from "../../state";
import * as forRoot from "./view.actions";
import { ViewService } from "./view.service";

@Injectable()
export class ViewEffects {
    @Effect()
    onGetViews = this.actions$
        .ofType(forRoot.VIEW_PROJECTS)
        .switchMap((action: forRoot.ViewProjects) =>
            this.viewService
                .getViews()
                .map(views => new forRoot.ViewSucceeded(views))
                .catch(error => Observable.of(new forRoot.ViewFailed(error)))
    );

    @Effect()
    onSave = this.actions$
        .ofType(forRoot.VIEW_ADD)
        .switchMap((action: forRoot.AddView) =>
            this.viewService
                .add(action.params)
                .map(views => new forRoot.ViewSucceeded(views))
                .catch(error => Observable.of(new forRoot.ViewFailed(error)))
    );

    @Effect()
    onDeleteView = this.actions$
        .ofType(forRoot.VIEW_DELETE)
        .switchMap((action: forRoot.ViewDeleted) =>
            this.viewService
                .deleteView(action.viewIdValue)
                .map(views => new forRoot.ViewProjects())
                .catch(error => Observable.of(new forRoot.ViewFailed(error)))
        );

    constructor(private actions$: Actions, private store: Store<AppState>, private viewService: ViewService) {
    }
}
