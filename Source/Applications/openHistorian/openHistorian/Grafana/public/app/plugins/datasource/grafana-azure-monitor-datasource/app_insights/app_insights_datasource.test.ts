import { DataFrame, getFrameDisplayName, toUtc } from '@grafana/data';
import { TemplateSrv } from 'app/features/templating/template_srv';
import { backendSrv } from 'app/core/services/backend_srv';
import { setBackendSrv } from '@grafana/runtime';
import AppInsightsDatasource from './app_insights_datasource';
import { of } from 'rxjs';

const templateSrv = new TemplateSrv();

jest.mock('app/core/services/backend_srv');
jest.mock('@grafana/runtime', () => ({
  ...((jest.requireActual('@grafana/runtime') as unknown) as object),
  getBackendSrv: () => backendSrv,
  getTemplateSrv: () => templateSrv,
}));

describe('AppInsightsDatasource', () => {
  const datasourceRequestMock = jest.spyOn(backendSrv, 'datasourceRequest');
  const fetchMock = jest.spyOn(backendSrv, 'fetch');

  const ctx: any = {};

  beforeEach(() => {
    jest.clearAllMocks();
    setBackendSrv(backendSrv);

    ctx.instanceSettings = {
      jsonData: { appInsightsAppId: '3ad4400f-ea7d-465d-a8fb-43fb20555d85' },
      url: 'http://appinsightsapi',
    };

    ctx.ds = new AppInsightsDatasource(ctx.instanceSettings);
  });

  describe('When performing testDatasource', () => {
    describe('and a list of metrics is returned', () => {
      const response = {
        metrics: {
          'requests/count': {
            displayName: 'Server requests',
            defaultAggregation: 'sum',
          },
          'requests/duration': {
            displayName: 'Server requests',
            defaultAggregation: 'sum',
          },
        },
        dimensions: {
          'request/source': {
            displayName: 'Request source',
          },
        },
      };

      beforeEach(() => {
        datasourceRequestMock.mockImplementation(() => {
          return Promise.resolve({ data: response, status: 200 });
        });
      });

      it('should return success status', () => {
        return ctx.ds.testDatasource().then((results: any) => {
          expect(results.status).toEqual('success');
        });
      });
    });

    describe('and a PathNotFoundError error is returned', () => {
      const error = {
        data: {
          error: {
            code: 'PathNotFoundError',
            message: `An error message.`,
          },
        },
        status: 404,
        statusText: 'Not Found',
      };

      beforeEach(() => {
        datasourceRequestMock.mockImplementation(() => {
          return Promise.reject(error);
        });
      });

      it.skip('should return error status and a detailed error message', () => {
        return ctx.ds.testDatasource().then((results: any) => {
          expect(results.status).toEqual('error');
          expect(results.message).toEqual(
            '1. Application Insights: Not Found: Invalid Application Id for Application Insights service. '
          );
        });
      });
    });

    describe('and an error is returned', () => {
      const error = {
        data: {
          error: {
            code: 'SomeOtherError',
            message: `An error message.`,
          },
        },
        status: 500,
        statusText: 'Error',
      };

      beforeEach(() => {
        datasourceRequestMock.mockImplementation(() => {
          return Promise.reject(error);
        });
      });

      it.skip('should return error status and a detailed error message', () => {
        return ctx.ds.testDatasource().then((results: any) => {
          expect(results.status).toEqual('error');
          expect(results.message).toEqual('1. Application Insights: Error: SomeOtherError. An error message. ');
        });
      });
    });
  });

  describe('When performing raw query', () => {
    const queryString =
      'metrics ' +
      '| where $__timeFilter(timestamp) ' +
      '| where name == "testMetrics" ' +
      '| summarize max=max(valueMax) by bin(timestamp, $__interval), partition';

    const options = {
      range: {
        from: toUtc('2017-08-22T20:00:00Z'),
        to: toUtc('2017-08-22T23:59:00Z'),
      },
      targets: [
        {
          apiVersion: '2016-09-01',
          refId: 'A',
          queryType: 'Application Insights',
          appInsights: {
            rawQuery: true,
            rawQueryString: queryString,
            timeColumn: 'timestamp',
            valueColumn: 'max',
            segmentColumn: (undefined as unknown) as string,
          },
        },
      ],
    };

    describe('with no grouping', () => {
      const response: any = {
        results: {
          A: {
            refId: 'A',
            meta: {},
            series: [
              {
                name: 'PrimaryResult',
                points: [[2.2075, 1558278660000]],
              },
            ],
            tables: null,
          },
        },
      };

      beforeEach(() => {
        fetchMock.mockImplementation((options: any) => {
          expect(options.url).toContain('/api/ds/query');
          expect(options.data.queries.length).toBe(1);
          expect(options.data.queries[0].refId).toBe('A');
          return of({ data: response, status: 200 } as any);
        });
      });

      it('should return a list of datapoints', () => {
        return ctx.ds
          .query(options)
          .toPromise()
          .then((results: any) => {
            expect(results.data.length).toBe(1);
            const data = results.data[0] as DataFrame;
            expect(getFrameDisplayName(data)).toEqual('PrimaryResult');
            expect(data.fields[0].values.length).toEqual(1);
            expect(data.fields[0].values.get(0)).toEqual(1558278660000);
            expect(data.fields[1].values.get(0)).toEqual(2.2075);
          });
      });
    });

    describe('with grouping', () => {
      const response: any = {
        results: {
          A: {
            refId: 'A',
            meta: {},
            series: [
              {
                name: 'paritionA',
                points: [[2.2075, 1558278660000]],
              },
            ],
            tables: null,
          },
        },
      };

      beforeEach(() => {
        options.targets[0].appInsights.segmentColumn = 'partition';
        fetchMock.mockImplementation((options: any) => {
          expect(options.url).toContain('/api/ds/query');
          expect(options.data.queries.length).toBe(1);
          expect(options.data.queries[0].refId).toBe('A');
          return of({ data: response, status: 200 } as any);
        });
      });

      it('should return a list of datapoints', () => {
        return ctx.ds
          .query(options)
          .toPromise()
          .then((results: any) => {
            expect(results.data.length).toBe(1);
            const data = results.data[0] as DataFrame;
            expect(getFrameDisplayName(data)).toEqual('paritionA');
            expect(data.fields[0].values.length).toEqual(1);
            expect(data.fields[0].values.get(0)).toEqual(1558278660000);
            expect(data.fields[1].values.get(0)).toEqual(2.2075);
          });
      });
    });
  });

  describe('When performing metric query', () => {
    const options = {
      range: {
        from: toUtc('2017-08-22T20:00:00Z'),
        to: toUtc('2017-08-22T23:59:00Z'),
      },
      targets: [
        {
          apiVersion: '2016-09-01',
          refId: 'A',
          queryType: 'Application Insights',
          appInsights: {
            metricName: 'exceptions/server',
            dimension: '',
            timeGrain: 'none',
          },
        },
      ],
    };

    describe('and with a single value', () => {
      const response: any = {
        results: {
          A: {
            refId: 'A',
            meta: {},
            series: [
              {
                name: 'exceptions/server',
                points: [[2.2075, 1558278660000]],
              },
            ],
            tables: null,
          },
        },
      };

      beforeEach(() => {
        fetchMock.mockImplementation((options: any) => {
          expect(options.url).toContain('/api/ds/query');
          expect(options.data.queries.length).toBe(1);
          expect(options.data.queries[0].refId).toBe('A');
          expect(options.data.queries[0].appInsights.rawQueryString).toBeUndefined();
          expect(options.data.queries[0].appInsights.metricName).toBe('exceptions/server');
          return of({ data: response, status: 200 } as any);
        });
      });

      it('should return a single datapoint', () => {
        return ctx.ds
          .query(options)
          .toPromise()
          .then((results: any) => {
            expect(results.data.length).toBe(1);
            const data = results.data[0] as DataFrame;
            expect(getFrameDisplayName(data)).toEqual('exceptions/server');
            expect(data.fields[0].values.get(0)).toEqual(1558278660000);
            expect(data.fields[1].values.get(0)).toEqual(2.2075);
          });
      });
    });

    describe('and with an interval group and without a segment group by', () => {
      const response: any = {
        results: {
          A: {
            refId: 'A',
            meta: {},
            series: [
              {
                name: 'exceptions/server',
                points: [
                  [3, 1504108800000],
                  [6, 1504112400000],
                ],
              },
            ],
            tables: null,
          },
        },
      };

      beforeEach(() => {
        options.targets[0].appInsights.timeGrain = 'PT30M';
        fetchMock.mockImplementation((options: any) => {
          expect(options.url).toContain('/api/ds/query');
          expect(options.data.queries[0].refId).toBe('A');
          expect(options.data.queries[0].appInsights.query).toBeUndefined();
          expect(options.data.queries[0].appInsights.metricName).toBe('exceptions/server');
          expect(options.data.queries[0].appInsights.timeGrain).toBe('PT30M');
          return of({ data: response, status: 200 } as any);
        });
      });

      it('should return a list of datapoints', () => {
        return ctx.ds
          .query(options)
          .toPromise()
          .then((results: any) => {
            expect(results.data.length).toBe(1);
            const data = results.data[0] as DataFrame;
            expect(getFrameDisplayName(data)).toEqual('exceptions/server');
            expect(data.fields[0].values.length).toEqual(2);
            expect(data.fields[0].values.get(0)).toEqual(1504108800000);
            expect(data.fields[1].values.get(0)).toEqual(3);
            expect(data.fields[0].values.get(1)).toEqual(1504112400000);
            expect(data.fields[1].values.get(1)).toEqual(6);
          });
      });
    });

    describe('and with a group by', () => {
      const response: any = {
        results: {
          A: {
            refId: 'A',
            meta: {},
            series: [
              {
                name: 'exceptions/server{client/city="Miami"}',
                points: [
                  [10, 1504108800000],
                  [20, 1504112400000],
                ],
              },
              {
                name: 'exceptions/server{client/city="San Antonio"}',
                points: [
                  [1, 1504108800000],
                  [2, 1504112400000],
                ],
              },
            ],
            tables: null,
          },
        },
      };

      describe('and with no alias specified', () => {
        beforeEach(() => {
          options.targets[0].appInsights.dimension = 'client/city';

          fetchMock.mockImplementation((options: any) => {
            expect(options.url).toContain('/api/ds/query');
            expect(options.data.queries[0].appInsights.rawQueryString).toBeUndefined();
            expect(options.data.queries[0].appInsights.metricName).toBe('exceptions/server');
            expect([...options.data.queries[0].appInsights.dimension]).toMatchObject(['client/city']);
            return of({ data: response, status: 200 } as any);
          });
        });

        it('should return a list of datapoints', () => {
          return ctx.ds
            .query(options)
            .toPromise()
            .then((results: any) => {
              expect(results.data.length).toBe(2);
              let data = results.data[0] as DataFrame;
              expect(getFrameDisplayName(data)).toEqual('exceptions/server{client/city="Miami"}');
              expect(data.fields[1].values.length).toEqual(2);
              expect(data.fields[0].values.get(0)).toEqual(1504108800000);
              expect(data.fields[1].values.get(0)).toEqual(10);
              expect(data.fields[0].values.get(1)).toEqual(1504112400000);
              expect(data.fields[1].values.get(1)).toEqual(20);
              data = results.data[1] as DataFrame;
              expect(getFrameDisplayName(data)).toEqual('exceptions/server{client/city="San Antonio"}');
              expect(data.fields[1].values.length).toEqual(2);
              expect(data.fields[0].values.get(0)).toEqual(1504108800000);
              expect(data.fields[1].values.get(0)).toEqual(1);
              expect(data.fields[0].values.get(1)).toEqual(1504112400000);
              expect(data.fields[1].values.get(1)).toEqual(2);
            });
        });
      });
    });
  });

  describe('When performing metricFindQuery', () => {
    describe('with a metric names query', () => {
      const response = {
        metrics: {
          'exceptions/server': {},
          'requests/count': {},
        },
      };

      beforeEach(() => {
        datasourceRequestMock.mockImplementation((options: { url: string }) => {
          expect(options.url).toContain('/metrics/metadata');
          return Promise.resolve({ data: response, status: 200 });
        });
      });

      it('should return a list of metric names', () => {
        return ctx.ds.metricFindQueryInternal('appInsightsMetricNames()').then((results: any) => {
          expect(results.length).toBe(2);
          expect(results[0].text).toBe('exceptions/server');
          expect(results[0].value).toBe('exceptions/server');
          expect(results[1].text).toBe('requests/count');
          expect(results[1].value).toBe('requests/count');
        });
      });
    });

    describe('with metadata group by query', () => {
      const response = {
        metrics: {
          'exceptions/server': {
            supportedAggregations: ['sum'],
            supportedGroupBy: {
              all: ['client/os', 'client/city', 'client/browser'],
            },
            defaultAggregation: 'sum',
          },
          'requests/count': {
            supportedAggregations: ['avg', 'sum', 'total'],
            supportedGroupBy: {
              all: ['client/os', 'client/city', 'client/browser'],
            },
            defaultAggregation: 'avg',
          },
        },
      };

      beforeEach(() => {
        datasourceRequestMock.mockImplementation((options: { url: string }) => {
          expect(options.url).toContain('/metrics/metadata');
          return Promise.resolve({ data: response, status: 200 });
        });
      });

      it('should return a list of group bys', () => {
        return ctx.ds.metricFindQueryInternal('appInsightsGroupBys(requests/count)').then((results: any) => {
          expect(results[0].text).toContain('client/os');
          expect(results[0].value).toContain('client/os');
          expect(results[1].text).toContain('client/city');
          expect(results[1].value).toContain('client/city');
          expect(results[2].text).toContain('client/browser');
          expect(results[2].value).toContain('client/browser');
        });
      });
    });
  });

  describe('When getting Metric Names', () => {
    const response = {
      metrics: {
        'exceptions/server': {},
        'requests/count': {},
      },
    };

    beforeEach(() => {
      datasourceRequestMock.mockImplementation((options: { url: string }) => {
        expect(options.url).toContain('/metrics/metadata');
        return Promise.resolve({ data: response, status: 200 });
      });
    });

    it.skip('should return a list of metric names', () => {
      return ctx.ds.getAppInsightsMetricNames().then((results: any) => {
        expect(results.length).toBe(2);
        expect(results[0].text).toBe('exceptions/server');
        expect(results[0].value).toBe('exceptions/server');
        expect(results[1].text).toBe('requests/count');
        expect(results[1].value).toBe('requests/count');
      });
    });
  });

  describe('When getting Metric Metadata', () => {
    const response = {
      metrics: {
        'exceptions/server': {
          supportedAggregations: ['sum'],
          supportedGroupBy: {
            all: ['client/os', 'client/city', 'client/browser'],
          },
          defaultAggregation: 'sum',
        },
        'requests/count': {
          supportedAggregations: ['avg', 'sum', 'total'],
          supportedGroupBy: {
            all: ['client/os', 'client/city', 'client/browser'],
          },
          defaultAggregation: 'avg',
        },
      },
    };

    beforeEach(() => {
      datasourceRequestMock.mockImplementation((options: { url: string }) => {
        expect(options.url).toContain('/metrics/metadata');
        return Promise.resolve({ data: response, status: 200 });
      });
    });

    it.skip('should return a list of group bys', () => {
      return ctx.ds.getAppInsightsMetricMetadata('requests/count').then((results: any) => {
        expect(results.primaryAggType).toEqual('avg');
        expect(results.supportedAggTypes).toContain('avg');
        expect(results.supportedAggTypes).toContain('sum');
        expect(results.supportedAggTypes).toContain('total');
        expect(results.supportedGroupBy).toContain('client/os');
        expect(results.supportedGroupBy).toContain('client/city');
        expect(results.supportedGroupBy).toContain('client/browser');
      });
    });
  });
});
