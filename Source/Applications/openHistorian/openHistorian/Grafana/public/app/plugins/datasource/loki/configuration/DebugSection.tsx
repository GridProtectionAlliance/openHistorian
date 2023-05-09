import { css } from '@emotion/css';
import cx from 'classnames';
import React, { ReactNode, useState } from 'react';

import { ArrayVector, Field, FieldType, LinkModel } from '@grafana/data';
import { LegacyForms } from '@grafana/ui';

import { getFieldLinksForExplore } from '../../../../features/explore/utils/links';
import { DerivedFieldConfig } from '../types';

const { FormField } = LegacyForms;

type Props = {
  derivedFields?: DerivedFieldConfig[];
  className?: string;
};
export const DebugSection = (props: Props) => {
  const { derivedFields, className } = props;
  const [debugText, setDebugText] = useState('');

  let debugFields: DebugField[] = [];
  if (debugText && derivedFields) {
    debugFields = makeDebugFields(derivedFields, debugText);
  }

  return (
    <div className={className}>
      <FormField
        labelWidth={12}
        label={'Debug log message'}
        inputEl={
          <textarea
            placeholder={'Paste an example log line here to test the regular expressions of your derived fields'}
            className={cx(
              'gf-form-input gf-form-textarea',
              css`
                width: 100%;
              `
            )}
            value={debugText}
            onChange={(event) => setDebugText(event.currentTarget.value)}
          />
        }
      />
      {!!debugFields.length && <DebugFields fields={debugFields} />}
    </div>
  );
};

type DebugFieldItemProps = {
  fields: DebugField[];
};
const DebugFields = ({ fields }: DebugFieldItemProps) => {
  return (
    <table className={'filter-table'}>
      <thead>
        <tr>
          <th>Name</th>
          <th>Value</th>
          <th>Url</th>
        </tr>
      </thead>
      <tbody>
        {fields.map((field) => {
          let value: ReactNode = field.value;
          if (field.error && field.error instanceof Error) {
            value = field.error.message;
          } else if (field.href) {
            value = <a href={field.href}>{value}</a>;
          }
          return (
            <tr key={`${field.name}=${field.value}`}>
              <td>{field.name}</td>
              <td>{value}</td>
              <td>{field.href ? <a href={field.href}>{field.href}</a> : ''}</td>
            </tr>
          );
        })}
      </tbody>
    </table>
  );
};

type DebugField = {
  name: string;
  error?: unknown;
  value?: string;
  href?: string;
};

function makeDebugFields(derivedFields: DerivedFieldConfig[], debugText: string): DebugField[] {
  return derivedFields
    .filter((field) => field.name && field.matcherRegex)
    .map((field) => {
      try {
        const testMatch = debugText.match(field.matcherRegex);
        const value = testMatch && testMatch[1];
        let link: LinkModel<Field> | null = null;

        if (field.url && value) {
          link = getFieldLinksForExplore({
            field: {
              name: '',
              type: FieldType.string,
              values: new ArrayVector([value]),
              config: {
                links: [{ title: '', url: field.url }],
              },
            },
            rowIndex: 0,
            range: {} as any,
          })[0];
        }

        const result: DebugField = {
          name: field.name,
          value: value || '<no match>',
          href: link ? link.href : undefined,
        };
        return result;
      } catch (error) {
        const result: DebugField = {
          name: field.name,
          error,
        };
        return result;
      }
    });
}
