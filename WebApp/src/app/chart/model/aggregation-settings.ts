export enum AggregationType {
    None = 0,
    Default = 1,
    VisvalingamWhyatt = 2
}

export interface AggregationSettings {
    algorithm: AggregationType;
}
