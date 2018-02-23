import * as forRoot from "./view.actions";
import { ViewState } from "./view.state";

const initialState: ViewState = {
    viewing: false,
    views: null,
    error: null
};

export function viewerReducer(state: ViewState = initialState, action: forRoot.ViewAction): ViewState {
    switch (action.type) {
        case forRoot.VIEW_PROJECTS:
            return {
                ...state,
                viewing: true,
                error: null
            };
       
        case forRoot.VIEW_SUCCEEDED:
            return {
                ...state,
                views: action.views,
                viewing: false
            };

        case forRoot.VIEW_FAILED:
            return {
                ...state,
                viewing: false,
                error: action.error
            };

        case forRoot.CLEAR_VIEW_ERROR:
            return {
                ...state,
                error: null
            };

        default:
            return state;
    }
}
