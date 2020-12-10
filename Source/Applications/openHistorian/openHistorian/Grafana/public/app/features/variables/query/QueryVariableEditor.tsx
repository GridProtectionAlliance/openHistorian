import React, { ChangeEvent, PureComponent } from 'react';
import { InlineFormLabel, LegacyForms } from '@grafana/ui';
import { selectors } from '@grafana/e2e-selectors';

import { getTemplateSrv } from '@grafana/runtime';
import { SelectionOptionsEditor } from '../editor/SelectionOptionsEditor';
import { QueryVariableModel, VariableRefresh, VariableSort, VariableWithMultiSupport } from '../types';
import { QueryVariableEditorState } from './reducer';
import { changeQueryVariableDataSource, changeQueryVariableQuery, initQueryVariableEditor } from './actions';
import { VariableEditorState } from '../editor/reducer';
import { OnPropChangeArguments, VariableEditorProps } from '../editor/types';
import { MapDispatchToProps, MapStateToProps } from 'react-redux';
import { StoreState } from '../../../types';
import { connectWithStore } from '../../../core/utils/connectWithReduxStore';
import { toVariableIdentifier } from '../state/types';
import { changeVariableMultiValue } from '../state/actions';

const { Switch } = LegacyForms;

export interface OwnProps extends VariableEditorProps<QueryVariableModel> {}

interface ConnectedProps {
  editor: VariableEditorState<QueryVariableEditorState>;
}

interface DispatchProps {
  initQueryVariableEditor: typeof initQueryVariableEditor;
  changeQueryVariableDataSource: typeof changeQueryVariableDataSource;
  changeQueryVariableQuery: typeof changeQueryVariableQuery;
  changeVariableMultiValue: typeof changeVariableMultiValue;
}

export type Props = OwnProps & ConnectedProps & DispatchProps;

export interface State {
  regex: string | null;
  tagsQuery: string | null;
  tagValuesQuery: string | null;
}

export class QueryVariableEditorUnConnected extends PureComponent<Props, State> {
  state: State = {
    regex: null,
    tagsQuery: null,
    tagValuesQuery: null,
  };

  async componentDidMount() {
    await this.props.initQueryVariableEditor(toVariableIdentifier(this.props.variable));
  }

  componentDidUpdate(prevProps: Readonly<Props>): void {
    if (prevProps.variable.datasource !== this.props.variable.datasource) {
      this.props.changeQueryVariableDataSource(
        toVariableIdentifier(this.props.variable),
        this.props.variable.datasource
      );
    }
  }

  getSelectedDataSourceValue = (): string => {
    if (!this.props.editor.extended?.dataSources.length) {
      return '';
    }
    const foundItem = this.props.editor.extended?.dataSources.find(ds => ds.value === this.props.variable.datasource);
    const value = foundItem ? foundItem.value : this.props.editor.extended?.dataSources[0].value;
    return value ?? '';
  };

  onDataSourceChange = (event: ChangeEvent<HTMLSelectElement>) => {
    this.props.onPropChange({ propName: 'query', propValue: '' });
    this.props.onPropChange({ propName: 'datasource', propValue: event.target.value });
  };

  onQueryChange = async (query: any, definition: string) => {
    if (this.props.variable.query !== query) {
      this.props.changeQueryVariableQuery(toVariableIdentifier(this.props.variable), query, definition);
    }
  };

  onRegExChange = (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ regex: event.target.value });
  };

  onRegExBlur = async (event: ChangeEvent<HTMLInputElement>) => {
    const regex = event.target.value;
    if (this.props.variable.regex !== regex) {
      this.props.onPropChange({ propName: 'regex', propValue: regex, updateOptions: true });
    }
  };

  onTagsQueryChange = async (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ tagsQuery: event.target.value });
  };

  onTagsQueryBlur = async (event: ChangeEvent<HTMLInputElement>) => {
    const tagsQuery = event.target.value;
    if (this.props.variable.tagsQuery !== tagsQuery) {
      this.props.onPropChange({ propName: 'tagsQuery', propValue: tagsQuery, updateOptions: true });
    }
  };

  onTagValuesQueryChange = async (event: ChangeEvent<HTMLInputElement>) => {
    this.setState({ tagValuesQuery: event.target.value });
  };

  onTagValuesQueryBlur = async (event: ChangeEvent<HTMLInputElement>) => {
    const tagValuesQuery = event.target.value;
    if (this.props.variable.tagValuesQuery !== tagValuesQuery) {
      this.props.onPropChange({ propName: 'tagValuesQuery', propValue: tagValuesQuery, updateOptions: true });
    }
  };

  onRefreshChange = (event: ChangeEvent<HTMLSelectElement>) => {
    this.props.onPropChange({ propName: 'refresh', propValue: parseInt(event.target.value, 10) });
  };

  onSortChange = async (event: ChangeEvent<HTMLSelectElement>) => {
    this.props.onPropChange({ propName: 'sort', propValue: parseInt(event.target.value, 10), updateOptions: true });
  };

  onSelectionOptionsChange = async ({ propValue, propName }: OnPropChangeArguments<VariableWithMultiSupport>) => {
    this.props.onPropChange({ propName, propValue, updateOptions: true });
  };

  onUseTagsChange = async (event: ChangeEvent<HTMLInputElement>) => {
    this.props.onPropChange({ propName: 'useTags', propValue: event.target.checked, updateOptions: true });
  };

  render() {
    const VariableQueryEditor = this.props.editor.extended?.VariableQueryEditor;
    return (
      <>
        <div className="gf-form-group">
          <h5 className="section-heading">Query Options</h5>
          <div className="gf-form-inline">
            <div className="gf-form max-width-21">
              <span className="gf-form-label width-10">Data source</span>
              <div className="gf-form-select-wrapper max-width-14">
                <select
                  className="gf-form-input"
                  value={this.getSelectedDataSourceValue()}
                  onChange={this.onDataSourceChange}
                  required
                  aria-label={
                    selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.queryOptionsDataSourceSelect
                  }
                >
                  {this.props.editor.extended?.dataSources.length &&
                    this.props.editor.extended?.dataSources.map(ds => (
                      <option key={ds.value ?? ''} value={ds.value ?? ''} label={ds.name}>
                        {ds.name}
                      </option>
                    ))}
                </select>
              </div>
            </div>

            <div className="gf-form max-width-22">
              <InlineFormLabel width={10} tooltip={'When to update the values of this variable.'}>
                Refresh
              </InlineFormLabel>
              <div className="gf-form-select-wrapper width-15">
                <select
                  className="gf-form-input"
                  value={this.props.variable.refresh}
                  onChange={this.onRefreshChange}
                  aria-label={selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.queryOptionsRefreshSelect}
                >
                  <option label="Never" value={VariableRefresh.never}>
                    Never
                  </option>
                  <option label="On Dashboard Load" value={VariableRefresh.onDashboardLoad}>
                    On Dashboard Load
                  </option>
                  <option label="On Time Range Change" value={VariableRefresh.onTimeRangeChanged}>
                    On Time Range Change
                  </option>
                </select>
              </div>
            </div>
          </div>

          {VariableQueryEditor && this.props.editor.extended?.dataSource && (
            <VariableQueryEditor
              datasource={this.props.editor.extended?.dataSource}
              query={this.props.variable.query}
              templateSrv={getTemplateSrv()}
              onChange={this.onQueryChange}
            />
          )}

          <div className="gf-form">
            <InlineFormLabel
              width={10}
              tooltip={'Optional, if you want to extract part of a series name or metric node segment.'}
            >
              Regex
            </InlineFormLabel>
            <input
              type="text"
              className="gf-form-input"
              placeholder="/.*-(.*)-.*/"
              value={this.state.regex ?? this.props.variable.regex}
              onChange={this.onRegExChange}
              onBlur={this.onRegExBlur}
              aria-label={selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.queryOptionsRegExInput}
            />
          </div>
          <div className="gf-form max-width-21">
            <InlineFormLabel width={10} tooltip={'How to sort the values of this variable.'}>
              Sort
            </InlineFormLabel>
            <div className="gf-form-select-wrapper max-width-14">
              <select
                className="gf-form-input"
                value={this.props.variable.sort}
                onChange={this.onSortChange}
                aria-label={selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.queryOptionsSortSelect}
              >
                <option label="Disabled" value={VariableSort.disabled}>
                  Disabled
                </option>
                <option label="Alphabetical (asc)" value={VariableSort.alphabeticalAsc}>
                  Alphabetical (asc)
                </option>
                <option label="Alphabetical (desc)" value={VariableSort.alphabeticalDesc}>
                  Alphabetical (desc)
                </option>
                <option label="Numerical (asc)" value={VariableSort.numericalAsc}>
                  Numerical (asc)
                </option>
                <option label="Numerical (desc)" value={VariableSort.numericalDesc}>
                  Numerical (desc)
                </option>
                <option
                  label="Alphabetical (case-insensitive, asc)"
                  value={VariableSort.alphabeticalCaseInsensitiveAsc}
                >
                  Alphabetical (case-insensitive, asc)
                </option>
                <option
                  label="Alphabetical (case-insensitive, desc)"
                  value={VariableSort.alphabeticalCaseInsensitiveDesc}
                >
                  Alphabetical (case-insensitive, desc)
                </option>
              </select>
            </div>
          </div>
        </div>

        <SelectionOptionsEditor
          variable={this.props.variable}
          onPropChange={this.onSelectionOptionsChange}
          onMultiChanged={this.props.changeVariableMultiValue}
        />

        <div className="gf-form-group">
          <h5>Value groups/tags (Experimental feature)</h5>
          <div
            aria-label={selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.valueGroupsTagsEnabledSwitch}
          >
            <Switch
              label="Enabled"
              label-class="width-10"
              checked={this.props.variable.useTags}
              onChange={this.onUseTagsChange}
            />
          </div>
          {this.props.variable.useTags && (
            <>
              <div className="gf-form last">
                <span className="gf-form-label width-10">Tags query</span>
                <input
                  type="text"
                  className="gf-form-input"
                  value={this.state.tagsQuery ?? this.props.variable.tagsQuery}
                  placeholder="metric name or tags query"
                  onChange={this.onTagsQueryChange}
                  onBlur={this.onTagsQueryBlur}
                  aria-label={
                    selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.valueGroupsTagsTagsQueryInput
                  }
                />
              </div>
              <div className="gf-form">
                <li className="gf-form-label width-10">Tag values query</li>
                <input
                  type="text"
                  className="gf-form-input"
                  value={this.state.tagValuesQuery ?? this.props.variable.tagValuesQuery}
                  placeholder="apps.$tag.*"
                  onChange={this.onTagValuesQueryChange}
                  onBlur={this.onTagValuesQueryBlur}
                  aria-label={
                    selectors.pages.Dashboard.Settings.Variables.Edit.QueryVariable.valueGroupsTagsTagsValuesQueryInput
                  }
                />
              </div>
            </>
          )}
        </div>
      </>
    );
  }
}

const mapStateToProps: MapStateToProps<ConnectedProps, OwnProps, StoreState> = (state, ownProps) => ({
  editor: state.templating.editor as VariableEditorState<QueryVariableEditorState>,
});

const mapDispatchToProps: MapDispatchToProps<DispatchProps, OwnProps> = {
  initQueryVariableEditor,
  changeQueryVariableDataSource,
  changeQueryVariableQuery,
  changeVariableMultiValue,
};

export const QueryVariableEditor = connectWithStore(
  QueryVariableEditorUnConnected,
  mapStateToProps,
  mapDispatchToProps
);
