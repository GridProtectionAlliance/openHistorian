import React, { useMemo, useState } from 'react';
import { useAsyncFn } from 'react-use';

import { SelectableValue, toOption } from '@grafana/data';
import { AccessoryButton, EditorList, InputGroup } from '@grafana/experimental';
import { Select } from '@grafana/ui';

import { COMPARISON_OPERATORS, EQUALS } from '../../cloudwatch-sql/language';
import { CloudWatchDatasource } from '../../datasource';
import { QueryEditorExpressionType, QueryEditorOperatorExpression, QueryEditorPropertyType } from '../../expressions';
import { useDimensionKeys } from '../../hooks';
import { CloudWatchMetricsQuery } from '../../types';
import { appendTemplateVariables } from '../../utils/utils';

import {
  getFlattenedFilters,
  getMetricNameFromExpression,
  getNamespaceFromExpression,
  sanitizeOperator,
  setOperatorExpressionName,
  setOperatorExpressionProperty,
  setOperatorExpressionValue,
  setSql,
} from './utils';

interface SQLFilterProps {
  query: CloudWatchMetricsQuery;
  datasource: CloudWatchDatasource;
  onQueryChange: (query: CloudWatchMetricsQuery) => void;
}

const OPERATORS = COMPARISON_OPERATORS.map(toOption);

const SQLFilter: React.FC<SQLFilterProps> = ({ query, onQueryChange, datasource }) => {
  const filtersFromQuery = useMemo(() => getFlattenedFilters(query.sql ?? {}), [query.sql]);
  const [filters, setFilters] = useState<QueryEditorOperatorExpression[]>(filtersFromQuery);

  const onChange = (newItems: Array<Partial<QueryEditorOperatorExpression>>) => {
    // As new (empty object) items come in, with need to make sure they have the correct type
    const cleaned = newItems.map(
      (v): QueryEditorOperatorExpression => ({
        type: QueryEditorExpressionType.Operator,
        property: v.property ?? { type: QueryEditorPropertyType.String },
        operator: v.operator ?? {
          name: EQUALS,
        },
      })
    );

    setFilters(cleaned);

    // Only save valid and complete filters into the query state
    const validExpressions: QueryEditorOperatorExpression[] = [];
    for (const operatorExpression of cleaned) {
      const validated = sanitizeOperator(operatorExpression);
      if (validated) {
        validExpressions.push(validated);
      }
    }

    const where = validExpressions.length
      ? {
          type: QueryEditorExpressionType.And as const,
          expressions: validExpressions,
        }
      : undefined;

    onQueryChange(setSql(query, { where }));
  };

  return <EditorList items={filters} onChange={onChange} renderItem={makeRenderFilter(datasource, query)} />;
};

// Making component functions in the render body is not recommended, but it works for now.
// If some problems arise (perhaps with state going missing), consider this to be a potential cause
function makeRenderFilter(datasource: CloudWatchDatasource, query: CloudWatchMetricsQuery) {
  function renderFilter(
    item: Partial<QueryEditorOperatorExpression>,
    onChange: (item: QueryEditorOperatorExpression) => void,
    onDelete: () => void
  ) {
    return <FilterItem datasource={datasource} query={query} filter={item} onChange={onChange} onDelete={onDelete} />;
  }

  return renderFilter;
}

export default SQLFilter;

interface FilterItemProps {
  datasource: CloudWatchDatasource;
  query: CloudWatchMetricsQuery;
  filter: Partial<QueryEditorOperatorExpression>;
  onChange: (item: QueryEditorOperatorExpression) => void;
  onDelete: () => void;
}

const FilterItem: React.FC<FilterItemProps> = (props) => {
  const { datasource, query, filter, onChange, onDelete } = props;
  const sql = query.sql ?? {};

  const namespace = getNamespaceFromExpression(sql.from);
  const metricName = getMetricNameFromExpression(sql.select);

  const dimensionKeys = useDimensionKeys(datasource, query.region, namespace, metricName);

  const loadDimensionValues = async () => {
    if (!filter.property?.name) {
      return [];
    }

    return datasource
      .getDimensionValues(query.region, namespace, metricName, filter.property.name, {})
      .then((result: Array<SelectableValue<string>>) => {
        return appendTemplateVariables(datasource, result);
      });
  };

  const [state, loadOptions] = useAsyncFn(loadDimensionValues, [
    query.region,
    namespace,
    metricName,
    filter.property?.name,
  ]);

  return (
    <InputGroup>
      <Select
        width="auto"
        value={filter.property?.name ? toOption(filter.property?.name) : null}
        options={dimensionKeys}
        allowCustomValue
        onChange={({ value }) => value && onChange(setOperatorExpressionProperty(filter, value))}
      />

      <Select
        width="auto"
        value={filter.operator?.name && toOption(filter.operator.name)}
        options={OPERATORS}
        onChange={({ value }) => value && onChange(setOperatorExpressionName(filter, value))}
      />

      <Select
        width="auto"
        isLoading={state.loading}
        value={
          filter.operator?.value && typeof filter.operator?.value === 'string' ? toOption(filter.operator?.value) : null
        }
        options={state.value}
        allowCustomValue
        onOpenMenu={loadOptions}
        onChange={({ value }) => value && onChange(setOperatorExpressionValue(filter, value))}
      />

      <AccessoryButton aria-label="remove" icon="times" variant="secondary" onClick={onDelete} />
    </InputGroup>
  );
};
