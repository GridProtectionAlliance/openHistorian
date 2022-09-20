import { map } from 'lodash';
import { Observable, of, throwError } from 'rxjs';

import {
  ArrayVector,
  CoreApp,
  DataLink,
  DataQueryRequest,
  DataSourceInstanceSettings,
  DataSourcePluginMeta,
  dateMath,
  DateTime,
  dateTime,
  Field,
  MutableDataFrame,
  TimeRange,
  toUtc,
} from '@grafana/data';
import { BackendSrvRequest, FetchResponse } from '@grafana/runtime';
import { backendSrv } from 'app/core/services/backend_srv'; // will use the version in __mocks__

import { createFetchResponse } from '../../../../test/helpers/createFetchResponse';

import { Filters } from './components/QueryEditor/BucketAggregationsEditor/aggregations';
import { ElasticDatasource, enhanceDataFrame } from './datasource';
import { ElasticsearchOptions, ElasticsearchQuery } from './types';

const ELASTICSEARCH_MOCK_URL = 'http://elasticsearch.local';

jest.mock('@grafana/runtime', () => ({
  ...(jest.requireActual('@grafana/runtime') as unknown as object),
  getBackendSrv: () => backendSrv,
  getDataSourceSrv: () => {
    return {
      getInstanceSettings: () => {
        return { name: 'elastic25' };
      },
    };
  },
}));

const createTimeRange = (from: DateTime, to: DateTime): TimeRange => ({
  from,
  to,
  raw: {
    from,
    to,
  },
});

interface Args {
  data?: any;
  from?: string;
  jsonData?: any;
  database?: string;
  mockImplementation?: (options: BackendSrvRequest) => Observable<FetchResponse>;
}

function getTestContext({
  data = {},
  from = 'now-5m',
  jsonData = {},
  database = '[asd-]YYYY.MM.DD',
  mockImplementation = undefined,
}: Args = {}) {
  jest.clearAllMocks();

  const defaultMock = (options: BackendSrvRequest) => of(createFetchResponse(data));

  const fetchMock = jest.spyOn(backendSrv, 'fetch');
  fetchMock.mockImplementation(mockImplementation ?? defaultMock);

  const templateSrv: any = {
    replace: jest.fn((text?: string) => {
      if (text?.startsWith('$')) {
        return `resolvedVariable`;
      } else {
        return text;
      }
    }),
    getAdhocFilters: jest.fn(() => []),
  };

  const timeSrv: any = {
    time: { from, to: 'now' },
  };

  timeSrv.timeRange = jest.fn(() => {
    return {
      from: dateMath.parse(timeSrv.time.from, false),
      to: dateMath.parse(timeSrv.time.to, true),
    };
  });

  timeSrv.setTime = jest.fn((time) => {
    timeSrv.time = time;
  });

  const instanceSettings: DataSourceInstanceSettings<ElasticsearchOptions> = {
    id: 1,
    meta: {} as DataSourcePluginMeta,
    name: 'test-elastic',
    type: 'type',
    uid: 'uid',
    access: 'proxy',
    url: ELASTICSEARCH_MOCK_URL,
    database,
    jsonData,
  };

  const ds = new ElasticDatasource(instanceSettings, templateSrv);

  return { timeSrv, ds, fetchMock };
}

describe('ElasticDatasource', function (this: any) {
  describe('When testing datasource with index pattern', () => {
    it('should translate index pattern to current day', () => {
      const { ds, fetchMock } = getTestContext({ jsonData: { interval: 'Daily', esVersion: '7.10.0' } });

      ds.testDatasource();

      const today = toUtc().format('YYYY.MM.DD');
      expect(fetchMock).toHaveBeenCalledTimes(1);
      expect(fetchMock.mock.calls[0][0].url).toBe(`${ELASTICSEARCH_MOCK_URL}/asd-${today}/_mapping`);
    });
  });

  describe('When issuing metric query with interval pattern', () => {
    async function runScenario() {
      const range = { from: toUtc([2015, 4, 30, 10]), to: toUtc([2015, 5, 1, 10]) };
      const targets = [
        {
          alias: '$varAlias',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
          metrics: [{ type: 'count', id: '1' }],
          query: 'escape\\:test',
        },
      ];
      const query: any = { range, targets };
      const data = {
        responses: [
          {
            aggregations: {
              '1': {
                buckets: [
                  {
                    doc_count: 10,
                    key: 1000,
                  },
                ],
              },
            },
          },
        ],
      };
      const { ds, fetchMock } = getTestContext({ jsonData: { interval: 'Daily', esVersion: '7.10.0' }, data });

      let result: any = {};
      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual({
          data: [
            {
              datapoints: [[10, 1000]],
              metric: 'count',
              props: {},
              refId: undefined,
              target: 'resolvedVariable',
            },
          ],
        });
        result = received[0];
      });

      expect(fetchMock).toHaveBeenCalledTimes(1);
      const requestOptions = fetchMock.mock.calls[0][0];
      const parts = requestOptions.data.split('\n');
      const header = JSON.parse(parts[0]);
      const body = JSON.parse(parts[1]);

      return { result, body, header, query };
    }

    it('should translate index pattern to current day', async () => {
      const { header } = await runScenario();
      expect(header.index).toEqual(['asd-2015.05.30', 'asd-2015.05.31', 'asd-2015.06.01']);
    });

    it('should not resolve the variable in the original alias field in the query', async () => {
      const { query } = await runScenario();
      expect(query.targets[0].alias).toEqual('$varAlias');
    });

    it('should resolve the alias variable for the alias/target in the result', async () => {
      const { result } = await runScenario();
      expect(result.data[0].target).toEqual('resolvedVariable');
    });

    it('should json escape lucene query', async () => {
      const { body } = await runScenario();
      expect(body.query.bool.filter[1].query_string.query).toBe('escape\\:test');
    });
  });

  describe('When issuing logs query with interval pattern', () => {
    async function setupDataSource(jsonData?: Partial<ElasticsearchOptions>) {
      jsonData = {
        interval: 'Daily',
        esVersion: '7.10.0',
        timeField: '@timestamp',
        ...(jsonData || {}),
      };
      const { ds } = getTestContext({
        jsonData,
        data: logsResponse.data,
        database: 'mock-index',
      });

      const query: DataQueryRequest<ElasticsearchQuery> = {
        range: createTimeRange(toUtc([2015, 4, 30, 10]), toUtc([2019, 7, 1, 10])),
        targets: [
          {
            alias: '$varAlias',
            refId: 'A',
            bucketAggs: [
              {
                type: 'date_histogram',
                settings: { interval: 'auto' },
                id: '2',
              },
            ],
            metrics: [{ type: 'logs', id: '1' }],
            query: 'escape\\:test',
            timeField: '@timestamp',
          },
        ],
      } as DataQueryRequest<ElasticsearchQuery>;

      const queryBuilderSpy = jest.spyOn(ds.queryBuilder, 'getLogsQuery');
      let response: any = {};

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        response = received[0];
      });

      return { queryBuilderSpy, response };
    }

    it('should call getLogsQuery()', async () => {
      const { queryBuilderSpy } = await setupDataSource();
      expect(queryBuilderSpy).toHaveBeenCalled();
    });

    it('should enhance fields with links', async () => {
      const { response } = await setupDataSource({
        dataLinks: [
          {
            field: 'host',
            url: 'http://localhost:3000/${__value.raw}',
            urlDisplayLabel: 'Custom Label',
          },
        ],
      });

      expect(response.data.length).toBe(1);
      const links: DataLink[] = response.data[0].fields.find((field: Field) => field.name === 'host').config.links;
      expect(links.length).toBe(1);
      expect(links[0].url).toBe('http://localhost:3000/${__value.raw}');
      expect(links[0].title).toBe('Custom Label');
    });
  });

  describe('When issuing document query', () => {
    async function runScenario() {
      const range = createTimeRange(dateTime([2015, 4, 30, 10]), dateTime([2015, 5, 1, 10]));
      const targets = [{ refId: 'A', metrics: [{ type: 'raw_document', id: '1' }], query: 'test' }];
      const query: any = { range, targets };
      const data = { responses: [] };

      const { ds, fetchMock } = getTestContext({ jsonData: { esVersion: '7.10.0' }, data, database: 'test' });

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual({ data: [] });
      });

      expect(fetchMock).toHaveBeenCalledTimes(1);
      const requestOptions = fetchMock.mock.calls[0][0];
      const parts = requestOptions.data.split('\n');
      const header = JSON.parse(parts[0]);
      const body = JSON.parse(parts[1]);

      return { body, header };
    }

    it('should set search type to query_then_fetch', async () => {
      const { header } = await runScenario();
      expect(header.search_type).toEqual('query_then_fetch');
    });

    it('should set size', async () => {
      const { body } = await runScenario();
      expect(body.size).toBe(500);
    });
  });

  describe('When getting an error on response', () => {
    const query: DataQueryRequest<ElasticsearchQuery> = {
      range: createTimeRange(toUtc([2020, 1, 1, 10]), toUtc([2020, 2, 1, 10])),
      targets: [
        {
          refId: 'A',
          alias: '$varAlias',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '1' }],
          metrics: [{ type: 'count', id: '1' }],
          query: 'escape\\:test',
        },
      ],
    } as DataQueryRequest<ElasticsearchQuery>;

    it('should process it properly', async () => {
      const { ds } = getTestContext({
        jsonData: { interval: 'Daily', esVersion: '7.10.0' },
        data: {
          took: 1,
          responses: [
            {
              error: {
                reason: 'all shards failed',
              },
              status: 400,
            },
          ],
        },
      });

      const errObject = {
        data: '{\n    "reason": "all shards failed"\n}',
        message: 'all shards failed',
        config: {
          url: 'http://localhost:3000/api/ds/query',
        },
      };

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual(errObject);
      });
    });

    it('should properly throw an error with just a message', async () => {
      const response: FetchResponse = {
        data: {
          error: 'Bad Request',
          message: 'Authentication to data source failed',
        },
        status: 400,
        url: 'http://localhost:3000/api/ds/query',
        config: { url: 'http://localhost:3000/api/ds/query' },
        type: 'basic',
        statusText: 'Bad Request',
        redirected: false,
        headers: {} as unknown as Headers,
        ok: false,
      };

      const { ds } = getTestContext({
        mockImplementation: () => throwError(response),
        from: undefined,
        jsonData: { esVersion: '7.10.0' },
      });

      const errObject = {
        error: 'Bad Request',
        message: 'Elasticsearch error: Authentication to data source failed',
      };

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual(errObject);
      });
    });

    it('should properly throw an unknown error', async () => {
      const { ds } = getTestContext({
        jsonData: { interval: 'Daily', esVersion: '7.10.0' },
        data: {
          took: 1,
          responses: [
            {
              error: {},
              status: 400,
            },
          ],
        },
      });

      const errObject = {
        data: '{}',
        message: 'Unknown elastic error response',
        config: {
          url: 'http://localhost:3000/api/ds/query',
        },
      };

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual(errObject);
      });
    });
  });

  // describe('When getting fields', () => {
  // const data = {
  //   metricbeat: {
  //     mappings: {
  //       metricsets: {
  //         _all: {},
  //         _meta: {
  //           test: 'something',
  //         },
  //         properties: {
  //           '@timestamp': { type: 'date' },
  //           __timestamp: { type: 'date' },
  //           '@timestampnano': { type: 'date_nanos' },
  //           beat: {
  //             properties: {
  //               name: {
  //                 fields: { raw: { type: 'keyword' } },
  //                 type: 'string',
  //               },
  //               hostname: { type: 'string' },
  //             },
  //           },
  //           system: {
  //             properties: {
  //               cpu: {
  //                 properties: {
  //                   system: { type: 'float' },
  //                   user: { type: 'float' },
  //                 },
  //               },
  //               process: {
  //                 properties: {
  //                   cpu: {
  //                     properties: {
  //                       total: { type: 'float' },
  //                     },
  //                   },
  //                   name: { type: 'string' },
  //                 },
  //               },
  //             },
  //           },
  //         },
  //       },
  //     },
  //   },
  // };

  // it('should return nested fields', async () => {
  //   const { ds } = getTestContext({ data, jsonData: { esVersion: 50 }, database: 'metricbeat' });

  //   await expect(ds.getFields()).toEmitValuesWith((received) => {
  //     expect(received.length).toBe(1);
  //     const fieldObjects = received[0];
  //     const fields = map(fieldObjects, 'text');

  //     expect(fields).toEqual([
  //       '@timestamp',
  //       '__timestamp',
  //       '@timestampnano',
  //       'beat.name.raw',
  //       'beat.name',
  //       'beat.hostname',
  //       'system.cpu.system',
  //       'system.cpu.user',
  //       'system.process.cpu.total',
  //       'system.process.name',
  //     ]);
  //   });
  // });

  // it('should return number fields', async () => {
  //   const { ds } = getTestContext({ data, jsonData: { esVersion: 50 }, database: 'metricbeat' });

  //   await expect(ds.getFields(['number'])).toEmitValuesWith((received) => {
  //     expect(received.length).toBe(1);
  //     const fieldObjects = received[0];
  //     const fields = map(fieldObjects, 'text');

  //     expect(fields).toEqual(['system.cpu.system', 'system.cpu.user', 'system.process.cpu.total']);
  //   });
  // });

  //   it('should return date fields', async () => {
  //     const { ds } = getTestContext({ data, jsonData: { esVersion: 50 }, database: 'metricbeat' });

  //     await expect(ds.getFields(['date'])).toEmitValuesWith((received) => {
  //       expect(received.length).toBe(1);
  //       const fieldObjects = received[0];
  //       const fields = map(fieldObjects, 'text');

  //       expect(fields).toEqual(['@timestamp', '__timestamp', '@timestampnano']);
  //     });
  //   });
  // });

  describe('When getting field mappings on indices with gaps', () => {
    const basicResponse = {
      metricbeat: {
        mappings: {
          metricsets: {
            _all: {},
            properties: {
              '@timestamp': { type: 'date' },
              beat: {
                properties: {
                  hostname: { type: 'string' },
                },
              },
            },
          },
        },
      },
    };

    // const alternateResponse = {
    //   metricbeat: {
    //     mappings: {
    //       metricsets: {
    //         _all: {},
    //         properties: {
    //           '@timestamp': { type: 'date' },
    //         },
    //       },
    //     },
    //   },
    // };

    // it('should return fields of the newest available index', async () => {
    //   const twoDaysBefore = toUtc().subtract(2, 'day').format('YYYY.MM.DD');
    //   const threeDaysBefore = toUtc().subtract(3, 'day').format('YYYY.MM.DD');
    //   const baseUrl = `${ELASTICSEARCH_MOCK_URL}/asd-${twoDaysBefore}/_mapping`;
    //   const alternateUrl = `${ELASTICSEARCH_MOCK_URL}/asd-${threeDaysBefore}/_mapping`;

    //   const { ds, timeSrv } = getTestContext({
    //     from: 'now-2w',
    //     jsonData: { interval: 'Daily', esVersion: 50 },
    //     mockImplementation: (options) => {
    //       if (options.url === baseUrl) {
    //         return of(createFetchResponse(basicResponse));
    //       } else if (options.url === alternateUrl) {
    //         return of(createFetchResponse(alternateResponse));
    //       }
    //       return throwError({ status: 404 });
    //     },
    //   });

    //   const range = timeSrv.timeRange();

    //   await expect(ds.getFields(undefined, range)).toEmitValuesWith((received) => {
    //     expect(received.length).toBe(1);
    //     const fieldObjects = received[0];
    //     const fields = map(fieldObjects, 'text');
    //     expect(fields).toEqual(['@timestamp', 'beat.hostname']);
    //   });
    // });

    it('should not retry when ES is down', async () => {
      const twoDaysBefore = toUtc().subtract(2, 'day').format('YYYY.MM.DD');

      const { ds, timeSrv, fetchMock } = getTestContext({
        from: 'now-2w',
        jsonData: { interval: 'Daily', esVersion: '7.10.0' },
        mockImplementation: (options) => {
          if (options.url === `${ELASTICSEARCH_MOCK_URL}/asd-${twoDaysBefore}/_mapping`) {
            return of(createFetchResponse(basicResponse));
          }
          return throwError({ status: 500 });
        },
      });

      const range = timeSrv.timeRange();

      await expect(ds.getFields(undefined, range)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toStrictEqual({ status: 500 });
        expect(fetchMock).toBeCalledTimes(1);
      });
    });

    it('should not retry more than 7 indices', async () => {
      const { ds, timeSrv, fetchMock } = getTestContext({
        from: 'now-2w',
        jsonData: { interval: 'Daily', esVersion: '7.10.0' },
        mockImplementation: (options) => {
          return throwError({ status: 404 });
        },
      });
      const range = timeSrv.timeRange();

      await expect(ds.getFields(undefined, range)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toStrictEqual('Could not find an available index for this time range.');
        expect(fetchMock).toBeCalledTimes(7);
      });
    });
  });

  describe('When getting fields from ES 7.0', () => {
    const data = {
      'genuine.es7._mapping.response': {
        mappings: {
          properties: {
            '@timestamp_millis': {
              type: 'date',
              format: 'epoch_millis',
            },
            classification_terms: {
              type: 'keyword',
            },
            domains: {
              type: 'keyword',
            },
            ip_address: {
              type: 'ip',
            },
            justification_blob: {
              properties: {
                criterion: {
                  type: 'text',
                  fields: {
                    keyword: {
                      type: 'keyword',
                      ignore_above: 256,
                    },
                  },
                },
                overall_vote_score: {
                  type: 'float',
                },
                shallow: {
                  properties: {
                    jsi: {
                      properties: {
                        sdb: {
                          properties: {
                            dsel2: {
                              properties: {
                                'bootlegged-gille': {
                                  properties: {
                                    botness: {
                                      type: 'float',
                                    },
                                    general_algorithm_score: {
                                      type: 'float',
                                    },
                                  },
                                },
                                'uncombed-boris': {
                                  properties: {
                                    botness: {
                                      type: 'float',
                                    },
                                    general_algorithm_score: {
                                      type: 'float',
                                    },
                                  },
                                },
                              },
                            },
                          },
                        },
                      },
                    },
                  },
                },
              },
            },
            overall_vote_score: {
              type: 'float',
            },
            ua_terms_long: {
              type: 'keyword',
            },
            ua_terms_short: {
              type: 'keyword',
            },
          },
        },
      },
    };

    const dateFields = ['@timestamp_millis'];
    const numberFields = [
      'justification_blob.overall_vote_score',
      'justification_blob.shallow.jsi.sdb.dsel2.bootlegged-gille.botness',
      'justification_blob.shallow.jsi.sdb.dsel2.bootlegged-gille.general_algorithm_score',
      'justification_blob.shallow.jsi.sdb.dsel2.uncombed-boris.botness',
      'justification_blob.shallow.jsi.sdb.dsel2.uncombed-boris.general_algorithm_score',
      'overall_vote_score',
    ];

    it('should return nested fields', async () => {
      const { ds } = getTestContext({
        data,
        database: 'genuine.es7._mapping.response',
        jsonData: { esVersion: '7.10.0' },
      });

      await expect(ds.getFields()).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);

        const fieldObjects = received[0];
        const fields = map(fieldObjects, 'text');
        expect(fields).toEqual([
          '@timestamp_millis',
          'classification_terms',
          'domains',
          'ip_address',
          'justification_blob.criterion.keyword',
          'justification_blob.criterion',
          'justification_blob.overall_vote_score',
          'justification_blob.shallow.jsi.sdb.dsel2.bootlegged-gille.botness',
          'justification_blob.shallow.jsi.sdb.dsel2.bootlegged-gille.general_algorithm_score',
          'justification_blob.shallow.jsi.sdb.dsel2.uncombed-boris.botness',
          'justification_blob.shallow.jsi.sdb.dsel2.uncombed-boris.general_algorithm_score',
          'overall_vote_score',
          'ua_terms_long',
          'ua_terms_short',
        ]);
      });
    });

    it('should return number fields', async () => {
      const { ds } = getTestContext({
        data,
        database: 'genuine.es7._mapping.response',
        jsonData: { esVersion: '7.10.0' },
      });

      await expect(ds.getFields(['number'])).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);

        const fieldObjects = received[0];
        const fields = map(fieldObjects, 'text');
        expect(fields).toEqual(numberFields);
      });
    });

    it('should return date fields', async () => {
      const { ds } = getTestContext({
        data,
        database: 'genuine.es7._mapping.response',
        jsonData: { esVersion: '7.10.0' },
      });

      await expect(ds.getFields(['date'])).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);

        const fieldObjects = received[0];
        const fields = map(fieldObjects, 'text');
        expect(fields).toEqual(dateFields);
      });
    });
  });

  describe('When issuing aggregation query on es5.x', () => {
    async function runScenario() {
      const range = createTimeRange(dateTime([2015, 4, 30, 10]), dateTime([2015, 5, 1, 10]));
      const targets = [
        {
          refId: 'A',
          bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2' }],
          metrics: [{ type: 'count', id: '1' }],
          query: 'test',
        },
      ];
      const query: any = { range, targets };
      const data = { responses: [] };

      const { ds, fetchMock } = getTestContext({ jsonData: { esVersion: '7.10.0' }, data, database: 'test' });

      await expect(ds.query(query)).toEmitValuesWith((received) => {
        expect(received.length).toBe(1);
        expect(received[0]).toEqual({ data: [] });
      });

      expect(fetchMock).toHaveBeenCalledTimes(1);
      const requestOptions = fetchMock.mock.calls[0][0];
      const parts = requestOptions.data.split('\n');
      const header = JSON.parse(parts[0]);
      const body = JSON.parse(parts[1]);

      return { body, header };
    }

    it('should not set search type to count', async () => {
      const { header } = await runScenario();
      expect(header.search_type).not.toEqual('count');
    });

    it('should set size to 0', async () => {
      const { body } = await runScenario();
      expect(body.size).toBe(0);
    });
  });

  describe('When issuing metricFind query on es5.x', () => {
    async function runScenario() {
      const data = {
        responses: [
          {
            aggregations: {
              '1': {
                buckets: [
                  { doc_count: 1, key: 'test' },
                  {
                    doc_count: 2,
                    key: 'test2',
                    key_as_string: 'test2_as_string',
                  },
                ],
              },
            },
          },
        ],
      };

      const { ds, fetchMock } = getTestContext({ jsonData: { esVersion: '7.10.0' }, data, database: 'test' });

      const results = await ds.metricFindQuery('{"find": "terms", "field": "test"}');

      expect(fetchMock).toHaveBeenCalledTimes(1);
      const requestOptions = fetchMock.mock.calls[0][0];
      const parts = requestOptions.data.split('\n');
      const header = JSON.parse(parts[0]);
      const body = JSON.parse(parts[1]);

      return { results, body, header };
    }

    it('should get results', async () => {
      const { results } = await runScenario();
      expect(results.length).toEqual(2);
    });

    it('should use key or key_as_string', async () => {
      const { results } = await runScenario();
      expect(results[0].text).toEqual('test');
      expect(results[1].text).toEqual('test2_as_string');
    });

    it('should not set search type to count', async () => {
      const { header } = await runScenario();
      expect(header.search_type).not.toEqual('count');
    });

    it('should set size to 0', async () => {
      const { body } = await runScenario();
      expect(body.size).toBe(0);
    });

    it('should not set terms aggregation size to 0', async () => {
      const { body } = await runScenario();
      expect(body['aggs']['1']['terms'].size).not.toBe(0);
    });
  });

  describe('query', () => {
    it('should replace range as integer not string', async () => {
      const { ds } = getTestContext({ jsonData: { interval: 'Daily', esVersion: '7.10.0', timeField: '@time' } });
      const postMock = jest.fn((url: string, data: any) => of(createFetchResponse({ responses: [] })));
      ds['post'] = postMock;

      await expect(ds.query(createElasticQuery())).toEmitValuesWith((received) => {
        expect(postMock).toHaveBeenCalledTimes(1);

        const query = postMock.mock.calls[0][1];
        expect(typeof JSON.parse(query.split('\n')[1]).query.bool.filter[0].range['@time'].gte).toBe('number');
      });
    });
  });

  it('should correctly interpolate variables in query', () => {
    const { ds } = getTestContext();
    const query: ElasticsearchQuery = {
      refId: 'A',
      bucketAggs: [{ type: 'filters', settings: { filters: [{ query: '$var', label: '' }] }, id: '1' }],
      metrics: [{ type: 'count', id: '1' }],
      query: '$var',
    };

    const interpolatedQuery = ds.interpolateVariablesInQueries([query], {})[0];

    expect(interpolatedQuery.query).toBe('resolvedVariable');
    expect((interpolatedQuery.bucketAggs![0] as Filters).settings!.filters![0].query).toBe('resolvedVariable');
  });

  it('should correctly handle empty query strings in filters bucket aggregation', () => {
    const { ds } = getTestContext();
    const query: ElasticsearchQuery = {
      refId: 'A',
      bucketAggs: [{ type: 'filters', settings: { filters: [{ query: '', label: '' }] }, id: '1' }],
      metrics: [{ type: 'count', id: '1' }],
      query: '',
    };

    const interpolatedQuery = ds.interpolateVariablesInQueries([query], {})[0];

    expect((interpolatedQuery.bucketAggs![0] as Filters).settings!.filters![0].query).toBe('*');
  });
});

describe('getMultiSearchUrl', () => {
  describe('When esVersion >= 7.10.0', () => {
    it('Should add correct params to URL if "includeFrozen" is enabled', () => {
      const { ds } = getTestContext({ jsonData: { esVersion: '7.10.0', includeFrozen: true, xpack: true } });

      expect(ds.getMultiSearchUrl()).toMatch(/ignore_throttled=false/);
    });

    it('Should NOT add ignore_throttled if "includeFrozen" is disabled', () => {
      const { ds } = getTestContext({ jsonData: { esVersion: '7.10.0', includeFrozen: false, xpack: true } });

      expect(ds.getMultiSearchUrl()).not.toMatch(/ignore_throttled=false/);
    });

    it('Should NOT add ignore_throttled if "xpack" is disabled', () => {
      const { ds } = getTestContext({ jsonData: { esVersion: '7.10.0', includeFrozen: true, xpack: false } });

      expect(ds.getMultiSearchUrl()).not.toMatch(/ignore_throttled=false/);
    });
  });
});

describe('enhanceDataFrame', () => {
  it('adds links to dataframe', () => {
    const df = new MutableDataFrame({
      fields: [
        {
          name: 'urlField',
          values: new ArrayVector([]),
        },
        {
          name: 'traceField',
          values: new ArrayVector([]),
        },
      ],
    });

    enhanceDataFrame(df, [
      {
        field: 'urlField',
        url: 'someUrl',
      },
      {
        field: 'urlField',
        url: 'someOtherUrl',
      },
      {
        field: 'traceField',
        url: 'query',
        datasourceUid: 'ds1',
      },
      {
        field: 'traceField',
        url: 'otherQuery',
        datasourceUid: 'ds2',
      },
    ]);

    expect(df.fields[0].config.links).toHaveLength(2);
    expect(df.fields[0].config.links).toContainEqual({
      title: '',
      url: 'someUrl',
    });
    expect(df.fields[0].config.links).toContainEqual({
      title: '',
      url: 'someOtherUrl',
    });

    expect(df.fields[1].config.links).toHaveLength(2);
    expect(df.fields[1].config.links).toContainEqual(
      expect.objectContaining({
        title: '',
        url: '',
        internal: expect.objectContaining({
          query: { query: 'query' },
          datasourceUid: 'ds1',
        }),
      })
    );
    expect(df.fields[1].config.links).toContainEqual(
      expect.objectContaining({
        title: '',
        url: '',
        internal: expect.objectContaining({
          query: { query: 'otherQuery' },
          datasourceUid: 'ds2',
        }),
      })
    );
  });

  it('adds limit to dataframe', () => {
    const df = new MutableDataFrame({
      fields: [
        {
          name: 'someField',
          values: new ArrayVector([]),
        },
      ],
    });
    enhanceDataFrame(df, [], 10);

    expect(df.meta?.limit).toBe(10);
  });
});

describe('modifyQuery', () => {
  let ds: ElasticDatasource;
  beforeEach(() => {
    ds = getTestContext().ds;
  });
  describe('with empty query', () => {
    let query: ElasticsearchQuery;
    beforeEach(() => {
      query = { query: '', refId: 'A' };
    });

    it('should add the filter', () => {
      expect(ds.modifyQuery(query, { type: 'ADD_FILTER', options: { key: 'foo', value: 'bar' } }).query).toBe(
        'foo:"bar"'
      );
    });

    it('should add the negative filter', () => {
      expect(ds.modifyQuery(query, { type: 'ADD_FILTER_OUT', options: { key: 'foo', value: 'bar' } }).query).toBe(
        '-foo:"bar"'
      );
    });

    it('should do nothing on unknown type', () => {
      expect(ds.modifyQuery(query, { type: 'unknown', options: { key: 'foo', value: 'bar' } }).query).toBe(query.query);
    });
  });

  describe('with non-empty query', () => {
    let query: ElasticsearchQuery;
    beforeEach(() => {
      query = { query: 'test:"value"', refId: 'A' };
    });

    it('should add the filter', () => {
      expect(ds.modifyQuery(query, { type: 'ADD_FILTER', options: { key: 'foo', value: 'bar' } }).query).toBe(
        'test:"value" AND foo:"bar"'
      );
    });

    it('should add the negative filter', () => {
      expect(ds.modifyQuery(query, { type: 'ADD_FILTER_OUT', options: { key: 'foo', value: 'bar' } }).query).toBe(
        'test:"value" AND -foo:"bar"'
      );
    });

    it('should do nothing on unknown type', () => {
      expect(ds.modifyQuery(query, { type: 'unknown', options: { key: 'foo', value: 'bar' } }).query).toBe(query.query);
    });
  });
});

const createElasticQuery = (): DataQueryRequest<ElasticsearchQuery> => {
  return {
    requestId: '',
    dashboardId: 0,
    interval: '',
    panelId: 0,
    intervalMs: 1,
    scopedVars: {},
    timezone: '',
    app: CoreApp.Dashboard,
    startTime: 0,
    range: {
      from: dateTime([2015, 4, 30, 10]),
      to: dateTime([2015, 5, 1, 10]),
    } as any,
    targets: [
      {
        refId: '',
        bucketAggs: [{ type: 'date_histogram', field: '@timestamp', id: '2' }],
        metrics: [{ type: 'count', id: '' }],
        query: 'test',
      },
    ],
  };
};

const logsResponse = {
  data: {
    responses: [
      {
        aggregations: {
          '2': {
            buckets: [
              {
                doc_count: 10,
                key: 1000,
              },
              {
                doc_count: 15,
                key: 2000,
              },
            ],
          },
        },
        hits: {
          hits: [
            {
              '@timestamp': ['2019-06-24T09:51:19.765Z'],
              _id: 'fdsfs',
              _type: '_doc',
              _index: 'mock-index',
              _source: {
                '@timestamp': '2019-06-24T09:51:19.765Z',
                host: 'djisaodjsoad',
                message: 'hello, i am a message',
              },
              fields: {
                '@timestamp': ['2019-06-24T09:51:19.765Z'],
              },
            },
            {
              '@timestamp': ['2019-06-24T09:52:19.765Z'],
              _id: 'kdospaidopa',
              _type: '_doc',
              _index: 'mock-index',
              _source: {
                '@timestamp': '2019-06-24T09:52:19.765Z',
                host: 'dsalkdakdop',
                message: 'hello, i am also message',
              },
              fields: {
                '@timestamp': ['2019-06-24T09:52:19.765Z'],
              },
            },
          ],
        },
      },
    ],
  },
};
