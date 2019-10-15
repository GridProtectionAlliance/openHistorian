import { Unsubscribable } from 'rxjs';
import { ComponentClass } from 'react';
import {
  DataQuery,
  DataSourceSelectItem,
  DataSourceApi,
  QueryHint,
  ExploreStartPageProps,
  PanelData,
  DataQueryRequest,
} from '@grafana/ui';

import {
  RawTimeRange,
  LogLevel,
  TimeRange,
  LogsModel,
  LogsDedupStrategy,
  AbsoluteTimeRange,
  GraphSeriesXY,
} from '@grafana/data';

import { Emitter } from 'app/core/core';
import TableModel from 'app/core/table_model';

import { Value } from 'slate';

import { Editor } from '@grafana/slate-react';
export enum ExploreMode {
  Metrics = 'Metrics',
  Logs = 'Logs',
}

export enum CompletionItemKind {
  GroupTitle = 'GroupTitle',
}

export interface CompletionItem {
  /**
   * The label of this completion item. By default
   * this is also the text that is inserted when selecting
   * this completion.
   */
  label: string;

  /**
   * The kind of this completion item. An icon is chosen
   * by the editor based on the kind.
   */
  kind?: CompletionItemKind | string;

  /**
   * A human-readable string with additional information
   * about this item, like type or symbol information.
   */
  detail?: string;

  /**
   * A human-readable string, can be Markdown, that represents a doc-comment.
   */
  documentation?: string;

  /**
   * A string that should be used when comparing this item
   * with other items. When `falsy` the `label` is used.
   */
  sortText?: string;

  /**
   * A string that should be used when filtering a set of
   * completion items. When `falsy` the `label` is used.
   */
  filterText?: string;

  /**
   * A string or snippet that should be inserted in a document when selecting
   * this completion. When `falsy` the `label` is used.
   */
  insertText?: string;

  /**
   * Delete number of characters before the caret position,
   * by default the letters from the beginning of the word.
   */
  deleteBackwards?: number;

  /**
   * Number of steps to move after the insertion, can be negative.
   */
  move?: number;
}

export interface CompletionItemGroup {
  /**
   * Label that will be displayed for all entries of this group.
   */
  label: string;

  /**
   * List of suggestions of this group.
   */
  items: CompletionItem[];

  /**
   * If true, match only by prefix (and not mid-word).
   */
  prefixMatch?: boolean;

  /**
   * If true, do not filter items in this group based on the search.
   */
  skipFilter?: boolean;

  /**
   * If true, do not sort items.
   */
  skipSort?: boolean;
}

export enum ExploreId {
  left = 'left',
  right = 'right',
}

/**
 * Global Explore state
 */
export interface ExploreState {
  /**
   * True if split view is active.
   */
  split: boolean;
  /**
   * Explore state of the left split (left is default in non-split view).
   */
  left: ExploreItemState;
  /**
   * Explore state of the right area in split view.
   */
  right: ExploreItemState;
}

export interface ExploreItemState {
  /**
   * React component to be shown when no queries have been run yet, e.g., for a query language cheat sheet.
   */
  StartPage?: ComponentClass<ExploreStartPageProps>;
  /**
   * Width used for calculating the graph interval (can't have more datapoints than pixels)
   */
  containerWidth: number;
  /**
   * Datasource instance that has been selected. Datasource-specific logic can be run on this object.
   */
  datasourceInstance: DataSourceApi | null;
  /**
   * Current data source name or null if default
   */
  requestedDatasourceName: string | null;
  /**
   * Error to be shown when datasource loading or testing failed.
   */
  datasourceError: string;
  /**
   * True if the datasource is loading. `null` if the loading has not started yet.
   */
  datasourceLoading: boolean | null;
  /**
   * True if there is no datasource to be selected.
   */
  datasourceMissing: boolean;
  /**
   * Emitter to send events to the rest of Grafana.
   */
  eventBridge?: Emitter;
  /**
   * List of datasources to be shown in the datasource selector.
   */
  exploreDatasources: DataSourceSelectItem[];
  /**
   * List of timeseries to be shown in the Explore graph result viewer.
   */
  graphResult?: GraphSeriesXY[];
  /**
   * History of recent queries. Datasource-specific and initialized via localStorage.
   */
  history: HistoryItem[];
  /**
   * Queries for this Explore, e.g., set via URL. Each query will be
   * converted to a query row.
   */
  queries: DataQuery[];
  /**
   * True if this Explore area has been initialized.
   * Used to distinguish URL state injection versus split view state injection.
   */
  initialized: boolean;
  /**
   * Log line substrings to be highlighted as you type in a query field.
   * Currently supports only the first query row.
   */
  logsHighlighterExpressions?: string[];
  /**
   * Log query result to be displayed in the logs result viewer.
   */
  logsResult?: LogsModel;

  /**
   * Time range for this Explore. Managed by the time picker and used by all query runs.
   */
  range: TimeRange;

  absoluteRange: AbsoluteTimeRange;
  /**
   * True if scanning for more results is active.
   */
  scanning?: boolean;
  /**
   * Current scanning range to be shown to the user while scanning is active.
   */
  scanRange?: RawTimeRange;
  /**
   * True if graph result viewer is expanded. Query runs will contain graph queries.
   */
  showingGraph: boolean;
  /**
   * True StartPage needs to be shown. Typically set to `false` once queries have been run.
   */
  showingStartPage?: boolean;
  /**
   * True if table result viewer is expanded. Query runs will contain table queries.
   */
  showingTable: boolean;

  loading: boolean;
  /**
   * Table model that combines all query table results into a single table.
   */
  tableResult?: TableModel;

  /**
   * React keys for rendering of QueryRows
   */
  queryKeys: string[];

  /**
   * Current logs deduplication strategy
   */
  dedupStrategy?: LogsDedupStrategy;

  /**
   * Currently hidden log series
   */
  hiddenLogLevels?: LogLevel[];

  /**
   * How often query should be refreshed
   */
  refreshInterval?: string;

  urlState: ExploreUrlState;

  update: ExploreUpdateState;

  latency: number;
  supportedModes: ExploreMode[];
  mode: ExploreMode;

  /**
   * If true, the view is in live tailing mode.
   */
  isLive: boolean;

  /**
   * If true, the live tailing view is paused.
   */
  isPaused: boolean;
  urlReplaced: boolean;

  querySubscription?: Unsubscribable;

  queryResponse: PanelData;
  originPanelId?: number;
}

export interface ExploreUpdateState {
  datasource: boolean;
  queries: boolean;
  range: boolean;
  mode: boolean;
  ui: boolean;
}

export interface ExploreUIState {
  showingTable: boolean;
  showingGraph: boolean;
  showingLogs: boolean;
  dedupStrategy?: LogsDedupStrategy;
}

export interface ExploreUrlState {
  datasource: string;
  queries: any[]; // Should be a DataQuery, but we're going to strip refIds, so typing makes less sense
  mode: ExploreMode;
  range: RawTimeRange;
  ui: ExploreUIState;
  originPanelId?: number;
}

export interface HistoryItem<TQuery extends DataQuery = DataQuery> {
  ts: number;
  query: TQuery;
}

export abstract class LanguageProvider {
  datasource: DataSourceApi;
  request: (url: string, params?: any) => Promise<any>;
  /**
   * Returns startTask that resolves with a task list when main syntax is loaded.
   * Task list consists of secondary promises that load more detailed language features.
   */
  start: () => Promise<any[]>;
  startTask?: Promise<any[]>;
}

export interface TypeaheadInput {
  text: string;
  prefix: string;
  wrapperClasses: string[];
  labelKey?: string;
  value?: Value;
  editor?: Editor;
}

export interface TypeaheadOutput {
  context?: string;
  suggestions: CompletionItemGroup[];
}

export interface QueryIntervals {
  interval: string;
  intervalMs: number;
}

export interface QueryOptions {
  minInterval: string;
  maxDataPoints?: number;
  liveStreaming?: boolean;
  showingGraph?: boolean;
  showingTable?: boolean;
}

export interface QueryTransaction {
  id: string;
  done: boolean;
  error?: string | JSX.Element;
  hints?: QueryHint[];
  latency: number;
  request: DataQueryRequest;
  queries: DataQuery[];
  result?: any; // Table model / Timeseries[] / Logs
  scanning?: boolean;
}
