export interface ProjectListItem {
    id: number;
    traceId: string;
    name: string;
    fileName: string;
    fileSize: number;
    testName: string;
    testType: string;
    channel: string;
    tag: string;
    mass?: number;
    area?: number;
    comments: string;
    createdAt: Date;
    updatedAt: Date;
    isReady: boolean;
    failed: boolean;
    error: string;
    numCycles: number;
    ownerName: string;
    stitchedFromNames: string;
}
