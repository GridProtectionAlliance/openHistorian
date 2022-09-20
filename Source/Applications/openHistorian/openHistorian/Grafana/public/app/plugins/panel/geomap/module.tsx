import React from 'react';

import { PanelPlugin } from '@grafana/data';
import { config } from '@grafana/runtime';
import { commonOptionsBuilder } from '@grafana/ui';

import { GeomapInstanceState, GeomapPanel } from './GeomapPanel';
import { LayersEditor } from './editor/LayersEditor';
import { MapViewEditor } from './editor/MapViewEditor';
import { getLayerEditor } from './editor/layerEditor';
import { mapPanelChangedHandler, mapMigrationHandler } from './migrations';
import { defaultView, GeomapPanelOptions, TooltipMode } from './types';

export const plugin = new PanelPlugin<GeomapPanelOptions>(GeomapPanel)
  .setNoPadding()
  .setPanelChangeHandler(mapPanelChangedHandler)
  .setMigrationHandler(mapMigrationHandler)
  .useFieldConfig({
    useCustomConfig: (builder) => {
      commonOptionsBuilder.addHideFrom(builder);
    },
  })
  .setPanelOptions((builder, context) => {
    let category = ['Map view'];
    builder.addCustomEditor({
      category,
      id: 'view',
      path: 'view',
      name: 'Initial view', // don't show it
      description: 'This location will show when the panel first loads.',
      editor: MapViewEditor,
      defaultValue: defaultView,
    });

    builder.addBooleanSwitch({
      category,
      path: 'view.shared',
      description: 'Use the same view across multiple panels.  Note: this may require a dashboard reload.',
      name: 'Share view',
      defaultValue: defaultView.shared,
    });

    const state = context.instanceState as GeomapInstanceState;
    if (!state?.layers) {
      // TODO? show spinner?
    } else {
      builder.addCustomEditor({
        category: ['Data layer'],
        id: 'layers',
        path: '',
        name: '',
        editor: LayersEditor,
      });

      const selected = state.layers[state.selected];
      if (state.selected && selected) {
        builder.addNestedOptions(
          getLayerEditor({
            state: selected,
            category: ['Data layer'],
            basemaps: false,
          })
        );
      }

      const baselayer = state.layers[0];
      if (config.geomapDisableCustomBaseLayer) {
        builder.addCustomEditor({
          category: ['Base layer'],
          id: 'layers',
          path: '',
          name: '',
          // eslint-disable-next-line react/display-name
          editor: () => <div>The base layer is configured by the server admin.</div>,
        });
      } else if (baselayer) {
        builder.addNestedOptions(
          getLayerEditor({
            state: baselayer,
            category: ['Base layer'],
            basemaps: true,
          })
        );
      }
    }

    // The controls section
    category = ['Map controls'];
    builder
      .addBooleanSwitch({
        category,
        path: 'controls.showZoom',
        description: 'Show zoom control buttons in the upper left corner',
        name: 'Show zoom control',
        defaultValue: true,
      })
      .addBooleanSwitch({
        category,
        path: 'controls.mouseWheelZoom',
        description: 'Enable zoom control via mouse wheel',
        name: 'Mouse wheel zoom',
        defaultValue: true,
      })
      .addBooleanSwitch({
        category,
        path: 'controls.showAttribution',
        name: 'Show attribution',
        description: 'Show the map source attribution info in the lower right',
        defaultValue: true,
      })
      .addBooleanSwitch({
        category,
        path: 'controls.showScale',
        name: 'Show scale',
        description: 'Indicate map scale',
        defaultValue: false,
      })
      .addBooleanSwitch({
        category,
        path: 'controls.showDebug',
        name: 'Show debug',
        description: 'Show map info',
        defaultValue: false,
      })
      .addRadio({
        category,
        path: 'tooltip.mode',
        name: 'Tooltip',
        defaultValue: TooltipMode.Details,
        settings: {
          options: [
            { label: 'None', value: TooltipMode.None, description: 'Show contents on click, not hover' },
            { label: 'Details', value: TooltipMode.Details, description: 'Show popup on hover' },
          ],
        },
      });
  });
