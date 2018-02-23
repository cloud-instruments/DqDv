import { Action } from "@ngrx/store";

export const SEND_FEEDBACK = "FEEDBACK: SEND_FEEDBACK";
export const SEND_FEEDBACK_PROGRESS = "FEEDBACK: SEND_FEEDBACK_PROGRESS";
export const SEND_FEEDBACK_SUCCEEDED = "FEEDBACK: SEND_FEEDBACK_SUCCEEDED";
export const SEND_FEEDBACK_FAILED = "FEEDBACK: SEND_FEEDBACK_FAILED";

export class SendFeedback implements Action {
    readonly type = SEND_FEEDBACK;

    constructor(public comment: string, public file: File) {
    }
}

export class SendFeedbackProgress implements Action {
    readonly type = SEND_FEEDBACK_PROGRESS;

    constructor(public percentDone: number) {
    }
}

export class SendFeedbackSucceeded implements Action {
    readonly type = SEND_FEEDBACK_SUCCEEDED;
}

export class SendFeedbackFailed implements Action {
    readonly type = SEND_FEEDBACK_FAILED;

    constructor(public error: string) {
    }
}

export type FeedbackAction
    = SendFeedback
    | SendFeedbackProgress
    | SendFeedbackSucceeded
    | SendFeedbackFailed;
