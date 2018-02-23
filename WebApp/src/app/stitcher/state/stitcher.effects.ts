import { Injectable } from "@angular/core";
import { Actions, Effect } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import "rxjs/add/observable/of";

import { AppState } from "../../state";
import * as forRoot from "./stitcher.actions";
import { StitcherService } from "./stitcher.service";

@Injectable()
export class StitcherEffects {
    @Effect()
    onExport = this.actions$
        .ofType(forRoot.STITCH_PROJECTS)
        .switchMap((action: forRoot.StitchProjects) =>
            this.stitcherService
                .stitch(action.params)
                .map(() => new forRoot.StitchSucceeded())
                .catch(error => Observable.of(new forRoot.StitchFailed(error)))
        );

    constructor(private actions$: Actions, private store: Store<AppState>, private stitcherService: StitcherService) {
    }
}
