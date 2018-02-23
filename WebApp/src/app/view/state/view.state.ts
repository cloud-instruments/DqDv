import { View } from "../model/view-projects-params";

export interface ViewState {
    viewing: boolean;
    views: View[];
    error: string;
}
