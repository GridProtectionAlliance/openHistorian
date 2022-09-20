import React, { FunctionComponent, useMemo } from 'react';

import { SelectableValue } from '@grafana/data';
import { MultiSelect } from '@grafana/ui';

import { INPUT_WIDTH, SYSTEM_LABELS } from '../constants';
import { labelsToGroupedOptions } from '../functions';
import { MetricDescriptor, MetricQuery } from '../types';

import { Aggregation, QueryEditorRow } from '.';

export interface Props {
  refId: string;
  variableOptionGroup: SelectableValue<string>;
  labels: string[];
  metricDescriptor?: MetricDescriptor;
  onChange: (query: MetricQuery) => void;
  query: MetricQuery;
}

export const GroupBy: FunctionComponent<Props> = ({
  refId,
  labels: groupBys = [],
  query,
  onChange,
  variableOptionGroup,
  metricDescriptor,
}) => {
  const options = useMemo(
    () => [variableOptionGroup, ...labelsToGroupedOptions([...groupBys, ...SYSTEM_LABELS])],
    [groupBys, variableOptionGroup]
  );

  return (
    <QueryEditorRow
      label="Group by"
      tooltip="You can reduce the amount of data returned for a metric by combining different time series. To combine multiple time series, you can specify a grouping and a function. Grouping is done on the basis of labels. The grouping function is used to combine the time series in the group into a single time series."
      htmlFor={`${refId}-group-by`}
    >
      <MultiSelect
        inputId={`${refId}-group-by`}
        width={INPUT_WIDTH}
        placeholder="Choose label"
        options={options}
        value={query.groupBys ?? []}
        onChange={(options) => {
          onChange({ ...query, groupBys: options.map((o) => o.value!) });
        }}
      ></MultiSelect>
      <Aggregation
        metricDescriptor={metricDescriptor}
        templateVariableOptions={variableOptionGroup.options}
        crossSeriesReducer={query.crossSeriesReducer}
        groupBys={query.groupBys ?? []}
        onChange={(crossSeriesReducer) => onChange({ ...query, crossSeriesReducer })}
        refId={refId}
      ></Aggregation>
    </QueryEditorRow>
  );
};
