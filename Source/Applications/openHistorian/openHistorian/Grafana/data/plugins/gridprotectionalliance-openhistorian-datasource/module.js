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
/******/ 	return __webpack_require__(__webpack_require__.s = "./module.ts");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./controllers/openHistorianAnnotations_ctrl.ts":
/*!******************************************************!*\
  !*** ./controllers/openHistorianAnnotations_ctrl.ts ***!
  \******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var OpenHistorianAnnotationsQueryCtrl = (function () {
    function OpenHistorianAnnotationsQueryCtrl() {
    }
    OpenHistorianAnnotationsQueryCtrl.templateUrl = 'partial/annotations.editor.html';
    return OpenHistorianAnnotationsQueryCtrl;
}());
exports.default = OpenHistorianAnnotationsQueryCtrl;


/***/ }),

/***/ "./controllers/openHistorianConfig_ctrl.ts":
/*!*************************************************!*\
  !*** ./controllers/openHistorianConfig_ctrl.ts ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var OpenHistorianConfigCtrl = (function () {
    function OpenHistorianConfigCtrl($scope) {
        var ctrl = this;
        ctrl.current.jsonData.Excluded = this.current.jsonData.Excluded || 0;
        ctrl.current.jsonData.Normal = this.current.jsonData.Normal || false;
        ctrl.current.jsonData.Alarms = this.current.jsonData.Alarms || false;
    }
    OpenHistorianConfigCtrl.templateUrl = 'partial/config.html';
    return OpenHistorianConfigCtrl;
}());
exports.default = OpenHistorianConfigCtrl;


/***/ }),

/***/ "./controllers/openHistorianElementPicker_ctrl.ts":
/*!********************************************************!*\
  !*** ./controllers/openHistorianElementPicker_ctrl.ts ***!
  \********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {
Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(/*! ./../js/openHistorianConstants */ "./js/openHistorianConstants.ts");
var OpenHistorianElementPickerCtrl = (function () {
    function OpenHistorianElementPickerCtrl($scope, uiSegmentSrv) {
        this.$scope = $scope;
        this.uiSegmentSrv = uiSegmentSrv;
        var ctrl = this;
        this.$scope = $scope;
        this.uiSegmentSrv = uiSegmentSrv;
        this.segments = (this.$scope.target.segments == undefined ? [] : this.$scope.target.segments.map(function (a) { return ctrl.uiSegmentSrv.newSegment({ value: a.text, expandable: true }); }));
        this.functionSegments = (this.$scope.target.functionSegments == undefined ? [] : this.$scope.target.functionSegments);
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
    OpenHistorianElementPickerCtrl.prototype.getElementSegments = function (newSegment) {
        var ctrl = this;
        var option = null;
        if (event.target['value'] != "")
            option = event.target['value'];
        return ctrl.$scope.datasource.metricFindQuery(option).then(function (data) {
            var altSegments = _.map(data, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
            });
            altSegments.sort(function (a, b) {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });
            _.each(ctrl.$scope.datasource.templateSrv.variables, function (item, index, list) {
                if (item.type == "query")
                    altSegments.unshift(ctrl.uiSegmentSrv.newCondition('$' + item.name));
            });
            if (!newSegment)
                altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));
            return _.filter(altSegments, function (segment) {
                return _.find(ctrl.segments, function (x) {
                    return x.value == segment.value;
                }) == undefined;
            });
        });
    };
    OpenHistorianElementPickerCtrl.prototype.addElementSegment = function () {
        if (event.target['text'] != null) {
            this.segments.push(this.uiSegmentSrv.newSegment({ value: event.target['text'], expandable: true }));
            this.setTargetWithElements();
        }
        var plusButton = this.uiSegmentSrv.newPlusButton();
        this.elementSegment.value = plusButton.value;
        this.elementSegment.html = plusButton.html;
        this.$scope.panel.refresh();
    };
    OpenHistorianElementPickerCtrl.prototype.segmentValueChanged = function (segment, index) {
        if (segment.value == "-REMOVE-") {
            this.segments.splice(index, 1);
        }
        else {
            this.segments[index] = segment;
        }
        this.setTargetWithElements();
    };
    OpenHistorianElementPickerCtrl.prototype.setTargetWithElements = function () {
        var functions = '';
        var ctrl = this;
        _.each(ctrl.functions, function (element, index, list) {
            if (element.value == 'QUERY')
                functions += ctrl.segments.map(function (a) { return a.value; }).join(';');
            else
                functions += element.value;
        });
        ctrl.$scope.target.target = (functions != "" ? functions : ctrl.segments.map(function (a) {
            return a.value;
        }).join(';'));
        ctrl.$scope.target.functionSegments = ctrl.functionSegments;
        ctrl.$scope.target.segments = ctrl.segments;
        ctrl.$scope.target.queryType = 'Element List';
        this.$scope.panel.refresh();
    };
    OpenHistorianElementPickerCtrl.prototype.getFunctionsToAddNew = function () {
        var _this = this;
        var ctrl = this;
        var array = [];
        _.each(Object.keys(openHistorianConstants_1.FunctionList), function (element, index, list) {
            array.push(ctrl.uiSegmentSrv.newSegment(element));
        });
        if (this.functions.length == 0)
            array = array.slice(2, array.length);
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
    };
    OpenHistorianElementPickerCtrl.prototype.getFunctionsToEdit = function (func, index) {
        var ctrl = this;
        var remove = [this.uiSegmentSrv.newSegment('-REMOVE-')];
        if (func.type == 'Operator')
            return Promise.resolve();
        else if (func.value == 'Set')
            return Promise.resolve(remove);
        return Promise.resolve(remove);
    };
    OpenHistorianElementPickerCtrl.prototype.functionValueChanged = function (func, index) {
        var funcSeg = openHistorianConstants_1.FunctionList[func.Function];
        if (func.value == "-REMOVE-") {
            var l = 1;
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
            if (func.Function == 'Slice')
                this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);
            else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                --fi;
                ++l;
            }
            this.functionSegments.splice(fi, l);
        }
        else if (func.Type != 'Function') {
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
            this.functionSegments[fi].Parameters[func.Index].Default = func.value;
        }
        this.buildFunctionArray();
        this.setTargetWithElements();
    };
    OpenHistorianElementPickerCtrl.prototype.addFunctionSegment = function () {
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
    };
    OpenHistorianElementPickerCtrl.prototype.buildFunctionArray = function () {
        var ctrl = this;
        ctrl.functions = [];
        if (this.functionSegments.length == 0)
            return;
        _.each(ctrl.functionSegments, function (element, index, list) {
            var newElement = ctrl.uiSegmentSrv.newSegment(element.Function);
            newElement.Type = 'Function';
            newElement.Function = element.Function;
            ctrl.functions.push(newElement);
            if (newElement.value == 'Set' || newElement.value == 'Slice')
                return;
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
    };
    OpenHistorianElementPickerCtrl.prototype.getBooleans = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.Booleans.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianElementPickerCtrl.prototype.getAngleUnits = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.AngleUnits.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianElementPickerCtrl.prototype.getTimeSelect = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.TimeUnits.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianElementPickerCtrl.prototype.inputChange = function (func, index) {
        var ctrl = this;
        clearTimeout(this.typingTimer);
        this.typingTimer = global.setTimeout(function () { ctrl.functionValueChanged(func, index); }, 3000);
        event.target['focus']();
    };
    return OpenHistorianElementPickerCtrl;
}());
exports.default = OpenHistorianElementPickerCtrl;

/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(/*! ./../node_modules/webpack/buildin/global.js */ "./node_modules/webpack/buildin/global.js")))

/***/ }),

/***/ "./controllers/openHistorianFilterExpression_ctrl.ts":
/*!***********************************************************!*\
  !*** ./controllers/openHistorianFilterExpression_ctrl.ts ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
/* WEBPACK VAR INJECTION */(function(global) {
Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(/*! ./../js/openHistorianConstants */ "./js/openHistorianConstants.ts");
var OpenHistorianFilterExpressionCtrl = (function () {
    function OpenHistorianFilterExpressionCtrl($scope, $injector, uiSegmentSrv) {
        this.uiSegmentSrv = uiSegmentSrv;
        this.uiSegmentSrv = uiSegmentSrv;
        this.$scope = $scope;
        this.target = $scope.target;
        this.datasource = $scope.datasource;
        this.panelCtrl = $scope.panel;
        var ctrl = this;
        this.wheres = (this.target.wheres == undefined ? [] : this.target.wheres.map(function (a) {
            if (a.type == 'operator')
                return ctrl.uiSegmentSrv.newOperator(a.text);
            else if (a.type == 'condition')
                return ctrl.uiSegmentSrv.newCondition(a.text);
            else
                return ctrl.uiSegmentSrv.newSegment(a.value);
        }));
        this.functionSegments = (this.target.functionSegments == undefined ? [] : this.target.functionSegments);
        this.topNSegment = (this.target.topNSegment == undefined ? null : this.target.topNSegment);
        this.functions = [];
        this.orderBys = (this.target.orderBys == undefined ? [] : this.target.orderBys.map(function (a) {
            if (a.type == 'condition')
                return ctrl.uiSegmentSrv.newCondition(a.value);
            else
                return ctrl.uiSegmentSrv.newSegment(a.value);
        }));
        this.whereSegment = this.uiSegmentSrv.newPlusButton();
        this.filterSegment = (this.target.filterSegment == undefined ? this.uiSegmentSrv.newSegment('ActiveMeasurements') : this.uiSegmentSrv.newSegment(this.target.filterSegment.value));
        this.orderBySegment = this.uiSegmentSrv.newPlusButton();
        this.functionSegment = this.uiSegmentSrv.newPlusButton();
        this.typingTimer;
        delete $scope.target.segments;
        delete $scope.target.targetText;
        this.setTargetWithQuery();
    }
    OpenHistorianFilterExpressionCtrl.prototype.setTargetWithQuery = function () {
        if (this.wheres.length == 0) {
            this.target.target = '';
            this.panelCtrl.refresh();
            return;
        }
        var filter = this.filterSegment.value + ' ';
        var topn = (this.topNSegment ? 'TOP ' + this.topNSegment + ' ' : '');
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
            if (element.value == 'QUERY')
                functions += query;
            else
                functions += element.value;
        });
        this.target.target = (functions != "" ? functions : query);
        this.target.topNSegment = this.topNSegment;
        this.target.filterSegment = this.filterSegment;
        this.target.orderBys = this.orderBys;
        this.target.wheres = this.wheres;
        this.target.functionSegments = this.functionSegments;
        this.target.queryType = 'Filter Expression';
        this.panelCtrl.refresh();
    };
    OpenHistorianFilterExpressionCtrl.prototype.topNValueChanged = function () {
        var ctrl = this;
        clearTimeout(ctrl.typingTimer);
        ctrl.typingTimer = global.setTimeout(function () { ctrl.setTargetWithQuery(); }, 1000);
        event.target['focus']();
    };
    OpenHistorianFilterExpressionCtrl.prototype.getWheresToEdit = function (where, index) {
        var _this = this;
        if (where.type === 'operator') {
            return Promise.resolve(this.uiSegmentSrv.newOperators(openHistorianConstants_1.WhereOperators));
        }
        else if (where.type === 'value') {
            return Promise.resolve(null);
        }
        else if (where.type === 'condition') {
            return Promise.resolve([this.uiSegmentSrv.newCondition('AND'), this.uiSegmentSrv.newCondition('OR')]);
        }
        else {
            return this.datasource.whereFindQuery(this.filterSegment.value).then(function (data) {
                var altSegments = _.map(data, function (item) {
                    return _this.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                });
                altSegments.sort(function (a, b) {
                    if (a.value < b.value)
                        return -1;
                    if (a.value > b.value)
                        return 1;
                    return 0;
                });
                altSegments.unshift(_this.uiSegmentSrv.newSegment('-REMOVE-'));
                return altSegments;
            });
        }
    };
    OpenHistorianFilterExpressionCtrl.prototype.whereValueChanged = function (where, index) {
        if (where.value == "-REMOVE-") {
            if (index == 0 && this.wheres.length > 3 && this.wheres[index + 3].type == 'condition')
                this.wheres.splice(index, 4);
            else if (index > 0 && this.wheres[index - 1].type == 'condition')
                this.wheres.splice(index - 1, 4);
            else
                this.wheres.splice(index, 3);
        }
        if (where.type == 'operator')
            this.wheres[index] = this.uiSegmentSrv.newOperator(where.value);
        else if (where.type == 'condition')
            this.wheres[index] = this.uiSegmentSrv.newCondition(where.value);
        else if (where.type == 'value' && !$.isNumeric(where.value) && where.value.toUpperCase() != 'NULL')
            this.wheres[index] = this.uiSegmentSrv.newSegment("'" + where.value + "'");
        this.setTargetWithQuery();
    };
    OpenHistorianFilterExpressionCtrl.prototype.getWheresToAddNew = function () {
        var ctrl = this;
        return this.datasource.whereFindQuery(ctrl.filterSegment.value).then(function (data) {
            var altSegments = _.map(data, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
            });
            altSegments.sort(function (a, b) {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });
            return altSegments;
        });
    };
    OpenHistorianFilterExpressionCtrl.prototype.addWhere = function () {
        if (this.wheres.length > 0)
            this.wheres.push(this.uiSegmentSrv.newCondition('AND'));
        this.wheres.push(this.uiSegmentSrv.newSegment(event.target['text']));
        this.wheres.push(this.uiSegmentSrv.newOperator('NOT LIKE'));
        this.wheres.push(this.uiSegmentSrv.newFake("''", 'value', 'query-segment-value'));
        var plusButton = this.uiSegmentSrv.newPlusButton();
        this.whereSegment.value = plusButton.value;
        this.whereSegment.html = plusButton.html;
        this.setTargetWithQuery();
    };
    OpenHistorianFilterExpressionCtrl.prototype.getFilterToEdit = function () {
        var ctrl = this;
        return this.datasource.filterFindQuery().then(function (data) {
            var altSegments = _.map(data, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
            });
            altSegments.sort(function (a, b) {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });
            return altSegments;
        });
    };
    OpenHistorianFilterExpressionCtrl.prototype.filterValueChanged = function () {
        this.orderBySegment = this.uiSegmentSrv.newPlusButton();
        this.wheres = [];
        this.setTargetWithQuery();
        this.panelCtrl.refresh();
    };
    OpenHistorianFilterExpressionCtrl.prototype.getOrderBysToAddNew = function () {
        var ctrl = this;
        return this.datasource.orderByFindQuery(ctrl.filterSegment.value).then(function (data) {
            var altSegments = _.map(data, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
            });
            altSegments.sort(function (a, b) {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });
            return _.filter(altSegments, function (segment) {
                return _.find(ctrl.orderBys, function (x) {
                    return x.value == segment.value;
                }) == undefined;
            });
        });
    };
    OpenHistorianFilterExpressionCtrl.prototype.getOrderBysToEdit = function (orderBy) {
        var _this = this;
        var ctrl = this;
        if (orderBy.type == 'condition')
            return Promise.resolve([this.uiSegmentSrv.newCondition('ASC'), this.uiSegmentSrv.newCondition('DESC')]);
        if (orderBy.type == 'condition')
            return Promise.resolve([this.uiSegmentSrv.newCondition('ASC'), this.uiSegmentSrv.newCondition('DESC')]);
        return this.datasource.orderByFindQuery(ctrl.filterSegment.value).then(function (data) {
            var altSegments = _.map(data, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
            });
            altSegments.sort(function (a, b) {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });
            if (orderBy.type !== 'plus-button')
                altSegments.unshift(_this.uiSegmentSrv.newSegment('-REMOVE-'));
            return _.filter(altSegments, function (segment) {
                return _.find(ctrl.orderBys, function (x) {
                    return x.value == segment.value;
                }) == undefined;
            });
        });
    };
    OpenHistorianFilterExpressionCtrl.prototype.addOrderBy = function () {
        this.orderBys.push(this.uiSegmentSrv.newSegment(event.target['text']));
        this.orderBys.push(this.uiSegmentSrv.newCondition('ASC'));
        var plusButton = this.uiSegmentSrv.newPlusButton();
        this.orderBySegment.value = plusButton.value;
        this.orderBySegment.html = plusButton.html;
        this.setTargetWithQuery();
    };
    OpenHistorianFilterExpressionCtrl.prototype.orderByValueChanged = function (orderBy, index) {
        if (event.target['text'] == "-REMOVE-")
            this.orderBys.splice(index, 2);
        else {
            if (orderBy.type == 'condition')
                this.orderBys[index] = this.uiSegmentSrv.newCondition(event.target['text']);
            else
                this.orderBys[index] = this.uiSegmentSrv.newSegment(event.target['text']);
        }
        this.setTargetWithQuery();
    };
    OpenHistorianFilterExpressionCtrl.prototype.getFunctionsToAddNew = function () {
        var _this = this;
        var ctrl = this;
        var array = [];
        _.each(Object.keys(openHistorianConstants_1.FunctionList), function (element, index, list) {
            array.push(ctrl.uiSegmentSrv.newSegment(element));
        });
        if (this.functions.length == 0)
            array = array.slice(2, array.length);
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
    };
    OpenHistorianFilterExpressionCtrl.prototype.getFunctionsToEdit = function (func, index) {
        var ctrl = this;
        var remove = [this.uiSegmentSrv.newSegment('-REMOVE-')];
        if (func.type == 'Operator')
            return Promise.resolve();
        else if (func.value == 'Set')
            return Promise.resolve(remove);
        return Promise.resolve(remove);
    };
    OpenHistorianFilterExpressionCtrl.prototype.functionValueChanged = function (func, index) {
        var funcSeg = openHistorianConstants_1.FunctionList[func.Function];
        if (func.value == "-REMOVE-") {
            var l = 1;
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
            if (func.Function == 'Slice')
                this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);
            else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                --fi;
                ++l;
            }
            this.functionSegments.splice(fi, l);
        }
        else if (func.Type != 'Function') {
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
            this.functionSegments[fi].Parameters[func.Index].Default = func.value;
        }
        this.buildFunctionArray();
        this.setTargetWithQuery();
    };
    OpenHistorianFilterExpressionCtrl.prototype.addFunctionSegment = function () {
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
    };
    OpenHistorianFilterExpressionCtrl.prototype.buildFunctionArray = function () {
        var ctrl = this;
        ctrl.functions = [];
        if (this.functionSegments.length == 0)
            return;
        _.each(ctrl.functionSegments, function (element, index, list) {
            var newElement = ctrl.uiSegmentSrv.newSegment(element.Function);
            newElement.Type = 'Function';
            newElement.Function = element.Function;
            ctrl.functions.push(newElement);
            if (newElement.value == 'Set' || newElement.value == 'Slice')
                return;
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
    };
    OpenHistorianFilterExpressionCtrl.prototype.getBooleans = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.Booleans.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianFilterExpressionCtrl.prototype.getAngleUnits = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.AngleUnits.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianFilterExpressionCtrl.prototype.getTimeSelect = function () {
        var _this = this;
        return Promise.resolve(openHistorianConstants_1.TimeUnits.map(function (value) { return _this.uiSegmentSrv.newSegment(value); }));
    };
    OpenHistorianFilterExpressionCtrl.prototype.inputChange = function (func, index) {
        var ctrl = this;
        clearTimeout(this.typingTimer);
        this.typingTimer = global.setTimeout(function () { ctrl.functionValueChanged(func, index); }, 3000);
        event.target['focus']();
    };
    return OpenHistorianFilterExpressionCtrl;
}());
exports.default = OpenHistorianFilterExpressionCtrl;

/* WEBPACK VAR INJECTION */}.call(this, __webpack_require__(/*! ./../node_modules/webpack/buildin/global.js */ "./node_modules/webpack/buildin/global.js")))

/***/ }),

/***/ "./controllers/openHistorianQueryOptions_ctrl.ts":
/*!*******************************************************!*\
  !*** ./controllers/openHistorianQueryOptions_ctrl.ts ***!
  \*******************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianConstants_1 = __webpack_require__(/*! ./../js/openHistorianConstants */ "./js/openHistorianConstants.ts");
var OpenHistorianQueryOptionsCtrl = (function () {
    function OpenHistorianQueryOptionsCtrl($scope, $compile) {
        var _this = this;
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
    OpenHistorianQueryOptionsCtrl.prototype.calculateFlags = function (flag) {
        var ctrl = this;
        var flagVarExcluded = ctrl.return.Excluded;
        if (flag == 'Select All') {
            _.each(Object.keys(ctrl.dataFlags), function (key, index, list) {
                if (key == "Normal")
                    ctrl.dataFlags[key].Value = false;
                else
                    ctrl.dataFlags[key].Value = ctrl.dataFlags['Select All'].Value;
            });
            if (ctrl.dataFlags['Select All'].Value)
                flagVarExcluded = 0xFFFFFFFF;
            else
                flagVarExcluded = 0;
        }
        else {
            ctrl.dataFlags['Select All'].Value = false;
            flagVarExcluded ^= ctrl.dataFlags[flag].Flag;
        }
        ctrl.return.Excluded = flagVarExcluded;
        ctrl.return.Normal = ctrl.dataFlags['Normal'].Value;
    };
    OpenHistorianQueryOptionsCtrl.prototype.changeAlarms = function () {
        var ctrl = this;
        ctrl.return.Alarms = ctrl.includeAlarm;
    };
    OpenHistorianQueryOptionsCtrl.prototype.hex2flags = function (hex) {
        var ctrl = this;
        var flag = hex;
        var flags = JSON.parse(JSON.stringify(openHistorianConstants_1.DefaultFlags));
        _.each(Object.keys(flags), function (key, index, list) {
            if (key == 'Select All')
                return;
            flags[key].Value = (flags[key].Flag & flag) != 0;
        });
        return flags;
    };
    return OpenHistorianQueryOptionsCtrl;
}());
exports.default = OpenHistorianQueryOptionsCtrl;


/***/ }),

/***/ "./controllers/openHistorianQuery_ctrl.ts":
/*!************************************************!*\
  !*** ./controllers/openHistorianQuery_ctrl.ts ***!
  \************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var sdk_1 = __webpack_require__(/*! ../node_modules/grafana-sdk-mocks/app/plugins/sdk */ "./node_modules/grafana-sdk-mocks/app/plugins/sdk.ts");
__webpack_require__(/*! ./../css/query-editor.css */ "./css/query-editor.css");
var OpenHistorianDataSourceQueryCtrl = (function (_super) {
    __extends(OpenHistorianDataSourceQueryCtrl, _super);
    function OpenHistorianDataSourceQueryCtrl($scope, $injector, uiSegmentSrv, templateSrv, $compile) {
        var _this = _super.call(this, $scope, $injector) || this;
        _this.uiSegmentSrv = uiSegmentSrv;
        _this.templateSrv = templateSrv;
        _this.$compile = $compile;
        _this.$scope = $scope;
        _this.$compile = $compile;
        var ctrl = _this;
        _this.uiSegmentSrv = uiSegmentSrv;
        _this.queryTypes = [
            "Element List", "Filter Expression", "Text Editor"
        ];
        _this.queryType = (_this.target.queryType == undefined ? "Element List" : _this.target.queryType);
        _this.queryOptionsOpen = false;
        if (ctrl.target.queryOptions == undefined)
            ctrl.target.queryOptions = { Excluded: ctrl.datasource.options.excludedDataFlags, Normal: ctrl.datasource.options.excludeNormalData };
        return _this;
    }
    OpenHistorianDataSourceQueryCtrl.prototype.toggleQueryOptions = function () {
        this.queryOptionsOpen = !this.queryOptionsOpen;
    };
    OpenHistorianDataSourceQueryCtrl.prototype.onChangeInternal = function () {
        this.panelCtrl.refresh();
    };
    OpenHistorianDataSourceQueryCtrl.prototype.changeQueryType = function () {
        if (this.queryType == 'Text Editor') {
            this.target.targetText = this.target.target;
        }
        else {
            this.target.target = '';
            delete this.target.functionSegments;
        }
    };
    OpenHistorianDataSourceQueryCtrl.templateUrl = 'partial/query.editor.html';
    return OpenHistorianDataSourceQueryCtrl;
}(sdk_1.QueryCtrl));
exports.default = OpenHistorianDataSourceQueryCtrl;


/***/ }),

/***/ "./controllers/openHistorianTextEditor_ctrl.ts":
/*!*****************************************************!*\
  !*** ./controllers/openHistorianTextEditor_ctrl.ts ***!
  \*****************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var OpenHistorianTextEditorCtrl = (function () {
    function OpenHistorianTextEditorCtrl($scope, templateSrv) {
        this.$scope = $scope;
        this.templateSrv = templateSrv;
        this.$scope = $scope;
        this.targetText = ($scope.target.targetText == undefined ? '' : $scope.target.targetText);
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
    OpenHistorianTextEditorCtrl.prototype.setTargetWithText = function () {
        this.$scope.target.targetText = this.targetText;
        this.$scope.target.target = this.targetText;
        this.$scope.target.queryType = 'Text Editor';
        this.$scope.panel.refresh();
    };
    return OpenHistorianTextEditorCtrl;
}());
exports.default = OpenHistorianTextEditorCtrl;


/***/ }),

/***/ "./css/query-editor.css":
/*!******************************!*\
  !*** ./css/query-editor.css ***!
  \******************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

var content = __webpack_require__(/*! !../node_modules/css-loader/dist/cjs.js!./query-editor.css */ "./node_modules/css-loader/dist/cjs.js!./css/query-editor.css");

if (typeof content === 'string') {
  content = [[module.i, content, '']];
}

var options = {}

options.insert = "head";
options.singleton = false;

var update = __webpack_require__(/*! ../node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js */ "./node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js")(content, options);

if (content.locals) {
  module.exports = content.locals;
}


/***/ }),

/***/ "./js/openHistorianConstants.ts":
/*!**************************************!*\
  !*** ./js/openHistorianConstants.ts ***!
  \**************************************/
/*! no static exports found */
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
    Label: { Function: 'Label', Parameters: [{ Default: 'Name', Type: 'string', Description: 'Renames a series with the specified label value.' }] },
};
exports.WhereOperators = ['=', '<>', '<', '<=', '>', '>=', 'LIKE', 'NOT LIKE', 'IN', 'NOT IN', 'IS', 'IS NOT'];
exports.Booleans = ['true', 'false'];
exports.AngleUnits = ['Degrees', 'Radians', 'Grads', 'ArcMinutes', 'ArcSeconds', 'AngularMil'];
exports.TimeUnits = ['Weeks', 'Days', 'Hours', 'Minutes', 'Seconds', 'Milliseconds', 'Microseconds', 'Nanoseconds', 'Ticks', 'PlankTime', 'AtomicUnitsOfTime', 'Ke'];


/***/ }),

/***/ "./module.ts":
/*!*******************!*\
  !*** ./module.ts ***!
  \*******************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var openHistorianDatasource_1 = __webpack_require__(/*! ./openHistorianDatasource */ "./openHistorianDatasource.ts");
exports.Datasource = openHistorianDatasource_1.default;
var openHistorianQuery_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianQuery_ctrl */ "./controllers/openHistorianQuery_ctrl.ts");
exports.QueryCtrl = openHistorianQuery_ctrl_1.default;
var openHistorianConfig_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianConfig_ctrl */ "./controllers/openHistorianConfig_ctrl.ts");
exports.ConfigCtrl = openHistorianConfig_ctrl_1.default;
var openHistorianQueryOptions_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianQueryOptions_ctrl */ "./controllers/openHistorianQueryOptions_ctrl.ts");
exports.QueryOptionsCtrl = openHistorianQueryOptions_ctrl_1.default;
var openHistorianAnnotations_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianAnnotations_ctrl */ "./controllers/openHistorianAnnotations_ctrl.ts");
exports.AnnotationsQueryCtrl = openHistorianAnnotations_ctrl_1.default;
var openHistorianElementPicker_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianElementPicker_ctrl */ "./controllers/openHistorianElementPicker_ctrl.ts");
var openHistorianTextEditor_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianTextEditor_ctrl */ "./controllers/openHistorianTextEditor_ctrl.ts");
var openHistorianFilterExpression_ctrl_1 = __webpack_require__(/*! ./controllers/openHistorianFilterExpression_ctrl */ "./controllers/openHistorianFilterExpression_ctrl.ts");
angular.module('grafana.directives').directive("queryOptions", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.options.html',
        restrict: 'E',
        controller: openHistorianQueryOptions_ctrl_1.default,
        controllerAs: 'queryOptionCtrl',
        scope: {
            flags: "=",
            return: "=",
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

/***/ "./node_modules/css-loader/dist/cjs.js!./css/query-editor.css":
/*!********************************************************************!*\
  !*** ./node_modules/css-loader/dist/cjs.js!./css/query-editor.css ***!
  \********************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

exports = module.exports = __webpack_require__(/*! ../node_modules/css-loader/dist/runtime/api.js */ "./node_modules/css-loader/dist/runtime/api.js")(false);
// Module
exports.push([module.i, ".generic-datasource-query-row .query-keyword {\r\n  width: 75px;\r\n}", ""]);


/***/ }),

/***/ "./node_modules/css-loader/dist/runtime/api.js":
/*!*****************************************************!*\
  !*** ./node_modules/css-loader/dist/runtime/api.js ***!
  \*****************************************************/
/*! no static exports found */
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

/***/ "./node_modules/grafana-sdk-mocks/app/features/panel/metrics_panel_ctrl.ts":
/*!*********************************************************************************!*\
  !*** ./node_modules/grafana-sdk-mocks/app/features/panel/metrics_panel_ctrl.ts ***!
  \*********************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var panel_ctrl_1 = __webpack_require__(/*! ./panel_ctrl */ "./node_modules/grafana-sdk-mocks/app/features/panel/panel_ctrl.ts");
var MetricsPanelCtrl = (function (_super) {
    __extends(MetricsPanelCtrl, _super);
    function MetricsPanelCtrl($scope, $injector) {
        var _this = _super.call(this, $scope, $injector) || this;
        _this.editorTabIndex = 1;
        if (!_this.panel.targets) {
            _this.panel.targets = [{}];
        }
        return _this;
    }
    MetricsPanelCtrl.prototype.onPanelTearDown = function () {
    };
    MetricsPanelCtrl.prototype.onInitMetricsPanelEditMode = function () {
    };
    MetricsPanelCtrl.prototype.onMetricsPanelRefresh = function () {
    };
    MetricsPanelCtrl.prototype.setTimeQueryStart = function () {
    };
    MetricsPanelCtrl.prototype.setTimeQueryEnd = function () {
    };
    MetricsPanelCtrl.prototype.updateTimeRange = function (datasource) {
    };
    MetricsPanelCtrl.prototype.calculateInterval = function () {
    };
    MetricsPanelCtrl.prototype.applyPanelTimeOverrides = function () {
    };
    MetricsPanelCtrl.prototype.issueQueries = function (datasource) {
    };
    MetricsPanelCtrl.prototype.handleQueryResult = function (result) {
    };
    MetricsPanelCtrl.prototype.handleDataStream = function (stream) {
    };
    MetricsPanelCtrl.prototype.setDatasource = function (datasource) {
    };
    MetricsPanelCtrl.prototype.addQuery = function (target) {
    };
    MetricsPanelCtrl.prototype.removeQuery = function (target) {
    };
    MetricsPanelCtrl.prototype.moveQuery = function (target, direction) {
    };
    return MetricsPanelCtrl;
}(panel_ctrl_1.PanelCtrl));
exports.MetricsPanelCtrl = MetricsPanelCtrl;


/***/ }),

/***/ "./node_modules/grafana-sdk-mocks/app/features/panel/panel_ctrl.ts":
/*!*************************************************************************!*\
  !*** ./node_modules/grafana-sdk-mocks/app/features/panel/panel_ctrl.ts ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var PanelCtrl = (function () {
    function PanelCtrl($scope, $injector) {
    }
    PanelCtrl.prototype.init = function () {
    };
    PanelCtrl.prototype.renderingCompleted = function () {
    };
    PanelCtrl.prototype.refresh = function () {
    };
    PanelCtrl.prototype.publishAppEvent = function (evtName, evt) {
    };
    PanelCtrl.prototype.changeView = function (fullscreen, edit) {
    };
    PanelCtrl.prototype.viewPanel = function () {
        this.changeView(true, false);
    };
    PanelCtrl.prototype.editPanel = function () {
        this.changeView(true, true);
    };
    PanelCtrl.prototype.exitFullscreen = function () {
        this.changeView(false, false);
    };
    PanelCtrl.prototype.initEditMode = function () {
    };
    PanelCtrl.prototype.changeTab = function (newIndex) {
    };
    PanelCtrl.prototype.addEditorTab = function (title, directiveFn, index) {
    };
    PanelCtrl.prototype.getMenu = function () {
        return [];
    };
    PanelCtrl.prototype.getExtendedMenu = function () {
        return [];
    };
    PanelCtrl.prototype.otherPanelInFullscreenMode = function () {
        return false;
    };
    PanelCtrl.prototype.calculatePanelHeight = function () {
    };
    PanelCtrl.prototype.render = function (payload) {
    };
    PanelCtrl.prototype.toggleEditorHelp = function (index) {
    };
    PanelCtrl.prototype.duplicate = function () {
    };
    PanelCtrl.prototype.updateColumnSpan = function (span) {
    };
    PanelCtrl.prototype.removePanel = function () {
    };
    PanelCtrl.prototype.editPanelJson = function () {
    };
    PanelCtrl.prototype.replacePanel = function (newPanel, oldPanel) {
    };
    PanelCtrl.prototype.sharePanel = function () {
    };
    PanelCtrl.prototype.getInfoMode = function () {
    };
    PanelCtrl.prototype.getInfoContent = function (options) {
    };
    PanelCtrl.prototype.openInspector = function () {
    };
    return PanelCtrl;
}());
exports.PanelCtrl = PanelCtrl;


/***/ }),

/***/ "./node_modules/grafana-sdk-mocks/app/features/panel/query_ctrl.ts":
/*!*************************************************************************!*\
  !*** ./node_modules/grafana-sdk-mocks/app/features/panel/query_ctrl.ts ***!
  \*************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var QueryCtrl = (function () {
    function QueryCtrl($scope, $injector) {
        this.$scope = $scope;
        this.$injector = $injector;
        this.panelCtrl = this.panelCtrl || { panel: {} };
        this.target = this.target || { target: '' };
        this.panel = this.panelCtrl.panel;
    }
    QueryCtrl.prototype.refresh = function () { };
    return QueryCtrl;
}());
exports.QueryCtrl = QueryCtrl;


/***/ }),

/***/ "./node_modules/grafana-sdk-mocks/app/plugins/sdk.ts":
/*!***********************************************************!*\
  !*** ./node_modules/grafana-sdk-mocks/app/plugins/sdk.ts ***!
  \***********************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var panel_ctrl_1 = __webpack_require__(/*! ../features/panel/panel_ctrl */ "./node_modules/grafana-sdk-mocks/app/features/panel/panel_ctrl.ts");
exports.PanelCtrl = panel_ctrl_1.PanelCtrl;
var metrics_panel_ctrl_1 = __webpack_require__(/*! ../features/panel/metrics_panel_ctrl */ "./node_modules/grafana-sdk-mocks/app/features/panel/metrics_panel_ctrl.ts");
exports.MetricsPanelCtrl = metrics_panel_ctrl_1.MetricsPanelCtrl;
var query_ctrl_1 = __webpack_require__(/*! ../features/panel/query_ctrl */ "./node_modules/grafana-sdk-mocks/app/features/panel/query_ctrl.ts");
exports.QueryCtrl = query_ctrl_1.QueryCtrl;
function loadPluginCss(options) {
}
exports.loadPluginCss = loadPluginCss;


/***/ }),

/***/ "./node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js":
/*!****************************************************************************!*\
  !*** ./node_modules/style-loader/dist/runtime/injectStylesIntoStyleTag.js ***!
  \****************************************************************************/
/*! no static exports found */
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

/***/ "./node_modules/webpack/buildin/global.js":
/*!***********************************!*\
  !*** (webpack)/buildin/global.js ***!
  \***********************************/
/*! no static exports found */
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

/***/ "./openHistorianDatasource.ts":
/*!************************************!*\
  !*** ./openHistorianDatasource.ts ***!
  \************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

Object.defineProperty(exports, "__esModule", { value: true });
var OpenHistorianDataSource = (function () {
    function OpenHistorianDataSource(instanceSettings, $q, backendSrv, templateSrv, uiSegmentSrv) {
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
            excludedDataFlags: (instanceSettings.jsonData.Excluded == undefined ? 0 : instanceSettings.jsonData.Excluded),
            excludeNormalData: (instanceSettings.jsonData.Normal == undefined ? false : instanceSettings.jsonData.Normal),
            updateAlarms: (instanceSettings.jsonData.Alarms == undefined ? false : instanceSettings.jsonData.Alarms),
        };
    }
    OpenHistorianDataSource.prototype.query = function (options) {
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
            }).catch(function (data) {
            });
        }
        return this.backendSrv.datasourceRequest({
            url: this.url + '/query',
            data: query,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
    };
    OpenHistorianDataSource.prototype.testDatasource = function () {
        return this.backendSrv.datasourceRequest({
            url: this.url + '/',
            method: 'GET'
        }).then(function (response) {
            if (response.status === 200) {
                return { status: "success", message: "Data source is working", title: "Success" };
            }
        });
    };
    OpenHistorianDataSource.prototype.annotationQuery = function (options) {
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
    };
    OpenHistorianDataSource.prototype.metricFindQuery = function (options, optionalOptions) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/search',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        }).then(this.mapToTextValue);
    };
    OpenHistorianDataSource.prototype.whereFindQuery = function (options) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/SearchFields',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        }).then(this.mapToTextValue);
    };
    OpenHistorianDataSource.prototype.mapToTextValue = function (result) {
        return _.map(result.data, function (d, i) {
            return { text: d, value: d };
        });
    };
    OpenHistorianDataSource.prototype.buildQueryParameters = function (options) {
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
    };
    OpenHistorianDataSource.prototype.filterFindQuery = function () {
        return this.backendSrv.datasourceRequest({
            url: this.url + '/SearchFilters',
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        }).then(this.mapToTextValue);
    };
    OpenHistorianDataSource.prototype.orderByFindQuery = function (options) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/SearchOrderBys',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        }).then(this.mapToTextValue);
    };
    OpenHistorianDataSource.prototype.getMetaData = function (options) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/GetMetadata',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
    };
    OpenHistorianDataSource.prototype.getAlarmStates = function (options) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/GetAlarmState',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
    };
    OpenHistorianDataSource.prototype.getDataAvailability = function (options) {
        var interpolated = {
            target: this.templateSrv.replace(options, null, 'regex')
        };
        return this.backendSrv.datasourceRequest({
            url: this.url + '/GetDataAvailability',
            data: interpolated,
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
    };
    OpenHistorianDataSource.prototype.fixTemplates = function (target) {
        if (target.target == undefined)
            return '';
        var ctrl = this;
        var sep = ' ';
        if (target.queryType == 'Element List')
            sep = ';';
        return target.target.split(sep).map(function (a) {
            if (ctrl.templateSrv.variableExists(a)) {
                return ctrl.templateSrv.replaceWithText(a);
            }
            else
                return a;
        }).join(sep);
    };
    OpenHistorianDataSource.prototype.queryLocation = function (target) {
        if ((target.target == null) || (target.target == undefined)) {
            target.target = {};
        }
        if ((target.radius == null) || (target.radius == undefined) || (target.zoom == null) || (target.zoom == undefined)) {
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
    };
    OpenHistorianDataSource.prototype.GetDashboard = function (alarms, query, ctrl) {
        ctrl.backendSrv.datasourceRequest({
            url: ctrl.url + '/QueryAlarms',
            method: 'POST',
            data: query,
            headers: { 'Content-Type': 'application/json' }
        }).then(function (data) {
        }).catch(function (data) {
        });
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
    };
    OpenHistorianDataSource.prototype.CheckPanel = function (alarms, query, dashboard, ctrl) {
        var alerts = dashboard.dashboard.panels;
        if (alerts === undefined)
            return;
        alerts = alerts.find(function (item) { return item.id == query.panelId; });
        if (alerts == undefined || alerts == null)
            return;
        alerts = alerts.thresholds;
        if ((alerts == undefined || alerts == null || alerts.length == 0) && (alarms.length == 0))
            return;
        if ((alerts == undefined || alerts == null || alerts.length == 0) && (alarms.length > 0)) {
            ctrl.UpdateAlarms(alarms, dashboard.dashboard.uid, query, ctrl);
            return;
        }
        var threshholds = [];
        try {
            threshholds = alerts.map(function (item) { return item.value; });
        }
        catch (_a) {
            return;
        }
        var needsUpdate = false;
        alarms.forEach(function (item) {
            if (!threshholds.includes(item.SetPoint))
                needsUpdate = true;
        });
        if (needsUpdate) {
            ctrl.UpdateAlarms(alarms, dashboard.dashboard.uid, query, ctrl);
        }
    };
    OpenHistorianDataSource.prototype.UpdateAlarms = function (alarms, dashboardUid, query, ctrl) {
        ctrl.backendSrv.datasourceRequest({
            url: 'api/dashboards/uid/' + dashboardUid,
            method: 'GET',
            headers: { 'Content-Type': 'application/json' }
        }).then(function (data) {
            var dashboard = data.data.dashboard;
            var panelIndex = dashboard.panels.findIndex(function (item) { return item.id == query.panelId; });
            dashboard.panels[panelIndex].thresholds = alarms.map(function (item) {
                var op = "gt";
                if (item.Operation == 21 || item.Operation == 22)
                    op = "lt";
                var fill = true;
                if (item.Operation == 1 || item.Operation == 2)
                    fill = false;
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
            }).catch(function (data) { console.log(data); });
        });
    };
    return OpenHistorianDataSource;
}());
exports.default = OpenHistorianDataSource;


/***/ })

/******/ })));
//# sourceMappingURL=data:application/json;charset=utf-8;base64,eyJ2ZXJzaW9uIjozLCJzb3VyY2VzIjpbIndlYnBhY2s6Ly8vd2VicGFjay9ib290c3RyYXAiLCJ3ZWJwYWNrOi8vLy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhbkFubm90YXRpb25zX2N0cmwudHMiLCJ3ZWJwYWNrOi8vLy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhbkNvbmZpZ19jdHJsLnRzIiwid2VicGFjazovLy8uL2NvbnRyb2xsZXJzL29wZW5IaXN0b3JpYW5FbGVtZW50UGlja2VyX2N0cmwudHMiLCJ3ZWJwYWNrOi8vLy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhbkZpbHRlckV4cHJlc3Npb25fY3RybC50cyIsIndlYnBhY2s6Ly8vLi9jb250cm9sbGVycy9vcGVuSGlzdG9yaWFuUXVlcnlPcHRpb25zX2N0cmwudHMiLCJ3ZWJwYWNrOi8vLy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhblF1ZXJ5X2N0cmwudHMiLCJ3ZWJwYWNrOi8vLy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhblRleHRFZGl0b3JfY3RybC50cyIsIndlYnBhY2s6Ly8vLi9jc3MvcXVlcnktZWRpdG9yLmNzcz9lYzM5Iiwid2VicGFjazovLy8uL2pzL29wZW5IaXN0b3JpYW5Db25zdGFudHMudHMiLCJ3ZWJwYWNrOi8vLy4vbW9kdWxlLnRzIiwid2VicGFjazovLy8uL2Nzcy9xdWVyeS1lZGl0b3IuY3NzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9jc3MtbG9hZGVyL2Rpc3QvcnVudGltZS9hcGkuanMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2dyYWZhbmEtc2RrLW1vY2tzL2FwcC9mZWF0dXJlcy9wYW5lbC9tZXRyaWNzX3BhbmVsX2N0cmwudHMiLCJ3ZWJwYWNrOi8vLy4vbm9kZV9tb2R1bGVzL2dyYWZhbmEtc2RrLW1vY2tzL2FwcC9mZWF0dXJlcy9wYW5lbC9wYW5lbF9jdHJsLnRzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9ncmFmYW5hLXNkay1tb2Nrcy9hcHAvZmVhdHVyZXMvcGFuZWwvcXVlcnlfY3RybC50cyIsIndlYnBhY2s6Ly8vLi9ub2RlX21vZHVsZXMvZ3JhZmFuYS1zZGstbW9ja3MvYXBwL3BsdWdpbnMvc2RrLnRzIiwid2VicGFjazovLy8uL25vZGVfbW9kdWxlcy9zdHlsZS1sb2FkZXIvZGlzdC9ydW50aW1lL2luamVjdFN0eWxlc0ludG9TdHlsZVRhZy5qcyIsIndlYnBhY2s6Ly8vKHdlYnBhY2spL2J1aWxkaW4vZ2xvYmFsLmpzIiwid2VicGFjazovLy8uL29wZW5IaXN0b3JpYW5EYXRhc291cmNlLnRzIl0sIm5hbWVzIjpbXSwibWFwcGluZ3MiOiI7UUFBQTtRQUNBOztRQUVBO1FBQ0E7O1FBRUE7UUFDQTtRQUNBO1FBQ0E7UUFDQTtRQUNBO1FBQ0E7UUFDQTtRQUNBO1FBQ0E7O1FBRUE7UUFDQTs7UUFFQTtRQUNBOztRQUVBO1FBQ0E7UUFDQTs7O1FBR0E7UUFDQTs7UUFFQTtRQUNBOztRQUVBO1FBQ0E7UUFDQTtRQUNBLDBDQUEwQyxnQ0FBZ0M7UUFDMUU7UUFDQTs7UUFFQTtRQUNBO1FBQ0E7UUFDQSx3REFBd0Qsa0JBQWtCO1FBQzFFO1FBQ0EsaURBQWlELGNBQWM7UUFDL0Q7O1FBRUE7UUFDQTtRQUNBO1FBQ0E7UUFDQTtRQUNBO1FBQ0E7UUFDQTtRQUNBO1FBQ0E7UUFDQTtRQUNBLHlDQUF5QyxpQ0FBaUM7UUFDMUUsZ0hBQWdILG1CQUFtQixFQUFFO1FBQ3JJO1FBQ0E7O1FBRUE7UUFDQTtRQUNBO1FBQ0EsMkJBQTJCLDBCQUEwQixFQUFFO1FBQ3ZELGlDQUFpQyxlQUFlO1FBQ2hEO1FBQ0E7UUFDQTs7UUFFQTtRQUNBLHNEQUFzRCwrREFBK0Q7O1FBRXJIO1FBQ0E7OztRQUdBO1FBQ0E7Ozs7Ozs7Ozs7Ozs7OztBQzNEQTtJQUFBO0lBRUEsQ0FBQztJQURTLDZDQUFXLEdBQUcsaUNBQWlDLENBQUM7SUFDMUQsd0NBQUM7Q0FBQTtrQkFGb0IsaUNBQWlDOzs7Ozs7Ozs7Ozs7Ozs7QUNHdEQ7SUFLSSxpQ0FBWSxNQUFNO1FBQ2QsSUFBSSxJQUFJLEdB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T0FBTyxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsQ0FBQyxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxFQUFHO0lBQ2xELFdBQVcsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLENBQUMsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLENBQUMsRUFBRTtJQUNyRCxjQUFjLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxDQUFDLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxDQUFDLEVBQUU7SUFDeEQsZUFBZSxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsQ0FBQyxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxFQUFFO0lBQ3pELFNBQVMsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLENBQUMsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLENBQUMsRUFBRTtJQUNuRCxRQUFRLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxDQUFDLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxDQUFDLEVBQUU7SUFDbEQsV0FBVyxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsQ0FBQyxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxFQUFFO0lBQ3JELFVBQVUsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLENBQUMsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLENBQUMsRUFBRTtJQUNwRCxhQUFhLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxDQUFDLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxDQUFDLEVBQUU7SUFDdkQsZUFBZSxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksQ0FBQyxFQUFFO0lBQzFELFFBQVEsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUNwRCxhQUFhLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxFQUFFLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxFQUFFLEVBQUU7SUFDekQsZUFBZSxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQzNELGdCQUFnQixFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQzVELGtCQUFrQixFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQzlELG1CQUFtQixFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQy9ELE9BQU8sRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUNuRCxXQUFXLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxFQUFFLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxFQUFFLEVBQUU7SUFDdkQsYUFBYSxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQ3pELGVBQWUsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUMzRCxTQUFTLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxFQUFFLEVBQUUsSUFBSSxFQUFFLENBQUMsSUFBSSxFQUFFLEVBQUU7SUFDckQsV0FBVyxFQUFFLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsRUFBRSxFQUFFLElBQUksRUFBRSxDQUFDLElBQUksRUFBRSxFQUFFO0lBQ3ZELGNBQWMsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUMxRCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxnQkFBZ0IsRUFBRSxFQUFFLEtBQUssRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEVBQUUsRUFBRSxJQUFJLEVBQUUsQ0FBQyxJQUFJLEVBQUUsRUFBRTtJQUM1RCxXQUFXLEVBQUUsRUFBRSxLQUFLLEVBQUUsS0FBSyxFQUFFLEtBQUssRUFB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QUUsSUFBSSxDQUFDLFFBQVE7aUJBQ3ZCO1lBQ0wsQ0FBQyxDQUFDLENBQUM7WUFFSCxJQUFJLENBQUMsVUFBVSxDQUFDLGlCQUFpQixDQUFDO2dCQUM5QixHQUFHLEVBQUUsbUJBQW1CO2dCQUN4QixNQUFNLEVBQUUsTUFBTTtnQkFDZCxJQUFJLEVBQUUsRUFBRSxTQUFTLEVBQUUsSUFBSSxFQUFFLFNBQVMsRUFBRSxTQUFTLEVBQUM7Z0JBQzlDLE9BQU8sRUFBRSxFQUFFLGNBQWMsRUFBRSxrQkFBa0IsRUFBRTthQUNsRCxDQUFDLENBQUMsS0FBSyxDQUFDLFVBQVUsSUFBSSxJQUFJLE9BQU8sQ0FBQyxHQUFHLENBQUMsSUFBSSxDQUFDLENBQUMsQ0FBQyxDQUFDLENBQUM7UUFDcEQsQ0FBQyxDQUFDLENBQUM7SUFFUCxDQUFDO0lBRUwsOEJBQUM7QUFBRCxDQUFDIiwiZmlsZSI6Im1vZHVsZS5qcyIsInNvdXJjZXNDb250ZW50IjpbIiBcdC8vIFRoZSBtb2R1bGUgY2FjaGVcbiBcdHZhciBpbnN0YWxsZWRNb2R1bGVzID0ge307XG5cbiBcdC8vIFRoZSByZXF1aXJlIGZ1bmN0aW9uXG4gXHRmdW5jdGlvbiBfX3dlYnBhY2tfcmVxdWlyZV9fKG1vZHVsZUlkKSB7XG5cbiBcdFx0Ly8gQ2hlY2sgaWYgbW9kdWxlIGlzIGluIGNhY2hlXG4gXHRcdGlmKGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdKSB7XG4gXHRcdFx0cmV0dXJuIGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdLmV4cG9ydHM7XG4gXHRcdH1cbiBcdFx0Ly8gQ3JlYXRlIGEgbmV3IG1vZHVsZSAoYW5kIHB1dCBpdCBpbnRvIHRoZSBjYWNoZSlcbiBcdFx0dmFyIG1vZHVsZSA9IGluc3RhbGxlZE1vZHVsZXNbbW9kdWxlSWRdID0ge1xuIFx0XHRcdGk6IG1vZHVsZUlkLFxuIFx0XHRcdGw6IGZhbHNlLFxuIFx0XHRcdGV4cG9ydHM6IHt9XG4gXHRcdH07XG5cbiBcdFx0Ly8gRXhlY3V0ZSB0aGUgbW9kdWxlIGZ1bmN0aW9uXG4gXHRcdG1vZHVsZXNbbW9kdWxlSWRdLmNhbGwobW9kdWxlLmV4cG9ydHMsIG1vZHVsZSwgbW9kdWxlLmV4cG9ydHMsIF9fd2VicGFja19yZXF1aXJlX18pO1xuXG4gXHRcdC8vIEZsYWcgdGhlIG1vZHVsZSBhcyBsb2FkZWRcbiBcdFx0bW9kdWxlLmwgPSB0cnVlO1xuXG4gXHRcdC8vIFJldHVybiB0aGUgZXhwb3J0cyBvZiB0aGUgbW9kdWxlXG4gXHRcdHJldHVybiBtb2R1bGUuZXhwb3J0cztcbiBcdH1cblxuXG4gXHQvLyBleHBvc2UgdGhlIG1vZHVsZXMgb2JqZWN0IChfX3dlYnBhY2tfbW9kdWxlc19fKVxuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5tID0gbW9kdWxlcztcblxuIFx0Ly8gZXhwb3NlIHRoZSBtb2R1bGUgY2FjaGVcbiBcdF9fd2VicGFja19yZXF1aXJlX18uYyA9IGluc3RhbGxlZE1vZHVsZXM7XG5cbiBcdC8vIGRlZmluZSBnZXR0ZXIgZnVuY3Rpb24gZm9yIGhhcm1vbnkgZXhwb3J0c1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5kID0gZnVuY3Rpb24oZXhwb3J0cywgbmFtZSwgZ2V0dGVyKSB7XG4gXHRcdGlmKCFfX3dlYnBhY2tfcmVxdWlyZV9fLm8oZXhwb3J0cywgbmFtZSkpIHtcbiBcdFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgbmFtZSwgeyBlbnVtZXJhYmxlOiB0cnVlLCBnZXQ6IGdldHRlciB9KTtcbiBcdFx0fVxuIFx0fTtcblxuIFx0Ly8gZGVmaW5lIF9fZXNNb2R1bGUgb24gZXhwb3J0c1xuIFx0X193ZWJwYWNrX3JlcXVpcmVfXy5yID0gZnVuY3Rpb24oZXhwb3J0cykge1xuIFx0XHRpZih0eXBlb2YgU3ltYm9sICE9PSAndW5kZWZpbmVkJyAmJiBTeW1ib2wudG9TdHJpbmdUYWcpIHtcbiBcdFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgU3ltYm9sLnRvU3RyaW5nVGFnLCB7IHZhbHVlOiAnTW9kdWxlJyB9KTtcbiBcdFx0fVxuIFx0XHRPYmplY3QuZGVmaW5lUHJvcGVydHkoZXhwb3J0cywgJ19fZXNNb2R1bGUnLCB7IHZhbHVlOiB0cnVlIH0pO1xuIFx0fTtcblxuIFx0Ly8gY3JlYXRlIGEgZmFrZSBuYW1lc3BhY2Ugb2JqZWN0XG4gXHQvLyBtb2RlICYgMTogdmFsdWUgaXMgYSBtb2R1bGUgaWQsIHJlcXVpcmUgaXRcbiBcdC8vIG1vZGUgJiAyOiBtZXJnZSBhbGwgcHJvcGVydGllcyBvZiB2YWx1ZSBpbnRvIHRoZSBuc1xuIFx0Ly8gbW9kZSAmIDQ6IHJldHVybiB2YWx1ZSB3aGVuIGFscmVhZHkgbnMgb2JqZWN0XG4gXHQvLyBtb2RlICYgOHwxOiBiZWhhdmUgbGlrZSByZXF1aXJlXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLnQgPSBmdW5jdGlvbih2YWx1ZSwgbW9kZSkge1xuIFx0XHRpZihtb2RlICYgMSkgdmFsdWUgPSBfX3dlYnBhY2tfcmVxdWlyZV9fKHZhbHVlKTtcbiBcdFx0aWYobW9kZSAmIDgpIHJldHVybiB2YWx1ZTtcbiBcdFx0aWYoKG1vZGUgJiA0KSAmJiB0eXBlb2YgdmFsdWUgPT09ICdvYmplY3QnICYmIHZhbHVlICYmIHZhbHVlLl9fZXNNb2R1bGUpIHJldHVybiB2YWx1ZTtcbiBcdFx0dmFyIG5zID0gT2JqZWN0LmNyZWF0ZShudWxsKTtcbiBcdFx0X193ZWJwYWNrX3JlcXVpcmVfXy5yKG5zKTtcbiBcdFx0T2JqZWN0LmRlZmluZVByb3BlcnR5KG5zLCAnZGVmYXVsdCcsIHsgZW51bWVyYWJsZTogdHJ1ZSwgdmFsdWU6IHZhbHVlIH0pO1xuIFx0XHRpZihtb2RlICYgMiAmJiB0eXBlb2YgdmFsdWUgIT0gJ3N0cmluZycpIGZvcih2YXIga2V5IGluIHZhbHVlKSBfX3dlYnBhY2tfcmVxdWlyZV9fLmQobnMsIGtleSwgZnVuY3Rpb24oa2V5KSB7IHJldHVybiB2YWx1ZVtrZXldOyB9LmJpbmQobnVsbCwga2V5KSk7XG4gXHRcdHJldHVybiBucztcbiBcdH07XG5cbiBcdC8vIGdldERlZmF1bHRFeHBvcnQgZnVuY3Rpb24gZm9yIGNvbXBhdGliaWxpdHkgd2l0aCBub24taGFybW9ueSBtb2R1bGVzXG4gXHRfX3dlYnBhY2tfcmVxdWlyZV9fLm4gPSBmdW5jdGlvbihtb2R1bGUpIHtcbiBcdFx0dmFyIGdldHRlciA9IG1vZHVsZSAmJiBtb2R1bGUuX19lc01vZHVsZSA/XG4gXHRcdFx0ZnVuY3Rpb24gZ2V0RGVmYXVsdCgpIHsgcmV0dXJuIG1vZHVsZVsnZGVmYXVsdCddOyB9IDpcbiBcdFx0XHRmdW5jdGlvbiBnZXRNb2R1bGVFeHBvcnRzKCkgeyByZXR1cm4gbW9kdWxlOyB9O1xuIFx0XHRfX3dlYnBhY2tfcmVxdWlyZV9fLmQoZ2V0dGVyLCAnYScsIGdldHRlcik7XG4gXHRcdHJldHVybiBnZXR0ZXI7XG4gXHR9O1xuXG4gXHQvLyBPYmplY3QucHJvdG90eXBlLmhhc093blByb3BlcnR5LmNhbGxcbiBcdF9fd2VicGFja19yZXF1aXJlX18ubyA9IGZ1bmN0aW9uKG9iamVjdCwgcHJvcGVydHkpIHsgcmV0dXJuIE9iamVjdC5wcm90b3R5cGUuaGFzT3duUHJvcGVydHkuY2FsbChvYmplY3QsIHByb3BlcnR5KTsgfTtcblxuIFx0Ly8gX193ZWJwYWNrX3B1YmxpY19wYXRoX19cbiBcdF9fd2VicGFja19yZXF1aXJlX18ucCA9IFwiXCI7XG5cblxuIFx0Ly8gTG9hZCBlbnRyeSBtb2R1bGUgYW5kIHJldHVybiBleHBvcnRzXG4gXHRyZXR1cm4gX193ZWJwYWNrX3JlcXVpcmVfXyhfX3dlYnBhY2tfcmVxdWlyZV9fLnMgPSBcIi4vbW9kdWxlLnRzXCIpO1xuIiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIG9wZW5IaXN0b3JpYW5Bbm5vdGF0aW9uc19jdHJsLnRzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IO+/vSAyMDE5LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAwOC8xOS8yMDE5IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5cclxuZXhwb3J0IGRlZmF1bHQgY2xhc3MgT3Blbkhpc3RvcmlhbkFubm90YXRpb25zUXVlcnlDdHJse1xyXG4gICBzdGF0aWMgdGVtcGxhdGVVcmwgPSAncGFydGlhbC9hbm5vdGF0aW9ucy5lZGl0b3IuaHRtbCc7XHJcbn1cclxuIiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIGNvbmZpZ19jdHJsLmpzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IO+/vSAyMDE3LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAxMC8zMC8yMDE3IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5cclxuZGVjbGFyZSB2YXIgXzogYW55O1xyXG5pbXBvcnQgeyBEZWZhdWx0RmxhZ3MgfSBmcm9tICcuLy4uL2pzL29wZW5IaXN0b3JpYW5Db25zdGFudHMnXHJcblxyXG5leHBvcnQgZGVmYXVsdCBjbGFzcyBPcGVuSGlzdG9yaWFuQ29uZmlnQ3RybHtcclxuICAgIHN0YXRpYyB0ZW1wbGF0ZVVybDpzdHJpbmcgPSAncGFydGlhbC9jb25maWcuaHRtbCc7XHJcbiAgICBjdXJyZW50OiBhbnk7XHJcbiAgICBmbGFnQXJyYXk6IEFycmF5PGFueT47XHJcblxyXG4gICAgY29uc3RydWN0b3IoJHNjb3BlKSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICBjdHJsLmN1cnJlbnQuanNvbkRhdGEuRXhjbHVkZWQgPSB0aGlzLmN1cnJlbnQuanNvbkRhdGEuRXhjbHVkZWQgfHwgMDtcclxuICAgICAgICBjdHJsLmN1cnJlbnQuanNvbkRhdGEuTm9ybWFsID0gdGhpcy5jdXJyZW50Lmpzb25EYXRhLk5vcm1hbCB8fCBmYWxzZTtcclxuICAgICAgICBjdHJsLmN1cnJlbnQuanNvbkRhdGEuQWxhcm1zID0gdGhpcy5jdXJyZW50Lmpzb25EYXRhLkFsYXJtcyB8fCBmYWxzZTtcclxuICAgIH1cclxuXHJcbiAgICBcclxufVxyXG5cclxuIiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIG9wZW5IaXN0b3JpYW5FbGVtZW50UGlja2VyX2N0cmwudHMgLSBHYnRjXHJcbi8vXHJcbi8vICBDb3B5cmlnaHQgwqkgMjAxNywgR3JpZCBQcm90ZWN0aW9uIEFsbGlhbmNlLiAgQWxsIFJpZ2h0cyBSZXNlcnZlZC5cclxuLy9cclxuLy8gIExpY2Vuc2VkIHRvIHRoZSBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UgKEdQQSkgdW5kZXIgb25lIG9yIG1vcmUgY29udHJpYnV0b3IgbGljZW5zZSBhZ3JlZW1lbnRzLiBTZWVcclxuLy8gIHRoZSBOT1RJQ0UgZmlsZSBkaXN0cmlidXRlZCB3aXRoIHRoaXMgd29yayBmb3IgYWRkaXRpb25hbCBpbmZvcm1hdGlvbiByZWdhcmRpbmcgY29weXJpZ2h0IG93bmVyc2hpcC5cclxuLy8gIFRoZSBHUEEgbGljZW5zZXMgdGhpcyBmaWxlIHRvIHlvdSB1bmRlciB0aGUgTUlUIExpY2Vuc2UgKE1JVCksIHRoZSBcIkxpY2Vuc2VcIjsgeW91IG1heSBub3QgdXNlIHRoaXNcclxuLy8gIGZpbGUgZXhjZXB0IGluIGNvbXBsaWFuY2Ugd2l0aCB0aGUgTGljZW5zZS4gWW91IG1heSBvYnRhaW4gYSBjb3B5IG9mIHRoZSBMaWNlbnNlIGF0OlxyXG4vL1xyXG4vLyAgICAgIGh0dHA6Ly9vcGVuc291cmNlLm9yZy9saWNlbnNlcy9NSVRcclxuLy9cclxuLy8gIFVubGVzcyBhZ3JlZWQgdG8gaW4gd3JpdGluZywgdGhlIHN1YmplY3Qgc29mdHdhcmUgZGlzdHJpYnV0ZWQgdW5kZXIgdGhlIExpY2Vuc2UgaXMgZGlzdHJpYnV0ZWQgb24gYW5cclxuLy8gIFwiQVMtSVNcIiBCQVNJUywgV0lUSE9VVCBXQVJSQU5USUVTIE9SIENPTkRJVElPTlMgT0YgQU5ZIEtJTkQsIGVpdGhlciBleHByZXNzIG9yIGltcGxpZWQuIFJlZmVyIHRvIHRoZVxyXG4vLyAgTGljZW5zZSBmb3IgdGhlIHNwZWNpZmljIGxhbmd1YWdlIGdvdmVybmluZyBwZXJtaXNzaW9ucyBhbmQgbGltaXRhdGlvbnMuXHJcbi8vXHJcbi8vICBDb2RlIE1vZGlmaWNhdGlvbiBIaXN0b3J5OlxyXG4vLyAgLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyAgMTIvMTIvMjAxNyAtIEJpbGx5IEVybmVzdFxyXG4vLyAgICAgICBHZW5lcmF0ZWQgb3JpZ2luYWwgdmVyc2lvbiBvZiBzb3VyY2UgY29kZS5cclxuLy9cclxuLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uL21vZHVsZS5kLnRzXCIvPlxyXG5cclxuaW1wb3J0IHsgRnVuY3Rpb25MaXN0LCBCb29sZWFucywgQW5nbGVVbml0cywgVGltZVVuaXRzLCBXaGVyZU9wZXJhdG9ycyB9IGZyb20gJy4vLi4vanMvb3Blbkhpc3RvcmlhbkNvbnN0YW50cydcclxuaW1wb3J0IE9wZW5IaXN0b3JpYW5EYXRhU291cmNlIGZyb20gJy4uL29wZW5IaXN0b3JpYW5EYXRhc291cmNlJztcclxuZGVjbGFyZSB2YXIgXzogYW55O1xyXG5cclxuZXhwb3J0IGRlZmF1bHQgY2xhc3MgT3Blbkhpc3RvcmlhbkVsZW1lbnRQaWNrZXJDdHJse1xyXG4gICAgZWxlbWVudFNlZ21lbnQ6IGlTZWdtZW50O1xyXG4gICAgc2VnbWVudHM6IEFycmF5PGlTZWdtZW50PjtcclxuICAgIGZ1bmN0aW9uU2VnbWVudDogaUZ1bmN0aW9uU2VnbWVudDtcclxuICAgIGZ1bmN0aW9uU2VnbWVudHM6IEFycmF5PGlGdW5jdGlvblNlZ21lbnQ+O1xyXG4gICAgZnVuY3Rpb25zOiBBcnJheTxpRnVuY3Rpb25TZWdtZW50PjtcclxuICAgIHR5cGluZ1RpbWVyOiBOb2RlSlMuVGltZW91dDtcclxuICAgIHRhcmdldDogaVRhcmdldDtcclxuICAgIGRhdGFzb3VyY2U6IE9wZW5IaXN0b3JpYW5EYXRhU291cmNlO1xyXG4gICAgY29uc3RydWN0b3IocHJpdmF0ZSAkc2NvcGUsIHByaXZhdGUgdWlTZWdtZW50U3J2KSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICB0aGlzLiRzY29wZSA9ICRzY29wZTtcclxuICAgICAgICB0aGlzLnVpU2VnbWVudFNydiA9IHVpU2VnbWVudFNydjtcclxuXHJcbiAgICAgICAgdGhpcy5zZWdtZW50cyA9ICh0aGlzLiRzY29wZS50YXJnZXQuc2VnbWVudHMgPT0gdW5kZWZpbmVkID8gW10gOiB0aGlzLiRzY29wZS50YXJnZXQuc2VnbWVudHMubWFwKGZ1bmN0aW9uIChhKSB7IHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KHsgdmFsdWU6IGEudGV4dCwgZXhwYW5kYWJsZTogdHJ1ZSB9KSB9KSk7XHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnRzID0gKHRoaXMuJHNjb3BlLnRhcmdldC5mdW5jdGlvblNlZ21lbnRzID09IHVuZGVmaW5lZCA/IFtdIDogdGhpcy4kc2NvcGUudGFyZ2V0LmZ1bmN0aW9uU2VnbWVudHMpO1xyXG4gICAgICAgIHRoaXMuZnVuY3Rpb25zID0gW107XHJcbiAgICAgICAgdGhpcy5lbGVtZW50U2VnbWVudCA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld1BsdXNCdXR0b24oKTtcclxuICAgICAgICB0aGlzLmZ1bmN0aW9uU2VnbWVudCA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld1BsdXNCdXR0b24oKTtcclxuXHJcbiAgICAgICAgdGhpcy5idWlsZEZ1bmN0aW9uQXJyYXkoKTtcclxuXHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoRWxlbWVudHMoKTtcclxuXHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQud2hlcmVzO1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0LnRvcE5TZWdtZW50O1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0Lm9yZGVyQnlzO1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0LndoZXJlU2VnbWVudDtcclxuICAgICAgICBkZWxldGUgJHNjb3BlLnRhcmdldC5maWx0ZXJTZWdtZW50O1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0Lm9yZGVyQnlTZWdtZW50O1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0LnRhcmdldFRleHQ7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGdldEVsZW1lbnRTZWdtZW50cyhuZXdTZWdtZW50KSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIHZhciBvcHRpb24gPSBudWxsO1xyXG4gICAgICAgIGlmIChldmVudC50YXJnZXRbJ3ZhbHVlJ10gIT0gXCJcIikgb3B0aW9uID0gZXZlbnQudGFyZ2V0Wyd2YWx1ZSddO1xyXG5cclxuICAgICAgICByZXR1cm4gY3RybC4kc2NvcGUuZGF0YXNvdXJjZS5tZXRyaWNGaW5kUXVlcnkob3B0aW9uKS50aGVuKGRhdGEgPT4ge1xyXG4gICAgICAgICAgICB2YXIgYWx0U2VnbWVudHMgPSBfLm1hcChkYXRhLCBpdGVtID0+IHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KHsgdmFsdWU6IGl0ZW0udGV4dCwgZXhwYW5kYWJsZTogaXRlbS5leHBhbmRhYmxlIH0pXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICBhbHRTZWdtZW50cy5zb3J0KChhLCBiKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoYS52YWx1ZSA8IGIudmFsdWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIC0xO1xyXG4gICAgICAgICAgICAgICAgaWYgKGEudmFsdWUgPiBiLnZhbHVlKVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAxO1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIDA7XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgXy5lYWNoKGN0cmwuJHNjb3BlLmRhdGFzb3VyY2UudGVtcGxhdGVTcnYudmFyaWFibGVzLCAoaXRlbSwgaW5kZXgsIGxpc3QpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChpdGVtLnR5cGUgPT0gXCJxdWVyeVwiKVxyXG4gICAgICAgICAgICAgICAgICAgIGFsdFNlZ21lbnRzLnVuc2hpZnQoY3RybC51aVNlZ21lbnRTcnYubmV3Q29uZGl0aW9uKCckJyArIGl0ZW0ubmFtZSkpO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGlmICghbmV3U2VnbWVudClcclxuICAgICAgICAgICAgICAgIGFsdFNlZ21lbnRzLnVuc2hpZnQoY3RybC51aVNlZ21lbnRTcnYubmV3U2VnbWVudCgnLVJFTU9WRS0nKSk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gXy5maWx0ZXIoYWx0U2VnbWVudHMsIHNlZ21lbnQgPT4ge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIF8uZmluZChjdHJsLnNlZ21lbnRzLCB4ID0+IHtcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4geC52YWx1ZSA9PSBzZWdtZW50LnZhbHVlXHJcbiAgICAgICAgICAgICAgICB9KSA9PSB1bmRlZmluZWQ7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgIH1cclxuXHJcbiAgICBhZGRFbGVtZW50U2VnbWVudCgpIHtcclxuICAgICAgICAvLyBpZiB2YWx1ZSBpcyBub3QgZW1wdHksIGFkZCBuZXcgYXR0cmlidXRlIHNlZ21lbnRcclxuICAgICAgICBpZiAoZXZlbnQudGFyZ2V0Wyd0ZXh0J10gIT0gbnVsbCkge1xyXG4gICAgICAgICAgICB0aGlzLnNlZ21lbnRzLnB1c2godGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCh7IHZhbHVlOiBldmVudC50YXJnZXRbJ3RleHQnXSwgZXhwYW5kYWJsZTogdHJ1ZSB9KSlcclxuICAgICAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoRWxlbWVudHMoKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gcmVzZXQgdGhlICsgYnV0dG9uXHJcbiAgICAgICAgdmFyIHBsdXNCdXR0b24gPSB0aGlzLnVpU2VnbWVudFNydi5uZXdQbHVzQnV0dG9uKClcclxuICAgICAgICB0aGlzLmVsZW1lbnRTZWdtZW50LnZhbHVlID0gcGx1c0J1dHRvbi52YWx1ZVxyXG4gICAgICAgIHRoaXMuZWxlbWVudFNlZ21lbnQuaHRtbCA9IHBsdXNCdXR0b24uaHRtbFxyXG4gICAgICAgIHRoaXMuJHNjb3BlLnBhbmVsLnJlZnJlc2goKVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBzZWdtZW50VmFsdWVDaGFuZ2VkKHNlZ21lbnQsIGluZGV4KSB7XHJcbiAgICAgICAgaWYgKHNlZ21lbnQudmFsdWUgPT0gXCItUkVNT1ZFLVwiKSB7XHJcbiAgICAgICAgICAgIHRoaXMuc2VnbWVudHMuc3BsaWNlKGluZGV4LCAxKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgIHRoaXMuc2VnbWVudHNbaW5kZXhdID0gc2VnbWVudDtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuc2V0VGFyZ2V0V2l0aEVsZW1lbnRzKClcclxuICAgIH1cclxuXHJcbiAgICBzZXRUYXJnZXRXaXRoRWxlbWVudHMoKSB7XHJcbiAgICAgICAgdmFyIGZ1bmN0aW9ucyA9ICcnO1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICBfLmVhY2goY3RybC5mdW5jdGlvbnMsIGZ1bmN0aW9uIChlbGVtZW50LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICBpZiAoZWxlbWVudC52YWx1ZSA9PSAnUVVFUlknKSBmdW5jdGlvbnMgKz0gY3RybC5zZWdtZW50cy5tYXAoZnVuY3Rpb24gKGEpIHsgcmV0dXJuIGEudmFsdWUgfSkuam9pbignOycpXHJcbiAgICAgICAgICAgIGVsc2UgZnVuY3Rpb25zICs9IGVsZW1lbnQudmFsdWU7XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIGN0cmwuJHNjb3BlLnRhcmdldC50YXJnZXQgPSAoZnVuY3Rpb25zICE9IFwiXCIgPyBmdW5jdGlvbnMgOiBjdHJsLnNlZ21lbnRzLm1hcChmdW5jdGlvbiAoYSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIGEudmFsdWVcclxuICAgICAgICB9KS5qb2luKCc7JykpO1xyXG5cclxuICAgICAgICBjdHJsLiRzY29wZS50YXJnZXQuZnVuY3Rpb25TZWdtZW50cyA9IGN0cmwuZnVuY3Rpb25TZWdtZW50cztcclxuICAgICAgICBjdHJsLiRzY29wZS50YXJnZXQuc2VnbWVudHMgPSBjdHJsLnNlZ21lbnRzO1xyXG4gICAgICAgIGN0cmwuJHNjb3BlLnRhcmdldC5xdWVyeVR5cGUgPSAnRWxlbWVudCBMaXN0JztcclxuICAgICAgICB0aGlzLiRzY29wZS5wYW5lbC5yZWZyZXNoKClcclxuXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0RnVuY3Rpb25zVG9BZGROZXcoKSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIHZhciBhcnJheSA9IFtdXHJcbiAgICAgICAgXy5lYWNoKE9iamVjdC5rZXlzKEZ1bmN0aW9uTGlzdCksIGZ1bmN0aW9uIChlbGVtZW50LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICBhcnJheS5wdXNoKGN0cmwudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoZWxlbWVudCkpO1xyXG4gICAgICAgIH0pO1xyXG5cclxuXHJcbiAgICAgICAgaWYgKHRoaXMuZnVuY3Rpb25zLmxlbmd0aCA9PSAwKSBhcnJheSA9IGFycmF5LnNsaWNlKDIsIGFycmF5Lmxlbmd0aCk7XHJcblxyXG4gICAgICAgIGFycmF5LnNvcnQoZnVuY3Rpb24oYSwgYikge1xyXG4gICAgICAgICAgICB2YXIgbmFtZUEgPSBhLnZhbHVlLnRvVXBwZXJDYXNlKCk7IC8vIGlnbm9yZSB1cHBlciBhbmQgbG93ZXJjYXNlXHJcbiAgICAgICAgICAgIHZhciBuYW1lQiA9IGIudmFsdWUudG9VcHBlckNhc2UoKTsgLy8gaWdub3JlIHVwcGVyIGFuZCBsb3dlcmNhc2VcclxuICAgICAgICAgICAgaWYobmFtZUEgPCBuYW1lQikge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIC0xO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgICAgaWYgKG5hbWVBID4gbmFtZUIpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiAxO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAgIC8vIG5hbWVzIG11c3QgYmUgZXF1YWxcclxuICAgICAgICAgICAgICByZXR1cm4gMDsgICAgICAgIFxyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKF8uZmlsdGVyKGFycmF5LCBzZWdtZW50ID0+IHtcclxuICAgICAgICAgICAgcmV0dXJuIF8uZmluZCh0aGlzLmZ1bmN0aW9ucywgeCA9PiB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4geC52YWx1ZSA9PSBzZWdtZW50LnZhbHVlO1xyXG4gICAgICAgICAgICB9KSA9PSB1bmRlZmluZWQ7XHJcbiAgICAgICAgfSkpO1xyXG4gICAgfVxyXG5cclxuICAgIGdldEZ1bmN0aW9uc1RvRWRpdChmdW5jLCBpbmRleCk6IGFueSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIHZhciByZW1vdmUgPSBbdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCgnLVJFTU9WRS0nKV07XHJcbiAgICAgICAgaWYgKGZ1bmMudHlwZSA9PSAnT3BlcmF0b3InKSByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKCk7XHJcbiAgICAgICAgZWxzZSBpZiAoZnVuYy52YWx1ZSA9PSAnU2V0JykgcmV0dXJuIFByb21pc2UucmVzb2x2ZShyZW1vdmUpXHJcblxyXG4gICAgICAgIHJldHVybiBQcm9taXNlLnJlc29sdmUocmVtb3ZlKTtcclxuICAgIH1cclxuXHJcbiAgICBmdW5jdGlvblZhbHVlQ2hhbmdlZChmdW5jLCBpbmRleCkge1xyXG4gICAgICAgIHZhciBmdW5jU2VnID0gRnVuY3Rpb25MaXN0W2Z1bmMuRnVuY3Rpb25dO1xyXG5cclxuICAgICAgICBpZiAoZnVuYy52YWx1ZSA9PSBcIi1SRU1PVkUtXCIpIHtcclxuICAgICAgICAgICAgdmFyIGwgPSAxO1xyXG4gICAgICAgICAgICB2YXIgZmkgPSBfLmZpbmRJbmRleCh0aGlzLmZ1bmN0aW9uU2VnbWVudHMsIGZ1bmN0aW9uIChzZWdtZW50KSB7IHJldHVybiBzZWdtZW50LkZ1bmN0aW9uID09IGZ1bmMuRnVuY3Rpb24gfSk7XHJcbiAgICAgICAgICAgIGlmIChmdW5jLkZ1bmN0aW9uID09ICdTbGljZScpXHJcbiAgICAgICAgICAgICAgICB0aGlzLmZ1bmN0aW9uU2VnbWVudHNbZmkgKyAxXS5QYXJhbWV0ZXJzID0gdGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpICsgMV0uUGFyYW1ldGVycy5zbGljZSgxLCB0aGlzLmZ1bmN0aW9uU2VnbWVudHNbZmkgKyAxXS5QYXJhbWV0ZXJzLmxlbmd0aCk7XHJcbiAgICAgICAgICAgIGVsc2UgaWYgKGZpID4gMCAmJiAodGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpIC0gMV0uRnVuY3Rpb24gPT0gJ1NldCcgfHwgdGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpIC0gMV0uRnVuY3Rpb24gPT0gJ1NsaWNlJykpIHtcclxuICAgICAgICAgICAgICAgIC0tZmk7XHJcbiAgICAgICAgICAgICAgICArK2w7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50cy5zcGxpY2UoZmksIGwpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlIGlmIChmdW5jLlR5cGUgIT0gJ0Z1bmN0aW9uJykge1xyXG4gICAgICAgICAgICB2YXIgZmkgPSBfLmZpbmRJbmRleCh0aGlzLmZ1bmN0aW9uU2VnbWVudHMsIGZ1bmN0aW9uIChzZWdtZW50KSB7IHJldHVybiBzZWdtZW50LkZ1bmN0aW9uID09IGZ1bmMuRnVuY3Rpb24gfSk7XHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50c1tmaV0uUGFyYW1ldGVyc1tmdW5jLkluZGV4XS5EZWZhdWx0ID0gZnVuYy52YWx1ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuYnVpbGRGdW5jdGlvbkFycmF5KClcclxuXHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoRWxlbWVudHMoKVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBhZGRGdW5jdGlvblNlZ21lbnQoKSB7XHJcbiAgICAgICAgdmFyIGZ1bmMgPSBGdW5jdGlvbkxpc3RbZXZlbnQudGFyZ2V0Wyd0ZXh0J11dO1xyXG5cclxuICAgICAgICBpZiAoZnVuYy5GdW5jdGlvbiA9PSAnU2xpY2UnKSB7XHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50c1swXS5QYXJhbWV0ZXJzLnVuc2hpZnQoZnVuYy5QYXJhbWV0ZXJzWzBdKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnRzLnVuc2hpZnQoSlNPTi5wYXJzZShKU09OLnN0cmluZ2lmeShmdW5jKSkpO1xyXG4gICAgICAgIHRoaXMuYnVpbGRGdW5jdGlvbkFycmF5KCk7XHJcblxyXG4gICAgICAgIC8vIHJlc2V0IHRoZSArIGJ1dHRvblxyXG4gICAgICAgIHZhciBwbHVzQnV0dG9uID0gdGhpcy51aVNlZ21lbnRTcnYubmV3UGx1c0J1dHRvbigpXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnQudmFsdWUgPSBwbHVzQnV0dG9uLnZhbHVlXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnQuaHRtbCA9IHBsdXNCdXR0b24uaHRtbFxyXG5cclxuICAgICAgICB0aGlzLnNldFRhcmdldFdpdGhFbGVtZW50cygpXHJcbiAgICB9XHJcblxyXG4gICAgYnVpbGRGdW5jdGlvbkFycmF5KCkge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICBjdHJsLmZ1bmN0aW9ucyA9IFtdO1xyXG5cclxuICAgICAgICBpZiAodGhpcy5mdW5jdGlvblNlZ21lbnRzLmxlbmd0aCA9PSAwKSByZXR1cm47XHJcblxyXG4gICAgICAgIF8uZWFjaChjdHJsLmZ1bmN0aW9uU2VnbWVudHMsIGZ1bmN0aW9uIChlbGVtZW50LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICB2YXIgbmV3RWxlbWVudCA9IGN0cmwudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoZWxlbWVudC5GdW5jdGlvbilcclxuICAgICAgICAgICAgbmV3RWxlbWVudC5UeXBlID0gJ0Z1bmN0aW9uJztcclxuICAgICAgICAgICAgbmV3RWxlbWVudC5GdW5jdGlvbiA9IGVsZW1lbnQuRnVuY3Rpb247XHJcblxyXG4gICAgICAgICAgICBjdHJsLmZ1bmN0aW9ucy5wdXNoKG5ld0VsZW1lbnQpXHJcblxyXG4gICAgICAgICAgICBpZiAobmV3RWxlbWVudC52YWx1ZSA9PSAnU2V0JyB8fCBuZXdFbGVtZW50LnZhbHVlID09ICdTbGljZScpIHJldHVybjtcclxuXHJcbiAgICAgICAgICAgIHZhciBvcGVyYXRvciA9IGN0cmwudWlTZWdtZW50U3J2Lm5ld09wZXJhdG9yKCcoJyk7XHJcbiAgICAgICAgICAgIG9wZXJhdG9yLlR5cGUgPSAnT3BlcmF0b3InO1xyXG4gICAgICAgICAgICBjdHJsLmZ1bmN0aW9ucy5wdXNoKG9wZXJhdG9yKTtcclxuXHJcbiAgICAgICAgICAgIF8uZWFjaChlbGVtZW50LlBhcmFtZXRlcnMsIGZ1bmN0aW9uIChwYXJhbSwgaSwgaikge1xyXG4gICAgICAgICAgICAgICAgdmFyIGQgPSBjdHJsLnVpU2VnbWVudFNydi5uZXdGYWtlKHBhcmFtLkRlZmF1bHQudG9TdHJpbmcoKSk7XHJcbiAgICAgICAgICAgICAgICBkLlR5cGUgPSBwYXJhbS5UeXBlO1xyXG4gICAgICAgICAgICAgICAgZC5GdW5jdGlvbiA9IGVsZW1lbnQuRnVuY3Rpb247XHJcbiAgICAgICAgICAgICAgICBkLkRlc2NyaXB0aW9uID0gcGFyYW0uRGVzY3JpcHRpb247XHJcbiAgICAgICAgICAgICAgICBkLkluZGV4ID0gaTtcclxuICAgICAgICAgICAgICAgIGN0cmwuZnVuY3Rpb25zLnB1c2goZCk7XHJcblxyXG4gICAgICAgICAgICAgICAgdmFyIG9wZXJhdG9yID0gY3RybC51aVNlZ21lbnRTcnYubmV3T3BlcmF0b3IoJywnKTtcclxuICAgICAgICAgICAgICAgIG9wZXJhdG9yLlR5cGUgPSAnT3BlcmF0b3InO1xyXG4gICAgICAgICAgICAgICAgY3RybC5mdW5jdGlvbnMucHVzaChvcGVyYXRvcik7XHJcbiAgICAgICAgICAgIH0pXHJcblxyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICB2YXIgcXVlcnkgPSBjdHJsLnVpU2VnbWVudFNydi5uZXdDb25kaXRpb24oJ1FVRVJZJyk7XHJcbiAgICAgICAgcXVlcnkuVHlwZSA9ICdRdWVyeSc7XHJcbiAgICAgICAgY3RybC5mdW5jdGlvbnMucHVzaChxdWVyeSk7XHJcblxyXG4gICAgICAgIGZvciAodmFyIGkgaW4gY3RybC5mdW5jdGlvblNlZ21lbnRzKSB7XHJcbiAgICAgICAgICAgIGlmIChjdHJsLmZ1bmN0aW9uU2VnbWVudHNbaV0uRnVuY3Rpb24gIT0gJ1NldCcgJiYgY3RybC5mdW5jdGlvblNlZ21lbnRzW2ldLkZ1bmN0aW9uICE9ICdTbGljZScpIHtcclxuICAgICAgICAgICAgICAgIHZhciBvcGVyYXRvciA9IGN0cmwudWlTZWdtZW50U3J2Lm5ld09wZXJhdG9yKCcpJyk7XHJcbiAgICAgICAgICAgICAgICBvcGVyYXRvci5UeXBlID0gJ09wZXJhdG9yJztcclxuICAgICAgICAgICAgICAgIGN0cmwuZnVuY3Rpb25zLnB1c2gob3BlcmF0b3IpO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgIH1cclxuXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0Qm9vbGVhbnMoKSB7XHJcbiAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZShCb29sZWFucy5tYXAodmFsdWUgPT4geyByZXR1cm4gdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCh2YWx1ZSkgfSkpO1xyXG4gICAgfVxyXG5cclxuICAgIGdldEFuZ2xlVW5pdHMoKSB7XHJcbiAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZShBbmdsZVVuaXRzLm1hcCh2YWx1ZSA9PiB7IHJldHVybiB0aGlzLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KHZhbHVlKSB9KSk7XHJcbiAgICB9XHJcblxyXG4gICAgZ2V0VGltZVNlbGVjdCgpIHtcclxuICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKFRpbWVVbml0cy5tYXAodmFsdWUgPT4geyByZXR1cm4gdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCh2YWx1ZSkgfSkpO1xyXG4gICAgfVxyXG5cclxuICAgIGlucHV0Q2hhbmdlKGZ1bmMsIGluZGV4KSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIGNsZWFyVGltZW91dCh0aGlzLnR5cGluZ1RpbWVyKTtcclxuICAgICAgICB0aGlzLnR5cGluZ1RpbWVyID0gZ2xvYmFsLnNldFRpbWVvdXQoZnVuY3Rpb24gKCkgeyBjdHJsLmZ1bmN0aW9uVmFsdWVDaGFuZ2VkKGZ1bmMsIGluZGV4KSB9LCAzMDAwKTtcclxuICAgICAgICBldmVudC50YXJnZXRbJ2ZvY3VzJ10oKTtcclxuXHJcbiAgICB9XHJcblxyXG59IiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIG9wZW5IaXN0b3JpYW5GaWx0ZXJFeHByZXNzaW9uX2N0cmwudHMgLSBHYnRjXHJcbi8vXHJcbi8vICBDb3B5cmlnaHQgwqkgMjAxNywgR3JpZCBQcm90ZWN0aW9uIEFsbGlhbmNlLiAgQWxsIFJpZ2h0cyBSZXNlcnZlZC5cclxuLy9cclxuLy8gIExpY2Vuc2VkIHRvIHRoZSBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UgKEdQQSkgdW5kZXIgb25lIG9yIG1vcmUgY29udHJpYnV0b3IgbGljZW5zZSBhZ3JlZW1lbnRzLiBTZWVcclxuLy8gIHRoZSBOT1RJQ0UgZmlsZSBkaXN0cmlidXRlZCB3aXRoIHRoaXMgd29yayBmb3IgYWRkaXRpb25hbCBpbmZvcm1hdGlvbiByZWdhcmRpbmcgY29weXJpZ2h0IG93bmVyc2hpcC5cclxuLy8gIFRoZSBHUEEgbGljZW5zZXMgdGhpcyBmaWxlIHRvIHlvdSB1bmRlciB0aGUgTUlUIExpY2Vuc2UgKE1JVCksIHRoZSBcIkxpY2Vuc2VcIjsgeW91IG1heSBub3QgdXNlIHRoaXNcclxuLy8gIGZpbGUgZXhjZXB0IGluIGNvbXBsaWFuY2Ugd2l0aCB0aGUgTGljZW5zZS4gWW91IG1heSBvYnRhaW4gYSBjb3B5IG9mIHRoZSBMaWNlbnNlIGF0OlxyXG4vL1xyXG4vLyAgICAgIGh0dHA6Ly9vcGVuc291cmNlLm9yZy9saWNlbnNlcy9NSVRcclxuLy9cclxuLy8gIFVubGVzcyBhZ3JlZWQgdG8gaW4gd3JpdGluZywgdGhlIHN1YmplY3Qgc29mdHdhcmUgZGlzdHJpYnV0ZWQgdW5kZXIgdGhlIExpY2Vuc2UgaXMgZGlzdHJpYnV0ZWQgb24gYW5cclxuLy8gIFwiQVMtSVNcIiBCQVNJUywgV0lUSE9VVCBXQVJSQU5USUVTIE9SIENPTkRJVElPTlMgT0YgQU5ZIEtJTkQsIGVpdGhlciBleHByZXNzIG9yIGltcGxpZWQuIFJlZmVyIHRvIHRoZVxyXG4vLyAgTGljZW5zZSBmb3IgdGhlIHNwZWNpZmljIGxhbmd1YWdlIGdvdmVybmluZyBwZXJtaXNzaW9ucyBhbmQgbGltaXRhdGlvbnMuXHJcbi8vXHJcbi8vICBDb2RlIE1vZGlmaWNhdGlvbiBIaXN0b3J5OlxyXG4vLyAgLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyAgMTIvMTIvMjAxNyAtIEJpbGx5IEVybmVzdFxyXG4vLyAgICAgICBHZW5lcmF0ZWQgb3JpZ2luYWwgdmVyc2lvbiBvZiBzb3VyY2UgY29kZS5cclxuLy9cclxuLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8vIDxyZWZlcmVuY2UgcGF0aD1cIi4uL21vZHVsZS5kLnRzXCIvPlxyXG5cclxuaW1wb3J0IHsgRnVuY3Rpb25MaXN0LCBCb29sZWFucywgQW5nbGVVbml0cywgVGltZVVuaXRzLCBXaGVyZU9wZXJhdG9ycyB9IGZyb20gJy4vLi4vanMvb3Blbkhpc3RvcmlhbkNvbnN0YW50cydcclxuXHJcbmltcG9ydCBPcGVuSGlzdG9yaWFuRGF0YVNvdXJjZSBmcm9tICcuLi9vcGVuSGlzdG9yaWFuRGF0YXNvdXJjZSc7XHJcbmltcG9ydCB7IFBhbmVsQ3RybCB9IGZyb20gJ2dyYWZhbmEtc2RrLW1vY2tzL2FwcC9wbHVnaW5zL3Nkayc7XHJcbmltcG9ydCB7IFF1ZXJ5Q3RybCB9IGZyb20gJ2dyYWZhbmEtc2RrLW1vY2tzL2FwcC9wbHVnaW5zL3Nkayc7XHJcbmRlY2xhcmUgdmFyIF86IGFueTtcclxuZGVjbGFyZSB2YXIgJDogYW55O1xyXG5cclxuZXhwb3J0IGRlZmF1bHQgY2xhc3MgT3Blbkhpc3RvcmlhbkZpbHRlckV4cHJlc3Npb25DdHJsIHtcclxuICAgIHdoZXJlczogQXJyYXk8aVNlZ21lbnQ+O1xyXG4gICAgZnVuY3Rpb25TZWdtZW50czogQXJyYXk8aUZ1bmN0aW9uU2VnbWVudD47XHJcbiAgICB0b3BOU2VnbWVudDogbnVtYmVyO1xyXG4gICAgZnVuY3Rpb25zOiBBcnJheTxpRnVuY3Rpb25TZWdtZW50PjtcclxuICAgIG9yZGVyQnlzOiBBcnJheTxpU2VnbWVudD47XHJcbiAgICB3aGVyZVNlZ21lbnQ6IGlTZWdtZW50O1xyXG4gICAgZmlsdGVyU2VnbWVudDogaVNlZ21lbnQ7XHJcbiAgICBvcmRlckJ5U2VnbWVudDogaVNlZ21lbnQ7XHJcbiAgICBmdW5jdGlvblNlZ21lbnQ6IGlGdW5jdGlvblNlZ21lbnQ7XHJcbiAgICB0eXBpbmdUaW1lcjogTm9kZUpTLlRpbWVvdXQ7XHJcbiAgICB0YXJnZXQ6IGlUYXJnZXQ7XHJcbiAgICBkYXRhc291cmNlOiBPcGVuSGlzdG9yaWFuRGF0YVNvdXJjZTtcclxuICAgIHBhbmVsQ3RybDogUGFuZWxDdHJsO1xyXG4gICAgJHNjb3BlOiBhbnk7XHJcbiAgICBjb25zdHJ1Y3Rvcigkc2NvcGUsICRpbmplY3RvciwgcHJpdmF0ZSB1aVNlZ21lbnRTcnY6IGFueSkge1xyXG4gICAgICAgIHRoaXMudWlTZWdtZW50U3J2ID0gdWlTZWdtZW50U3J2O1xyXG4gICAgICAgIHRoaXMuJHNjb3BlID0gJHNjb3BlO1xyXG4gICAgICAgIHRoaXMudGFyZ2V0ID0gJHNjb3BlLnRhcmdldDtcclxuICAgICAgICB0aGlzLmRhdGFzb3VyY2UgPSAkc2NvcGUuZGF0YXNvdXJjZTtcclxuICAgICAgICB0aGlzLnBhbmVsQ3RybCA9ICRzY29wZS5wYW5lbDtcclxuXHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICB0aGlzLndoZXJlcyA9ICh0aGlzLnRhcmdldC53aGVyZXMgPT0gdW5kZWZpbmVkID8gW10gOiB0aGlzLnRhcmdldC53aGVyZXMubWFwKGZ1bmN0aW9uIChhKSB7XHJcbiAgICAgICAgICAgIGlmIChhLnR5cGUgPT0gJ29wZXJhdG9yJykgcmV0dXJuIGN0cmwudWlTZWdtZW50U3J2Lm5ld09wZXJhdG9yKGEudGV4dCk7XHJcbiAgICAgICAgICAgIGVsc2UgaWYgKGEudHlwZSA9PSAnY29uZGl0aW9uJykgcmV0dXJuIGN0cmwudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbihhLnRleHQpO1xyXG4gICAgICAgICAgICBlbHNlIHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KGEudmFsdWUpO1xyXG4gICAgICAgIH0pKTtcclxuICAgICAgICB0aGlzLmZ1bmN0aW9uU2VnbWVudHMgPSAodGhpcy50YXJnZXQuZnVuY3Rpb25TZWdtZW50cyA9PSB1bmRlZmluZWQgPyBbXSA6IHRoaXMudGFyZ2V0LmZ1bmN0aW9uU2VnbWVudHMpO1xyXG4gICAgICAgIHRoaXMudG9wTlNlZ21lbnQgPSAodGhpcy50YXJnZXQudG9wTlNlZ21lbnQgPT0gdW5kZWZpbmVkID8gbnVsbCA6IHRoaXMudGFyZ2V0LnRvcE5TZWdtZW50KTtcclxuICAgICAgICB0aGlzLmZ1bmN0aW9ucyA9IFtdO1xyXG4gICAgICAgIHRoaXMub3JkZXJCeXMgPSAodGhpcy50YXJnZXQub3JkZXJCeXMgPT0gdW5kZWZpbmVkID8gW10gOiB0aGlzLnRhcmdldC5vcmRlckJ5cy5tYXAoZnVuY3Rpb24gKGEpIHtcclxuICAgICAgICAgICAgaWYgKGEudHlwZSA9PSAnY29uZGl0aW9uJylcclxuICAgICAgICAgICAgICAgIHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdDb25kaXRpb24oYS52YWx1ZSk7XHJcbiAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgIHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KGEudmFsdWUpO1xyXG4gICAgICAgIH0pKTtcclxuICAgICAgICB0aGlzLndoZXJlU2VnbWVudCA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld1BsdXNCdXR0b24oKTtcclxuICAgICAgICB0aGlzLmZpbHRlclNlZ21lbnQgPSAodGhpcy50YXJnZXQuZmlsdGVyU2VnbWVudCA9PSB1bmRlZmluZWQgPyB0aGlzLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KCdBY3RpdmVNZWFzdXJlbWVudHMnKSA6IHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQodGhpcy50YXJnZXQuZmlsdGVyU2VnbWVudC52YWx1ZSkpO1xyXG4gICAgICAgIHRoaXMub3JkZXJCeVNlZ21lbnQgPSB0aGlzLnVpU2VnbWVudFNydi5uZXdQbHVzQnV0dG9uKCk7XHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnQgPSB0aGlzLnVpU2VnbWVudFNydi5uZXdQbHVzQnV0dG9uKCk7XHJcbiAgICAgICAgdGhpcy50eXBpbmdUaW1lcjtcclxuXHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQuc2VnbWVudHM7XHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQudGFyZ2V0VGV4dDtcclxuICAgICAgICB0aGlzLnNldFRhcmdldFdpdGhRdWVyeSgpXHJcbiAgICB9XHJcblxyXG5cclxuICAgIHNldFRhcmdldFdpdGhRdWVyeSgpIHtcclxuICAgICAgICBpZiAodGhpcy53aGVyZXMubGVuZ3RoID09IDApIHtcclxuICAgICAgICAgICAgdGhpcy50YXJnZXQudGFyZ2V0ID0gJyc7XHJcbiAgICAgICAgICAgIHRoaXMucGFuZWxDdHJsLnJlZnJlc2goKVxyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICB2YXIgZmlsdGVyID0gdGhpcy5maWx0ZXJTZWdtZW50LnZhbHVlICsgJyAnO1xyXG4gICAgICAgIHZhciB0b3BuID0gKHRoaXMudG9wTlNlZ21lbnQgPyAnVE9QICcgKyB0aGlzLnRvcE5TZWdtZW50ICsgJyAnIDogJycpO1xyXG4gICAgICAgIHZhciB3aGVyZSA9ICdXSEVSRSAnO1xyXG5cclxuICAgICAgICBfLmVhY2godGhpcy53aGVyZXMsIGZ1bmN0aW9uIChlbGVtZW50LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICB3aGVyZSArPSBlbGVtZW50LnZhbHVlICsgJyAnXHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIHZhciBvcmRlcmJ5ID0gJyc7XHJcbiAgICAgICAgXy5lYWNoKHRoaXMub3JkZXJCeXMsIGZ1bmN0aW9uIChlbGVtZW50LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICBvcmRlcmJ5ICs9IChpbmRleCA9PSAwID8gJ09SREVSIEJZICcgOiAnJykgKyBlbGVtZW50LnZhbHVlICsgKGVsZW1lbnQudHlwZSA9PSAnY29uZGl0aW9uJyAmJiBpbmRleCA8IGxpc3QubGVuZ3RoIC0gMSA/ICcsJyA6ICcnKSArICcgJztcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgdmFyIHF1ZXJ5ID0gJ0ZJTFRFUiAnICsgdG9wbiArIGZpbHRlciArIHdoZXJlICsgb3JkZXJieTtcclxuICAgICAgICB2YXIgZnVuY3Rpb25zID0gJyc7XHJcblxyXG4gICAgICAgIF8uZWFjaCh0aGlzLmZ1bmN0aW9ucywgZnVuY3Rpb24gKGVsZW1lbnQsIGluZGV4LCBsaXN0KSB7XHJcbiAgICAgICAgICAgIGlmIChlbGVtZW50LnZhbHVlID09ICdRVUVSWScpIGZ1bmN0aW9ucyArPSBxdWVyeTtcclxuICAgICAgICAgICAgZWxzZSBmdW5jdGlvbnMgKz0gZWxlbWVudC52YWx1ZTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgdGhpcy50YXJnZXQudGFyZ2V0ID0gKGZ1bmN0aW9ucyAhPSBcIlwiID8gZnVuY3Rpb25zIDogcXVlcnkpO1xyXG4gICAgICAgIHRoaXMudGFyZ2V0LnRvcE5TZWdtZW50ID0gdGhpcy50b3BOU2VnbWVudDtcclxuICAgICAgICB0aGlzLnRhcmdldC5maWx0ZXJTZWdtZW50ID0gdGhpcy5maWx0ZXJTZWdtZW50O1xyXG4gICAgICAgIHRoaXMudGFyZ2V0Lm9yZGVyQnlzID0gdGhpcy5vcmRlckJ5cztcclxuICAgICAgICB0aGlzLnRhcmdldC53aGVyZXMgPSB0aGlzLndoZXJlcztcclxuICAgICAgICB0aGlzLnRhcmdldC5mdW5jdGlvblNlZ21lbnRzID0gdGhpcy5mdW5jdGlvblNlZ21lbnRzO1xyXG4gICAgICAgIHRoaXMudGFyZ2V0LnF1ZXJ5VHlwZSA9ICdGaWx0ZXIgRXhwcmVzc2lvbic7XHJcbiAgICAgICAgdGhpcy5wYW5lbEN0cmwucmVmcmVzaCgpXHJcblxyXG4gICAgfVxyXG5cclxuICAgIC8vICNyZWdpb24gVE9QIE5cclxuICAgIHRvcE5WYWx1ZUNoYW5nZWQoKSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICBjbGVhclRpbWVvdXQoY3RybC50eXBpbmdUaW1lcik7XHJcbiAgICAgICAgY3RybC50eXBpbmdUaW1lciA9IGdsb2JhbC5zZXRUaW1lb3V0KGZ1bmN0aW9uICgpIHsgY3RybC5zZXRUYXJnZXRXaXRoUXVlcnkoKSB9LCAxMDAwKTtcclxuICAgICAgICBldmVudC50YXJnZXRbJ2ZvY3VzJ10oKTtcclxuXHJcbiAgICB9XHJcbiAgICAvLyAjZW5kcmVnaW9uXHJcblxyXG4gICAgLy8gI3JlZ2lvbiBXaGVyZXNcclxuICAgIGdldFdoZXJlc1RvRWRpdCh3aGVyZSwgaW5kZXgpIHtcclxuXHJcbiAgICAgICAgaWYgKHdoZXJlLnR5cGUgPT09ICdvcGVyYXRvcicpIHtcclxuICAgICAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZSh0aGlzLnVpU2VnbWVudFNydi5uZXdPcGVyYXRvcnMoV2hlcmVPcGVyYXRvcnMpKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSBpZiAod2hlcmUudHlwZSA9PT0gJ3ZhbHVlJykge1xyXG4gICAgICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKG51bGwpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlIGlmICh3aGVyZS50eXBlID09PSAnY29uZGl0aW9uJykge1xyXG4gICAgICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKFt0aGlzLnVpU2VnbWVudFNydi5uZXdDb25kaXRpb24oJ0FORCcpLCB0aGlzLnVpU2VnbWVudFNydi5uZXdDb25kaXRpb24oJ09SJyldKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgIHJldHVybiB0aGlzLmRhdGFzb3VyY2Uud2hlcmVGaW5kUXVlcnkodGhpcy5maWx0ZXJTZWdtZW50LnZhbHVlKS50aGVuKGRhdGEgPT4ge1xyXG4gICAgICAgICAgICAgICAgdmFyIGFsdFNlZ21lbnRzID0gXy5tYXAoZGF0YSwgaXRlbSA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoeyB2YWx1ZTogaXRlbS50ZXh0LCBleHBhbmRhYmxlOiBpdGVtLmV4cGFuZGFibGUgfSlcclxuICAgICAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICAgICAgYWx0U2VnbWVudHMuc29ydCgoYSwgYikgPT4ge1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChhLnZhbHVlIDwgYi52YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIC0xO1xyXG4gICAgICAgICAgICAgICAgICAgIGlmIChhLnZhbHVlID4gYi52YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICAgICAgcmV0dXJuIDE7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIDA7XHJcbiAgICAgICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgICAgIGFsdFNlZ21lbnRzLnVuc2hpZnQodGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCgnLVJFTU9WRS0nKSk7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gYWx0U2VnbWVudHM7XHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICB9XHJcblxyXG4gICAgd2hlcmVWYWx1ZUNoYW5nZWQod2hlcmUsIGluZGV4KSB7XHJcblxyXG4gICAgICAgIGlmICh3aGVyZS52YWx1ZSA9PSBcIi1SRU1PVkUtXCIpIHtcclxuICAgICAgICAgICAgaWYgKGluZGV4ID09IDAgJiYgdGhpcy53aGVyZXMubGVuZ3RoID4gMyAmJiB0aGlzLndoZXJlc1tpbmRleCArIDNdLnR5cGUgPT0gJ2NvbmRpdGlvbicpXHJcbiAgICAgICAgICAgICAgICB0aGlzLndoZXJlcy5zcGxpY2UoaW5kZXgsIDQpXHJcbiAgICAgICAgICAgIGVsc2UgaWYgKGluZGV4ID4gMCAmJiB0aGlzLndoZXJlc1tpbmRleCAtIDFdLnR5cGUgPT0gJ2NvbmRpdGlvbicpXHJcbiAgICAgICAgICAgICAgICB0aGlzLndoZXJlcy5zcGxpY2UoaW5kZXggLSAxLCA0KVxyXG4gICAgICAgICAgICBlbHNlXHJcbiAgICAgICAgICAgICAgICB0aGlzLndoZXJlcy5zcGxpY2UoaW5kZXgsIDMpXHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBpZiAod2hlcmUudHlwZSA9PSAnb3BlcmF0b3InKVxyXG4gICAgICAgICAgICB0aGlzLndoZXJlc1tpbmRleF0gPSB0aGlzLnVpU2VnbWVudFNydi5uZXdPcGVyYXRvcih3aGVyZS52YWx1ZSlcclxuICAgICAgICBlbHNlIGlmICh3aGVyZS50eXBlID09ICdjb25kaXRpb24nKVxyXG4gICAgICAgICAgICB0aGlzLndoZXJlc1tpbmRleF0gPSB0aGlzLnVpU2VnbWVudFNydi5uZXdDb25kaXRpb24od2hlcmUudmFsdWUpXHJcbiAgICAgICAgZWxzZSBpZiAod2hlcmUudHlwZSA9PSAndmFsdWUnICYmICEkLmlzTnVtZXJpYyh3aGVyZS52YWx1ZSkgJiYgd2hlcmUudmFsdWUudG9VcHBlckNhc2UoKSAhPSAnTlVMTCcpXHJcbiAgICAgICAgICAgIHRoaXMud2hlcmVzW2luZGV4XSA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoXCInXCIgKyB3aGVyZS52YWx1ZSArIFwiJ1wiKTtcclxuXHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoUXVlcnkoKTtcclxuICAgIH1cclxuXHJcbiAgICBnZXRXaGVyZXNUb0FkZE5ldygpIHtcclxuICAgICAgICB2YXIgY3RybCA9IHRoaXM7XHJcbiAgICAgICAgcmV0dXJuIHRoaXMuZGF0YXNvdXJjZS53aGVyZUZpbmRRdWVyeShjdHJsLmZpbHRlclNlZ21lbnQudmFsdWUpLnRoZW4oZGF0YSA9PiB7XHJcbiAgICAgICAgICAgIHZhciBhbHRTZWdtZW50cyA9IF8ubWFwKGRhdGEsIGl0ZW0gPT4ge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIGN0cmwudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoeyB2YWx1ZTogaXRlbS50ZXh0LCBleHBhbmRhYmxlOiBpdGVtLmV4cGFuZGFibGUgfSlcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIGFsdFNlZ21lbnRzLnNvcnQoKGEsIGIpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChhLnZhbHVlIDwgYi52YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gLTE7XHJcbiAgICAgICAgICAgICAgICBpZiAoYS52YWx1ZSA+IGIudmFsdWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIDE7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gMDtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIHJldHVybiBhbHRTZWdtZW50c1xyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIGFkZFdoZXJlKCkge1xyXG4gICAgICAgIGlmICh0aGlzLndoZXJlcy5sZW5ndGggPiAwKVxyXG4gICAgICAgICAgICB0aGlzLndoZXJlcy5wdXNoKHRoaXMudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbignQU5EJykpO1xyXG5cclxuICAgICAgICB0aGlzLndoZXJlcy5wdXNoKHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoZXZlbnQudGFyZ2V0Wyd0ZXh0J10pKTtcclxuICAgICAgICB0aGlzLndoZXJlcy5wdXNoKHRoaXMudWlTZWdtZW50U3J2Lm5ld09wZXJhdG9yKCdOT1QgTElLRScpKTtcclxuICAgICAgICB0aGlzLndoZXJlcy5wdXNoKHRoaXMudWlTZWdtZW50U3J2Lm5ld0Zha2UoXCInJ1wiLCAndmFsdWUnLCAncXVlcnktc2VnbWVudC12YWx1ZScpKTtcclxuXHJcbiAgICAgICAgLy8gcmVzZXQgdGhlICsgYnV0dG9uXHJcbiAgICAgICAgdmFyIHBsdXNCdXR0b24gPSB0aGlzLnVpU2VnbWVudFNydi5uZXdQbHVzQnV0dG9uKClcclxuICAgICAgICB0aGlzLndoZXJlU2VnbWVudC52YWx1ZSA9IHBsdXNCdXR0b24udmFsdWVcclxuICAgICAgICB0aGlzLndoZXJlU2VnbWVudC5odG1sID0gcGx1c0J1dHRvbi5odG1sXHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoUXVlcnkoKTtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgLy8gI2VuZHJlZ2lvblxyXG5cclxuICAgIC8vICNyZWdpb24gRmlsdGVyc1xyXG4gICAgZ2V0RmlsdGVyVG9FZGl0KCkge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICByZXR1cm4gdGhpcy5kYXRhc291cmNlLmZpbHRlckZpbmRRdWVyeSgpLnRoZW4oZGF0YSA9PiB7XHJcbiAgICAgICAgICAgIHZhciBhbHRTZWdtZW50cyA9IF8ubWFwKGRhdGEsIGl0ZW0gPT4ge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIGN0cmwudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoeyB2YWx1ZTogaXRlbS50ZXh0LCBleHBhbmRhYmxlOiBpdGVtLmV4cGFuZGFibGUgfSlcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgICAgIGFsdFNlZ21lbnRzLnNvcnQoKGEsIGIpID0+IHtcclxuICAgICAgICAgICAgICAgIGlmIChhLnZhbHVlIDwgYi52YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gLTE7XHJcbiAgICAgICAgICAgICAgICBpZiAoYS52YWx1ZSA+IGIudmFsdWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIDE7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gMDtcclxuICAgICAgICAgICAgfSk7XHJcblxyXG4gICAgICAgICAgICByZXR1cm4gYWx0U2VnbWVudHM7XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgZmlsdGVyVmFsdWVDaGFuZ2VkKCkge1xyXG4gICAgICAgIC8vdGhpcy50YXJnZXQucG9saWN5ID0gdGhpcy50b3BOU2VnbWVudDtcclxuICAgICAgICB0aGlzLm9yZGVyQnlTZWdtZW50ID0gdGhpcy51aVNlZ21lbnRTcnYubmV3UGx1c0J1dHRvbigpO1xyXG4gICAgICAgIHRoaXMud2hlcmVzID0gW107XHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoUXVlcnkoKTtcclxuXHJcbiAgICAgICAgdGhpcy5wYW5lbEN0cmwucmVmcmVzaCgpO1xyXG4gICAgfVxyXG5cclxuICAgIC8vICNlbmRyZWdpb25cclxuXHJcbiAgICAvLyAjcmVnaW9uIE9yZGVyQnlzXHJcbiAgICBnZXRPcmRlckJ5c1RvQWRkTmV3KCkge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICByZXR1cm4gdGhpcy5kYXRhc291cmNlLm9yZGVyQnlGaW5kUXVlcnkoY3RybC5maWx0ZXJTZWdtZW50LnZhbHVlKS50aGVuKGRhdGEgPT4ge1xyXG4gICAgICAgICAgICB2YXIgYWx0U2VnbWVudHMgPSBfLm1hcChkYXRhLCBpdGVtID0+IHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KHsgdmFsdWU6IGl0ZW0udGV4dCwgZXhwYW5kYWJsZTogaXRlbS5leHBhbmRhYmxlIH0pXHJcbiAgICAgICAgICAgIH0pO1xyXG4gICAgICAgICAgICBhbHRTZWdtZW50cy5zb3J0KChhLCBiKSA9PiB7XHJcbiAgICAgICAgICAgICAgICBpZiAoYS52YWx1ZSA8IGIudmFsdWUpXHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIC0xO1xyXG4gICAgICAgICAgICAgICAgaWYgKGEudmFsdWUgPiBiLnZhbHVlKVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAxO1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIDA7XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIF8uZmlsdGVyKGFsdFNlZ21lbnRzLCBzZWdtZW50ID0+IHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBfLmZpbmQoY3RybC5vcmRlckJ5cywgeCA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHgudmFsdWUgPT0gc2VnbWVudC52YWx1ZVxyXG4gICAgICAgICAgICAgICAgfSkgPT0gdW5kZWZpbmVkO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBnZXRPcmRlckJ5c1RvRWRpdChvcmRlckJ5KSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIGlmIChvcmRlckJ5LnR5cGUgPT0gJ2NvbmRpdGlvbicpIHJldHVybiBQcm9taXNlLnJlc29sdmUoW3RoaXMudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbignQVNDJyksIHRoaXMudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbignREVTQycpXSk7XHJcbiAgICAgICAgaWYgKG9yZGVyQnkudHlwZSA9PSAnY29uZGl0aW9uJykgcmV0dXJuIFByb21pc2UucmVzb2x2ZShbdGhpcy51aVNlZ21lbnRTcnYubmV3Q29uZGl0aW9uKCdBU0MnKSwgdGhpcy51aVNlZ21lbnRTcnYubmV3Q29uZGl0aW9uKCdERVNDJyldKTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuZGF0YXNvdXJjZS5vcmRlckJ5RmluZFF1ZXJ5KGN0cmwuZmlsdGVyU2VnbWVudC52YWx1ZSkudGhlbihkYXRhID0+IHtcclxuICAgICAgICAgICAgdmFyIGFsdFNlZ21lbnRzID0gXy5tYXAoZGF0YSwgaXRlbSA9PiB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gY3RybC51aVNlZ21lbnRTcnYubmV3U2VnbWVudCh7IHZhbHVlOiBpdGVtLnRleHQsIGV4cGFuZGFibGU6IGl0ZW0uZXhwYW5kYWJsZSB9KVxyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICAgICAgYWx0U2VnbWVudHMuc29ydCgoYSwgYikgPT4ge1xyXG4gICAgICAgICAgICAgICAgaWYgKGEudmFsdWUgPCBiLnZhbHVlKVxyXG4gICAgICAgICAgICAgICAgICAgIHJldHVybiAtMTtcclxuICAgICAgICAgICAgICAgIGlmIChhLnZhbHVlID4gYi52YWx1ZSlcclxuICAgICAgICAgICAgICAgICAgICByZXR1cm4gMTtcclxuICAgICAgICAgICAgICAgIHJldHVybiAwO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGlmIChvcmRlckJ5LnR5cGUgIT09ICdwbHVzLWJ1dHRvbicpXHJcbiAgICAgICAgICAgICAgICBhbHRTZWdtZW50cy51bnNoaWZ0KHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQoJy1SRU1PVkUtJykpO1xyXG5cclxuICAgICAgICAgICAgcmV0dXJuIF8uZmlsdGVyKGFsdFNlZ21lbnRzLCBzZWdtZW50ID0+IHtcclxuICAgICAgICAgICAgICAgIHJldHVybiBfLmZpbmQoY3RybC5vcmRlckJ5cywgeCA9PiB7XHJcbiAgICAgICAgICAgICAgICAgICAgcmV0dXJuIHgudmFsdWUgPT0gc2VnbWVudC52YWx1ZVxyXG4gICAgICAgICAgICAgICAgfSkgPT0gdW5kZWZpbmVkO1xyXG4gICAgICAgICAgICB9KTtcclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBhZGRPcmRlckJ5KCkge1xyXG4gICAgICAgIHRoaXMub3JkZXJCeXMucHVzaCh0aGlzLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KGV2ZW50LnRhcmdldFsndGV4dCddKSk7XHJcbiAgICAgICAgdGhpcy5vcmRlckJ5cy5wdXNoKHRoaXMudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbignQVNDJykpO1xyXG5cclxuICAgICAgICAvLyByZXNldCB0aGUgKyBidXR0b25cclxuICAgICAgICB2YXIgcGx1c0J1dHRvbiA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld1BsdXNCdXR0b24oKVxyXG4gICAgICAgIHRoaXMub3JkZXJCeVNlZ21lbnQudmFsdWUgPSBwbHVzQnV0dG9uLnZhbHVlXHJcbiAgICAgICAgdGhpcy5vcmRlckJ5U2VnbWVudC5odG1sID0gcGx1c0J1dHRvbi5odG1sXHJcblxyXG4gICAgICAgIHRoaXMuc2V0VGFyZ2V0V2l0aFF1ZXJ5KCk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIG9yZGVyQnlWYWx1ZUNoYW5nZWQob3JkZXJCeSwgaW5kZXgpIHtcclxuICAgICAgICBpZiAoZXZlbnQudGFyZ2V0Wyd0ZXh0J10gPT0gXCItUkVNT1ZFLVwiKVxyXG4gICAgICAgICAgICB0aGlzLm9yZGVyQnlzLnNwbGljZShpbmRleCwgMik7XHJcbiAgICAgICAgZWxzZSB7XHJcbiAgICAgICAgICAgIGlmIChvcmRlckJ5LnR5cGUgPT0gJ2NvbmRpdGlvbicpXHJcbiAgICAgICAgICAgICAgICB0aGlzLm9yZGVyQnlzW2luZGV4XSA9IHRoaXMudWlTZWdtZW50U3J2Lm5ld0NvbmRpdGlvbihldmVudC50YXJnZXRbJ3RleHQnXSk7XHJcbiAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgIHRoaXMub3JkZXJCeXNbaW5kZXhdID0gdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudChldmVudC50YXJnZXRbJ3RleHQnXSk7XHJcblxyXG4gICAgICAgIH1cclxuICAgICAgICB0aGlzLnNldFRhcmdldFdpdGhRdWVyeSgpO1xyXG5cclxuICAgIH1cclxuXHJcbiAgICAvLyAjZW5kcmVnaW9uXHJcblxyXG4gICAgLy8gI3JlZ2lvbiBGdW5jdGlvbnNcclxuICAgIGdldEZ1bmN0aW9uc1RvQWRkTmV3KCkge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICB2YXIgYXJyYXkgPSBbXVxyXG4gICAgICAgIF8uZWFjaChPYmplY3Qua2V5cyhGdW5jdGlvbkxpc3QpLCBmdW5jdGlvbiAoZWxlbWVudCwgaW5kZXgsIGxpc3QpIHtcclxuICAgICAgICAgICAgYXJyYXkucHVzaChjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KGVsZW1lbnQpKTtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuZnVuY3Rpb25zLmxlbmd0aCA9PSAwKSBhcnJheSA9IGFycmF5LnNsaWNlKDIsIGFycmF5Lmxlbmd0aCk7XHJcblxyXG4gICAgICAgIGFycmF5LnNvcnQoZnVuY3Rpb24gKGEsIGIpIHtcclxuICAgICAgICAgICAgdmFyIG5hbWVBID0gYS52YWx1ZS50b1VwcGVyQ2FzZSgpOyAvLyBpZ25vcmUgdXBwZXIgYW5kIGxvd2VyY2FzZVxyXG4gICAgICAgICAgICB2YXIgbmFtZUIgPSBiLnZhbHVlLnRvVXBwZXJDYXNlKCk7IC8vIGlnbm9yZSB1cHBlciBhbmQgbG93ZXJjYXNlXHJcbiAgICAgICAgICAgIGlmIChuYW1lQSA8IG5hbWVCKSB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4gLTE7XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICAgICAgaWYgKG5hbWVBID4gbmFtZUIpIHtcclxuICAgICAgICAgICAgICAgIHJldHVybiAxO1xyXG4gICAgICAgICAgICB9XHJcblxyXG4gICAgICAgICAgICAvLyBuYW1lcyBtdXN0IGJlIGVxdWFsXHJcbiAgICAgICAgICAgIHJldHVybiAwO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKF8uZmlsdGVyKGFycmF5LCBzZWdtZW50ID0+IHtcclxuICAgICAgICAgICAgcmV0dXJuIF8uZmluZCh0aGlzLmZ1bmN0aW9ucywgeCA9PiB7XHJcbiAgICAgICAgICAgICAgICByZXR1cm4geC52YWx1ZSA9PSBzZWdtZW50LnZhbHVlO1xyXG4gICAgICAgICAgICB9KSA9PSB1bmRlZmluZWQ7XHJcbiAgICAgICAgfSkpO1xyXG4gICAgfVxyXG5cclxuICAgIGdldEZ1bmN0aW9uc1RvRWRpdChmdW5jLCBpbmRleCk6IGFueSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIHZhciByZW1vdmUgPSBbdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCgnLVJFTU9WRS0nKV07XHJcbiAgICAgICAgaWYgKGZ1bmMudHlwZSA9PSAnT3BlcmF0b3InKSByZXR1cm4gUHJvbWlzZS5yZXNvbHZlKCk7XHJcbiAgICAgICAgZWxzZSBpZiAoZnVuYy52YWx1ZSA9PSAnU2V0JykgcmV0dXJuIFByb21pc2UucmVzb2x2ZShyZW1vdmUpXHJcblxyXG4gICAgICAgIHJldHVybiBQcm9taXNlLnJlc29sdmUocmVtb3ZlKTtcclxuICAgIH1cclxuXHJcbiAgICBmdW5jdGlvblZhbHVlQ2hhbmdlZChmdW5jLCBpbmRleCkge1xyXG4gICAgICAgIHZhciBmdW5jU2VnID0gRnVuY3Rpb25MaXN0W2Z1bmMuRnVuY3Rpb25dO1xyXG5cclxuICAgICAgICBpZiAoZnVuYy52YWx1ZSA9PSBcIi1SRU1PVkUtXCIpIHtcclxuICAgICAgICAgICAgdmFyIGwgPSAxO1xyXG4gICAgICAgICAgICB2YXIgZmkgPSBfLmZpbmRJbmRleCh0aGlzLmZ1bmN0aW9uU2VnbWVudHMsIGZ1bmN0aW9uIChzZWdtZW50KSB7IHJldHVybiBzZWdtZW50LkZ1bmN0aW9uID09IGZ1bmMuRnVuY3Rpb24gfSk7XHJcbiAgICAgICAgICAgIGlmIChmdW5jLkZ1bmN0aW9uID09ICdTbGljZScpXHJcbiAgICAgICAgICAgICAgICB0aGlzLmZ1bmN0aW9uU2VnbWVudHNbZmkgKyAxXS5QYXJhbWV0ZXJzID0gdGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpICsgMV0uUGFyYW1ldGVycy5zbGljZSgxLCB0aGlzLmZ1bmN0aW9uU2VnbWVudHNbZmkgKyAxXS5QYXJhbWV0ZXJzLmxlbmd0aCk7XHJcbiAgICAgICAgICAgIGVsc2UgaWYgKGZpID4gMCAmJiAodGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpIC0gMV0uRnVuY3Rpb24gPT0gJ1NldCcgfHwgdGhpcy5mdW5jdGlvblNlZ21lbnRzW2ZpIC0gMV0uRnVuY3Rpb24gPT0gJ1NsaWNlJykpIHtcclxuICAgICAgICAgICAgICAgIC0tZmk7XHJcbiAgICAgICAgICAgICAgICArK2w7XHJcbiAgICAgICAgICAgIH1cclxuXHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50cy5zcGxpY2UoZmksIGwpO1xyXG4gICAgICAgIH1cclxuICAgICAgICBlbHNlIGlmIChmdW5jLlR5cGUgIT0gJ0Z1bmN0aW9uJykge1xyXG4gICAgICAgICAgICB2YXIgZmkgPSBfLmZpbmRJbmRleCh0aGlzLmZ1bmN0aW9uU2VnbWVudHMsIGZ1bmN0aW9uIChzZWdtZW50KSB7IHJldHVybiBzZWdtZW50LkZ1bmN0aW9uID09IGZ1bmMuRnVuY3Rpb24gfSk7XHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50c1tmaV0uUGFyYW1ldGVyc1tmdW5jLkluZGV4XS5EZWZhdWx0ID0gZnVuYy52YWx1ZTtcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHRoaXMuYnVpbGRGdW5jdGlvbkFycmF5KClcclxuICAgICAgICB0aGlzLnNldFRhcmdldFdpdGhRdWVyeSgpO1xyXG5cclxuICAgIH1cclxuXHJcbiAgICBhZGRGdW5jdGlvblNlZ21lbnQoKSB7XHJcbiAgICAgICAgdmFyIGZ1bmMgPSBGdW5jdGlvbkxpc3RbZXZlbnQudGFyZ2V0Wyd0ZXh0J11dO1xyXG5cclxuICAgICAgICBpZiAoZnVuYy5GdW5jdGlvbiA9PSAnU2xpY2UnKSB7XHJcbiAgICAgICAgICAgIHRoaXMuZnVuY3Rpb25TZWdtZW50c1swXS5QYXJhbWV0ZXJzLnVuc2hpZnQoZnVuYy5QYXJhbWV0ZXJzWzBdKVxyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnRzLnVuc2hpZnQoSlNPTi5wYXJzZShKU09OLnN0cmluZ2lmeShmdW5jKSkpO1xyXG4gICAgICAgIHRoaXMuYnVpbGRGdW5jdGlvbkFycmF5KCk7XHJcblxyXG4gICAgICAgIC8vIHJlc2V0IHRoZSArIGJ1dHRvblxyXG4gICAgICAgIHZhciBwbHVzQnV0dG9uID0gdGhpcy51aVNlZ21lbnRTcnYubmV3UGx1c0J1dHRvbigpXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnQudmFsdWUgPSBwbHVzQnV0dG9uLnZhbHVlXHJcbiAgICAgICAgdGhpcy5mdW5jdGlvblNlZ21lbnQuaHRtbCA9IHBsdXNCdXR0b24uaHRtbFxyXG5cclxuICAgICAgICB0aGlzLnNldFRhcmdldFdpdGhRdWVyeSgpO1xyXG4gICAgfVxyXG5cclxuICAgIGJ1aWxkRnVuY3Rpb25BcnJheSgpIHtcclxuICAgICAgICB2YXIgY3RybCA9IHRoaXM7XHJcbiAgICAgICAgY3RybC5mdW5jdGlvbnMgPSBbXTtcclxuXHJcbiAgICAgICAgaWYgKHRoaXMuZnVuY3Rpb25TZWdtZW50cy5sZW5ndGggPT0gMCkgcmV0dXJuO1xyXG5cclxuICAgICAgICBfLmVhY2goY3RybC5mdW5jdGlvblNlZ21lbnRzLCBmdW5jdGlvbiAoZWxlbWVudDogaUZ1bmN0aW9uU2VnbWVudCwgaW5kZXgsIGxpc3QpIHtcclxuICAgICAgICAgICAgdmFyIG5ld0VsZW1lbnQgPSBjdHJsLnVpU2VnbWVudFNydi5uZXdTZWdtZW50KGVsZW1lbnQuRnVuY3Rpb24pXHJcbiAgICAgICAgICAgIG5ld0VsZW1lbnQuVHlwZSA9ICdGdW5jdGlvbic7XHJcbiAgICAgICAgICAgIG5ld0VsZW1lbnQuRnVuY3Rpb24gPSBlbGVtZW50LkZ1bmN0aW9uO1xyXG5cclxuICAgICAgICAgICAgY3RybC5mdW5jdGlvbnMucHVzaChuZXdFbGVtZW50KVxyXG5cclxuICAgICAgICAgICAgaWYgKG5ld0VsZW1lbnQudmFsdWUgPT0gJ1NldCcgfHwgbmV3RWxlbWVudC52YWx1ZSA9PSAnU2xpY2UnKSByZXR1cm47XHJcblxyXG4gICAgICAgICAgICB2YXIgb3BlcmF0b3IgPSBjdHJsLnVpU2VnbWVudFNydi5uZXdPcGVyYXRvcignKCcpO1xyXG4gICAgICAgICAgICBvcGVyYXRvci5UeXBlID0gJ09wZXJhdG9yJztcclxuICAgICAgICAgICAgY3RybC5mdW5jdGlvbnMucHVzaChvcGVyYXRvcik7XHJcblxyXG4gICAgICAgICAgICBfLmVhY2goZWxlbWVudC5QYXJhbWV0ZXJzLCBmdW5jdGlvbiAocGFyYW0sIGksIGopIHtcclxuICAgICAgICAgICAgICAgIHZhciBkID0gY3RybC51aVNlZ21lbnRTcnYubmV3RmFrZShwYXJhbS5EZWZhdWx0LnRvU3RyaW5nKCkpO1xyXG4gICAgICAgICAgICAgICAgZC5UeXBlID0gcGFyYW0uVHlwZTtcclxuICAgICAgICAgICAgICAgIGQuRnVuY3Rpb24gPSBlbGVtZW50LkZ1bmN0aW9uO1xyXG4gICAgICAgICAgICAgICAgZC5EZXNjcmlwdGlvbiA9IHBhcmFtLkRlc2NyaXB0aW9uO1xyXG4gICAgICAgICAgICAgICAgZC5JbmRleCA9IGk7XHJcbiAgICAgICAgICAgICAgICBjdHJsLmZ1bmN0aW9ucy5wdXNoKGQpO1xyXG5cclxuICAgICAgICAgICAgICAgIHZhciBvcGVyYXRvciA9IGN0cmwudWlTZWdtZW50U3J2Lm5ld09wZXJhdG9yKCcsJyk7XHJcbiAgICAgICAgICAgICAgICBvcGVyYXRvci5UeXBlID0gJ09wZXJhdG9yJztcclxuICAgICAgICAgICAgICAgIGN0cmwuZnVuY3Rpb25zLnB1c2gob3BlcmF0b3IpO1xyXG4gICAgICAgICAgICB9KVxyXG5cclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgdmFyIHF1ZXJ5ID0gY3RybC51aVNlZ21lbnRTcnYubmV3Q29uZGl0aW9uKCdRVUVSWScpO1xyXG4gICAgICAgIHF1ZXJ5LlR5cGUgPSAnUXVlcnknO1xyXG4gICAgICAgIGN0cmwuZnVuY3Rpb25zLnB1c2gocXVlcnkpO1xyXG5cclxuICAgICAgICBmb3IgKHZhciBpIGluIGN0cmwuZnVuY3Rpb25TZWdtZW50cykge1xyXG4gICAgICAgICAgICBpZiAoY3RybC5mdW5jdGlvblNlZ21lbnRzW2ldLkZ1bmN0aW9uICE9ICdTZXQnICYmIGN0cmwuZnVuY3Rpb25TZWdtZW50c1tpXS5GdW5jdGlvbiAhPSAnU2xpY2UnKSB7XHJcbiAgICAgICAgICAgICAgICB2YXIgb3BlcmF0b3IgPSBjdHJsLnVpU2VnbWVudFNydi5uZXdPcGVyYXRvcignKScpO1xyXG4gICAgICAgICAgICAgICAgb3BlcmF0b3IuVHlwZSA9ICdPcGVyYXRvcic7XHJcbiAgICAgICAgICAgICAgICBjdHJsLmZ1bmN0aW9ucy5wdXNoKG9wZXJhdG9yKTtcclxuICAgICAgICAgICAgfVxyXG5cclxuICAgICAgICB9XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGdldEJvb2xlYW5zKCkge1xyXG4gICAgICAgIHJldHVybiBQcm9taXNlLnJlc29sdmUoQm9vbGVhbnMubWFwKHZhbHVlID0+IHsgcmV0dXJuIHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQodmFsdWUpIH0pKTtcclxuICAgIH1cclxuXHJcbiAgICBnZXRBbmdsZVVuaXRzKCkge1xyXG4gICAgICAgIHJldHVybiBQcm9taXNlLnJlc29sdmUoQW5nbGVVbml0cy5tYXAodmFsdWUgPT4geyByZXR1cm4gdGhpcy51aVNlZ21lbnRTcnYubmV3U2VnbWVudCh2YWx1ZSkgfSkpO1xyXG4gICAgfVxyXG5cclxuICAgIGdldFRpbWVTZWxlY3QoKSB7XHJcbiAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZShUaW1lVW5pdHMubWFwKHZhbHVlID0+IHsgcmV0dXJuIHRoaXMudWlTZWdtZW50U3J2Lm5ld1NlZ21lbnQodmFsdWUpIH0pKTtcclxuICAgIH1cclxuXHJcbiAgICBpbnB1dENoYW5nZShmdW5jLCBpbmRleCkge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICBjbGVhclRpbWVvdXQodGhpcy50eXBpbmdUaW1lcik7XHJcbiAgICAgICAgdGhpcy50eXBpbmdUaW1lciA9IGdsb2JhbC5zZXRUaW1lb3V0KGZ1bmN0aW9uICgpIHsgY3RybC5mdW5jdGlvblZhbHVlQ2hhbmdlZChmdW5jLCBpbmRleCkgfSwgMzAwMCk7XHJcbiAgICAgICAgZXZlbnQudGFyZ2V0Wydmb2N1cyddKCk7XHJcblxyXG4gICAgfVxyXG5cclxuICAvLyAjZW5kcmVnaW9uXHJcblxyXG5cclxufSIsIi8vKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXHJcbi8vICBxdWVyeU9wdGlvbnNfY3RybC5qcyAtIEdidGNcclxuLy9cclxuLy8gIENvcHlyaWdodCDvv70gMjAxNywgR3JpZCBQcm90ZWN0aW9uIEFsbGlhbmNlLiAgQWxsIFJpZ2h0cyBSZXNlcnZlZC5cclxuLy9cclxuLy8gIExpY2Vuc2VkIHRvIHRoZSBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UgKEdQQSkgdW5kZXIgb25lIG9yIG1vcmUgY29udHJpYnV0b3IgbGljZW5zZSBhZ3JlZW1lbnRzLiBTZWVcclxuLy8gIHRoZSBOT1RJQ0UgZmlsZSBkaXN0cmlidXRlZCB3aXRoIHRoaXMgd29yayBmb3IgYWRkaXRpb25hbCBpbmZvcm1hdGlvbiByZWdhcmRpbmcgY29weXJpZ2h0IG93bmVyc2hpcC5cclxuLy8gIFRoZSBHUEEgbGljZW5zZXMgdGhpcyBmaWxlIHRvIHlvdSB1bmRlciB0aGUgTUlUIExpY2Vuc2UgKE1JVCksIHRoZSBcIkxpY2Vuc2VcIjsgeW91IG1heSBub3QgdXNlIHRoaXNcclxuLy8gIGZpbGUgZXhjZXB0IGluIGNvbXBsaWFuY2Ugd2l0aCB0aGUgTGljZW5zZS4gWW91IG1heSBvYnRhaW4gYSBjb3B5IG9mIHRoZSBMaWNlbnNlIGF0OlxyXG4vL1xyXG4vLyAgICAgIGh0dHA6Ly9vcGVuc291cmNlLm9yZy9saWNlbnNlcy9NSVRcclxuLy9cclxuLy8gIFVubGVzcyBhZ3JlZWQgdG8gaW4gd3JpdGluZywgdGhlIHN1YmplY3Qgc29mdHdhcmUgZGlzdHJpYnV0ZWQgdW5kZXIgdGhlIExpY2Vuc2UgaXMgZGlzdHJpYnV0ZWQgb24gYW5cclxuLy8gIFwiQVMtSVNcIiBCQVNJUywgV0lUSE9VVCBXQVJSQU5USUVTIE9SIENPTkRJVElPTlMgT0YgQU5ZIEtJTkQsIGVpdGhlciBleHByZXNzIG9yIGltcGxpZWQuIFJlZmVyIHRvIHRoZVxyXG4vLyAgTGljZW5zZSBmb3IgdGhlIHNwZWNpZmljIGxhbmd1YWdlIGdvdmVybmluZyBwZXJtaXNzaW9ucyBhbmQgbGltaXRhdGlvbnMuXHJcbi8vXHJcbi8vICBDb2RlIE1vZGlmaWNhdGlvbiBIaXN0b3J5OlxyXG4vLyAgLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLVxyXG4vLyAgMTAvMzEvMjAxNyAtIEJpbGx5IEVybmVzdFxyXG4vLyAgICAgICBHZW5lcmF0ZWQgb3JpZ2luYWwgdmVyc2lvbiBvZiBzb3VyY2UgY29kZS5cclxuLy9cclxuLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuZGVjbGFyZSB2YXIgXzogYW55O1xyXG5pbXBvcnQgeyBEZWZhdWx0RmxhZ3MgfSBmcm9tICcuLy4uL2pzL29wZW5IaXN0b3JpYW5Db25zdGFudHMnXHJcblxyXG5leHBvcnQgZGVmYXVsdCBjbGFzcyBPcGVuSGlzdG9yaWFuUXVlcnlPcHRpb25zQ3RybHtcclxuICAgIGRhdGFGbGFnczogYW55O1xyXG4gICAgcmV0dXJuOiBhbnk7XHJcbiAgICBmbGFnQXJyYXk6IEFycmF5PGFueT47XHJcbiAgICBpbmNsdWRlQWxhcm06IGFueTtcclxuXHJcbiAgICBjb25zdHJ1Y3Rvcihwcml2YXRlICRzY29wZSxwcml2YXRlICRjb21waWxlKSB7XHJcblxyXG4gICAgICAgIHRoaXMuJHNjb3BlID0gJHNjb3BlO1xyXG4gICAgICAgIHZhciB2YWx1ZSA9IEpTT04ucGFyc2UoSlNPTi5zdHJpbmdpZnkoJHNjb3BlLnJldHVybikpO1xyXG5cclxuICAgICAgICB0aGlzLmRhdGFGbGFncyA9IHRoaXMuaGV4MmZsYWdzKHBhcnNlSW50KHZhbHVlLkV4Y2x1ZGVkKSk7XHJcbiAgICAgICAgdGhpcy5kYXRhRmxhZ3NbJ05vcm1hbCddLlZhbHVlID0gdmFsdWUuTm9ybWFsO1xyXG4gICAgICAgIHRoaXMuaW5jbHVkZUFsYXJtID0gdmFsdWUuQWxhcm1zO1xyXG5cclxuICAgICAgICB0aGlzLnJldHVybiA9ICRzY29wZS5yZXR1cm47XHJcblxyXG4gICAgICAgIHRoaXMuZmxhZ0FycmF5ID0gXy5tYXAoT2JqZWN0LmtleXModGhpcy5kYXRhRmxhZ3MpLCBhID0+IHtcclxuICAgICAgICAgICAgcmV0dXJuIHsga2V5OiBhLCBvcmRlcjogdGhpcy5kYXRhRmxhZ3NbYV0uT3JkZXIgfTtcclxuICAgICAgICB9KS5zb3J0KChhLCBiKSA9PiB7XHJcbiAgICAgICAgICAgIHJldHVybiBhLm9yZGVyIC0gYi5vcmRlcjtcclxuICAgICAgICB9KTtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgY2FsY3VsYXRlRmxhZ3MoZmxhZykge1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICB2YXIgZmxhZ1ZhckV4Y2x1ZGVkID0gY3RybC5yZXR1cm4uRXhjbHVkZWQ7XHJcblxyXG4gICAgICAgIGlmIChmbGFnID09ICdTZWxlY3QgQWxsJykge1xyXG4gICAgICAgICAgICBfLmVhY2goT2JqZWN0LmtleXMoY3RybC5kYXRhRmxhZ3MpLCBmdW5jdGlvbiAoa2V5LCBpbmRleCwgbGlzdCkge1xyXG5cclxuICAgICAgICAgICAgICAgIGlmKGtleSA9PSBcIk5vcm1hbFwiKSBcclxuICAgICAgICAgICAgICAgICAgICBjdHJsLmRhdGFGbGFnc1trZXldLlZhbHVlID0gZmFsc2U7XHJcbiAgICAgICAgICAgICAgICBlbHNlIFxyXG4gICAgICAgICAgICAgICAgICAgIGN0cmwuZGF0YUZsYWdzW2tleV0uVmFsdWUgPSBjdHJsLmRhdGFGbGFnc1snU2VsZWN0IEFsbCddLlZhbHVlO1xyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGlmIChjdHJsLmRhdGFGbGFnc1snU2VsZWN0IEFsbCddLlZhbHVlKSBcclxuICAgICAgICAgICAgICAgIGZsYWdWYXJFeGNsdWRlZCA9IDB4RkZGRkZGRkY7XHJcbiAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgIGZsYWdWYXJFeGNsdWRlZCA9IDA7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIGVsc2Uge1xyXG4gICAgICAgICAgICBjdHJsLmRhdGFGbGFnc1snU2VsZWN0IEFsbCddLlZhbHVlID0gZmFsc2U7XHJcblxyXG4gICAgICAgICAgICBmbGFnVmFyRXhjbHVkZWQgXj0gY3RybC5kYXRhRmxhZ3NbZmxhZ10uRmxhZztcclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIGN0cmwucmV0dXJuLkV4Y2x1ZGVkID0gZmxhZ1ZhckV4Y2x1ZGVkO1xyXG4gICAgICAgIGN0cmwucmV0dXJuLk5vcm1hbCA9IGN0cmwuZGF0YUZsYWdzWydOb3JtYWwnXS5WYWx1ZTtcclxuICAgIH1cclxuXHJcbiAgICBjaGFuZ2VBbGFybXMoKSB7XHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIGN0cmwucmV0dXJuLkFsYXJtcyA9IGN0cmwuaW5jbHVkZUFsYXJtO1xyXG4gICAgfVxyXG5cclxuICAgIGhleDJmbGFncyhoZXgpe1xyXG4gICAgICAgIHZhciBjdHJsID0gdGhpcztcclxuICAgICAgICB2YXIgZmxhZyA9IGhleDtcclxuICAgICAgICB2YXIgZmxhZ3MgPSBKU09OLnBhcnNlKEpTT04uc3RyaW5naWZ5KERlZmF1bHRGbGFncykpO1xyXG5cclxuICAgICAgICBfLmVhY2goT2JqZWN0LmtleXMoZmxhZ3MpLCBmdW5jdGlvbiAoa2V5LCBpbmRleCwgbGlzdCkge1xyXG4gICAgICAgICAgICBpZiAoa2V5ID09ICdTZWxlY3QgQWxsJykgcmV0dXJuO1xyXG4gICAgICAgICAgICBcclxuICAgICAgICAgICAgZmxhZ3Nba2V5XS5WYWx1ZSA9IChmbGFnc1trZXldLkZsYWcgJiBmbGFnKSAhPSAwXHJcbiAgICAgICAgfSk7XHJcbiAgICAgICAgXHJcbiAgICAgICAgcmV0dXJuIGZsYWdzO1xyXG4gICAgfVxyXG59XHJcbiIsIi8vKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXHJcbi8vICBxdWVyeV9jdHJsLmpzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IO+/vSAyMDE3LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAxMS8wMi8yMDE3IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5cclxuaW1wb3J0IHsgUXVlcnlDdHJsIH0gZnJvbSAnLi4vbm9kZV9tb2R1bGVzL2dyYWZhbmEtc2RrLW1vY2tzL2FwcC9wbHVnaW5zL3NkaydcclxuaW1wb3J0ICcuLy4uL2Nzcy9xdWVyeS1lZGl0b3IuY3NzJ1xyXG5kZWNsYXJlIHZhciBfOiBhbnk7XHJcblxyXG5leHBvcnQgZGVmYXVsdCBjbGFzcyBPcGVuSGlzdG9yaWFuRGF0YVNvdXJjZVF1ZXJ5Q3RybCBleHRlbmRzIFF1ZXJ5Q3RybHtcclxuICAgIHN0YXRpYyB0ZW1wbGF0ZVVybCA9ICdwYXJ0aWFsL3F1ZXJ5LmVkaXRvci5odG1sJztcclxuXHJcbiAgICBxdWVyeVR5cGVzOiBBcnJheTxzdHJpbmc+O1xyXG4gICAgcXVlcnlUeXBlOiBzdHJpbmc7XHJcbiAgICBxdWVyeU9wdGlvbnNPcGVuOiBib29sZWFuO1xyXG5cclxuICAgIGNvbnN0cnVjdG9yKCRzY29wZSwgJGluamVjdG9yLCBwcml2YXRlIHVpU2VnbWVudFNydixwcml2YXRlIHRlbXBsYXRlU3J2LHByaXZhdGUgJGNvbXBpbGUpIHtcclxuICAgICAgICBzdXBlcigkc2NvcGUsICRpbmplY3Rvcik7XHJcblxyXG4gICAgICAgIHRoaXMuJHNjb3BlID0gJHNjb3BlO1xyXG4gICAgICAgIHRoaXMuJGNvbXBpbGUgPSAkY29tcGlsZTtcclxuXHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG4gICAgICAgIHRoaXMudWlTZWdtZW50U3J2ID0gdWlTZWdtZW50U3J2O1xyXG5cclxuICAgICAgICB0aGlzLnF1ZXJ5VHlwZXMgPSBbXHJcbiAgICAgICAgICAgIFwiRWxlbWVudCBMaXN0XCIsIFwiRmlsdGVyIEV4cHJlc3Npb25cIiwgXCJUZXh0IEVkaXRvclwiXHJcbiAgICAgICAgXTtcclxuXHJcbiAgICAgICAgdGhpcy5xdWVyeVR5cGUgPSAodGhpcy50YXJnZXQucXVlcnlUeXBlID09IHVuZGVmaW5lZCA/IFwiRWxlbWVudCBMaXN0XCIgOiB0aGlzLnRhcmdldC5xdWVyeVR5cGUpO1xyXG4gICAgICAgIFxyXG4gICAgICAgIHRoaXMucXVlcnlPcHRpb25zT3BlbiA9IGZhbHNlO1xyXG5cclxuICAgICAgICBpZihjdHJsLnRhcmdldC5xdWVyeU9wdGlvbnMgPT0gdW5kZWZpbmVkKSBcclxuICAgICAgICAgICAgY3RybC50YXJnZXQucXVlcnlPcHRpb25zID0ge0V4Y2x1ZGVkOiBjdHJsLmRhdGFzb3VyY2Uub3B0aW9ucy5leGNsdWRlZERhdGFGbGFncywgTm9ybWFsOiBjdHJsLmRhdGFzb3VyY2Uub3B0aW9ucy5leGNsdWRlTm9ybWFsRGF0YX07XHJcbiAgICB9XHJcblxyXG4gICAgdG9nZ2xlUXVlcnlPcHRpb25zKCl7XHJcbiAgICAgICAgdGhpcy5xdWVyeU9wdGlvbnNPcGVuID0gIXRoaXMucXVlcnlPcHRpb25zT3BlbjtcclxuICAgIH1cclxuXHJcbiAgb25DaGFuZ2VJbnRlcm5hbCgpIHtcclxuICAgIHRoaXMucGFuZWxDdHJsLnJlZnJlc2goKTsgLy8gQXNrcyB0aGUgcGFuZWwgdG8gcmVmcmVzaCBkYXRhLlxyXG4gIH1cclxuXHJcbiAgLy8gdXNlZCB0byBjaGFuZ2UgdGhlIHF1ZXJ5IHR5cGUgZnJvbSBlbGVtZW50IGxpc3QgdG8gZmlsdGVyIGV4cHJlc3Npb25cclxuICBjaGFuZ2VRdWVyeVR5cGUoKSB7XHJcbiAgICAgIGlmICh0aGlzLnF1ZXJ5VHlwZSA9PSAnVGV4dCBFZGl0b3InKSB7XHJcbiAgICAgICAgICB0aGlzLnRhcmdldC50YXJnZXRUZXh0ID0gdGhpcy50YXJnZXQudGFyZ2V0O1xyXG4gICAgICB9XHJcbiAgICAgIGVsc2V7XHJcbiAgICAgICAgICB0aGlzLnRhcmdldC50YXJnZXQgPSAnJztcclxuICAgICAgICAgIGRlbGV0ZSB0aGlzLnRhcmdldC5mdW5jdGlvblNlZ21lbnRzXHJcbiAgICAgIH1cclxuICB9XHJcblxyXG59XHJcblxyXG5cclxuIiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIG9wZW5IaXN0b3JpYW5UZXh0RWRpdG9yLnRzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IMKpIDIwMTcsIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZS4gIEFsbCBSaWdodHMgUmVzZXJ2ZWQuXHJcbi8vXHJcbi8vICBMaWNlbnNlZCB0byB0aGUgR3JpZCBQcm90ZWN0aW9uIEFsbGlhbmNlIChHUEEpIHVuZGVyIG9uZSBvciBtb3JlIGNvbnRyaWJ1dG9yIGxpY2Vuc2UgYWdyZWVtZW50cy4gU2VlXHJcbi8vICB0aGUgTk9USUNFIGZpbGUgZGlzdHJpYnV0ZWQgd2l0aCB0aGlzIHdvcmsgZm9yIGFkZGl0aW9uYWwgaW5mb3JtYXRpb24gcmVnYXJkaW5nIGNvcHlyaWdodCBvd25lcnNoaXAuXHJcbi8vICBUaGUgR1BBIGxpY2Vuc2VzIHRoaXMgZmlsZSB0byB5b3UgdW5kZXIgdGhlIE1JVCBMaWNlbnNlIChNSVQpLCB0aGUgXCJMaWNlbnNlXCI7IHlvdSBtYXkgbm90IHVzZSB0aGlzXHJcbi8vICBmaWxlIGV4Y2VwdCBpbiBjb21wbGlhbmNlIHdpdGggdGhlIExpY2Vuc2UuIFlvdSBtYXkgb2J0YWluIGEgY29weSBvZiB0aGUgTGljZW5zZSBhdDpcclxuLy9cclxuLy8gICAgICBodHRwOi8vb3BlbnNvdXJjZS5vcmcvbGljZW5zZXMvTUlUXHJcbi8vXHJcbi8vICBVbmxlc3MgYWdyZWVkIHRvIGluIHdyaXRpbmcsIHRoZSBzdWJqZWN0IHNvZnR3YXJlIGRpc3RyaWJ1dGVkIHVuZGVyIHRoZSBMaWNlbnNlIGlzIGRpc3RyaWJ1dGVkIG9uIGFuXHJcbi8vICBcIkFTLUlTXCIgQkFTSVMsIFdJVEhPVVQgV0FSUkFOVElFUyBPUiBDT05ESVRJT05TIE9GIEFOWSBLSU5ELCBlaXRoZXIgZXhwcmVzcyBvciBpbXBsaWVkLiBSZWZlciB0byB0aGVcclxuLy8gIExpY2Vuc2UgZm9yIHRoZSBzcGVjaWZpYyBsYW5ndWFnZSBnb3Zlcm5pbmcgcGVybWlzc2lvbnMgYW5kIGxpbWl0YXRpb25zLlxyXG4vL1xyXG4vLyAgQ29kZSBNb2RpZmljYXRpb24gSGlzdG9yeTpcclxuLy8gIC0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS1cclxuLy8gIDEyLzEyLzIwMTcgLSBCaWxseSBFcm5lc3RcclxuLy8gICAgICAgR2VuZXJhdGVkIG9yaWdpbmFsIHZlcnNpb24gb2Ygc291cmNlIGNvZGUuXHJcbi8vXHJcbi8vKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXHJcbmltcG9ydCB7IFBhbmVsQ3RybCB9IGZyb20gXCJncmFmYW5hLXNkay1tb2Nrcy9hcHAvcGx1Z2lucy9zZGtcIjtcclxuXHJcbmRlY2xhcmUgdmFyIF86IGFueTtcclxuXHJcbmV4cG9ydCBkZWZhdWx0IGNsYXNzIE9wZW5IaXN0b3JpYW5UZXh0RWRpdG9yQ3RybHtcclxuICAgIHRhcmdldFRleHQ6IHN0cmluZztcclxuICAgIHRhcmdldDogaVRhcmdldDtcclxuICAgIGNvbnN0cnVjdG9yKHByaXZhdGUgJHNjb3BlOiB7dGFyZ2V0OiBpVGFyZ2V0LCB0aGlzdGFyZ2V0VGV4dDogc3RyaW5nLCBwYW5lbDogUGFuZWxDdHJsfSwgcHJpdmF0ZSB0ZW1wbGF0ZVNydikge1xyXG5cclxuICAgICAgICB0aGlzLiRzY29wZSA9ICRzY29wZTtcclxuICAgICAgICB0aGlzLnRhcmdldFRleHQgPSAoJHNjb3BlLnRhcmdldC50YXJnZXRUZXh0ID09IHVuZGVmaW5lZCA/ICcnIDogJHNjb3BlLnRhcmdldC50YXJnZXRUZXh0KTtcclxuXHJcbiAgICAgICAgdGhpcy5zZXRUYXJnZXRXaXRoVGV4dCgpO1xyXG5cclxuICAgICAgICBkZWxldGUgJHNjb3BlLnRhcmdldC5zZWdtZW50cztcclxuICAgICAgICBkZWxldGUgJHNjb3BlLnRhcmdldC5mdW5jdGlvblNlZ21lbnRzO1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0LndoZXJlcztcclxuICAgICAgICBkZWxldGUgJHNjb3BlLnRhcmdldC50b3BOU2VnbWVudDtcclxuICAgICAgICBkZWxldGUgJHNjb3BlLnRhcmdldC5mdW5jdGlvbnM7XHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQub3JkZXJCeXM7XHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQud2hlcmVTZWdtZW50O1xyXG4gICAgICAgIGRlbGV0ZSAkc2NvcGUudGFyZ2V0LmZpbHRlclNlZ21lbnQ7XHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQub3JkZXJCeVNlZ21lbnQ7XHJcbiAgICAgICAgZGVsZXRlICRzY29wZS50YXJnZXQuZnVuY3Rpb25TZWdtZW50O1xyXG4gICAgfVxyXG5cclxuICAgIHNldFRhcmdldFdpdGhUZXh0KCkge1xyXG4gICAgICAgIHRoaXMuJHNjb3BlLnRhcmdldC50YXJnZXRUZXh0ID0gdGhpcy50YXJnZXRUZXh0O1xyXG4gICAgICAgIHRoaXMuJHNjb3BlLnRhcmdldC50YXJnZXQgPSB0aGlzLnRhcmdldFRleHQ7XHJcbiAgICAgICAgdGhpcy4kc2NvcGUudGFyZ2V0LnF1ZXJ5VHlwZSA9ICdUZXh0IEVkaXRvcic7XHJcbiAgICAgICAgdGhpcy4kc2NvcGUucGFuZWwucmVmcmVzaCgpOyAvLyBBc2tzIHRoZSBwYW5lbCB0byByZWZyZXNoIGRhdGEuXHJcbiAgICB9XHJcblxyXG59IiwidmFyIGNvbnRlbnQgPSByZXF1aXJlKFwiISEuLi9ub2RlX21vZHVsZXMvY3NzLWxvYWRlci9kaXN0L2Nqcy5qcyEuL3F1ZXJ5LWVkaXRvci5jc3NcIik7XG5cbmlmICh0eXBlb2YgY29udGVudCA9PT0gJ3N0cmluZycpIHtcbiAgY29udGVudCA9IFtbbW9kdWxlLmlkLCBjb250ZW50LCAnJ11dO1xufVxuXG52YXIgb3B0aW9ucyA9IHt9XG5cbm9wdGlvbnMuaW5zZXJ0ID0gXCJoZWFkXCI7XG5vcHRpb25zLnNpbmdsZXRvbiA9IGZhbHNlO1xuXG52YXIgdXBkYXRlID0gcmVxdWlyZShcIiEuLi9ub2RlX21vZHVsZXMvc3R5bGUtbG9hZGVyL2Rpc3QvcnVudGltZS9pbmplY3RTdHlsZXNJbnRvU3R5bGVUYWcuanNcIikoY29udGVudCwgb3B0aW9ucyk7XG5cbmlmIChjb250ZW50LmxvY2Fscykge1xuICBtb2R1bGUuZXhwb3J0cyA9IGNvbnRlbnQubG9jYWxzO1xufVxuIiwiLy8qKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKipcclxuLy8gIGNvbnN0YW50cy5qcyAtIEdidGNcclxuLy9cclxuLy8gIENvcHlyaWdodCDCqSAyMDE3LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAxMS8wMS8yMDE3IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5cclxuLy8gI3JlZ2lvbiBDb25zdGFudHNcclxuZXhwb3J0IGNvbnN0IERlZmF1bHRGbGFncyA9IHtcclxuICAgICdTZWxlY3QgQWxsJzogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAtMSwgRmxhZzogMCB9LFxyXG4gICAgTm9ybWFsOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDAsIEZsYWc6IDAgIH0sXHJcbiAgICBCYWREYXRhOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDEsIEZsYWc6IDEgPDwgMCAgfSxcclxuICAgIFN1c3BlY3REYXRhOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDIsIEZsYWc6IDEgPDwgMSB9LFxyXG4gICAgT3ZlclJhbmdlRXJyb3I6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMywgRmxhZzogMSA8PCAyIH0sXHJcbiAgICBVbmRlclJhbmdlRXJyb3I6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogNCwgRmxhZzogMSA8PCAzIH0sXHJcbiAgICBBbGFybUhpZ2g6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogNSwgRmxhZzogMSA8PCA0IH0sXHJcbiAgICBBbGFybUxvdzogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiA2LCBGbGFnOiAxIDw8IDUgfSxcclxuICAgIFdhcm5pbmdIaWdoOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDcsIEZsYWc6IDEgPDwgNiB9LFxyXG4gICAgV2FybmluZ0xvdzogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiA4LCBGbGFnOiAxIDw8IDcgfSxcclxuICAgIEZsYXRsaW5lQWxhcm06IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogOSwgRmxhZzogMSA8PCA4IH0sXHJcbiAgICBDb21wYXJpc29uQWxhcm06IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMTAsIEZsYWc6IDEgPDwgOSB9LFxyXG4gICAgUk9DQWxhcm06IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMTEsIEZsYWc6IDEgPDwgMTAgfSxcclxuICAgIFJlY2VpdmVkQXNCYWQ6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMTIsIEZsYWc6IDEgPDwgMTEgfSxcclxuICAgIENhbGN1bGF0ZWRWYWx1ZTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAxMywgRmxhZzogMSA8PCAxMiB9LFxyXG4gICAgQ2FsY3VsYXRpb25FcnJvcjogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAxNCwgRmxhZzogMSA8PCAxMyB9LFxyXG4gICAgQ2FsY3VsYXRpb25XYXJuaW5nOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDE1LCBGbGFnOiAxIDw8IDE0IH0sXHJcbiAgICBSZXNlcnZlZFF1YWxpdHlGbGFnOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDE2LCBGbGFnOiAxIDw8IDE1IH0sXHJcbiAgICBCYWRUaW1lOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDE3LCBGbGFnOiAxIDw8IDE2IH0sXHJcbiAgICBTdXNwZWN0VGltZTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAxOCwgRmxhZzogMSA8PCAxNyB9LFxyXG4gICAgTGF0ZVRpbWVBbGFybTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAxOSwgRmxhZzogMSA8PCAxOCB9LFxyXG4gICAgRnV0dXJlVGltZUFsYXJtOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDIwLCBGbGFnOiAxIDw8IDE5IH0sXHJcbiAgICBVcFNhbXBsZWQ6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMjEsIEZsYWc6IDEgPDwgMjAgfSxcclxuICAgIERvd25TYW1wbGVkOiB7IFZhbHVlOiBmYWxzZSwgT3JkZXI6IDIyLCBGbGFnOiAxIDw8IDIxIH0sXHJcbiAgICBEaXNjYXJkZWRWYWx1ZTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyMywgRmxhZzogMSA8PCAyMiB9LFxyXG4gICAgUmVzZXJ2ZWRUaW1lRmxhZzogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyNCwgRmxhZzogMSA8PCAyMyB9LFxyXG4gICAgVXNlckRlZmluZWRGbGFnMTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyNSwgRmxhZzogMSA8PCAyNCB9LFxyXG4gICAgVXNlckRlZmluZWRGbGFnMjogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyNiwgRmxhZzogMSA8PCAyNSB9LFxyXG4gICAgVXNlckRlZmluZWRGbGFnMzogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyNywgRmxhZzogMSA8PCAyNiB9LFxyXG4gICAgVXNlckRlZmluZWRGbGFnNDogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyOCwgRmxhZzogMSA8PCAyNyB9LFxyXG4gICAgVXNlckRlZmluZWRGbGFnNTogeyBWYWx1ZTogZmFsc2UsIE9yZGVyOiAyOSwgRmxhZzogMSA8PCAyOCB9LFxyXG4gICAgU3lzdGVtRXJyb3I6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMzAsIEZsYWc6IDEgPDwgMjkgfSxcclxuICAgIFN5c3RlbVdhcm5pbmc6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMzEsIEZsYWc6IDEgPDwgMzAgfSxcclxuICAgIE1lYXN1cmVtZW50RXJyb3I6IHsgVmFsdWU6IGZhbHNlLCBPcmRlcjogMzIsIEZsYWc6IDEgPDwgMzEgfVxyXG59XHJcblxyXG5leHBvcnQgY29uc3QgRnVuY3Rpb25MaXN0ID0ge1xyXG4gICAgU2V0OiB7IEZ1bmN0aW9uOiAnU2V0JywgUGFyYW1ldGVyczogW10gfSxcclxuICAgIFNsaWNlOiB7IEZ1bmN0aW9uOiAnU2xpY2UnLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAxLCBUeXBlOiAnZG91YmxlJywgRGVzY3JpcHRpb246ICdBIGZsb2F0aW5nLXBvaW50IHZhbHVlIHRoYXQgbXVzdCBiZSBncmVhdGVyIHRoYW4gb3IgZXF1YWwgdG8gemVybyB0aGF0IHJlcHJlc2VudHMgdGhlIGRlc2lyZWQgdGltZSB0b2xlcmFuY2UsIGluIHNlY29uZHMsIGZvciB0aGUgdGltZSBzbGljZS4nIH1dIH0sXHJcbiAgICBBdmVyYWdlOiB7IEZ1bmN0aW9uOiAnQXZlcmFnZScsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBNaW5pbXVtOiB7IEZ1bmN0aW9uOiAnTWluaW11bScsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBNYXhpbXVtOiB7IEZ1bmN0aW9uOiAnTWF4aW11bScsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBUb3RhbDogeyBGdW5jdGlvbjogJ1RvdGFsJywgUGFyYW1ldGVyczogW10gfSxcclxuICAgIFJhbmdlOiB7IEZ1bmN0aW9uOiAnUmFuZ2UnLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgQ291bnQ6IHsgRnVuY3Rpb246ICdDb3VudCcsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBEaXN0aW5jdDogeyBGdW5jdGlvbjogJ0Rpc3RpbmN0JywgUGFyYW1ldGVyczogW10gfSxcclxuICAgIEFic29sdXRlVmFsdXRlOiB7IEZ1bmN0aW9uOiAnQWJzb2x1dGVWYWx1ZScsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBBZGQ6IHsgRnVuY3Rpb246ICdBZGQnLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAwLCBUeXBlOiAnc3RyaW5nJywgRGVzY3JpcHRpb246ICdBIGZsb2F0aW5nIHBvaW50IHZhbHVlIHJlcHJlc2VudGluZyBhbiBhZGRpdGl2ZSBvZmZzZXQgdG8gYmUgYXBwbGllZCB0byBlYWNoIHZhbHVlIHRoZSBzb3VyY2Ugc2VyaWVzLicgfV0gfSxcclxuICAgIFN1YnRyYWN0OiB7IEZ1bmN0aW9uOiAnU3VidHJhY3QnLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAwLCBUeXBlOiAnc3RyaW5nJywgRGVzY3JpcHRpb246ICdBIGZsb2F0aW5nIHBvaW50IHZhbHVlIHJlcHJlc2VudGluZyBhbiBhZGRpdGl2ZSBvZmZzZXQgdG8gYmUgYXBwbGllZCB0byBlYWNoIHZhbHVlIHRoZSBzb3VyY2Ugc2VyaWVzLicgfV0gfSxcclxuICAgIE11bHRpcGx5OiB7IEZ1bmN0aW9uOiAnTXVsdGlwbHknLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAxLCBUeXBlOiAnc3RyaW5nJywgRGVzY3JpcHRpb246ICdBIGZsb2F0aW5nIHBvaW50IHZhbHVlIHJlcHJlc2VudGluZyBhbiBhZGRpdGl2ZSBvZmZzZXQgdG8gYmUgYXBwbGllZCB0byBlYWNoIHZhbHVlIHRoZSBzb3VyY2Ugc2VyaWVzLicgfV0gfSxcclxuICAgIERpdmlkZTogeyBGdW5jdGlvbjogJ011bHRpcGx5JywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogMSwgVHlwZTogJ3N0cmluZycsIERlc2NyaXB0aW9uOiAnQSBmbG9hdGluZyBwb2ludCB2YWx1ZSByZXByZXNlbnRpbmcgYW4gYWRkaXRpdmUgb2Zmc2V0IHRvIGJlIGFwcGxpZWQgdG8gZWFjaCB2YWx1ZSB0aGUgc291cmNlIHNlcmllcy4nIH1dIH0sXHJcbiAgICBSb3VuZDogeyBGdW5jdGlvbjogJ1JvdW5kJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogMCwgVHlwZTogJ2RvdWJsZScsIERlc2NyaXB0aW9uOiAnQSBwb3NpdGl2ZSBpbnRlZ2VyIHZhbHVlIHJlcHJlc2VudGluZyB0aGUgbnVtYmVyIG9mIGRlY2ltYWwgcGxhY2VzIGluIHRoZSByZXR1cm4gdmFsdWUgLSBkZWZhdWx0cyB0byAwLicgfV0gfSxcclxuICAgIEZsb29yOiB7IEZ1bmN0aW9uOiAnRmxvb3InLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgQ2VpbGluZzogeyBGdW5jdGlvbjogJ0NlaWxpbmcnLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgVHJ1bmNhdGU6IHsgRnVuY3Rpb246ICdUcnVuY2F0ZScsIFBhcmFtZXRlcnM6IFtdIH0sXHJcbiAgICBTdGFuZGFyZERldmlhdGlvbjogeyBGdW5jdGlvbjogJ1N0YW5kYXJkRGV2aWF0aW9uJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogZmFsc2UsIFR5cGU6ICdib29sZWFuJywgRGVzY3JpcHRpb246ICdBIGJvb2xlYW4gZmxhZyByZXByZXNlbnRpbmcgaWYgdGhlIHNhbXBsZSBiYXNlZCBjYWxjdWxhdGlvbiBzaG91bGQgYmUgdXNlZCAtIGRlZmF1bHRzIHRvIGZhbHNlLCB3aGljaCBtZWFucyB0aGUgcG9wdWxhdGlvbiBiYXNlZCBjYWxjdWxhdGlvbiBzaG91bGQgYmUgdXNlZC4nIH1dIH0sXHJcbiAgICBNZWRpYW46IHsgRnVuY3Rpb246ICdNZWRpYW4nLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgTW9kZTogeyBGdW5jdGlvbjogJ01vZGUnLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgVG9wOiB7IEZ1bmN0aW9uOiAnVG9wJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogJzEwMCUnLCBUeXBlOiAnc3RyaW5nJywgRGVzY3JpcHRpb246ICdBIHBvc2l0aXZlIGludGVnZXIgdmFsdWUsIHJlcHJlc2VudGluZyBhIHRvdGFsLCB0aGF0IGlzIGdyZWF0ZXIgdGhhbiB6ZXJvIC0gb3IgLSBhIGZsb2F0aW5nIHBvaW50IHZhbHVlLCBzdWZmaXhlZCB3aXRoIFxcJyAlXFwnIHJlcHJlc2VudGluZyBhIHBlcmNlbnRhZ2UsIHRoYXQgbXVzdCByYW5nZSBmcm9tIGdyZWF0ZXIgdGhhbiAwIHRvIGxlc3MgdGhhbiBvciBlcXVhbCB0byAxMDAuJyB9LCB7IERlZmF1bHQ6IHRydWUsIFR5cGU6ICdib29sZWFuJywgRGVzY3JpcHRpb246ICdBIGJvb2xlYW4gZmxhZyByZXByZXNlbnRpbmcgaWYgdGltZSBpbiBkYXRhc2V0IHNob3VsZCBiZSBub3JtYWxpemVkIC0gZGVmYXVsdHMgdG8gdHJ1ZS4nIH1dIH0sXHJcbiAgICBCb3R0b206IHsgRnVuY3Rpb246ICdCb3R0b20nLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAnMTAwJScsIFR5cGU6ICdzdHJpbmcnLCBEZXNjcmlwdGlvbjogJ0EgcG9zaXRpdmUgaW50ZWdlciB2YWx1ZSwgcmVwcmVzZW50aW5nIGEgdG90YWwsIHRoYXQgaXMgZ3JlYXRlciB0aGFuIHplcm8gLSBvciAtIGEgZmxvYXRpbmcgcG9pbnQgdmFsdWUsIHN1ZmZpeGVkIHdpdGggXFwnICVcXCcgcmVwcmVzZW50aW5nIGEgcGVyY2VudGFnZSwgdGhhdCBtdXN0IHJhbmdlIGZyb20gZ3JlYXRlciB0aGFuIDAgdG8gbGVzcyB0aGFuIG9yIGVxdWFsIHRvIDEwMC4nIH0sIHsgRGVmYXVsdDogdHJ1ZSwgVHlwZTogJ2Jvb2xlYW4nLCBEZXNjcmlwdGlvbjogJ0EgYm9vbGVhbiBmbGFnIHJlcHJlc2VudGluZyBpZiB0aW1lIGluIGRhdGFzZXQgc2hvdWxkIGJlIG5vcm1hbGl6ZWQgLSBkZWZhdWx0cyB0byB0cnVlLicgfV0gfSxcclxuICAgIFJhbmRvbTogeyBGdW5jdGlvbjogJ1JhbmRvbScsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICcxMDAlJywgVHlwZTogJ3N0cmluZycsIERlc2NyaXB0aW9uOiAnQSBwb3NpdGl2ZSBpbnRlZ2VyIHZhbHVlLCByZXByZXNlbnRpbmcgYSB0b3RhbCwgdGhhdCBpcyBncmVhdGVyIHRoYW4gemVybyAtIG9yIC0gYSBmbG9hdGluZyBwb2ludCB2YWx1ZSwgc3VmZml4ZWQgd2l0aCBcXCcgJVxcJyByZXByZXNlbnRpbmcgYSBwZXJjZW50YWdlLCB0aGF0IG11c3QgcmFuZ2UgZnJvbSBncmVhdGVyIHRoYW4gMCB0byBsZXNzIHRoYW4gb3IgZXF1YWwgdG8gMTAwLicgfSwgeyBEZWZhdWx0OiB0cnVlLCBUeXBlOiAnYm9vbGVhbicsIERlc2NyaXB0aW9uOiAnQSBib29sZWFuIGZsYWcgcmVwcmVzZW50aW5nIGlmIHRpbWUgaW4gZGF0YXNldCBzaG91bGQgYmUgbm9ybWFsaXplZCAtIGRlZmF1bHRzIHRvIHRydWUuJyB9XSB9LFxyXG4gICAgRmlyc3Q6IHsgRnVuY3Rpb246ICdGaXJzdCcsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICcxJywgVHlwZTogJ3N0cmluZycsIERlc2NyaXB0aW9uOiAnQSBwb3NpdGl2ZSBpbnRlZ2VyIHZhbHVlLCByZXByZXNlbnRpbmcgYSB0b3RhbCwgdGhhdCBpcyBncmVhdGVyIHRoYW4gemVybyAtIG9yIC0gYSBmbG9hdGluZyBwb2ludCB2YWx1ZSwgc3VmZml4ZWQgd2l0aCBcXCcgJVxcJyByZXByZXNlbnRpbmcgYSBwZXJjZW50YWdlLCB0aGF0IG11c3QgcmFuZ2UgZnJvbSBncmVhdGVyIHRoYW4gMCB0byBsZXNzIHRoYW4gb3IgZXF1YWwgdG8gMTAwIC0gZGVmYXVsdHMgdG8gMS4nIH1dIH0sXHJcbiAgICBMYXN0OiB7IEZ1bmN0aW9uOiAnTGFzdCcsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICcxJywgVHlwZTogJ3N0cmluZycsIERlc2NyaXB0aW9uOiAnQSBwb3NpdGl2ZSBpbnRlZ2VyIHZhbHVlLCByZXByZXNlbnRpbmcgYSB0b3RhbCwgdGhhdCBpcyBncmVhdGVyIHRoYW4gemVybyAtIG9yIC0gYSBmbG9hdGluZyBwb2ludCB2YWx1ZSwgc3VmZml4ZWQgd2l0aCBcXCcgJVxcJyByZXByZXNlbnRpbmcgYSBwZXJjZW50YWdlLCB0aGF0IG11c3QgcmFuZ2UgZnJvbSBncmVhdGVyIHRoYW4gMCB0byBsZXNzIHRoYW4gb3IgZXF1YWwgdG8gMTAwIC0gZGVmYXVsdHMgdG8gMS4nIH1dIH0sXHJcbiAgICBQZXJjZW50aWxlOiB7IEZ1bmN0aW9uOiAnUGVyY2VudGlsZScsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICcxMDAlJywgVHlwZTogJ3N0cmluZycsIERlc2NyaXB0aW9uOiAnQSBmbG9hdGluZyBwb2ludCB2YWx1ZSwgcmVwcmVzZW50aW5nIGEgcGVyY2VudGFnZSwgdGhhdCBtdXN0IHJhbmdlIGZyb20gMCB0byAxMDAuJyB9XSB9LFxyXG4gICAgRGlmZmVyZW5jZTogeyBGdW5jdGlvbjogJ0RpZmZlcmVuY2UnLCBQYXJhbWV0ZXJzOiBbXSB9LFxyXG4gICAgVGltZURpZmZlcmVuY2U6IHsgRnVuY3Rpb246ICdUaW1lRGlmZmVyZW5jZScsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICdTZWNvbmRzJywgVHlwZTogJ3RpbWUnLCBEZXNjcmlwdGlvbjogJ1NwZWNpZmllcyB0aGUgdHlwZSBvZiB0aW1lIHVuaXRzIGFuZCBtdXN0IGJlIG9uZSBvZiB0aGUgZm9sbG93aW5nOiBTZWNvbmRzLCBOYW5vc2Vjb25kcywgTWljcm9zZWNvbmRzLCBNaWxsaXNlY29uZHMsIE1pbnV0ZXMsIEhvdXJzLCBEYXlzLCBXZWVrcywgS2UgKGkuZS4sIHRyYWRpdGlvbmFsIENoaW5lc2UgdW5pdCBvZiBkZWNpbWFsIHRpbWUpLCBUaWNrcyAoaS5lLiwgMTAwLW5hbm9zZWNvbmQgaW50ZXJ2YWxzKSwgUGxhbmNrVGltZSBvciBBdG9taWNVbml0c09mVGltZSAtIGRlZmF1bHRzIHRvIFNlY29uZHMuJyB9XSB9LFxyXG4gICAgRGVyaXZhdGl2ZTogeyBGdW5jdGlvbjogJ0Rlcml2YXRpdmUnLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiAnU2Vjb25kcycsIFR5cGU6ICd0aW1lJywgRGVzY3JpcHRpb246ICdTcGVjaWZpZXMgdGhlIHR5cGUgb2YgdGltZSB1bml0cyBhbmQgbXVzdCBiZSBvbmUgb2YgdGhlIGZvbGxvd2luZzogU2Vjb25kcywgTmFub3NlY29uZHMsIE1pY3Jvc2Vjb25kcywgTWlsbGlzZWNvbmRzLCBNaW51dGVzLCBIb3VycywgRGF5cywgV2Vla3MsIEtlIChpLmUuLCB0cmFkaXRpb25hbCBDaGluZXNlIHVuaXQgb2YgZGVjaW1hbCB0aW1lKSwgVGlja3MgKGkuZS4sIDEwMC1uYW5vc2Vjb25kIGludGVydmFscyksIFBsYW5ja1RpbWUgb3IgQXRvbWljVW5pdHNPZlRpbWUgLSBkZWZhdWx0cyB0byBTZWNvbmRzLicgfV0gfSxcclxuICAgIFRpbWVJbnRlZ3JhdGlvbjogeyBGdW5jdGlvbjogJ1RpbWVJbnRlZ3JhdGlvbicsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICdIb3VycycsIFR5cGU6ICd0aW1lJywgRGVzY3JpcHRpb246ICdTcGVjaWZpZXMgdGhlIHR5cGUgb2YgdGltZSB1bml0cyBhbmQgbXVzdCBiZSBvbmUgb2YgdGhlIGZvbGxvd2luZzogU2Vjb25kcywgTmFub3NlY29uZHMsIE1pY3Jvc2Vjb25kcywgTWlsbGlzZWNvbmRzLCBNaW51dGVzLCBIb3VycywgRGF5cywgV2Vla3MsIEtlIChpLmUuLCB0cmFkaXRpb25hbCBDaGluZXNlIHVuaXQgb2YgZGVjaW1hbCB0aW1lKSwgVGlja3MgKGkuZS4sIDEwMC1uYW5vc2Vjb25kIGludGVydmFscyksIFBsYW5ja1RpbWUgb3IgQXRvbWljVW5pdHNPZlRpbWUgLSBkZWZhdWx0cyB0byBIb3Vycy4nIH1dIH0sXHJcbiAgICBJbnRlcnZhbDogeyBGdW5jdGlvbjogJ0ludGVydmFsJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogMCwgVHlwZTogJ2RvdWJsZScsIERlc2NyaXB0aW9uOiAnQSBmbG9hdGluZy1wb2ludCB2YWx1ZSB0aGF0IG11c3QgYmUgZ3JlYXRlciB0aGFuIG9yIGVxdWFsIHRvIHplcm8gdGhhdCByZXByZXNlbnRzIHRoZSBkZXNpcmVkIHRpbWUgaW50ZXJ2YWwsIGluIHRpbWUgdW5pdHMsIGZvciB0aGUgcmV0dXJuZWQgZGF0YS4gJyB9LCB7IERlZmF1bHQ6ICdTZWNvbmRzJywgVHlwZTogJ3RpbWUnLCBEZXNjcmlwdGlvbjogJ1NwZWNpZmllcyB0aGUgdHlwZSBvZiB0aW1lIHVuaXRzIGFuZCBtdXN0IGJlIG9uZSBvZiB0aGUgZm9sbG93aW5nOiBTZWNvbmRzLCBOYW5vc2Vjb25kcywgTWljcm9zZWNvbmRzLCBNaWxsaXNlY29uZHMsIE1pbnV0ZXMsIEhvdXJzLCBEYXlzLCBXZWVrcywgS2UgKGkuZS4sIHRyYWRpdGlvbmFsIENoaW5lc2UgdW5pdCBvZiBkZWNpbWFsIHRpbWUpLCBUaWNrcyAoaS5lLiwgMTAwLW5hbm9zZWNvbmQgaW50ZXJ2YWxzKSwgUGxhbmNrVGltZSBvciBBdG9taWNVbml0c09mVGltZSAtIGRlZmF1bHRzIHRvIFNlY29uZHMuJyB9XSB9LFxyXG4gICAgSW5jbHVkZVJhbmdlOiB7IEZ1bmN0aW9uOiAnSW5jbHVkZVJhbmdlJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogMCwgVHlwZTogJ2RvdWJsZScsIERlc2NyaXB0aW9uOiAnRmxvYXRpbmctcG9pbnQgbnVtYmVyIHRoYXQgcmVwcmVzZW50cyB0aGUgbG93IHJhbmdlIG9mIHZhbHVlcyBhbGxvd2VkIGluIHRoZSByZXR1cm4gc2VyaWVzLicgfSwgeyBEZWZhdWx0OiAwLCBUeXBlOiAnZG91YmxlJywgRGVzY3JpcHRpb246ICdGbG9hdGluZy1wb2ludCBudW1iZXIgdGhhdCByZXByZXNlbnRzIHRoZSBoaWdoIHJhbmdlIG9mIHZhbHVlcyBhbGxvd2VkIGluIHRoZSByZXR1cm4gc2VyaWVzLicgfSwgeyBEZWZhdWx0OiBmYWxzZSwgVHlwZTogJ2Jvb2xlYW4nLCBEZXNjcmlwdGlvbjogJ0EgYm9vbGVhbiBmbGFnIHRoYXQgZGV0ZXJtaW5lcyBpZiByYW5nZSB2YWx1ZXMgYXJlIGluY2x1c2l2ZSwgaS5lLiwgYWxsb3dlZCB2YWx1ZXMgYXJlID49IGxvdyBhbmQgPD0gaGlnaCAtIGRlZmF1bHRzIHRvIGZhbHNlLCB3aGljaCBtZWFucyB2YWx1ZXMgYXJlIGV4Y2x1c2l2ZSwgaS5lLiwgYWxsb3dlZCB2YWx1ZXMgYXJlID4gbG93IGFuZCA8IGhpZ2guJyB9LCB7IERlZmF1bHQ6IGZhbHNlLCBUeXBlOiAnYm9vbGVhbicsIERlc2NyaXB0aW9uOiAnQSBib29sZWFuIGZsYWcgLSB3aGVuIGZvdXIgcGFyYW1ldGVycyBhcmUgcHJvdmlkZWQsIHRoaXJkIHBhcmFtZXRlciBkZXRlcm1pbmVzIGlmIGxvdyB2YWx1ZSBpcyBpbmNsdXNpdmUgYW5kIGZvcnRoIHBhcmFtZXRlciBkZXRlcm1pbmVzIGlmIGhpZ2ggdmFsdWUgaXMgaW5jbHVzaXZlLicgfV0gfSxcclxuICAgIEV4Y2x1ZGVSYW5nZTogeyBGdW5jdGlvbjogJ0V4Y2x1ZGVSYW5nZScsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6IDAsIFR5cGU6ICdkb3VibGUnLCBEZXNjcmlwdGlvbjogJ0Zsb2F0aW5nLXBvaW50IG51bWJlciB0aGF0IHJlcHJlc2VudHMgdGhlIGxvdyByYW5nZSBvZiB2YWx1ZXMgYWxsb3dlZCBpbiB0aGUgcmV0dXJuIHNlcmllcy4nIH0sIHsgRGVmYXVsdDogMCwgVHlwZTogJ2RvdWJsZScsIERlc2NyaXB0aW9uOiAnRmxvYXRpbmctcG9pbnQgbnVtYmVyIHRoYXQgcmVwcmVzZW50cyB0aGUgaGlnaCByYW5nZSBvZiB2YWx1ZXMgYWxsb3dlZCBpbiB0aGUgcmV0dXJuIHNlcmllcy4nIH0sIHsgRGVmYXVsdDogZmFsc2UsIFR5cGU6ICdib29sZWFuJywgRGVzY3JpcHRpb246ICdBIGJvb2xlYW4gZmxhZyB0aGF0IGRldGVybWluZXMgaWYgcmFuZ2UgdmFsdWVzIGFyZSBpbmNsdXNpdmUsIGkuZS4sIGFsbG93ZWQgdmFsdWVzIGFyZSA+PSBsb3cgYW5kIDw9IGhpZ2ggLSBkZWZhdWx0cyB0byBmYWxzZSwgd2hpY2ggbWVhbnMgdmFsdWVzIGFyZSBleGNsdXNpdmUsIGkuZS4sIGFsbG93ZWQgdmFsdWVzIGFyZSA+IGxvdyBhbmQgPCBoaWdoLicgfSwgeyBEZWZhdWx0OiBmYWxzZSwgVHlwZTogJ2Jvb2xlYW4nLCBEZXNjcmlwdGlvbjogJ0EgYm9vbGVhbiBmbGFnIC0gd2hlbiBmb3VyIHBhcmFtZXRlcnMgYXJlIHByb3ZpZGVkLCB0aGlyZCBwYXJhbWV0ZXIgZGV0ZXJtaW5lcyBpZiBsb3cgdmFsdWUgaXMgaW5jbHVzaXZlIGFuZCBmb3J0aCBwYXJhbWV0ZXIgZGV0ZXJtaW5lcyBpZiBoaWdoIHZhbHVlIGlzIGluY2x1c2l2ZS4nIH1dIH0sXHJcbiAgICBGaWx0ZXJOYU46IHsgRnVuY3Rpb246ICdGaWx0ZXJOYU4nLCBQYXJhbWV0ZXJzOiBbeyBEZWZhdWx0OiB0cnVlLCBUeXBlOiAnYm9vbGVhbicsIERlc2NyaXB0aW9uOiAnQSBib29sZWFuIGZsYWcgdGhhdCBkZXRlcm1pbmVzIGlmIGluZmluaXRlIHZhbHVlcyBzaG91bGQgYWxzbyBiZSBleGNsdWRlZCAtIGRlZmF1bHRzIHRvIHRydWUuJyB9XSB9LFxyXG4gICAgVW53cmFwQW5nbGU6IHsgRnVuY3Rpb246ICdVbndyYXBBbmdsZScsIFBhcmFtZXRlcnM6IFt7IERlZmF1bHQ6ICdEZWdyZWVzJywgVHlwZTogJ2FuZ2xlVW5pdHMnLCBEZXNjcmlwdGlvbjogJ1NwZWNpZmllcyB0aGUgdHlwZSBvZiBhbmdsZSB1bml0cyBhbmQgbXVzdCBiZSBvbmUgb2YgdGhlIGZvbGxvd2luZzogRGVncmVlcywgUmFkaWFucywgR3JhZHMsIEFyY01pbnV0ZXMsIEFyY1NlY29uZHMgb3IgQW5ndWxhck1pbCAtIGRlZmF1bHRzIHRvIERlZ3JlZXMuJyB9XSB9LFxyXG4gICAgV3JhcEFuZ2xlOiB7IEZ1bmN0aW9uOiAnV3JhcEFuZ2xlJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogJ0RlZ3JlZXMnLCBUeXBlOiAnYW5nbGVVbml0cycsIERlc2NyaXB0aW9uOiAnU3BlY2lmaWVzIHRoZSB0eXBlIG9mIGFuZ2xlIHVuaXRzIGFuZCBtdXN0IGJlIG9uZSBvZiB0aGUgZm9sbG93aW5nOiBEZWdyZWVzLCBSYWRpYW5zLCBHcmFkcywgQXJjTWludXRlcywgQXJjU2Vjb25kcyBvciBBbmd1bGFyTWlsIC0gZGVmYXVsdHMgdG8gRGVncmVlcy4nIH1dIH0sXHJcbiAgICBMYWJlbDogeyBGdW5jdGlvbjogJ0xhYmVsJywgUGFyYW1ldGVyczogW3sgRGVmYXVsdDogJ05hbWUnLCBUeXBlOiAnc3RyaW5nJywgRGVzY3JpcHRpb246ICdSZW5hbWVzIGEgc2VyaWVzIHdpdGggdGhlIHNwZWNpZmllZCBsYWJlbCB2YWx1ZS4nIH1dIH0sXHJcbn07XHJcblxyXG5leHBvcnQgY29uc3QgV2hlcmVPcGVyYXRvcnMgPSBbJz0nLCAnPD4nLCAnPCcsICc8PScsICc+JywgJz49JywgJ0xJS0UnLCAnTk9UIExJS0UnLCAnSU4nLCAnTk9UIElOJywgJ0lTJywgJ0lTIE5PVCddO1xyXG5cclxuZXhwb3J0IGNvbnN0IEJvb2xlYW5zID0gWyd0cnVlJywgJ2ZhbHNlJ107XHJcblxyXG5leHBvcnQgY29uc3QgQW5nbGVVbml0cyA9IFsnRGVncmVlcycsICdSYWRpYW5zJywgJ0dyYWRzJywgJ0FyY01pbnV0ZXMnLCAnQXJjU2Vjb25kcycsICdBbmd1bGFyTWlsJ11cclxuXHJcbmV4cG9ydCBjb25zdCBUaW1lVW5pdHMgPSBbJ1dlZWtzJywgJ0RheXMnLCAnSG91cnMnLCAnTWludXRlcycsICdTZWNvbmRzJywgJ01pbGxpc2Vjb25kcycsICdNaWNyb3NlY29uZHMnLCAnTmFub3NlY29uZHMnLCAnVGlja3MnLCAnUGxhbmtUaW1lJywgJ0F0b21pY1VuaXRzT2ZUaW1lJywgJ0tlJ11cclxuLy8gI2VuZHJlZ2lvblxyXG4iLCIvLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG4vLyAgbW9kdWxlLmpzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IO+/vSAyMDE3LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAxMS8wMi8yMDE3IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5kZWNsYXJlIHZhciBhbmd1bGFyOiBhbnk7XHJcbmltcG9ydCBPcGVuSGlzdG9yaWFuRGF0YVNvdXJjZSBmcm9tICcuL29wZW5IaXN0b3JpYW5EYXRhc291cmNlJ1xyXG5pbXBvcnQgT3Blbkhpc3RvcmlhbkRhdGFTb3VyY2VRdWVyeUN0cmwgZnJvbSAnLi9jb250cm9sbGVycy9vcGVuSGlzdG9yaWFuUXVlcnlfY3RybCdcclxuaW1wb3J0IE9wZW5IaXN0b3JpYW5Db25maWdDdHJsIGZyb20gJy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhbkNvbmZpZ19jdHJsJ1xyXG5pbXBvcnQgT3Blbkhpc3RvcmlhblF1ZXJ5T3B0aW9uc0N0cmwgZnJvbSAnLi9jb250cm9sbGVycy9vcGVuSGlzdG9yaWFuUXVlcnlPcHRpb25zX2N0cmwnXHJcbmltcG9ydCBPcGVuSGlzdG9yaWFuQW5ub3RhdGlvbnNRdWVyeUN0cmwgZnJvbSAnLi9jb250cm9sbGVycy9vcGVuSGlzdG9yaWFuQW5ub3RhdGlvbnNfY3RybCdcclxuaW1wb3J0IE9wZW5IaXN0b3JpYW5FbGVtZW50UGlja2VyQ3RybCBmcm9tICcuL2NvbnRyb2xsZXJzL29wZW5IaXN0b3JpYW5FbGVtZW50UGlja2VyX2N0cmwnXHJcbmltcG9ydCBPcGVuSGlzdG9yaWFuVGV4dEVkaXRvckN0cmwgZnJvbSAnLi9jb250cm9sbGVycy9vcGVuSGlzdG9yaWFuVGV4dEVkaXRvcl9jdHJsJ1xyXG5pbXBvcnQgT3Blbkhpc3RvcmlhbkZpbHRlckV4cHJlc3Npb25DdHJsIGZyb20gJy4vY29udHJvbGxlcnMvb3Blbkhpc3RvcmlhbkZpbHRlckV4cHJlc3Npb25fY3RybCdcclxuXHJcbi8vaW1wb3J0IGFuZ3VsYXIgZnJvbSBcImFuZ3VsYXJcIlxyXG5leHBvcnQge1xyXG4gICAgT3Blbkhpc3RvcmlhbkRhdGFTb3VyY2UgYXMgRGF0YXNvdXJjZSxcclxuICAgIE9wZW5IaXN0b3JpYW5EYXRhU291cmNlUXVlcnlDdHJsIGFzIFF1ZXJ5Q3RybCxcclxuICAgIE9wZW5IaXN0b3JpYW5Db25maWdDdHJsIGFzIENvbmZpZ0N0cmwsXHJcbiAgICBPcGVuSGlzdG9yaWFuUXVlcnlPcHRpb25zQ3RybCBhcyBRdWVyeU9wdGlvbnNDdHJsLFxyXG4gICAgT3Blbkhpc3RvcmlhbkFubm90YXRpb25zUXVlcnlDdHJsIGFzIEFubm90YXRpb25zUXVlcnlDdHJsXHJcbn1cclxuXHJcbmFuZ3VsYXIubW9kdWxlKCdncmFmYW5hLmRpcmVjdGl2ZXMnKS5kaXJlY3RpdmUoXCJxdWVyeU9wdGlvbnNcIiwgZnVuY3Rpb24oKSB7XHJcbiAgcmV0dXJuIHtcclxuICAgIHRlbXBsYXRlVXJsOiAncHVibGljL3BsdWdpbnMvZ3JpZHByb3RlY3Rpb25hbGxpYW5jZS1vcGVuaGlzdG9yaWFuLWRhdGFzb3VyY2UvcGFydGlhbC9xdWVyeS5vcHRpb25zLmh0bWwnLFxyXG4gICAgcmVzdHJpY3Q6ICdFJyxcclxuICAgIGNvbnRyb2xsZXI6IE9wZW5IaXN0b3JpYW5RdWVyeU9wdGlvbnNDdHJsLFxyXG4gICAgY29udHJvbGxlckFzOiAncXVlcnlPcHRpb25DdHJsJyxcclxuICAgIHNjb3BlOiB7XHJcbiAgICAgIGZsYWdzOiBcIj1cIixcclxuICAgICAgcmV0dXJuOiBcIj1cIixcclxuICAgIH1cclxuICB9O1xyXG59KTtcclxuXHJcbmFuZ3VsYXIubW9kdWxlKCdncmFmYW5hLmRpcmVjdGl2ZXMnKS5kaXJlY3RpdmUoXCJlbGVtZW50UGlja2VyXCIsIGZ1bmN0aW9uICgpIHtcclxuICAgIHJldHVybiB7XHJcbiAgICAgICAgdGVtcGxhdGVVcmw6ICdwdWJsaWMvcGx1Z2lucy9ncmlkcHJvdGVjdGlvbmFsbGlhbmNlLW9wZW5oaXN0b3JpYW4tZGF0YXNvdXJjZS9wYXJ0aWFsL3F1ZXJ5LmVsZW1lbnRQaWNrZXIuaHRtbCcsXHJcbiAgICAgICAgcmVzdHJpY3Q6ICdFJyxcclxuICAgICAgICBjb250cm9sbGVyOiBPcGVuSGlzdG9yaWFuRWxlbWVudFBpY2tlckN0cmwsXHJcbiAgICAgICAgY29udHJvbGxlckFzOiAnb3Blbkhpc3RvcmlhbkVsZW1lbnRQaWNrZXJDdHJsJyxcclxuICAgICAgICBzY29wZToge1xyXG4gICAgICAgICAgICB0YXJnZXQ6IFwiPVwiLFxyXG4gICAgICAgICAgICBkYXRhc291cmNlOiBcIj1cIixcclxuICAgICAgICAgICAgcGFuZWw6IFwiPVwiXHJcbiAgICAgICAgfVxyXG4gICAgfTtcclxufSk7XHJcblxyXG5hbmd1bGFyLm1vZHVsZSgnZ3JhZmFuYS5kaXJlY3RpdmVzJykuZGlyZWN0aXZlKFwidGV4dEVkaXRvclwiLCBmdW5jdGlvbiAoKSB7XHJcbiAgICByZXR1cm4ge1xyXG4gICAgICAgIHRlbXBsYXRlVXJsOiAncHVibGljL3BsdWdpbnMvZ3JpZHByb3RlY3Rpb25hbGxpYW5jZS1vcGVuaGlzdG9yaWFuLWRhdGFzb3VyY2UvcGFydGlhbC9xdWVyeS50ZXh0RWRpdG9yLmh0bWwnLFxyXG4gICAgICAgIHJlc3RyaWN0OiAnRScsXHJcbiAgICAgICAgY29udHJvbGxlcjogT3Blbkhpc3RvcmlhblRleHRFZGl0b3JDdHJsLFxyXG4gICAgICAgIGNvbnRyb2xsZXJBczogJ29wZW5IaXN0b3JpYW5UZXh0RWRpdG9yQ3RybCcsXHJcbiAgICAgICAgc2NvcGU6IHtcclxuICAgICAgICAgICAgdGFyZ2V0OiBcIj1cIixcclxuICAgICAgICAgICAgZGF0YXNvdXJjZTogXCI9XCIsXHJcbiAgICAgICAgICAgIHBhbmVsOiBcIj1cIlxyXG4gICAgICAgIH1cclxuICAgIH07XHJcbn0pO1xyXG5cclxuYW5ndWxhci5tb2R1bGUoJ2dyYWZhbmEuZGlyZWN0aXZlcycpLmRpcmVjdGl2ZShcImZpbHRlckV4cHJlc3Npb25cIiwgZnVuY3Rpb24gKCkge1xyXG4gICAgcmV0dXJuIHtcclxuICAgICAgICB0ZW1wbGF0ZVVybDogJ3B1YmxpYy9wbHVnaW5zL2dyaWRwcm90ZWN0aW9uYWxsaWFuY2Utb3Blbmhpc3Rvcmlhbi1kYXRhc291cmNlL3BhcnRpYWwvcXVlcnkuZmlsdGVyRXhwcmVzc2lvbi5odG1sJyxcclxuICAgICAgICByZXN0cmljdDogJ0UnLFxyXG4gICAgICAgIGNvbnRyb2xsZXI6IE9wZW5IaXN0b3JpYW5GaWx0ZXJFeHByZXNzaW9uQ3RybCxcclxuICAgICAgICBjb250cm9sbGVyQXM6ICdvcGVuSGlzdG9yaWFuRmlsdGVyRXhwcmVzc2lvbkN0cmwnLFxyXG4gICAgICAgIHNjb3BlOiB7XHJcbiAgICAgICAgICAgIHRhcmdldDogXCI9XCIsXHJcbiAgICAgICAgICAgIGRhdGFzb3VyY2U6IFwiPVwiLFxyXG4gICAgICAgICAgICBwYW5lbDogXCI9XCJcclxuICAgICAgICB9XHJcbiAgICB9O1xyXG59KTsiLCJleHBvcnRzID0gbW9kdWxlLmV4cG9ydHMgPSByZXF1aXJlKFwiLi4vbm9kZV9tb2R1bGVzL2Nzcy1sb2FkZXIvZGlzdC9ydW50aW1lL2FwaS5qc1wiKShmYWxzZSk7XG4vLyBNb2R1bGVcbmV4cG9ydHMucHVzaChbbW9kdWxlLmlkLCBcIi5nZW5lcmljLWRhdGFzb3VyY2UtcXVlcnktcm93IC5xdWVyeS1rZXl3b3JkIHtcXHJcXG4gIHdpZHRoOiA3NXB4O1xcclxcbn1cIiwgXCJcIl0pO1xuIiwiXCJ1c2Ugc3RyaWN0XCI7XG5cbi8qXG4gIE1JVCBMaWNlbnNlIGh0dHA6Ly93d3cub3BlbnNvdXJjZS5vcmcvbGljZW5zZXMvbWl0LWxpY2Vuc2UucGhwXG4gIEF1dGhvciBUb2JpYXMgS29wcGVycyBAc29rcmFcbiovXG4vLyBjc3MgYmFzZSBjb2RlLCBpbmplY3RlZCBieSB0aGUgY3NzLWxvYWRlclxuLy8gZXNsaW50LWRpc2FibGUtbmV4dC1saW5lIGZ1bmMtbmFtZXNcbm1vZHVsZS5leHBvcnRzID0gZnVuY3Rpb24gKHVzZVNvdXJjZU1hcCkge1xuICB2YXIgbGlzdCA9IFtdOyAvLyByZXR1cm4gdGhlIGxpc3Qgb2YgbW9kdWxlcyBhcyBjc3Mgc3RyaW5nXG5cbiAgbGlzdC50b1N0cmluZyA9IGZ1bmN0aW9uIHRvU3RyaW5nKCkge1xuICAgIHJldHVybiB0aGlzLm1hcChmdW5jdGlvbiAoaXRlbSkge1xuICAgICAgdmFyIGNvbnRlbnQgPSBjc3NXaXRoTWFwcGluZ1RvU3RyaW5nKGl0ZW0sIHVzZVNvdXJjZU1hcCk7XG5cbiAgICAgIGlmIChpdGVtWzJdKSB7XG4gICAgICAgIHJldHVybiBcIkBtZWRpYSBcIi5jb25jYXQoaXRlbVsyXSwgXCJ7XCIpLmNvbmNhdChjb250ZW50LCBcIn1cIik7XG4gICAgICB9XG5cbiAgICAgIHJldHVybiBjb250ZW50O1xuICAgIH0pLmpvaW4oJycpO1xuICB9OyAvLyBpbXBvcnQgYSBsaXN0IG9mIG1vZHVsZXMgaW50byB0aGUgbGlzdFxuICAvLyBlc2xpbnQtZGlzYWJsZS1uZXh0LWxpbmUgZnVuYy1uYW1lc1xuXG5cbiAgbGlzdC5pID0gZnVuY3Rpb24gKG1vZHVsZXMsIG1lZGlhUXVlcnkpIHtcbiAgICBpZiAodHlwZW9mIG1vZHVsZXMgPT09ICdzdHJpbmcnKSB7XG4gICAgICAvLyBlc2xpbnQtZGlzYWJsZS1uZXh0LWxpbmUgbm8tcGFyYW0tcmVhc3NpZ25cbiAgICAgIG1vZHVsZXMgPSBbW251bGwsIG1vZHVsZXMsICcnXV07XG4gICAgfVxuXG4gICAgdmFyIGFscmVhZHlJbXBvcnRlZE1vZHVsZXMgPSB7fTtcblxuICAgIGZvciAodmFyIGkgPSAwOyBpIDwgdGhpcy5sZW5ndGg7IGkrKykge1xuICAgICAgLy8gZXNsaW50LWRpc2FibGUtbmV4dC1saW5lIHByZWZlci1kZXN0cnVjdHVyaW5nXG4gICAgICB2YXIgaWQgPSB0aGlzW2ldWzBdO1xuXG4gICAgICBpZiAoaWQgIT0gbnVsbCkge1xuICAgICAgICBhbHJlYWR5SW1wb3J0ZWRNb2R1bGVzW2lkXSA9IHRydWU7XG4gICAgICB9XG4gICAgfVxuXG4gICAgZm9yICh2YXIgX2kgPSAwOyBfaSA8IG1vZHVsZXMubGVuZ3RoOyBfaSsrKSB7XG4gICAgICB2YXIgaXRlbSA9IG1vZHVsZXNbX2ldOyAvLyBza2lwIGFscmVhZHkgaW1wb3J0ZWQgbW9kdWxlXG4gICAgICAvLyB0aGlzIGltcGxlbWVudGF0aW9uIGlzIG5vdCAxMDAlIHBlcmZlY3QgZm9yIHdlaXJkIG1lZGlhIHF1ZXJ5IGNvbWJpbmF0aW9uc1xuICAgICAgLy8gd2hlbiBhIG1vZHVsZSBpcyBpbXBvcnRlZCBtdWx0aXBsZSB0aW1lcyB3aXRoIGRpZmZlcmVudCBtZWRpYSBxdWVyaWVzLlxuICAgICAgLy8gSSBob3BlIHRoaXMgd2lsbCBuZXZlciBvY2N1ciAoSGV5IHRoaXMgd2F5IHdlIGhhdmUgc21hbGxlciBidW5kbGVzKVxuXG4gICAgICBpZiAoaXRlbVswXSA9PSBudWxsIHx8ICFhbHJlYWR5SW1wb3J0ZWRNb2R1bGVzW2l0ZW1bMF1dKSB7XG4gICAgICAgIGlmIChtZWRpYVF1ZXJ5ICYmICFpdGVtWzJdKSB7XG4gICAgICAgICAgaXRlbVsyXSA9IG1lZGlhUXVlcnk7XG4gICAgICAgIH0gZWxzZSBpZiAobWVkaWFRdWVyeSkge1xuICAgICAgICAgIGl0ZW1bMl0gPSBcIihcIi5jb25jYXQoaXRlbVsyXSwgXCIpIGFuZCAoXCIpLmNvbmNhdChtZWRpYVF1ZXJ5LCBcIilcIik7XG4gICAgICAgIH1cblxuICAgICAgICBsaXN0LnB1c2goaXRlbSk7XG4gICAgICB9XG4gICAgfVxuICB9O1xuXG4gIHJldHVybiBsaXN0O1xufTtcblxuZnVuY3Rpb24gY3NzV2l0aE1hcHBpbmdUb1N0cmluZyhpdGVtLCB1c2VTb3VyY2VNYXApIHtcbiAgdmFyIGNvbnRlbnQgPSBpdGVtWzFdIHx8ICcnOyAvLyBlc2xpbnQtZGlzYWJsZS1uZXh0LWxpbmUgcHJlZmVyLWRlc3RydWN0dXJpbmdcblxuICB2YXIgY3NzTWFwcGluZyA9IGl0ZW1bM107XG5cbiAgaWYgKCFjc3NNYXBwaW5nKSB7XG4gICAgcmV0dXJuIGNvbnRlbnQ7XG4gIH1cblxuICBpZiAodXNlU291cmNlTWFwICYmIHR5cGVvZiBidG9hID09PSAnZnVuY3Rpb24nKSB7XG4gICAgdmFyIHNvdXJjZU1hcHBpbmcgPSB0b0NvbW1lbnQoY3NzTWFwcGluZyk7XG4gICAgdmFyIHNvdXJjZVVSTHMgPSBjc3NNYXBwaW5nLnNvdXJjZXMubWFwKGZ1bmN0aW9uIChzb3VyY2UpIHtcbiAgICAgIHJldHVybiBcIi8qIyBzb3VyY2VVUkw9XCIuY29uY2F0KGNzc01hcHBpbmcuc291cmNlUm9vdCkuY29uY2F0KHNvdXJjZSwgXCIgKi9cIik7XG4gICAgfSk7XG4gICAgcmV0dXJuIFtjb250ZW50XS5jb25jYXQoc291cmNlVVJMcykuY29uY2F0KFtzb3VyY2VNYXBwaW5nXSkuam9pbignXFxuJyk7XG4gIH1cblxuICByZXR1cm4gW2NvbnRlbnRdLmpvaW4oJ1xcbicpO1xufSAvLyBBZGFwdGVkIGZyb20gY29udmVydC1zb3VyY2UtbWFwIChNSVQpXG5cblxuZnVuY3Rpb24gdG9Db21tZW50KHNvdXJjZU1hcCkge1xuICAvLyBlc2xpbnQtZGlzYWJsZS1uZXh0LWxpbmUgbm8tdW5kZWZcbiAgdmFyIGJhc2U2NCA9IGJ0b2EodW5lc2NhcGUoZW5jb2RlVVJJQ29tcG9uZW50KEpTT04uc3RyaW5naWZ5KHNvdXJjZU1hcCkpKSk7XG4gIHZhciBkYXRhID0gXCJzb3VyY2VNYXBwaW5nVVJMPWRhdGE6YXBwbGljYXRpb24vanNvbjtjaGFyc2V0PXV0Zi04O2Jhc2U2NCxcIi5jb25jYXQoYmFzZTY0KTtcbiAgcmV0dXJuIFwiLyojIFwiLmNvbmNhdChkYXRhLCBcIiAqL1wiKTtcbn0iLCIvLy88cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi9oZWFkZXJzL2NvbW1vbi5kLnRzXCIgLz5cclxuXHJcbmltcG9ydCB7UGFuZWxDdHJsfSBmcm9tICcuL3BhbmVsX2N0cmwnO1xyXG5cclxuY2xhc3MgTWV0cmljc1BhbmVsQ3RybCBleHRlbmRzIFBhbmVsQ3RybCB7XHJcbiAgc2NvcGU6IGFueTtcclxuICBkYXRhc291cmNlOiBhbnk7XHJcbiAgZGF0YXNvdXJjZU5hbWU6IGFueTtcclxuICAkcTogYW55O1xyXG4gICR0aW1lb3V0OiBhbnk7XHJcbiAgZGF0YXNvdXJjZVNydjogYW55O1xyXG4gIHRpbWVTcnY6IGFueTtcclxuICB0ZW1wbGF0ZVNydjogYW55O1xyXG4gIHRpbWluZzogYW55O1xyXG4gIHJhbmdlOiBhbnk7XHJcbiAgaW50ZXJ2YWw6IGFueTtcclxuICBpbnRlcnZhbE1zOiBhbnk7XHJcbiAgcmVzb2x1dGlvbjogYW55O1xyXG4gIHRpbWVJbmZvOiBhbnk7XHJcbiAgc2tpcERhdGFPbkluaXQ6IGJvb2xlYW47XHJcbiAgZGF0YVN0cmVhbTogYW55O1xyXG4gIGRhdGFTdWJzY3JpcHRpb246IGFueTtcclxuICBkYXRhTGlzdDogYW55O1xyXG4gIG5leHRSZWZJZDogc3RyaW5nO1xyXG5cclxuICBjb25zdHJ1Y3Rvcigkc2NvcGUsICRpbmplY3Rvcikge1xyXG4gICAgc3VwZXIoJHNjb3BlLCAkaW5qZWN0b3IpO1xyXG5cclxuICAgIC8vIG1ha2UgbWV0cmljcyB0YWIgdGhlIGRlZmF1bHRcclxuICAgIHRoaXMuZWRpdG9yVGFiSW5kZXggPSAxO1xyXG4gICAgLy8gdGhpcy4kcSA9ICRpbmplY3Rvci5nZXQoJyRxJyk7XHJcbiAgICAvLyB0aGlzLmRhdGFzb3VyY2VTcnYgPSAkaW5qZWN0b3IuZ2V0KCdkYXRhc291cmNlU3J2Jyk7XHJcbiAgICAvLyB0aGlzLnRpbWVTcnYgPSAkaW5qZWN0b3IuZ2V0KCd0aW1lU3J2Jyk7XHJcbiAgICAvLyB0aGlzLnRlbXBsYXRlU3J2ID0gJGluamVjdG9yLmdldCgndGVtcGxhdGVTcnYnKTtcclxuXHJcbiAgICBpZiAoIXRoaXMucGFuZWwudGFyZ2V0cykge1xyXG4gICAgICB0aGlzLnBhbmVsLnRhcmdldHMgPSBbe31dO1xyXG4gICAgfVxyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBvblBhbmVsVGVhckRvd24oKSB7XHJcbiAgfVxyXG5cclxuICBwcml2YXRlIG9uSW5pdE1ldHJpY3NQYW5lbEVkaXRNb2RlKCkge1xyXG4gIH1cclxuXHJcbiAgcHJpdmF0ZSBvbk1ldHJpY3NQYW5lbFJlZnJlc2goKSB7XHJcbiAgfVxyXG5cclxuXHJcbiAgc2V0VGltZVF1ZXJ5U3RhcnQoKSB7XHJcbiAgfVxyXG5cclxuICBzZXRUaW1lUXVlcnlFbmQoKSB7XHJcbiAgfVxyXG5cclxuICB1cGRhdGVUaW1lUmFuZ2UoZGF0YXNvdXJjZT8pIHtcclxuICB9XHJcblxyXG4gIGNhbGN1bGF0ZUludGVydmFsKCkge1xyXG4gIH1cclxuXHJcbiAgYXBwbHlQYW5lbFRpbWVPdmVycmlkZXMoKSB7XHJcbiAgfVxyXG5cclxuICBpc3N1ZVF1ZXJpZXMoZGF0YXNvdXJjZSkge1xyXG4gIH1cclxuXHJcbiAgaGFuZGxlUXVlcnlSZXN1bHQocmVzdWx0KSB7XHJcbiAgfVxyXG5cclxuICBoYW5kbGVEYXRhU3RyZWFtKHN0cmVhbSkge1xyXG4gIH1cclxuXHJcbiAgc2V0RGF0YXNvdXJjZShkYXRhc291cmNlKSB7XHJcbiAgfVxyXG5cclxuICBhZGRRdWVyeSh0YXJnZXQpIHtcclxuICB9XHJcblxyXG4gIHJlbW92ZVF1ZXJ5KHRhcmdldCkge1xyXG4gIH1cclxuXHJcbiAgbW92ZVF1ZXJ5KHRhcmdldCwgZGlyZWN0aW9uKSB7XHJcbiAgfVxyXG59XHJcblxyXG5leHBvcnQge01ldHJpY3NQYW5lbEN0cmx9O1xyXG4iLCIvLy88cmVmZXJlbmNlIHBhdGg9XCIuLi8uLi9oZWFkZXJzL2NvbW1vbi5kLnRzXCIgLz5cclxuXHJcbmV4cG9ydCBjbGFzcyBQYW5lbEN0cmwge1xyXG4gIHBhbmVsOiBhbnk7XHJcbiAgZXJyb3I6IGFueTtcclxuICByb3c6IGFueTtcclxuICBkYXNoYm9hcmQ6IGFueTtcclxuICBlZGl0b3JUYWJJbmRleDogbnVtYmVyO1xyXG4gIHBsdWdpbk5hbWU6IHN0cmluZztcclxuICBwbHVnaW5JZDogc3RyaW5nO1xyXG4gIGVkaXRvclRhYnM6IGFueTtcclxuICAkc2NvcGU6IGFueTtcclxuICAkaW5qZWN0b3I6IGFueTtcclxuICAkdGltZW91dDogYW55O1xyXG4gIGZ1bGxzY3JlZW46IGJvb2xlYW47XHJcbiAgaW5zcGVjdG9yOiBhbnk7XHJcbiAgZWRpdE1vZGVJbml0aWF0ZWQ6IGJvb2xlYW47XHJcbiAgZWRpdG9ySGVscEluZGV4OiBudW1iZXI7XHJcbiAgZWRpdE1vZGU6IGFueTtcclxuICBoZWlnaHQ6IGFueTtcclxuICBjb250YWluZXJIZWlnaHQ6IGFueTtcclxuICBldmVudHM6IGFueTtcclxuICB0aW1pbmc6IGFueTtcclxuICBsb2FkaW5nOiBib29sZWFuO1xyXG5cclxuICBjb25zdHJ1Y3Rvcigkc2NvcGUsICRpbmplY3Rvcikge1xyXG4gIH1cclxuXHJcbiAgaW5pdCgpIHtcclxuICB9XHJcblxyXG4gIHJlbmRlcmluZ0NvbXBsZXRlZCgpIHtcclxuICB9XHJcblxyXG4gIHJlZnJlc2goKSB7XHJcbiAgfVxyXG5cclxuICBwdWJsaXNoQXBwRXZlbnQoZXZ0TmFtZSwgZXZ0KSB7XHJcbiAgfVxyXG5cclxuICBjaGFuZ2VWaWV3KGZ1bGxzY3JlZW4sIGVkaXQpIHtcclxuICB9XHJcblxyXG4gIHZpZXdQYW5lbCgpIHtcclxuICAgIHRoaXMuY2hhbmdlVmlldyh0cnVlLCBmYWxzZSk7XHJcbiAgfVxyXG5cclxuICBlZGl0UGFuZWwoKSB7XHJcbiAgICB0aGlzLmNoYW5nZVZpZXcodHJ1ZSwgdHJ1ZSk7XHJcbiAgfVxyXG5cclxuICBleGl0RnVsbHNjcmVlbigpIHtcclxuICAgIHRoaXMuY2hhbmdlVmlldyhmYWxzZSwgZmFsc2UpO1xyXG4gIH1cclxuXHJcbiAgaW5pdEVkaXRNb2RlKCkge1xyXG4gIH1cclxuXHJcbiAgY2hhbmdlVGFiKG5ld0luZGV4KSB7XHJcbiAgfVxyXG5cclxuICBhZGRFZGl0b3JUYWIodGl0bGUsIGRpcmVjdGl2ZUZuLCBpbmRleD8pIHtcclxuICB9XHJcblxyXG4gIGdldE1lbnUoKSB7XHJcbiAgICByZXR1cm4gW107XHJcbiAgfVxyXG5cclxuICBnZXRFeHRlbmRlZE1lbnUoKSB7XHJcbiAgICByZXR1cm4gW107XHJcbiAgfVxyXG5cclxuICBvdGhlclBhbmVsSW5GdWxsc2NyZWVuTW9kZSgpIHtcclxuICAgIHJldHVybiBmYWxzZTtcclxuICB9XHJcblxyXG4gIGNhbGN1bGF0ZVBhbmVsSGVpZ2h0KCkge1xyXG4gIH1cclxuXHJcbiAgcmVuZGVyKHBheWxvYWQ/KSB7XHJcbiAgfVxyXG5cclxuICB0b2dnbGVFZGl0b3JIZWxwKGluZGV4KSB7XHJcbiAgfVxyXG5cclxuICBkdXBsaWNhdGUoKSB7XHJcbiAgfVxyXG5cclxuICB1cGRhdGVDb2x1bW5TcGFuKHNwYW4pIHtcclxuICB9XHJcblxyXG4gIHJlbW92ZVBhbmVsKCkge1xyXG4gIH1cclxuXHJcbiAgZWRpdFBhbmVsSnNvbigpIHtcclxuICB9XHJcblxyXG4gIHJlcGxhY2VQYW5lbChuZXdQYW5lbCwgb2xkUGFuZWwpIHtcclxuICB9XHJcblxyXG4gIHNoYXJlUGFuZWwoKSB7XHJcbiAgfVxyXG5cclxuICBnZXRJbmZvTW9kZSgpIHtcclxuICB9XHJcblxyXG4gIGdldEluZm9Db250ZW50KG9wdGlvbnMpIHtcclxuICB9XHJcblxyXG4gIG9wZW5JbnNwZWN0b3IoKSB7XHJcbiAgfVxyXG59XHJcbiIsIi8vLzxyZWZlcmVuY2UgcGF0aD1cIi4uLy4uL2hlYWRlcnMvY29tbW9uLmQudHNcIiAvPlxyXG5cclxuZXhwb3J0IGNsYXNzIFF1ZXJ5Q3RybCB7XHJcbiAgdGFyZ2V0OiBhbnk7XHJcbiAgZGF0YXNvdXJjZTogYW55O1xyXG4gIHBhbmVsQ3RybDogYW55O1xyXG4gIHBhbmVsOiBhbnk7XHJcbiAgaGFzUmF3TW9kZTogYm9vbGVhbjtcclxuICBlcnJvcjogc3RyaW5nO1xyXG5cclxuICBjb25zdHJ1Y3RvcihwdWJsaWMgJHNjb3BlLCBwcml2YXRlICRpbmplY3Rvcikge1xyXG4gICAgdGhpcy5wYW5lbEN0cmwgPSB0aGlzLnBhbmVsQ3RybCB8fCB7cGFuZWw6IHt9fTtcclxuICAgIHRoaXMudGFyZ2V0ID0gdGhpcy50YXJnZXQgfHwge3RhcmdldDogJyd9O1xyXG4gICAgdGhpcy5wYW5lbCA9IHRoaXMucGFuZWxDdHJsLnBhbmVsO1xyXG4gIH1cclxuXHJcbiAgcmVmcmVzaCgpIHt9XHJcbn1cclxuIiwiaW1wb3J0IHtQYW5lbEN0cmx9IGZyb20gJy4uL2ZlYXR1cmVzL3BhbmVsL3BhbmVsX2N0cmwnO1xyXG5pbXBvcnQge01ldHJpY3NQYW5lbEN0cmx9IGZyb20gJy4uL2ZlYXR1cmVzL3BhbmVsL21ldHJpY3NfcGFuZWxfY3RybCc7XHJcbmltcG9ydCB7UXVlcnlDdHJsfSBmcm9tICcuLi9mZWF0dXJlcy9wYW5lbC9xdWVyeV9jdHJsJztcclxuXHJcbmV4cG9ydCBmdW5jdGlvbiBsb2FkUGx1Z2luQ3NzKG9wdGlvbnMpIHtcclxufVxyXG5cclxuZXhwb3J0IHtcclxuICBQYW5lbEN0cmwsXHJcbiAgTWV0cmljc1BhbmVsQ3RybCxcclxuICBRdWVyeUN0cmwsXHJcbn07XHJcbiIsIlwidXNlIHN0cmljdFwiO1xuXG52YXIgc3R5bGVzSW5Eb20gPSB7fTtcblxudmFyIGlzT2xkSUUgPSBmdW5jdGlvbiBpc09sZElFKCkge1xuICB2YXIgbWVtbztcbiAgcmV0dXJuIGZ1bmN0aW9uIG1lbW9yaXplKCkge1xuICAgIGlmICh0eXBlb2YgbWVtbyA9PT0gJ3VuZGVmaW5lZCcpIHtcbiAgICAgIC8vIFRlc3QgZm9yIElFIDw9IDkgYXMgcHJvcG9zZWQgYnkgQnJvd3NlcmhhY2tzXG4gICAgICAvLyBAc2VlIGh0dHA6Ly9icm93c2VyaGFja3MuY29tLyNoYWNrLWU3MWQ4NjkyZjY1MzM0MTczZmVlNzE1YzIyMmNiODA1XG4gICAgICAvLyBUZXN0cyBmb3IgZXhpc3RlbmNlIG9mIHN0YW5kYXJkIGdsb2JhbHMgaXMgdG8gYWxsb3cgc3R5bGUtbG9hZGVyXG4gICAgICAvLyB0byBvcGVyYXRlIGNvcnJlY3RseSBpbnRvIG5vbi1zdGFuZGFyZCBlbnZpcm9ubWVudHNcbiAgICAgIC8vIEBzZWUgaHR0cHM6Ly9naXRodWIuY29tL3dlYnBhY2stY29udHJpYi9zdHlsZS1sb2FkZXIvaXNzdWVzLzE3N1xuICAgICAgbWVtbyA9IEJvb2xlYW4od2luZG93ICYmIGRvY3VtZW50ICYmIGRvY3VtZW50LmFsbCAmJiAhd2luZG93LmF0b2IpO1xuICAgIH1cblxuICAgIHJldHVybiBtZW1vO1xuICB9O1xufSgpO1xuXG52YXIgZ2V0VGFyZ2V0ID0gZnVuY3Rpb24gZ2V0VGFyZ2V0KCkge1xuICB2YXIgbWVtbyA9IHt9O1xuICByZXR1cm4gZnVuY3Rpb24gbWVtb3JpemUodGFyZ2V0KSB7XG4gICAgaWYgKHR5cGVvZiBtZW1vW3RhcmdldF0gPT09ICd1bmRlZmluZWQnKSB7XG4gICAgICB2YXIgc3R5bGVUYXJnZXQgPSBkb2N1bWVudC5xdWVyeVNlbGVjdG9yKHRhcmdldCk7IC8vIFNwZWNpYWwgY2FzZSB0byByZXR1cm4gaGVhZCBvZiBpZnJhbWUgaW5zdGVhZCBvZiBpZnJhbWUgaXRzZWxmXG5cbiAgICAgIGlmICh3aW5kb3cuSFRNTElGcmFtZUVsZW1lbnQgJiYgc3R5bGVUYXJnZXQgaW5zdGFuY2VvZiB3aW5kb3cuSFRNTElGcmFtZUVsZW1lbnQpIHtcbiAgICAgICAgdHJ5IHtcbiAgICAgICAgICAvLyBUaGlzIHdpbGwgdGhyb3cgYW4gZXhjZXB0aW9uIGlmIGFjY2VzcyB0byBpZnJhbWUgaXMgYmxvY2tlZFxuICAgICAgICAgIC8vIGR1ZSB0byBjcm9zcy1vcmlnaW4gcmVzdHJpY3Rpb25zXG4gICAgICAgICAgc3R5bGVUYXJnZXQgPSBzdHlsZVRhcmdldC5jb250ZW50RG9jdW1lbnQuaGVhZDtcbiAgICAgICAgfSBjYXRjaCAoZSkge1xuICAgICAgICAgIC8vIGlzdGFuYnVsIGlnbm9yZSBuZXh0XG4gICAgICAgICAgc3R5bGVUYXJnZXQgPSBudWxsO1xuICAgICAgICB9XG4gICAgICB9XG5cbiAgICAgIG1lbW9bdGFyZ2V0XSA9IHN0eWxlVGFyZ2V0O1xuICAgIH1cblxuICAgIHJldHVybiBtZW1vW3RhcmdldF07XG4gIH07XG59KCk7XG5cbmZ1bmN0aW9uIGxpc3RUb1N0eWxlcyhsaXN0LCBvcHRpb25zKSB7XG4gIHZhciBzdHlsZXMgPSBbXTtcbiAgdmFyIG5ld1N0eWxlcyA9IHt9O1xuXG4gIGZvciAodmFyIGkgPSAwOyBpIDwgbGlzdC5sZW5ndGg7IGkrKykge1xuICAgIHZhciBpdGVtID0gbGlzdFtpXTtcbiAgICB2YXIgaWQgPSBvcHRpb25zLmJhc2UgPyBpdGVtWzBdICsgb3B0aW9ucy5iYXNlIDogaXRlbVswXTtcbiAgICB2YXIgY3NzID0gaXRlbVsxXTtcbiAgICB2YXIgbWVkaWEgPSBpdGVtWzJdO1xuICAgIHZhciBzb3VyY2VNYXAgPSBpdGVtWzNdO1xuICAgIHZhciBwYXJ0ID0ge1xuICAgICAgY3NzOiBjc3MsXG4gICAgICBtZWRpYTogbWVkaWEsXG4gICAgICBzb3VyY2VNYXA6IHNvdXJjZU1hcFxuICAgIH07XG5cbiAgICBpZiAoIW5ld1N0eWxlc1tpZF0pIHtcbiAgICAgIHN0eWxlcy5wdXNoKG5ld1N0eWxlc1tpZF0gPSB7XG4gICAgICAgIGlkOiBpZCxcbiAgICAgICAgcGFydHM6IFtwYXJ0XVxuICAgICAgfSk7XG4gICAgfSBlbHNlIHtcbiAgICAgIG5ld1N0eWxlc1tpZF0ucGFydHMucHVzaChwYXJ0KTtcbiAgICB9XG4gIH1cblxuICByZXR1cm4gc3R5bGVzO1xufVxuXG5mdW5jdGlvbiBhZGRTdHlsZXNUb0RvbShzdHlsZXMsIG9wdGlvbnMpIHtcbiAgZm9yICh2YXIgaSA9IDA7IGkgPCBzdHlsZXMubGVuZ3RoOyBpKyspIHtcbiAgICB2YXIgaXRlbSA9IHN0eWxlc1tpXTtcbiAgICB2YXIgZG9tU3R5bGUgPSBzdHlsZXNJbkRvbVtpdGVtLmlkXTtcbiAgICB2YXIgaiA9IDA7XG5cbiAgICBpZiAoZG9tU3R5bGUpIHtcbiAgICAgIGRvbVN0eWxlLnJlZnMrKztcblxuICAgICAgZm9yICg7IGogPCBkb21TdHlsZS5wYXJ0cy5sZW5ndGg7IGorKykge1xuICAgICAgICBkb21TdHlsZS5wYXJ0c1tqXShpdGVtLnBhcnRzW2pdKTtcbiAgICAgIH1cblxuICAgICAgZm9yICg7IGogPCBpdGVtLnBhcnRzLmxlbmd0aDsgaisrKSB7XG4gICAgICAgIGRvbVN0eWxlLnBhcnRzLnB1c2goYWRkU3R5bGUoaXRlbS5wYXJ0c1tqXSwgb3B0aW9ucykpO1xuICAgICAgfVxuICAgIH0gZWxzZSB7XG4gICAgICB2YXIgcGFydHMgPSBbXTtcblxuICAgICAgZm9yICg7IGogPCBpdGVtLnBhcnRzLmxlbmd0aDsgaisrKSB7XG4gICAgICAgIHBhcnRzLnB1c2goYWRkU3R5bGUoaXRlbS5wYXJ0c1tqXSwgb3B0aW9ucykpO1xuICAgICAgfVxuXG4gICAgICBzdHlsZXNJbkRvbVtpdGVtLmlkXSA9IHtcbiAgICAgICAgaWQ6IGl0ZW0uaWQsXG4gICAgICAgIHJlZnM6IDEsXG4gICAgICAgIHBhcnRzOiBwYXJ0c1xuICAgICAgfTtcbiAgICB9XG4gIH1cbn1cblxuZnVuY3Rpb24gaW5zZXJ0U3R5bGVFbGVtZW50KG9wdGlvbnMpIHtcbiAgdmFyIHN0eWxlID0gZG9jdW1lbnQuY3JlYXRlRWxlbWVudCgnc3R5bGUnKTtcblxuICBpZiAodHlwZW9mIG9wdGlvbnMuYXR0cmlidXRlcy5ub25jZSA9PT0gJ3VuZGVmaW5lZCcpIHtcbiAgICB2YXIgbm9uY2UgPSB0eXBlb2YgX193ZWJwYWNrX25vbmNlX18gIT09ICd1bmRlZmluZWQnID8gX193ZWJwYWNrX25vbmNlX18gOiBudWxsO1xuXG4gICAgaWYgKG5vbmNlKSB7XG4gICAgICBvcHRpb25zLmF0dHJpYnV0ZXMubm9uY2UgPSBub25jZTtcbiAgICB9XG4gIH1cblxuICBPYmplY3Qua2V5cyhvcHRpb25zLmF0dHJpYnV0ZXMpLmZvckVhY2goZnVuY3Rpb24gKGtleSkge1xuICAgIHN0eWxlLnNldEF0dHJpYnV0ZShrZXksIG9wdGlvbnMuYXR0cmlidXRlc1trZXldKTtcbiAgfSk7XG5cbiAgaWYgKHR5cGVvZiBvcHRpb25zLmluc2VydCA9PT0gJ2Z1bmN0aW9uJykge1xuICAgIG9wdGlvbnMuaW5zZXJ0KHN0eWxlKTtcbiAgfSBlbHNlIHtcbiAgICB2YXIgdGFyZ2V0ID0gZ2V0VGFyZ2V0KG9wdGlvbnMuaW5zZXJ0IHx8ICdoZWFkJyk7XG5cbiAgICBpZiAoIXRhcmdldCkge1xuICAgICAgdGhyb3cgbmV3IEVycm9yKFwiQ291bGRuJ3QgZmluZCBhIHN0eWxlIHRhcmdldC4gVGhpcyBwcm9iYWJseSBtZWFucyB0aGF0IHRoZSB2YWx1ZSBmb3IgdGhlICdpbnNlcnQnIHBhcmFtZXRlciBpcyBpbnZhbGlkLlwiKTtcbiAgICB9XG5cbiAgICB0YXJnZXQuYXBwZW5kQ2hpbGQoc3R5bGUpO1xuICB9XG5cbiAgcmV0dXJuIHN0eWxlO1xufVxuXG5mdW5jdGlvbiByZW1vdmVTdHlsZUVsZW1lbnQoc3R5bGUpIHtcbiAgLy8gaXN0YW5idWwgaWdub3JlIGlmXG4gIGlmIChzdHlsZS5wYXJlbnROb2RlID09PSBudWxsKSB7XG4gICAgcmV0dXJuIGZhbHNlO1xuICB9XG5cbiAgc3R5bGUucGFyZW50Tm9kZS5yZW1vdmVDaGlsZChzdHlsZSk7XG59XG4vKiBpc3RhbmJ1bCBpZ25vcmUgbmV4dCAgKi9cblxuXG52YXIgcmVwbGFjZVRleHQgPSBmdW5jdGlvbiByZXBsYWNlVGV4dCgpIHtcbiAgdmFyIHRleHRTdG9yZSA9IFtdO1xuICByZXR1cm4gZnVuY3Rpb24gcmVwbGFjZShpbmRleCwgcmVwbGFjZW1lbnQpIHtcbiAgICB0ZXh0U3RvcmVbaW5kZXhdID0gcmVwbGFjZW1lbnQ7XG4gICAgcmV0dXJuIHRleHRTdG9yZS5maWx0ZXIoQm9vbGVhbikuam9pbignXFxuJyk7XG4gIH07XG59KCk7XG5cbmZ1bmN0aW9uIGFwcGx5VG9TaW5nbGV0b25UYWcoc3R5bGUsIGluZGV4LCByZW1vdmUsIG9iaikge1xuICB2YXIgY3NzID0gcmVtb3ZlID8gJycgOiBvYmouY3NzOyAvLyBGb3Igb2xkIElFXG5cbiAgLyogaXN0YW5idWwgaWdub3JlIGlmICAqL1xuXG4gIGlmIChzdHlsZS5zdHlsZVNoZWV0KSB7XG4gICAgc3R5bGUuc3R5bGVTaGVldC5jc3NUZXh0ID0gcmVwbGFjZVRleHQoaW5kZXgsIGNzcyk7XG4gIH0gZWxzZSB7XG4gICAgdmFyIGNzc05vZGUgPSBkb2N1bWVudC5jcmVhdGVUZXh0Tm9kZShjc3MpO1xuICAgIHZhciBjaGlsZE5vZGVzID0gc3R5bGUuY2hpbGROb2RlcztcblxuICAgIGlmIChjaGlsZE5vZGVzW2luZGV4XSkge1xuICAgICAgc3R5bGUucmVtb3ZlQ2hpbGQoY2hpbGROb2Rlc1tpbmRleF0pO1xuICAgIH1cblxuICAgIGlmIChjaGlsZE5vZGVzLmxlbmd0aCkge1xuICAgICAgc3R5bGUuaW5zZXJ0QmVmb3JlKGNzc05vZGUsIGNoaWxkTm9kZXNbaW5kZXhdKTtcbiAgICB9IGVsc2Uge1xuICAgICAgc3R5bGUuYXBwZW5kQ2hpbGQoY3NzTm9kZSk7XG4gICAgfVxuICB9XG59XG5cbmZ1bmN0aW9uIGFwcGx5VG9UYWcoc3R5bGUsIG9wdGlvbnMsIG9iaikge1xuICB2YXIgY3NzID0gb2JqLmNzcztcbiAgdmFyIG1lZGlhID0gb2JqLm1lZGlhO1xuICB2YXIgc291cmNlTWFwID0gb2JqLnNvdXJjZU1hcDtcblxuICBpZiAobWVkaWEpIHtcbiAgICBzdHlsZS5zZXRBdHRyaWJ1dGUoJ21lZGlhJywgbWVkaWEpO1xuICB9XG5cbiAgaWYgKHNvdXJjZU1hcCAmJiBidG9hKSB7XG4gICAgY3NzICs9IFwiXFxuLyojIHNvdXJjZU1hcHBpbmdVUkw9ZGF0YTphcHBsaWNhdGlvbi9qc29uO2Jhc2U2NCxcIi5jb25jYXQoYnRvYSh1bmVzY2FwZShlbmNvZGVVUklDb21wb25lbnQoSlNPTi5zdHJpbmdpZnkoc291cmNlTWFwKSkpKSwgXCIgKi9cIik7XG4gIH0gLy8gRm9yIG9sZCBJRVxuXG4gIC8qIGlzdGFuYnVsIGlnbm9yZSBpZiAgKi9cblxuXG4gIGlmIChzdHlsZS5zdHlsZVNoZWV0KSB7XG4gICAgc3R5bGUuc3R5bGVTaGVldC5jc3NUZXh0ID0gY3NzO1xuICB9IGVsc2Uge1xuICAgIHdoaWxlIChzdHlsZS5maXJzdENoaWxkKSB7XG4gICAgICBzdHlsZS5yZW1vdmVDaGlsZChzdHlsZS5maXJzdENoaWxkKTtcbiAgICB9XG5cbiAgICBzdHlsZS5hcHBlbmRDaGlsZChkb2N1bWVudC5jcmVhdGVUZXh0Tm9kZShjc3MpKTtcbiAgfVxufVxuXG52YXIgc2luZ2xldG9uID0gbnVsbDtcbnZhciBzaW5nbGV0b25Db3VudGVyID0gMDtcblxuZnVuY3Rpb24gYWRkU3R5bGUob2JqLCBvcHRpb25zKSB7XG4gIHZhciBzdHlsZTtcbiAgdmFyIHVwZGF0ZTtcbiAgdmFyIHJlbW92ZTtcblxuICBpZiAob3B0aW9ucy5zaW5nbGV0b24pIHtcbiAgICB2YXIgc3R5bGVJbmRleCA9IHNpbmdsZXRvbkNvdW50ZXIrKztcbiAgICBzdHlsZSA9IHNpbmdsZXRvbiB8fCAoc2luZ2xldG9uID0gaW5zZXJ0U3R5bGVFbGVtZW50KG9wdGlvbnMpKTtcbiAgICB1cGRhdGUgPSBhcHBseVRvU2luZ2xldG9uVGFnLmJpbmQobnVsbCwgc3R5bGUsIHN0eWxlSW5kZXgsIGZhbHNlKTtcbiAgICByZW1vdmUgPSBhcHBseVRvU2luZ2xldG9uVGFnLmJpbmQobnVsbCwgc3R5bGUsIHN0eWxlSW5kZXgsIHRydWUpO1xuICB9IGVsc2Uge1xuICAgIHN0eWxlID0gaW5zZXJ0U3R5bGVFbGVtZW50KG9wdGlvbnMpO1xuICAgIHVwZGF0ZSA9IGFwcGx5VG9UYWcuYmluZChudWxsLCBzdHlsZSwgb3B0aW9ucyk7XG5cbiAgICByZW1vdmUgPSBmdW5jdGlvbiByZW1vdmUoKSB7XG4gICAgICByZW1vdmVTdHlsZUVsZW1lbnQoc3R5bGUpO1xuICAgIH07XG4gIH1cblxuICB1cGRhdGUob2JqKTtcbiAgcmV0dXJuIGZ1bmN0aW9uIHVwZGF0ZVN0eWxlKG5ld09iaikge1xuICAgIGlmIChuZXdPYmopIHtcbiAgICAgIGlmIChuZXdPYmouY3NzID09PSBvYmouY3NzICYmIG5ld09iai5tZWRpYSA9PT0gb2JqLm1lZGlhICYmIG5ld09iai5zb3VyY2VNYXAgPT09IG9iai5zb3VyY2VNYXApIHtcbiAgICAgICAgcmV0dXJuO1xuICAgICAgfVxuXG4gICAgICB1cGRhdGUob2JqID0gbmV3T2JqKTtcbiAgICB9IGVsc2Uge1xuICAgICAgcmVtb3ZlKCk7XG4gICAgfVxuICB9O1xufVxuXG5tb2R1bGUuZXhwb3J0cyA9IGZ1bmN0aW9uIChsaXN0LCBvcHRpb25zKSB7XG4gIG9wdGlvbnMgPSBvcHRpb25zIHx8IHt9O1xuICBvcHRpb25zLmF0dHJpYnV0ZXMgPSB0eXBlb2Ygb3B0aW9ucy5hdHRyaWJ1dGVzID09PSAnb2JqZWN0JyA/IG9wdGlvbnMuYXR0cmlidXRlcyA6IHt9OyAvLyBGb3JjZSBzaW5nbGUtdGFnIHNvbHV0aW9uIG9uIElFNi05LCB3aGljaCBoYXMgYSBoYXJkIGxpbWl0IG9uIHRoZSAjIG9mIDxzdHlsZT5cbiAgLy8gdGFncyBpdCB3aWxsIGFsbG93IG9uIGEgcGFnZVxuXG4gIGlmICghb3B0aW9ucy5zaW5nbGV0b24gJiYgdHlwZW9mIG9wdGlvbnMuc2luZ2xldG9uICE9PSAnYm9vbGVhbicpIHtcbiAgICBvcHRpb25zLnNpbmdsZXRvbiA9IGlzT2xkSUUoKTtcbiAgfVxuXG4gIHZhciBzdHlsZXMgPSBsaXN0VG9TdHlsZXMobGlzdCwgb3B0aW9ucyk7XG4gIGFkZFN0eWxlc1RvRG9tKHN0eWxlcywgb3B0aW9ucyk7XG4gIHJldHVybiBmdW5jdGlvbiB1cGRhdGUobmV3TGlzdCkge1xuICAgIHZhciBtYXlSZW1vdmUgPSBbXTtcblxuICAgIGZvciAodmFyIGkgPSAwOyBpIDwgc3R5bGVzLmxlbmd0aDsgaSsrKSB7XG4gICAgICB2YXIgaXRlbSA9IHN0eWxlc1tpXTtcbiAgICAgIHZhciBkb21TdHlsZSA9IHN0eWxlc0luRG9tW2l0ZW0uaWRdO1xuXG4gICAgICBpZiAoZG9tU3R5bGUpIHtcbiAgICAgICAgZG9tU3R5bGUucmVmcy0tO1xuICAgICAgICBtYXlSZW1vdmUucHVzaChkb21TdHlsZSk7XG4gICAgICB9XG4gICAgfVxuXG4gICAgaWYgKG5ld0xpc3QpIHtcbiAgICAgIHZhciBuZXdTdHlsZXMgPSBsaXN0VG9TdHlsZXMobmV3TGlzdCwgb3B0aW9ucyk7XG4gICAgICBhZGRTdHlsZXNUb0RvbShuZXdTdHlsZXMsIG9wdGlvbnMpO1xuICAgIH1cblxuICAgIGZvciAodmFyIF9pID0gMDsgX2kgPCBtYXlSZW1vdmUubGVuZ3RoOyBfaSsrKSB7XG4gICAgICB2YXIgX2RvbVN0eWxlID0gbWF5UmVtb3ZlW19pXTtcblxuICAgICAgaWYgKF9kb21TdHlsZS5yZWZzID09PSAwKSB7XG4gICAgICAgIGZvciAodmFyIGogPSAwOyBqIDwgX2RvbVN0eWxlLnBhcnRzLmxlbmd0aDsgaisrKSB7XG4gICAgICAgICAgX2RvbVN0eWxlLnBhcnRzW2pdKCk7XG4gICAgICAgIH1cblxuICAgICAgICBkZWxldGUgc3R5bGVzSW5Eb21bX2RvbVN0eWxlLmlkXTtcbiAgICAgIH1cbiAgICB9XG4gIH07XG59OyIsInZhciBnO1xuXG4vLyBUaGlzIHdvcmtzIGluIG5vbi1zdHJpY3QgbW9kZVxuZyA9IChmdW5jdGlvbigpIHtcblx0cmV0dXJuIHRoaXM7XG59KSgpO1xuXG50cnkge1xuXHQvLyBUaGlzIHdvcmtzIGlmIGV2YWwgaXMgYWxsb3dlZCAoc2VlIENTUClcblx0ZyA9IGcgfHwgbmV3IEZ1bmN0aW9uKFwicmV0dXJuIHRoaXNcIikoKTtcbn0gY2F0Y2ggKGUpIHtcblx0Ly8gVGhpcyB3b3JrcyBpZiB0aGUgd2luZG93IHJlZmVyZW5jZSBpcyBhdmFpbGFibGVcblx0aWYgKHR5cGVvZiB3aW5kb3cgPT09IFwib2JqZWN0XCIpIGcgPSB3aW5kb3c7XG59XG5cbi8vIGcgY2FuIHN0aWxsIGJlIHVuZGVmaW5lZCwgYnV0IG5vdGhpbmcgdG8gZG8gYWJvdXQgaXQuLi5cbi8vIFdlIHJldHVybiB1bmRlZmluZWQsIGluc3RlYWQgb2Ygbm90aGluZyBoZXJlLCBzbyBpdCdzXG4vLyBlYXNpZXIgdG8gaGFuZGxlIHRoaXMgY2FzZS4gaWYoIWdsb2JhbCkgeyAuLi59XG5cbm1vZHVsZS5leHBvcnRzID0gZztcbiIsIi8vKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqXHJcbi8vICBkYXRhc291cmNlLmpzIC0gR2J0Y1xyXG4vL1xyXG4vLyAgQ29weXJpZ2h0IO+/vSAyMDE3LCBHcmlkIFByb3RlY3Rpb24gQWxsaWFuY2UuICBBbGwgUmlnaHRzIFJlc2VydmVkLlxyXG4vL1xyXG4vLyAgTGljZW5zZWQgdG8gdGhlIEdyaWQgUHJvdGVjdGlvbiBBbGxpYW5jZSAoR1BBKSB1bmRlciBvbmUgb3IgbW9yZSBjb250cmlidXRvciBsaWNlbnNlIGFncmVlbWVudHMuIFNlZVxyXG4vLyAgdGhlIE5PVElDRSBmaWxlIGRpc3RyaWJ1dGVkIHdpdGggdGhpcyB3b3JrIGZvciBhZGRpdGlvbmFsIGluZm9ybWF0aW9uIHJlZ2FyZGluZyBjb3B5cmlnaHQgb3duZXJzaGlwLlxyXG4vLyAgVGhlIEdQQSBsaWNlbnNlcyB0aGlzIGZpbGUgdG8geW91IHVuZGVyIHRoZSBNSVQgTGljZW5zZSAoTUlUKSwgdGhlIFwiTGljZW5zZVwiOyB5b3UgbWF5IG5vdCB1c2UgdGhpc1xyXG4vLyAgZmlsZSBleGNlcHQgaW4gY29tcGxpYW5jZSB3aXRoIHRoZSBMaWNlbnNlLiBZb3UgbWF5IG9idGFpbiBhIGNvcHkgb2YgdGhlIExpY2Vuc2UgYXQ6XHJcbi8vXHJcbi8vICAgICAgaHR0cDovL29wZW5zb3VyY2Uub3JnL2xpY2Vuc2VzL01JVFxyXG4vL1xyXG4vLyAgVW5sZXNzIGFncmVlZCB0byBpbiB3cml0aW5nLCB0aGUgc3ViamVjdCBzb2Z0d2FyZSBkaXN0cmlidXRlZCB1bmRlciB0aGUgTGljZW5zZSBpcyBkaXN0cmlidXRlZCBvbiBhblxyXG4vLyAgXCJBUy1JU1wiIEJBU0lTLCBXSVRIT1VUIFdBUlJBTlRJRVMgT1IgQ09ORElUSU9OUyBPRiBBTlkgS0lORCwgZWl0aGVyIGV4cHJlc3Mgb3IgaW1wbGllZC4gUmVmZXIgdG8gdGhlXHJcbi8vICBMaWNlbnNlIGZvciB0aGUgc3BlY2lmaWMgbGFuZ3VhZ2UgZ292ZXJuaW5nIHBlcm1pc3Npb25zIGFuZCBsaW1pdGF0aW9ucy5cclxuLy9cclxuLy8gIENvZGUgTW9kaWZpY2F0aW9uIEhpc3Rvcnk6XHJcbi8vICAtLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tLS0tXHJcbi8vICAxMC8zMC8yMDE3IC0gQmlsbHkgRXJuZXN0XHJcbi8vICAgICAgIEdlbmVyYXRlZCBvcmlnaW5hbCB2ZXJzaW9uIG9mIHNvdXJjZSBjb2RlLlxyXG4vL1xyXG4vLyoqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKioqKlxyXG5cclxuZGVjbGFyZSB2YXIgXzogYW55O1xyXG5cclxuZXhwb3J0IGRlZmF1bHQgY2xhc3MgT3Blbkhpc3RvcmlhbkRhdGFTb3VyY2V7XHJcbiAgICB0eXBlOiBhbnk7XHJcbiAgICB1cmw6IHN0cmluZztcclxuICAgIG5hbWU6IHN0cmluZztcclxuICAgIHE6IGFueTtcclxuICAgIGRhdGFGbGFnczogYW55O1xyXG4gICAgb3B0aW9uczogYW55O1xyXG5cclxuICAgIC8qKiBAbmdJbmplY3QgKi9cclxuICAgIGNvbnN0cnVjdG9yKGluc3RhbmNlU2V0dGluZ3MsICRxLCBwcml2YXRlIGJhY2tlbmRTcnYsIHByaXZhdGUgdGVtcGxhdGVTcnYsIHByaXZhdGUgdWlTZWdtZW50U3J2KSB7XHJcbiAgICAgICAgdGhpcy50eXBlID0gaW5zdGFuY2VTZXR0aW5ncy50eXBlO1xyXG4gICAgICAgIHRoaXMudXJsID0gaW5zdGFuY2VTZXR0aW5ncy51cmw7XHJcbiAgICAgICAgdGhpcy5uYW1lID0gaW5zdGFuY2VTZXR0aW5ncy5uYW1lO1xyXG4gICAgICAgIHRoaXMucSA9ICRxO1xyXG4gICAgICAgIHRoaXMuYmFja2VuZFNydiA9IGJhY2tlbmRTcnY7XHJcbiAgICAgICAgdGhpcy50ZW1wbGF0ZVNydiA9IHRlbXBsYXRlU3J2O1xyXG4gICAgICAgIHRoaXMudWlTZWdtZW50U3J2ID0gdWlTZWdtZW50U3J2O1xyXG5cclxuICAgICAgICB0aGlzLmRhdGFGbGFncyA9IGluc3RhbmNlU2V0dGluZ3MuanNvbkRhdGEuZmxhZ3M7XHJcbiAgICAgICAgdGhpcy5vcHRpb25zID0ge1xyXG4gICAgICAgICAgICBleGNsdWRlZERhdGFGbGFnczogKGluc3RhbmNlU2V0dGluZ3MuanNvbkRhdGEuRXhjbHVkZWQgPT0gdW5kZWZpbmVkID8gMCA6IGluc3RhbmNlU2V0dGluZ3MuanNvbkRhdGEuRXhjbHVkZWQpLFxyXG4gICAgICAgICAgICBleGNsdWRlTm9ybWFsRGF0YTogKGluc3RhbmNlU2V0dGluZ3MuanNvbkRhdGEuTm9ybWFsID09IHVuZGVmaW5lZCA/IGZhbHNlIDogaW5zdGFuY2VTZXR0aW5ncy5qc29uRGF0YS5Ob3JtYWwpLFxyXG4gICAgICAgICAgICB1cGRhdGVBbGFybXM6IChpbnN0YW5jZVNldHRpbmdzLmpzb25EYXRhLkFsYXJtcyA9PSB1bmRlZmluZWQgPyBmYWxzZSA6IGluc3RhbmNlU2V0dGluZ3MuanNvbkRhdGEuQWxhcm1zKSxcclxuICAgICAgICB9XHJcblxyXG4gICAgfVxyXG5cclxuICAgIHF1ZXJ5KG9wdGlvbnMpIHtcclxuXHJcbiAgICAgICAgXHJcbiAgICAgICAgdmFyIHF1ZXJ5ID0gdGhpcy5idWlsZFF1ZXJ5UGFyYW1ldGVycyhvcHRpb25zKTtcclxuICAgICAgICAgICAgcXVlcnkudGFyZ2V0cyA9IHF1ZXJ5LnRhcmdldHMuZmlsdGVyKGZ1bmN0aW9uICh0KSB7XHJcbiAgICAgICAgICAgIHJldHVybiAhdC5oaWRlO1xyXG4gICAgICAgIH0pO1xyXG5cclxuICAgICAgICBxdWVyeS5vcHRpb25zID0gSlNPTi5wYXJzZShKU09OLnN0cmluZ2lmeSh0aGlzLm9wdGlvbnMpKTtcclxuXHJcbiAgICAgICAgaWYgKHF1ZXJ5LnRhcmdldHMubGVuZ3RoIDw9IDApIHtcclxuICAgICAgICAgICAgcmV0dXJuIFByb21pc2UucmVzb2x2ZSh7IGRhdGE6IFtdIH0pO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgbGV0IGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICBpZiAodGhpcy5vcHRpb25zLnVwZGF0ZUFsYXJtcykge1xyXG4gICAgICAgICAgICAvLyBHZXQgQWxlcnRzIGFuZCBEYXNoYm9hcmQgSW5mb3JtYXRpb25cclxuICAgICAgICAgICAgdGhpcy5iYWNrZW5kU3J2LmRhdGFzb3VyY2VSZXF1ZXN0KHtcclxuICAgICAgICAgICAgICAgIHVybDogdGhpcy51cmwgKyAnL0dldEFsYXJtcycsXHJcbiAgICAgICAgICAgICAgICBkYXRhOiBxdWVyeSxcclxuICAgICAgICAgICAgICAgIG1ldGhvZDogJ1BPU1QnLFxyXG4gICAgICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICAgICAgfSkudGhlbihmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgY3RybC5HZXREYXNoYm9hcmQoZGF0YS5kYXRhLCBxdWVyeSwgY3RybClcclxuICAgICAgICAgICAgfSkuY2F0Y2goZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfVxyXG4gICAgICAgIC8vMyBjYXNlczogSWYgQWxlcnRzIGFyZSBlbXB0eSBhbmQgQWxhcm1zIGFyZSBub3QgLT4gQ3JlYXRlIEFsZXJ0cyAtPiBTYXZlXHJcbiAgICAgICAgLy8gSWYgQWxhcm1zIGFyZSBlbXB0eSBhbmQgQWxlcnRzIGFyZSBub3QgLT4gUmVtb3ZlIEFsZXJ0cyAtPiBTYXZlXHJcbiAgICAgICAgLy8gSWYgQWxhcm1zIGFuZCBBbGVydHMgZXhpc3QgLT4gZW5zdXJlIGVhY2ggQWxhcm0gaGFzIGNvcnJlc3BvbmRpbmcgQ29uZGl0aW9uIGFuZCByZW1vdmUgYWxsIG90aGVycyAtPiBTYXZlXHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6IHRoaXMudXJsICsgJy9xdWVyeScsXHJcbiAgICAgICAgICAgIGRhdGE6IHF1ZXJ5LFxyXG4gICAgICAgICAgICBtZXRob2Q6ICdQT1NUJyxcclxuICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICB0ZXN0RGF0YXNvdXJjZSgpIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5iYWNrZW5kU3J2LmRhdGFzb3VyY2VSZXF1ZXN0KHtcclxuICAgICAgICAgICAgdXJsOiB0aGlzLnVybCArICcvJyxcclxuICAgICAgICAgICAgbWV0aG9kOiAnR0VUJ1xyXG4gICAgICAgIH0pLnRoZW4oZnVuY3Rpb24gKHJlc3BvbnNlKSB7XHJcbiAgICAgICAgICAgIGlmIChyZXNwb25zZS5zdGF0dXMgPT09IDIwMCkge1xyXG4gICAgICAgICAgICByZXR1cm4geyBzdGF0dXM6IFwic3VjY2Vzc1wiLCBtZXNzYWdlOiBcIkRhdGEgc291cmNlIGlzIHdvcmtpbmdcIiwgdGl0bGU6IFwiU3VjY2Vzc1wiIH07XHJcbiAgICAgICAgICAgIH1cclxuICAgICAgICB9KTtcclxuICAgIH1cclxuXHJcbiAgICBhbm5vdGF0aW9uUXVlcnkob3B0aW9ucykge1xyXG4gICAgICAgIHZhciBxdWVyeSA9IHRoaXMudGVtcGxhdGVTcnYucmVwbGFjZShvcHRpb25zLmFubm90YXRpb24ucXVlcnksIHt9LCAnZ2xvYicpO1xyXG4gICAgICAgIHZhciBhbm5vdGF0aW9uUXVlcnkgPSB7XHJcbiAgICAgICAgICAgIHJhbmdlOiBvcHRpb25zLnJhbmdlLFxyXG4gICAgICAgICAgICBhbm5vdGF0aW9uOiB7XHJcbiAgICAgICAgICAgIG5hbWU6IG9wdGlvbnMuYW5ub3RhdGlvbi5uYW1lLFxyXG4gICAgICAgICAgICBkYXRhc291cmNlOiBvcHRpb25zLmFubm90YXRpb24uZGF0YXNvdXJjZSxcclxuICAgICAgICAgICAgZW5hYmxlOiBvcHRpb25zLmFubm90YXRpb24uZW5hYmxlLFxyXG4gICAgICAgICAgICBpY29uQ29sb3I6IG9wdGlvbnMuYW5ub3RhdGlvbi5pY29uQ29sb3IsXHJcbiAgICAgICAgICAgIHF1ZXJ5OiBxdWVyeVxyXG4gICAgICAgICAgICB9LFxyXG4gICAgICAgICAgICByYW5nZVJhdzogb3B0aW9ucy5yYW5nZVJhd1xyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6IHRoaXMudXJsICsgJy9hbm5vdGF0aW9ucycsXHJcbiAgICAgICAgICAgIG1ldGhvZDogJ1BPU1QnLFxyXG4gICAgICAgICAgICBkYXRhOiBhbm5vdGF0aW9uUXVlcnlcclxuICAgICAgICB9KS50aGVuKGZ1bmN0aW9uIChyZXN1bHQpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHJlc3VsdC5kYXRhO1xyXG4gICAgICAgIH0pO1xyXG4gICAgfVxyXG5cclxuICAgIG1ldHJpY0ZpbmRRdWVyeShvcHRpb25zOiBzdHJpbmcsIG9wdGlvbmFsT3B0aW9uczogYW55KSB7XHJcbiAgICAgICAgdmFyIGludGVycG9sYXRlZCA9IHtcclxuICAgICAgICAgICAgdGFyZ2V0OiB0aGlzLnRlbXBsYXRlU3J2LnJlcGxhY2Uob3B0aW9ucywgbnVsbCwgJ3JlZ2V4JylcclxuICAgICAgICB9O1xyXG5cclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuYmFja2VuZFNydi5kYXRhc291cmNlUmVxdWVzdCh7XHJcbiAgICAgICAgICAgIHVybDogdGhpcy51cmwgKyAnL3NlYXJjaCcsXHJcbiAgICAgICAgICAgIGRhdGE6IGludGVycG9sYXRlZCxcclxuICAgICAgICAgICAgbWV0aG9kOiAnUE9TVCcsXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSkudGhlbih0aGlzLm1hcFRvVGV4dFZhbHVlKTtcclxuICAgIH1cclxuXHJcbiAgICB3aGVyZUZpbmRRdWVyeShvcHRpb25zKSB7XHJcblxyXG4gICAgICAgIHZhciBpbnRlcnBvbGF0ZWQgPSB7XHJcbiAgICAgICAgICAgIHRhcmdldDogdGhpcy50ZW1wbGF0ZVNydi5yZXBsYWNlKG9wdGlvbnMsIG51bGwsICdyZWdleCcpXHJcbiAgICAgICAgfTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuYmFja2VuZFNydi5kYXRhc291cmNlUmVxdWVzdCh7XHJcbiAgICAgICAgICAgIHVybDogdGhpcy51cmwgKyAnL1NlYXJjaEZpZWxkcycsXHJcbiAgICAgICAgICAgIGRhdGE6IGludGVycG9sYXRlZCxcclxuICAgICAgICAgICAgbWV0aG9kOiAnUE9TVCcsXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSkudGhlbih0aGlzLm1hcFRvVGV4dFZhbHVlKTtcclxuICAgIH1cclxuXHJcbiAgICBtYXBUb1RleHRWYWx1ZShyZXN1bHQpIHtcclxuICAgICAgICByZXR1cm4gXy5tYXAocmVzdWx0LmRhdGEsIGZ1bmN0aW9uIChkLCBpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB7IHRleHQ6IGQsIHZhbHVlOiBkIH07XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgYnVpbGRRdWVyeVBhcmFtZXRlcnMob3B0aW9ucykge1xyXG4gICAgICAgIHZhciBfdGhpcyA9IHRoaXM7XHJcblxyXG4gICAgICAgIC8vcmVtb3ZlIHBsYWNlaG9sZGVyIHRhcmdldHNcclxuICAgICAgICBvcHRpb25zLnRhcmdldHMgPSBfLmZpbHRlcihvcHRpb25zLnRhcmdldHMsIGZ1bmN0aW9uICh0YXJnZXQpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHRhcmdldC50YXJnZXQgIT09ICdzZWxlY3QgbWV0cmljJztcclxuICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgdmFyIHRhcmdldHMgPSBfLm1hcChvcHRpb25zLnRhcmdldHMsIGZ1bmN0aW9uICh0YXJnZXQpIHtcclxuICAgICAgICAgICAgcmV0dXJuIHtcclxuICAgICAgICAgICAgdGFyZ2V0OiBfdGhpcy5maXhUZW1wbGF0ZXModGFyZ2V0KSxcclxuICAgICAgICAgICAgcmVmSWQ6IHRhcmdldC5yZWZJZCxcclxuICAgICAgICAgICAgaGlkZTogdGFyZ2V0LmhpZGUsIFxyXG4gICAgICAgICAgICBleGNsdWRlZEZsYWdzOiAoKHRhcmdldHx8e30pLnF1ZXJ5T3B0aW9uc3x8e30pLkV4Y2x1ZGVkIHx8IDAsXHJcbiAgICAgICAgICAgIGV4Y2x1ZGVOb3JtYWxGbGFnczogKCh0YXJnZXR8fHt9KS5xdWVyeU9wdGlvbnN8fHt9KS5Ob3JtYWwgfHwgZmFsc2UsXHJcbiAgICAgICAgICAgIHF1ZXJ5VHlwZTogdGFyZ2V0LnF1ZXJ5VHlwZSxcclxuICAgICAgICAgICAgcXVlcnlPcHRpb25zOiB0YXJnZXQucXVlcnlPcHRpb25zXHJcbiAgICAgICAgICAgIH07XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgICAgIG9wdGlvbnMudGFyZ2V0cyA9IHRhcmdldHM7XHJcblxyXG4gICAgICAgIHJldHVybiBvcHRpb25zO1xyXG4gICAgfVxyXG5cclxuICAgIGZpbHRlckZpbmRRdWVyeSgpIHtcclxuICAgICAgICByZXR1cm4gdGhpcy5iYWNrZW5kU3J2LmRhdGFzb3VyY2VSZXF1ZXN0KHtcclxuICAgICAgICAgICAgdXJsOiB0aGlzLnVybCArICcvU2VhcmNoRmlsdGVycycsXHJcbiAgICAgICAgICAgIG1ldGhvZDogJ1BPU1QnLFxyXG4gICAgICAgICAgICBoZWFkZXJzOiB7ICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbicgfVxyXG4gICAgICAgIH0pLnRoZW4odGhpcy5tYXBUb1RleHRWYWx1ZSk7XHJcbiAgICB9XHJcblxyXG5cclxuICAgIG9yZGVyQnlGaW5kUXVlcnkob3B0aW9ucykge1xyXG4gICAgICAgIHZhciBpbnRlcnBvbGF0ZWQgPSB7XHJcbiAgICAgICAgICAgIHRhcmdldDogdGhpcy50ZW1wbGF0ZVNydi5yZXBsYWNlKG9wdGlvbnMsIG51bGwsICdyZWdleCcpXHJcbiAgICAgICAgfTtcclxuXHJcbiAgICAgICAgcmV0dXJuIHRoaXMuYmFja2VuZFNydi5kYXRhc291cmNlUmVxdWVzdCh7XHJcbiAgICAgICAgICAgIHVybDogdGhpcy51cmwgKyAnL1NlYXJjaE9yZGVyQnlzJyxcclxuICAgICAgICAgICAgZGF0YTogaW50ZXJwb2xhdGVkLFxyXG4gICAgICAgICAgICBtZXRob2Q6ICdQT1NUJyxcclxuICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICB9KS50aGVuKHRoaXMubWFwVG9UZXh0VmFsdWUpO1xyXG4gICAgfVxyXG5cclxuICAgIGdldE1ldGFEYXRhKG9wdGlvbnMpIHtcclxuICAgICAgICB2YXIgaW50ZXJwb2xhdGVkID0ge1xyXG4gICAgICAgICAgICB0YXJnZXQ6IHRoaXMudGVtcGxhdGVTcnYucmVwbGFjZShvcHRpb25zLCBudWxsLCAncmVnZXgnKVxyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6IHRoaXMudXJsICsgJy9HZXRNZXRhZGF0YScsXHJcbiAgICAgICAgICAgIGRhdGE6IGludGVycG9sYXRlZCxcclxuICAgICAgICAgICAgbWV0aG9kOiAnUE9TVCcsXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGdldEFsYXJtU3RhdGVzKG9wdGlvbnMpIHtcclxuICAgICAgICB2YXIgaW50ZXJwb2xhdGVkID0ge1xyXG4gICAgICAgICAgICB0YXJnZXQ6IHRoaXMudGVtcGxhdGVTcnYucmVwbGFjZShvcHRpb25zLCBudWxsLCAncmVnZXgnKVxyXG4gICAgICAgIH07XHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6IHRoaXMudXJsICsgJy9HZXRBbGFybVN0YXRlJyxcclxuICAgICAgICAgICAgZGF0YTogaW50ZXJwb2xhdGVkLFxyXG4gICAgICAgICAgICBtZXRob2Q6ICdQT1NUJyxcclxuICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICB9KTtcclxuXHJcbiAgICB9XHJcblxyXG4gICAgZ2V0RGF0YUF2YWlsYWJpbGl0eShvcHRpb25zKSB7XHJcbiAgICAgICAgdmFyIGludGVycG9sYXRlZCA9IHtcclxuICAgICAgICAgICAgdGFyZ2V0OiB0aGlzLnRlbXBsYXRlU3J2LnJlcGxhY2Uob3B0aW9ucywgbnVsbCwgJ3JlZ2V4JylcclxuICAgICAgICB9O1xyXG5cclxuICAgICAgICByZXR1cm4gdGhpcy5iYWNrZW5kU3J2LmRhdGFzb3VyY2VSZXF1ZXN0KHtcclxuICAgICAgICAgICAgdXJsOiB0aGlzLnVybCArICcvR2V0RGF0YUF2YWlsYWJpbGl0eScsXHJcbiAgICAgICAgICAgIGRhdGE6IGludGVycG9sYXRlZCxcclxuICAgICAgICAgICAgbWV0aG9kOiAnUE9TVCcsXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSk7XHJcblxyXG4gICAgfVxyXG5cclxuICAgIGZpeFRlbXBsYXRlcyh0YXJnZXQpIHtcclxuICAgICAgICBpZiAodGFyZ2V0LnRhcmdldCA9PSB1bmRlZmluZWQpIHJldHVybiAnJztcclxuXHJcbiAgICAgICAgdmFyIGN0cmwgPSB0aGlzO1xyXG5cclxuICAgICAgICB2YXIgc2VwID0gJyAnO1xyXG4gICAgICAgIGlmKHRhcmdldC5xdWVyeVR5cGUgPT0gJ0VsZW1lbnQgTGlzdCcpXHJcbiAgICAgICAgICAgIHNlcCA9ICc7J1xyXG5cclxuICAgICAgICByZXR1cm4gdGFyZ2V0LnRhcmdldC5zcGxpdChzZXApLm1hcChmdW5jdGlvbiAoYSkge1xyXG4gICAgICAgICAgICBpZiAoY3RybC50ZW1wbGF0ZVNydi52YXJpYWJsZUV4aXN0cyhhKSkge1xyXG4gICAgICAgICAgICAgICAgcmV0dXJuIGN0cmwudGVtcGxhdGVTcnYucmVwbGFjZVdpdGhUZXh0KGEpO1xyXG4gICAgICAgICAgICB9XHJcbiAgICAgICAgICAgIGVsc2VcclxuICAgICAgICAgICAgICAgIHJldHVybiBhXHJcbiAgICAgICAgfSkuam9pbihzZXApO1xyXG4gICAgfVxyXG5cclxuICAgIHF1ZXJ5TG9jYXRpb24odGFyZ2V0KSB7XHJcblxyXG4gICAgICAgIGlmICgodGFyZ2V0LnRhcmdldCA9PSBudWxsKSB8fCAodGFyZ2V0LnRhcmdldCA9PSB1bmRlZmluZWQpKVxyXG4gICAgICAgIHtcclxuICAgICAgICAgICAgdGFyZ2V0LnRhcmdldCA9IHt9O1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgaWYgKCh0YXJnZXQucmFkaXVzID09IG51bGwpIHx8ICh0YXJnZXQucmFkaXVzID09IHVuZGVmaW5lZCkgfHwgKHRhcmdldC56b29tID09IG51bGwpIHx8ICh0YXJnZXQuem9vbSA9PSB1bmRlZmluZWQpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICAgICAgbWV0aG9kOiBcIlBPU1RcIixcclxuICAgICAgICAgICAgICAgIHVybDogdGhpcy51cmwgKyAnL0dldExvY2F0aW9uRGF0YScsXHJcbiAgICAgICAgICAgICAgICBkYXRhOiBKU09OLnN0cmluZ2lmeSh0YXJnZXQudGFyZ2V0KSxcclxuICAgICAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICB9XHJcblxyXG4gICAgICAgIHJldHVybiB0aGlzLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICBtZXRob2Q6IFwiUE9TVFwiLFxyXG4gICAgICAgICAgICB1cmw6IHRoaXMudXJsICsgJy9HZXRMb2NhdGlvbkRhdGE/cmFkaXVzPScgKyB0YXJnZXQucmFkaXVzICsgJyZ6b29tPScgKyB0YXJnZXQuem9vbSxcclxuICAgICAgICAgICAgZGF0YTogSlNPTi5zdHJpbmdpZnkodGFyZ2V0LnRhcmdldCksXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSk7XHJcblxyXG5cclxuICAgIH1cclxuXHJcbiAgICAvLyBUaGVzZSBGdWN0aW9ucyB1cGRhdGUgdGhlIGRhc2hib2FyZCBpZiBhbiBPSCBBbGFybSBpcyBuZWNjZXNhcnkuXHJcbiAgICBHZXREYXNoYm9hcmQoYWxhcm1zLCBxdWVyeSwgY3RybCkge1xyXG5cclxuICAgICAgICBjdHJsLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6IGN0cmwudXJsICsgJy9RdWVyeUFsYXJtcycsXHJcbiAgICAgICAgICAgIG1ldGhvZDogJ1BPU1QnLFxyXG4gICAgICAgICAgICBkYXRhOiBxdWVyeSxcclxuICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICB9KS50aGVuKGZ1bmN0aW9uIChkYXRhKSB7XHJcbiAgICAgICAgICAgIC8vY29uc29sZS5sb2coZGF0YSk7XHJcbiAgICAgICAgfSkuY2F0Y2goZnVuY3Rpb24gKGRhdGEpIHtcclxuICAgICAgICAgICAgLy9jb25zb2xlLmxvZyhkYXRhKTtcclxuICAgICAgICB9KTtcclxuXHJcblxyXG5cclxuICAgICAgICBjdHJsLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICB1cmw6ICdhcGkvc2VhcmNoP2Rhc2hib2FyZElkcz0nICsgcXVlcnkuZGFzaGJvYXJkSWQsXHJcbiAgICAgICAgICAgIG1ldGhvZDogJ0dFVCcsXHJcbiAgICAgICAgICAgIGhlYWRlcnM6IHsgJ0NvbnRlbnQtVHlwZSc6ICdhcHBsaWNhdGlvbi9qc29uJyB9XHJcbiAgICAgICAgfSkudGhlbihmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICBjdHJsLmJhY2tlbmRTcnYuZGF0YXNvdXJjZVJlcXVlc3Qoe1xyXG4gICAgICAgICAgICAgICAgdXJsOiAnYXBpL2Rhc2hib2FyZHMvdWlkLycgKyBkYXRhLmRhdGFbMF1bXCJ1aWRcIl0sXHJcbiAgICAgICAgICAgICAgICBtZXRob2Q6ICdHRVQnLFxyXG4gICAgICAgICAgICAgICAgaGVhZGVyczogeyAnQ29udGVudC1UeXBlJzogJ2FwcGxpY2F0aW9uL2pzb24nIH1cclxuICAgICAgICAgICAgfSkudGhlbihmdW5jdGlvbiAoZGF0YSkge1xyXG4gICAgICAgICAgICAgICAgY3RybC5DaGVja1BhbmVsKGFsYXJtcywgcXVlcnksIGRhdGEuZGF0YSwgY3RybClcclxuICAgICAgICAgICAgfSk7XHJcbiAgICAgICAgfSk7XHJcbiAgICB9XHJcblxyXG4gICAgQ2hlY2tQYW5lbChhbGFybXMsIHF1ZXJ5LCBkYXNoYm9hcmQsIGN0cmwpXHJcbiAgICB7XHJcbiAgICAgICAgLy9HZXQgQWxhZXJ0cyBmcm9tIFBhbmVsXHJcbiAgICAgICAgbGV0IGFsZXJ0cyA9IGRhc2hib2FyZC5kYXNoYm9hcmQucGFuZWxzO1xyXG4gICAgICAgIGlmIChhbGVydHMgPT09IHVuZGVmaW5lZClcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG5cclxuICAgICAgICBhbGVydHMgPSBhbGVydHMuZmluZChpdGVtID0+IGl0ZW0uaWQgPT0gcXVlcnkucGFuZWxJZCk7XHJcbiAgICAgICAgaWYgKGFsZXJ0cyA9PSB1bmRlZmluZWQgfHwgYWxlcnRzID09IG51bGwpXHJcbiAgICAgICAgICAgIHJldHVybjtcclxuXHJcbiAgICAgICAgYWxlcnRzID0gYWxlcnRzLnRocmVzaG9sZHM7XHJcblxyXG4gICAgICAgIC8vIENoZWNrIENhc2UgTm8gYWxlcnRzIGFuZCBObyBBbGFybXMgaW4gdGhlIE9IXHJcbiAgICAgICAgaWYgKChhbGVydHMgPT0gdW5kZWZpbmVkIHx8IGFsZXJ0cyA9PSBudWxsIHx8IGFsZXJ0cy5sZW5ndGggPT0gMCkgJiYgKGFsYXJtcy5sZW5ndGggPT0gMCkpIFxyXG4gICAgICAgICAgICByZXR1cm5cclxuXHJcbiAgICAgICAgLy8gQ2hlY2sgY2FzZSBubyBBbGVydHMgYnV0IHdlIGhhdmUgQWxhcm1zXHJcbiAgICAgICAgaWYgKChhbGVydHMgPT0gdW5kZWZpbmVkIHx8IGFsZXJ0cyA9PSBudWxsIHx8IGFsZXJ0cy5sZW5ndGggPT0gMCkgJiYgKGFsYXJtcy5sZW5ndGggPiAwKSkge1xyXG4gICAgICAgICAgICBjdHJsLlVwZGF0ZUFsYXJtcyhhbGFybXMsIGRhc2hib2FyZC5kYXNoYm9hcmQudWlkLCBxdWVyeSwgY3RybClcclxuICAgICAgICAgICAgcmV0dXJuO1xyXG4gICAgICAgIH1cclxuXHJcbiAgICAgICAgLy8gbWFrZSBzdXJlIHRoaXMgaXMgbm90IGEgR1BBIFBoYXNvck1hcCBQYW5lbFxyXG4gICAgICAgIGxldCB0aHJlc2hob2xkcyA9IFtdXHJcblxyXG4gICAgICAgIHRyeSB7XHJcbiAgICAgICAgICAgIC8vIExhc3QgQ2hlY2sgaWYgZXZlcnkgYWxhcm0gaGFzIGNvcnJlc3BvbmRpbmcgdGhyZXNoaG9sZFxyXG4gICAgICAgICAgICB0aHJlc2hob2xkcyA9IGFsZXJ0cy5tYXAoaXRlbSA9PiBpdGVtLnZhbHVlKTtcclxuICAgICAgICB9XHJcbiAgICAgICAgY2F0Y2gge1xyXG4gICAgICAgICAgICByZXR1cm47XHJcbiAgICAgICAgfVxyXG5cclxuICAgICAgICBsZXQgbmVlZHNVcGRhdGUgPSBmYWxzZTtcclxuXHJcbiAgICAgICAgYWxhcm1zLmZvckVhY2goaXRlbSA9PiB7XHJcbiAgICAgICAgICAgIGlmICghdGhyZXNoaG9sZHMuaW5jbHVkZXMoaXRlbS5TZXRQb2ludCkpXHJcbiAgICAgICAgICAgICAgICBuZWVkc1VwZGF0ZSA9IHRydWU7XHJcbiAgICAgICAgICAgIH0pO1xyXG5cclxuICAgICAgICBpZiAobmVlZHNVcGRhdGUpIHtcclxuICAgICAgICAgICAgY3RybC5VcGRhdGVBbGFybXMoYWxhcm1zLCBkYXNoYm9hcmQuZGFzaGJvYXJkLnVpZCwgcXVlcnksIGN0cmwpXHJcbiAgICAgICAgfVxyXG5cclxuICAgIH1cclxuXHJcbiAgICBVcGRhdGVBbGFybXMoYWxhcm1zLCBkYXNoYm9hcmRVaWQsIHF1ZXJ5LCBjdHJsKSB7XHJcblxyXG4gICAgICAgIFxyXG4gICAgICAgIGN0cmwuYmFja2VuZFNydi5kYXRhc291cmNlUmVxdWVzdCh7XHJcbiAgICAgICAgICAgIHVybDogJ2FwaS9kYXNoYm9hcmRzL3VpZC8nICsgZGFzaGJvYXJkVWlkLFxyXG4gICAgICAgICAgICBtZXRob2Q6ICdHRVQnLFxyXG4gICAgICAgICAgICBoZWFkZXJzOiB7ICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbicgfVxyXG4gICAgICAgIH0pLnRoZW4oZnVuY3Rpb24gKGRhdGEpIHtcclxuXHJcbiAgICAgICAgICAgIGxldCBkYXNoYm9hcmQgPSBkYXRhLmRhdGEuZGFzaGJvYXJkO1xyXG4gICAgICAgICAgICBsZXQgcGFuZWxJbmRleCA9IGRhc2hib2FyZC5wYW5lbHMuZmluZEluZGV4KGl0ZW0gPT4gaXRlbS5pZCA9PSBxdWVyeS5wYW5lbElkKTtcclxuICAgICAgICAgICAgZGFzaGJvYXJkLnBhbmVsc1twYW5lbEluZGV4XS50aHJlc2hvbGRzID0gYWxhcm1zLm1hcChpdGVtID0+IHtcclxuICAgICAgICAgICAgICAgIGxldCBvcCA9IFwiZ3RcIjtcclxuICAgICAgICAgICAgICAgIGlmIChpdGVtLk9wZXJhdGlvbiA9PSAyMSB8fCBpdGVtLk9wZXJhdGlvbiA9PSAyMilcclxuICAgICAgICAgICAgICAgICAgICBvcCA9IFwibHRcIlxyXG5cclxuICAgICAgICAgICAgICAgIGxldCBmaWxsID0gdHJ1ZTtcclxuICAgICAgICAgICAgICAgIGlmIChpdGVtLk9wZXJhdGlvbiA9PSAxIHx8IGl0ZW0uT3BlcmF0aW9uID09IDIpXHJcbiAgICAgICAgICAgICAgICAgICAgZmlsbCA9IGZhbHNlO1xyXG5cclxuICAgICAgICAgICAgICAgIHJldHVybiB7XHJcbiAgICAgICAgICAgICAgICAgICAgY29sb3JNb2RlOiBcImNyaXRpY2FsXCIsXHJcbiAgICAgICAgICAgICAgICAgICAgZmlsbDogZmlsbCxcclxuICAgICAgICAgICAgICAgICAgICBsaW5lOiB0cnVlLFxyXG4gICAgICAgICAgICAgICAgICAgIG9wOiBvcCxcclxuICAgICAgICAgICAgICAgICAgICB2YWx1ZTogaXRlbS5TZXRQb2ludFxyXG4gICAgICAgICAgICAgICAgfVxyXG4gICAgICAgICAgICB9KTtcclxuXHJcbiAgICAgICAgICAgIGN0cmwuYmFja2VuZFNydi5kYXRhc291cmNlUmVxdWVzdCh7XHJcbiAgICAgICAgICAgICAgICB1cmw6ICdhcGkvZGFzaGJvYXJkcy9kYicsXHJcbiAgICAgICAgICAgICAgICBtZXRob2Q6ICdQT1NUJyxcclxuICAgICAgICAgICAgICAgIGRhdGE6IHsgb3ZlcndyaXRlOiB0cnVlLCBkYXNoYm9hcmQ6IGRhc2hib2FyZH0sXHJcbiAgICAgICAgICAgICAgICBoZWFkZXJzOiB7ICdDb250ZW50LVR5cGUnOiAnYXBwbGljYXRpb24vanNvbicgfVxyXG4gICAgICAgICAgICB9KS5jYXRjaChmdW5jdGlvbiAoZGF0YSkgeyBjb25zb2xlLmxvZyhkYXRhKTsgfSlcclxuICAgICAgICB9KTtcclxuXHJcbiAgICB9XHJcblxyXG59XHJcbiJdLCJzb3VyY2VSb290IjoiIn0=