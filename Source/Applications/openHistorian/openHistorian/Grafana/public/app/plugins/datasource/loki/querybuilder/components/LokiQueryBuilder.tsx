import React, { useEffect, useMemo, useState } from 'react';

import { DataSourceApi, getDefaultTimeRange, LoadingState, PanelData, SelectableValue } from '@grafana/data';
import { EditorRow } from '@grafana/experimental';
import { LabelFilters } from 'app/plugins/datasource/prometheus/querybuilder/shared/LabelFilters';
import { OperationExplainedBox } from 'app/plugins/datasource/prometheus/querybuilder/shared/OperationExplainedBox';
import { OperationList } from 'app/plugins/datasource/prometheus/querybuilder/shared/OperationList';
import { OperationListExplained } from 'app/plugins/datasource/prometheus/querybuilder/shared/OperationListExplained';
import { OperationsEditorRow } from 'app/plugins/datasource/prometheus/querybuilder/shared/OperationsEditorRow';
import { QueryBuilderHints } from 'app/plugins/datasource/prometheus/querybuilder/shared/QueryBuilderHints';
import { RawQuery } from 'app/plugins/datasource/prometheus/querybuilder/shared/RawQuery';
import {
  QueryBuilderLabelFilter,
  QueryBuilderOperation,
} from 'app/plugins/datasource/prometheus/querybuilder/shared/types';

import { LokiDatasource } from '../../datasource';
import { escapeLabelValueInSelector } from '../../language_utils';
import logqlGrammar from '../../syntax';
import { lokiQueryModeller } from '../LokiQueryModeller';
import { buildVisualQueryFromString } from '../parsing';
import { LokiOperationId, LokiVisualQuery } from '../types';

import { NestedQueryList } from './NestedQueryList';

export interface Props {
  query: LokiVisualQuery;
  datasource: LokiDatasource;
  showExplain: boolean;
  onChange: (update: LokiVisualQuery) => void;
  onRunQuery: () => void;
}

export const LokiQueryBuilder = React.memo<Props>(({ datasource, query, onChange, onRunQuery, showExplain }) => {
  const [sampleData, setSampleData] = useState<PanelData>();
  const [highlightedOp, setHighlightedOp] = useState<QueryBuilderOperation | undefined>(undefined);

  const onChangeLabels = (labels: QueryBuilderLabelFilter[]) => {
    onChange({ ...query, labels });
  };

  const withTemplateVariableOptions = async (optionsPromise: Promise<string[]>): Promise<SelectableValue[]> => {
    const options = await optionsPromise;
    return [...datasource.getVariables(), ...options].map((value) => ({ label: value, value }));
  };

  const onGetLabelNames = async (forLabel: Partial<QueryBuilderLabelFilter>): Promise<any> => {
    const labelsToConsider = query.labels.filter((x) => x !== forLabel);

    if (labelsToConsider.length === 0) {
      await datasource.languageProvider.refreshLogLabels();
      return datasource.languageProvider.getLabelKeys();
    }

    const expr = lokiQueryModeller.renderLabels(labelsToConsider);
    const series = await datasource.languageProvider.fetchSeriesLabels(expr);
    return Object.keys(series).sort();
  };

  const onGetLabelValues = async (forLabel: Partial<QueryBuilderLabelFilter>) => {
    if (!forLabel.label) {
      return [];
    }

    let values;
    const labelsToConsider = query.labels.filter((x) => x !== forLabel);
    if (labelsToConsider.length === 0) {
      values = await datasource.languageProvider.fetchLabelValues(forLabel.label);
    } else {
      const expr = lokiQueryModeller.renderLabels(labelsToConsider);
      const result = await datasource.languageProvider.fetchSeriesLabels(expr);
      values = result[datasource.interpolateString(forLabel.label)];
    }

    return values ? values.map((v) => escapeLabelValueInSelector(v, forLabel.op)) : []; // Escape values in return
  };

  const labelFilterError: string | undefined = useMemo(() => {
    const { labels, operations: op } = query;
    if (!labels.length && op.length) {
      // We don't want to show error for initial state with empty line contains operation
      if (op.length === 1 && op[0].id === LokiOperationId.LineContains && op[0].params[0] === '') {
        return undefined;
      }
      return 'You need to specify at least 1 label filter (stream selector)';
    }
    return undefined;
  }, [query]);

  useEffect(() => {
    const onGetSampleData = async () => {
      const lokiQuery = { expr: lokiQueryModeller.renderQuery(query), refId: 'data-samples' };
      const series = await datasource.getDataSamples(lokiQuery);
      const sampleData = { series, state: LoadingState.Done, timeRange: getDefaultTimeRange() };
      setSampleData(sampleData);
    };

    onGetSampleData().catch(console.error);
  }, [datasource, query]);

  const lang = { grammar: logqlGrammar, name: 'logql' };
  return (
    <>
      <EditorRow>
        <LabelFilters
          onGetLabelNames={(forLabel: Partial<QueryBuilderLabelFilter>) =>
            withTemplateVariableOptions(onGetLabelNames(forLabel))
          }
          onGetLabelValues={(forLabel: Partial<QueryBuilderLabelFilter>) =>
            withTemplateVariableOptions(onGetLabelValues(forLabel))
          }
          labelsFilters={query.labels}
          onChange={onChangeLabels}
          error={labelFilterError}
        />
      </EditorRow>
      {showExplain && (
        <OperationExplainedBox
          stepNumber={1}
          title={<RawQuery query={`${lokiQueryModeller.renderLabels(query.labels)}`} lang={lang} />}
        >
          Fetch all log lines matching label filters.
        </OperationExplainedBox>
      )}
      <OperationsEditorRow>
        <OperationList
          queryModeller={lokiQueryModeller}
          query={query}
          onChange={onChange}
          onRunQuery={onRunQuery}
          datasource={datasource as DataSourceApi}
          highlightedOp={highlightedOp}
        />
        <QueryBuilderHints<LokiVisualQuery>
          datasource={datasource}
          query={query}
          onChange={onChange}
          data={sampleData}
          queryModeller={lokiQueryModeller}
          buildVisualQueryFromString={buildVisualQueryFromString}
        />
      </OperationsEditorRow>
      {showExplain && (
        <OperationListExplained<LokiVisualQuery>
          stepNumber={2}
          queryModeller={lokiQueryModeller}
          query={query}
          lang={lang}
          onMouseEnter={(op) => {
            setHighlightedOp(op);
          }}
          onMouseLeave={() => {
            setHighlightedOp(undefined);
          }}
        />
      )}
      {query.binaryQueries && query.binaryQueries.length > 0 && (
        <NestedQueryList
          query={query}
          datasource={datasource}
          onChange={onChange}
          onRunQuery={onRunQuery}
          showExplain={showExplain}
        />
      )}
    </>
  );
});

LokiQueryBuilder.displayName = 'LokiQueryBuilder';
