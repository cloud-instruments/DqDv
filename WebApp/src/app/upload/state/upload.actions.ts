import { Action } from "@ngrx/store";

import { Project } from "../model/project";

export const BEFORE_START_UPLOAD = "UPLOAD: BEFORE_START_UPLOAD";
export const START_UPLOAD = "UPLOAD: START_UPLOAD";
export const UPLOAD_PROGRESS = "UPLOAD: UPLOAD_PROGRESS";
export const UPLOAD_SUCCEEDED = "UPLOAD: UPLOAD_SUCCEEDED";
export const UPLOAD_FAILED = "UPLOAD: UPLOAD_FAILED";
export const UPLOAD_OVERWRITE_CONFIRMATION = "UPLOAD: UPLOAD_OVERWRITE_CONFIRMATION";

export class BeforeStartUpload implements Action {
    readonly type = BEFORE_START_UPLOAD;

    constructor(public project: Project) {
    }
}

export class StartUpload implements Action {
    readonly type = START_UPLOAD;

    constructor(public project: Project) {
    }
}

export class UploadProgress implements Action {
    readonly type = UPLOAD_PROGRESS;

    constructor(public percentDone: number) {
    }
}

export class UploadSucceeded implements Action {
    readonly type = UPLOAD_SUCCEEDED;
}

export class UploadFailed implements Action {
    readonly type = UPLOAD_FAILED;

    constructor(public error: string) {
    }
}

export class ProjectOverwriteComfirmation implements Action {
    readonly type = UPLOAD_OVERWRITE_CONFIRMATION;

    constructor(public project: Project) {
    }
}


export type UploadAction
    = BeforeStartUpload
    | StartUpload
    | UploadProgress
    | UploadSucceeded
    | UploadFailed
    | ProjectOverwriteComfirmation;
