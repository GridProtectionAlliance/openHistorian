import { toDataFrame, applyFieldOverrides, GrafanaTheme } from '@grafana/data';
import { getFieldDisplayValuesProxy } from './fieldDisplayValuesProxy';

describe('getFieldDisplayValuesProxy', () => {
  const data = applyFieldOverrides({
    data: [
      toDataFrame({
        fields: [
          { name: 'Time', values: [1, 2, 3] },
          {
            name: 'power',
            values: [100, 200, 300],
            config: {
              title: 'The Power',
            },
          },
          {
            name: 'Last',
            values: ['a', 'b', 'c'],
          },
        ],
      }),
    ],
    fieldOptions: {
      defaults: {},
      overrides: [],
    },
    replaceVariables: (val: string) => val,
    timeZone: 'utc',
    theme: {} as GrafanaTheme,
    autoMinMax: true,
  })[0];

  it('should define all display functions', () => {
    // Field display should be set
    for (const field of data.fields) {
      expect(field.display).toBeDefined();
    }
  });

  it('should format the time values in UTC', () => {
    // Test Proxies in general
    const p = getFieldDisplayValuesProxy(data, 0);
    const time = p.Time;
    expect(time.numeric).toEqual(1);
    expect(time.text).toEqual('1970-01-01 00:00:00');

    // Should get to the same values by name or index
    const time2 = p[0];
    expect(time2.toString()).toEqual(time.toString());
  });

  it('Lookup by name, index, or title', () => {
    const p = getFieldDisplayValuesProxy(data, 2);
    expect(p.power.numeric).toEqual(300);
    expect(p['power'].numeric).toEqual(300);
    expect(p['The Power'].numeric).toEqual(300);
    expect(p[1].numeric).toEqual(300);
  });

  it('should return undefined when missing', () => {
    const p = getFieldDisplayValuesProxy(data, 0);
    expect(p.xyz).toBeUndefined();
    expect(p[100]).toBeUndefined();
  });
});
