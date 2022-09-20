import { DataFrame, FieldType, getParser, Labels, LogsParsers } from '@grafana/data';

export function dataFrameHasLokiError(frame: DataFrame): boolean {
  const labelSets: Labels[] = frame.fields.find((f) => f.name === 'labels')?.values.toArray() ?? [];
  return labelSets.some((labels) => labels.__error__ !== undefined);
}

export function dataFrameHasLevelLabel(frame: DataFrame): boolean {
  const labelSets: Labels[] = frame.fields.find((f) => f.name === 'labels')?.values.toArray() ?? [];
  return labelSets.some((labels) => labels.level !== undefined);
}

export function extractLogParserFromDataFrame(frame: DataFrame): { hasLogfmt: boolean; hasJSON: boolean } {
  const lineField = frame.fields.find((field) => field.type === FieldType.string);
  if (lineField == null) {
    return { hasJSON: false, hasLogfmt: false };
  }

  const logLines: string[] = lineField.values.toArray();

  let hasJSON = false;
  let hasLogfmt = false;

  logLines.forEach((line) => {
    const parser = getParser(line);
    if (parser === LogsParsers.JSON) {
      hasJSON = true;
    }
    if (parser === LogsParsers.logfmt) {
      hasLogfmt = true;
    }
  });

  return { hasLogfmt, hasJSON };
}

export function extractHasErrorLabelFromDataFrame(frame: DataFrame): boolean {
  const labelField = frame.fields.find((field) => field.name === 'labels' && field.type === FieldType.other);
  if (labelField == null) {
    return false;
  }

  const labels: Array<{ [key: string]: string }> = labelField.values.toArray();
  return labels.some((label) => label['__error__']);
}

export function extractLevelLikeLabelFromDataFrame(frame: DataFrame): string | null {
  const labelField = frame.fields.find((field) => field.name === 'labels' && field.type === FieldType.other);
  if (labelField == null) {
    return null;
  }

  // Depending on number of labels, this can be pretty heavy operation.
  // Let's just look at first 2 lines If needed, we can introduce more later.
  const labelsArray: Array<{ [key: string]: string }> = labelField.values.toArray().slice(0, 2);
  let levelLikeLabel: string | null = null;

  // Find first level-like label
  for (let labels of labelsArray) {
    const label = Object.keys(labels).find((label) => label === 'lvl' || label.includes('level'));
    if (label) {
      levelLikeLabel = label;
      break;
    }
  }
  return levelLikeLabel;
}
