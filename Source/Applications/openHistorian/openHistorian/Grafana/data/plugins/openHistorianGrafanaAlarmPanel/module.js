define(["app/plugins/sdk","lodash"], function(__WEBPACK_EXTERNAL_MODULE__2__, __WEBPACK_EXTERNAL_MODULE__3__) { return /******/ (function(modules) { // webpackBootstrap
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
var openHistorianGrafanaAlarmPanel_ctrl_1 = __webpack_require__(1);
exports.PanelCtrl = openHistorianGrafanaAlarmPanel_ctrl_1.OpenHistorianGrafanaAlarmPanel;

/***/ }),
/* 1 */
/***/ (function(module, exports, __webpack_require__) {

"use strict";

//******************************************************************************************************
//  openHistorianGrafanaAlarmPanel.ts - Gbtc
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

var _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError("this hasn't been initialised - super() hasn't been called"); } return call && (typeof call === "object" || typeof call === "function") ? call : self; }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

Object.defineProperty(exports, "__esModule", { value: true });
//<reference path="../node_modules/grafana-sdk-mocks/app/headers/common.d.ts" />
var sdk_1 = __webpack_require__(2);
var _ = __webpack_require__(3);
//import { varName } from '../js/constants'   // import constants from constant file using this format

var OpenHistorianGrafanaAlarmPanel = function (_sdk_1$MetricsPanelCt) {
    _inherits(OpenHistorianGrafanaAlarmPanel, _sdk_1$MetricsPanelCt);

    function OpenHistorianGrafanaAlarmPanel($scope, $injector, $rootScope) {
        _classCallCheck(this, OpenHistorianGrafanaAlarmPanel);

        var _this = _possibleConstructorReturn(this, (OpenHistorianGrafanaAlarmPanel.__proto__ || Object.getPrototypeOf(OpenHistorianGrafanaAlarmPanel)).call(this, $scope, $injector));

        _this.$rootScope = $rootScope;
        _this.events.on('init-edit-mode', _this.onInitEditMode.bind(_this));
        _this.events.on('panel-teardown', _this.onPanelTeardown.bind(_this));
        _this.events.on('render', _this.onRender.bind(_this));
        _this.events.on('panel-initialized', _this.onPanelInitialized.bind(_this));
        _this.events.on('data-received', _this.onDataRecieved.bind(_this));
        //this.events.on('data-snapshot-load', console.log('data-snapshot-load'));
        _this.events.on('data-error', _this.onDataError.bind(_this));
        _this.events.on('refresh', _this.onRefresh.bind(_this));
        _this.panel.link = _this.panel.link != undefined ? _this.panel.link : '..';
        _this.panel.filter = _this.panel.filter != undefined ? _this.panel.filter : '';
        _this.panel.showLegend = _this.panel.showLegend != undefined ? _this.panel.showLegend : true;
        return _this;
    }
    // #region Events from Graphana Handlers


    _createClass(OpenHistorianGrafanaAlarmPanel, [{
        key: "onInitEditMode",
        value: function onInitEditMode() {
            this.addEditorTab('Options', 'public/plugins/openhistorian-alarm-panel/partials/editor.html', 2);
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
            var ctrl = this;
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

            this.datasource.getAlarmStates().then(function (data) {
                //console.log(data);
                var filterdata = data.data;
                if (_this2.panel.filter !== "") {
                    var filtereddata = [];
                    var re = new RegExp(_this2.panel.filter);
                    filterdata.forEach(function (item) {
                        if (re.test(item.Name)) {
                            filtereddata.push(item);
                        }
                    });
                    filterdata = filtereddata;
                }
                _this2.$scope.data = filterdata;
                _this2.$scope.colors = _.uniqBy(filterdata, 'State');
            });
            //console.log('data-recieved');
        }
    }, {
        key: "onDataError",
        value: function onDataError(msg) {
            //console.log('data-error');
        }
    }, {
        key: "handleClick",
        value: function handleClick(d) {
            window.open(this.panel.link + '/DeviceStatus.cshtml?ID=' + d.ID);
        }
    }]);

    return OpenHistorianGrafanaAlarmPanel;
}(sdk_1.MetricsPanelCtrl);

OpenHistorianGrafanaAlarmPanel.templateUrl = 'partials/module.html';
exports.OpenHistorianGrafanaAlarmPanel = OpenHistorianGrafanaAlarmPanel;

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