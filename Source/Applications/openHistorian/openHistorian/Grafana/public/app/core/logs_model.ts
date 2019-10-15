import _ from 'lodash';
import { colors, getFlotPairs, ansicolor } from '@grafana/ui';

import {
  Labels,
  LogLevel,
  DataFrame,
  findCommonLabels,
  findUniqueLabels,
  getLogLevel,
  FieldType,
  getLogLevelFromKey,
  LogRowModel,
  LogsModel,
  LogsMetaItem,
  LogsMetaKind,
  LogsDedupStrategy,
  GraphSeriesXY,
  dateTime,
  toUtc,
  NullValueMode,
  toDataFrame,
  FieldCache,
} from '@grafana/data';
import { getThemeColor } from 'app/core/utils/colors';
import { hasAnsiCodes } from 'app/core/utils/text';
import { sortInAscendingOrder } from 'app/core/utils/explore';
import { getGraphSeriesModel } from 'app/plugins/panel/graph2/getGraphSeriesModel';

export const LogLevelColor = {
  [LogLevel.critical]: colors[7],
  [LogLevel.warning]: colors[1],
  [LogLevel.error]: colors[4],
  [LogLevel.info]: colors[0],
  [LogLevel.debug]: colors[5],
  [LogLevel.trace]: colors[2],
  [LogLevel.unknown]: getThemeColor('#8e8e8e', '#dde4ed'),
};

const isoDateRegexp = /\d{4}-[01]\d-[0-3]\dT[0-2]\d:[0-5]\d:[0-6]\d[,\.]\d+([+-][0-2]\d:[0-5]\d|Z)/g;
function isDuplicateRow(row: LogRowModel, other: LogRowModel, strategy: LogsDedupStrategy): boolean {
  switch (strategy) {
    case LogsDedupStrategy.exact:
      // Exact still strips dates
      return row.entry.replace(isoDateRegexp, '') === other.entry.replace(isoDateRegexp, '');

    case LogsDedupStrategy.numbers:
      return row.entry.replace(/\d/g, '') === other.entry.replace(/\d/g, '');

    case LogsDedupStrategy.signature:
      return row.entry.replace(/\w/g, '') === other.entry.replace(/\w/g, '');

    default:
      return false;
  }
}

export function dedupLogRows(logs: LogsModel, strategy: LogsDedupStrategy): LogsModel {
  if (strategy === LogsDedupStrategy.none) {
    return logs;
  }

  const dedupedRows = logs.rows.reduce((result: LogRowModel[], row: LogRowModel, index, list) => {
    const rowCopy = { ...row };
    const previous = result[result.length - 1];
    if (index > 0 && isDuplicateRow(row, previous, strategy)) {
      previous.duplicates++;
    } else {
      rowCopy.duplicates = 0;
      result.push(rowCopy);
    }
    return result;
  }, []);

  return {
    ...logs,
    rows: dedupedRows,
  };
}

export function filterLogLevels(logs: LogsModel, hiddenLogLevels: Set<LogLevel>): LogsModel {
  if (hiddenLogLevels.size === 0) {
    return logs;
  }

  const filteredRows = logs.rows.reduce((result: LogRowModel[], row: LogRowModel, index, list) => {
    if (!hiddenLogLevels.has(row.logLevel)) {
      result.push(row);
    }
    return result;
  }, []);

  return {
    ...logs,
    rows: filteredRows,
  };
}

export function makeSeriesForLogs(rows: LogRowModel[], intervalMs: number): GraphSeriesXY[] {
  // currently interval is rangeMs / resolution, which is too low for showing series as bars.
  // need at least 10px per bucket, so we multiply interval by 10. Should be solved higher up the chain
  // when executing queries & interval calculated and not here but this is a temporary fix.
  // intervalMs = intervalMs * 10;

  // Graph time series by log level
  const seriesByLevel: any = {};
  const bucketSize = intervalMs * 10;
  const seriesList: any[] = [];

  const sortedRows = rows.sort(sortInAscendingOrder);
  for (const row of sortedRows) {
    let series = seriesByLevel[row.logLevel];

    if (!series) {
      seriesByLevel[row.logLevel] = series = {
        lastTs: null,
        datapoints: [],
        alias: row.logLevel,
        color: LogLevelColor[row.logLevel],
      };

      seriesList.push(series);
    }

    // align time to bucket size - used Math.floor for calculation as time of the bucket
    // must be in the past (before Date.now()) to be displayed on the graph
    const time = Math.floor(row.timeEpochMs / bucketSize) * bucketSize;

    // Entry for time
    if (time === series.lastTs) {
      series.datapoints[series.datapoints.length - 1][0]++;
    } else {
      series.datapoints.push([1, time]);
      series.lastTs = time;
    }

    // add zero to other levels to aid stacking so each level series has same number of points
    for (const other of seriesList) {
      if (other !== series && other.lastTs !== time) {
        other.datapoints.push([0, time]);
        other.lastTs = time;
      }
    }
  }

  return seriesList.map(series => {
    series.datapoints.sort((a: number[], b: number[]) => {
      return a[1] - b[1];
    });

    // EEEP: converts GraphSeriesXY to DataFrame and back again!
    const data = toDataFrame(series);
    const points = getFlotPairs({
      xField: data.fields[1],
      yField: data.fields[0],
      nullValueMode: NullValueMode.Null,
    });

    const graphSeries: GraphSeriesXY = {
      color: series.color,
      label: series.alias,
      data: points,
      isVisible: true,
      yAxis: {
        index: 1,
        min: 0,
        tickDecimals: 0,
      },
    };

    return graphSeries;
  });
}

function isLogsData(series: DataFrame) {
  return series.fields.some(f => f.type === FieldType.time) && series.fields.some(f => f.type === FieldType.string);
}

export function dataFrameToLogsModel(dataFrame: DataFrame[], intervalMs: number): LogsModel {
  const metricSeries: DataFrame[] = [];
  const logSeries: DataFrame[] = [];

  for (const series of dataFrame) {
    if (isLogsData(series)) {
      logSeries.push(series);
      continue;
    }

    metricSeries.push(series);
  }

  const logsModel = logSeriesToLogsModel(logSeries);
  if (logsModel) {
    if (metricSeries.length === 0) {
      logsModel.series = makeSeriesForLogs(logsModel.rows, intervalMs);
    } else {
      logsModel.series = getGraphSeriesModel(
        metricSeries,
        {},
        { showBars: true, showLines: false, showPoints: false },
        {
          asTable: false,
          isVisible: true,
          placement: 'under',
        }
      );
    }

    return logsModel;
  }

  return {
    hasUniqueLabels: false,
    rows: [],
    meta: [],
    series: [],
  };
}

export function logSeriesToLogsModel(logSeries: DataFrame[]): LogsModel {
  if (logSeries.length === 0) {
    return undefined;
  }

  const allLabels: Labels[] = [];
  for (let n = 0; n < logSeries.length; n++) {
    const series = logSeries[n];
    if (series.labels) {
      allLabels.push(series.labels);
    }
  }

  let commonLabels: Labels = {};
  if (allLabels.length > 0) {
    commonLabels = findCommonLabels(allLabels);
  }

  const rows: LogRowModel[] = [];
  let hasUniqueLabels = false;

  for (let i = 0; i < logSeries.length; i++) {
    const series = logSeries[i];
    const fieldCache = new FieldCache(series);
    const uniqueLabels = findUniqueLabels(series.labels, commonLabels);
    if (Object.keys(uniqueLabels).length > 0) {
      hasUniqueLabels = true;
    }

    const timeField = fieldCache.getFirstFieldOfType(FieldType.time);
    const stringField = fieldCache.getFirstFieldOfType(FieldType.string);
    const logLevelField = fieldCache.getFieldByName('level');

    let seriesLogLevel: LogLevel | undefined = undefined;
    if (series.labels && Object.keys(series.labels).indexOf('level') !== -1) {
      seriesLogLevel = getLogLevelFromKey(series.labels['level']);
    }

    for (let j = 0; j < series.length; j++) {
      const ts = timeField.values.get(j);
      const time = dateTime(ts);
      const timeEpochMs = time.valueOf();
      const timeFromNow = time.fromNow();
      const timeLocal = time.format('YYYY-MM-DD HH:mm:ss');
      const timeUtc = toUtc(ts).format('YYYY-MM-DD HH:mm:ss');

      let message = stringField.values.get(j);
      // This should be string but sometimes isn't (eg elastic) because the dataFrame is not strongly typed.
      message = typeof message === 'string' ? message : JSON.stringify(message);

      let logLevel = LogLevel.unknown;
      if (logLevelField) {
        logLevel = getLogLevelFromKey(logLevelField.values.get(j));
      } else if (seriesLogLevel) {
        logLevel = seriesLogLevel;
      } else {
        logLevel = getLogLevel(message);
      }
      const hasAnsi = hasAnsiCodes(message);
      const searchWords = series.meta && series.meta.searchWords ? series.meta.searchWords : [];

      rows.push({
        logLevel,
        timeFromNow,
        timeEpochMs,
        timeLocal,
        timeUtc,
        uniqueLabels,
        hasAnsi,
        searchWords,
        entry: hasAnsi ? ansicolor.strip(message) : message,
        raw: message,
        labels: series.labels,
        timestamp: ts,
      });
    }
  }

  // Meta data to display in status
  const meta: LogsMetaItem[] = [];
  if (_.size(commonLabels) > 0) {
    meta.push({
      label: 'Common labels',
      value: commonLabels,
      kind: LogsMetaKind.LabelsMap,
    });
  }

  const limits = logSeries.filter(series => series.meta && series.meta.limit);

  if (limits.length > 0) {
    meta.push({
      label: 'Limit',
      value: `${limits[0].meta.limit} (${rows.length} returned)`,
      kind: LogsMetaKind.String,
    });
  }

  return {
    hasUniqueLabels,
    meta,
    rows,
  };
}
