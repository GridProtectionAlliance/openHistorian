import React, { useCallback, useEffect, useRef } from 'react';

import { DataSourceApi, QueryEditorProps, SelectableValue } from '@grafana/data';
import { InlineField, Select } from '@grafana/ui';

import { ClassicConditions } from './components/ClassicConditions';
import { Math } from './components/Math';
import { Reduce } from './components/Reduce';
import { Resample } from './components/Resample';
import { ExpressionQuery, ExpressionQueryType, gelTypes } from './types';
import { getDefaults } from './utils/expressionTypes';

type Props = QueryEditorProps<DataSourceApi<ExpressionQuery>, ExpressionQuery>;

const labelWidth = 14;

type NonClassicExpressionType = Exclude<ExpressionQueryType, ExpressionQueryType.classic>;
type ExpressionTypeConfigStorage = Partial<Record<NonClassicExpressionType, string>>;

function useExpressionsCache() {
  const expressionCache = useRef<ExpressionTypeConfigStorage>({});

  const getCachedExpression = useCallback((queryType: ExpressionQueryType) => {
    switch (queryType) {
      case ExpressionQueryType.math:
      case ExpressionQueryType.reduce:
      case ExpressionQueryType.resample:
        return expressionCache.current[queryType];
      case ExpressionQueryType.classic:
        return undefined;
    }
  }, []);

  const setCachedExpression = useCallback((queryType: ExpressionQueryType, value: string | undefined) => {
    switch (queryType) {
      case ExpressionQueryType.math:
        expressionCache.current.math = value;
        break;

      // We want to use the same value for Reduce and Resample
      case ExpressionQueryType.reduce:
      case ExpressionQueryType.resample:
        expressionCache.current.reduce = value;
        expressionCache.current.resample = value;
        break;
    }
  }, []);

  return { getCachedExpression, setCachedExpression };
}

export function ExpressionQueryEditor(props: Props) {
  const { query, queries, onRunQuery, onChange } = props;
  const { getCachedExpression, setCachedExpression } = useExpressionsCache();

  useEffect(() => {
    setCachedExpression(query.type, query.expression);
  }, [query.expression, query.type, setCachedExpression]);

  const onSelectExpressionType = useCallback(
    (item: SelectableValue<ExpressionQueryType>) => {
      const cachedExpression = getCachedExpression(item.value!);
      const defaults = getDefaults({ ...query, type: item.value! });

      onChange({ ...defaults, expression: cachedExpression ?? defaults.expression });
    },
    [query, onChange, getCachedExpression]
  );

  const renderExpressionType = () => {
    const refIds = queries!.filter((q) => query.refId !== q.refId).map((q) => ({ value: q.refId, label: q.refId }));

    switch (query.type) {
      case ExpressionQueryType.math:
        return <Math onChange={onChange} query={query} labelWidth={labelWidth} onRunQuery={onRunQuery} />;

      case ExpressionQueryType.reduce:
        return <Reduce refIds={refIds} onChange={onChange} labelWidth={labelWidth} query={query} />;

      case ExpressionQueryType.resample:
        return <Resample query={query} labelWidth={labelWidth} onChange={onChange} refIds={refIds} />;

      case ExpressionQueryType.classic:
        return <ClassicConditions onChange={onChange} query={query} refIds={refIds} />;
    }
  };

  const selected = gelTypes.find((o) => o.value === query.type);

  return (
    <div>
      <InlineField label="Operation" labelWidth={labelWidth}>
        <Select options={gelTypes} value={selected} onChange={onSelectExpressionType} width={25} />
      </InlineField>
      {renderExpressionType()}
    </div>
  );
}
