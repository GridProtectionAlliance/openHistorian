import React from 'react';

import { DataSourcePluginOptionsEditorProps } from '@grafana/data';
import { DataSourceHttpSettings } from '@grafana/ui';

import { OpenTsdbOptions } from '../types';

import { OpenTsdbDetails } from './OpenTsdbDetails';

export const ConfigEditor = (props: DataSourcePluginOptionsEditorProps<OpenTsdbOptions>) => {
  const { options, onOptionsChange } = props;

  return (
    <>
      <DataSourceHttpSettings
        defaultUrl="http://localhost:4242"
        dataSourceConfig={options}
        onChange={onOptionsChange}
      />
      <OpenTsdbDetails value={options} onChange={onOptionsChange} />
    </>
  );
};
