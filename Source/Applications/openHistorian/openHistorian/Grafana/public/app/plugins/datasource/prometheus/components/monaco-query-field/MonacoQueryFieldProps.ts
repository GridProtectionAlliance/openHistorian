import { HistoryItem } from '@grafana/data';

import type PromQlLanguageProvider from '../../language_provider';
import { PromQuery } from '../../types';

// we need to store this in a separate file,
// because we have an async-wrapper around,
// the react-component, and it needs the same
// props as the sync-component.
export type Props = {
  initialValue: string;
  languageProvider: PromQlLanguageProvider;
  history: Array<HistoryItem<PromQuery>>;
  onRunQuery: (value: string) => void;
  onBlur: (value: string) => void;
};
