import angular, { IQService } from 'angular';
import _ from 'lodash';
import { dateMath, ScopedVars } from '@grafana/data';
import kbn from 'app/core/utils/kbn';
import { CloudWatchQuery } from './types';
import { DataSourceApi, DataQueryRequest, DataSourceInstanceSettings } from '@grafana/ui';
import { BackendSrv } from 'app/core/services/backend_srv';
import { TemplateSrv } from 'app/features/templating/template_srv';
import { TimeSrv } from 'app/features/dashboard/services/TimeSrv';
// import * as moment from 'moment';

export default class CloudWatchDatasource extends DataSourceApi<CloudWatchQuery> {
  type: any;
  proxyUrl: any;
  defaultRegion: any;
  standardStatistics: any;

  /** @ngInject */
  constructor(
    private instanceSettings: DataSourceInstanceSettings,
    private $q: IQService,
    private backendSrv: BackendSrv,
    private templateSrv: TemplateSrv,
    private timeSrv: TimeSrv
  ) {
    super(instanceSettings);
    this.type = 'cloudwatch';
    this.proxyUrl = instanceSettings.url;
    this.defaultRegion = instanceSettings.jsonData.defaultRegion;
    this.instanceSettings = instanceSettings;
    this.standardStatistics = ['Average', 'Maximum', 'Minimum', 'Sum', 'SampleCount'];
  }

  query(options: DataQueryRequest<CloudWatchQuery>) {
    options = angular.copy(options);
    options.targets = this.expandTemplateVariable(options.targets, options.scopedVars, this.templateSrv);

    const queries = _.filter(options.targets, item => {
      return (
        (item.id !== '' || item.hide !== true) &&
        ((!!item.region && !!item.namespace && !!item.metricName && !_.isEmpty(item.statistics)) ||
          item.expression.length > 0)
      );
    }).map(item => {
      item.region = this.templateSrv.replace(this.getActualRegion(item.region), options.scopedVars);
      item.namespace = this.templateSrv.replace(item.namespace, options.scopedVars);
      item.metricName = this.templateSrv.replace(item.metricName, options.scopedVars);
      item.dimensions = this.convertDimensionFormat(item.dimensions, options.scopedVars);
      item.statistics = item.statistics.map(s => {
        return this.templateSrv.replace(s, options.scopedVars);
      });
      item.period = String(this.getPeriod(item, options)); // use string format for period in graph query, and alerting
      item.id = this.templateSrv.replace(item.id, options.scopedVars);
      item.expression = this.templateSrv.replace(item.expression, options.scopedVars);

      // valid ExtendedStatistics is like p90.00, check the pattern
      const hasInvalidStatistics = item.statistics.some(s => {
        if (s.indexOf('p') === 0) {
          const matches = /^p\d{2}(?:\.\d{1,2})?$/.exec(s);
          return !matches || matches[0] !== s;
        }

        return false;
      });

      if (hasInvalidStatistics) {
        throw { message: 'Invalid extended statistics' };
      }

      return _.extend(
        {
          refId: item.refId,
          intervalMs: options.intervalMs,
          maxDataPoints: options.maxDataPoints,
          datasourceId: this.instanceSettings.id,
          type: 'timeSeriesQuery',
        },
        item
      );
    });

    // No valid targets, return the empty result to save a round trip.
    if (_.isEmpty(queries)) {
      const d = this.$q.defer();
      d.resolve({ data: [] });
      return d.promise;
    }

    const request = {
      from: options.range.from.valueOf().toString(),
      to: options.range.to.valueOf().toString(),
      queries: queries,
    };

    return this.performTimeSeriesQuery(request);
  }

  getPeriod(target: any, options: any, now?: number) {
    const start = this.convertToCloudWatchTime(options.range.from, false);
    const end = this.convertToCloudWatchTime(options.range.to, true);
    now = Math.round((now || Date.now()) / 1000);

    let period;
    const range = end - start;

    const hourSec = 60 * 60;
    const daySec = hourSec * 24;
    let periodUnit = 60;
    if (!target.period) {
      if (now - start <= daySec * 15) {
        // until 15 days ago
        if (target.namespace === 'AWS/EC2') {
          periodUnit = period = 300;
        } else {
          periodUnit = period = 60;
        }
      } else if (now - start <= daySec * 63) {
        // until 63 days ago
        periodUnit = period = 60 * 5;
      } else if (now - start <= daySec * 455) {
        // until 455 days ago
        periodUnit = period = 60 * 60;
      } else {
        // over 455 days, should return error, but try to long period
        periodUnit = period = 60 * 60;
      }
    } else {
      if (/^\d+$/.test(target.period)) {
        period = parseInt(target.period, 10);
      } else {
        period = kbn.interval_to_seconds(this.templateSrv.replace(target.period, options.scopedVars));
      }
    }
    if (period < 1) {
      period = 1;
    }
    if (!target.highResolution && range / period >= 1440) {
      period = Math.ceil(range / 1440 / periodUnit) * periodUnit;
    }

    return period;
  }

  performTimeSeriesQuery(request: any) {
    return this.awsRequest('/api/tsdb/query', request).then((res: any) => {
      const data = [];

      if (res.results) {
        for (const query of request.queries) {
          const queryRes = res.results[query.refId];
          if (queryRes) {
            for (const series of queryRes.series) {
              const s = { target: series.name, datapoints: series.points } as any;
              if (queryRes.meta.unit) {
                s.unit = queryRes.meta.unit;
              }
              data.push(s);
            }
          }
        }
      }

      return { data: data };
    });
  }

  transformSuggestDataFromTable(suggestData: any) {
    return _.map(suggestData.results['metricFindQuery'].tables[0].rows, v => {
      return {
        text: v[0],
        value: v[1],
      };
    });
  }

  doMetricQueryRequest(subtype: any, parameters: any) {
    const range = this.timeSrv.timeRange();
    return this.awsRequest('/api/tsdb/query', {
      from: range.from.valueOf().toString(),
      to: range.to.valueOf().toString(),
      queries: [
        _.extend(
          {
            refId: 'metricFindQuery',
            intervalMs: 1, // dummy
            maxDataPoints: 1, // dummy
            datasourceId: this.instanceSettings.id,
            type: 'metricFindQuery',
            subtype: subtype,
          },
          parameters
        ),
      ],
    }).then((r: any) => {
      return this.transformSuggestDataFromTable(r);
    });
  }

  getRegions() {
    return this.doMetricQueryRequest('regions', null);
  }

  getNamespaces() {
    return this.doMetricQueryRequest('namespaces', null);
  }

  getMetrics(namespace: string, region: string) {
    return this.doMetricQueryRequest('metrics', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      namespace: this.templateSrv.replace(namespace),
    });
  }

  getDimensionKeys(namespace: string, region: string) {
    return this.doMetricQueryRequest('dimension_keys', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      namespace: this.templateSrv.replace(namespace),
    });
  }

  getDimensionValues(
    region: string,
    namespace: string,
    metricName: string,
    dimensionKey: string,
    filterDimensions: {}
  ) {
    return this.doMetricQueryRequest('dimension_values', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      namespace: this.templateSrv.replace(namespace),
      metricName: this.templateSrv.replace(metricName),
      dimensionKey: this.templateSrv.replace(dimensionKey),
      dimensions: this.convertDimensionFormat(filterDimensions, {}),
    });
  }

  getEbsVolumeIds(region: string, instanceId: string) {
    return this.doMetricQueryRequest('ebs_volume_ids', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      instanceId: this.templateSrv.replace(instanceId),
    });
  }

  getEc2InstanceAttribute(region: string, attributeName: string, filters: any) {
    return this.doMetricQueryRequest('ec2_instance_attribute', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      attributeName: this.templateSrv.replace(attributeName),
      filters: filters,
    });
  }

  getResourceARNs(region: string, resourceType: string, tags: any) {
    return this.doMetricQueryRequest('resource_arns', {
      region: this.templateSrv.replace(this.getActualRegion(region)),
      resourceType: this.templateSrv.replace(resourceType),
      tags: tags,
    });
  }

  metricFindQuery(query: string) {
    let region;
    let namespace;
    let metricName;
    let filterJson;

    const regionQuery = query.match(/^regions\(\)/);
    if (regionQuery) {
      return this.getRegions();
    }

    const namespaceQuery = query.match(/^namespaces\(\)/);
    if (namespaceQuery) {
      return this.getNamespaces();
    }

    const metricNameQuery = query.match(/^metrics\(([^\)]+?)(,\s?([^,]+?))?\)/);
    if (metricNameQuery) {
      namespace = metricNameQuery[1];
      region = metricNameQuery[3];
      return this.getMetrics(namespace, region);
    }

    const dimensionKeysQuery = query.match(/^dimension_keys\(([^\)]+?)(,\s?([^,]+?))?\)/);
    if (dimensionKeysQuery) {
      namespace = dimensionKeysQuery[1];
      region = dimensionKeysQuery[3];
      return this.getDimensionKeys(namespace, region);
    }

    const dimensionValuesQuery = query.match(
      /^dimension_values\(([^,]+?),\s?([^,]+?),\s?([^,]+?),\s?([^,]+?)(,\s?(.+))?\)/
    );
    if (dimensionValuesQuery) {
      region = dimensionValuesQuery[1];
      namespace = dimensionValuesQuery[2];
      metricName = dimensionValuesQuery[3];
      const dimensionKey = dimensionValuesQuery[4];
      filterJson = {};
      if (dimensionValuesQuery[6]) {
        filterJson = JSON.parse(this.templateSrv.replace(dimensionValuesQuery[6]));
      }

      return this.getDimensionValues(region, namespace, metricName, dimensionKey, filterJson);
    }

    const ebsVolumeIdsQuery = query.match(/^ebs_volume_ids\(([^,]+?),\s?([^,]+?)\)/);
    if (ebsVolumeIdsQuery) {
      region = ebsVolumeIdsQuery[1];
      const instanceId = ebsVolumeIdsQuery[2];
      return this.getEbsVolumeIds(region, instanceId);
    }

    const ec2InstanceAttributeQuery = query.match(/^ec2_instance_attribute\(([^,]+?),\s?([^,]+?),\s?(.+?)\)/);
    if (ec2InstanceAttributeQuery) {
      region = ec2InstanceAttributeQuery[1];
      const targetAttributeName = ec2InstanceAttributeQuery[2];
      filterJson = JSON.parse(this.templateSrv.replace(ec2InstanceAttributeQuery[3]));
      return this.getEc2InstanceAttribute(region, targetAttributeName, filterJson);
    }

    const resourceARNsQuery = query.match(/^resource_arns\(([^,]+?),\s?([^,]+?),\s?(.+?)\)/);
    if (resourceARNsQuery) {
      region = resourceARNsQuery[1];
      const resourceType = resourceARNsQuery[2];
      const tagsJSON = JSON.parse(this.templateSrv.replace(resourceARNsQuery[3]));
      return this.getResourceARNs(region, resourceType, tagsJSON);
    }

    return this.$q.when([]);
  }

  annotationQuery(options: any) {
    const annotation = options.annotation;
    const statistics = _.map(annotation.statistics, s => {
      return this.templateSrv.replace(s);
    });
    const defaultPeriod = annotation.prefixMatching ? '' : '300';
    let period = annotation.period || defaultPeriod;
    period = parseInt(period, 10);
    const parameters = {
      prefixMatching: annotation.prefixMatching,
      region: this.templateSrv.replace(this.getActualRegion(annotation.region)),
      namespace: this.templateSrv.replace(annotation.namespace),
      metricName: this.templateSrv.replace(annotation.metricName),
      dimensions: this.convertDimensionFormat(annotation.dimensions, {}),
      statistics: statistics,
      period: period,
      actionPrefix: annotation.actionPrefix || '',
      alarmNamePrefix: annotation.alarmNamePrefix || '',
    };

    return this.awsRequest('/api/tsdb/query', {
      from: options.range.from.valueOf().toString(),
      to: options.range.to.valueOf().toString(),
      queries: [
        _.extend(
          {
            refId: 'annotationQuery',
            intervalMs: 1, // dummy
            maxDataPoints: 1, // dummy
            datasourceId: this.instanceSettings.id,
            type: 'annotationQuery',
          },
          parameters
        ),
      ],
    }).then((r: any) => {
      return _.map(r.results['annotationQuery'].tables[0].rows, v => {
        return {
          annotation: annotation,
          time: Date.parse(v[0]),
          title: v[1],
          tags: [v[2]],
          text: v[3],
        };
      });
    });
  }

  targetContainsTemplate(target: any) {
    return (
      this.templateSrv.variableExists(target.region) ||
      this.templateSrv.variableExists(target.namespace) ||
      this.templateSrv.variableExists(target.metricName) ||
      _.find(target.dimensions, (v, k) => {
        return this.templateSrv.variableExists(k) || this.templateSrv.variableExists(v);
      })
    );
  }

  testDatasource() {
    /* use billing metrics for test */
    const region = this.defaultRegion;
    const namespace = 'AWS/Billing';
    const metricName = 'EstimatedCharges';
    const dimensions = {};

    return this.getDimensionValues(region, namespace, metricName, 'ServiceName', dimensions).then(() => {
      return { status: 'success', message: 'Data source is working' };
    });
  }

  awsRequest(url: string, data: any) {
    const options = {
      method: 'POST',
      url,
      data,
    };

    return this.backendSrv.datasourceRequest(options).then((result: any) => {
      return result.data;
    });
  }

  getDefaultRegion() {
    return this.defaultRegion;
  }

  getActualRegion(region: string) {
    if (region === 'default' || _.isEmpty(region)) {
      return this.getDefaultRegion();
    }
    return region;
  }

  getExpandedVariables(target: any, dimensionKey: any, variable: any, templateSrv: TemplateSrv) {
    /* if the all checkbox is marked we should add all values to the targets */
    const allSelected: any = _.find(variable.options, { selected: true, text: 'All' });
    const selectedVariables = _.filter(variable.options, v => {
      if (allSelected) {
        return v.text !== 'All';
      } else {
        return v.selected;
      }
    });
    const currentVariables = !_.isArray(variable.current.value)
      ? [variable.current]
      : variable.current.value.map((v: any) => {
          return {
            text: v,
            value: v,
          };
        });
    const useSelectedVariables =
      selectedVariables.some((s: any) => {
        return s.value === currentVariables[0].value;
      }) || currentVariables[0].value === '$__all';
    return (useSelectedVariables ? selectedVariables : currentVariables).map((v: any) => {
      const t = angular.copy(target);
      const scopedVar: any = {};
      scopedVar[variable.name] = v;
      t.refId = target.refId + '_' + v.value;
      t.dimensions[dimensionKey] = templateSrv.replace(t.dimensions[dimensionKey], scopedVar);
      if (variable.multi && target.id) {
        t.id = target.id + window.btoa(v.value).replace(/=/g, '0'); // generate unique id
      } else {
        t.id = target.id;
      }
      return t;
    });
  }

  expandTemplateVariable(targets: any, scopedVars: ScopedVars, templateSrv: TemplateSrv) {
    // Datasource and template srv logic uber-complected. This should be cleaned up.
    return _.chain(targets)
      .map(target => {
        if (target.id && target.id.length > 0 && target.expression && target.expression.length > 0) {
          return [target];
        }

        const variableIndex = _.keyBy(templateSrv.variables, 'name');
        const dimensionKey = _.findKey(target.dimensions, v => {
          const variableName = templateSrv.getVariableName(v);
          return templateSrv.variableExists(v) && !_.has(scopedVars, variableName) && variableIndex[variableName].multi;
        });

        if (dimensionKey) {
          const multiVariable = variableIndex[templateSrv.getVariableName(target.dimensions[dimensionKey])];
          return this.getExpandedVariables(target, dimensionKey, multiVariable, templateSrv);
        } else {
          return [target];
        }
      })
      .flatten()
      .value();
  }

  convertToCloudWatchTime(date: any, roundUp: any) {
    if (_.isString(date)) {
      date = dateMath.parse(date, roundUp);
    }
    return Math.round(date.valueOf() / 1000);
  }

  convertDimensionFormat(dimensions: any, scopedVars: ScopedVars) {
    const convertedDimensions: any = {};
    _.each(dimensions, (value, key) => {
      convertedDimensions[this.templateSrv.replace(key, scopedVars)] = this.templateSrv.replace(value, scopedVars);
    });
    return convertedDimensions;
  }
}
