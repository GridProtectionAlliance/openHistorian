import { of } from 'rxjs';
import { TestScheduler } from 'rxjs/testing';

import {
  dataFrameToJSON,
  DataQueryRequest,
  DataSourceInstanceSettings,
  dateTime,
  MutableDataFrame,
} from '@grafana/data';
import { FetchResponse } from '@grafana/runtime';
import { backendSrv } from 'app/core/services/backend_srv'; // will use the version in __mocks__
import { TemplateSrv } from 'app/features/templating/template_srv';

import { initialCustomVariableModelState } from '../../../../features/variables/custom/reducer';
import { PostgresDatasource } from '../datasource';
import { PostgresOptions, PostgresQuery } from '../types';

jest.mock('@grafana/runtime', () => ({
  ...(jest.requireActual('@grafana/runtime') as unknown as object),
  getBackendSrv: () => backendSrv,
}));

jest.mock('@grafana/runtime/src/services', () => ({
  ...(jest.requireActual('@grafana/runtime/src/services') as unknown as object),
  getBackendSrv: () => backendSrv,
  getDataSourceSrv: () => {
    return {
      getInstanceSettings: () => ({ id: 8674 }),
    };
  },
}));

describe('PostgreSQLDatasource', () => {
  const fetchMock = jest.spyOn(backendSrv, 'fetch');
  const setupTestContext = (data: any) => {
    jest.clearAllMocks();
    fetchMock.mockImplementation(() => of(createFetchResponse(data)));
    const instanceSettings = {
      jsonData: {
        defaultProject: 'testproject',
      },
    } as unknown as DataSourceInstanceSettings<PostgresOptions>;
    const templateSrv: TemplateSrv = new TemplateSrv();
    const variable = { ...initialCustomVariableModelState };
    const ds = new PostgresDatasource(instanceSettings, templateSrv);

    return { ds, templateSrv, variable };
  };

  // https://rxjs-dev.firebaseapp.com/guide/testing/marble-testing
  const runMarbleTest = (args: {
    options: any;
    values: { [marble: string]: FetchResponse };
    marble: string;
    expectedValues: { [marble: string]: any };
    expectedMarble: string;
  }) => {
    const { expectedValues, expectedMarble, options, values, marble } = args;
    const scheduler: TestScheduler = new TestScheduler((actual, expected) => {
      expect(actual).toEqual(expected);
    });

    const { ds } = setupTestContext({});

    scheduler.run(({ cold, expectObservable }) => {
      const source = cold(marble, values);
      jest.clearAllMocks();
      fetchMock.mockImplementation(() => source);

      const result = ds.query(options);
      expectObservable(result).toBe(expectedMarble, expectedValues);
    });
  };

  describe('When performing a time series query', () => {
    it('should transform response correctly', () => {
      const options = {
        range: {
          from: dateTime(1432288354),
          to: dateTime(1432288401),
        },
        targets: [
          {
            format: 'time_series',
            rawQuery: true,
            rawSql: 'select time, metric from grafana_metric',
            refId: 'A',
            datasource: 'gdev-ds',
          },
        ],
      };
      const response = {
        results: {
          A: {
            refId: 'A',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'time', values: [1599643351085] },
                    { name: 'metric', values: [30.226249741223704], labels: { metric: 'America' } },
                  ],
                  meta: {
                    executedQueryString: 'select time, metric from grafana_metric',
                  },
                })
              ),
            ],
          },
        },
      };

      const values = { a: createFetchResponse(response) };
      const marble = '-a|';
      const expectedMarble = '-a|';
      const expectedValues = {
        a: {
          data: [
            {
              fields: [
                {
                  config: {},
                  entities: {},
                  name: 'time',
                  type: 'time',
                  values: {
                    buffer: [1599643351085],
                  },
                },
                {
                  config: {},
                  entities: {},
                  labels: {
                    metric: 'America',
                  },
                  name: 'metric',
                  type: 'number',
                  values: {
                    buffer: [30.226249741223704],
                  },
                },
              ],
              length: 1,
              meta: {
                executedQueryString: 'select time, metric from grafana_metric',
              },
              name: undefined,
              refId: 'A',
            },
          ],
          state: 'Done',
        },
      };

      runMarbleTest({ options, marble, values, expectedMarble, expectedValues });
    });
  });

  describe('When performing a table query', () => {
    it('should transform response correctly', () => {
      const options = {
        range: {
          from: dateTime(1432288354),
          to: dateTime(1432288401),
        },
        targets: [
          {
            format: 'table',
            rawQuery: true,
            rawSql: 'select time, metric, value from grafana_metric',
            refId: 'A',
            datasource: 'gdev-ds',
          },
        ],
      };
      const response = {
        results: {
          A: {
            refId: 'A',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'time', values: [1599643351085] },
                    { name: 'metric', values: ['America'] },
                    { name: 'value', values: [30.226249741223704] },
                  ],
                  meta: {
                    executedQueryString: 'select time, metric, value from grafana_metric',
                  },
                })
              ),
            ],
          },
        },
      };

      const values = { a: createFetchResponse(response) };
      const marble = '-a|';
      const expectedMarble = '-a|';
      const expectedValues = {
        a: {
          data: [
            {
              fields: [
                {
                  config: {},
                  entities: {},
                  name: 'time',
                  type: 'time',
                  values: {
                    buffer: [1599643351085],
                  },
                },
                {
                  config: {},
                  entities: {},
                  name: 'metric',
                  type: 'string',
                  values: {
                    buffer: ['America'],
                  },
                },
                {
                  config: {},
                  entities: {},
                  name: 'value',
                  type: 'number',
                  values: {
                    buffer: [30.226249741223704],
                  },
                },
              ],
              length: 1,
              meta: {
                executedQueryString: 'select time, metric, value from grafana_metric',
              },
              name: undefined,
              refId: 'A',
            },
          ],
          state: 'Done',
        },
      };

      runMarbleTest({ options, marble, values, expectedMarble, expectedValues });
    });
  });

  describe('When performing a query with hidden target', () => {
    it('should return empty result and backendSrv.fetch should not be called', async () => {
      const options = {
        range: {
          from: dateTime(1432288354),
          to: dateTime(1432288401),
        },
        targets: [
          {
            format: 'table',
            rawQuery: true,
            rawSql: 'select time, metric, value from grafana_metric',
            refId: 'A',
            datasource: 'gdev-ds',
            hide: true,
          },
        ],
      } as unknown as DataQueryRequest<PostgresQuery>;

      const { ds } = setupTestContext({});

      await expect(ds.query(options)).toEmitValuesWith((received) => {
        expect(received[0]).toEqual({ data: [] });
        expect(fetchMock).not.toHaveBeenCalled();
      });
    });
  });

  describe('When performing annotationQuery', () => {
    let results: any;
    const annotationName = 'MyAnno';
    const options = {
      annotation: {
        name: annotationName,
        rawQuery: 'select time, title, text, tags from table;',
      },
      range: {
        from: dateTime(1432288354),
        to: dateTime(1432288401),
      },
    };
    const response = {
      results: {
        MyAnno: {
          frames: [
            dataFrameToJSON(
              new MutableDataFrame({
                fields: [
                  { name: 'time', values: [1432288355, 1432288390, 1432288400] },
                  { name: 'text', values: ['some text', 'some text2', 'some text3'] },
                  { name: 'tags', values: ['TagA,TagB', ' TagB , TagC', null] },
                ],
              })
            ),
          ],
        },
      },
    };

    beforeEach(async () => {
      const { ds } = setupTestContext(response);
      results = await ds.annotationQuery(options);
    });

    it('should return annotation list', async () => {
      expect(results.length).toBe(3);

      expect(results[0].text).toBe('some text');
      expect(results[0].tags[0]).toBe('TagA');
      expect(results[0].tags[1]).toBe('TagB');

      expect(results[1].tags[0]).toBe('TagB');
      expect(results[1].tags[1]).toBe('TagC');

      expect(results[2].tags.length).toBe(0);
    });
  });

  describe('When performing metricFindQuery that returns multiple string fields', () => {
    it('should return list of all string field values', async () => {
      const query = 'select * from atable';
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'title', values: ['aTitle', 'aTitle2', 'aTitle3'] },
                    { name: 'text', values: ['some text', 'some text2', 'some text3'] },
                  ],
                  meta: {
                    executedQueryString: 'select * from atable',
                  },
                })
              ),
            ],
          },
        },
      };

      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results.length).toBe(6);
      expect(results[0].text).toBe('aTitle');
      expect(results[5].text).toBe('some text3');
    });
  });

  describe('When performing metricFindQuery with $__searchFilter and a searchFilter is given', () => {
    it('should return list of all column values', async () => {
      const query = "select title from atable where title LIKE '$__searchFilter'";
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'title', values: ['aTitle', 'aTitle2', 'aTitle3'] },
                    { name: 'text', values: ['some text', 'some text2', 'some text3'] },
                  ],
                  meta: {
                    executedQueryString: 'select * from atable',
                  },
                })
              ),
            ],
          },
        },
      };

      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, { searchFilter: 'aTit' });

      expect(fetchMock).toBeCalledTimes(1);
      expect(fetchMock.mock.calls[0][0].data.queries[0].rawSql).toBe(
        "select title from atable where title LIKE 'aTit%'"
      );
      expect(results).toEqual([
        { text: 'aTitle' },
        { text: 'aTitle2' },
        { text: 'aTitle3' },
        { text: 'some text' },
        { text: 'some text2' },
        { text: 'some text3' },
      ]);
    });
  });

  describe('When performing metricFindQuery with $__searchFilter but no searchFilter is given', () => {
    it('should return list of all column values', async () => {
      const query = "select title from atable where title LIKE '$__searchFilter'";
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'title', values: ['aTitle', 'aTitle2', 'aTitle3'] },
                    { name: 'text', values: ['some text', 'some text2', 'some text3'] },
                  ],
                  meta: {
                    executedQueryString: 'select * from atable',
                  },
                })
              ),
            ],
          },
        },
      };

      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(fetchMock).toBeCalledTimes(1);
      expect(fetchMock.mock.calls[0][0].data.queries[0].rawSql).toBe("select title from atable where title LIKE '%'");
      expect(results).toEqual([
        { text: 'aTitle' },
        { text: 'aTitle2' },
        { text: 'aTitle3' },
        { text: 'some text' },
        { text: 'some text2' },
        { text: 'some text3' },
      ]);
    });
  });

  describe('When performing metricFindQuery with key, value columns', () => {
    it('should return list of as text, value', async () => {
      const query = 'select * from atable';
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: '__value', values: ['value1', 'value2', 'value3'] },
                    { name: '__text', values: ['aTitle', 'aTitle2', 'aTitle3'] },
                  ],
                  meta: {
                    executedQueryString: 'select * from atable',
                  },
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results).toEqual([
        { text: 'aTitle', value: 'value1' },
        { text: 'aTitle2', value: 'value2' },
        { text: 'aTitle3', value: 'value3' },
      ]);
    });
  });

  describe('When performing metricFindQuery without key, value columns', () => {
    it('should return list of all field values as text', async () => {
      const query = 'select id, values from atable';
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'id', values: [1, 2, 3] },
                    { name: 'values', values: ['test1', 'test2', 'test3'] },
                  ],
                  meta: {
                    executedQueryString: 'select id, values from atable',
                  },
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results).toEqual([
        { text: 1 },
        { text: 2 },
        { text: 3 },
        { text: 'test1' },
        { text: 'test2' },
        { text: 'test3' },
      ]);
    });
  });

  describe('When performing metricFindQuery with key, value columns and with duplicate keys', () => {
    it('should return list of unique keys', async () => {
      const query = 'select * from atable';
      const response = {
        results: {
          tempvar: {
            refId: 'tempvar',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: '__text', values: ['aTitle', 'aTitle', 'aTitle'] },
                    { name: '__value', values: ['same', 'same', 'diff'] },
                  ],
                  meta: {
                    executedQueryString: 'select * from atable',
                  },
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results).toEqual([{ text: 'aTitle', value: 'same' }]);
    });
  });

  describe('When interpolating variables', () => {
    describe('and value is a string', () => {
      it('should return an unquoted value', () => {
        const { ds, variable } = setupTestContext({});
        expect(ds.interpolateVariable('abc', variable)).toEqual('abc');
      });
    });

    describe('and value is a number', () => {
      it('should return an unquoted value', () => {
        const { ds, variable } = setupTestContext({});
        expect(ds.interpolateVariable(1000 as unknown as string, variable)).toEqual(1000);
      });
    });

    describe('and value is an array of strings', () => {
      it('should return comma separated quoted values', () => {
        const { ds, variable } = setupTestContext({});
        expect(ds.interpolateVariable(['a', 'b', 'c'], variable)).toEqual("'a','b','c'");
      });
    });

    describe('and variable allows multi-value and is a string', () => {
      it('should return a quoted value', () => {
        const { ds, variable } = setupTestContext({});
        variable.multi = true;
        expect(ds.interpolateVariable('abc', variable)).toEqual("'abc'");
      });
    });

    describe('and variable contains single quote', () => {
      it('should return a quoted value', () => {
        const { ds, variable } = setupTestContext({});
        variable.multi = true;
        expect(ds.interpolateVariable("a'bc", variable)).toEqual("'a''bc'");
        expect(ds.interpolateVariable("a'b'c", variable)).toEqual("'a''b''c'");
      });
    });

    describe('and variable allows all and is a string', () => {
      it('should return a quoted value', () => {
        const { ds, variable } = setupTestContext({});
        variable.includeAll = true;
        expect(ds.interpolateVariable('abc', variable)).toEqual("'abc'");
      });
    });
  });

  describe('targetContainsTemplate', () => {
    it('given query that contains template variable it should return true', () => {
      const rawSql = `SELECT
      $__timeGroup("createdAt",'$summarize'),
      avg(value) as "value",
      hostname as "metric"
    FROM
      grafana_metric
    WHERE
      $__timeFilter("createdAt") AND
      measurement = 'logins.count' AND
      hostname IN($host)
    GROUP BY time, metric
    ORDER BY time`;
      const query = {
        rawSql,
        rawQuery: true,
      };
      const { templateSrv, ds } = setupTestContext({});

      templateSrv.init([
        { type: 'query', name: 'summarize', current: { value: '1m' } },
        { type: 'query', name: 'host', current: { value: 'a' } },
      ]);

      expect(ds.targetContainsTemplate(query)).toBeTruthy();
    });

    it('given query that only contains global template variable it should return false', () => {
      const rawSql = `SELECT
      $__timeGroup("createdAt",'$__interval'),
      avg(value) as "value",
      hostname as "metric"
    FROM
      grafana_metric
    WHERE
      $__timeFilter("createdAt") AND
      measurement = 'logins.count'
    GROUP BY time, metric
    ORDER BY time`;
      const query = {
        rawSql,
        rawQuery: true,
      };
      const { templateSrv, ds } = setupTestContext({});

      templateSrv.init([
        { type: 'query', name: 'summarize', current: { value: '1m' } },
        { type: 'query', name: 'host', current: { value: 'a' } },
      ]);

      expect(ds.targetContainsTemplate(query)).toBeFalsy();
    });
  });
});

const createFetchResponse = <T>(data: T): FetchResponse<T> => ({
  data,
  status: 200,
  url: 'http://localhost:3000/api/query',
  config: { url: 'http://localhost:3000/api/query' },
  type: 'basic',
  statusText: 'Ok',
  redirected: false,
  headers: {} as unknown as Headers,
  ok: true,
});
