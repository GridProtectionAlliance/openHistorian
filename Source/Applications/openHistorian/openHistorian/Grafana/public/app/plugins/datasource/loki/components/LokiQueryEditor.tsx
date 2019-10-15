// Libraries
import React, { memo } from 'react';

// Types
import { AbsoluteTimeRange } from '@grafana/data';
import { QueryEditorProps, DataSourceStatus } from '@grafana/ui';
import { LokiDatasource } from '../datasource';
import { LokiQuery } from '../types';
import { LokiQueryField } from './LokiQueryField';
import { useLokiSyntax } from './useLokiSyntax';

type Props = QueryEditorProps<LokiDatasource, LokiQuery>;

export const LokiQueryEditor = memo(function LokiQueryEditor(props: Props) {
  const { query, panelData, datasource, onChange, onRunQuery } = props;

  let absolute: AbsoluteTimeRange;
  if (panelData && panelData.request) {
    const { range } = panelData.request;
    absolute = {
      from: range.from.valueOf(),
      to: range.to.valueOf(),
    };
  } else {
    absolute = {
      from: Date.now() - 10000,
      to: Date.now(),
    };
  }

  const { isSyntaxReady, setActiveOption, refreshLabels, ...syntaxProps } = useLokiSyntax(
    datasource.languageProvider,
    // TODO maybe use real status
    DataSourceStatus.Connected,
    absolute
  );

  return (
    <div>
      <LokiQueryField
        datasource={datasource}
        datasourceStatus={DataSourceStatus.Connected}
        query={query}
        onChange={onChange}
        onRunQuery={onRunQuery}
        history={[]}
        panelData={panelData}
        onLoadOptions={setActiveOption}
        onLabelsRefresh={refreshLabels}
        syntaxLoaded={isSyntaxReady}
        absoluteRange={absolute}
        {...syntaxProps}
      />
    </div>
  );
});

export default LokiQueryEditor;
