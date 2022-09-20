import { assertIsDefined } from 'test/helpers/asserts';

import {
  createTheme,
  DefaultTimeZone,
  EventBusSrv,
  FieldConfig,
  FieldType,
  getDefaultTimeRange,
  MutableDataFrame,
  VizOrientation,
} from '@grafana/data';
import {
  LegendDisplayMode,
  TooltipDisplayMode,
  VisibilityMode,
  GraphGradientMode,
  StackingMode,
  SortOrder,
} from '@grafana/schema';

import { BarChartFieldConfig } from './models.gen';
import { BarChartOptionsEX, prepareBarChartDisplayValues, preparePlotConfigBuilder } from './utils';

function mockDataFrame() {
  const df1 = new MutableDataFrame({
    refId: 'A',
    fields: [{ name: 'ts', type: FieldType.string, values: ['a', 'b', 'c'] }],
  });

  const df2 = new MutableDataFrame({
    refId: 'B',
    fields: [{ name: 'ts', type: FieldType.time, values: [1, 2, 4] }],
  });

  const f1Config: FieldConfig<BarChartFieldConfig> = {
    displayName: 'Metric 1',
    decimals: 2,
    unit: 'm/s',
    custom: {
      gradientMode: GraphGradientMode.Opacity,
      lineWidth: 2,
      fillOpacity: 0.1,
    },
  };

  const f2Config: FieldConfig<BarChartFieldConfig> = {
    displayName: 'Metric 2',
    decimals: 2,
    unit: 'kWh',
    custom: {
      gradientMode: GraphGradientMode.Hue,
      lineWidth: 2,
      fillOpacity: 0.1,
    },
  };

  df1.addField({
    name: 'metric1',
    type: FieldType.number,
    config: f1Config,
    state: {},
  });

  df2.addField({
    name: 'metric2',
    type: FieldType.number,
    config: f2Config,
    state: {},
  });

  const info = prepareBarChartDisplayValues([df1], createTheme(), {} as any);

  if (!('aligned' in info)) {
    throw new Error('Bar chart not prepared correctly');
  }

  return info.aligned;
}

jest.mock('@grafana/data', () => ({
  ...jest.requireActual('@grafana/data'),
  DefaultTimeZone: 'utc',
}));

describe('BarChart utils', () => {
  describe('preparePlotConfigBuilder', () => {
    const frame = mockDataFrame();

    const config: BarChartOptionsEX = {
      orientation: VizOrientation.Auto,
      groupWidth: 20,
      barWidth: 2,
      showValue: VisibilityMode.Always,
      legend: {
        displayMode: LegendDisplayMode.List,
        showLegend: true,
        placement: 'bottom',
        calcs: [],
      },
      xTickLabelRotation: 0,
      xTickLabelMaxLength: 20,
      stacking: StackingMode.None,
      tooltip: {
        mode: TooltipDisplayMode.None,
        sort: SortOrder.None,
      },
      text: {
        valueSize: 10,
      },
      rawValue: (seriesIdx: number, valueIdx: number) => frame.fields[seriesIdx].values.get(valueIdx),
    };

    it.each([VizOrientation.Auto, VizOrientation.Horizontal, VizOrientation.Vertical])('orientation', (v) => {
      const result = preparePlotConfigBuilder({
        ...config,
        orientation: v,
        frame: frame!,
        theme: createTheme(),
        timeZones: [DefaultTimeZone],
        getTimeRange: getDefaultTimeRange,
        eventBus: new EventBusSrv(),
        allFrames: [frame],
      }).getConfig();
      expect(result).toMatchSnapshot();
    });

    it.each([VisibilityMode.Always, VisibilityMode.Auto])('value visibility', (v) => {
      expect(
        preparePlotConfigBuilder({
          ...config,
          showValue: v,
          frame: frame!,
          theme: createTheme(),
          timeZones: [DefaultTimeZone],
          getTimeRange: getDefaultTimeRange,
          eventBus: new EventBusSrv(),
          allFrames: [frame],
        }).getConfig()
      ).toMatchSnapshot();
    });

    it.each([StackingMode.None, StackingMode.Percent, StackingMode.Normal])('stacking', (v) => {
      expect(
        preparePlotConfigBuilder({
          ...config,
          stacking: v,
          frame: frame!,
          theme: createTheme(),
          timeZones: [DefaultTimeZone],
          getTimeRange: getDefaultTimeRange,
          eventBus: new EventBusSrv(),
          allFrames: [frame],
        }).getConfig()
      ).toMatchSnapshot();
    });
  });

  describe('prepareGraphableFrames', () => {
    it('will warn when there is no data in the response', () => {
      const result = prepareBarChartDisplayValues([], createTheme(), { stacking: StackingMode.None } as any);
      const warning = assertIsDefined('warn' in result ? result : null);

      expect(warning.warn).toEqual('No data in response');
    });

    it('will warn when there is no string or time field', () => {
      const df = new MutableDataFrame({
        fields: [
          { name: 'a', type: FieldType.other, values: [1, 2, 3, 4, 5] },
          { name: 'value', values: [1, 2, 3, 4, 5] },
        ],
      });
      const result = prepareBarChartDisplayValues([df], createTheme(), { stacking: StackingMode.None } as any);
      const warning = assertIsDefined('warn' in result ? result : null);
      expect(warning.warn).toEqual('Bar charts requires a string or time field');
      expect(warning).not.toHaveProperty('viz');
    });

    it('will warn when there are no numeric fields in the response', () => {
      const df = new MutableDataFrame({
        fields: [
          { name: 'a', type: FieldType.string, values: ['a', 'b', 'c', 'd', 'e'] },
          { name: 'value', type: FieldType.boolean, values: [true, true, true, true, true] },
        ],
      });
      const result = prepareBarChartDisplayValues([df], createTheme(), { stacking: StackingMode.None } as any);
      const warning = assertIsDefined('warn' in result ? result : null);
      expect(warning.warn).toEqual('No numeric fields found');
      expect(warning).not.toHaveProperty('viz');
    });

    it('will convert NaN and Infinty to nulls', () => {
      const df = new MutableDataFrame({
        fields: [
          { name: 'a', type: FieldType.string, values: ['a', 'b', 'c', 'd', 'e'] },
          { name: 'value', values: [-10, NaN, 10, -Infinity, +Infinity] },
        ],
      });
      const result = prepareBarChartDisplayValues([df], createTheme(), { stacking: StackingMode.None } as any);
      const displayValues = assertIsDefined('viz' in result ? result : null);

      const field = displayValues.viz[0].fields[1];
      expect(field.values.toArray()).toMatchInlineSnapshot(`
      Array [
        -10,
        null,
        10,
        null,
        null,
      ]
    `);
    });

    it('should sort fields when legend sortBy and sortDesc are set', () => {
      const frame = new MutableDataFrame({
        fields: [
          { name: 'string', type: FieldType.string, values: ['a', 'b', 'c'] },
          { name: 'a', values: [-10, 20, 10], state: { calcs: { min: -10 } } },
          { name: 'b', values: [20, 20, 20], state: { calcs: { min: 20 } } },
          { name: 'c', values: [10, 10, 10], state: { calcs: { min: 10 } } },
        ],
      });

      const resultAsc = prepareBarChartDisplayValues([frame], createTheme(), {
        legend: { sortBy: 'Min', sortDesc: false },
      } as any);
      const displayValuesAsc = assertIsDefined('viz' in resultAsc ? resultAsc : null).viz[0];
      expect(displayValuesAsc.fields[0].type).toBe(FieldType.string);
      expect(displayValuesAsc.fields[1].name).toBe('a');
      expect(displayValuesAsc.fields[2].name).toBe('c');
      expect(displayValuesAsc.fields[3].name).toBe('b');

      const resultDesc = prepareBarChartDisplayValues([frame], createTheme(), {
        legend: { sortBy: 'Min', sortDesc: true },
      } as any);
      const displayValuesDesc = assertIsDefined('viz' in resultDesc ? resultDesc : null).viz[0];
      expect(displayValuesDesc.fields[0].type).toBe(FieldType.string);
      expect(displayValuesDesc.fields[1].name).toBe('b');
      expect(displayValuesDesc.fields[2].name).toBe('c');
      expect(displayValuesDesc.fields[3].name).toBe('a');
    });
  });
});
