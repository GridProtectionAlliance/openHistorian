import { css } from '@emotion/css';
import React from 'react';

import {
  DataSourceJsonData,
  DataSourcePluginOptionsEditorProps,
  GrafanaTheme,
  KeyValue,
  updateDatasourcePluginJsonDataOption,
} from '@grafana/data';
import { DataSourcePicker } from '@grafana/runtime';
import { Button, InlineField, InlineFieldRow, Input, useStyles } from '@grafana/ui';

import KeyValueInput from '../TraceToLogs/KeyValueInput';

export interface TraceToMetricsOptions {
  datasourceUid?: string;
  tags?: Array<KeyValue<string>>;
  queries: TraceToMetricQuery[];
}

export interface TraceToMetricQuery {
  name?: string;
  query?: string;
}

export interface TraceToMetricsData extends DataSourceJsonData {
  tracesToMetrics?: TraceToMetricsOptions;
}

interface Props extends DataSourcePluginOptionsEditorProps<TraceToMetricsData> {}

export function TraceToMetricsSettings({ options, onOptionsChange }: Props) {
  const styles = useStyles(getStyles);

  return (
    <div className={css({ width: '100%' })}>
      <h3 className="page-heading">Trace to metrics</h3>

      <div className={styles.infoText}>
        Trace to metrics lets you navigate from a trace span to the selected data source.
      </div>

      <InlineFieldRow className={styles.row}>
        <InlineField tooltip="The data source the trace is going to navigate to" label="Data source" labelWidth={26}>
          <DataSourcePicker
            inputId="trace-to-metrics-data-source-picker"
            pluginId="prometheus"
            current={options.jsonData.tracesToMetrics?.datasourceUid}
            noDefault={true}
            width={40}
            onChange={(ds) =>
              updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                ...options.jsonData.tracesToMetrics,
                datasourceUid: ds.uid,
              })
            }
          />
        </InlineField>
        {options.jsonData.tracesToMetrics?.datasourceUid ? (
          <Button
            type="button"
            variant="secondary"
            size="sm"
            fill="text"
            onClick={() => {
              updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                ...options.jsonData.tracesToMetrics,
                datasourceUid: undefined,
              });
            }}
          >
            Clear
          </Button>
        ) : null}
      </InlineFieldRow>

      <InlineFieldRow>
        <InlineField tooltip="Tags that will be used in the metrics query." label="Tags" labelWidth={26}>
          <KeyValueInput
            keyPlaceholder="Tag"
            values={options.jsonData.tracesToMetrics?.tags ?? []}
            onChange={(v) =>
              updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                ...options.jsonData.tracesToMetrics,
                tags: v,
              })
            }
          />
        </InlineField>
      </InlineFieldRow>

      {options.jsonData.tracesToMetrics?.queries?.map((query, i) => (
        <div key={i} className={styles.queryRow}>
          <InlineField label="Link Label" labelWidth={10}>
            <Input
              label="Link Label"
              type="text"
              allowFullScreen
              value={query.name}
              onChange={(e) => {
                let newQueries = options.jsonData.tracesToMetrics?.queries.slice() ?? [];
                newQueries[i].name = e.currentTarget.value;
                updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                  ...options.jsonData.tracesToMetrics,
                  queries: newQueries,
                });
              }}
            />
          </InlineField>
          <InlineField
            label="Query"
            labelWidth={10}
            tooltip="The Prometheus query that will run when navigating from a trace to metrics. Interpolate tags using the `$__tags` keyword."
            grow
          >
            <Input
              label="Query"
              type="text"
              allowFullScreen
              value={query.query}
              onChange={(e) => {
                let newQueries = options.jsonData.tracesToMetrics?.queries.slice() ?? [];
                newQueries[i].query = e.currentTarget.value;
                updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                  ...options.jsonData.tracesToMetrics,
                  queries: newQueries,
                });
              }}
            />
          </InlineField>

          <Button
            variant="destructive"
            title="Remove query"
            icon="times"
            type="button"
            onClick={() => {
              let newQueries = options.jsonData.tracesToMetrics?.queries.slice();
              newQueries?.splice(i, 1);
              updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
                ...options.jsonData.tracesToMetrics,
                queries: newQueries,
              });
            }}
          />
        </div>
      ))}

      <Button
        variant="secondary"
        title="Add query"
        icon="plus"
        type="button"
        onClick={() => {
          updateDatasourcePluginJsonDataOption({ onOptionsChange, options }, 'tracesToMetrics', {
            ...options.jsonData.tracesToMetrics,
            queries: [...(options.jsonData.tracesToMetrics?.queries ?? []), { query: '' }],
          });
        }}
      >
        Add query
      </Button>
    </div>
  );
}

const getStyles = (theme: GrafanaTheme) => ({
  infoText: css`
    padding-bottom: ${theme.spacing.md};
    color: ${theme.colors.textSemiWeak};
  `,
  row: css`
    label: row;
    align-items: baseline;
  `,
  queryRow: css`
    display: flex;
  `,
});
