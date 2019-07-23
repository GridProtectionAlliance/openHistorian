'use strict';

System.register(['app/plugins/sdk', 'lodash', 'app/core/utils/kbn', 'app/core/time_series', './legend', 'https://scadavis.io/synoptic/synopticapi.js'], function (_export, _context) {
  "use strict";

  var MetricsPanelCtrl, _, kbn, TimeSeries, legend, _createClass, SCADAvisCtrl;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  function _possibleConstructorReturn(self, call) {
    if (!self) {
      throw new ReferenceError("this hasn't been initialised - super() hasn't been called");
    }

    return call && (typeof call === "object" || typeof call === "function") ? call : self;
  }

  function _inherits(subClass, superClass) {
    if (typeof superClass !== "function" && superClass !== null) {
      throw new TypeError("Super expression must either be null or a function, not " + typeof superClass);
    }

    subClass.prototype = Object.create(superClass && superClass.prototype, {
      constructor: {
        value: subClass,
        enumerable: false,
        writable: true,
        configurable: true
      }
    });
    if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass;
  }

  return {
    setters: [function (_appPluginsSdk) {
      MetricsPanelCtrl = _appPluginsSdk.MetricsPanelCtrl;
    }, function (_lodash) {
      _ = _lodash.default;
    }, function (_appCoreUtilsKbn) {
      kbn = _appCoreUtilsKbn.default;
    }, function (_appCoreTime_series) {
      TimeSeries = _appCoreTime_series.default;
    }, function (_legend) {
      legend = _legend.default;
    }, function (_httpsScadavisIoSynopticSynopticapiJs) {}],
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

      _export('SCADAvisCtrl', SCADAvisCtrl = function (_MetricsPanelCtrl) {
        _inherits(SCADAvisCtrl, _MetricsPanelCtrl);

        function SCADAvisCtrl($scope, $injector, $rootScope) {
          _classCallCheck(this, SCADAvisCtrl);

          var _this = _possibleConstructorReturn(this, (SCADAvisCtrl.__proto__ || Object.getPrototypeOf(SCADAvisCtrl)).call(this, $scope, $injector));

          _this.$rootScope = $rootScope;
          _this.hiddenSeries = {};

          var panelDefaults = {
            svgurl: 'https://raw.githubusercontent.com/riclolsen/displayfiles/master/helloworld.svg',
            showZoomPan: false,
            zoomLevel: 1.0,
            legend: {
              show: true, // disable/enable legend
              values: true
            },
            links: [],
            datasource: null,
            maxDataPoints: 3,
            interval: null,
            targets: [{}],
            cacheTimeout: null,
            nullPointMode: 'connected',
            legendType: 'Under graph',
            breakPoint: '50%',
            aliasColors: {},
            format: 'short',
            valueName: 'current',
            strokeWidth: 1,
            fontSize: '80%'
          };

          _.defaults(_this.panel, panelDefaults);
          _.defaults(_this.panel.legend, panelDefaults.legend);

          _this.events.on('render', _this.onRender.bind(_this));
          _this.events.on('data-received', _this.onDataReceived.bind(_this));
          _this.events.on('data-error', _this.onDataError.bind(_this));
          _this.events.on('data-snapshot-load', _this.onDataReceived.bind(_this));
          _this.events.on('init-edit-mode', _this.onInitEditMode.bind(_this));

          _this.setLegendWidthForLegacyBrowser();
          return _this;
        }

        _createClass(SCADAvisCtrl, [{
          key: 'onInitEditMode',
          value: function onInitEditMode() {
            this.addEditorTab('Options', 'public/plugins/scadavis-synoptic-panel/editor.html', 2);
            this.unitFormats = kbn.getUnitFormats();
          }
        }, {
          key: 'setUnitFormat',
          value: function setUnitFormat(subItem) {
            this.panel.format = subItem.value;
            this.render();
          }
        }, {
          key: 'onDataError',
          value: function onDataError() {
            this.series = [];
            this.render();
          }
        }, {
          key: 'changeSeriesColor',
          value: function changeSeriesColor(series, color) {
            series.color = color;
            this.panel.aliasColors[series.alias] = series.color;
            this.render();
          }
        }, {
          key: 'onRender',
          value: function onRender() {
            this.data = this.parseSeries(this.series);
          }
        }, {
          key: 'parseSeries',
          value: function parseSeries(series) {
            var _this2 = this;

            return _.map(this.series, function (serie, i) {
              return {
                label: serie.alias,
                data: serie.stats[_this2.panel.valueName],
                color: _this2.panel.aliasColors[serie.alias] || _this2.$rootScope.colors[i],
                legendData: serie.stats[_this2.panel.valueName]
              };
            });
          }
        }, {
          key: 'onDataReceived',
          value: function onDataReceived(dataList) {
            this.series = dataList.map(this.seriesHandler.bind(this));
            this.data = this.parseSeries(this.series);
            this.render(this.data);
            if (typeof this.$panelContainer != 'undefined') {
              var svelem = this.$panelContainer[0].querySelector('.scadavis-panel__chart');

              if (svelem && typeof svelem.svgraph === "undefined") {
                svelem.svgraph = new scadavis({ svgurl: this.panel.svgurl,
                  container: svelem,
                  iframeparams: 'frameborder="0" width="' + svelem.clientWidth + '" height="' + svelem.clientHeight + '"'
                });
                svelem.svgraph.enableMouse(false, false);
                svelem.svgraph.zoomTo(this.panel.zoomLevel);
                svelem.svgraph.hideWatermark();
                this.panel._lastZoomLevel = this.panel.zoomLevel;
              }

              if (svelem && typeof svelem.svgraph !== "undefined") {
                if (this.panel._lastZoomLevel != this.panel.zoomLevel) {
                  svelem.svgraph.zoomToOriginal();
                  svelem.svgraph.zoomTo(this.panel.zoomLevel);
                  this.panel._lastZoomLevel = this.panel.zoomLevel;
                }
                //if (svelem.svgraph.getComponentState() === 0) {
                //   svelem.svgraph.loadURL(this.panel.svgurl);
                //   }
                //else
                if (svelem.svgraph.svgurl != this.panel.svgurl) {
                  svelem.svgraph.loadURL(this.panel.svgurl);
                }
                svelem.svgraph.enableTools(this.panel.showZoomPan, this.panel.showZoomPan);
                for (var i = 0; i < this.data.length; i++) {
                  svelem.svgraph.setValue(this.data[i].label, this.data[i].data);
                }
              }
            }
          }
        }, {
          key: 'seriesHandler',
          value: function seriesHandler(seriesData) {
            var series = new TimeSeries({
              datapoints: seriesData.datapoints,
              alias: seriesData.target
            });

            series.flotpairs = series.getFlotPairs(this.panel.nullPointMode);
            return series;
          }
        }, {
          key: 'getDecimalsForValue',
          value: function getDecimalsForValue(value) {
            if (_.isNumber(this.panel.decimals)) {
              return { decimals: this.panel.decimals, scaledDecimals: null };
            }

            var delta = value / 2;
            var dec = -Math.floor(Math.log(delta) / Math.LN10);

            var magn = Math.pow(10, -dec);
            var norm = delta / magn; // norm is between 1.0 and 10.0
            var size;

            if (norm < 1.5) {
              size = 1;
            } else if (norm < 3) {
              size = 2;
              // special case for 2.5, requires an extra decimal
              if (norm > 2.25) {
                size = 2.5;
                ++dec;
              }
            } else if (norm < 7.5) {
              size = 5;
            } else {
              size = 10;
            }

            size *= magn;

            // reduce starting decimals if not needed
            if (Math.floor(value) === value) {
              dec = 0;
            }

            var result = {};
            result.decimals = Math.max(0, dec);
            result.scaledDecimals = result.decimals - Math.floor(Math.log(size) / Math.LN10) + 2;

            return result;
          }
        }, {
          key: 'formatValue',
          value: function formatValue(value) {
            var decimalInfo = this.getDecimalsForValue(value);
            var formatFunc = kbn.valueFormats[this.panel.format];
            if (formatFunc) {
              return formatFunc(value, decimalInfo.decimals, decimalInfo.scaledDecimals);
            }
            return value;
          }
        }, {
          key: 'link',
          value: function link(scope, elem, attrs, ctrl) {
            this.$panelContainer = elem.find('.panel-container');
            // rendering(scope, elem, attrs, ctrl);
          }
        }, {
          key: 'toggleSeries',
          value: function toggleSeries(serie) {
            if (this.hiddenSeries[serie.label]) {
              delete this.hiddenSeries[serie.label];
            } else {
              this.hiddenSeries[serie.label] = true;
            }
            this.render();
          }
        }, {
          key: 'onLegendTypeChanged',
          value: function onLegendTypeChanged() {
            this.setLegendWidthForLegacyBrowser();
            this.render();
          }
        }, {
          key: 'setLegendWidthForLegacyBrowser',
          value: function setLegendWidthForLegacyBrowser() {
            var isIE11 = !!window.MSInputMethodContext && !!document.documentMode;
            if (isIE11 && this.panel.legendType === 'Right side' && !this.panel.legend.sideWidth) {
              this.panel.legend.sideWidth = 150;
            }
          }
        }]);

        return SCADAvisCtrl;
      }(MetricsPanelCtrl));

      _export('SCADAvisCtrl', SCADAvisCtrl);

      SCADAvisCtrl.templateUrl = 'module.html';
    }
  };
});
//# sourceMappingURL=scadavis_ctrl.js.map
