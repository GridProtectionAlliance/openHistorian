import { isEqual } from 'lodash';
import React, { useEffect, useState } from 'react';

import { SelectableValue } from '@grafana/data';
import { EditorFieldGroup, EditorField, EditorList } from '@grafana/experimental';

import { QueryBuilderLabelFilter } from '../shared/types';

import { LabelFilterItem } from './LabelFilterItem';

export const MISSING_LABEL_FILTER_ERROR_MESSAGE = 'Select at least 1 label filter (label and value)';

export interface Props {
  labelsFilters: QueryBuilderLabelFilter[];
  onChange: (labelFilters: QueryBuilderLabelFilter[]) => void;
  onGetLabelNames: (forLabel: Partial<QueryBuilderLabelFilter>) => Promise<SelectableValue[]>;
  onGetLabelValues: (forLabel: Partial<QueryBuilderLabelFilter>) => Promise<SelectableValue[]>;
  /** If set to true, component will show error message until at least 1 filter is selected */
  labelFilterRequired?: boolean;
}

export function LabelFilters({
  labelsFilters,
  onChange,
  onGetLabelNames,
  onGetLabelValues,
  labelFilterRequired,
}: Props) {
  const defaultOp = '=';
  const [items, setItems] = useState<Array<Partial<QueryBuilderLabelFilter>>>([{ op: defaultOp }]);

  useEffect(() => {
    if (labelsFilters.length > 0) {
      setItems(labelsFilters);
    } else {
      setItems([{ op: defaultOp }]);
    }
  }, [labelsFilters]);

  const onLabelsChange = (newItems: Array<Partial<QueryBuilderLabelFilter>>) => {
    setItems(newItems);

    // Extract full label filters with both label & value
    const newLabels = newItems.filter((x) => x.label != null && x.value != null);
    if (!isEqual(newLabels, labelsFilters)) {
      onChange(newLabels as QueryBuilderLabelFilter[]);
    }
  };

  const hasLabelFilter = items.some((item) => item.label && item.value);

  return (
    <EditorFieldGroup>
      <EditorField
        label="Label filters"
        error={MISSING_LABEL_FILTER_ERROR_MESSAGE}
        invalid={labelFilterRequired && !hasLabelFilter}
      >
        <EditorList
          items={items}
          onChange={onLabelsChange}
          renderItem={(item: Partial<QueryBuilderLabelFilter>, onChangeItem, onDelete) => (
            <LabelFilterItem
              item={item}
              items={items}
              defaultOp={defaultOp}
              onChange={onChangeItem}
              onDelete={onDelete}
              onGetLabelNames={onGetLabelNames}
              onGetLabelValues={onGetLabelValues}
              invalidLabel={labelFilterRequired && !item.label}
              invalidValue={labelFilterRequired && !item.value}
            />
          )}
        />
      </EditorField>
    </EditorFieldGroup>
  );
}
