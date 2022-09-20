import React, { FormEvent, PureComponent } from 'react';

import { selectors } from '@grafana/e2e-selectors';
import { VerticalGroup } from '@grafana/ui';

import { VariableSectionHeader } from '../editor/VariableSectionHeader';
import { VariableTextField } from '../editor/VariableTextField';
import { VariableEditorProps } from '../editor/types';
import { ConstantVariableModel } from '../types';

export interface Props extends VariableEditorProps<ConstantVariableModel> {}

export class ConstantVariableEditor extends PureComponent<Props> {
  onChange = (event: FormEvent<HTMLInputElement>) => {
    this.props.onPropChange({
      propName: 'query',
      propValue: event.currentTarget.value,
    });
  };

  onBlur = (event: FormEvent<HTMLInputElement>) => {
    this.props.onPropChange({
      propName: 'query',
      propValue: event.currentTarget.value,
      updateOptions: true,
    });
  };

  render() {
    return (
      <VerticalGroup spacing="xs">
        <VariableSectionHeader name="Constant options" />
        <VariableTextField
          value={this.props.variable.query}
          name="Value"
          placeholder="your metric prefix"
          onChange={this.onChange}
          onBlur={this.onBlur}
          labelWidth={20}
          testId={selectors.pages.Dashboard.Settings.Variables.Edit.ConstantVariable.constantOptionsQueryInputV2}
          grow
        />
      </VerticalGroup>
    );
  }
}
