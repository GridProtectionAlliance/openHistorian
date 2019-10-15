// Libraries
import React from 'react';
// @ts-ignore
import Cascader from 'rc-cascader';

import { SlatePrism } from '@grafana/ui';

// Components
import QueryField, { TypeaheadInput } from 'app/features/explore/QueryField';
// Utils & Services
// dom also includes Element polyfills
import BracesPlugin from 'app/features/explore/slate-plugins/braces';
import { Plugin, Node } from 'slate';

// Types
import { LokiQuery } from '../types';
import { TypeaheadOutput } from 'app/types/explore';
import { DataSourceApi, ExploreQueryFieldProps, DataSourceStatus, DOMUtil } from '@grafana/ui';
import { AbsoluteTimeRange } from '@grafana/data';
import { Grammar } from 'prismjs';
import LokiLanguageProvider, { LokiHistoryItem } from '../language_provider';
import { SuggestionsState } from 'app/features/explore/slate-plugins/suggestions';

function getChooserText(hasSyntax: boolean, hasLogLabels: boolean, datasourceStatus: DataSourceStatus) {
  if (datasourceStatus === DataSourceStatus.Disconnected) {
    return '(Disconnected)';
  }
  if (!hasSyntax) {
    return 'Loading labels...';
  }
  if (!hasLogLabels) {
    return '(No labels found)';
  }
  return 'Log labels';
}

function willApplySuggestion(suggestion: string, { typeaheadContext, typeaheadText }: SuggestionsState): string {
  // Modify suggestion based on context
  switch (typeaheadContext) {
    case 'context-labels': {
      const nextChar = DOMUtil.getNextCharacter();
      if (!nextChar || nextChar === '}' || nextChar === ',') {
        suggestion += '=';
      }
      break;
    }

    case 'context-label-values': {
      // Always add quotes and remove existing ones instead
      if (!typeaheadText.match(/^(!?=~?"|")/)) {
        suggestion = `"${suggestion}`;
      }
      if (DOMUtil.getNextCharacter() !== '"') {
        suggestion = `${suggestion}"`;
      }
      break;
    }

    default:
  }
  return suggestion;
}

export interface CascaderOption {
  label: string;
  value: string;
  children?: CascaderOption[];
  disabled?: boolean;
}

export interface LokiQueryFieldFormProps extends ExploreQueryFieldProps<DataSourceApi<LokiQuery>, LokiQuery> {
  history: LokiHistoryItem[];
  syntax: Grammar;
  logLabelOptions: any[];
  syntaxLoaded: boolean;
  absoluteRange: AbsoluteTimeRange;
  onLoadOptions: (selectedOptions: CascaderOption[]) => void;
  onLabelsRefresh?: () => void;
}

export class LokiQueryFieldForm extends React.PureComponent<LokiQueryFieldFormProps> {
  plugins: Plugin[];
  modifiedSearch: string;
  modifiedQuery: string;

  constructor(props: LokiQueryFieldFormProps, context: React.Context<any>) {
    super(props, context);

    this.plugins = [
      BracesPlugin(),
      SlatePrism({
        onlyIn: (node: Node) => node.object === 'block' && node.type === 'code_block',
        getSyntax: (node: Node) => 'promql',
      }),
    ];
  }

  loadOptions = (selectedOptions: CascaderOption[]) => {
    this.props.onLoadOptions(selectedOptions);
  };

  onChangeLogLabels = (values: string[], selectedOptions: CascaderOption[]) => {
    if (selectedOptions.length === 2) {
      const key = selectedOptions[0].value;
      const value = selectedOptions[1].value;
      const query = `{${key}="${value}"}`;
      this.onChangeQuery(query, true);
    }
  };

  onChangeQuery = (value: string, override?: boolean) => {
    // Send text change to parent
    const { query, onChange, onRunQuery } = this.props;
    if (onChange) {
      const nextQuery = { ...query, expr: value };
      onChange(nextQuery);

      if (override && onRunQuery) {
        onRunQuery();
      }
    }
  };

  onTypeahead = async (typeahead: TypeaheadInput): Promise<TypeaheadOutput> => {
    const { datasource } = this.props;

    if (!datasource.languageProvider) {
      return { suggestions: [] };
    }

    const lokiLanguageProvider = datasource.languageProvider as LokiLanguageProvider;
    const { history, absoluteRange } = this.props;
    const { prefix, text, value, wrapperClasses, labelKey } = typeahead;

    const result = await lokiLanguageProvider.provideCompletionItems(
      { text, value, prefix, wrapperClasses, labelKey },
      { history, absoluteRange }
    );

    //console.log('handleTypeahead', wrapperClasses, text, prefix, nextChar, labelKey, result.context);

    return result;
  };

  render() {
    const {
      queryResponse,
      query,
      syntaxLoaded,
      logLabelOptions,
      onLoadOptions,
      onLabelsRefresh,
      datasource,
      datasourceStatus,
    } = this.props;
    const lokiLanguageProvider = datasource.languageProvider as LokiLanguageProvider;
    const cleanText = datasource.languageProvider ? lokiLanguageProvider.cleanText : undefined;
    const hasLogLabels = logLabelOptions && logLabelOptions.length > 0;
    const chooserText = getChooserText(syntaxLoaded, hasLogLabels, datasourceStatus);
    const buttonDisabled = !syntaxLoaded || datasourceStatus === DataSourceStatus.Disconnected;
    const showError = queryResponse && queryResponse.error && queryResponse.error.refId === query.refId;

    return (
      <>
        <div className="gf-form-inline">
          <div className="gf-form">
            <Cascader
              options={logLabelOptions}
              onChange={this.onChangeLogLabels}
              loadData={onLoadOptions}
              expandIcon={null}
              onPopupVisibleChange={(isVisible: boolean) => {
                if (isVisible && onLabelsRefresh) {
                  onLabelsRefresh();
                }
              }}
            >
              <button className="gf-form-label gf-form-label--btn" disabled={buttonDisabled}>
                {chooserText} <i className="fa fa-caret-down" />
              </button>
            </Cascader>
          </div>
          <div className="gf-form gf-form--grow">
            <QueryField
              additionalPlugins={this.plugins}
              cleanText={cleanText}
              initialQuery={query.expr}
              onTypeahead={this.onTypeahead}
              onWillApplySuggestion={willApplySuggestion}
              onChange={this.onChangeQuery}
              onRunQuery={this.props.onRunQuery}
              placeholder="Enter a Loki query"
              portalOrigin="loki"
              syntaxLoaded={syntaxLoaded}
            />
          </div>
        </div>
        <div>
          {showError ? <div className="prom-query-field-info text-error">{queryResponse.error.message}</div> : null}
        </div>
      </>
    );
  }
}
