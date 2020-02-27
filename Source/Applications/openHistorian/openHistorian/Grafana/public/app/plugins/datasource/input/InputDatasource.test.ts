import InputDatasource, { describeDataFrame } from './InputDatasource';
import { InputQuery, InputOptions } from './types';
import { readCSV, DataFrame, MutableDataFrame, DataSourceInstanceSettings, PluginMeta } from '@grafana/data';

import { getQueryOptions } from 'test/helpers/getQueryOptions';

describe('InputDatasource', () => {
  const data = readCSV('a,b,c\n1,2,3\n4,5,6');
  const instanceSettings: DataSourceInstanceSettings<InputOptions> = {
    id: 1,
    type: 'x',
    name: 'xxx',
    meta: {} as PluginMeta,
    jsonData: {
      data,
    },
  };

  describe('when querying', () => {
    test('should return the saved data with a query', () => {
      const ds = new InputDatasource(instanceSettings);
      const options = getQueryOptions<InputQuery>({
        targets: [{ refId: 'Z' }],
      });

      return ds.query(options).then(rsp => {
        expect(rsp.data.length).toBe(1);

        const series: DataFrame = rsp.data[0];
        expect(series.refId).toBe('Z');
        expect(series.fields[0].values).toEqual(data[0].fields[0].values);
      });
    });
  });

  test('DataFrame descriptions', () => {
    expect(describeDataFrame([])).toEqual('');
    expect(describeDataFrame(null)).toEqual('');
    expect(
      describeDataFrame([
        new MutableDataFrame({
          name: 'x',
          fields: [{ name: 'a' }],
        }),
      ])
    ).toEqual('1 Fields, 0 Rows');
  });
});
