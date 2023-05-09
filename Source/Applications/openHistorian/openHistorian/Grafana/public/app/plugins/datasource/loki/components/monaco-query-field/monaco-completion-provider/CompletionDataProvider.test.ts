import { HistoryItem } from '@grafana/data';

import LokiLanguageProvider from '../../../LanguageProvider';
import { LokiDatasource } from '../../../datasource';
import { createLokiDatasource } from '../../../mocks';
import { LokiQuery } from '../../../types';

import { CompletionDataProvider } from './CompletionDataProvider';
import { Label } from './situation';

const history: Array<HistoryItem<LokiQuery>> = [
  {
    ts: 12345678,
    query: {
      refId: 'test-1',
      expr: '{test: unit}',
    },
  },
  {
    ts: 87654321,
    query: {
      refId: 'test-1',
      expr: '{unit: test}',
    },
  },
  {
    ts: 87654321,
    query: {
      refId: 'test-1',
      expr: '{unit: test}',
    },
  },
  {
    ts: 0,
    query: {
      refId: 'test-0',
      expr: '',
    },
  },
];
const labelKeys = ['place', 'source'];
const labelValues = ['moon', 'luna'];
const otherLabels: Label[] = [
  {
    name: 'place',
    value: 'luna',
    op: '=',
  },
];
const seriesLabels = { place: ['series', 'labels'], source: [], other: [] };
const parserAndLabelKeys = {
  extractedLabelKeys: ['extracted', 'label', 'keys'],
  unwrapLabelKeys: ['unwrap', 'labels'],
  hasJSON: true,
  hasLogfmt: false,
  hasPack: false,
};

describe('CompletionDataProvider', () => {
  let completionProvider: CompletionDataProvider, languageProvider: LokiLanguageProvider, datasource: LokiDatasource;
  let historyRef: { current: Array<HistoryItem<LokiQuery>> } = { current: [] };
  beforeEach(() => {
    datasource = createLokiDatasource();
    languageProvider = new LokiLanguageProvider(datasource);
    historyRef.current = history;
    completionProvider = new CompletionDataProvider(languageProvider, historyRef);

    jest.spyOn(languageProvider, 'getLabelKeys').mockReturnValue(labelKeys);
    jest.spyOn(languageProvider, 'getLabelValues').mockResolvedValue(labelValues);
    jest.spyOn(languageProvider, 'getSeriesLabels').mockResolvedValue(seriesLabels);
    jest.spyOn(languageProvider, 'getParserAndLabelKeys').mockResolvedValue(parserAndLabelKeys);
  });

  test('Returns the expected history entries', () => {
    expect(completionProvider.getHistory()).toEqual(['{test: unit}', '{unit: test}']);
  });

  test('Processes updates to the current historyRef value', () => {
    expect(completionProvider.getHistory()).toEqual(['{test: unit}', '{unit: test}']);

    historyRef.current = [
      {
        ts: 87654321,
        query: {
          refId: 'test-2',
          expr: '{value="other"}',
        },
      },
    ];

    expect(completionProvider.getHistory()).toEqual(['{value="other"}']);
  });

  test('Returns the expected label names with no other labels', async () => {
    expect(await completionProvider.getLabelNames([])).toEqual(labelKeys);
  });

  test('Returns the expected label names with other labels', async () => {
    expect(await completionProvider.getLabelNames(otherLabels)).toEqual(['source', 'other']);
  });

  test('Returns the expected label values with no other labels', async () => {
    expect(await completionProvider.getLabelValues('label', [])).toEqual(labelValues);
  });

  test('Returns the expected label values with other labels', async () => {
    expect(await completionProvider.getLabelValues('place', otherLabels)).toEqual(['series', 'labels']);
    expect(await completionProvider.getLabelValues('other label', otherLabels)).toEqual([]);
  });

  test('Returns the expected parser and label keys', async () => {
    expect(await completionProvider.getParserAndLabelKeys('')).toEqual(parserAndLabelKeys);
  });

  test('Returns the expected series labels', async () => {
    expect(await completionProvider.getSeriesLabels([])).toEqual(seriesLabels);
  });
});
