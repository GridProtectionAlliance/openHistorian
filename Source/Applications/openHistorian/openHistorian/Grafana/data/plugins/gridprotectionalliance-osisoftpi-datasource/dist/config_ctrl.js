'use strict';

System.register(['lodash', './css/query-editor.css!'], function (_export, _context) {
  "use strict";

  var _, PiWebApiConfigCtrl;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  return {
    setters: [function (_lodash) {
      _ = _lodash.default;
    }, function (_cssQueryEditorCss) {}],
    execute: function () {
      _export('PiWebApiConfigCtrl', PiWebApiConfigCtrl = function PiWebApiConfigCtrl($scope) {
        _classCallCheck(this, PiWebApiConfigCtrl);

        this.current.jsonData = this.current.jsonData || {};

        if (!this.current.jsonData.url) {
          this.current.jsonData.url = this.current.url;
        }
        if (!this.current.jsonData.access) {
          this.current.jsonData.access = this.current.access;
        }
      });

      _export('PiWebApiConfigCtrl', PiWebApiConfigCtrl);

      PiWebApiConfigCtrl.templateUrl = 'partials/config.html';
    }
  };
});
//# sourceMappingURL=config_ctrl.js.map
