export interface UserPreferences {
    chartPreferences: ChartPreferences
}

export interface ChartPreferences {
    pointSize?: number;
    xLineVisible: boolean;
    yLineVisible: boolean;
    showLegend: boolean;
    fontSize?: number;
    fontFamilyName: string,
    paletteColors: ChartPaletteColor[];
}

export interface ChartPaletteColor {
    color: string;
}
