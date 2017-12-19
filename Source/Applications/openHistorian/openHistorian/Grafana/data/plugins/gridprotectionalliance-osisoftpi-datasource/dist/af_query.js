'use strict';

System.register(['lodash'], function (_export, _context) {
  "use strict";

  var _, PiWebApiQuery;

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
      _export('PiWebApiQuery', PiWebApiQuery = function PiWebApiQuery() {
        _classCallCheck(this, PiWebApiQuery);

        this.server = '';
        this.path = '';
        this.attributes = [];
        this.webids = [];
      });

      _export('PiWebApiQuery', PiWebApiQuery);
    }
  };
});
//# sourceMappingURL=af_query.js.map
