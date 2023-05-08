import { css } from '@emotion/css';
import React from 'react';

import { PanelData, QueryResultMetaStat, TimeZone } from '@grafana/data';
import { selectors } from '@grafana/e2e-selectors';
import { t } from 'app/core/internationalization';

import { InspectStatsTable } from './InspectStatsTable';

interface InspectStatsTabProps {
  data: PanelData;
  timeZone: TimeZone;
}

export const InspectStatsTab = ({ data, timeZone }: InspectStatsTabProps) => {
  if (!data.request) {
    return null;
  }
  let stats: QueryResultMetaStat[] = [];

  const requestTime = data.request.endTime ? data.request.endTime - data.request.startTime : -1;
  const processingTime = data.timings?.dataProcessingTime || -1;
  let dataRows = 0;

  for (const frame of data.series) {
    dataRows += frame.length;
  }

  if (requestTime > 0) {
    stats.push({
      displayName: t('dashboard.inspect-stats.request-time', 'Total request time'),
      value: requestTime,
      unit: 'ms',
    });
  }
  if (processingTime > 0) {
    stats.push({
      displayName: t('dashboard.inspect-stats.processing-time', 'Data processing time'),
      value: processingTime,
      unit: 'ms',
    });
  }
  stats.push({
    displayName: t('dashboard.inspect-stats.queries', 'Number of queries'),
    value: data.request.targets.length,
  });
  stats.push({
    displayName: t('dashboard.inspect-stats.rows', 'Total number rows'),
    value: dataRows,
  });

  let dataStats: QueryResultMetaStat[] = [];

  for (const series of data.series) {
    if (series.meta && series.meta.stats) {
      dataStats = dataStats.concat(series.meta.stats);
    }
  }

  const statsTableName = t('dashboard.inspect-stats.table-title', 'Stats');
  const dataStatsTableName = t('dashboard.inspect-stats.data-title', 'Data source stats');

  return (
    <div aria-label={selectors.components.PanelInspector.Stats.content} className={containerStyles}>
      <InspectStatsTable timeZone={timeZone} name={statsTableName} stats={stats} />
      <InspectStatsTable timeZone={timeZone} name={dataStatsTableName} stats={dataStats} />
    </div>
  );
};

const containerStyles = css`
  height: 100%;
  overflow-y: scroll;
`;
