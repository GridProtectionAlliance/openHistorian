import React, { FC, ReactElement } from 'react';

import { DataSourceRef, SelectableValue } from '@grafana/data';
import { Icon, SegmentAsync } from '@grafana/ui';

import { getDatasourceSrv } from '../../../plugins/datasource_srv';

interface Props {
  datasource: DataSourceRef;
  filterKey: string | null;
  onChange: (item: SelectableValue<string | null>) => void;
  getTagKeysOptions?: any;
  disabled?: boolean;
}

const MIN_WIDTH = 90;
export const AdHocFilterKey: FC<Props> = ({ datasource, onChange, disabled, filterKey, getTagKeysOptions }) => {
  const loadKeys = () => fetchFilterKeys(datasource, getTagKeysOptions);
  const loadKeysWithRemove = () => fetchFilterKeysWithRemove(datasource, getTagKeysOptions);

  if (filterKey === null) {
    return (
      <div className="gf-form" data-testid="AdHocFilterKey-add-key-wrapper">
        <SegmentAsync
          disabled={disabled}
          className="query-segment-key"
          Component={plusSegment}
          value={filterKey}
          onChange={onChange}
          loadOptions={loadKeys}
          inputMinWidth={MIN_WIDTH}
        />
      </div>
    );
  }

  return (
    <div className="gf-form" data-testid="AdHocFilterKey-key-wrapper">
      <SegmentAsync
        disabled={disabled}
        className="query-segment-key"
        value={filterKey}
        onChange={onChange}
        loadOptions={loadKeysWithRemove}
        inputMinWidth={MIN_WIDTH}
      />
    </div>
  );
};

export const REMOVE_FILTER_KEY = '-- remove filter --';
const REMOVE_VALUE = { label: REMOVE_FILTER_KEY, value: REMOVE_FILTER_KEY };

const plusSegment: ReactElement = (
  <a className="gf-form-label query-part" aria-label="Add Filter">
    <Icon name="plus" />
  </a>
);

const fetchFilterKeys = async (
  datasource: DataSourceRef,
  getTagKeysOptions?: any
): Promise<Array<SelectableValue<string>>> => {
  const ds = await getDatasourceSrv().get(datasource);

  if (!ds || !ds.getTagKeys) {
    return [];
  }

  const metrics = await ds.getTagKeys(getTagKeysOptions);
  return metrics.map((m) => ({ label: m.text, value: m.text }));
};

const fetchFilterKeysWithRemove = async (
  datasource: DataSourceRef,
  getTagKeysOptions?: any
): Promise<Array<SelectableValue<string>>> => {
  const keys = await fetchFilterKeys(datasource, getTagKeysOptions);
  return [REMOVE_VALUE, ...keys];
};
