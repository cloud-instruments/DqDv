import { AuthState } from "../auth/state/auth.state";
import { ChartState } from "../chart/state/chart.state";
import { FeedbackState } from "../feedback/state/feedback.state";
import { StitcherState } from "../stitcher/state/stitcher.state";
import { StatisticStitcherState } from "../statistic-project/state/statistic-stitcher.state";
import { ViewState } from "../view/state/view.state";
import { UploadState } from "../upload/state/upload.state";
import { UserPreferencesState } from "../preferences/state/preferences.state";

export interface AppState {
    auth: AuthState;
    chart: ChartState;
    feedback: FeedbackState;
    stitcher: StitcherState;
    statisticStitcher: StatisticStitcherState;
    viewer: ViewState;
    upload: UploadState;
    userPreferences: UserPreferencesState;
}
