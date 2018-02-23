import { Action } from "@ngrx/store";

import { StitchProjectsParams } from "../model/stitch-projects-params";

export const STITCH_PROJECTS = "STITCHER: STITCH_PROJECTS";
export const STITCH_SUCCEEDED = "STITCHER: STITCH_SUCCEEDED";
export const STITCH_FAILED = "STITCHER: STITCH_FAILED";
export const CLEAR_STITCH_ERROR = "STITCHER: CLEAR_STITCH_ERROR";

export class StitchProjects implements Action {
    readonly type = STITCH_PROJECTS;

    constructor(public params: StitchProjectsParams) {
    }
}

export class StitchSucceeded implements Action {
    readonly type = STITCH_SUCCEEDED;
}

export class StitchFailed implements Action {
    readonly type = STITCH_FAILED;

    constructor(public error: string) {
    }
}

export class ClearStitchError implements Action {
    readonly type = CLEAR_STITCH_ERROR;
}

export type StitcherAction
    = StitchProjects
    | StitchSucceeded
    | StitchFailed
    | ClearStitchError;
