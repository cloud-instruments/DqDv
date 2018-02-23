import { Action } from "@ngrx/store";

import { StatisticProjectParams } from "../model/statistic-project-params";

export const STATISTIC_STITCH_PROJECTS = "STATISTIC_STITCHER: STITCH_PROJECTS";
export const STATISTIC_STITCH_SUCCEEDED = "STATISTIC_STITCHER: STITCH_SUCCEEDED";
export const STATISTIC_STITCH_FAILED = "STATISTIC_STITCHER: STITCH_FAILED";
export const STATISTIC_CLEAR_STITCH_ERROR = "STATISTIC_STITCHER: CLEAR_STITCH_ERROR";

export class StatisticStitchProjects implements Action {
    readonly type = STATISTIC_STITCH_PROJECTS;

    constructor(public params: StatisticProjectParams) {
    }
}

export class StatisticStitchSucceeded implements Action {
    readonly type = STATISTIC_STITCH_SUCCEEDED;
}

export class StatisticStitchFailed implements Action {
    readonly type = STATISTIC_STITCH_FAILED;

    constructor(public error: string) {
    }
}

export class StatisticClearStitchError implements Action {
    readonly type = STATISTIC_CLEAR_STITCH_ERROR;
}

export type StatisticStitcherAction
    = StatisticStitchProjects
    | StatisticStitchSucceeded
    | StatisticStitchFailed
    | StatisticClearStitchError;
