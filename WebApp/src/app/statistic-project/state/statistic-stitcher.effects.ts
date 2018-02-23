import { Injectable } from "@angular/core";
import { Actions, Effect } from "@ngrx/effects";
import { Store } from "@ngrx/store";
import { Observable } from "rxjs/Observable";
import "rxjs/add/observable/of";

import { AppState } from "../../state";
import * as forRoot from "./statistic-stitcher.actions";
import { StatisticStitcherService } from "./statistic-stitcher.service";

@Injectable()
export class StatisticStitcherEffects {
    @Effect()
    onCalculateAverage = this.actions$
        .ofType(forRoot.STATISTIC_STITCH_PROJECTS)
        .switchMap((action: forRoot.StatisticStitchProjects) =>
            this.stitcherService
                .stitch(action.params)
                .map(() => new forRoot.StatisticStitchSucceeded())
                .catch(error => Observable.of(new forRoot.StatisticStitchFailed(error)))
        );

    constructor(private actions$: Actions, private store: Store<AppState>, private stitcherService: StatisticStitcherService) {
    }
}
