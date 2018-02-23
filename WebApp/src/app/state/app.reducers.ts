import { ActionReducerMap } from "@ngrx/store";

import { AppState } from "./app.state";
import { authReducer } from "../auth/state/auth.reducers";
import { chartReducer } from "../chart/state/chart.reducers";
import { feedbackReducer } from "../feedback/state/feedback.reducer";
import { stitcherReducer } from "../stitcher/state/stitcher.reducer";
import { statisticStitcherReducer } from "../statistic-project/state/statistic-stitcher.reducer";
import { viewerReducer } from "../view/state/view.reducer";
import { uploadReducer } from "../upload/state/upload.reducers";
import { userPreferencesReducer } from "../preferences/state/preferences.reducer";

export const appReducers: ActionReducerMap<AppState> = {
    auth: authReducer,
    chart: chartReducer,
    feedback: feedbackReducer,
    viewer: viewerReducer,
    stitcher: stitcherReducer,
    statisticStitcher: statisticStitcherReducer,
    upload: uploadReducer,
    userPreferences: userPreferencesReducer
};
