import * as forRoot from "../../state";

const initialState: forRoot.FeedbackState = {
    uploading: false,
    percentDone: 0,
    error: null
};

export function feedbackReducer(state: forRoot.FeedbackState = initialState, action: forRoot.FeedbackAction): forRoot.FeedbackState {
    switch (action.type) {
        case forRoot.SEND_FEEDBACK:
            return {
                ...state,
                uploading: true,
                percentDone: 0,
                error: null
            };

        case forRoot.SEND_FEEDBACK_PROGRESS:
            return {
                ...state,
                percentDone: action.percentDone
            };

        case forRoot.SEND_FEEDBACK_SUCCEEDED:
            return {
                ...state,
                uploading: false
            };

        case forRoot.SEND_FEEDBACK_FAILED:
            return {
                ...state,
                uploading: false,
                error: action.error
            };

        default:
            return state;
    }
}
