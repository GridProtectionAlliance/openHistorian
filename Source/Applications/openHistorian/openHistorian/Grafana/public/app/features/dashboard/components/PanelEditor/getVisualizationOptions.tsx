import { get as lodashGet } from 'lodash';
import React from 'react';

import {
  EventBus,
  InterpolateFunction,
  PanelData,
  StandardEditorContext,
  VariableSuggestionsScope,
} from '@grafana/data';
import { PanelOptionsSupplier } from '@grafana/data/src/panel/PanelPlugin';
import {
  isNestedPanelOptions,
  NestedValueAccess,
  PanelOptionsEditorBuilder,
} from '@grafana/data/src/utils/OptionsUIBuilders';
import { getDataLinksVariableSuggestions } from 'app/features/panel/panellinks/link_srv';

import { OptionsPaneCategoryDescriptor } from './OptionsPaneCategoryDescriptor';
import { OptionsPaneItemDescriptor } from './OptionsPaneItemDescriptor';
import { getOptionOverrides } from './state/getOptionOverrides';
import { OptionPaneRenderProps } from './types';
import { setOptionImmutably, updateDefaultFieldConfigValue } from './utils';

type categoryGetter = (categoryNames?: string[]) => OptionsPaneCategoryDescriptor;

interface GetStandardEditorContextProps {
  data: PanelData | undefined;
  replaceVariables: InterpolateFunction;
  options: Record<string, unknown>;
  eventBus: EventBus;
  instanceState: OptionPaneRenderProps['instanceState'];
}

export function getStandardEditorContext({
  data,
  replaceVariables,
  options,
  eventBus,
  instanceState,
}: GetStandardEditorContextProps): StandardEditorContext<unknown, unknown> {
  const dataSeries = data?.series ?? [];

  const context: StandardEditorContext<unknown, unknown> = {
    data: dataSeries,
    replaceVariables,
    options,
    eventBus,
    getSuggestions: (scope?: VariableSuggestionsScope) => getDataLinksVariableSuggestions(dataSeries, scope),
    instanceState,
  };

  return context;
}

export function getVisualizationOptions(props: OptionPaneRenderProps): OptionsPaneCategoryDescriptor[] {
  const { plugin, panel, onPanelOptionsChanged, onFieldConfigsChange, data, dashboard, instanceState } = props;
  const currentOptions = panel.getOptions();
  const currentFieldConfig = panel.fieldConfig;
  const categoryIndex: Record<string, OptionsPaneCategoryDescriptor> = {};

  const context = getStandardEditorContext({
    data,
    replaceVariables: panel.replaceVariables,
    options: currentOptions,
    eventBus: dashboard.events,
    instanceState,
  });

  const getOptionsPaneCategory = (categoryNames?: string[]): OptionsPaneCategoryDescriptor => {
    const categoryName = (categoryNames && categoryNames[0]) ?? `${plugin.meta.name}`;
    const category = categoryIndex[categoryName];

    if (category) {
      return category;
    }

    return (categoryIndex[categoryName] = new OptionsPaneCategoryDescriptor({
      title: categoryName,
      id: categoryName,
    }));
  };

  const access: NestedValueAccess = {
    getValue: (path: string) => lodashGet(currentOptions, path),
    onChange: (path: string, value: any) => {
      const newOptions = setOptionImmutably(currentOptions, path, value);
      onPanelOptionsChanged(newOptions);
    },
  };

  // Load the options into categories
  fillOptionsPaneItems(plugin.getPanelOptionsSupplier(), access, getOptionsPaneCategory, context);

  /**
   * Field options
   */
  for (const fieldOption of plugin.fieldConfigRegistry.list()) {
    if (
      fieldOption.isCustom &&
      fieldOption.showIf &&
      !fieldOption.showIf(currentFieldConfig.defaults.custom, data?.series)
    ) {
      continue;
    }

    if (fieldOption.hideFromDefaults) {
      continue;
    }

    const category = getOptionsPaneCategory(fieldOption.category);
    const Editor = fieldOption.editor;

    const defaults = currentFieldConfig.defaults;
    const value = fieldOption.isCustom
      ? defaults.custom
        ? lodashGet(defaults.custom, fieldOption.path)
        : undefined
      : lodashGet(defaults, fieldOption.path);

    if (fieldOption.getItemsCount) {
      category.props.itemsCount = fieldOption.getItemsCount(value);
    }

    category.addItem(
      new OptionsPaneItemDescriptor({
        title: fieldOption.name,
        description: fieldOption.description,
        overrides: getOptionOverrides(fieldOption, currentFieldConfig, data?.series),
        render: function renderEditor() {
          const onChange = (v: any) => {
            onFieldConfigsChange(
              updateDefaultFieldConfigValue(currentFieldConfig, fieldOption.path, v, fieldOption.isCustom)
            );
          };

          return <Editor value={value} onChange={onChange} item={fieldOption} context={context} id={fieldOption.id} />;
        },
      })
    );
  }

  return Object.values(categoryIndex);
}

/**
 * This will iterate all options panes and add register them with the configured categories
 *
 * @internal
 */
export function fillOptionsPaneItems(
  supplier: PanelOptionsSupplier<any>,
  access: NestedValueAccess,
  getOptionsPaneCategory: categoryGetter,
  context: StandardEditorContext<any, any>,
  parentCategory?: OptionsPaneCategoryDescriptor
) {
  const builder = new PanelOptionsEditorBuilder<any>();
  supplier(builder, context);

  for (const pluginOption of builder.getItems()) {
    if (pluginOption.showIf && !pluginOption.showIf(context.options, context.data)) {
      continue;
    }

    let category = parentCategory;
    if (!category) {
      category = getOptionsPaneCategory(pluginOption.category);
    } else if (pluginOption.category?.[0]?.length) {
      category = category.getCategory(pluginOption.category[0]);
    }

    // Nested options get passed up one level
    if (isNestedPanelOptions(pluginOption)) {
      const subAccess = pluginOption.getNestedValueAccess(access);
      const subContext = subAccess.getContext
        ? subAccess.getContext(context)
        : { ...context, options: access.getValue(pluginOption.path) };

      fillOptionsPaneItems(
        pluginOption.getBuilder(),
        subAccess,
        getOptionsPaneCategory,
        subContext,
        category // parent category
      );
      continue;
    }

    const Editor = pluginOption.editor;
    category.addItem(
      new OptionsPaneItemDescriptor({
        title: pluginOption.name,
        description: pluginOption.description,
        render: function renderEditor() {
          return (
            <Editor
              value={access.getValue(pluginOption.path)}
              onChange={(value: any) => {
                access.onChange(pluginOption.path, value);
              }}
              item={pluginOption}
              context={context}
              id={pluginOption.id}
            />
          );
        },
      })
    );
  }
}
