import { DataQuery, ReducerID, SelectableValue } from '@grafana/data';

import { EvalFunction } from '../alerting/state/alertDef';

export enum ExpressionQueryType {
  math = 'math',
  reduce = 'reduce',
  resample = 'resample',
  classic = 'classic_conditions',
}

export const gelTypes: Array<SelectableValue<ExpressionQueryType>> = [
  { value: ExpressionQueryType.math, label: 'Math' },
  { value: ExpressionQueryType.reduce, label: 'Reduce' },
  { value: ExpressionQueryType.resample, label: 'Resample' },
  { value: ExpressionQueryType.classic, label: 'Classic condition' },
];

export const reducerTypes: Array<SelectableValue<string>> = [
  { value: ReducerID.min, label: 'Min', description: 'Get the minimum value' },
  { value: ReducerID.max, label: 'Max', description: 'Get the maximum value' },
  { value: ReducerID.mean, label: 'Mean', description: 'Get the average value' },
  { value: ReducerID.sum, label: 'Sum', description: 'Get the sum of all values' },
  { value: ReducerID.count, label: 'Count', description: 'Get the number of values' },
  { value: ReducerID.last, label: 'Last', description: 'Get the last value' },
];

export enum ReducerMode {
  Strict = '', // backend API wants an empty string to support "strict" mode
  ReplaceNonNumbers = 'replaceNN',
  DropNonNumbers = 'dropNN',
}

export const reducerMode: Array<SelectableValue<ReducerMode>> = [
  {
    value: ReducerMode.Strict,
    label: 'Strict',
    description: 'Result can be NaN if series contains non-numeric data',
  },
  {
    value: ReducerMode.DropNonNumbers,
    label: 'Drop Non-numeric Values',
    description: 'Drop NaN, +/-Inf and null from input series before reducing',
  },
  {
    value: ReducerMode.ReplaceNonNumbers,
    label: 'Replace Non-numeric Values',
    description: 'Replace NaN, +/-Inf and null with a constant value before reducing',
  },
];

export const downsamplingTypes: Array<SelectableValue<string>> = [
  { value: ReducerID.min, label: 'Min', description: 'Fill with the minimum value' },
  { value: ReducerID.max, label: 'Max', description: 'Fill with the maximum value' },
  { value: ReducerID.mean, label: 'Mean', description: 'Fill with the average value' },
  { value: ReducerID.sum, label: 'Sum', description: 'Fill with the sum of all values' },
];

export const upsamplingTypes: Array<SelectableValue<string>> = [
  { value: 'pad', label: 'pad', description: 'fill with the last known value' },
  { value: 'backfilling', label: 'backfilling', description: 'fill with the next known value' },
  { value: 'fillna', label: 'fillna', description: 'Fill with NaNs' },
];

/**
 * For now this is a single object to cover all the types.... would likely
 * want to split this up by type as the complexity increases
 */
export interface ExpressionQuery extends DataQuery {
  type: ExpressionQueryType;
  reducer?: string;
  expression?: string;
  window?: string;
  downsampler?: string;
  upsampler?: string;
  conditions?: ClassicCondition[];
  settings?: ExpressionQuerySettings;
}

export interface ExpressionQuerySettings {
  mode?: ReducerMode;
  replaceWithValue?: number;
}

export interface ClassicCondition {
  evaluator: {
    params: number[];
    type: EvalFunction;
  };
  operator?: {
    type: string;
  };
  query: {
    params: string[];
  };
  reducer: {
    params: [];
    type: ReducerType;
  };
  type: 'query';
}

export type ReducerType =
  | 'avg'
  | 'min'
  | 'max'
  | 'sum'
  | 'count'
  | 'last'
  | 'median'
  | 'diff'
  | 'diff_abs'
  | 'percent_diff'
  | 'percent_diff_abs'
  | 'count_non_null';
