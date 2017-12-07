'use strict';

System.register(['./datasource', './query_ctrl'], function (_export, _context) {
  "use strict";

  var openHistorianDatasource, openHistorianDatasourceQueryCtrl, openHistorianConfigCtrl, openHistorianQueryOptionsCtrl, openHistorianAnnotationsQueryCtrl;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  return {
    setters: [function (_datasource) {
      openHistorianDatasource = _datasource.openHistorianDatasource;
    }, function (_query_ctrl) {
      openHistorianDatasourceQueryCtrl = _query_ctrl.openHistorianDatasourceQueryCtrl;
    }],
    execute: function () {
      _export('ConfigCtrl', openHistorianConfigCtrl = function openHistorianConfigCtrl() {
        _classCallCheck(this, openHistorianConfigCtrl);
      });

      openHistorianConfigCtrl.templateUrl = 'partials/config.html';

      _export('QueryOptionsCtrl', openHistorianQueryOptionsCtrl = function openHistorianQueryOptionsCtrl() {
        _classCallCheck(this, openHistorianQueryOptionsCtrl);
      });

      openHistorianQueryOptionsCtrl.templateUrl = 'partials/query.options.html';

      _export('AnnotationsQueryCtrl', openHistorianAnnotationsQueryCtrl = function openHistorianAnnotationsQueryCtrl() {
        _classCallCheck(this, openHistorianAnnotationsQueryCtrl);
      });

      openHistorianAnnotationsQueryCtrl.templateUrl = 'partials/annotations.editor.html';

      _export('Datasource', openHistorianDatasource);

      _export('QueryCtrl', openHistorianDatasourceQueryCtrl);

      _export('ConfigCtrl', openHistorianConfigCtrl);

      _export('QueryOptionsCtrl', openHistorianQueryOptionsCtrl);

      _export('AnnotationsQueryCtrl', openHistorianAnnotationsQueryCtrl);
    }
  };
});