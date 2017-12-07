'use strict';

System.register(['./config_ctrl', './datasource', './query_ctrl', './annotation_ctrl'], function (_export, _context) {
  "use strict";

  var PiWebApiConfigCtrl, PiWebApiDatasource, PiWebApiDatasourceQueryCtrl, PiWebApiAnnotationsQueryCtrl, PiWebApiQueryOptionsCtrl;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  return {
    setters: [function (_config_ctrl) {
      PiWebApiConfigCtrl = _config_ctrl.PiWebApiConfigCtrl;
    }, function (_datasource) {
      PiWebApiDatasource = _datasource.PiWebApiDatasource;
    }, function (_query_ctrl) {
      PiWebApiDatasourceQueryCtrl = _query_ctrl.PiWebApiDatasourceQueryCtrl;
    }, function (_annotation_ctrl) {
      PiWebApiAnnotationsQueryCtrl = _annotation_ctrl.PiWebApiAnnotationsQueryCtrl;
    }],
    execute: function () {
      _export('QueryOptionsCtrl', PiWebApiQueryOptionsCtrl = function PiWebApiQueryOptionsCtrl() {
        _classCallCheck(this, PiWebApiQueryOptionsCtrl);
      });

      PiWebApiQueryOptionsCtrl.templateUrl = 'partials/query.options.html';

      _export('Datasource', PiWebApiDatasource);

      _export('QueryCtrl', PiWebApiDatasourceQueryCtrl);

      _export('ConfigCtrl', PiWebApiConfigCtrl);

      _export('QueryOptionsCtrl', PiWebApiQueryOptionsCtrl);

      _export('AnnotationsQueryCtrl', PiWebApiAnnotationsQueryCtrl);
    }
  };
});
//# sourceMappingURL=module.js.map
