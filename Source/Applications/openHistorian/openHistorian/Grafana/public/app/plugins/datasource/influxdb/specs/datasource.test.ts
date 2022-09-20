import { lastValueFrom, of } from 'rxjs';
import { TemplateSrvStub } from 'test/specs/helpers';

import { ScopedVars } from '@grafana/data/src';
import { FetchResponse } from '@grafana/runtime';
import config from 'app/core/config';
import { backendSrv } from 'app/core/services/backend_srv'; // will use the version in __mocks__

import InfluxDatasource from '../datasource';

//@ts-ignore
const templateSrv = new TemplateSrvStub();

jest.mock('@grafana/runtime', () => ({
  ...(jest.requireActual('@grafana/runtime') as unknown as object),
  getBackendSrv: () => backendSrv,
}));

describe('InfluxDataSource', () => {
  const ctx: any = {
    instanceSettings: { url: 'url', name: 'influxDb', jsonData: { httpMode: 'GET' } },
  };

  const fetchMock = jest.spyOn(backendSrv, 'fetch');

  beforeEach(() => {
    jest.clearAllMocks();
    ctx.instanceSettings.url = '/api/datasources/proxy/1';
    ctx.ds = new InfluxDatasource(ctx.instanceSettings, templateSrv);
  });

  describe('When issuing metricFindQuery', () => {
    const query = 'SELECT max(value) FROM measurement WHERE $timeFilter';
    const queryOptions: any = {
      range: {
        from: '2018-01-01T00:00:00Z',
        to: '2018-01-02T00:00:00Z',
      },
    };
    let requestQuery: any, requestMethod: any, requestData: any, response: any;

    beforeEach(async () => {
      fetchMock.mockImplementation((req: any) => {
        requestMethod = req.method;
        requestQuery = req.params.q;
        requestData = req.data;
        return of({
          data: {
            status: 'success',
            results: [
              {
                series: [
                  {
                    name: 'measurement',
                    columns: ['name'],
                    values: [['cpu']],
                  },
                ],
              },
            ],
          },
        } as FetchResponse);
      });

      response = await ctx.ds.metricFindQuery(query, queryOptions);
    });

    it('should replace $timefilter', () => {
      expect(requestQuery).toMatch('time >= 1514764800000ms and time <= 1514851200000ms');
    });

    it('should use the HTTP GET method', () => {
      expect(requestMethod).toBe('GET');
    });

    it('should not have any data in request body', () => {
      expect(requestData).toBeNull();
    });

    it('parse response correctly', () => {
      expect(response).toEqual([{ text: 'cpu' }]);
    });
  });

  describe('When getting error on 200 after issuing a query', () => {
    const queryOptions = {
      range: {
        from: '2018-01-01T00:00:00Z',
        to: '2018-01-02T00:00:00Z',
      },
      rangeRaw: {
        from: '2018-01-01T00:00:00Z',
        to: '2018-01-02T00:00:00Z',
      },
      targets: [{}],
      timezone: 'UTC',
      scopedVars: {
        interval: { text: '1m', value: '1m' },
        __interval: { text: '1m', value: '1m' },
        __interval_ms: { text: 60000, value: 60000 },
      },
    };

    it('throws an error', async () => {
      fetchMock.mockImplementation(() => {
        return of({
          data: {
            results: [
              {
                error: 'Query timeout',
              },
            ],
          },
        } as FetchResponse);
      });

      try {
        await lastValueFrom(ctx.ds.query(queryOptions));
      } catch (err) {
        if (err instanceof Error) {
          expect(err.message).toBe('InfluxDB Error: Query timeout');
        }
      }
    });
  });

  describe('InfluxDataSource in POST query mode', () => {
    const ctx: any = {
      instanceSettings: { url: 'url', name: 'influxDb', jsonData: { httpMode: 'POST' } },
    };

    beforeEach(() => {
      ctx.instanceSettings.url = '/api/datasources/proxy/1';
      ctx.ds = new InfluxDatasource(ctx.instanceSettings, templateSrv);
    });

    describe('When issuing metricFindQuery', () => {
      const query = 'SELECT max(value) FROM measurement';
      const queryOptions: any = {};
      let requestMethod: any, requestQueryParameter: any, queryEncoded: any, requestQuery: any;

      beforeEach(async () => {
        fetchMock.mockImplementation((req: any) => {
          requestMethod = req.method;
          requestQueryParameter = req.params;
          requestQuery = req.data;
          return of({
            data: {
              results: [
                {
                  series: [
                    {
                      name: 'measurement',
                      columns: ['max'],
                      values: [[1]],
                    },
                  ],
                },
              ],
            },
          } as FetchResponse);
        });

        queryEncoded = await ctx.ds.serializeParams({ q: query });
        await ctx.ds.metricFindQuery(query, queryOptions).then(() => {});
      });

      it('should have the query form urlencoded', () => {
        expect(requestQuery).toBe(queryEncoded);
      });

      it('should use the HTTP POST method', () => {
        expect(requestMethod).toBe('POST');
      });

      it('should not have q as a query parameter', () => {
        expect(requestQueryParameter).not.toHaveProperty('q');
      });
    });
  });

  describe('Variables should be interpolated correctly', () => {
    const instanceSettings: any = {};
    const text = 'interpolationText';
    const text2 = 'interpolationText2';
    const textWithoutFormatRegex = 'interpolationText,interpolationText2';
    const textWithFormatRegex = 'interpolationText|interpolationText2';
    const variableMap: Record<string, string> = {
      $interpolationVar: text,
      $interpolationVar2: text2,
    };
    const templateSrv: any = {
      replace: jest.fn((target?: string, scopedVars?: ScopedVars, format?: string | Function): string => {
        if (!format) {
          return variableMap[target!] || '';
        }
        if (format === 'regex') {
          return textWithFormatRegex;
        }
        return textWithoutFormatRegex;
      }),
    };
    const ds = new InfluxDatasource(instanceSettings, templateSrv);

    const influxQuery = {
      refId: 'x',
      alias: '$interpolationVar',
      measurement: '$interpolationVar',
      policy: '$interpolationVar',
      limit: '$interpolationVar',
      slimit: '$interpolationVar',
      tz: '$interpolationVar',
      tags: [
        {
          key: 'cpu',
          operator: '=~',
          value: '/^$interpolationVar,$interpolationVar2$/',
        },
      ],
      groupBy: [
        {
          params: ['$interpolationVar'],
          type: 'tag',
        },
      ],
      select: [
        [
          {
            params: ['$interpolationVar'],
            type: 'field',
          },
        ],
      ],
    };

    function influxChecks(query: any) {
      expect(templateSrv.replace).toBeCalledTimes(10);
      expect(query.alias).toBe(text);
      expect(query.measurement).toBe(textWithFormatRegex);
      expect(query.policy).toBe(textWithFormatRegex);
      expect(query.limit).toBe(textWithFormatRegex);
      expect(query.slimit).toBe(textWithFormatRegex);
      expect(query.tz).toBe(text);
      expect(query.tags![0].value).toBe(textWithFormatRegex);
      expect(query.groupBy![0].params![0]).toBe(textWithFormatRegex);
      expect(query.select![0][0].params![0]).toBe(textWithFormatRegex);
    }

    describe('when interpolating query variables for dashboard->explore', () => {
      it('should interpolate all variables with Flux mode', () => {
        ds.isFlux = true;
        const fluxQuery = {
          refId: 'x',
          query: '$interpolationVar,$interpolationVar2',
        };
        const queries = ds.interpolateVariablesInQueries([fluxQuery], {
          interpolationVar: { text: text, value: text },
          interpolationVar2: { text: text2, value: text2 },
        });
        expect(templateSrv.replace).toBeCalledTimes(1);
        expect(queries[0].query).toBe(textWithFormatRegex);
      });

      it('should interpolate all variables with InfluxQL mode', () => {
        ds.isFlux = false;
        const queries = ds.interpolateVariablesInQueries([influxQuery], {
          interpolationVar: { text: text, value: text },
          interpolationVar2: { text: text2, value: text2 },
        });
        influxChecks(queries[0]);
      });
    });

    describe('when interpolating template variables', () => {
      it('should apply all template variables with Flux mode', () => {
        ds.isFlux = true;
        const fluxQuery = {
          refId: 'x',
          query: '$interpolationVar',
        };
        const query = ds.applyTemplateVariables(fluxQuery, {
          interpolationVar: {
            text: text,
            value: text,
          },
        });
        expect(templateSrv.replace).toBeCalledTimes(1);
        expect(query.query).toBe(text);
      });

      it('should apply all template variables with InfluxQL mode', () => {
        ds.isFlux = false;
        ds.access = 'proxy';
        config.featureToggles.influxdbBackendMigration = true;
        const query = ds.applyTemplateVariables(influxQuery, {
          interpolationVar: { text: text, value: text },
          interpolationVar2: { text: 'interpolationText2', value: 'interpolationText2' },
        });
        influxChecks(query);
      });
    });
  });
});
