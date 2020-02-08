define(["app/plugins/sdk","moment"], function(__WEBPACK_EXTERNAL_MODULE__2__, __WEBPACK_EXTERNAL_MODULE__3__) { return /******/ (function(modules) { // webpackBootstrap
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
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//******************************************************************************************************
//  module.ts - Gbtc
//
//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  12/15/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************

Object.defineProperty(exports, "__esModule", { value: true });
var OHDownload_ctrl_1 = __webpack_require__(1);
exports.PanelCtrl = OHDownload_ctrl_1.OHDownloadCtrl;

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//******************************************************************************************************
//  OHDowload_ctrl.ts - Gbtc
//
//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  02/03/2020 - C.Lackner
//       Generated original version of source code.
//
//******************************************************************************************************

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

Object.defineProperty(exports, "__esModule", { value: true });
//<reference path="../node_modules/grafana-sdk-mocks/app/headers/common.d.ts" />
var sdk_1 = __webpack_require__(2);
var moment = __webpack_require__(3);
//import { varName } from '../js/constants'   // import constants from constant file using this format

var OHDownloadCtrl = function (_sdk_1$MetricsPanelCt) {
    _inherits(OHDownloadCtrl, _sdk_1$MetricsPanelCt);

    function OHDownloadCtrl($scope, $injector, $rootScope) {
        _classCallCheck(this, OHDownloadCtrl);

        var _this = _possibleConstructorReturn(this, (OHDownloadCtrl.__proto__ || Object.getPrototypeOf(OHDownloadCtrl)).call(this, $scope, $injector));

        _this.$rootScope = $rootScope;
        _this.textSizeOptions = ['50%', '60%', '70%', '80%', '90%', '100%', '110%', '120%', '130%', '140%', '150%', '160%', '170%', '180%', '190%', '200%', '250%', '300%'];
        _this.events.on('init-edit-mode', _this.onInitEditMode.bind(_this));
        _this.events.on('panel-teardown', _this.onPanelTeardown.bind(_this));
        _this.events.on('render', _this.onRender.bind(_this));
        _this.events.on('panel-initialized', _this.onPanelInitialized.bind(_this));
        _this.events.on('data-received', _this.onDataRecieved.bind(_this));
        //this.events.on('data-snapshot-load', console.log('data-snapshot-load'));
        _this.events.on('data-error', _this.onDataError.bind(_this));
        _this.events.on('refresh', _this.onRefresh.bind(_this));
        _this.panel.link = _this.panel.link != undefined ? _this.panel.link : '..';
        _this.panel.rate = _this.panel.rate != undefined ? _this.panel.rate : 30;
        _this.panel.rateBase = _this.panel.rateBase != undefined ? _this.panel.rateBase : 'frames per second';
        _this.panel.firstTS = _this.panel.firstTS != undefined ? _this.panel.firstTS : 'first available measurement';
        _this.panel.alignTS = _this.panel.alignTS != undefined ? _this.panel.alignTS : true;
        _this.panel.exportNaN = _this.panel.exportNaN != undefined ? _this.panel.exportNaN : true;
        _this.panel.missingTS = _this.panel.missingTS != undefined ? _this.panel.missingTS : true;
        _this.panel.round = _this.panel.round != undefined ? _this.panel.round : true;
        _this.panel.color = _this.panel.color != undefined ? _this.panel.color : '#ff0000';
        _this.panel.textcolor = _this.panel.textcolor != undefined ? _this.panel.textcolor : '#ffffff';
        _this.panel.fontsize = _this.panel.fontsize != undefined ? _this.panel.fontsize : '100%';
        _this.fileLink = "";
        _this.startTime = null;
        _this.endTime = null;
        return _this;
    }
    // #region Events from Graphana Handlers


    _createClass(OHDownloadCtrl, [{
        key: "onInitEditMode",
        value: function onInitEditMode() {
            this.addEditorTab('Options', 'public/plugins/openhistorian-datadownload-panel/partials/editor.html', 2);
        }
    }, {
        key: "onPanelTeardown",
        value: function onPanelTeardown() {
            //console.log('panel-teardown');
        }
    }, {
        key: "onPanelInitialized",
        value: function onPanelInitialized() {
            //console.log('panel-initialized');
        }
    }, {
        key: "onRefresh",
        value: function onRefresh() {
            //console.log('refresh');
        }
    }, {
        key: "onResize",
        value: function onResize() {
            //console.log('refresh');
        }
    }, {
        key: "onRender",
        value: function onRender() {
            //console.log('render');
        }
    }, {
        key: "onDataRecieved",
        value: function onDataRecieved(data) {
            var _this2 = this;

            var ctrl = this;
            var promiseData = [];
            ctrl.pointIDs = [];
            var minTS = Number.MAX_SAFE_INTEGER;
            var maxTS = Number.MIN_SAFE_INTEGER;
            data.forEach(function (point) {
                promiseData.push(_this2.datasource.getMetaData(point.target).then(function (metaData) {
                    var jsonMetaData = JSON.parse(metaData.data);
                    var id = jsonMetaData[0]["ID"].split(":");
                    ctrl.pointIDs.push(id[1].trim());
                }));
                if (point.datapoints.length > 0) {
                    if (Math.min.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {
                        return item[1];
                    }))) < minTS) minTS = Math.min.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {
                        return item[1];
                    })));
                    if (Math.max.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {
                        return item[1];
                    }))) > maxTS) maxTS = Math.max.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {
                        return item[1];
                    })));
                }
            });
            this.startTime = moment.utc(minTS);
            this.endTime = moment.utc(maxTS);
            Promise.all(promiseData).then(function () {
                ctrl.updateLink();
            });
        }
    }, {
        key: "onDataError",
        value: function onDataError(msg) {
            //console.log('data-error');
        }
    }, {
        key: "handleClick",
        value: function handleClick(d) {
            window.location.href = this.fileLink;
        }
    }, {
        key: "updateLink",
        value: function updateLink() {
            var framerate = this.panel.rate;
            if (this.panel.rateBase == 'frames per minute') {
                framerate = framerate / 60.0;
            }
            if (this.panel.rateBase == 'frames per hour') {
                framerate = framerate / (60.0 * 60.0);
            }
            var tolerance = 0.5;
            if (this.panel.round) {
                tolerance = 500 / framerate;
            }
            this.fileLink = this.panel.link + '\ExportDataHandler.ashx?PointIDs=' + this.pointIDs.join(",") + "&StartTime=" + this.startTime.format("MM/DD/YYYY HH:mm:ss.000000") + "&EndTime=" + this.endTime.format("MM/DD/YYYY HH:mm:ss.000000") + "&FrameRate=" + framerate.toString() + "&AlignTimestamps=" + (this.panel.alignTS ? "true" : "false") + "&MissingAsNaNParam=" + (this.panel.exportNaN ? "true" : "false") + "&FillMissingTimestamps=" + (this.panel.missingTS ? "true" : "false") + "&InstanceName=" + "PPA" + "&TSSnap=" + this.formatTSSnap().toString() + "&TSTolerance=" + tolerance.toString();
        }
    }, {
        key: "formatTSSnap",
        value: function formatTSSnap() {
            if (this.panel.firstTS == 'first available measurement') return 1;
            return 2;
        }
    }]);

    return OHDownloadCtrl;
}(sdk_1.MetricsPanelCtrl);

OHDownloadCtrl.templateUrl = 'partials/module.html';
exports.OHDownloadCtrl = OHDownloadCtrl;

/***/ }),
/* 2 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__2__;

/***/ }),
/* 3 */
/***/ (function(module, exports) {

module.exports = __WEBPACK_EXTERNAL_MODULE__3__;

/***/ })
/******/ ])});;