export interface StatisticProjectParams {
    projects: number[];
    name: string;
    testName: string;
    testType: string;
    channel: string;
    tag: string;
    mass?: number;
    area?: number;
    comments: string;
}
