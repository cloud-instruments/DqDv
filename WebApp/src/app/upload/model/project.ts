export interface Project {
    file: File[];
    name: string;
    testName: string;
    testType: string;
    channel: string;
    tag: string;
    mass?: number;
    theoreticalCapacity?: number;
    activeMaterialFraction?: number;
    area?: number;
    comments: string;
}
