import React, { PureComponent } from 'react';
import { connect, ConnectedProps } from 'react-redux';

import { DataSourceInstanceSettings, getDataSourceRef } from '@grafana/data';
import { DataSourcePicker } from '@grafana/runtime';
import { Alert, InlineField, InlineFieldRow, VerticalGroup } from '@grafana/ui';
import { StoreState } from 'app/types';

import { VariableSectionHeader } from '../editor/VariableSectionHeader';
import { initialVariableEditorState } from '../editor/reducer';
import { getAdhocVariableEditorState } from '../editor/selectors';
import { VariableEditorProps } from '../editor/types';
import { getVariablesState } from '../state/selectors';
import { AdHocVariableModel } from '../types';
import { toKeyedVariableIdentifier } from '../utils';

import { changeVariableDatasource } from './actions';

const mapStateToProps = (state: StoreState, ownProps: OwnProps) => {
  const { rootStateKey } = ownProps.variable;

  if (!rootStateKey) {
    console.error('AdHocVariableEditor: variable has no rootStateKey');
    return {
      extended: getAdhocVariableEditorState(initialVariableEditorState),
    };
  }

  const { editor } = getVariablesState(rootStateKey, state);

  return {
    extended: getAdhocVariableEditorState(editor),
  };
};

const mapDispatchToProps = {
  changeVariableDatasource,
};

const connector = connect(mapStateToProps, mapDispatchToProps);

export interface OwnProps extends VariableEditorProps<AdHocVariableModel> {}

type Props = OwnProps & ConnectedProps<typeof connector>;

export class AdHocVariableEditorUnConnected extends PureComponent<Props> {
  componentDidMount() {
    const { rootStateKey } = this.props.variable;
    if (!rootStateKey) {
      console.error('AdHocVariableEditor: variable has no rootStateKey');
      return;
    }
  }

  onDatasourceChanged = (ds: DataSourceInstanceSettings) => {
    this.props.changeVariableDatasource(toKeyedVariableIdentifier(this.props.variable), getDataSourceRef(ds));
  };

  render() {
    const { variable, extended } = this.props;
    const infoText = extended?.infoText ?? null;

    return (
      <VerticalGroup spacing="xs">
        <VariableSectionHeader name="Options" />
        <VerticalGroup spacing="sm">
          <InlineFieldRow>
            <InlineField label="Data source" labelWidth={20} htmlFor="data-source-picker">
              <DataSourcePicker current={variable.datasource} onChange={this.onDatasourceChanged} noDefault />
            </InlineField>
          </InlineFieldRow>

          {infoText ? <Alert title={infoText} severity="info" /> : null}
        </VerticalGroup>
      </VerticalGroup>
    );
  }
}

export const AdHocVariableEditor = connector(AdHocVariableEditorUnConnected);
