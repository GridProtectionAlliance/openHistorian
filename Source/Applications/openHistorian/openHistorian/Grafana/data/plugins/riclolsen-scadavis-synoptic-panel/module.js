'use strict';

System.register(['./scadavis_ctrl', 'app/plugins/sdk'], function (_export, _context) {
  "use strict";

  var SCADAvisCtrl, loadPluginCss;
  return {
    setters: [function (_scadavis_ctrl) {
      SCADAvisCtrl = _scadavis_ctrl.SCADAvisCtrl;
    }, function (_appPluginsSdk) {
      loadPluginCss = _appPluginsSdk.loadPluginCss;
    }],
    execute: function () {

      loadPluginCss({
        dark: 'plugins/scadavis-synoptic-panel/css/scadavis.dark.css',
        light: 'plugins/scadavis-synoptic-panel/css/scadavis.light.css'
      });

      _export('PanelCtrl', SCADAvisCtrl);
    }
  };
});
//# sourceMappingURL=module.js.map
