import { TimeRange, DataFrame, FieldType, getTimeZoneInfo } from '@grafana/data';
import { TimeRegionConfig, calculateTimesWithin } from 'app/core/utils/timeRegions';

export function doTimeRegionQuery(
  name: string,
  config: TimeRegionConfig,
  range: TimeRange,
  tz: string
): DataFrame | undefined {
  if (!config) {
    return undefined;
  }
  const regions = calculateTimesWithin(config, range); // UTC
  if (!regions.length) {
    return undefined;
  }

  const times: number[] = [];
  const timesEnd: number[] = [];
  const texts: string[] = [];

  const regionTimezone = config.timezone ?? tz;

  for (const region of regions) {
    let from = region.from;
    let to = region.to;

    const info = getTimeZoneInfo(regionTimezone, from);
    if (info) {
      const offset = info.offsetInMins * 60 * 1000;
      from += offset;
      to += offset;
    }

    times.push(from);
    timesEnd.push(to);
    texts.push(name);
  }

  return {
    fields: [
      { name: 'time', type: FieldType.time, values: times, config: {} },
      { name: 'timeEnd', type: FieldType.time, values: timesEnd, config: {} },
      { name: 'text', type: FieldType.string, values: texts, config: {} },
    ],
    length: times.length,
  };
}
