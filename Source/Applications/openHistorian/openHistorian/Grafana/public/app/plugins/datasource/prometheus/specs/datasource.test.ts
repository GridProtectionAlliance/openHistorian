import _ from 'lodash';
// @ts-ignore
import q from 'q';
import {
  alignRange,
  extractRuleMappingFromGroups,
  PrometheusDatasource,
  prometheusRegularEscape,
  prometheusSpecialRegexEscape,
} from '../datasource';
import { dateTime } from '@grafana/data';
import { DataSourceInstanceSettings, DataQueryResponseData, DataQueryRequest } from '@grafana/ui';
import { PromOptions, PromQuery, PromContext } from '../types';
import { TemplateSrv } from 'app/features/templating/template_srv';
import { TimeSrv } from 'app/features/dashboard/services/TimeSrv';
import { CustomVariable } from 'app/features/templating/custom_variable';

jest.mock('../metric_find_query');

const DEFAULT_TEMPLATE_SRV_MOCK = {
  getAdhocFilters: () => [] as any[],
  replace: (a: string) => a,
};

describe('PrometheusDatasource', () => {
  const ctx: any = {};
  const instanceSettings = ({
    url: 'proxied',
    directUrl: 'direct',
    user: 'test',
    password: 'mupp',
    jsonData: {} as any,
  } as unknown) as DataSourceInstanceSettings<PromOptions>;

  ctx.backendSrvMock = {};

  ctx.templateSrvMock = DEFAULT_TEMPLATE_SRV_MOCK;

  ctx.timeSrvMock = {
    timeRange: () => {
      return {
        from: dateTime(1531468681),
        to: dateTime(1531489712),
      };
    },
  };

  beforeEach(() => {
    ctx.ds = new PrometheusDatasource(instanceSettings, q, ctx.backendSrvMock, ctx.templateSrvMock, ctx.timeSrvMock);
  });

  describe('Datasource metadata requests', () => {
    it('should perform a GET request with the default config', () => {
      ctx.backendSrvMock.datasourceRequest = jest.fn();
      ctx.ds.metadataRequest('/foo');
      expect(ctx.backendSrvMock.datasourceRequest.mock.calls.length).toBe(1);
      expect(ctx.backendSrvMock.datasourceRequest.mock.calls[0][0].method).toBe('GET');
    });

    it('should still perform a GET request with the DS HTTP method set to POST', () => {
      ctx.backendSrvMock.datasourceRequest = jest.fn();
      const postSettings = _.cloneDeep(instanceSettings);
      postSettings.jsonData.httpMethod = 'POST';
      const ds = new PrometheusDatasource(postSettings, q, ctx.backendSrvMock, ctx.templateSrvMock, ctx.timeSrvMock);
      ds.metadataRequest('/foo');
      expect(ctx.backendSrvMock.datasourceRequest.mock.calls.length).toBe(1);
      expect(ctx.backendSrvMock.datasourceRequest.mock.calls[0][0].method).toBe('GET');
    });
  });

  describe('When using adhoc filters', () => {
    const DEFAULT_QUERY_EXPRESSION = 'metric{job="foo"} - metric';
    const target = { expr: DEFAULT_QUERY_EXPRESSION };

    afterEach(() => {
      ctx.templateSrvMock.getAdhocFilters = DEFAULT_TEMPLATE_SRV_MOCK.getAdhocFilters;
    });

    it('should not modify expression with no filters', () => {
      const result = ctx.ds.createQuery(target, { interval: '15s' });
      expect(result).toMatchObject({ expr: DEFAULT_QUERY_EXPRESSION });
    });

    it('should add filters to expression', () => {
      ctx.templateSrvMock.getAdhocFilters = () => [
        {
          key: 'k1',
          operator: '=',
          value: 'v1',
        },
        {
          key: 'k2',
          operator: '!=',
          value: 'v2',
        },
      ];
      const result = ctx.ds.createQuery(target, { interval: '15s' });
      expect(result).toMatchObject({ expr: 'metric{job="foo",k1="v1",k2!="v2"} - metric{k1="v1",k2!="v2"}' });
    });

    it('should add escaping if needed to regex filter expressions', () => {
      ctx.templateSrvMock.getAdhocFilters = () => [
        {
          key: 'k1',
          operator: '=~',
          value: 'v.*',
        },
        {
          key: 'k2',
          operator: '=~',
          value: `v'.*`,
        },
      ];
      const result = ctx.ds.createQuery(target, { interval: '15s' });
      expect(result).toMatchObject({
        expr: `metric{job="foo",k1=~"v.*",k2=~"v\\\\'.*"} - metric{k1=~"v.*",k2=~"v\\\\'.*"}`,
      });
    });
  });

  describe('When performing performSuggestQuery', () => {
    it('should cache response', async () => {
      ctx.backendSrvMock.datasourceRequest.mockReturnValue(
        Promise.resolve({
          status: 'success',
          data: { data: ['value1', 'value2', 'value3'] },
        })
      );

      let results = await ctx.ds.performSuggestQuery('value', true);

      expect(results).toHaveLength(3);

      ctx.backendSrvMock.datasourceRequest.mockReset();
      results = await ctx.ds.performSuggestQuery('value', true);

      expect(results).toHaveLength(3);
    });
  });

  describe('When converting prometheus histogram to heatmap format', () => {
    beforeEach(() => {
      ctx.query = {
        range: { from: dateTime(1443454528000), to: dateTime(1443454528000) },
        targets: [{ expr: 'test{job="testjob"}', format: 'heatmap', legendFormat: '{{le}}' }],
        interval: '1s',
      };
    });

    it('should convert cumullative histogram to ordinary', () => {
      const resultMock = [
        {
          metric: { __name__: 'metric', job: 'testjob', le: '10' },
          values: [[1443454528.0, '10'], [1443454528.0, '10']],
        },
        {
          metric: { __name__: 'metric', job: 'testjob', le: '20' },
          values: [[1443454528.0, '20'], [1443454528.0, '10']],
        },
        {
          metric: { __name__: 'metric', job: 'testjob', le: '30' },
          values: [[1443454528.0, '25'], [1443454528.0, '10']],
        },
      ];
      const responseMock = { data: { data: { result: resultMock } } };

      const expected = [
        {
          target: '10',
          datapoints: [[10, 1443454528000], [10, 1443454528000]],
        },
        {
          target: '20',
          datapoints: [[10, 1443454528000], [0, 1443454528000]],
        },
        {
          target: '30',
          datapoints: [[5, 1443454528000], [0, 1443454528000]],
        },
      ];

      ctx.ds.performTimeSeriesQuery = jest.fn().mockReturnValue([responseMock]);
      ctx.ds.query(ctx.query).subscribe((result: any) => {
        const results = result.data;
        return expect(results).toMatchObject(expected);
      });
    });

    it('should sort series by label value', () => {
      const resultMock = [
        {
          metric: { __name__: 'metric', job: 'testjob', le: '2' },
          values: [[1443454528.0, '10'], [1443454528.0, '10']],
        },
        {
          metric: { __name__: 'metric', job: 'testjob', le: '4' },
          values: [[1443454528.0, '20'], [1443454528.0, '10']],
        },
        {
          metric: { __name__: 'metric', job: 'testjob', le: '+Inf' },
          values: [[1443454528.0, '25'], [1443454528.0, '10']],
        },
        {
          metric: { __name__: 'metric', job: 'testjob', le: '1' },
          values: [[1443454528.0, '25'], [1443454528.0, '10']],
        },
      ];
      const responseMock = { data: { data: { result: resultMock } } };

      const expected = ['1', '2', '4', '+Inf'];

      ctx.ds.performTimeSeriesQuery = jest.fn().mockReturnValue([responseMock]);
      ctx.ds.query(ctx.query).subscribe((result: any) => {
        const seriesLabels = _.map(result.data, 'target');
        return expect(seriesLabels).toEqual(expected);
      });
    });
  });

  describe('alignRange', () => {
    it('does not modify already aligned intervals with perfect step', () => {
      const range = alignRange(0, 3, 3, 0);
      expect(range.start).toEqual(0);
      expect(range.end).toEqual(3);
    });
    it('does modify end-aligned intervals to reflect number of steps possible', () => {
      const range = alignRange(1, 6, 3, 0);
      expect(range.start).toEqual(0);
      expect(range.end).toEqual(6);
    });
    it('does align intervals that are a multiple of steps', () => {
      const range = alignRange(1, 4, 3, 0);
      expect(range.start).toEqual(0);
      expect(range.end).toEqual(3);
    });
    it('does align intervals that are not a multiple of steps', () => {
      const range = alignRange(1, 5, 3, 0);
      expect(range.start).toEqual(0);
      expect(range.end).toEqual(3);
    });
    it('does align intervals with local midnight -UTC offset', () => {
      //week range, location 4+ hours UTC offset, 24h step time
      const range = alignRange(4 * 60 * 60, (7 * 24 + 4) * 60 * 60, 24 * 60 * 60, -4 * 60 * 60); //04:00 UTC, 7 day range
      expect(range.start).toEqual(4 * 60 * 60);
      expect(range.end).toEqual((7 * 24 + 4) * 60 * 60);
    });
    it('does align intervals with local midnight +UTC offset', () => {
      //week range, location 4- hours UTC offset, 24h step time
      const range = alignRange(20 * 60 * 60, (8 * 24 - 4) * 60 * 60, 24 * 60 * 60, 4 * 60 * 60); //20:00 UTC on day1, 7 days later is 20:00 on day8
      expect(range.start).toEqual(20 * 60 * 60);
      expect(range.end).toEqual((8 * 24 - 4) * 60 * 60);
    });
  });

  describe('extractRuleMappingFromGroups()', () => {
    it('returns empty mapping for no rule groups', () => {
      expect(extractRuleMappingFromGroups([])).toEqual({});
    });

    it('returns a mapping for recording rules only', () => {
      const groups = [
        {
          rules: [
            {
              name: 'HighRequestLatency',
              query: 'job:request_latency_seconds:mean5m{job="myjob"} > 0.5',
              type: 'alerting',
            },
            {
              name: 'job:http_inprogress_requests:sum',
              query: 'sum(http_inprogress_requests) by (job)',
              type: 'recording',
            },
          ],
          file: '/rules.yaml',
          interval: 60,
          name: 'example',
        },
      ];
      const mapping = extractRuleMappingFromGroups(groups);
      expect(mapping).toEqual({ 'job:http_inprogress_requests:sum': 'sum(http_inprogress_requests) by (job)' });
    });
  });

  describe('Prometheus regular escaping', () => {
    it('should not escape non-string', () => {
      expect(prometheusRegularEscape(12)).toEqual(12);
    });
    it('should not escape simple string', () => {
      expect(prometheusRegularEscape('cryptodepression')).toEqual('cryptodepression');
    });
    it("should escape '", () => {
      expect(prometheusRegularEscape("looking'glass")).toEqual("looking\\\\'glass");
    });
    it('should escape multiple characters', () => {
      expect(prometheusRegularEscape("'looking'glass'")).toEqual("\\\\'looking\\\\'glass\\\\'");
    });
  });

  describe('Prometheus regexes escaping', () => {
    it('should not escape simple string', () => {
      expect(prometheusSpecialRegexEscape('cryptodepression')).toEqual('cryptodepression');
    });
    it('should escape $^*+?.()|\\', () => {
      expect(prometheusSpecialRegexEscape("looking'glass")).toEqual("looking\\\\'glass");
      expect(prometheusSpecialRegexEscape('looking{glass')).toEqual('looking\\\\{glass');
      expect(prometheusSpecialRegexEscape('looking}glass')).toEqual('looking\\\\}glass');
      expect(prometheusSpecialRegexEscape('looking[glass')).toEqual('looking\\\\[glass');
      expect(prometheusSpecialRegexEscape('looking]glass')).toEqual('looking\\\\]glass');
      expect(prometheusSpecialRegexEscape('looking$glass')).toEqual('looking\\\\$glass');
      expect(prometheusSpecialRegexEscape('looking^glass')).toEqual('looking\\\\^glass');
      expect(prometheusSpecialRegexEscape('looking*glass')).toEqual('looking\\\\*glass');
      expect(prometheusSpecialRegexEscape('looking+glass')).toEqual('looking\\\\+glass');
      expect(prometheusSpecialRegexEscape('looking?glass')).toEqual('looking\\\\?glass');
      expect(prometheusSpecialRegexEscape('looking.glass')).toEqual('looking\\\\.glass');
      expect(prometheusSpecialRegexEscape('looking(glass')).toEqual('looking\\\\(glass');
      expect(prometheusSpecialRegexEscape('looking)glass')).toEqual('looking\\\\)glass');
      expect(prometheusSpecialRegexEscape('looking\\glass')).toEqual('looking\\\\\\\\glass');
      expect(prometheusSpecialRegexEscape('looking|glass')).toEqual('looking\\\\|glass');
    });
    it('should escape multiple special characters', () => {
      expect(prometheusSpecialRegexEscape('+looking$glass?')).toEqual('\\\\+looking\\\\$glass\\\\?');
    });
  });

  describe('When interpolating variables', () => {
    beforeEach(() => {
      ctx.ds = new PrometheusDatasource(instanceSettings, q, ctx.backendSrvMock, ctx.templateSrvMock, ctx.timeSrvMock);
      ctx.variable = new CustomVariable({}, {} as any);
    });

    describe('and value is a string', () => {
      it('should only escape single quotes', () => {
        expect(ctx.ds.interpolateQueryExpr("abc'$^*{}[]+?.()|", ctx.variable)).toEqual("abc\\\\'$^*{}[]+?.()|");
      });
    });

    describe('and value is a number', () => {
      it('should return a number', () => {
        expect(ctx.ds.interpolateQueryExpr(1000, ctx.variable)).toEqual(1000);
      });
    });

    describe('and variable allows multi-value', () => {
      beforeEach(() => {
        ctx.variable.multi = true;
      });

      it('should regex escape values if the value is a string', () => {
        expect(ctx.ds.interpolateQueryExpr('looking*glass', ctx.variable)).toEqual('looking\\\\*glass');
      });

      it('should return pipe separated values if the value is an array of strings', () => {
        expect(ctx.ds.interpolateQueryExpr(['a|bc', 'de|f'], ctx.variable)).toEqual('a\\\\|bc|de\\\\|f');
      });
    });

    describe('and variable allows all', () => {
      beforeEach(() => {
        ctx.variable.includeAll = true;
      });

      it('should regex escape values if the array is a string', () => {
        expect(ctx.ds.interpolateQueryExpr('looking*glass', ctx.variable)).toEqual('looking\\\\*glass');
      });

      it('should return pipe separated values if the value is an array of strings', () => {
        expect(ctx.ds.interpolateQueryExpr(['a|bc', 'de|f'], ctx.variable)).toEqual('a\\\\|bc|de\\\\|f');
      });
    });
  });

  describe('metricFindQuery', () => {
    beforeEach(() => {
      const query = 'query_result(topk(5,rate(http_request_duration_microseconds_count[$__interval])))';
      ctx.templateSrvMock.replace = jest.fn();
      ctx.timeSrvMock.timeRange = () => {
        return {
          from: dateTime(1531468681),
          to: dateTime(1531489712),
        };
      };
      ctx.ds = new PrometheusDatasource(instanceSettings, q, ctx.backendSrvMock, ctx.templateSrvMock, ctx.timeSrvMock);
      ctx.ds.metricFindQuery(query);
    });

    it('should call templateSrv.replace with scopedVars', () => {
      expect(ctx.templateSrvMock.replace.mock.calls[0][1]).toBeDefined();
    });

    it('should have the correct range and range_ms', () => {
      const range = ctx.templateSrvMock.replace.mock.calls[0][1].__range;
      const rangeMs = ctx.templateSrvMock.replace.mock.calls[0][1].__range_ms;
      const rangeS = ctx.templateSrvMock.replace.mock.calls[0][1].__range_s;
      expect(range).toEqual({ text: '21s', value: '21s' });
      expect(rangeMs).toEqual({ text: 21031, value: 21031 });
      expect(rangeS).toEqual({ text: 21, value: 21 });
    });

    it('should pass the default interval value', () => {
      const interval = ctx.templateSrvMock.replace.mock.calls[0][1].__interval;
      const intervalMs = ctx.templateSrvMock.replace.mock.calls[0][1].__interval_ms;
      expect(interval).toEqual({ text: '15s', value: '15s' });
      expect(intervalMs).toEqual({ text: 15000, value: 15000 });
    });
  });
});

const SECOND = 1000;
const MINUTE = 60 * SECOND;
const HOUR = 60 * MINUTE;

const time = ({ hours = 0, seconds = 0, minutes = 0 }) => dateTime(hours * HOUR + minutes * MINUTE + seconds * SECOND);

const ctx = {} as any;
const instanceSettings = ({
  url: 'proxied',
  directUrl: 'direct',
  user: 'test',
  password: 'mupp',
  jsonData: { httpMethod: 'GET' },
} as unknown) as DataSourceInstanceSettings<PromOptions>;
const backendSrv = {
  datasourceRequest: jest.fn(),
} as any;

const templateSrv = ({
  getAdhocFilters: (): any[] => [],
  replace: jest.fn(str => str),
} as unknown) as TemplateSrv;

const timeSrv = ({
  timeRange: () => {
    return {
      from: dateTime(1531468681),
      to: dateTime(1531468681 + 2000),
    };
  },
} as unknown) as TimeSrv;

describe('PrometheusDatasource', () => {
  describe('When querying prometheus with one target using query editor target spec', () => {
    describe('and query syntax is valid', () => {
      let results: any;
      const query = {
        range: { from: time({ seconds: 63 }), to: time({ seconds: 183 }) },
        targets: [{ expr: 'test{job="testjob"}', format: 'time_series' }],
        interval: '60s',
      };
      // Interval alignment with step
      const urlExpected =
        'proxied/api/v1/query_range?query=' + encodeURIComponent('test{job="testjob"}') + '&start=60&end=180&step=60';

      beforeEach(async () => {
        const response = {
          data: {
            status: 'success',
            data: {
              resultType: 'matrix',
              result: [
                {
                  metric: { __name__: 'test', job: 'testjob' },
                  values: [[60, '3846']],
                },
              ],
            },
          },
        };
        backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);

        ctx.ds.query(query).subscribe((data: any) => {
          results = data;
        });
      });

      it('should generate the correct query', () => {
        const res = backendSrv.datasourceRequest.mock.calls[0][0];
        expect(res.method).toBe('GET');
        expect(res.url).toBe(urlExpected);
      });

      it('should return series list', async () => {
        expect(results.data.length).toBe(1);
        expect(results.data[0].target).toBe('test{job="testjob"}');
      });
    });

    describe('and query syntax is invalid', () => {
      let results: string;
      const query = {
        range: { from: time({ seconds: 63 }), to: time({ seconds: 183 }) },
        targets: [{ expr: 'tes;;t{job="testjob"}', format: 'time_series' }],
        interval: '60s',
      };

      const errMessage = 'parse error at char 25: could not parse remaining input';
      const response = {
        data: {
          status: 'error',
          errorType: 'bad_data',
          error: errMessage,
        },
      };

      it('should generate an error', () => {
        backendSrv.datasourceRequest = jest.fn(() => Promise.reject(response));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
        ctx.ds.query(query).subscribe((e: any) => {
          results = e.message;
          expect(results).toBe(`"${errMessage}"`);
        });
      });
    });
  });

  describe('When querying prometheus with one target which returns multiple series', () => {
    let results: any;
    const start = 60;
    const end = 360;
    const step = 60;

    const query = {
      range: { from: time({ seconds: start }), to: time({ seconds: end }) },
      targets: [{ expr: 'test{job="testjob"}', format: 'time_series' }],
      interval: '60s',
    };

    beforeEach(async () => {
      const response = {
        status: 'success',
        data: {
          data: {
            resultType: 'matrix',
            result: [
              {
                metric: { __name__: 'test', job: 'testjob', series: 'series 1' },
                values: [[start + step * 1, '3846'], [start + step * 3, '3847'], [end - step * 1, '3848']],
              },
              {
                metric: { __name__: 'test', job: 'testjob', series: 'series 2' },
                values: [[start + step * 2, '4846']],
              },
            ],
          },
        },
      };

      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);

      ctx.ds.query(query).subscribe((data: any) => {
        results = data;
      });
    });

    it('should be same length', () => {
      expect(results.data.length).toBe(2);
      expect(results.data[0].datapoints.length).toBe((end - start) / step + 1);
      expect(results.data[1].datapoints.length).toBe((end - start) / step + 1);
    });

    it('should fill null until first datapoint in response', () => {
      expect(results.data[0].datapoints[0][1]).toBe(start * 1000);
      expect(results.data[0].datapoints[0][0]).toBe(null);
      expect(results.data[0].datapoints[1][1]).toBe((start + step * 1) * 1000);
      expect(results.data[0].datapoints[1][0]).toBe(3846);
    });
    it('should fill null after last datapoint in response', () => {
      const length = (end - start) / step + 1;
      expect(results.data[0].datapoints[length - 2][1]).toBe((end - step * 1) * 1000);
      expect(results.data[0].datapoints[length - 2][0]).toBe(3848);
      expect(results.data[0].datapoints[length - 1][1]).toBe(end * 1000);
      expect(results.data[0].datapoints[length - 1][0]).toBe(null);
    });
    it('should fill null at gap between series', () => {
      expect(results.data[0].datapoints[2][1]).toBe((start + step * 2) * 1000);
      expect(results.data[0].datapoints[2][0]).toBe(null);
      expect(results.data[1].datapoints[1][1]).toBe((start + step * 1) * 1000);
      expect(results.data[1].datapoints[1][0]).toBe(null);
      expect(results.data[1].datapoints[3][1]).toBe((start + step * 3) * 1000);
      expect(results.data[1].datapoints[3][0]).toBe(null);
    });
  });

  describe('When querying prometheus with one target and instant = true', () => {
    let results: any;
    const urlExpected = 'proxied/api/v1/query?query=' + encodeURIComponent('test{job="testjob"}') + '&time=123';
    const query = {
      range: { from: time({ seconds: 63 }), to: time({ seconds: 123 }) },
      targets: [{ expr: 'test{job="testjob"}', format: 'time_series', instant: true }],
      interval: '60s',
    };

    beforeEach(async () => {
      const response = {
        status: 'success',
        data: {
          data: {
            resultType: 'vector',
            result: [
              {
                metric: { __name__: 'test', job: 'testjob' },
                value: [123, '3846'],
              },
            ],
          },
        },
      };

      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query).subscribe((data: any) => {
        results = data;
      });
    });
    it('should generate the correct query', () => {
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should return series list', () => {
      expect(results.data.length).toBe(1);
      expect(results.data[0].target).toBe('test{job="testjob"}');
    });
  });

  describe('When performing annotationQuery', () => {
    let results: any;

    const options: any = {
      annotation: {
        expr: 'ALERTS{alertstate="firing"}',
        tagKeys: 'job',
        titleFormat: '{{alertname}}',
        textFormat: '{{instance}}',
      },
      range: {
        from: time({ seconds: 63 }),
        to: time({ seconds: 123 }),
      },
    };

    const response = {
      status: 'success',
      data: {
        data: {
          resultType: 'matrix',
          result: [
            {
              metric: {
                __name__: 'ALERTS',
                alertname: 'InstanceDown',
                alertstate: 'firing',
                instance: 'testinstance',
                job: 'testjob',
              },
              values: [[123, '1']],
            },
          ],
        },
      },
    };

    describe('when time series query is cancelled', () => {
      it('should return empty results', async () => {
        backendSrv.datasourceRequest = jest.fn(() => Promise.resolve({ cancelled: true }));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);

        await ctx.ds.annotationQuery(options).then((data: any) => {
          results = data;
        });

        expect(results).toEqual([]);
      });
    });

    describe('not use useValueForTime', () => {
      beforeEach(async () => {
        options.annotation.useValueForTime = false;
        backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);

        await ctx.ds.annotationQuery(options).then((data: any) => {
          results = data;
        });
      });

      it('should return annotation list', () => {
        expect(results.length).toBe(1);
        expect(results[0].tags).toContain('testjob');
        expect(results[0].title).toBe('InstanceDown');
        expect(results[0].text).toBe('testinstance');
        expect(results[0].time).toBe(123 * 1000);
      });
    });

    describe('use useValueForTime', () => {
      beforeEach(async () => {
        options.annotation.useValueForTime = true;
        backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);

        await ctx.ds.annotationQuery(options).then((data: any) => {
          results = data;
        });
      });

      it('should return annotation list', () => {
        expect(results[0].time).toEqual(1);
      });
    });

    describe('step parameter', () => {
      beforeEach(() => {
        backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
        ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      });

      it('should use default step for short range if no interval is given', () => {
        const query = {
          ...options,
          range: {
            from: time({ seconds: 63 }),
            to: time({ seconds: 123 }),
          },
        };
        ctx.ds.annotationQuery(query);
        const req = backendSrv.datasourceRequest.mock.calls[0][0];
        expect(req.url).toContain('step=60');
      });

      it('should use custom step for short range', () => {
        const annotation = {
          ...options.annotation,
          step: '10s',
        };
        const query = {
          ...options,
          annotation,
          range: {
            from: time({ seconds: 63 }),
            to: time({ seconds: 123 }),
          },
        };
        ctx.ds.annotationQuery(query);
        const req = backendSrv.datasourceRequest.mock.calls[0][0];
        expect(req.url).toContain('step=10');
      });

      it('should use custom step for short range', () => {
        const annotation = {
          ...options.annotation,
          step: '10s',
        };
        const query = {
          ...options,
          annotation,
          range: {
            from: time({ seconds: 63 }),
            to: time({ seconds: 123 }),
          },
        };
        ctx.ds.annotationQuery(query);
        const req = backendSrv.datasourceRequest.mock.calls[0][0];
        expect(req.url).toContain('step=10');
      });

      it('should use dynamic step on long ranges if no option was given', () => {
        const query = {
          ...options,
          range: {
            from: time({ seconds: 63 }),
            to: time({ hours: 24 * 30, seconds: 63 }),
          },
        };
        ctx.ds.annotationQuery(query);
        const req = backendSrv.datasourceRequest.mock.calls[0][0];
        // Range in seconds: (to - from) / 1000
        // Max_datapoints: 11000
        // Step: range / max_datapoints
        const step = 236;
        expect(req.url).toContain(`step=${step}`);
      });
    });
  });

  describe('When resultFormat is table and instant = true', () => {
    let results: any;
    const query = {
      range: { from: time({ seconds: 63 }), to: time({ seconds: 123 }) },
      targets: [{ expr: 'test{job="testjob"}', format: 'time_series', instant: true }],
      interval: '60s',
    };

    beforeEach(async () => {
      const response = {
        status: 'success',
        data: {
          data: {
            resultType: 'vector',
            result: [
              {
                metric: { __name__: 'test', job: 'testjob' },
                value: [123, '3846'],
              },
            ],
          },
        },
      };

      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query).subscribe((data: any) => {
        results = data;
      });
    });

    it('should return result', () => {
      expect(results).not.toBe(null);
    });
  });

  describe('The "step" query parameter', () => {
    const response = {
      status: 'success',
      data: {
        data: {
          resultType: 'matrix',
          result: [] as DataQueryResponseData[],
        },
      },
    };

    it('should be min interval when greater than auto interval', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'test',
            interval: '10s',
          },
        ],
        interval: '5s',
      };
      const urlExpected = 'proxied/api/v1/query_range?query=test&start=60&end=420&step=10';

      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });

    it('step should never go below 1', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [{ expr: 'test' }],
        interval: '100ms',
      };
      const urlExpected = 'proxied/api/v1/query_range?query=test&start=60&end=420&step=1';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });

    it('should be auto interval when greater than min interval', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'test',
            interval: '5s',
          },
        ],
        interval: '10s',
      };
      const urlExpected = 'proxied/api/v1/query_range?query=test&start=60&end=420&step=10';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should result in querying fewer than 11000 data points', async () => {
      const query = {
        // 6 hour range
        range: { from: time({ hours: 1 }), to: time({ hours: 7 }) },
        targets: [{ expr: 'test' }],
        interval: '1s',
      };
      const end = 7 * 60 * 60;
      const start = 60 * 60;
      const urlExpected = 'proxied/api/v1/query_range?query=test&start=' + start + '&end=' + end + '&step=2';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should not apply min interval when interval * intervalFactor greater', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'test',
            interval: '10s',
            intervalFactor: 10,
          },
        ],
        interval: '5s',
      };
      // times get rounded up to interval
      const urlExpected = 'proxied/api/v1/query_range?query=test&start=50&end=400&step=50';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should apply min interval when interval * intervalFactor smaller', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'test',
            interval: '15s',
            intervalFactor: 2,
          },
        ],
        interval: '5s',
      };
      const urlExpected = 'proxied/api/v1/query_range?query=test' + '&start=60&end=420&step=15';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should apply intervalFactor to auto interval when greater', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'test',
            interval: '5s',
            intervalFactor: 10,
          },
        ],
        interval: '10s',
      };
      // times get aligned to interval
      const urlExpected = 'proxied/api/v1/query_range?query=test' + '&start=0&end=400&step=100';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should not not be affected by the 11000 data points limit when large enough', async () => {
      const query = {
        // 1 week range
        range: { from: time({}), to: time({ hours: 7 * 24 }) },
        targets: [
          {
            expr: 'test',
            intervalFactor: 10,
          },
        ],
        interval: '10s',
      };
      const end = 7 * 24 * 60 * 60;
      const start = 0;
      const urlExpected = 'proxied/api/v1/query_range?query=test' + '&start=' + start + '&end=' + end + '&step=100';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
    it('should be determined by the 11000 data points limit when too small', async () => {
      const query = {
        // 1 week range
        range: { from: time({}), to: time({ hours: 7 * 24 }) },
        targets: [
          {
            expr: 'test',
            intervalFactor: 10,
          },
        ],
        interval: '5s',
      };
      const end = 7 * 24 * 60 * 60;
      const start = 0;
      const urlExpected = 'proxied/api/v1/query_range?query=test' + '&start=' + start + '&end=' + end + '&step=60';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);
    });
  });

  describe('The __interval and __interval_ms template variables', () => {
    const response = {
      status: 'success',
      data: {
        data: {
          resultType: 'matrix',
          result: [] as DataQueryResponseData[],
        },
      },
    };

    it('should be unchanged when auto interval is greater than min interval', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            interval: '5s',
          },
        ],
        interval: '10s',
        scopedVars: {
          __interval: { text: '10s', value: '10s' },
          __interval_ms: { text: 10 * 1000, value: 10 * 1000 },
        },
      };

      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=60&end=420&step=10';

      templateSrv.replace = jest.fn(str => str);
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '10s',
          value: '10s',
        },
        __interval_ms: {
          text: 10000,
          value: 10000,
        },
      });
    });
    it('should be min interval when it is greater than auto interval', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            interval: '10s',
          },
        ],
        interval: '5s',
        scopedVars: {
          __interval: { text: '5s', value: '5s' },
          __interval_ms: { text: 5 * 1000, value: 5 * 1000 },
        },
      };
      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=60&end=420&step=10';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      templateSrv.replace = jest.fn(str => str);
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '5s',
          value: '5s',
        },
        __interval_ms: {
          text: 5000,
          value: 5000,
        },
      });
    });
    it('should account for intervalFactor', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            interval: '5s',
            intervalFactor: 10,
          },
        ],
        interval: '10s',
        scopedVars: {
          __interval: { text: '10s', value: '10s' },
          __interval_ms: { text: 10 * 1000, value: 10 * 1000 },
        },
      };
      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=0&end=400&step=100';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      templateSrv.replace = jest.fn(str => str);
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '10s',
          value: '10s',
        },
        __interval_ms: {
          text: 10000,
          value: 10000,
        },
      });

      expect(query.scopedVars.__interval.text).toBe('10s');
      expect(query.scopedVars.__interval.value).toBe('10s');
      expect(query.scopedVars.__interval_ms.text).toBe(10 * 1000);
      expect(query.scopedVars.__interval_ms.value).toBe(10 * 1000);
    });
    it('should be interval * intervalFactor when greater than min interval', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            interval: '10s',
            intervalFactor: 10,
          },
        ],
        interval: '5s',
        scopedVars: {
          __interval: { text: '5s', value: '5s' },
          __interval_ms: { text: 5 * 1000, value: 5 * 1000 },
        },
      };
      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=50&end=400&step=50';

      templateSrv.replace = jest.fn(str => str);
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '5s',
          value: '5s',
        },
        __interval_ms: {
          text: 5000,
          value: 5000,
        },
      });
    });
    it('should be min interval when greater than interval * intervalFactor', async () => {
      const query = {
        // 6 minute range
        range: { from: time({ minutes: 1 }), to: time({ minutes: 7 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            interval: '15s',
            intervalFactor: 2,
          },
        ],
        interval: '5s',
        scopedVars: {
          __interval: { text: '5s', value: '5s' },
          __interval_ms: { text: 5 * 1000, value: 5 * 1000 },
        },
      };
      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=60&end=420&step=15';

      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '5s',
          value: '5s',
        },
        __interval_ms: {
          text: 5000,
          value: 5000,
        },
      });
    });
    it('should be determined by the 11000 data points limit, accounting for intervalFactor', async () => {
      const query = {
        // 1 week range
        range: { from: time({}), to: time({ hours: 7 * 24 }) },
        targets: [
          {
            expr: 'rate(test[$__interval])',
            intervalFactor: 10,
          },
        ],
        interval: '5s',
        scopedVars: {
          __interval: { text: '5s', value: '5s' },
          __interval_ms: { text: 5 * 1000, value: 5 * 1000 },
        },
      };
      const end = 7 * 24 * 60 * 60;
      const start = 0;
      const urlExpected =
        'proxied/api/v1/query_range?query=' +
        encodeURIComponent('rate(test[$__interval])') +
        '&start=' +
        start +
        '&end=' +
        end +
        '&step=60';
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      templateSrv.replace = jest.fn(str => str);
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('GET');
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[0][1]).toEqual({
        __interval: {
          text: '5s',
          value: '5s',
        },
        __interval_ms: {
          text: 5000,
          value: 5000,
        },
      });
    });
  });

  describe('The __range, __range_s and __range_ms variables', () => {
    const response = {
      status: 'success',
      data: {
        data: {
          resultType: 'matrix',
          result: [] as DataQueryResponseData[],
        },
      },
    };

    it('should use overridden ranges, not dashboard ranges', async () => {
      const expectedRangeSecond = 3600;
      const expectedRangeString = '1h';
      const query = {
        range: {
          from: time({}),
          to: time({ hours: 1 }),
        },
        targets: [
          {
            expr: 'test[${__range_s}s]',
          },
        ],
        interval: '60s',
      };
      const urlExpected = `proxied/api/v1/query_range?query=${encodeURIComponent(
        query.targets[0].expr
      )}&start=0&end=3600&step=60`;

      templateSrv.replace = jest.fn(str => str);
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query);
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.url).toBe(urlExpected);

      // @ts-ignore
      expect(templateSrv.replace.mock.calls[1][1]).toEqual({
        __range_s: {
          text: expectedRangeSecond,
          value: expectedRangeSecond,
        },
        __range: {
          text: expectedRangeString,
          value: expectedRangeString,
        },
        __range_ms: {
          text: expectedRangeSecond * 1000,
          value: expectedRangeSecond * 1000,
        },
      });
    });
  });
});

describe('PrometheusDatasource for POST', () => {
  //   const ctx = new helpers.ServiceTestContext();
  const instanceSettings = ({
    url: 'proxied',
    directUrl: 'direct',
    user: 'test',
    password: 'mupp',
    jsonData: { httpMethod: 'POST' },
  } as unknown) as DataSourceInstanceSettings<PromOptions>;

  describe('When querying prometheus with one target using query editor target spec', () => {
    let results: any;
    const urlExpected = 'proxied/api/v1/query_range';
    const dataExpected = {
      query: 'test{job="testjob"}',
      start: 1 * 60,
      end: 2 * 60,
      step: 60,
    };
    const query = {
      range: { from: time({ minutes: 1, seconds: 3 }), to: time({ minutes: 2, seconds: 3 }) },
      targets: [{ expr: 'test{job="testjob"}', format: 'time_series' }],
      interval: '60s',
    };

    beforeEach(async () => {
      const response = {
        status: 'success',
        data: {
          data: {
            resultType: 'matrix',
            result: [
              {
                metric: { __name__: 'test', job: 'testjob' },
                values: [[2 * 60, '3846']],
              },
            ],
          },
        },
      };
      backendSrv.datasourceRequest = jest.fn(() => Promise.resolve(response));
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds.query(query).subscribe((data: any) => {
        results = data;
      });
    });

    it('should generate the correct query', () => {
      const res = backendSrv.datasourceRequest.mock.calls[0][0];
      expect(res.method).toBe('POST');
      expect(res.url).toBe(urlExpected);
      expect(res.data).toEqual(dataExpected);
    });

    it('should return series list', () => {
      expect(results.data.length).toBe(1);
      expect(results.data[0].target).toBe('test{job="testjob"}');
    });
  });

  describe('When querying prometheus via check headers X-Dashboard-Id and X-Panel-Id', () => {
    const options = { dashboardId: 1, panelId: 2 };

    const httpOptions = {
      headers: {} as { [key: string]: number | undefined },
    };

    it('with proxy access tracing headers should be added', () => {
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds._addTracingHeaders(httpOptions, options);
      expect(httpOptions.headers['X-Dashboard-Id']).toBe(1);
      expect(httpOptions.headers['X-Panel-Id']).toBe(2);
    });

    it('with direct access tracing headers should not be added', () => {
      instanceSettings.url = 'http://127.0.0.1:8000';
      ctx.ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
      ctx.ds._addTracingHeaders(httpOptions, options);
      expect(httpOptions.headers['X-Dashboard-Id']).toBe(undefined);
      expect(httpOptions.headers['X-Panel-Id']).toBe(undefined);
    });
  });
});

const getPrepareTargetsContext = (target: PromQuery) => {
  const instanceSettings = ({
    url: 'proxied',
    directUrl: 'direct',
    user: 'test',
    password: 'mupp',
    jsonData: { httpMethod: 'POST' },
  } as unknown) as DataSourceInstanceSettings<PromOptions>;
  const start = 0;
  const end = 1;
  const panelId = '2';
  const options = ({ targets: [target], interval: '1s', panelId } as any) as DataQueryRequest<PromQuery>;

  const ds = new PrometheusDatasource(instanceSettings, q, backendSrv as any, templateSrv as any, timeSrv as any);
  const { queries, activeTargets } = ds.prepareTargets(options, start, end);

  return {
    queries,
    activeTargets,
    start,
    end,
    panelId,
  };
};

describe('prepareTargets', () => {
  describe('when run from a Panel', () => {
    it('then it should just add targets', () => {
      const target: PromQuery = {
        refId: 'A',
        expr: 'up',
        context: PromContext.Panel,
      };

      const { queries, activeTargets, panelId, end, start } = getPrepareTargetsContext(target);

      expect(queries.length).toBe(1);
      expect(activeTargets.length).toBe(1);
      expect(queries[0]).toEqual({
        end,
        expr: 'up',
        headers: {
          'X-Dashboard-Id': undefined,
          'X-Panel-Id': panelId,
        },
        hinting: undefined,
        instant: undefined,
        refId: target.refId,
        requestId: panelId + target.refId,
        start,
        step: 1,
      });
      expect(activeTargets[0]).toEqual(target);
    });
  });

  describe('when run from Explore', () => {
    describe('and both Graph and Table are shown', () => {
      it('then it should return both instant and time series related objects', () => {
        const target: PromQuery = {
          refId: 'A',
          expr: 'up',
          context: PromContext.Explore,
          showingGraph: true,
          showingTable: true,
        };

        const { queries, activeTargets, panelId, end, start } = getPrepareTargetsContext(target);

        expect(queries.length).toBe(2);
        expect(activeTargets.length).toBe(2);
        expect(queries[0]).toEqual({
          end,
          expr: 'up',
          headers: {
            'X-Dashboard-Id': undefined,
            'X-Panel-Id': panelId,
          },
          hinting: undefined,
          instant: true,
          refId: target.refId,
          requestId: panelId + target.refId + '_instant',
          start,
          step: 1,
        });
        expect(activeTargets[0]).toEqual({
          ...target,
          format: 'table',
          instant: true,
          requestId: panelId + target.refId + '_instant',
          valueWithRefId: true,
        });
        expect(queries[1]).toEqual({
          end,
          expr: 'up',
          headers: {
            'X-Dashboard-Id': undefined,
            'X-Panel-Id': panelId,
          },
          hinting: undefined,
          instant: false,
          refId: target.refId,
          requestId: panelId + target.refId,
          start,
          step: 1,
        });
        expect(activeTargets[1]).toEqual({
          ...target,
          format: 'time_series',
          instant: false,
          requestId: panelId + target.refId,
        });
      });
    });
    describe('and both Graph and Table are hidden', () => {
      it('then it should return empty arrays', () => {
        const target: PromQuery = {
          refId: 'A',
          expr: 'up',
          context: PromContext.Explore,
          showingGraph: false,
          showingTable: false,
        };

        const { queries, activeTargets } = getPrepareTargetsContext(target);

        expect(queries.length).toBe(0);
        expect(activeTargets.length).toBe(0);
      });
    });

    describe('and Graph is hidden', () => {
      it('then it should return only intant related objects', () => {
        const target: PromQuery = {
          refId: 'A',
          expr: 'up',
          context: PromContext.Explore,
          showingGraph: false,
          showingTable: true,
        };

        const { queries, activeTargets, panelId, end, start } = getPrepareTargetsContext(target);

        expect(queries.length).toBe(1);
        expect(activeTargets.length).toBe(1);
        expect(queries[0]).toEqual({
          end,
          expr: 'up',
          headers: {
            'X-Dashboard-Id': undefined,
            'X-Panel-Id': panelId,
          },
          hinting: undefined,
          instant: true,
          refId: target.refId,
          requestId: panelId + target.refId + '_instant',
          start,
          step: 1,
        });
        expect(activeTargets[0]).toEqual({
          ...target,
          format: 'table',
          instant: true,
          requestId: panelId + target.refId + '_instant',
          valueWithRefId: true,
        });
      });
    });

    describe('and Table is hidden', () => {
      it('then it should return only time series related objects', () => {
        const target: PromQuery = {
          refId: 'A',
          expr: 'up',
          context: PromContext.Explore,
          showingGraph: true,
          showingTable: false,
        };

        const { queries, activeTargets, panelId, end, start } = getPrepareTargetsContext(target);

        expect(queries.length).toBe(1);
        expect(activeTargets.length).toBe(1);
        expect(queries[0]).toEqual({
          end,
          expr: 'up',
          headers: {
            'X-Dashboard-Id': undefined,
            'X-Panel-Id': panelId,
          },
          hinting: undefined,
          instant: false,
          refId: target.refId,
          requestId: panelId + target.refId,
          start,
          step: 1,
        });
        expect(activeTargets[0]).toEqual({
          ...target,
          format: 'time_series',
          instant: false,
          requestId: panelId + target.refId,
        });
      });
    });
  });
});
