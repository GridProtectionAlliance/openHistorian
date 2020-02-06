define(["app/plugins/sdk","moment"], function(__WEBPACK_EXTERNAL_MODULE_grafana_app_plugins_sdk__, __WEBPACK_EXTERNAL_MODULE_moment__) { return /******/ (function(modules) { // webpackBootstrap
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

/***/ "./OHDownload_ctrl.ts":
/*!****************************!*\
  !*** ./OHDownload_ctrl.ts ***!
  \****************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
eval("\n//******************************************************************************************************\n//  OHDowload_ctrl.ts - Gbtc\n//\n//  Copyright © 2020, Grid Protection Alliance.  All Rights Reserved.\n//\n//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See\n//  the NOTICE file distributed with this work for additional information regarding copyright ownership.\n//  The GPA licenses this file to you under the MIT License (MIT), the \"License\"; you may not use this\n//  file except in compliance with the License. You may obtain a copy of the License at:\n//\n//      http://opensource.org/licenses/MIT\n//\n//  Unless agreed to in writing, the subject software distributed under the License is distributed on an\n//  \"AS-IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the\n//  License for the specific language governing permissions and limitations.\n//\n//  Code Modification History:\n//  ----------------------------------------------------------------------------------------------------\n//  02/03/2020 - C.Lackner\n//       Generated original version of source code.\n//\n//******************************************************************************************************\n\nvar _createClass = function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if (\"value\" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; }();\n\nfunction _toConsumableArray(arr) { if (Array.isArray(arr)) { for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) { arr2[i] = arr[i]; } return arr2; } else { return Array.from(arr); } }\n\nfunction _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError(\"Cannot call a class as a function\"); } }\n\nfunction _possibleConstructorReturn(self, call) { if (!self) { throw new ReferenceError(\"this hasn't been initialised - super() hasn't been called\"); } return call && (typeof call === \"object\" || typeof call === \"function\") ? call : self; }\n\nfunction _inherits(subClass, superClass) { if (typeof superClass !== \"function\" && superClass !== null) { throw new TypeError(\"Super expression must either be null or a function, not \" + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }\n\nObject.defineProperty(exports, \"__esModule\", { value: true });\n//<reference path=\"../node_modules/grafana-sdk-mocks/app/headers/common.d.ts\" />\nvar sdk_1 = __webpack_require__(/*! grafana/app/plugins/sdk */ \"grafana/app/plugins/sdk\");\nvar moment = __webpack_require__(/*! moment */ \"moment\");\n//import { varName } from '../js/constants'   // import constants from constant file using this format\n\nvar OHDownloadCtrl = function (_sdk_1$MetricsPanelCt) {\n    _inherits(OHDownloadCtrl, _sdk_1$MetricsPanelCt);\n\n    function OHDownloadCtrl($scope, $injector, $rootScope) {\n        _classCallCheck(this, OHDownloadCtrl);\n\n        var _this = _possibleConstructorReturn(this, (OHDownloadCtrl.__proto__ || Object.getPrototypeOf(OHDownloadCtrl)).call(this, $scope, $injector));\n\n        _this.$rootScope = $rootScope;\n        _this.textSizeOptions = ['50%', '60%', '70%', '80%', '90%', '100%', '110%', '120%', '130%', '140%', '150%', '160%', '170%', '180%', '190%', '200%', '250%', '300%'];\n        _this.events.on('init-edit-mode', _this.onInitEditMode.bind(_this));\n        _this.events.on('panel-teardown', _this.onPanelTeardown.bind(_this));\n        _this.events.on('render', _this.onRender.bind(_this));\n        _this.events.on('panel-initialized', _this.onPanelInitialized.bind(_this));\n        _this.events.on('data-received', _this.onDataRecieved.bind(_this));\n        //this.events.on('data-snapshot-load', console.log('data-snapshot-load'));\n        _this.events.on('data-error', _this.onDataError.bind(_this));\n        _this.events.on('refresh', _this.onRefresh.bind(_this));\n        _this.panel.link = _this.panel.link != undefined ? _this.panel.link : '..';\n        _this.panel.rate = _this.panel.rate != undefined ? _this.panel.rate : 30;\n        _this.panel.rateBase = _this.panel.rateBase != undefined ? _this.panel.rateBase : 'frames per second';\n        _this.panel.firstTS = _this.panel.firstTS != undefined ? _this.panel.firstTS : 'first available measurement';\n        _this.panel.alignTS = _this.panel.alignTS != undefined ? _this.panel.alignTS : true;\n        _this.panel.exportNaN = _this.panel.exportNaN != undefined ? _this.panel.exportNaN : true;\n        _this.panel.missingTS = _this.panel.missingTS != undefined ? _this.panel.missingTS : true;\n        _this.panel.round = _this.panel.round != undefined ? _this.panel.round : true;\n        _this.panel.color = _this.panel.color != undefined ? _this.panel.color : '#ff0000';\n        _this.panel.textcolor = _this.panel.textcolor != undefined ? _this.panel.textcolor : '#ffffff';\n        _this.panel.fontsize = _this.panel.fontsize != undefined ? _this.panel.fontsize : '100%';\n        _this.fileLink = \"\";\n        _this.startTime = null;\n        _this.endTime = null;\n        return _this;\n    }\n    // #region Events from Graphana Handlers\n\n\n    _createClass(OHDownloadCtrl, [{\n        key: \"onInitEditMode\",\n        value: function onInitEditMode() {\n            this.addEditorTab('Options', 'public/plugins/openhistorian-datadownload-panel/partials/editor.html', 2);\n        }\n    }, {\n        key: \"onPanelTeardown\",\n        value: function onPanelTeardown() {\n            //console.log('panel-teardown');\n        }\n    }, {\n        key: \"onPanelInitialized\",\n        value: function onPanelInitialized() {\n            //console.log('panel-initialized');\n        }\n    }, {\n        key: \"onRefresh\",\n        value: function onRefresh() {\n            //console.log('refresh');\n        }\n    }, {\n        key: \"onResize\",\n        value: function onResize() {\n            //console.log('refresh');\n        }\n    }, {\n        key: \"onRender\",\n        value: function onRender() {\n            //console.log('render');\n        }\n    }, {\n        key: \"onDataRecieved\",\n        value: function onDataRecieved(data) {\n            var _this2 = this;\n\n            var ctrl = this;\n            var promiseData = [];\n            ctrl.pointIDs = [];\n            var minTS = Number.MAX_SAFE_INTEGER;\n            var maxTS = Number.MIN_SAFE_INTEGER;\n            data.forEach(function (point) {\n                promiseData.push(_this2.datasource.getMetaData(point.target).then(function (metaData) {\n                    var jsonMetaData = JSON.parse(metaData.data);\n                    var id = jsonMetaData[0][\"ID\"].split(\":\");\n                    ctrl.pointIDs.push(id[1].trim());\n                }));\n                if (point.datapoints.length > 0) {\n                    if (Math.min.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {\n                        return item[1];\n                    }))) < minTS) minTS = Math.min.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {\n                        return item[1];\n                    })));\n                    if (Math.max.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {\n                        return item[1];\n                    }))) > maxTS) maxTS = Math.max.apply(Math, _toConsumableArray(point.datapoints.map(function (item) {\n                        return item[1];\n                    })));\n                }\n            });\n            this.startTime = moment.utc(minTS);\n            this.endTime = moment.utc(maxTS);\n            Promise.all(promiseData).then(function () {\n                ctrl.updateLink();\n            });\n        }\n    }, {\n        key: \"onDataError\",\n        value: function onDataError(msg) {\n            //console.log('data-error');\n        }\n    }, {\n        key: \"handleClick\",\n        value: function handleClick(d) {\n            window.location.href = this.fileLink;\n        }\n    }, {\n        key: \"updateLink\",\n        value: function updateLink() {\n            var framerate = this.panel.rate;\n            if (this.panel.rateBase == 'frames per minute') {\n                framerate = framerate / 60.0;\n            }\n            if (this.panel.rateBase == 'frames per hour') {\n                framerate = framerate / (60.0 * 60.0);\n            }\n            var tolerance = 0.5;\n            if (this.panel.round) {\n                tolerance = 500 / framerate;\n            }\n            this.fileLink = this.panel.link + '\\ExportDataHandler.ashx?PointIDs=' + this.pointIDs.join(\",\") + \"&StartTime=\" + this.startTime.format(\"MM/DD/YYYY HH:mm:ss.000000\") + \"&EndTime=\" + this.endTime.format(\"MM/DD/YYYY HH:mm:ss.000000\") + \"&FrameRate=\" + framerate.toString() + \"&AlignTimestamps=\" + (this.panel.alignTS ? \"true\" : \"false\") + \"&MissingAsNaNParam=\" + (this.panel.exportNaN ? \"true\" : \"false\") + \"&FillMissingTimestamps=\" + (this.panel.missingTS ? \"true\" : \"false\") + \"&InstanceName=\" + \"PPA\" + \"&TSSnap=\" + this.formatTSSnap().toString() + \"&TSTolerance=\" + tolerance.toString();\n        }\n    }, {\n        key: \"formatTSSnap\",\n        value: function formatTSSnap() {\n            if (this.panel.firstTS == 'first available measurement') return 1;\n            return 2;\n        }\n    }]);\n\n    return OHDownloadCtrl;\n}(sdk_1.MetricsPanelCtrl);\n\nOHDownloadCtrl.templateUrl = 'partials/module.html';\nexports.OHDownloadCtrl = OHDownloadCtrl;\n\n//# sourceURL=webpack:///./OHDownload_ctrl.ts?");

/***/ }),

/***/ "./module.ts":
/*!*******************!*\
  !*** ./module.ts ***!
  \*******************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

"use strict";
eval("\n//******************************************************************************************************\n//  module.ts - Gbtc\n//\n//  Copyright © 2017, Grid Protection Alliance.  All Rights Reserved.\n//\n//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See\n//  the NOTICE file distributed with this work for additional information regarding copyright ownership.\n//  The GPA licenses this file to you under the MIT License (MIT), the \"License\"; you may not use this\n//  file except in compliance with the License. You may obtain a copy of the License at:\n//\n//      http://opensource.org/licenses/MIT\n//\n//  Unless agreed to in writing, the subject software distributed under the License is distributed on an\n//  \"AS-IS\" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the\n//  License for the specific language governing permissions and limitations.\n//\n//  Code Modification History:\n//  ----------------------------------------------------------------------------------------------------\n//  12/15/2017 - Billy Ernest\n//       Generated original version of source code.\n//\n//******************************************************************************************************\n\nObject.defineProperty(exports, \"__esModule\", { value: true });\nvar OHDownload_ctrl_1 = __webpack_require__(/*! ./OHDownload_ctrl */ \"./OHDownload_ctrl.ts\");\nexports.PanelCtrl = OHDownload_ctrl_1.OHDownloadCtrl;\n\n//# sourceURL=webpack:///./module.ts?");

/***/ }),

/***/ "grafana/app/plugins/sdk":
/*!**********************************!*\
  !*** external "app/plugins/sdk" ***!
  \**********************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("module.exports = __WEBPACK_EXTERNAL_MODULE_grafana_app_plugins_sdk__;\n\n//# sourceURL=webpack:///external_%22app/plugins/sdk%22?");

/***/ }),

/***/ "moment":
/*!*************************!*\
  !*** external "moment" ***!
  \*************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("module.exports = __WEBPACK_EXTERNAL_MODULE_moment__;\n\n//# sourceURL=webpack:///external_%22moment%22?");

/***/ })

/******/ })});;