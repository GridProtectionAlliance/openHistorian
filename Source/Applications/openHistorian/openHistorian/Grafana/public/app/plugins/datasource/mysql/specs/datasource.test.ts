import { of } from 'rxjs';

import {
  dataFrameToJSON,
  DataQueryRequest,
  DataSourceInstanceSettings,
  dateTime,
  FieldType,
  MutableDataFrame,
} from '@grafana/data';
import { FetchResponse, setBackendSrv } from '@grafana/runtime';
import { backendSrv } from 'app/core/services/backend_srv'; // will use the version in __mocks__
import { SQLQuery } from 'app/features/plugins/sql/types';
import { TemplateSrv } from 'app/features/templating/template_srv';
import { initialCustomVariableModelState } from 'app/features/variables/custom/reducer';

import { MySqlDatasource } from '../MySqlDatasource';
import { MySQLOptions } from '../types';

describe('MySQLDatasource', () => {
  const setupTestContext = (response: unknown) => {
    jest.clearAllMocks();
    setBackendSrv(backendSrv);
    const fetchMock = jest.spyOn(backendSrv, 'fetch');
    const instanceSettings = {
      jsonData: {
        defaultProject: 'testproject',
      },
    } as unknown as DataSourceInstanceSettings<MySQLOptions>;
    const templateSrv: TemplateSrv = new TemplateSrv();
    const variable = { ...initialCustomVariableModelState };
    fetchMock.mockImplementation((options) => of(createFetchResponse(response)));

    const ds = new MySqlDatasource(instanceSettings);
    Reflect.set(ds, 'templateSrv', templateSrv);

    return { ds, variable, templateSrv, fetchMock };
  };

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
      } as unknown as DataQueryRequest<SQLQuery>;

      const { ds, fetchMock } = setupTestContext({});

      await expect(ds.query(options)).toEmitValuesWith((received) => {
        expect(received[0]).toEqual({ data: [] });
        expect(fetchMock).not.toHaveBeenCalled();
      });
    });
  });

  describe('When runSql returns an empty dataframe', () => {
    let ds: MySqlDatasource;
    const response = {
      results: {
        tempvar: {
          refId: 'tempvar',
          frames: [],
        },
      },
    };

    beforeEach(async () => {
      ds = setupTestContext(response).ds;
    });

    it('should return an empty array when metricFindQuery is called', async () => {
      const query = 'select * from atable';
      const results = await ds.metricFindQuery(query);
      expect(results.length).toBe(0);
    });

    it('should return an empty array when fetchDatasets is called', async () => {
      const results = await ds.fetchDatasets();
      expect(results.length).toBe(0);
    });

    it('should return an empty array when fetchTables is called', async () => {
      const results = await ds.fetchTables();
      expect(results.length).toBe(0);
    });

    it('should return an empty array when fetchFields is called', async () => {
      const query: SQLQuery = {
        refId: 'refId',
        table: 'schema.table',
        dataset: 'dataset',
      };
      const results = await ds.fetchFields(query);
      expect(results.length).toBe(0);
    });
  });

  describe('When runSql returns a populated dataframe', () => {
    it('should return a list of datasets when fetchDatasets is called', async () => {
      const fetchDatasetsResponse = {
        results: {
          datasets: {
            refId: 'datasets',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [{ name: 'name', type: FieldType.string, values: ['test1', 'test2', 'test3'] }],
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(fetchDatasetsResponse);

      const results = await ds.fetchDatasets();
      expect(results.length).toBe(3);
      expect(results).toEqual(['test1', 'test2', 'test3']);
    });

    it('should return a list of tables when fetchTables is called', async () => {
      const fetchTableResponse = {
        results: {
          tables: {
            refId: 'tables',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [{ name: 'table_name', type: FieldType.string, values: ['test1', 'test2', 'test3'] }],
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(fetchTableResponse);

      const results = await ds.fetchTables();
      expect(results.length).toBe(3);
      expect(results).toEqual(['test1', 'test2', 'test3']);
    });

    it('should return a list of fields when fetchFields is called', async () => {
      const fetchFieldsResponse = {
        results: {
          fields: {
            refId: 'fields',
            frames: [
              dataFrameToJSON(
                new MutableDataFrame({
                  fields: [
                    { name: 'column_name', type: FieldType.string, values: ['test1', 'test2', 'test3'] },
                    { name: 'data_type', type: FieldType.string, values: ['int', 'char', 'bool'] },
                  ],
                })
              ),
            ],
          },
        },
      };
      const { ds } = setupTestContext(fetchFieldsResponse);

      const sqlQuery: SQLQuery = {
        refId: 'fields',
        table: 'table',
        dataset: 'dataset',
      };
      const results = await ds.fetchFields(sqlQuery);
      expect(results.length).toBe(3);
      expect(results[0].label).toBe('test1');
      expect(results[0].value).toBe('test1');
      expect(results[0].type).toBe('int');
      expect(results[1].label).toBe('test2');
      expect(results[1].value).toBe('test2');
      expect(results[1].type).toBe('char');
      expect(results[2].label).toBe('test3');
      expect(results[2].value).toBe('test3');
      expect(results[2].type).toBe('bool');
    });
  });

  describe('When performing metricFindQuery that returns multiple string fields', () => {
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

    it('should return list of all string field values', async () => {
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results.length).toBe(6);
      expect(results[0].text).toBe('aTitle');
      expect(results[5].text).toBe('some text3');
    });
  });

  describe('When performing metricFindQuery with $__searchFilter and a searchFilter is given', () => {
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

    it('should return list of all column values', async () => {
      const { ds, fetchMock } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, { searchFilter: 'aTit' });

      expect(fetchMock).toBeCalledTimes(1);
      expect(fetchMock.mock.calls[0][0].data.queries[0].rawSql).toBe(
        "select title from atable where title LIKE 'aTit%'"
      );
      expect(results.length).toBe(6);
    });
  });

  describe('When performing metricFindQuery with $__searchFilter but no searchFilter is given', () => {
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

    it('should return list of all column values', async () => {
      const { ds, fetchMock } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(fetchMock).toBeCalledTimes(1);
      expect(fetchMock.mock.calls[0][0].data.queries[0].rawSql).toBe("select title from atable where title LIKE '%'");
      expect(results.length).toBe(6);
    });
  });

  describe('When performing metricFindQuery with key, value columns', () => {
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

    it('should return list of as text, value', async () => {
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results.length).toBe(3);
      expect(results[0].text).toBe('aTitle');
      expect(results[0].value).toBe('value1');
      expect(results[2].text).toBe('aTitle3');
      expect(results[2].value).toBe('value3');
    });
  });

  describe('When performing metricFindQuery without key, value columns', () => {
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

    it('should return list of all field values as text', async () => {
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

    it('should return list of unique keys', async () => {
      const { ds } = setupTestContext(response);
      const results = await ds.metricFindQuery(query, {});

      expect(results.length).toBe(1);
      expect(results[0].text).toBe('aTitle');
      expect(results[0].value).toBe('same');
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
        expect(ds.interpolateVariable(1000, variable)).toEqual(1000);
      });
    });

    describe('and value is an array of strings', () => {
      it('should return comma separated quoted values', () => {
        const { ds, variable } = setupTestContext({});
        expect(ds.interpolateVariable(['a', 'b', 'c'], variable)).toEqual("'a','b','c'");
      });
    });

    describe('and variable allows multi-value and value is a string', () => {
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
      });
    });

    describe('and variable allows all and value is a string', () => {
      it('should return a quoted value', () => {
        const { ds, variable } = setupTestContext({});
        variable.includeAll = true;
        expect(ds.interpolateVariable('abc', variable)).toEqual("'abc'");
      });
    });
  });

  describe('targetContainsTemplate', () => {
    it('given query that contains template variable it should return true', () => {
      const { ds, templateSrv } = setupTestContext({});
      const rawSql = `SELECT
      $__timeGroup(createdAt,'$summarize') as time_sec,
      avg(value) as value,
      hostname as metric
    FROM
      grafana_metric
    WHERE
      $__timeFilter(createdAt) AND
      foo = 'bar' AND
      measurement = 'logins.count' AND
      hostname IN($host)
    GROUP BY 1, 3
    ORDER BY 1`;
      const query = {
        rawSql,
        rawQuery: true,
        refId: '',
      };
      templateSrv.init([
        { type: 'query', name: 'summarize', current: { value: '1m' } },
        { type: 'query', name: 'host', current: { value: 'a' } },
      ]);
      expect(ds.targetContainsTemplate(query)).toBeTruthy();
    });

    it('given query that only contains global template variable it should return false', () => {
      const { ds, templateSrv } = setupTestContext({});
      const rawSql = `SELECT
      $__timeGroup(createdAt,'$__interval') as time_sec,
      avg(value) as value,
      hostname as metric
    FROM
      grafana_metric
    WHERE
      $__timeFilter(createdAt) AND
      measurement = 'logins.count'
    GROUP BY 1, 3
    ORDER BY 1`;
      const query = {
        rawSql,
        rawQuery: true,
        refId: '',
      };
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
