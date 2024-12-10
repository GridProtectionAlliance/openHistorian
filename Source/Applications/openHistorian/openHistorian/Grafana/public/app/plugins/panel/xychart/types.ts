import { DataFrame, Field, FieldColorMode } from '@grafana/data';
import { LineStyle, ScaleDimensionConfig, VisibilityMode } from '@grafana/schema';
import { VizLegendItem } from '@grafana/ui';

/**
 * @internal
 */
export type DimensionValues<T> = (frame: DataFrame, from?: number) => T | T[];

// Using field where we will need formatting/scale/axis info
// Use raw or DimensionValues when the values can be used directly
export interface ScatterSeries {
  name: string;

  /** Finds the relevant frame from the raw panel data */
  frame: (frames: DataFrame[]) => DataFrame;

  x: (frame: DataFrame) => Field;
  y: (frame: DataFrame) => Field;

  legend: () => VizLegendItem[]; // could be single if symbol is constant

  showLine: boolean;
  lineWidth: number;
  lineStyle: LineStyle;
  lineColor: (frame: DataFrame) => CanvasRenderingContext2D['strokeStyle'];

  showPoints: VisibilityMode;
  pointSize: DimensionValues<number>;
  pointColor: DimensionValues<CanvasRenderingContext2D['strokeStyle']>;
  pointSymbol: DimensionValues<string>; // single field, multiple symbols.... kinda equals multiple series

  label: VisibilityMode;
  labelValue: DimensionValues<string>;
  show: boolean;

  hints: {
    pointSize: ScaleDimensionConfig;
    pointColor: {
      mode: FieldColorMode;
    };
  };
}
