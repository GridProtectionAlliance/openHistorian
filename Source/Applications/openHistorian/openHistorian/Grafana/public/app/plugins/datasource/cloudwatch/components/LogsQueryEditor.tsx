// Libraries
import React, { memo } from 'react';

// Types
import { AbsoluteTimeRange, QueryEditorProps } from '@grafana/data';
import { InlineFormLabel } from '@grafana/ui';
import { CloudWatchDatasource } from '../datasource';
import { CloudWatchLogsQuery, CloudWatchQuery } from '../types';
import { CloudWatchLogsQueryField } from './LogsQueryField';
import { useCloudWatchSyntax } from '../useCloudwatchSyntax';
import { CloudWatchLanguageProvider } from '../language_provider';
import CloudWatchLink from './CloudWatchLink';
import { css } from 'emotion';

type Props = QueryEditorProps<CloudWatchDatasource, CloudWatchQuery> & { allowCustomValue?: boolean };

const labelClass = css`
  margin-left: 3px;
  flex-grow: 0;
`;

export const CloudWatchLogsQueryEditor = memo(function CloudWatchLogsQueryEditor(props: Props) {
  const { query, data, datasource, onRunQuery, onChange, exploreId, allowCustomValue = false } = props;

  let absolute: AbsoluteTimeRange;
  if (data?.request?.range?.from) {
    const { range } = data.request;
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

  const { isSyntaxReady, syntax } = useCloudWatchSyntax(
    datasource.languageProvider as CloudWatchLanguageProvider,
    absolute
  );

  return (
    <CloudWatchLogsQueryField
      exploreId={exploreId}
      datasource={datasource}
      query={query}
      onBlur={() => {}}
      onChange={(val: CloudWatchLogsQuery) => onChange({ ...val, queryMode: 'Logs' })}
      onRunQuery={onRunQuery}
      history={[]}
      data={data}
      absoluteRange={absolute}
      syntaxLoaded={isSyntaxReady}
      syntax={syntax}
      allowCustomValue={allowCustomValue}
      ExtraFieldElement={
        <InlineFormLabel className={`gf-form-label--btn ${labelClass}`} width="auto" tooltip="Link to Graph in AWS">
          <CloudWatchLink query={query as CloudWatchLogsQuery} panelData={data} datasource={datasource} />
        </InlineFormLabel>
      }
    />
  );
});

export default CloudWatchLogsQueryEditor;
