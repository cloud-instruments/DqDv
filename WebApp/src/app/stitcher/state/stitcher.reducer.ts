import * as forRoot from "./stitcher.actions";
import { StitcherState } from "./stitcher.state";

const initialState: StitcherState = {
    stitching: false,
    error: null
};

export function stitcherReducer(state: StitcherState = initialState, action: forRoot.StitcherAction): StitcherState {
    switch (action.type) {
        case forRoot.STITCH_PROJECTS:
            return {
                ...state,
                stitching: true,
                error: null
            };

        case forRoot.STITCH_SUCCEEDED:
            return {
                ...state,
                stitching: false
            };

        case forRoot.STITCH_FAILED:
            return {
                ...state,
                stitching: false,
                error: action.error
            };

        case forRoot.CLEAR_STITCH_ERROR:
            return {
                ...state,
                error: null
            };

        default:
            return state;
    }
}
