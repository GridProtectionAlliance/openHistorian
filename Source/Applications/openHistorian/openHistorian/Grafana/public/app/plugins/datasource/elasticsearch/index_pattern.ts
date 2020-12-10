import { toUtc, dateTime } from '@grafana/data';

const intervalMap: any = {
  Hourly: { startOf: 'hour', amount: 'hours' },
  Daily: { startOf: 'day', amount: 'days' },
  Weekly: { startOf: 'isoWeek', amount: 'weeks' },
  Monthly: { startOf: 'month', amount: 'months' },
  Yearly: { startOf: 'year', amount: 'years' },
};

export class IndexPattern {
  private dateLocale = 'en';

  constructor(private pattern: any, private interval?: string) {}

  getIndexForToday() {
    if (this.interval) {
      return toUtc()
        .locale(this.dateLocale)
        .format(this.pattern);
    } else {
      return this.pattern;
    }
  }

  getIndexList(from: any, to: any) {
    if (!this.interval) {
      return this.pattern;
    }

    const intervalInfo = intervalMap[this.interval];
    const start = dateTime(from)
      .utc()
      .startOf(intervalInfo.startOf);
    const endEpoch = dateTime(to)
      .utc()
      .startOf(intervalInfo.startOf)
      .valueOf();
    const indexList = [];

    while (start.valueOf() <= endEpoch) {
      indexList.push(start.locale(this.dateLocale).format(this.pattern));
      start.add(1, intervalInfo.amount);
    }

    return indexList;
  }
}
