import _ from 'lodash';

import {
  dateMath,
  DataSourceInstanceSettings,
  ScopedVars,
  DataQueryRequest,
  DataQueryResponse,
  dateTime,
  LoadingState,
  QueryResultMeta,
  MetricFindValue,
  AnnotationQueryRequest,
  AnnotationEvent,
} from '@grafana/data';
import { v4 as uuidv4 } from 'uuid';
import InfluxSeries from './influx_series';
import InfluxQueryModel from './influx_query_model';
import ResponseParser from './response_parser';
import { InfluxQueryBuilder } from './query_builder';
import { InfluxQuery, InfluxOptions, InfluxVersion } from './types';
import { getTemplateSrv, TemplateSrv } from 'app/features/templating/template_srv';
import { getBackendSrv, DataSourceWithBackend, frameToMetricFindValue } from '@grafana/runtime';
import { Observable, from } from 'rxjs';
import { FluxQueryEditor } from './components/FluxQueryEditor';

export default class InfluxDatasource extends DataSourceWithBackend<InfluxQuery, InfluxOptions> {
  type: string;
  urls: string[];
  username: string;
  password: string;
  name: string;
  database: any;
  basicAuth: any;
  withCredentials: any;
  interval: any;
  responseParser: any;
  httpMode: string;
  isFlux: boolean;

  constructor(
    instanceSettings: DataSourceInstanceSettings<InfluxOptions>,
    private readonly templateSrv: TemplateSrv = getTemplateSrv()
  ) {
    super(instanceSettings);

    this.type = 'influxdb';
    this.urls = (instanceSettings.url ?? '').split(',').map(url => {
      return url.trim();
    });

    this.username = instanceSettings.username ?? '';
    this.password = instanceSettings.password ?? '';
    this.name = instanceSettings.name;
    this.database = instanceSettings.database;
    this.basicAuth = instanceSettings.basicAuth;
    this.withCredentials = instanceSettings.withCredentials;
    const settingsData = instanceSettings.jsonData || ({} as InfluxOptions);
    this.interval = settingsData.timeInterval;
    this.httpMode = settingsData.httpMode || 'GET';
    this.responseParser = new ResponseParser();
    this.isFlux = settingsData.version === InfluxVersion.Flux;

    if (this.isFlux) {
      // When flux, use an annotation processor rather than the `annotationQuery` lifecycle
      this.annotations = {
        QueryEditor: FluxQueryEditor,
      };
    }
  }

  query(request: DataQueryRequest<InfluxQuery>): Observable<DataQueryResponse> {
    if (this.isFlux) {
      return super.query(request);
    }

    // Fallback to classic query support
    return from(this.classicQuery(request));
  }

  getQueryDisplayText(query: InfluxQuery) {
    if (this.isFlux) {
      return query.query;
    }
    return new InfluxQueryModel(query).render(false);
  }

  /**
   * Returns false if the query should be skipped
   */
  filterQuery(query: InfluxQuery): boolean {
    if (this.isFlux) {
      return !!query.query;
    }
    return true;
  }

  /**
   * Only applied on flux queries
   */
  applyTemplateVariables(query: InfluxQuery, scopedVars: ScopedVars): Record<string, any> {
    return {
      ...query,
      query: this.templateSrv.replace(query.query ?? '', scopedVars), // The raw query text
    };
  }

  /**
   * The unchanged pre 7.1 query implementation
   */
  async classicQuery(options: any): Promise<DataQueryResponse> {
    let timeFilter = this.getTimeFilter(options);
    const scopedVars = options.scopedVars;
    const targets = _.cloneDeep(options.targets);
    const queryTargets: any[] = [];

    let i, y;

    let allQueries = _.map(targets, target => {
      if (target.hide) {
        return '';
      }

      queryTargets.push(target);

      // backward compatibility
      scopedVars.interval = scopedVars.__interval;

      return new InfluxQueryModel(target, this.templateSrv, scopedVars).render(true);
    }).reduce((acc, current) => {
      if (current !== '') {
        acc += ';' + current;
      }
      return acc;
    });

    if (allQueries === '') {
      return Promise.resolve({ data: [] });
    }

    // add global adhoc filters to timeFilter
    const adhocFilters = this.templateSrv.getAdhocFilters(this.name);
    if (adhocFilters.length > 0) {
      const tmpQuery = new InfluxQueryModel({ refId: 'A' }, this.templateSrv, scopedVars);
      timeFilter += ' AND ' + tmpQuery.renderAdhocFilters(adhocFilters);
    }

    // replace grafana variables
    scopedVars.timeFilter = { value: timeFilter };

    // replace templated variables
    allQueries = this.templateSrv.replace(allQueries, scopedVars);

    return this._seriesQuery(allQueries, options).then((data: any): any => {
      if (!data || !data.results) {
        return [];
      }

      const seriesList = [];
      for (i = 0; i < data.results.length; i++) {
        const result = data.results[i];
        if (!result || !result.series) {
          continue;
        }

        const target = queryTargets[i];
        let alias = target.alias;
        if (alias) {
          alias = this.templateSrv.replace(target.alias, options.scopedVars);
        }

        const meta: QueryResultMeta = {
          executedQueryString: data.executedQueryString,
        };

        const influxSeries = new InfluxSeries({
          refId: target.refId,
          series: data.results[i].series,
          alias: alias,
          meta,
        });

        switch (target.resultFormat) {
          case 'logs':
            meta.preferredVisualisationType = 'logs';
          case 'table': {
            seriesList.push(influxSeries.getTable());
            break;
          }
          default: {
            const timeSeries = influxSeries.getTimeSeries();
            for (y = 0; y < timeSeries.length; y++) {
              seriesList.push(timeSeries[y]);
            }
            break;
          }
        }
      }

      return { data: seriesList };
    });
  }

  async annotationQuery(options: AnnotationQueryRequest<any>): Promise<AnnotationEvent[]> {
    if (this.isFlux) {
      return Promise.reject({
        message: 'Flux requires the standard annotation query',
      });
    }

    // InfluxQL puts a query string on the annotation
    if (!options.annotation.query) {
      return Promise.reject({
        message: 'Query missing in annotation definition',
      });
    }

    const timeFilter = this.getTimeFilter({ rangeRaw: options.rangeRaw, timezone: options.dashboard.timezone });
    let query = options.annotation.query.replace('$timeFilter', timeFilter);
    query = this.templateSrv.replace(query, undefined, 'regex');

    return this._seriesQuery(query, options).then((data: any) => {
      if (!data || !data.results || !data.results[0]) {
        throw { message: 'No results in response from InfluxDB' };
      }
      return new InfluxSeries({
        series: data.results[0].series,
        annotation: options.annotation,
      }).getAnnotations();
    });
  }

  targetContainsTemplate(target: any) {
    for (const group of target.groupBy) {
      for (const param of group.params) {
        if (this.templateSrv.variableExists(param)) {
          return true;
        }
      }
    }

    for (const i in target.tags) {
      if (this.templateSrv.variableExists(target.tags[i].value)) {
        return true;
      }
    }

    return false;
  }

  interpolateVariablesInQueries(queries: InfluxQuery[], scopedVars: ScopedVars): InfluxQuery[] {
    if (!queries || queries.length === 0) {
      return [];
    }

    let expandedQueries = queries;
    if (queries && queries.length > 0) {
      expandedQueries = queries.map(query => {
        const expandedQuery = {
          ...query,
          datasource: this.name,
          measurement: this.templateSrv.replace(query.measurement ?? '', scopedVars, 'regex'),
          policy: this.templateSrv.replace(query.policy ?? '', scopedVars, 'regex'),
        };

        if (query.rawQuery) {
          expandedQuery.query = this.templateSrv.replace(query.query ?? '', scopedVars, 'regex');
        }

        if (query.tags) {
          const expandedTags = query.tags.map(tag => {
            const expandedTag = {
              ...tag,
              value: this.templateSrv.replace(tag.value, undefined, 'regex'),
            };
            return expandedTag;
          });
          expandedQuery.tags = expandedTags;
        }
        return expandedQuery;
      });
    }
    return expandedQueries;
  }

  async metricFindQuery(query: string, options?: any): Promise<MetricFindValue[]> {
    if (this.isFlux) {
      const target: InfluxQuery = {
        refId: 'metricFindQuery',
        query,
      };
      return super
        .query({
          ...options, // includes 'range'
          targets: [target],
        } as DataQueryRequest)
        .toPromise()
        .then(rsp => {
          if (rsp.data?.length) {
            return frameToMetricFindValue(rsp.data[0]);
          }
          return [];
        });
    }

    const interpolated = this.templateSrv.replace(query, undefined, 'regex');

    return this._seriesQuery(interpolated, options).then(resp => {
      return this.responseParser.parse(query, resp);
    });
  }

  getTagKeys(options: any = {}) {
    const queryBuilder = new InfluxQueryBuilder({ measurement: options.measurement || '', tags: [] }, this.database);
    const query = queryBuilder.buildExploreQuery('TAG_KEYS');
    return this.metricFindQuery(query, options);
  }

  getTagValues(options: any = {}) {
    const queryBuilder = new InfluxQueryBuilder({ measurement: options.measurement || '', tags: [] }, this.database);
    const query = queryBuilder.buildExploreQuery('TAG_VALUES', options.key);
    return this.metricFindQuery(query, options);
  }

  _seriesQuery(query: string, options?: any) {
    if (!query) {
      return Promise.resolve({ results: [] });
    }

    if (options && options.range) {
      const timeFilter = this.getTimeFilter({ rangeRaw: options.range, timezone: options.timezone });
      query = query.replace('$timeFilter', timeFilter);
    }

    return this._influxRequest(this.httpMode, '/query', { q: query, epoch: 'ms' }, options);
  }

  serializeParams(params: any) {
    if (!params) {
      return '';
    }

    return _.reduce(
      params,
      (memo, value, key) => {
        if (value === null || value === undefined) {
          return memo;
        }
        memo.push(encodeURIComponent(key) + '=' + encodeURIComponent(value));
        return memo;
      },
      [] as string[]
    ).join('&');
  }

  testDatasource() {
    if (this.isFlux) {
      // TODO: eventually use the real /health endpoint
      const request: DataQueryRequest<InfluxQuery> = {
        targets: [{ refId: 'test', query: 'buckets()' }],
        requestId: `${this.id}-health-${uuidv4()}`,
        dashboardId: 0,
        panelId: 0,
        interval: '1m',
        intervalMs: 60000,
        maxDataPoints: 423,
        range: {
          from: dateTime(1000),
          to: dateTime(2000),
        },
      } as DataQueryRequest<InfluxQuery>;

      return super
        .query(request)
        .toPromise()
        .then((res: DataQueryResponse) => {
          if (!res || !res.data || res.state !== LoadingState.Done) {
            console.error('InfluxDB Error', res);
            return { status: 'error', message: 'Error reading InfluxDB' };
          }
          const first = res.data[0];
          if (first && first.length) {
            return { status: 'success', message: `${first.length} buckets found` };
          }
          console.error('InfluxDB Error', res);
          return { status: 'error', message: 'Error reading buckets' };
        })
        .catch((err: any) => {
          console.error('InfluxDB Error', err);
          return { status: 'error', message: err.message };
        });
    }

    const queryBuilder = new InfluxQueryBuilder({ measurement: '', tags: [] }, this.database);
    const query = queryBuilder.buildExploreQuery('RETENTION POLICIES');

    return this._seriesQuery(query)
      .then((res: any) => {
        const error = _.get(res, 'results[0].error');
        if (error) {
          return { status: 'error', message: error };
        }
        return { status: 'success', message: 'Data source is working' };
      })
      .catch((err: any) => {
        return { status: 'error', message: err.message };
      });
  }

  _influxRequest(method: string, url: string, data: any, options?: any) {
    const currentUrl = this.urls.shift()!;
    this.urls.push(currentUrl);

    const params: any = {};

    if (this.username) {
      params.u = this.username;
      params.p = this.password;
    }

    if (options && options.database) {
      params.db = options.database;
    } else if (this.database) {
      params.db = this.database;
    }

    const { q } = data;

    if (method === 'POST' && _.has(data, 'q')) {
      // verb is POST and 'q' param is defined
      _.extend(params, _.omit(data, ['q']));
      data = this.serializeParams(_.pick(data, ['q']));
    } else if (method === 'GET' || method === 'POST') {
      // verb is GET, or POST without 'q' param
      _.extend(params, data);
      data = null;
    }

    const req: any = {
      method: method,
      url: currentUrl + url,
      params: params,
      data: data,
      precision: 'ms',
      inspect: { type: 'influxdb' },
      paramSerializer: this.serializeParams,
    };

    req.headers = req.headers || {};
    if (this.basicAuth || this.withCredentials) {
      req.withCredentials = true;
    }
    if (this.basicAuth) {
      req.headers.Authorization = this.basicAuth;
    }

    if (method === 'POST') {
      req.headers['Content-type'] = 'application/x-www-form-urlencoded';
    }

    return getBackendSrv()
      .datasourceRequest(req)
      .then(
        (result: any) => {
          const { data } = result;
          if (data) {
            data.executedQueryString = q;
            if (data.results) {
              const errors = result.data.results.filter((elem: any) => elem.error);
              if (errors.length > 0) {
                throw {
                  message: 'InfluxDB Error: ' + errors[0].error,
                  data,
                };
              }
            }
          }
          return data;
        },
        (err: any) => {
          if ((Number.isInteger(err.status) && err.status !== 0) || err.status >= 300) {
            if (err.data && err.data.error) {
              throw {
                message: 'InfluxDB Error: ' + err.data.error,
                data: err.data,
                config: err.config,
              };
            } else {
              throw {
                message: 'Network Error: ' + err.statusText + '(' + err.status + ')',
                data: err.data,
                config: err.config,
              };
            }
          } else {
            throw err;
          }
        }
      );
  }

  getTimeFilter(options: any) {
    const from = this.getInfluxTime(options.rangeRaw.from, false, options.timezone);
    const until = this.getInfluxTime(options.rangeRaw.to, true, options.timezone);
    const fromIsAbsolute = from[from.length - 1] === 'ms';

    if (until === 'now()' && !fromIsAbsolute) {
      return 'time >= ' + from;
    }

    return 'time >= ' + from + ' and time <= ' + until;
  }

  getInfluxTime(date: any, roundUp: any, timezone: any) {
    if (_.isString(date)) {
      if (date === 'now') {
        return 'now()';
      }

      const parts = /^now-(\d+)([dhms])$/.exec(date);
      if (parts) {
        const amount = parseInt(parts[1], 10);
        const unit = parts[2];
        return 'now() - ' + amount + unit;
      }
      date = dateMath.parse(date, roundUp, timezone);
    }

    return date.valueOf() + 'ms';
  }
}
