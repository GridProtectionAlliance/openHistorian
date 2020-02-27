import { sharedSingleStatPanelChangedHandler } from '@grafana/ui';
import { PanelPlugin } from '@grafana/data';
import { BarGaugePanel } from './BarGaugePanel';
import { BarGaugePanelEditor } from './BarGaugePanelEditor';
import { BarGaugeOptions, defaults } from './types';
import { barGaugePanelMigrationHandler } from './BarGaugeMigrations';

export const plugin = new PanelPlugin<BarGaugeOptions>(BarGaugePanel)
  .setDefaults(defaults)
  .setEditor(BarGaugePanelEditor)
  .setPanelChangeHandler(sharedSingleStatPanelChangedHandler)
  .setMigrationHandler(barGaugePanelMigrationHandler);
