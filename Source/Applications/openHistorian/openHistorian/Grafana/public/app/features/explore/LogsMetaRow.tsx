import React from 'react';

import { LogsDedupStrategy, LogsMetaItem, LogsMetaKind, LogRowModel } from '@grafana/data';
import { Button, Tooltip, LogLabels } from '@grafana/ui';
import { MAX_CHARACTERS } from '@grafana/ui/src/components/Logs/LogRowMessage';

import { MetaInfoText, MetaItemProps } from './MetaInfoText';

export type Props = {
  meta: LogsMetaItem[];
  dedupStrategy: LogsDedupStrategy;
  dedupCount: number;
  showDetectedFields: string[];
  hasUnescapedContent: boolean;
  forceEscape: boolean;
  logRows: LogRowModel[];
  onEscapeNewlines: () => void;
  clearDetectedFields: () => void;
};

export const LogsMetaRow: React.FC<Props> = React.memo(
  ({
    meta,
    dedupStrategy,
    dedupCount,
    showDetectedFields,
    clearDetectedFields,
    hasUnescapedContent,
    forceEscape,
    onEscapeNewlines,
    logRows,
  }) => {
    const logsMetaItem: Array<LogsMetaItem | MetaItemProps> = [...meta];

    // Add deduplication info
    if (dedupStrategy !== LogsDedupStrategy.none) {
      logsMetaItem.push({
        label: 'Dedup count',
        value: dedupCount,
        kind: LogsMetaKind.Number,
      });
    }
    // Add info about limit for highlighting
    if (logRows.some((r) => r.entry.length > MAX_CHARACTERS)) {
      logsMetaItem.push({
        label: 'Info',
        value: 'Logs with more than 100,000 characters could not be parsed and highlighted',
        kind: LogsMetaKind.String,
      });
    }

    // Add detected fields info
    if (showDetectedFields?.length > 0) {
      logsMetaItem.push(
        {
          label: 'Showing only detected fields',
          value: renderMetaItem(showDetectedFields, LogsMetaKind.LabelsMap),
        },
        {
          label: '',
          value: (
            <Button variant="secondary" size="sm" onClick={clearDetectedFields}>
              Show original line
            </Button>
          ),
        }
      );
    }

    // Add unescaped content info
    if (hasUnescapedContent) {
      logsMetaItem.push({
        label: 'Your logs might have incorrectly escaped content',
        value: (
          <Tooltip
            content="Fix incorrectly escaped newline and tab sequences in log lines. Manually review the results to confirm that the replacements are correct."
            placement="right"
          >
            <Button variant="secondary" size="sm" onClick={onEscapeNewlines}>
              {forceEscape ? 'Remove escaping' : 'Escape newlines'}
            </Button>
          </Tooltip>
        ),
      });
    }

    return (
      <>
        {logsMetaItem && (
          <MetaInfoText
            metaItems={logsMetaItem.map((item) => {
              return {
                label: item.label,
                value: 'kind' in item ? renderMetaItem(item.value, item.kind) : item.value,
              };
            })}
          />
        )}
      </>
    );
  }
);

LogsMetaRow.displayName = 'LogsMetaRow';

function renderMetaItem(value: any, kind: LogsMetaKind) {
  if (kind === LogsMetaKind.LabelsMap) {
    return <LogLabels labels={value} />;
  } else if (kind === LogsMetaKind.Error) {
    return <span className="logs-meta-item__error">{value}</span>;
  }
  return value;
}
