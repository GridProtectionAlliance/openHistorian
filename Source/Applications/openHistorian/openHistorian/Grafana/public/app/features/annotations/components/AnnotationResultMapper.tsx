import React, { PureComponent } from 'react';

import {
  SelectableValue,
  getFieldDisplayName,
  AnnotationEvent,
  AnnotationEventMappings,
  AnnotationEventFieldMapping,
  formattedValueToString,
  AnnotationEventFieldSource,
  getValueFormat,
} from '@grafana/data';

import { annotationEventNames, AnnotationFieldInfo } from '../standardAnnotationSupport';
import { Select, Tooltip, Icon } from '@grafana/ui';
import { AnnotationQueryResponse } from '../types';

// const valueOptions: Array<SelectableValue<AnnotationEventFieldSource>> = [
//   { value: AnnotationEventFieldSource.Field, label: 'Field', description: 'Set the field value from a response field' },
//   { value: AnnotationEventFieldSource.Text, label: 'Text', description: 'Enter direct text for the value' },
//   { value: AnnotationEventFieldSource.Skip, label: 'Skip', description: 'Hide this field' },
// ];

interface Props {
  response?: AnnotationQueryResponse;

  mappings?: AnnotationEventMappings;

  change: (mappings?: AnnotationEventMappings) => void;
}

interface State {
  fieldNames: Array<SelectableValue<string>>;
}

export class AnnotationFieldMapper extends PureComponent<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      fieldNames: [],
    };
  }

  updateFields = () => {
    const frame = this.props.response?.panelData?.series[0];
    if (frame && frame.fields) {
      const fieldNames = frame.fields.map(f => {
        const name = getFieldDisplayName(f, frame);

        let description = '';
        for (let i = 0; i < frame.length; i++) {
          if (i > 0) {
            description += ', ';
          }
          if (i > 2) {
            description += '...';
            break;
          }
          description += f.values.get(i);
        }

        if (description.length > 50) {
          description = description.substring(0, 50) + '...';
        }

        return {
          label: `${name} (${f.type})`,
          value: name,
          description,
        };
      });
      this.setState({ fieldNames });
    }
  };

  componentDidMount() {
    this.updateFields();
  }

  componentDidUpdate(oldProps: Props) {
    if (oldProps.response !== this.props.response) {
      this.updateFields();
    }
  }

  onFieldSourceChange = (k: keyof AnnotationEvent, v: SelectableValue<AnnotationEventFieldSource>) => {
    const mappings = this.props.mappings || {};
    const mapping = mappings[k] || {};

    this.props.change({
      ...mappings,
      [k]: {
        ...mapping,
        source: v.value || AnnotationEventFieldSource.Field,
      },
    });
  };

  onFieldNameChange = (k: keyof AnnotationEvent, v: SelectableValue<string>) => {
    const mappings = this.props.mappings || {};
    const mapping = mappings[k] || {};

    this.props.change({
      ...mappings,
      [k]: {
        ...mapping,
        value: v.value,
        source: AnnotationEventFieldSource.Field,
      },
    });
  };

  renderRow(row: AnnotationFieldInfo, mapping: AnnotationEventFieldMapping, first?: AnnotationEvent) {
    const { fieldNames } = this.state;

    let picker = fieldNames;
    const current = mapping.value;
    let currentValue = fieldNames.find(f => current === f.value);
    if (current) {
      picker = [...fieldNames];
      if (!currentValue) {
        picker.push({
          label: current,
          value: current,
        });
      }
    }

    let value = first ? first[row.key] : '';
    if (value && row.key.startsWith('time')) {
      const fmt = getValueFormat('dateTimeAsIso');
      value = formattedValueToString(fmt(value as number));
    }
    if (value === null || value === undefined) {
      value = ''; // empty string
    }

    return (
      <tr key={row.key}>
        <td>
          {row.key}{' '}
          {row.help && (
            <Tooltip content={row.help}>
              <Icon name="info-circle" />
            </Tooltip>
          )}
        </td>
        {/* <td>
          <Select
            value={valueOptions.find(v => v.value === mapping.source) || valueOptions[0]}
            options={valueOptions}
            onChange={(v: SelectableValue<AnnotationEventFieldSource>) => {
              this.onFieldSourceChange(row.key, v);
            }}
          />
        </td> */}
        <td>
          <Select
            value={currentValue}
            options={picker}
            placeholder={row.placeholder || row.key}
            onChange={(v: SelectableValue<string>) => {
              this.onFieldNameChange(row.key, v);
            }}
            noOptionsMessage="Unknown field names"
            allowCustomValue={true}
          />
        </td>
        <td>{`${value}`}</td>
      </tr>
    );
  }

  render() {
    const first = this.props.response?.events?.[0];
    const mappings = this.props.mappings || {};

    return (
      <table className="filter-table">
        <thead>
          <tr>
            <th>Annotation</th>
            <th>From</th>
            <th>First Value</th>
          </tr>
        </thead>
        <tbody>
          {annotationEventNames.map(row => {
            return this.renderRow(row, mappings[row.key] || {}, first);
          })}
        </tbody>
      </table>
    );
  }
}
