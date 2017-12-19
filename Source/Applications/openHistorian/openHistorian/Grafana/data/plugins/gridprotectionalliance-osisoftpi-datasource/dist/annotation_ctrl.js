'use strict';

System.register(['lodash'], function (_export, _context) {
  "use strict";

  var _, _createClass, PiWebApiAnnotationsQueryCtrl;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  return {
    setters: [function (_lodash) {
      _ = _lodash.default;
    }],
    execute: function () {
      _createClass = function () {
        function defineProperties(target, props) {
          for (var i = 0; i < props.length; i++) {
            var descriptor = props[i];
            descriptor.enumerable = descriptor.enumerable || false;
            descriptor.configurable = true;
            if ("value" in descriptor) descriptor.writable = true;
            Object.defineProperty(target, descriptor.key, descriptor);
          }
        }

        return function (Constructor, protoProps, staticProps) {
          if (protoProps) defineProperties(Constructor.prototype, protoProps);
          if (staticProps) defineProperties(Constructor, staticProps);
          return Constructor;
        };
      }();

      _export('PiWebApiAnnotationsQueryCtrl', PiWebApiAnnotationsQueryCtrl = function () {
        function PiWebApiAnnotationsQueryCtrl($scope) {
          _classCallCheck(this, PiWebApiAnnotationsQueryCtrl);

          this.$scope = $scope;
          this.annotation.query = this.annotation.query || {};
          this.databases = [];
          this.templates = [];
          this.getDatabases();
        }

        _createClass(PiWebApiAnnotationsQueryCtrl, [{
          key: 'templateChanged',
          value: function templateChanged() {}
        }, {
          key: 'databaseChanged',
          value: function databaseChanged() {
            this.getEventFrames();
          }
        }, {
          key: 'getDatabases',
          value: function getDatabases() {
            var ctrl = this;
            ctrl.datasource.getDatabases(this.datasource.afserver.webid).then(function (dbs) {
              ctrl.databases = dbs;
            });
          }
        }, {
          key: 'getEventFrames',
          value: function getEventFrames() {
            var ctrl = this;
            ctrl.datasource.getEventFrameTemplates(ctrl.database.WebId).then(function (templates) {
              ctrl.templates = templates;
            });
          }
        }]);

        return PiWebApiAnnotationsQueryCtrl;
      }());

      _export('PiWebApiAnnotationsQueryCtrl', PiWebApiAnnotationsQueryCtrl);

      PiWebApiAnnotationsQueryCtrl.templateUrl = 'partials/annotations.editor.html';
    }
  };
});
//# sourceMappingURL=annotation_ctrl.js.map
