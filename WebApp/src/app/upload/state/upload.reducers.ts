import * as forRoot from "./upload.actions";
import { UploadState } from "./upload.state";

const initialState: UploadState = {
    uploading: false,
    percentDone: 0,
    error: null
};

export function uploadReducer(state: UploadState = initialState, action: forRoot.UploadAction): UploadState {
    switch (action.type) {
        case forRoot.BEFORE_START_UPLOAD:
            return {
                ...state,
                uploading: false,
                percentDone: 0,
                error: null
            };
        case forRoot.START_UPLOAD:
            return {
                ...state,
                uploading: true,
                percentDone: 0,
                error: null
            };

        case forRoot.UPLOAD_PROGRESS:
            return {
                ...state,
                percentDone: action.percentDone
            };

        case forRoot.UPLOAD_SUCCEEDED:
            return {
                ...state,
                uploading: false
            };

        case forRoot.UPLOAD_FAILED:
            return {
                ...state,
                uploading: false,
                error: action.error
            };

        default:
            return state;
    }
}
