// Libraries
import React, { PureComponent } from 'react';

import {
  ThresholdsEditor,
  ValueMappingsEditor,
  PanelOptionsGrid,
  FieldDisplayEditor,
  FieldPropertiesEditor,
  PanelOptionsGroup,
  FormLabel,
  Select,
  DataLinksEditor,
  Switch,
} from '@grafana/ui';
import {
  ThresholdsConfig,
  ValueMapping,
  FieldDisplayOptions,
  FieldConfig,
  DataLink,
  PanelEditorProps,
} from '@grafana/data';
import { BarGaugeOptions, displayModes } from './types';
import { orientationOptions } from '../gauge/types';
import {
  getDataLinksVariableSuggestions,
  getCalculationValueDataLinksVariableSuggestions,
} from 'app/features/panel/panellinks/link_srv';
import { config } from 'app/core/config';

export class BarGaugePanelEditor extends PureComponent<PanelEditorProps<BarGaugeOptions>> {
  onThresholdsChanged = (thresholds: ThresholdsConfig) => {
    const current = this.props.options.fieldOptions.defaults;
    this.onDefaultsChange({
      ...current,
      thresholds,
    });
  };

  onValueMappingsChanged = (mappings: ValueMapping[]) => {
    const current = this.props.options.fieldOptions.defaults;
    this.onDefaultsChange({
      ...current,
      mappings,
    });
  };

  onDisplayOptionsChanged = (fieldOptions: FieldDisplayOptions) =>
    this.props.onOptionsChange({
      ...this.props.options,
      fieldOptions,
    });

  onDefaultsChange = (field: FieldConfig) => {
    this.onDisplayOptionsChanged({
      ...this.props.options.fieldOptions,
      defaults: field,
    });
  };

  onOrientationChange = ({ value }: any) => this.props.onOptionsChange({ ...this.props.options, orientation: value });
  onDisplayModeChange = ({ value }: any) => this.props.onOptionsChange({ ...this.props.options, displayMode: value });
  onToggleShowUnfilled = () => {
    this.props.onOptionsChange({ ...this.props.options, showUnfilled: !this.props.options.showUnfilled });
  };

  onDataLinksChanged = (links: DataLink[]) => {
    this.onDefaultsChange({
      ...this.props.options.fieldOptions.defaults,
      links,
    });
  };
  render() {
    const { options } = this.props;
    const { fieldOptions } = options;
    const { defaults } = fieldOptions;

    const suggestions = fieldOptions.values
      ? getDataLinksVariableSuggestions(this.props.data.series)
      : getCalculationValueDataLinksVariableSuggestions(this.props.data.series);
    const labelWidth = 6;

    return (
      <>
        <PanelOptionsGrid>
          <PanelOptionsGroup title="Display">
            <FieldDisplayEditor onChange={this.onDisplayOptionsChanged} value={fieldOptions} labelWidth={labelWidth} />
            <div className="form-field">
              <FormLabel width={labelWidth}>Orientation</FormLabel>
              <Select
                width={12}
                options={orientationOptions}
                defaultValue={orientationOptions[0]}
                onChange={this.onOrientationChange}
                value={orientationOptions.find(item => item.value === options.orientation)}
              />
            </div>
            <div className="form-field">
              <FormLabel width={labelWidth}>Mode</FormLabel>
              <Select
                width={12}
                options={displayModes}
                defaultValue={displayModes[0]}
                onChange={this.onDisplayModeChange}
                value={displayModes.find(item => item.value === options.displayMode)}
              />
            </div>
            {options.displayMode !== 'lcd' && (
              <Switch
                label="Unfilled"
                labelClass={`width-${labelWidth}`}
                checked={options.showUnfilled}
                onChange={this.onToggleShowUnfilled}
              />
            )}
          </PanelOptionsGroup>
          <PanelOptionsGroup title="Field">
            <FieldPropertiesEditor
              showMinMax={true}
              showTitle={true}
              onChange={this.onDefaultsChange}
              value={defaults}
            />
          </PanelOptionsGroup>

          <ThresholdsEditor
            onChange={this.onThresholdsChanged}
            thresholds={defaults.thresholds}
            theme={config.theme}
            showAlphaUI={config.featureToggles.newEdit}
          />
        </PanelOptionsGrid>

        <ValueMappingsEditor onChange={this.onValueMappingsChanged} valueMappings={defaults.mappings} />

        <PanelOptionsGroup title="Data links">
          <DataLinksEditor
            value={defaults.links}
            onChange={this.onDataLinksChanged}
            suggestions={suggestions}
            maxLinks={10}
          />
        </PanelOptionsGroup>
      </>
    );
  }
}
