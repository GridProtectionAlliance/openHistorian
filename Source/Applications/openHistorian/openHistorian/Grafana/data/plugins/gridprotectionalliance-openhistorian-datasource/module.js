(function(e, a) { for(var i in a) e[i] = a[i]; }(exports, /******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 3);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", { value: true });
exports.DefaultFlags = {
    'Select All': { Value: false, Order: -1, Flag: 0 },
    Normal: { Value: false, Order: 0, Flag: 0 },
    BadData: { Value: false, Order: 1, Flag: 1 << 0 },
    SuspectData: { Value: false, Order: 2, Flag: 1 << 1 },
    OverRangeError: { Value: false, Order: 3, Flag: 1 << 2 },
    UnderRangeError: { Value: false, Order: 4, Flag: 1 << 3 },
    AlarmHigh: { Value: false, Order: 5, Flag: 1 << 4 },
    AlarmLow: { Value: false, Order: 6, Flag: 1 << 5 },
    WarningHigh: { Value: false, Order: 7, Flag: 1 << 6 },
    WarningLow: { Value: false, Order: 8, Flag: 1 << 7 },
    FlatlineAlarm: { Value: false, Order: 9, Flag: 1 << 8 },
    ComparisonAlarm: { Value: false, Order: 10, Flag: 1 << 9 },
    ROCAlarm: { Value: false, Order: 11, Flag: 1 << 10 },
    ReceivedAsBad: { Value: false, Order: 12, Flag: 1 << 11 },
    CalculatedValue: { Value: false, Order: 13, Flag: 1 << 12 },
    CalculationError: { Value: false, Order: 14, Flag: 1 << 13 },
    CalculationWarning: { Value: false, Order: 15, Flag: 1 << 14 },
    ReservedQualityFlag: { Value: false, Order: 16, Flag: 1 << 15 },
    BadTime: { Value: false, Order: 17, Flag: 1 << 16 },
    SuspectTime: { Value: false, Order: 18, Flag: 1 << 17 },
    LateTimeAlarm: { Value: false, Order: 19, Flag: 1 << 18 },
    FutureTimeAlarm: { Value: false, Order: 20, Flag: 1 << 19 },
    UpSampled: { Value: false, Order: 21, Flag: 1 << 20 },
    DownSampled: { Value: false, Order: 22, Flag: 1 << 21 },
    DiscardedValue: { Value: false, Order: 23, Flag: 1 << 22 },
    ReservedTimeFlag: { Value: false, Order: 24, Flag: 1 << 23 },
    UserDefinedFlag1: { Value: false, Order: 25, Flag: 1 << 24 },
    UserDefinedFlag2: { Value: false, Order: 26, Flag: 1 << 25 },
    UserDefinedFlag3: { Value: false, Order: 27, Flag: 1 << 26 },
    UserDefinedFlag4: { Value: false, Order: 28, Flag: 1 << 27 },
    UserDefinedFlag5: { Value: false, Order: 29, Flag: 1 << 28 },
    SystemError: { Value: false, Order: 30, Flag: 1 << 29 },
    SystemWarning: { Value: false, Order: 31, Flag: 1 << 30 },
    MeasurementError: { Value: false, Order: 32, Flag: 1 << 31 }
};
exports.FunctionList = {
    Set: { Function: 'Set', Parameters: [] },
    Slice: { Function: 'Slice', Parameters: [{ Default: 1, Type: 'double', Description: 'A floating-point value that must be greater than or equal to zero that represents the desired time tolerance, in seconds, for the time slice.' }] },
    Average: { Function: 'Average', Parameters: [] },
    Minimum: { Function: 'Minimum', Parameters: [] },
    Maximum: { Function: 'Maximum', Parameters: [] },
    Total: { Function: 'Total', Parameters: [] },
    Range: { Function: 'Range', Parameters: [] },
    Count: { Function: 'Count', Parameters: [] },
    Distinct: { Function: 'Distinct', Parameters: [] },
    AbsoluteValute: { Function: 'AbsoluteValue', Parameters: [] },
    Add: { Function: 'Add', Parameters: [{ Default: 0, Type: 'string', Description: 'A floating point value representing an additive offset to be applied to each value the source series.' }] },
    Subtract: { Function: 'Subtract', Parameters: [{ Default: 0, Type: 'string', Description: 'A floating point value representing an additive offset to be applied to each value the source series.' }] },
    Multiply: { Function: 'Multiply', Parameters: [{ Default: 1, Type: 'string', Description: 'A floating point value representing an additive offset to be applied to each value the source series.' }] },
    Divide: { Function: 'Multiply', Parameters: [{ Default: 1, Type: 'string', Description: 'A floating point value representing an additive offset to be applied to each value the source series.' }] },
    Round: { Function: 'Round', Parameters: [{ Default: 0, Type: 'double', Description: 'A positive integer value representing the number of decimal places in the return value - defaults to 0.' }] },
    Floor: { Function: 'Floor', Parameters: [] },
    Ceiling: { Function: 'Ceiling', Parameters: [] },
    Truncate: { Function: 'Truncate', Parameters: [] },
    StandardDeviation: { Function: 'StandardDeviation', Parameters: [{ Default: false, Type: 'boolean', Description: 'A boolean flag representing if the sample based calculation should be used - defaults to false, which means the population based calculation should be used.' }] },
    Median: { Function: 'Median', Parameters: [] },
    Mode: { Function: 'Mode', Parameters: [] },
    Top: { Function: 'Top', Parameters: [{ Default: '100%', Type: 'string', Description: 'A positive integer value, representing a total, that is greater than zero - or - a floating point value, suffixed with \' %\' representing a percentage, that must range from greater than 0 to less than or equal to 100.' }, { Default: true, Type: 'boolean', Description: 'A boolean flag representing if time in dataset should be normalized - defaults to true.' }] },
    Bottom: { Function: 'Bottom', Parameters: [{ Default: '100%', Type: 'string', Description: 'A positive integer value, representing a total, that is greater than zero - or - a floating point value, suffixed with \' %\' representing a percentage, that must range from greater than 0 to less than or equal to 100.' }, { Default: true, Type: 'boolean', Description: 'A boolean flag representing if time in dataset should be normalized - defaults to true.' }] },
    Random: { Function: 'Random', Parameters: [{ Default: '100%', Type: 'string', Description: 'A positive integer value, representing a total, that is greater than zero - or - a floating point value, suffixed with \' %\' representing a percentage, that must range from greater than 0 to less than or equal to 100.' }, { Default: true, Type: 'boolean', Description: 'A boolean flag representing if time in dataset should be normalized - defaults to true.' }] },
    First: { Function: 'First', Parameters: [{ Default: '1', Type: 'string', Description: 'A positive integer value, representing a total, that is greater than zero - or - a floating point value, suffixed with \' %\' representing a percentage, that must range from greater than 0 to less than or equal to 100 - defaults to 1.' }] },
    Last: { Function: 'Last', Parameters: [{ Default: '1', Type: 'string', Description: 'A positive integer value, representing a total, that is greater than zero - or - a floating point value, suffixed with \' %\' representing a percentage, that must range from greater than 0 to less than or equal to 100 - defaults to 1.' }] },
    Percentile: { Function: 'Percentile', Parameters: [{ Default: '100%', Type: 'string', Description: 'A floating point value, representing a percentage, that must range from 0 to 100.' }] },
    Difference: { Function: 'Difference', Parameters: [] },
    TimeDifference: { Function: 'TimeDifference', Parameters: [{ Default: 'Seconds', Type: 'time', Description: 'Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or AtomicUnitsOfTime - defaults to Seconds.' }] },
    Derivative: { Function: 'Derivative', Parameters: [{ Default: 'Seconds', Type: 'time', Description: 'Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or AtomicUnitsOfTime - defaults to Seconds.' }] },
    TimeIntegration: { Function: 'TimeIntegration', Parameters: [{ Default: 'Hours', Type: 'time', Description: 'Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or AtomicUnitsOfTime - defaults to Hours.' }] },
    Interval: { Function: 'Interval', Parameters: [{ Default: 0, Type: 'double', Description: 'A floating-point value that must be greater than or equal to zero that represents the desired time interval, in time units, for the returned data. ' }, { Default: 'Seconds', Type: 'time', Description: 'Specifies the type of time units and must be one of the following: Seconds, Nanoseconds, Microseconds, Milliseconds, Minutes, Hours, Days, Weeks, Ke (i.e., traditional Chinese unit of decimal time), Ticks (i.e., 100-nanosecond intervals), PlanckTime or AtomicUnitsOfTime - defaults to Seconds.' }] },
    IncludeRange: { Function: 'IncludeRange', Parameters: [{ Default: 0, Type: 'double', Description: 'Floating-point number that represents the low range of values allowed in the return series.' }, { Default: 0, Type: 'double', Description: 'Floating-point number that represents the high range of values allowed in the return series.' }, { Default: false, Type: 'boolean', Description: 'A boolean flag that determines if range values are inclusive, i.e., allowed values are >= low and <= high - defaults to false, which means values are exclusive, i.e., allowed values are > low and < high.' }, { Default: false, Type: 'boolean', Description: 'A boolean flag - when four parameters are provided, third parameter determines if low value is inclusive and forth parameter determines if high value is inclusive.' }] },
    ExcludeRange: { Function: 'ExcludeRange', Parameters: [{ Default: 0, Type: 'double', Description: 'Floating-point number that represents the low range of values allowed in the return series.' }, { Default: 0, Type: 'double', Description: 'Floating-point number that represents the high range of values allowed in the return series.' }, { Default: false, Type: 'boolean', Description: 'A boolean flag that determines if range values are inclusive, i.e., allowed values are >= low and <= high - defaults to false, which means values are exclusive, i.e., allowed values are > low and < high.' }, { Default: false, Type: 'boolean', Description: 'A boolean flag - when four parameters are provided, third parameter determines if low value is inclusive and forth parameter determines if high value is inclusive.' }] },
    FilterNaN: { Function: 'FilterNaN', Parameters: [{ Default: true, Type: 'boolean', Description: 'A boolean flag that determines if infinite values should also be excluded - defaults to true.' }] },
    UnwrapAngle: { Function: 'UnwrapAngle', Parameters: [{ Default: 'Degrees', Type: 'angleUnits', Description: 'Specifies the type of angle units and must be one of the following: Degrees, Radians, Grads, ArcMinutes, ArcSeconds or AngularMil - defaults to Degrees.' }] },
    WrapAngle: { Function: 'WrapAngle', Parameters: [{ Default: 'Degrees', Type: 'angleUnits', Description: 'Specifies the type of angle units and must be one of the following: Degrees, Radians, Grads, ArcMinutes, ArcSeconds or AngularMil - defaults to Degrees.' }] },
    Label: { Function: 'Label', Parameters: [{ Default: 'Name', Type: 'string', Description: 'Renames a series with the specified label value.' }] }
};
exports.WhereOperators = ['=', '<>', '<', '<=', '>', '>=', 'LIKE', 'NOT LIKE', 'IN', 'NOT IN', 'IS', 'IS NOT'];
exports.Booleans = ['true', 'false'];
exports.AngleUnits = ['Degrees', 'Radians', 'Grads', 'ArcMinutes', 'ArcSeconds', 'AngularMil'];
exports.TimeUnits = ['Weeks', 'Days', 'Hours', 'Minutes', 'Seconds', 'Milliseconds', 'Microseconds', 'Nanoseconds', 'Ticks', 'PlankTime', 'AtomicUnitsOfTime', 'Ke'];

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var PanelCtrl = function () {
    function PanelCtrl($scope, $injector) {
        _classCallCheck(this, PanelCtrl);
    }

    _createClass(PanelCtrl, [{
        key: "init",
        value: function init() {}
    }, {
        key: "renderingCompleted",
        value: function renderingCompleted() {}
    }, {
        key: "refresh",
        value: function refresh() {}
    }, {
        key: "publishAppEvent",
        value: function publishAppEvent(evtName, evt) {}
    }, {
        key: "changeView",
        value: function changeView(fullscreen, edit) {}
    }, {
        key: "viewPanel",
        value: function viewPanel() {
            this.changeView(true, false);
        }
    }, {
        key: "editPanel",
        value: function editPanel() {
            this.changeView(true, true);
        }
    }, {
        key: "exitFullscreen",
        value: function exitFullscreen() {
            this.changeView(false, false);
        }
    }, {
        key: "initEditMode",
        value: function initEditMode() {}
    }, {
        key: "changeTab",
        value: function changeTab(newIndex) {}
    }, {
        key: "addEditorTab",
        value: function addEditorTab(title, directiveFn, index) {}
    }, {
        key: "getMenu",
        value: function getMenu() {
            return [];
        }
    }, {
        key: "getExtendedMenu",
        value: function getExtendedMenu() {
            return [];
        }
    }, {
        key: "otherPanelInFullscreenMode",
        value: function otherPanelInFullscreenMode() {
            return false;
        }
    }, {
        key: "calculatePanelHeight",
        value: function calculatePanelHeight() {}
    }, {
        key: "render",
        value: function render(payload) {}
    }, {
        key: "toggleEditorHelp",
        value: function toggleEditorHelp(index) {}
    }, {
        key: "duplicate",
        value: function duplicate() {}
    }, {
        key: "updateColumnSpan",
        value: function updateColumnSpan(span) {}
    }, {
        key: "removePanel",
        value: function removePanel() {}
    }, {
        key: "editPanelJson",
        value: function editPanelJson() {}
    }, {
        key: "replacePanel",
        value: function replacePanel(newPanel, oldPanel) {}
    }, {
        key: "sharePanel",
        value: function sharePanel() {}
    }, {
        key: "getInfoMode",
        value: function getInfoMode() {}
    }, {
        key: "getInfoContent",
        value: function getInfoContent(options) {}
    }, {
        key: "openInspector",
        value: function openInspector() {}
    }]);

    return PanelCtrl;
}();

exports.PanelCtrl = PanelCtrl;

/***/ }),
/* 2 */
/***/ (function(module, exports) {

var g;

// This works in non-strict mode
g = (function() {
	return this;
})();

try {
	// This works if eval is allowed (see CSP)
	g = g || new Function("return this")();
} catch (e) {
	// This works if the window reference is available
	if (typeof window === "object") g = window;
}

// g can still be undefined, but nothing to do about it...
// We return undefined, instead of nothing here, so it's
// easier to handle this case. if(!global) { ...}

module.exports = g;


/***/ }),
/* 3 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianDatasource_1 = __webpack_require__(4);
exports.Datasource = openHistorianDatasource_1.default;
var openHistorianQuery_ctrl_1 = __webpack_require__(5);
exports.QueryCtrl = openHistorianQuery_ctrl_1.default;
var openHistorianConfig_ctrl_1 = __webpack_require__(13);
exports.ConfigCtrl = openHistorianConfig_ctrl_1.default;
var openHistorianQueryOptions_ctrl_1 = __webpack_require__(14);
exports.QueryOptionsCtrl = openHistorianQueryOptions_ctrl_1.default;
var openHistorianAnnotations_ctrl_1 = __webpack_require__(15);
exports.AnnotationsQueryCtrl = openHistorianAnnotations_ctrl_1.default;
var openHistorianElementPicker_ctrl_1 = __webpack_require__(16);
var openHistorianTextEditor_ctrl_1 = __webpack_require__(17);
var openHistorianFilterExpression_ctrl_1 = __webpack_require__(18);
angular.module('grafana.directives').directive("queryOptions", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.options.html',
        restrict: 'E',
        controller: openHistorianQueryOptions_ctrl_1.default,
        controllerAs: 'queryOptionCtrl',
        scope: {
            flags: "=",
            return: "="
        }
    };
});
angular.module('grafana.directives').directive("elementPicker", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.elementPicker.html',
        restrict: 'E',
        controller: openHistorianElementPicker_ctrl_1.default,
        controllerAs: 'openHistorianElementPickerCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});
angular.module('grafana.directives').directive("textEditor", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.textEditor.html',
        restrict: 'E',
        controller: openHistorianTextEditor_ctrl_1.default,
        controllerAs: 'openHistorianTextEditorCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});
angular.module('grafana.directives').directive("filterExpression", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.filterExpression.html',
        restrict: 'E',
        controller: openHistorianFilterExpression_ctrl_1.default,
        controllerAs: 'openHistorianFilterExpressionCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});

/***/ }),
/* 4 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var OpenHistorianDataSource = function () {
    function OpenHistorianDataSource(instanceSettings, $q, backendSrv, templateSrv, uiSegmentSrv) {
        _classCallCheck(this, OpenHistorianDataSource);

        this.backendSrv = backendSrv;
        this.templateSrv = templateSrv;
        this.uiSegmentSrv = uiSegmentSrv;
        this.type = instanceSettings.type;
        this.url = instanceSettings.url;
        this.name = instanceSettings.name;
        this.q = $q;
        this.backendSrv = backendSrv;
        this.templateSrv = templateSrv;
        this.uiSegmentSrv = uiSegmentSrv;
        this.dataFlags = instanceSettings.jsonData.flags;
        this.options = {
            excludedDataFlags: instanceSettings.jsonData.Excluded == undefined ? 0 : instanceSettings.jsonData.Excluded,
            excludeNormalData: instanceSettings.jsonData.Normal == undefined ? false : instanceSettings.jsonData.Normal,
            updateAlarms: instanceSettings.jsonData.Alarms == undefined ? false : instanceSettings.jsonData.Alarms
        };
    }

    _createClass(OpenHistorianDataSource, [{
        key: "query",
        value: function query(options) {
            var query = this.buildQueryParameters(options);
            query.targets = query.targets.filter(function (t) {
                return !t.hide;
            });
            query.options = JSON.parse(JSON.stringify(this.options));
            if (query.targets.length <= 0) {
                return Promise.resolve({ data: [] });
            }
            var ctrl = this;
            if (this.options.updateAlarms) {
                this.backendSrv.datasourceRequest({
                    url: this.url + '/GetAlarms',
                    data: query,
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' }
                }).then(function (data) {
                    ctrl.GetDashboard(data.data, query, ctrl);
                }).catch(function (data) {});
            }
            return this.backendSrv.datasourceRequest({
                url: this.url + '/query',
                data: query,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "testDatasource",
        value: function testDatasource() {
            return this.backendSrv.datasourceRequest({
                url: this.url + '/',
                method: 'GET'
            }).then(function (response) {
                if (response.status === 200) {
                    return { status: "success", message: "Data source is working", title: "Success" };
                }
            });
        }
    }, {
        key: "annotationQuery",
        value: function annotationQuery(options) {
            var query = this.templateSrv.replace(options.annotation.query, {}, 'glob');
            var annotationQuery = {
                range: options.range,
                annotation: {
                    name: options.annotation.name,
                    datasource: options.annotation.datasource,
                    enable: options.annotation.enable,
                    iconColor: options.annotation.iconColor,
                    query: query
                },
                rangeRaw: options.rangeRaw
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/annotations',
                method: 'POST',
                data: annotationQuery
            }).then(function (result) {
                return result.data;
            });
        }
    }, {
        key: "metricFindQuery",
        value: function metricFindQuery(options, optionalOptions) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/search',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            }).then(this.mapToTextValue);
        }
    }, {
        key: "whereFindQuery",
        value: function whereFindQuery(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/SearchFields',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            }).then(this.mapToTextValue);
        }
    }, {
        key: "mapToTextValue",
        value: function mapToTextValue(result) {
            return _.map(result.data, function (d, i) {
                return { text: d, value: d };
            });
        }
    }, {
        key: "buildQueryParameters",
        value: function buildQueryParameters(options) {
            var _this = this;
            options.targets = _.filter(options.targets, function (target) {
                return target.target !== 'select metric';
            });
            var targets = _.map(options.targets, function (target) {
                return {
                    target: _this.fixTemplates(target),
                    refId: target.refId,
                    hide: target.hide,
                    excludedFlags: ((target || {}).queryOptions || {}).Excluded || 0,
                    excludeNormalFlags: ((target || {}).queryOptions || {}).Normal || false,
                    queryType: target.queryType,
                    queryOptions: target.queryOptions
                };
            });
            options.targets = targets;
            return options;
        }
    }, {
        key: "filterFindQuery",
        value: function filterFindQuery() {
            return this.backendSrv.datasourceRequest({
                url: this.url + '/SearchFilters',
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            }).then(this.mapToTextValue);
        }
    }, {
        key: "orderByFindQuery",
        value: function orderByFindQuery(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/SearchOrderBys',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            }).then(this.mapToTextValue);
        }
    }, {
        key: "getMetaData",
        value: function getMetaData(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/GetMetadata',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "getAlarmStates",
        value: function getAlarmStates(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/GetAlarmState',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "getPossibleAlarmStates",
        value: function getPossibleAlarmStates(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/GetDeviceAlarms',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "getDeviceGroups",
        value: function getDeviceGroups(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/GetDeviceGroups',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "getDataAvailability",
        value: function getDataAvailability(options) {
            var interpolated = {
                target: this.templateSrv.replace(options, null, 'regex')
            };
            return this.backendSrv.datasourceRequest({
                url: this.url + '/GetDataAvailability',
                data: interpolated,
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "fixTemplates",
        value: function fixTemplates(target) {
            if (target.target == undefined) return '';
            var ctrl = this;
            var sep = ' ';
            if (target.queryType == 'Element List') sep = ';';
            return target.target.split(sep).map(function (a) {
                if (ctrl.templateSrv.variableExists(a)) {
                    return ctrl.templateSrv.replaceWithText(a);
                } else return a;
            }).join(sep);
        }
    }, {
        key: "queryLocation",
        value: function queryLocation(target) {
            if (target.target == null || target.target == undefined) {
                target.target = {};
            }
            if (target.radius == null || target.radius == undefined || target.zoom == null || target.zoom == undefined) {
                return this.backendSrv.datasourceRequest({
                    method: "POST",
                    url: this.url + '/GetLocationData',
                    data: JSON.stringify(target.target),
                    headers: { 'Content-Type': 'application/json' }
                });
            }
            return this.backendSrv.datasourceRequest({
                method: "POST",
                url: this.url + '/GetLocationData?radius=' + target.radius + '&zoom=' + target.zoom,
                data: JSON.stringify(target.target),
                headers: { 'Content-Type': 'application/json' }
            });
        }
    }, {
        key: "GetDashboard",
        value: function GetDashboard(alarms, query, ctrl) {
            ctrl.backendSrv.datasourceRequest({
                url: ctrl.url + '/QueryAlarms',
                method: 'POST',
                data: query,
                headers: { 'Content-Type': 'application/json' }
            }).then(function (data) {}).catch(function (data) {});
            ctrl.backendSrv.datasourceRequest({
                url: 'api/search?dashboardIds=' + query.dashboardId,
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            }).then(function (data) {
                ctrl.backendSrv.datasourceRequest({
                    url: 'api/dashboards/uid/' + data.data[0]["uid"],
                    method: 'GET',
                    headers: { 'Content-Type': 'application/json' }
                }).then(function (data) {
                    ctrl.CheckPanel(alarms, query, data.data, ctrl);
                });
            });
        }
    }, {
        key: "CheckPanel",
        value: function CheckPanel(alarms, query, dashboard, ctrl) {
            var alerts = dashboard.dashboard.panels;
            if (alerts === undefined) return;
            alerts = alerts.find(function (item) {
                return item.id == query.panelId;
            });
            if (alerts == undefined || alerts == null) return;
            alerts = alerts.thresholds;
            if ((alerts == undefined || alerts == null || alerts.length == 0) && alarms.length == 0) return;
            if ((alerts == undefined || alerts == null || alerts.length == 0) && alarms.length > 0) {
                ctrl.UpdateAlarms(alarms, dashboard.dashboard.uid, query, ctrl);
                return;
            }
            var threshholds = [];
            try {
                threshholds = alerts.map(function (item) {
                    return item.value;
                });
            } catch (_a) {
                return;
            }
            var needsUpdate = false;
            alarms.forEach(function (item) {
                if (!threshholds.includes(item.SetPoint)) needsUpdate = true;
            });
            if (needsUpdate) {
                ctrl.UpdateAlarms(alarms, dashboard.dashboard.uid, query, ctrl);
            }
        }
    }, {
        key: "UpdateAlarms",
        value: function UpdateAlarms(alarms, dashboardUid, query, ctrl) {
            ctrl.backendSrv.datasourceRequest({
                url: 'api/dashboards/uid/' + dashboardUid,
                method: 'GET',
                headers: { 'Content-Type': 'application/json' }
            }).then(function (data) {
                var dashboard = data.data.dashboard;
                var panelIndex = dashboard.panels.findIndex(function (item) {
                    return item.id == query.panelId;
                });
                dashboard.panels[panelIndex].thresholds = alarms.map(function (item) {
                    var op = "gt";
                    if (item.Operation == 21 || item.Operation == 22) op = "lt";
                    var fill = true;
                    if (item.Operation == 1 || item.Operation == 2) fill = false;
                    return {
                        colorMode: "critical",
                        fill: fill,
                        line: true,
                        op: op,
                        value: item.SetPoint
                    };
                });
                ctrl.backendSrv.datasourceRequest({
                    url: 'api/dashboards/db',
                    method: 'POST',
                    data: { overwrite: true, dashboard: dashboard },
                    headers: { 'Content-Type': 'application/json' }
                }).catch(function (data) {
                    console.log(data);
                });
            });
        }
    }]);

    return OpenHistorianDataSource;
}();

exports.default = OpenHistorianDataSource;

/***/ }),
/* 5 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

Object.defineProperty(exports, "__esModule", { value: true });
var sdk_1 = __webpack_require__(6);
__webpack_require__(9);

var OpenHistorianDataSourceQueryCtrl = function (_sdk_1$QueryCtrl) {
    _inherits(OpenHistorianDataSourceQueryCtrl, _sdk_1$QueryCtrl);

    function OpenHistorianDataSourceQueryCtrl($scope, $injector, uiSegmentSrv, templateSrv, $compile) {
        _classCallCheck(this, OpenHistorianDataSourceQueryCtrl);

        var _this = _possibleConstructorReturn(this, (OpenHistorianDataSourceQueryCtrl.__proto__ || Object.getPrototypeOf(OpenHistorianDataSourceQueryCtrl)).call(this, $scope, $injector));

        _this.uiSegmentSrv = uiSegmentSrv;
        _this.templateSrv = templateSrv;
        _this.$compile = $compile;
        _this.$scope = $scope;
        _this.$compile = $compile;
        var ctrl = _this;
        _this.uiSegmentSrv = uiSegmentSrv;
        _this.queryTypes = ["Element List", "Filter Expression", "Text Editor"];
        _this.queryType = _this.target.queryType == undefined ? "Element List" : _this.target.queryType;
        _this.queryOptionsOpen = false;
        if (ctrl.target.queryOptions == undefined) ctrl.target.queryOptions = { Excluded: ctrl.datasource.options.excludedDataFlags, Normal: ctrl.datasource.options.excludeNormalData };
        return _this;
    }

    _createClass(OpenHistorianDataSourceQueryCtrl, [{
        key: "toggleQueryOptions",
        value: function toggleQueryOptions() {
            this.queryOptionsOpen = !this.queryOptionsOpen;
        }
    }, {
        key: "onChangeInternal",
        value: function onChangeInternal() {
            this.panelCtrl.refresh();
        }
    }, {
        key: "changeQueryType",
        value: function changeQueryType() {
            if (this.queryType == 'Text Editor') {
                this.target.targetText = this.target.target;
            } else {
                this.target.target = '';
                delete this.target.functionSegments;
            }
        }
    }]);

    return OpenHistorianDataSourceQueryCtrl;
}(sdk_1.QueryCtrl);

exports.default = OpenHistorianDataSourceQueryCtrl;
OpenHistorianDataSourceQueryCtrl.templateUrl = 'partial/query.editor.html';

/***/ }),
/* 6 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


Object.defineProperty(exports, "__esModule", { value: true });
var panel_ctrl_1 = __webpack_require__(1);
exports.PanelCtrl = panel_ctrl_1.PanelCtrl;
var metrics_panel_ctrl_1 = __webpack_require__(7);
exports.MetricsPanelCtrl = metrics_panel_ctrl_1.MetricsPanelCtrl;
var query_ctrl_1 = __webpack_require__(8);
exports.QueryCtrl = query_ctrl_1.QueryCtrl;
function loadPluginCss(options) {}
exports.loadPluginCss = loadPluginCss;

/***/ }),
/* 7 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

Object.defineProperty(exports, "__esModule", { value: true });
var panel_ctrl_1 = __webpack_require__(1);

var MetricsPanelCtrl = function (_panel_ctrl_1$PanelCt) {
    _inherits(MetricsPanelCtrl, _panel_ctrl_1$PanelCt);

    function MetricsPanelCtrl($scope, $injector) {
        _classCallCheck(this, MetricsPanelCtrl);

        var _this = _possibleConstructorReturn(this, (MetricsPanelCtrl.__proto__ || Object.getPrototypeOf(MetricsPanelCtrl)).call(this, $scope, $injector));

        _this.editorTabIndex = 1;
        if (!_this.panel.targets) {
            _this.panel.targets = [{}];
        }
        return _this;
    }

    _createClass(MetricsPanelCtrl, [{
        key: "onPanelTearDown",
        value: function onPanelTearDown() {}
    }, {
        key: "onInitMetricsPanelEditMode",
        value: function onInitMetricsPanelEditMode() {}
    }, {
        key: "onMetricsPanelRefresh",
        value: function onMetricsPanelRefresh() {}
    }, {
        key: "setTimeQueryStart",
        value: function setTimeQueryStart() {}
    }, {
        key: "setTimeQueryEnd",
        value: function setTimeQueryEnd() {}
    }, {
        key: "updateTimeRange",
        value: function updateTimeRange(datasource) {}
    }, {
        key: "calculateInterval",
        value: function calculateInterval() {}
    }, {
        key: "applyPanelTimeOverrides",
        value: function applyPanelTimeOverrides() {}
    }, {
        key: "issueQueries",
        value: function issueQueries(datasource) {}
    }, {
        key: "handleQueryResult",
        value: function handleQueryResult(result) {}
    }, {
        key: "handleDataStream",
        value: function handleDataStream(stream) {}
    }, {
        key: "setDatasource",
        value: function setDatasource(datasource) {}
    }, {
        key: "addQuery",
        value: function addQuery(target) {}
    }, {
        key: "removeQuery",
        value: function removeQuery(target) {}
    }, {
        key: "moveQuery",
        value: function moveQuery(target, direction) {}
    }]);

    return MetricsPanelCtrl;
}(panel_ctrl_1.PanelCtrl);

exports.MetricsPanelCtrl = MetricsPanelCtrl;

/***/ }),
/* 8 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var QueryCtrl = function () {
    function QueryCtrl($scope, $injector) {
        _classCallCheck(this, QueryCtrl);

        this.$scope = $scope;
        this.$injector = $injector;
        this.panelCtrl = this.panelCtrl || { panel: {} };
        this.target = this.target || { target: '' };
        this.panel = this.panelCtrl.panel;
    }

    _createClass(QueryCtrl, [{
        key: "refresh",
        value: function refresh() {}
    }]);

    return QueryCtrl;
}();

exports.QueryCtrl = QueryCtrl;

/***/ }),
/* 9 */
/***/ (function(module, exports, __webpack_require__) {

var content = __webpack_require__(10);

if (typeof content === 'string') {
  content = [[module.i, content, '']];
}

var options = {}

options.insert = "head";
options.singleton = false;

var update = __webpack_require__(12)(content, options);

if (content.locals) {
  module.exports = content.locals;
}


/***/ }),
/* 10 */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(11)(false);
// Module
exports.push([module.i, ".generic-datasource-query-row .query-keyword {\n  width: 75px;\n}", ""]);


/***/ }),
/* 11 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


/*
  MIT License http://www.opensource.org/licenses/mit-license.php
  Author Tobias Koppers @sokra
*/
// css base code, injected by the css-loader
// eslint-disable-next-line func-names
module.exports = function (useSourceMap) {
  var list = []; // return the list of modules as css string

  list.toString = function toString() {
    return this.map(function (item) {
      var content = cssWithMappingToString(item, useSourceMap);

      if (item[2]) {
        return "@media ".concat(item[2], "{").concat(content, "}");
      }

      return content;
    }).join('');
  }; // import a list of modules into the list
  // eslint-disable-next-line func-names


  list.i = function (modules, mediaQuery) {
    if (typeof modules === 'string') {
      // eslint-disable-next-line no-param-reassign
      modules = [[null, modules, '']];
    }

    var alreadyImportedModules = {};

    for (var i = 0; i < this.length; i++) {
      // eslint-disable-next-line prefer-destructuring
      var id = this[i][0];

      if (id != null) {
        alreadyImportedModules[id] = true;
      }
    }

    for (var _i = 0; _i < modules.length; _i++) {
      var item = modules[_i]; // skip already imported module
      // this implementation is not 100% perfect for weird media query combinations
      // when a module is imported multiple times with different media queries.
      // I hope this will never occur (Hey this way we have smaller bundles)

      if (item[0] == null || !alreadyImportedModules[item[0]]) {
        if (mediaQuery && !item[2]) {
          item[2] = mediaQuery;
        } else if (mediaQuery) {
          item[2] = "(".concat(item[2], ") and (").concat(mediaQuery, ")");
        }

        list.push(item);
      }
    }
  };

  return list;
};

function cssWithMappingToString(item, useSourceMap) {
  var content = item[1] || ''; // eslint-disable-next-line prefer-destructuring

  var cssMapping = item[3];

  if (!cssMapping) {
    return content;
  }

  if (useSourceMap && typeof btoa === 'function') {
    var sourceMapping = toComment(cssMapping);
    var sourceURLs = cssMapping.sources.map(function (source) {
      return "/*# sourceURL=".concat(cssMapping.sourceRoot).concat(source, " */");
    });
    return [content].concat(sourceURLs).concat([sourceMapping]).join('\n');
  }

  return [content].join('\n');
} // Adapted from convert-source-map (MIT)


function toComment(sourceMap) {
  // eslint-disable-next-line no-undef
  var base64 = btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap))));
  var data = "sourceMappingURL=data:application/json;charset=utf-8;base64,".concat(base64);
  return "/*# ".concat(data, " */");
}

/***/ }),
/* 12 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var stylesInDom = {};

var isOldIE = function isOldIE() {
  var memo;
  return function memorize() {
    if (typeof memo === 'undefined') {
      // Test for IE <= 9 as proposed by Browserhacks
      // @see http://browserhacks.com/#hack-e71d8692f65334173fee715c222cb805
      // Tests for existence of standard globals is to allow style-loader
      // to operate correctly into non-standard environments
      // @see https://github.com/webpack-contrib/style-loader/issues/177
      memo = Boolean(window && document && document.all && !window.atob);
    }

    return memo;
  };
}();

var getTarget = function getTarget() {
  var memo = {};
  return function memorize(target) {
    if (typeof memo[target] === 'undefined') {
      var styleTarget = document.querySelector(target); // Special case to return head of iframe instead of iframe itself

      if (window.HTMLIFrameElement && styleTarget instanceof window.HTMLIFrameElement) {
        try {
          // This will throw an exception if access to iframe is blocked
          // due to cross-origin restrictions
          styleTarget = styleTarget.contentDocument.head;
        } catch (e) {
          // istanbul ignore next
          styleTarget = null;
        }
      }

      memo[target] = styleTarget;
    }

    return memo[target];
  };
}();

function listToStyles(list, options) {
  var styles = [];
  var newStyles = {};

  for (var i = 0; i < list.length; i++) {
    var item = list[i];
    var id = options.base ? item[0] + options.base : item[0];
    var css = item[1];
    var media = item[2];
    var sourceMap = item[3];
    var part = {
      css: css,
      media: media,
      sourceMap: sourceMap
    };

    if (!newStyles[id]) {
      styles.push(newStyles[id] = {
        id: id,
        parts: [part]
      });
    } else {
      newStyles[id].parts.push(part);
    }
  }

  return styles;
}

function addStylesToDom(styles, options) {
  for (var i = 0; i < styles.length; i++) {
    var item = styles[i];
    var domStyle = stylesInDom[item.id];
    var j = 0;

    if (domStyle) {
      domStyle.refs++;

      for (; j < domStyle.parts.length; j++) {
        domStyle.parts[j](item.parts[j]);
      }

      for (; j < item.parts.length; j++) {
        domStyle.parts.push(addStyle(item.parts[j], options));
      }
    } else {
      var parts = [];

      for (; j < item.parts.length; j++) {
        parts.push(addStyle(item.parts[j], options));
      }

      stylesInDom[item.id] = {
        id: item.id,
        refs: 1,
        parts: parts
      };
    }
  }
}

function insertStyleElement(options) {
  var style = document.createElement('style');

  if (typeof options.attributes.nonce === 'undefined') {
    var nonce =  true ? __webpack_require__.nc : undefined;

    if (nonce) {
      options.attributes.nonce = nonce;
    }
  }

  Object.keys(options.attributes).forEach(function (key) {
    style.setAttribute(key, options.attributes[key]);
  });

  if (typeof options.insert === 'function') {
    options.insert(style);
  } else {
    var target = getTarget(options.insert || 'head');

    if (!target) {
      throw new Error("Couldn't find a style target. This probably means that the value for the 'insert' parameter is invalid.");
    }

    target.appendChild(style);
  }

  return style;
}

function removeStyleElement(style) {
  // istanbul ignore if
  if (style.parentNode === null) {
    return false;
  }

  style.parentNode.removeChild(style);
}
/* istanbul ignore next  */


var replaceText = function replaceText() {
  var textStore = [];
  return function replace(index, replacement) {
    textStore[index] = replacement;
    return textStore.filter(Boolean).join('\n');
  };
}();

function applyToSingletonTag(style, index, remove, obj) {
  var css = remove ? '' : obj.css; // For old IE

  /* istanbul ignore if  */

  if (style.styleSheet) {
    style.styleSheet.cssText = replaceText(index, css);
  } else {
    var cssNode = document.createTextNode(css);
    var childNodes = style.childNodes;

    if (childNodes[index]) {
      style.removeChild(childNodes[index]);
    }

    if (childNodes.length) {
      style.insertBefore(cssNode, childNodes[index]);
    } else {
      style.appendChild(cssNode);
    }
  }
}

function applyToTag(style, options, obj) {
  var css = obj.css;
  var media = obj.media;
  var sourceMap = obj.sourceMap;

  if (media) {
    style.setAttribute('media', media);
  }

  if (sourceMap && btoa) {
    css += "\n/*# sourceMappingURL=data:application/json;base64,".concat(btoa(unescape(encodeURIComponent(JSON.stringify(sourceMap)))), " */");
  } // For old IE

  /* istanbul ignore if  */


  if (style.styleSheet) {
    style.styleSheet.cssText = css;
  } else {
    while (style.firstChild) {
      style.removeChild(style.firstChild);
    }

    style.appendChild(document.createTextNode(css));
  }
}

var singleton = null;
var singletonCounter = 0;

function addStyle(obj, options) {
  var style;
  var update;
  var remove;

  if (options.singleton) {
    var styleIndex = singletonCounter++;
    style = singleton || (singleton = insertStyleElement(options));
    update = applyToSingletonTag.bind(null, style, styleIndex, false);
    remove = applyToSingletonTag.bind(null, style, styleIndex, true);
  } else {
    style = insertStyleElement(options);
    update = applyToTag.bind(null, style, options);

    remove = function remove() {
      removeStyleElement(style);
    };
  }

  update(obj);
  return function updateStyle(newObj) {
    if (newObj) {
      if (newObj.css === obj.css && newObj.media === obj.media && newObj.sourceMap === obj.sourceMap) {
        return;
      }

      update(obj = newObj);
    } else {
      remove();
    }
  };
}

module.exports = function (list, options) {
  options = options || {};
  options.attributes = typeof options.attributes === 'object' ? options.attributes : {}; // Force single-tag solution on IE6-9, which has a hard limit on the # of <style>
  // tags it will allow on a page

  if (!options.singleton && typeof options.singleton !== 'boolean') {
    options.singleton = isOldIE();
  }

  var styles = listToStyles(list, options);
  addStylesToDom(styles, options);
  return function update(newList) {
    var mayRemove = [];

    for (var i = 0; i < styles.length; i++) {
      var item = styles[i];
      var domStyle = stylesInDom[item.id];

      if (domStyle) {
        domStyle.refs--;
        mayRemove.push(domStyle);
      }
    }

    if (newList) {
      var newStyles = listToStyles(newList, options);
      addStylesToDom(newStyles, options);
    }

    for (var _i = 0; _i < mayRemove.length; _i++) {
      var _domStyle = mayRemove[_i];

      if (_domStyle.refs === 0) {
        for (var j = 0; j < _domStyle.parts.length; j++) {
          _domStyle.parts[j]();
        }

        delete stylesInDom[_domStyle.id];
      }
    }
  };
};

/***/ }),
/* 13 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var OpenHistorianConfigCtrl = function OpenHistorianConfigCtrl($scope) {
    _classCallCheck(this, OpenHistorianConfigCtrl);

    var ctrl = this;
    ctrl.current.jsonData.Excluded = this.current.jsonData.Excluded || 0;
    ctrl.current.jsonData.Normal = this.current.jsonData.Normal || false;
    ctrl.current.jsonData.Alarms = this.current.jsonData.Alarms || false;
};

exports.default = OpenHistorianConfigCtrl;
OpenHistorianConfigCtrl.templateUrl = 'partial/config.html';

/***/ }),
/* 14 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(0);

var OpenHistorianQueryOptionsCtrl = function () {
    function OpenHistorianQueryOptionsCtrl($scope, $compile) {
        var _this = this;

        _classCallCheck(this, OpenHistorianQueryOptionsCtrl);

        this.$scope = $scope;
        this.$compile = $compile;
        this.$scope = $scope;
        var value = JSON.parse(JSON.stringify($scope.return));
        this.dataFlags = this.hex2flags(parseInt(value.Excluded));
        this.dataFlags['Normal'].Value = value.Normal;
        this.includeAlarm = value.Alarms;
        this.return = $scope.return;
        this.flagArray = _.map(Object.keys(this.dataFlags), function (a) {
            return { key: a, order: _this.dataFlags[a].Order };
        }).sort(function (a, b) {
            return a.order - b.order;
        });
    }

    _createClass(OpenHistorianQueryOptionsCtrl, [{
        key: "calculateFlags",
        value: function calculateFlags(flag) {
            var ctrl = this;
            var flagVarExcluded = ctrl.return.Excluded;
            if (flag == 'Select All') {
                _.each(Object.keys(ctrl.dataFlags), function (key, index, list) {
                    if (key == "Normal") ctrl.dataFlags[key].Value = false;else ctrl.dataFlags[key].Value = ctrl.dataFlags['Select All'].Value;
                });
                if (ctrl.dataFlags['Select All'].Value) flagVarExcluded = 0xFFFFFFFF;else flagVarExcluded = 0;
            } else {
                ctrl.dataFlags['Select All'].Value = false;
                flagVarExcluded ^= ctrl.dataFlags[flag].Flag;
            }
            ctrl.return.Excluded = flagVarExcluded;
            ctrl.return.Normal = ctrl.dataFlags['Normal'].Value;
        }
    }, {
        key: "changeAlarms",
        value: function changeAlarms() {
            var ctrl = this;
            ctrl.return.Alarms = ctrl.includeAlarm;
        }
    }, {
        key: "hex2flags",
        value: function hex2flags(hex) {
            var ctrl = this;
            var flag = hex;
            var flags = JSON.parse(JSON.stringify(openHistorianConstants_1.DefaultFlags));
            _.each(Object.keys(flags), function (key, index, list) {
                if (key == 'Select All') return;
                flags[key].Value = (flags[key].Flag & flag) != 0;
            });
            return flags;
        }
    }]);

    return OpenHistorianQueryOptionsCtrl;
}();

exports.default = OpenHistorianQueryOptionsCtrl;

/***/ }),
/* 15 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var OpenHistorianAnnotationsQueryCtrl = function OpenHistorianAnnotationsQueryCtrl() {
  _classCallCheck(this, OpenHistorianAnnotationsQueryCtrl);
};

exports.default = OpenHistorianAnnotationsQueryCtrl;
OpenHistorianAnnotationsQueryCtrl.templateUrl = 'partial/annotations.editor.html';

/***/ }),
/* 16 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(0);

var OpenHistorianElementPickerCtrl = function () {
    function OpenHistorianElementPickerCtrl($scope, uiSegmentSrv) {
        _classCallCheck(this, OpenHistorianElementPickerCtrl);

        this.$scope = $scope;
        this.uiSegmentSrv = uiSegmentSrv;
        var ctrl = this;
        this.$scope = $scope;
        this.uiSegmentSrv = uiSegmentSrv;
        this.segments = this.$scope.target.segments == undefined ? [] : this.$scope.target.segments.map(function (a) {
            return ctrl.uiSegmentSrv.newSegment({ value: a.text, expandable: true });
        });
        this.functionSegments = this.$scope.target.functionSegments == undefined ? [] : this.$scope.target.functionSegments;
        this.functions = [];
        this.elementSegment = this.uiSegmentSrv.newPlusButton();
        this.functionSegment = this.uiSegmentSrv.newPlusButton();
        this.buildFunctionArray();
        this.setTargetWithElements();
        delete $scope.target.wheres;
        delete $scope.target.topNSegment;
        delete $scope.target.orderBys;
        delete $scope.target.whereSegment;
        delete $scope.target.filterSegment;
        delete $scope.target.orderBySegment;
        delete $scope.target.targetText;
    }

    _createClass(OpenHistorianElementPickerCtrl, [{
        key: "getElementSegments",
        value: function getElementSegments(newSegment) {
            var ctrl = this;
            var option = null;
            if (event.target['value'] != "") option = event.target['value'];
            return ctrl.$scope.datasource.metricFindQuery(option).then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value) return -1;
                    if (a.value > b.value) return 1;
                    return 0;
                });
                _.each(ctrl.$scope.datasource.templateSrv.variables, function (item, index, list) {
                    if (item.type == "query") altSegments.unshift(ctrl.uiSegmentSrv.newCondition('$' + item.name));
                });
                if (!newSegment) altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));
                return _.filter(altSegments, function (segment) {
                    return _.find(ctrl.segments, function (x) {
                        return x.value == segment.value;
                    }) == undefined;
                });
            });
        }
    }, {
        key: "addElementSegment",
        value: function addElementSegment() {
            if (event.target['text'] != null) {
                this.segments.push(this.uiSegmentSrv.newSegment({ value: event.target['text'], expandable: true }));
                this.setTargetWithElements();
            }
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.elementSegment.value = plusButton.value;
            this.elementSegment.html = plusButton.html;
            this.$scope.panel.refresh();
        }
    }, {
        key: "segmentValueChanged",
        value: function segmentValueChanged(segment, index) {
            if (segment.value == "-REMOVE-") {
                this.segments.splice(index, 1);
            } else {
                this.segments[index] = segment;
            }
            this.setTargetWithElements();
        }
    }, {
        key: "setTargetWithElements",
        value: function setTargetWithElements() {
            var functions = '';
            var ctrl = this;
            _.each(ctrl.functions, function (element, index, list) {
                if (element.value == 'QUERY') functions += ctrl.segments.map(function (a) {
                    return a.value;
                }).join(';');else functions += element.value;
            });
            ctrl.$scope.target.target = functions != "" ? functions : ctrl.segments.map(function (a) {
                return a.value;
            }).join(';');
            ctrl.$scope.target.functionSegments = ctrl.functionSegments;
            ctrl.$scope.target.segments = ctrl.segments;
            ctrl.$scope.target.queryType = 'Element List';
            this.$scope.panel.refresh();
        }
    }, {
        key: "getFunctionsToAddNew",
        value: function getFunctionsToAddNew() {
            var _this = this;

            var ctrl = this;
            var array = [];
            _.each(Object.keys(openHistorianConstants_1.FunctionList), function (element, index, list) {
                array.push(ctrl.uiSegmentSrv.newSegment(element));
            });
            if (this.functions.length == 0) array = array.slice(2, array.length);
            array.sort(function (a, b) {
                var nameA = a.value.toUpperCase();
                var nameB = b.value.toUpperCase();
                if (nameA < nameB) {
                    return -1;
                }
                if (nameA > nameB) {
                    return 1;
                }
                return 0;
            });
            return Promise.resolve(_.filter(array, function (segment) {
                return _.find(_this.functions, function (x) {
                    return x.value == segment.value;
                }) == undefined;
            }));
        }
    }, {
        key: "getFunctionsToEdit",
        value: function getFunctionsToEdit(func, index) {
            var ctrl = this;
            var remove = [this.uiSegmentSrv.newSegment('-REMOVE-')];
            if (func.type == 'Operator') return Promise.resolve();else if (func.value == 'Set') return Promise.resolve(remove);
            return Promise.resolve(remove);
        }
    }, {
        key: "functionValueChanged",
        value: function functionValueChanged(func, index) {
            var funcSeg = openHistorianConstants_1.FunctionList[func.Function];
            if (func.value == "-REMOVE-") {
                var l = 1;
                var fi = _.findIndex(this.functionSegments, function (segment) {
                    return segment.Function == func.Function;
                });
                if (func.Function == 'Slice') this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                    --fi;
                    ++l;
                }
                this.functionSegments.splice(fi, l);
            } else if (func.Type != 'Function') {
                var fi = _.findIndex(this.functionSegments, function (segment) {
                    return segment.Function == func.Function;
                });
                this.functionSegments[fi].Parameters[func.Index].Default = func.value;
            }
            this.buildFunctionArray();
            this.setTargetWithElements();
        }
    }, {
        key: "addFunctionSegment",
        value: function addFunctionSegment() {
            var func = openHistorianConstants_1.FunctionList[event.target['text']];
            if (func.Function == 'Slice') {
                this.functionSegments[0].Parameters.unshift(func.Parameters[0]);
            }
            this.functionSegments.unshift(JSON.parse(JSON.stringify(func)));
            this.buildFunctionArray();
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.functionSegment.value = plusButton.value;
            this.functionSegment.html = plusButton.html;
            this.setTargetWithElements();
        }
    }, {
        key: "buildFunctionArray",
        value: function buildFunctionArray() {
            var ctrl = this;
            ctrl.functions = [];
            if (this.functionSegments.length == 0) return;
            _.each(ctrl.functionSegments, function (element, index, list) {
                var newElement = ctrl.uiSegmentSrv.newSegment(element.Function);
                newElement.Type = 'Function';
                newElement.Function = element.Function;
                ctrl.functions.push(newElement);
                if (newElement.value == 'Set' || newElement.value == 'Slice') return;
                var operator = ctrl.uiSegmentSrv.newOperator('(');
                operator.Type = 'Operator';
                ctrl.functions.push(operator);
                _.each(element.Parameters, function (param, i, j) {
                    var d = ctrl.uiSegmentSrv.newFake(param.Default.toString());
                    d.Type = param.Type;
                    d.Function = element.Function;
                    d.Description = param.Description;
                    d.Index = i;
                    ctrl.functions.push(d);
                    var operator = ctrl.uiSegmentSrv.newOperator(',');
                    operator.Type = 'Operator';
                    ctrl.functions.push(operator);
                });
            });
            var query = ctrl.uiSegmentSrv.newCondition('QUERY');
            query.Type = 'Query';
            ctrl.functions.push(query);
            for (var i in ctrl.functionSegments) {
                if (ctrl.functionSegments[i].Function != 'Set' && ctrl.functionSegments[i].Function != 'Slice') {
                    var operator = ctrl.uiSegmentSrv.newOperator(')');
                    operator.Type = 'Operator';
                    ctrl.functions.push(operator);
                }
            }
        }
    }, {
        key: "getBooleans",
        value: function getBooleans() {
            var _this2 = this;

            return Promise.resolve(openHistorianConstants_1.Booleans.map(function (value) {
                return _this2.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "getAngleUnits",
        value: function getAngleUnits() {
            var _this3 = this;

            return Promise.resolve(openHistorianConstants_1.AngleUnits.map(function (value) {
                return _this3.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "getTimeSelect",
        value: function getTimeSelect() {
            var _this4 = this;

            return Promise.resolve(openHistorianConstants_1.TimeUnits.map(function (value) {
                return _this4.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "inputChange",
        value: function inputChange(func, index) {
            var ctrl = this;
            clearTimeout(this.typingTimer);
            this.typingTimer = global.setTimeout(function () {
                ctrl.functionValueChanged(func, index);
            }, 3000);
            event.target['focus']();
        }
    }]);

    return OpenHistorianElementPickerCtrl;
}();

exports.default = OpenHistorianElementPickerCtrl;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(2)))

/***/ }),
/* 17 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";


var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });

var OpenHistorianTextEditorCtrl = function () {
    function OpenHistorianTextEditorCtrl($scope, templateSrv) {
        _classCallCheck(this, OpenHistorianTextEditorCtrl);

        this.$scope = $scope;
        this.templateSrv = templateSrv;
        this.$scope = $scope;
        this.targetText = $scope.target.targetText == undefined ? '' : $scope.target.targetText;
        this.setTargetWithText();
        delete $scope.target.segments;
        delete $scope.target.functionSegments;
        delete $scope.target.wheres;
        delete $scope.target.topNSegment;
        delete $scope.target.functions;
        delete $scope.target.orderBys;
        delete $scope.target.whereSegment;
        delete $scope.target.filterSegment;
        delete $scope.target.orderBySegment;
        delete $scope.target.functionSegment;
    }

    _createClass(OpenHistorianTextEditorCtrl, [{
        key: "setTargetWithText",
        value: function setTargetWithText() {
            this.$scope.target.targetText = this.targetText;
            this.$scope.target.target = this.targetText;
            this.$scope.target.queryType = 'Text Editor';
            this.$scope.panel.refresh();
        }
    }]);

    return OpenHistorianTextEditorCtrl;
}();

exports.default = OpenHistorianTextEditorCtrl;

/***/ }),
/* 18 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(0);

var OpenHistorianFilterExpressionCtrl = function () {
    function OpenHistorianFilterExpressionCtrl($scope, $injector, uiSegmentSrv) {
        _classCallCheck(this, OpenHistorianFilterExpressionCtrl);

        this.uiSegmentSrv = uiSegmentSrv;
        this.uiSegmentSrv = uiSegmentSrv;
        this.$scope = $scope;
        this.target = $scope.target;
        this.datasource = $scope.datasource;
        this.panelCtrl = $scope.panel;
        var ctrl = this;
        this.wheres = this.target.wheres == undefined ? [] : this.target.wheres.map(function (a) {
            if (a.type == 'operator') return ctrl.uiSegmentSrv.newOperator(a.text);else if (a.type == 'condition') return ctrl.uiSegmentSrv.newCondition(a.text);else return ctrl.uiSegmentSrv.newSegment(a.value);
        });
        this.functionSegments = this.target.functionSegments == undefined ? [] : this.target.functionSegments;
        this.topNSegment = this.target.topNSegment == undefined ? null : this.target.topNSegment;
        this.functions = [];
        this.orderBys = this.target.orderBys == undefined ? [] : this.target.orderBys.map(function (a) {
            if (a.type == 'condition') return ctrl.uiSegmentSrv.newCondition(a.value);else return ctrl.uiSegmentSrv.newSegment(a.value);
        });
        this.whereSegment = this.uiSegmentSrv.newPlusButton();
        this.filterSegment = this.target.filterSegment == undefined ? this.uiSegmentSrv.newSegment('ActiveMeasurements') : this.uiSegmentSrv.newSegment(this.target.filterSegment.value);
        this.orderBySegment = this.uiSegmentSrv.newPlusButton();
        this.functionSegment = this.uiSegmentSrv.newPlusButton();
        this.typingTimer;
        delete $scope.target.segments;
        delete $scope.target.targetText;
        this.setTargetWithQuery();
    }

    _createClass(OpenHistorianFilterExpressionCtrl, [{
        key: "setTargetWithQuery",
        value: function setTargetWithQuery() {
            if (this.wheres.length == 0) {
                this.target.target = '';
                this.panelCtrl.refresh();
                return;
            }
            var filter = this.filterSegment.value + ' ';
            var topn = this.topNSegment ? 'TOP ' + this.topNSegment + ' ' : '';
            var where = 'WHERE ';
            _.each(this.wheres, function (element, index, list) {
                where += element.value + ' ';
            });
            var orderby = '';
            _.each(this.orderBys, function (element, index, list) {
                orderby += (index == 0 ? 'ORDER BY ' : '') + element.value + (element.type == 'condition' && index < list.length - 1 ? ',' : '') + ' ';
            });
            var query = 'FILTER ' + topn + filter + where + orderby;
            var functions = '';
            _.each(this.functions, function (element, index, list) {
                if (element.value == 'QUERY') functions += query;else functions += element.value;
            });
            this.target.target = functions != "" ? functions : query;
            this.target.topNSegment = this.topNSegment;
            this.target.filterSegment = this.filterSegment;
            this.target.orderBys = this.orderBys;
            this.target.wheres = this.wheres;
            this.target.functionSegments = this.functionSegments;
            this.target.queryType = 'Filter Expression';
            this.panelCtrl.refresh();
        }
    }, {
        key: "topNValueChanged",
        value: function topNValueChanged() {
            var ctrl = this;
            clearTimeout(ctrl.typingTimer);
            ctrl.typingTimer = global.setTimeout(function () {
                ctrl.setTargetWithQuery();
            }, 1000);
            event.target['focus']();
        }
    }, {
        key: "getWheresToEdit",
        value: function getWheresToEdit(where, index) {
            var _this = this;

            if (where.type === 'operator') {
                return Promise.resolve(this.uiSegmentSrv.newOperators(openHistorianConstants_1.WhereOperators));
            } else if (where.type === 'value') {
                return Promise.resolve(null);
            } else if (where.type === 'condition') {
                return Promise.resolve([this.uiSegmentSrv.newCondition('AND'), this.uiSegmentSrv.newCondition('OR')]);
            } else {
                return this.datasource.whereFindQuery(this.filterSegment.value).then(function (data) {
                    var altSegments = _.map(data, function (item) {
                        return _this.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                    });
                    altSegments.sort(function (a, b) {
                        if (a.value < b.value) return -1;
                        if (a.value > b.value) return 1;
                        return 0;
                    });
                    altSegments.unshift(_this.uiSegmentSrv.newSegment('-REMOVE-'));
                    return altSegments;
                });
            }
        }
    }, {
        key: "whereValueChanged",
        value: function whereValueChanged(where, index) {
            if (where.value == "-REMOVE-") {
                if (index == 0 && this.wheres.length > 3 && this.wheres[index + 3].type == 'condition') this.wheres.splice(index, 4);else if (index > 0 && this.wheres[index - 1].type == 'condition') this.wheres.splice(index - 1, 4);else this.wheres.splice(index, 3);
            }
            if (where.type == 'operator') this.wheres[index] = this.uiSegmentSrv.newOperator(where.value);else if (where.type == 'condition') this.wheres[index] = this.uiSegmentSrv.newCondition(where.value);else if (where.type == 'value' && !$.isNumeric(where.value) && where.value.toUpperCase() != 'NULL') this.wheres[index] = this.uiSegmentSrv.newSegment("'" + where.value + "'");
            this.setTargetWithQuery();
        }
    }, {
        key: "getWheresToAddNew",
        value: function getWheresToAddNew() {
            var ctrl = this;
            return this.datasource.whereFindQuery(ctrl.filterSegment.value).then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value) return -1;
                    if (a.value > b.value) return 1;
                    return 0;
                });
                return altSegments;
            });
        }
    }, {
        key: "addWhere",
        value: function addWhere() {
            if (this.wheres.length > 0) this.wheres.push(this.uiSegmentSrv.newCondition('AND'));
            this.wheres.push(this.uiSegmentSrv.newSegment(event.target['text']));
            this.wheres.push(this.uiSegmentSrv.newOperator('NOT LIKE'));
            this.wheres.push(this.uiSegmentSrv.newFake("''", 'value', 'query-segment-value'));
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.whereSegment.value = plusButton.value;
            this.whereSegment.html = plusButton.html;
            this.setTargetWithQuery();
        }
    }, {
        key: "getFilterToEdit",
        value: function getFilterToEdit() {
            var ctrl = this;
            return this.datasource.filterFindQuery().then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value) return -1;
                    if (a.value > b.value) return 1;
                    return 0;
                });
                return altSegments;
            });
        }
    }, {
        key: "filterValueChanged",
        value: function filterValueChanged() {
            this.orderBySegment = this.uiSegmentSrv.newPlusButton();
            this.wheres = [];
            this.setTargetWithQuery();
            this.panelCtrl.refresh();
        }
    }, {
        key: "getOrderBysToAddNew",
        value: function getOrderBysToAddNew() {
            var ctrl = this;
            return this.datasource.orderByFindQuery(ctrl.filterSegment.value).then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value) return -1;
                    if (a.value > b.value) return 1;
                    return 0;
                });
                return _.filter(altSegments, function (segment) {
                    return _.find(ctrl.orderBys, function (x) {
                        return x.value == segment.value;
                    }) == undefined;
                });
            });
        }
    }, {
        key: "getOrderBysToEdit",
        value: function getOrderBysToEdit(orderBy) {
            var _this2 = this;

            var ctrl = this;
            if (orderBy.type == 'condition') return Promise.resolve([this.uiSegmentSrv.newCondition('ASC'), this.uiSegmentSrv.newCondition('DESC')]);
            if (orderBy.type == 'condition') return Promise.resolve([this.uiSegmentSrv.newCondition('ASC'), this.uiSegmentSrv.newCondition('DESC')]);
            return this.datasource.orderByFindQuery(ctrl.filterSegment.value).then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value) return -1;
                    if (a.value > b.value) return 1;
                    return 0;
                });
                if (orderBy.type !== 'plus-button') altSegments.unshift(_this2.uiSegmentSrv.newSegment('-REMOVE-'));
                return _.filter(altSegments, function (segment) {
                    return _.find(ctrl.orderBys, function (x) {
                        return x.value == segment.value;
                    }) == undefined;
                });
            });
        }
    }, {
        key: "addOrderBy",
        value: function addOrderBy() {
            this.orderBys.push(this.uiSegmentSrv.newSegment(event.target['text']));
            this.orderBys.push(this.uiSegmentSrv.newCondition('ASC'));
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.orderBySegment.value = plusButton.value;
            this.orderBySegment.html = plusButton.html;
            this.setTargetWithQuery();
        }
    }, {
        key: "orderByValueChanged",
        value: function orderByValueChanged(orderBy, index) {
            if (event.target['text'] == "-REMOVE-") this.orderBys.splice(index, 2);else {
                if (orderBy.type == 'condition') this.orderBys[index] = this.uiSegmentSrv.newCondition(event.target['text']);else this.orderBys[index] = this.uiSegmentSrv.newSegment(event.target['text']);
            }
            this.setTargetWithQuery();
        }
    }, {
        key: "getFunctionsToAddNew",
        value: function getFunctionsToAddNew() {
            var _this3 = this;

            var ctrl = this;
            var array = [];
            _.each(Object.keys(openHistorianConstants_1.FunctionList), function (element, index, list) {
                array.push(ctrl.uiSegmentSrv.newSegment(element));
            });
            if (this.functions.length == 0) array = array.slice(2, array.length);
            array.sort(function (a, b) {
                var nameA = a.value.toUpperCase();
                var nameB = b.value.toUpperCase();
                if (nameA < nameB) {
                    return -1;
                }
                if (nameA > nameB) {
                    return 1;
                }
                return 0;
            });
            return Promise.resolve(_.filter(array, function (segment) {
                return _.find(_this3.functions, function (x) {
                    return x.value == segment.value;
                }) == undefined;
            }));
        }
    }, {
        key: "getFunctionsToEdit",
        value: function getFunctionsToEdit(func, index) {
            var ctrl = this;
            var remove = [this.uiSegmentSrv.newSegment('-REMOVE-')];
            if (func.type == 'Operator') return Promise.resolve();else if (func.value == 'Set') return Promise.resolve(remove);
            return Promise.resolve(remove);
        }
    }, {
        key: "functionValueChanged",
        value: function functionValueChanged(func, index) {
            var funcSeg = openHistorianConstants_1.FunctionList[func.Function];
            if (func.value == "-REMOVE-") {
                var l = 1;
                var fi = _.findIndex(this.functionSegments, function (segment) {
                    return segment.Function == func.Function;
                });
                if (func.Function == 'Slice') this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                    --fi;
                    ++l;
                }
                this.functionSegments.splice(fi, l);
            } else if (func.Type != 'Function') {
                var fi = _.findIndex(this.functionSegments, function (segment) {
                    return segment.Function == func.Function;
                });
                this.functionSegments[fi].Parameters[func.Index].Default = func.value;
            }
            this.buildFunctionArray();
            this.setTargetWithQuery();
        }
    }, {
        key: "addFunctionSegment",
        value: function addFunctionSegment() {
            var func = openHistorianConstants_1.FunctionList[event.target['text']];
            if (func.Function == 'Slice') {
                this.functionSegments[0].Parameters.unshift(func.Parameters[0]);
            }
            this.functionSegments.unshift(JSON.parse(JSON.stringify(func)));
            this.buildFunctionArray();
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.functionSegment.value = plusButton.value;
            this.functionSegment.html = plusButton.html;
            this.setTargetWithQuery();
        }
    }, {
        key: "buildFunctionArray",
        value: function buildFunctionArray() {
            var ctrl = this;
            ctrl.functions = [];
            if (this.functionSegments.length == 0) return;
            _.each(ctrl.functionSegments, function (element, index, list) {
                var newElement = ctrl.uiSegmentSrv.newSegment(element.Function);
                newElement.Type = 'Function';
                newElement.Function = element.Function;
                ctrl.functions.push(newElement);
                if (newElement.value == 'Set' || newElement.value == 'Slice') return;
                var operator = ctrl.uiSegmentSrv.newOperator('(');
                operator.Type = 'Operator';
                ctrl.functions.push(operator);
                _.each(element.Parameters, function (param, i, j) {
                    var d = ctrl.uiSegmentSrv.newFake(param.Default.toString());
                    d.Type = param.Type;
                    d.Function = element.Function;
                    d.Description = param.Description;
                    d.Index = i;
                    ctrl.functions.push(d);
                    var operator = ctrl.uiSegmentSrv.newOperator(',');
                    operator.Type = 'Operator';
                    ctrl.functions.push(operator);
                });
            });
            var query = ctrl.uiSegmentSrv.newCondition('QUERY');
            query.Type = 'Query';
            ctrl.functions.push(query);
            for (var i in ctrl.functionSegments) {
                if (ctrl.functionSegments[i].Function != 'Set' && ctrl.functionSegments[i].Function != 'Slice') {
                    var operator = ctrl.uiSegmentSrv.newOperator(')');
                    operator.Type = 'Operator';
                    ctrl.functions.push(operator);
                }
            }
        }
    }, {
        key: "getBooleans",
        value: function getBooleans() {
            var _this4 = this;

            return Promise.resolve(openHistorianConstants_1.Booleans.map(function (value) {
                return _this4.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "getAngleUnits",
        value: function getAngleUnits() {
            var _this5 = this;

            return Promise.resolve(openHistorianConstants_1.AngleUnits.map(function (value) {
                return _this5.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "getTimeSelect",
        value: function getTimeSelect() {
            var _this6 = this;

            return Promise.resolve(openHistorianConstants_1.TimeUnits.map(function (value) {
                return _this6.uiSegmentSrv.newSegment(value);
            }));
        }
    }, {
        key: "inputChange",
        value: function inputChange(func, index) {
            var ctrl = this;
            clearTimeout(this.typingTimer);
            this.typingTimer = global.setTimeout(function () {
                ctrl.functionValueChanged(func, index);
            }, 3000);
            event.target['focus']();
        }
    }]);

    return OpenHistorianFilterExpressionCtrl;
}();

exports.default = OpenHistorianFilterExpressionCtrl;
/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(2)))

/***/ })
/******/ ])));