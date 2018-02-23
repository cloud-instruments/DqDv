import { Action } from "@ngrx/store";

import { ViewProjectsParams, View } from "../model/view-projects-params";

export const VIEW_PROJECTS = "VIEW: VIEW_PROJECTS";
export const VIEW_ADD = "VIEW: VIEW_ADD";

export const VIEW_SUCCEEDED = "VIEW: VIEW_SUCCEEDED";
export const VIEW_FAILED = "VIEW: VIEW_FAILED";
export const CLEAR_VIEW_ERROR = "VIEW: CLEAR_VIEW_ERROR";
export const VIEW_DELETE = "VIEW: VIEW_DELETE";


export class ViewProjects implements Action {
    readonly type = VIEW_PROJECTS;

    constructor() {
    }
}

export class AddView implements Action {
    readonly type = VIEW_ADD;

    constructor(public params: ViewProjectsParams) {
    }
}

export class ViewSucceeded implements Action {
    readonly type = VIEW_SUCCEEDED;
    constructor(public views: View[]) {
    }
}

export class ViewFailed implements Action {
    readonly type = VIEW_FAILED;

    constructor(public error: string) {
    }
}

export class ViewDeleted implements Action {
    readonly type = VIEW_DELETE;
    constructor(public viewIdValue: number) {
    }
}

export class ClearViewError implements Action {
    readonly type = CLEAR_VIEW_ERROR;
}

export type ViewAction
    = ViewProjects
    | ViewSucceeded
    | AddView
    | ViewFailed
    | ClearViewError
    | ViewDeleted;
