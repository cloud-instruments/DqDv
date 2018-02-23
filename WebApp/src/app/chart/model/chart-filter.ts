export interface ChartFilter {
    from?: number;
    to?: number;
    everyNth?: number;
    custom?: string;

    disableCharge?: boolean;
    disableDischarge?: boolean;

    threshold?: number;
    minY?: number;
    maxY?: number;
}
