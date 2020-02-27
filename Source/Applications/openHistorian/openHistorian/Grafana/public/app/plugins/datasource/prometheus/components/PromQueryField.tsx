import _ from 'lodash';
import React, { ReactNode } from 'react';

import { Plugin } from 'slate';
import {
  ButtonCascader,
  CascaderOption,
  SlatePrism,
  TypeaheadInput,
  TypeaheadOutput,
  QueryField,
  BracesPlugin,
} from '@grafana/ui';

import Prism from 'prismjs';

// dom also includes Element polyfills
import { PromQuery, PromOptions, PromMetricsMetadata } from '../types';
import { CancelablePromise, makePromiseCancelable } from 'app/core/utils/CancelablePromise';
import { ExploreQueryFieldProps, QueryHint, isDataFrame, toLegacyResponseData, HistoryItem } from '@grafana/data';
import { DOMUtil, SuggestionsState } from '@grafana/ui';
import { PrometheusDatasource } from '../datasource';
import PromQlLanguageProvider from '../language_provider';

const HISTOGRAM_GROUP = '__histograms__';
const PRISM_SYNTAX = 'promql';
export const RECORDING_RULES_GROUP = '__recording_rules__';

function getChooserText(hasSyntax: boolean, metrics: string[]) {
  if (!hasSyntax) {
    return 'Loading metrics...';
  }
  if (metrics && metrics.length === 0) {
    return '(No metrics found)';
  }
  return 'Metrics';
}

function addMetricsMetadata(metric: string, metadata?: PromMetricsMetadata): CascaderOption {
  const option: CascaderOption = { label: metric, value: metric };
  if (metadata && metadata[metric]) {
    const { type = '', help } = metadata[metric][0];
    option.title = [metric, type.toUpperCase(), help].join('\n');
  }
  return option;
}

export function groupMetricsByPrefix(metrics: string[], metadata?: PromMetricsMetadata): CascaderOption[] {
  // Filter out recording rules and insert as first option
  const ruleRegex = /:\w+:/;
  const ruleNames = metrics.filter(metric => ruleRegex.test(metric));
  const rulesOption = {
    label: 'Recording rules',
    value: RECORDING_RULES_GROUP,
    children: ruleNames
      .slice()
      .sort()
      .map(name => ({ label: name, value: name })),
  };

  const options = ruleNames.length > 0 ? [rulesOption] : [];

  const delimiter = '_';
  const metricsOptions = _.chain(metrics)
    .filter((metric: string) => !ruleRegex.test(metric))
    .groupBy((metric: string) => metric.split(delimiter)[0])
    .map(
      (metricsForPrefix: string[], prefix: string): CascaderOption => {
        const prefixIsMetric = metricsForPrefix.length === 1 && metricsForPrefix[0] === prefix;
        const children = prefixIsMetric ? [] : metricsForPrefix.sort().map(m => addMetricsMetadata(m, metadata));
        return {
          children,
          label: prefix,
          value: prefix,
        };
      }
    )
    .sortBy('label')
    .value();

  return [...options, ...metricsOptions];
}

export function willApplySuggestion(suggestion: string, { typeaheadContext, typeaheadText }: SuggestionsState): string {
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

interface PromQueryFieldProps extends ExploreQueryFieldProps<PrometheusDatasource, PromQuery, PromOptions> {
  history: Array<HistoryItem<PromQuery>>;
  ExtraFieldElement?: ReactNode;
}

interface PromQueryFieldState {
  metricsOptions: any[];
  syntaxLoaded: boolean;
  hint: QueryHint | null;
}

class PromQueryField extends React.PureComponent<PromQueryFieldProps, PromQueryFieldState> {
  plugins: Plugin[];
  languageProvider: PromQlLanguageProvider;
  languageProviderInitializationPromise: CancelablePromise<any>;

  constructor(props: PromQueryFieldProps, context: React.Context<any>) {
    super(props, context);

    if (props.datasource.languageProvider) {
      this.languageProvider = props.datasource.languageProvider;
    }

    this.plugins = [
      BracesPlugin(),
      SlatePrism({
        onlyIn: (node: any) => node.type === 'code_block',
        getSyntax: (node: any) => 'promql',
      }),
    ];

    this.state = {
      metricsOptions: [],
      syntaxLoaded: false,
      hint: null,
    };
  }

  componentDidMount() {
    if (this.languageProvider) {
      Prism.languages[PRISM_SYNTAX] = this.languageProvider.syntax;
      this.refreshMetrics(makePromiseCancelable(this.languageProvider.start()));
    }
    this.refreshHint();
  }

  componentWillUnmount() {
    if (this.languageProviderInitializationPromise) {
      this.languageProviderInitializationPromise.cancel();
    }
  }

  componentDidUpdate(prevProps: PromQueryFieldProps) {
    const { data } = this.props;

    if (data && prevProps.data && prevProps.data.series !== data.series) {
      this.refreshHint();
    }
  }

  refreshHint = () => {
    const { datasource, query, data } = this.props;

    if (!data || data.series.length === 0) {
      this.setState({ hint: null });
      return;
    }

    const result = isDataFrame(data.series[0]) ? data.series.map(toLegacyResponseData) : data.series;
    const hints = datasource.getQueryHints(query, result);
    const hint = hints && hints.length > 0 ? hints[0] : null;
    this.setState({ hint });
  };

  refreshMetrics = (cancelablePromise: CancelablePromise<any>) => {
    this.languageProviderInitializationPromise = cancelablePromise;
    this.languageProviderInitializationPromise.promise
      .then(remaining => {
        remaining.map((task: Promise<any>) => task.then(this.onUpdateLanguage).catch(() => {}));
      })
      .then(() => this.onUpdateLanguage())
      .catch(({ isCanceled }) => {
        if (isCanceled) {
          console.warn('PromQueryField has unmounted, language provider intialization was canceled');
        }
      });
  };

  onChangeMetrics = (values: string[], selectedOptions: CascaderOption[]) => {
    let query;
    if (selectedOptions.length === 1) {
      if (selectedOptions[0].children.length === 0) {
        query = selectedOptions[0].value;
      } else {
        // Ignore click on group
        return;
      }
    } else {
      const prefix = selectedOptions[0].value;
      const metric = selectedOptions[1].value;
      if (prefix === HISTOGRAM_GROUP) {
        query = `histogram_quantile(0.95, sum(rate(${metric}[5m])) by (le))`;
      } else {
        query = metric;
      }
    }
    this.onChangeQuery(query, true);
  };

  onChangeQuery = (value: string, override?: boolean) => {
    // Send text change to parent
    const { query, onChange, onRunQuery } = this.props;
    if (onChange) {
      const nextQuery: PromQuery = { ...query, expr: value };
      onChange(nextQuery);

      if (override && onRunQuery) {
        onRunQuery();
      }
    }
  };

  onClickHintFix = () => {
    const { datasource, query, onChange, onRunQuery } = this.props;
    const { hint } = this.state;

    onChange(datasource.modifyQuery(query, hint.fix.action));
    onRunQuery();
  };

  onUpdateLanguage = () => {
    const {
      histogramMetrics,
      metrics,
      metricsMetadata,
      lookupsDisabled,
      lookupMetricsThreshold,
    } = this.languageProvider;
    if (!metrics) {
      return;
    }

    // Build metrics tree
    const metricsByPrefix = groupMetricsByPrefix(metrics, metricsMetadata);
    const histogramOptions = histogramMetrics.map((hm: any) => ({ label: hm, value: hm }));
    const metricsOptions =
      histogramMetrics.length > 0
        ? [
            { label: 'Histograms', value: HISTOGRAM_GROUP, children: histogramOptions, isLeaf: false },
            ...metricsByPrefix,
          ]
        : metricsByPrefix;

    // Hint for big disabled lookups
    let hint: QueryHint;
    if (lookupsDisabled) {
      hint = {
        label: `Dynamic label lookup is disabled for datasources with more than ${lookupMetricsThreshold} metrics.`,
        type: 'INFO',
      };
    }

    this.setState({ hint, metricsOptions, syntaxLoaded: true });
  };

  onTypeahead = async (typeahead: TypeaheadInput): Promise<TypeaheadOutput> => {
    if (!this.languageProvider) {
      return { suggestions: [] };
    }

    const { history } = this.props;
    const { prefix, text, value, wrapperClasses, labelKey } = typeahead;

    const result = await this.languageProvider.provideCompletionItems(
      { text, value, prefix, wrapperClasses, labelKey },
      { history }
    );

    // console.log('handleTypeahead', wrapperClasses, text, prefix, labelKey, result.context);

    return result;
  };

  render() {
    const { data, query, ExtraFieldElement } = this.props;
    const { metricsOptions, syntaxLoaded, hint } = this.state;
    const cleanText = this.languageProvider ? this.languageProvider.cleanText : undefined;
    const chooserText = getChooserText(syntaxLoaded, metricsOptions);
    const buttonDisabled = !(syntaxLoaded && metricsOptions && metricsOptions.length > 0);
    const showError = data && data.error && data.error.refId === query.refId;

    return (
      <>
        <div className="gf-form-inline gf-form-inline--nowrap flex-grow-1">
          <div className="gf-form flex-shrink-0">
            <ButtonCascader options={metricsOptions} disabled={buttonDisabled} onChange={this.onChangeMetrics}>
              {chooserText}
            </ButtonCascader>
          </div>
          <div className="gf-form gf-form--grow flex-shrink-1">
            <QueryField
              additionalPlugins={this.plugins}
              cleanText={cleanText}
              query={query.expr}
              onTypeahead={this.onTypeahead}
              onWillApplySuggestion={willApplySuggestion}
              onBlur={this.props.onBlur}
              onChange={this.onChangeQuery}
              onRunQuery={this.props.onRunQuery}
              placeholder="Enter a PromQL query"
              portalOrigin="prometheus"
              syntaxLoaded={syntaxLoaded}
            />
          </div>
          {ExtraFieldElement}
        </div>
        {showError ? (
          <div className="query-row-break">
            <div className="prom-query-field-info text-error">{data.error.message}</div>
          </div>
        ) : null}
        {hint ? (
          <div className="query-row-break">
            <div className="prom-query-field-info text-warning">
              {hint.label}{' '}
              {hint.fix ? (
                <a className="text-link muted" onClick={this.onClickHintFix}>
                  {hint.fix.label}
                </a>
              ) : null}
            </div>
          </div>
        ) : null}
      </>
    );
  }
}

export default PromQueryField;
