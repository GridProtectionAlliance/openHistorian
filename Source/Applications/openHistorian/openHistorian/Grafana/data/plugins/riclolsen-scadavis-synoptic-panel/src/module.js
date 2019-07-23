import { SCADAvisCtrl } from './scadavis_ctrl';
import { loadPluginCss } from 'app/plugins/sdk';

loadPluginCss({
  dark: 'plugins/scadavis-synoptic-panel/css/scadavis.dark.css',
  light: 'plugins/scadavis-synoptic-panel/css/scadavis.light.css',
});

export { SCADAvisCtrl as PanelCtrl };
