import { PanelModel } from '@grafana/data';

import { PanelOptions } from './models.gen';

export const canvasMigrationHandler = (panel: PanelModel): Partial<PanelOptions> => {
  const pluginVersion = panel?.pluginVersion ?? '';

  // Rename text-box to rectangle
  // Initial plugin version is empty string for first migration
  if (pluginVersion === '') {
    const root = panel.options?.root;
    if (root?.elements) {
      for (const element of root.elements) {
        if (element.type === 'text-box') {
          element.type = 'rectangle';
        }
      }
    }
  }

  return panel.options;
};
