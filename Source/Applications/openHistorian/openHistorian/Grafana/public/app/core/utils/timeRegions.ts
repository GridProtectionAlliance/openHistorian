import { AbsoluteTimeRange, dateTime, TimeRange } from '@grafana/data';

export interface TimeRegionConfig {
  from?: string;
  fromDayOfWeek?: number; // 1-7

  to?: string;
  toDayOfWeek?: number; // 1-7
}

interface ParsedTime {
  dayOfWeek?: number; // 1-7
  h?: number; // 0-23
  m?: number; // 0-59
  s?: number; // 0-59
}

export function calculateTimesWithin(cfg: TimeRegionConfig, tRange: TimeRange): AbsoluteTimeRange[] {
  if (!(cfg.fromDayOfWeek || cfg.from) && !(cfg.toDayOfWeek || cfg.to)) {
    return [];
  }

  // So we can mutate
  const timeRegion = { ...cfg };

  if (timeRegion.from && !timeRegion.to) {
    timeRegion.to = timeRegion.from;
  }

  if (!timeRegion.from && timeRegion.to) {
    timeRegion.from = timeRegion.to;
  }

  const hRange = {
    from: parseTimeRange(timeRegion.from),
    to: parseTimeRange(timeRegion.to),
  };

  if (!timeRegion.fromDayOfWeek && timeRegion.toDayOfWeek) {
    timeRegion.fromDayOfWeek = timeRegion.toDayOfWeek;
  }

  if (!timeRegion.toDayOfWeek && timeRegion.fromDayOfWeek) {
    timeRegion.toDayOfWeek = timeRegion.fromDayOfWeek;
  }

  if (timeRegion.fromDayOfWeek) {
    hRange.from.dayOfWeek = Number(timeRegion.fromDayOfWeek);
  }

  if (timeRegion.toDayOfWeek) {
    hRange.to.dayOfWeek = Number(timeRegion.toDayOfWeek);
  }

  if (hRange.from.dayOfWeek && hRange.from.h == null && hRange.from.m == null) {
    hRange.from.h = 0;
    hRange.from.m = 0;
    hRange.from.s = 0;
  }

  if (hRange.to.dayOfWeek && hRange.to.h == null && hRange.to.m == null) {
    hRange.to.h = 23;
    hRange.to.m = 59;
    hRange.to.s = 59;
  }

  if (!hRange.from || !hRange.to) {
    return [];
  }

  if (hRange.from.h == null) {
    hRange.from.h = 0;
  }

  if (hRange.to.h == null) {
    hRange.to.h = 23;
  }

  const regions: AbsoluteTimeRange[] = [];

  const fromStart = dateTime(tRange.from);
  fromStart.set('hour', 0);
  fromStart.set('minute', 0);
  fromStart.set('second', 0);
  fromStart.add(hRange.from.h, 'hours');
  fromStart.add(hRange.from.m, 'minutes');
  fromStart.add(hRange.from.s, 'seconds');

  while (fromStart.unix() <= tRange.to.unix()) {
    while (hRange.from.dayOfWeek && hRange.from.dayOfWeek !== fromStart.isoWeekday()) {
      fromStart.add(24, 'hours');
    }

    if (fromStart.unix() > tRange.to.unix()) {
      break;
    }

    const fromEnd = dateTime(fromStart);

    if (fromEnd.hour) {
      if (hRange.from.h <= hRange.to.h) {
        fromEnd.add(hRange.to.h - hRange.from.h, 'hours');
      } else if (hRange.from.h > hRange.to.h) {
        while (fromEnd.hour() !== hRange.to.h) {
          fromEnd.add(1, 'hours');
        }
      } else {
        fromEnd.add(24 - hRange.from.h, 'hours');

        while (fromEnd.hour() !== hRange.to.h) {
          fromEnd.add(1, 'hours');
        }
      }
    }

    fromEnd.set('minute', hRange.to.m ?? 0);
    fromEnd.set('second', hRange.to.s ?? 0);

    while (hRange.to.dayOfWeek && hRange.to.dayOfWeek !== fromEnd.isoWeekday()) {
      fromEnd.add(24, 'hours');
    }

    const outsideRange =
      (fromStart.unix() < tRange.from.unix() && fromEnd.unix() < tRange.from.unix()) ||
      (fromStart.unix() > tRange.to.unix() && fromEnd.unix() > tRange.to.unix());

    if (!outsideRange) {
      regions.push({ from: fromStart.valueOf(), to: fromEnd.valueOf() });
    }

    fromStart.add(24, 'hours');
  }

  return regions;
}

function parseTimeRange(str?: string): ParsedTime {
  const result: ParsedTime = {};
  if (!str?.length) {
    return result;
  }

  const timeRegex = /^([\d]+):?(\d{2})?/;
  const match = timeRegex.exec(str);

  if (!match) {
    return result;
  }

  if (match.length > 1) {
    result.h = Number(match[1]);
    result.m = 0;

    if (match.length > 2 && match[2] !== undefined) {
      result.m = Number(match[2]);
    }

    if (result.h > 23) {
      result.h = 23;
    }

    if (result.m > 59) {
      result.m = 59;
    }
  }

  return result;
}
