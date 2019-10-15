import _ from 'lodash';
import { TimeRange } from '@grafana/data';
import { TimeSrv } from 'app/features/dashboard/services/TimeSrv';
import { PrometheusDatasource, PromDataQueryResponse } from './datasource';
import { PromQueryRequest } from './types';

export default class PrometheusMetricFindQuery {
  range: TimeRange;

  constructor(private datasource: PrometheusDatasource, private query: string, timeSrv: TimeSrv) {
    this.datasource = datasource;
    this.query = query;
    this.range = timeSrv.timeRange();
  }

  process() {
    const labelNamesRegex = /^label_names\(\)\s*$/;
    const labelValuesRegex = /^label_values\((?:(.+),\s*)?([a-zA-Z_][a-zA-Z0-9_]*)\)\s*$/;
    const metricNamesRegex = /^metrics\((.+)\)\s*$/;
    const queryResultRegex = /^query_result\((.+)\)\s*$/;

    const labelNamesQuery = this.query.match(labelNamesRegex);
    if (labelNamesQuery) {
      return this.labelNamesQuery();
    }

    const labelValuesQuery = this.query.match(labelValuesRegex);
    if (labelValuesQuery) {
      if (labelValuesQuery[1]) {
        return this.labelValuesQuery(labelValuesQuery[2], labelValuesQuery[1]);
      } else {
        return this.labelValuesQuery(labelValuesQuery[2], null);
      }
    }

    const metricNamesQuery = this.query.match(metricNamesRegex);
    if (metricNamesQuery) {
      return this.metricNameQuery(metricNamesQuery[1]);
    }

    const queryResultQuery = this.query.match(queryResultRegex);
    if (queryResultQuery) {
      return this.queryResultQuery(queryResultQuery[1]);
    }

    // if query contains full metric name, return metric name and label list
    return this.metricNameAndLabelsQuery(this.query);
  }

  labelNamesQuery() {
    const url = '/api/v1/labels';
    return this.datasource.metadataRequest(url).then((result: any) => {
      return _.map(result.data.data, value => {
        return { text: value };
      });
    });
  }

  labelValuesQuery(label: string, metric?: string) {
    let url: string;

    if (!metric) {
      // return label values globally
      url = '/api/v1/label/' + label + '/values';

      return this.datasource.metadataRequest(url).then((result: any) => {
        return _.map(result.data.data, value => {
          return { text: value };
        });
      });
    } else {
      const start = this.datasource.getPrometheusTime(this.range.from, false);
      const end = this.datasource.getPrometheusTime(this.range.to, true);
      url = '/api/v1/series?match[]=' + encodeURIComponent(metric) + '&start=' + start + '&end=' + end;

      return this.datasource.metadataRequest(url).then((result: any) => {
        const _labels = _.map(result.data.data, metric => {
          return metric[label] || '';
        }).filter(label => {
          return label !== '';
        });

        return _.uniq(_labels).map(metric => {
          return {
            text: metric,
            expandable: true,
          };
        });
      });
    }
  }

  metricNameQuery(metricFilterPattern: string) {
    const url = '/api/v1/label/__name__/values';

    return this.datasource.metadataRequest(url).then((result: any) => {
      return _.chain(result.data.data)
        .filter(metricName => {
          const r = new RegExp(metricFilterPattern);
          return r.test(metricName);
        })
        .map(matchedMetricName => {
          return {
            text: matchedMetricName,
            expandable: true,
          };
        })
        .value();
    });
  }

  queryResultQuery(query: string) {
    const end = this.datasource.getPrometheusTime(this.range.to, true);
    const instantQuery: PromQueryRequest = { expr: query } as PromQueryRequest;
    return this.datasource.performInstantQuery(instantQuery, end).then((result: PromDataQueryResponse) => {
      return _.map(result.data.data.result, metricData => {
        let text = metricData.metric.__name__ || '';
        delete metricData.metric.__name__;
        text +=
          '{' +
          _.map(metricData.metric, (v, k) => {
            return k + '="' + v + '"';
          }).join(',') +
          '}';
        text += ' ' + metricData.value[1] + ' ' + metricData.value[0] * 1000;

        return {
          text: text,
          expandable: true,
        };
      });
    });
  }

  metricNameAndLabelsQuery(query: string) {
    const start = this.datasource.getPrometheusTime(this.range.from, false);
    const end = this.datasource.getPrometheusTime(this.range.to, true);
    const url = '/api/v1/series?match[]=' + encodeURIComponent(query) + '&start=' + start + '&end=' + end;

    const self = this;
    return this.datasource.metadataRequest(url).then((result: PromDataQueryResponse) => {
      return _.map(result.data.data, (metric: { [key: string]: string }) => {
        return {
          text: self.datasource.getOriginalMetricName(metric),
          expandable: true,
        };
      });
    });
  }
}
