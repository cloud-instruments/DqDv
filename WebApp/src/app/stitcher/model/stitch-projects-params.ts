export interface StitchProjectsParams {
    projects: number[];
    name: string;
    testName: string;
    testType: string;
    channel: string;
    tag: string;
    mass?: number;
    area?: number;
    comments: string;
    tryMergeAdjacentCycles: boolean;
}
