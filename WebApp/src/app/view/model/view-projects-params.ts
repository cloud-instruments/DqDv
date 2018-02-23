export interface ViewProjectsParams {
    projects: number[];
    name: string;   
    plotTemplateId: number; 
    comments: string;
}
export interface View {
    id: number;
    name: string;
    template: string;
    comments: string;
    projects: number[];
    projectsCount: number;
    createdAt: string;
    updatedAt: string;
}

export interface PlotTemplateShort {
    id: number;
    name: string;
}