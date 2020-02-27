import React, { FC } from 'react';
import { FormLabel, Input, Switch } from '@grafana/ui';
import { e2e } from '@grafana/e2e';

export interface Props {
  dataSourceName: string;
  isDefault: boolean;
  onNameChange: (name: string) => void;
  onDefaultChange: (value: boolean) => void;
}

const BasicSettings: FC<Props> = ({ dataSourceName, isDefault, onDefaultChange, onNameChange }) => {
  return (
    <div className="gf-form-group" aria-label="Datasource settings page basic settings">
      <div className="gf-form-inline">
        <div className="gf-form max-width-30" style={{ marginRight: '3px' }}>
          <FormLabel
            tooltip={
              'The name is used when you select the data source in panels. The Default data source is ' +
              'preselected in new panels.'
            }
          >
            Name
          </FormLabel>
          <Input
            className="gf-form-input max-width-23"
            type="text"
            value={dataSourceName}
            placeholder="Name"
            onChange={event => onNameChange(event.target.value)}
            required
            aria-label={e2e.pages.DataSource.selectors.name}
          />
        </div>
        <Switch
          label="Default"
          checked={isDefault}
          onChange={event => {
            // @ts-ignore
            onDefaultChange(event.target.checked);
          }}
        />
      </div>
    </div>
  );
};

export default BasicSettings;
