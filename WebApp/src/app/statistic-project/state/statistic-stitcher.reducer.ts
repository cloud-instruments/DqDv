import * as forRoot from "./statistic-stitcher.actions";
import { StatisticStitcherState } from "./statistic-stitcher.state";

const initialState: StatisticStitcherState = {
    stitching: false,
    error: null
};

export function statisticStitcherReducer(state: StatisticStitcherState = initialState, action: forRoot.StatisticStitcherAction): StatisticStitcherState {
    switch (action.type) {
        case forRoot.STATISTIC_STITCH_PROJECTS:
            return {
                ...state,
                stitching: true,
                error: null
            };

        case forRoot.STATISTIC_STITCH_SUCCEEDED:
            return {
                ...state,
                stitching: false
            };

        case forRoot.STATISTIC_STITCH_FAILED:
            return {
                ...state,
                stitching: false,
                error: action.error
            };

        case forRoot.STATISTIC_CLEAR_STITCH_ERROR:
            return {
                ...state,
                error: null
            };

        default:
            return state;
    }
}
