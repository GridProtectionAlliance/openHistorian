import {MetricsPanelCtrl} from 'app/plugins/sdk';
import _ from 'lodash';
import kbn from 'app/core/utils/kbn';
import TimeSeries from 'app/core/time_series';
// import rendering from './rendering';
import legend from './legend';
import 'https://scadavis.io/synoptic/synopticapi.js';

export class SCADAvisCtrl extends MetricsPanelCtrl {

  constructor($scope, $injector, $rootScope) {
    super($scope, $injector);
    this.$rootScope = $rootScope;
    this.hiddenSeries = {};

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

    _.defaults(this.panel, panelDefaults);
    _.defaults(this.panel.legend, panelDefaults.legend);

    this.events.on('render', this.onRender.bind(this));
    this.events.on('data-received', this.onDataReceived.bind(this));
    this.events.on('data-error', this.onDataError.bind(this));
    this.events.on('data-snapshot-load', this.onDataReceived.bind(this));
    this.events.on('init-edit-mode', this.onInitEditMode.bind(this));

    this.setLegendWidthForLegacyBrowser();
  }

  onInitEditMode() {
    this.addEditorTab('Options', 'public/plugins/scadavis-synoptic-panel/editor.html', 2);
    this.unitFormats = kbn.getUnitFormats();
  }

  setUnitFormat(subItem) {
    this.panel.format = subItem.value;
    this.render();
  }

  onDataError() {
    this.series = [];
    this.render();
  }

  changeSeriesColor(series, color) {
    series.color = color;
    this.panel.aliasColors[series.alias] = series.color;
    this.render();
  }

  onRender() {
    this.data = this.parseSeries(this.series);
  }

  parseSeries(series) {
    return _.map(this.series, (serie, i) => {
      return {
        label: serie.alias,
        data: serie.stats[this.panel.valueName],
        color: this.panel.aliasColors[serie.alias] || this.$rootScope.colors[i],
        legendData: serie.stats[this.panel.valueName],
      };
    });
  }

  onDataReceived(dataList) {
    this.series = dataList.map(this.seriesHandler.bind(this));
    this.data = this.parseSeries(this.series);
    this.render(this.data);
    if ( typeof this.$panelContainer != 'undefined' ) {
      var svelem = this.$panelContainer[0].querySelector('.scadavis-panel__chart');

      if (svelem && typeof svelem.svgraph === "undefined") {      
        svelem.svgraph = new scadavis({ svgurl: this.panel.svgurl, 
                                        container: svelem, 
                                        iframeparams: 'frameborder="0" width="'+ svelem.clientWidth +'" height="'+ svelem.clientHeight +'"'
                                      });
        svelem.svgraph.enableMouse(false, false);
        svelem.svgraph.zoomTo(this.panel.zoomLevel);
        svelem.svgraph.hideWatermark();
        this.panel._lastZoomLevel = this.panel.zoomLevel;
        }

      if (svelem && typeof svelem.svgraph !== "undefined") {
        if ( this.panel._lastZoomLevel != this.panel.zoomLevel ) {  
          svelem.svgraph.zoomToOriginal();
          svelem.svgraph.zoomTo(this.panel.zoomLevel);
          this.panel._lastZoomLevel = this.panel.zoomLevel;
          }
        //if (svelem.svgraph.getComponentState() === 0) {
        //   svelem.svgraph.loadURL(this.panel.svgurl);
        //   }
        //else
        if ( svelem.svgraph.svgurl != this.panel.svgurl ) {
           svelem.svgraph.loadURL(this.panel.svgurl);
           }        
        svelem.svgraph.enableTools(this.panel.showZoomPan, this.panel.showZoomPan);
        for (var i=0; i<this.data.length; i++)
          svelem.svgraph.setValue(this.data[i].label, this.data[i].data); 
      }

    }

  }

  seriesHandler(seriesData) {
    var series = new TimeSeries({
      datapoints: seriesData.datapoints,
      alias: seriesData.target
    });

    series.flotpairs = series.getFlotPairs(this.panel.nullPointMode);
    return series;
  }

  getDecimalsForValue(value) {
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
    if (Math.floor(value) === value) { dec = 0; }

    var result = {};
    result.decimals = Math.max(0, dec);
    result.scaledDecimals = result.decimals - Math.floor(Math.log(size) / Math.LN10) + 2;

    return result;
  }

  formatValue(value) {
    var decimalInfo = this.getDecimalsForValue(value);
    var formatFunc = kbn.valueFormats[this.panel.format];
    if (formatFunc) {
      return formatFunc(value, decimalInfo.decimals, decimalInfo.scaledDecimals);
    }
    return value;
  }

  link(scope, elem, attrs, ctrl) {
    this.$panelContainer = elem.find('.panel-container');
    // rendering(scope, elem, attrs, ctrl);
  }

  toggleSeries(serie) {
    if (this.hiddenSeries[serie.label]) {
      delete this.hiddenSeries[serie.label];
    } else {
      this.hiddenSeries[serie.label] = true;
    }
    this.render();
  }

  onLegendTypeChanged() {
    this.setLegendWidthForLegacyBrowser();
    this.render();
  }

  setLegendWidthForLegacyBrowser() {
    var isIE11 = !!window.MSInputMethodContext && !!document.documentMode;
    if (isIE11 && this.panel.legendType === 'Right side' && !this.panel.legend.sideWidth) {
      this.panel.legend.sideWidth = 150;
    }
  }
}

SCADAvisCtrl.templateUrl = 'module.html';
