import { css } from '@emotion/css';
import React, { FormEvent, PropsWithChildren, ReactElement, useCallback } from 'react';

import { GrafanaTheme } from '@grafana/data';
import { InlineField, TextArea, useStyles } from '@grafana/ui';

interface VariableTextAreaFieldProps<T> {
  name: string;
  value: string;
  placeholder: string;
  onChange: (event: FormEvent<HTMLTextAreaElement>) => void;
  width: number;
  tooltip?: string;
  ariaLabel?: string;
  required?: boolean;
  labelWidth?: number;
  testId?: string;
  onBlur?: (event: FormEvent<HTMLTextAreaElement>) => void;
}

export function VariableTextAreaField({
  name,
  value,
  placeholder,
  tooltip,
  onChange,
  onBlur,
  ariaLabel,
  required,
  width,
  labelWidth,
  testId,
}: PropsWithChildren<VariableTextAreaFieldProps<any>>): ReactElement {
  const styles = useStyles(getStyles);
  const getLineCount = useCallback((value: any) => {
    if (value && typeof value === 'string') {
      return value.split('\n').length;
    }

    return 1;
  }, []);

  return (
    <InlineField label={name} labelWidth={labelWidth ?? 12} tooltip={tooltip}>
      <TextArea
        rows={getLineCount(value)}
        value={value}
        onChange={onChange}
        onBlur={onBlur}
        placeholder={placeholder}
        required={required}
        aria-label={ariaLabel}
        cols={width}
        className={styles.textarea}
        data-testid={testId}
      />
    </InlineField>
  );
}

function getStyles(theme: GrafanaTheme) {
  return {
    textarea: css`
      white-space: pre-wrap;
      min-height: 32px;
      height: auto;
      overflow: auto;
      padding: 6px 8px;
    `,
  };
}
