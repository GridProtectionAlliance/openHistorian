'use strict';

System.register(['angular', 'lodash', 'app/plugins/sdk', './css/query-editor.css!'], function (_export, _context) {
  "use strict";

  var angular, _, QueryCtrl, _createClass, PiWebApiDatasourceQueryCtrl;

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
    setters: [function (_angular) {
      angular = _angular.default;
    }, function (_lodash) {
      _ = _lodash.default;
    }, function (_appPluginsSdk) {
      QueryCtrl = _appPluginsSdk.QueryCtrl;
    }, function (_cssQueryEditorCss) {}],
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

      _export('PiWebApiDatasourceQueryCtrl', PiWebApiDatasourceQueryCtrl = function (_QueryCtrl) {
        _inherits(PiWebApiDatasourceQueryCtrl, _QueryCtrl);

        function PiWebApiDatasourceQueryCtrl($scope, $injector, uiSegmentSrv, templateSrv, $q) {
          _classCallCheck(this, PiWebApiDatasourceQueryCtrl);

          var _this = _possibleConstructorReturn(this, Object.getPrototypeOf(PiWebApiDatasourceQueryCtrl).call(this, $scope, $injector));

          _this.uiSegmentSrv = uiSegmentSrv;
          _this.templateSrv = templateSrv;
          _this.$q = $q;
          _this.segments = [];
          _this.attributes = [];
          _this.availableAttributes = {};
          _this.attributeSegment = _this.uiSegmentSrv.newPlusButton();

          _this.summaries = [];
          _this.summarySegment = _this.uiSegmentSrv.newPlusButton();
          _this.summaryTypes = [
          // 'None', // A summary type is not specified.
          'Total', // A totalization over the time range.
          'Average', // The average value over the time range.
          'Minimum', // The minimum value over the time range.
          'Maximum', // The maximum value over the time range.
          'Range', // The range value over the time range (minimum-maximum).
          'StdDev', // The standard deviation over the time range.
          'PopulationStdDev', // The population standard deviation over the time range.
          'Count', // The sum of event count over the time range when calculation basis is event weighted. The sum of event time duration over the time range when calculation basis is time weighted.
          'PercentGood', // Percent of data with good value during the calculation period. For time weighted calculations, the percentage is based on time. For event weighted calculations, the percent is based on event count.
          'All', // A convenience for requesting all available summary calculations.
          'AllForNonNumeric' // A convenience for requesting all available summary calculations for non-numeric data.
          ];

          _this.calculationBasisSegment = _this.uiSegmentSrv.newSegment('EventWeighted');
          _this.calculationBasis = ['TimeWeighted', // Weight the values in the calculation by the time over which they apply. Interpolation is based on whether the attribute is stepped. Interpolated events are generated at the boundaries if necessary.
          'EventWeighted', // Evaluate values with equal weighting for each event. No interpolation is done. There must be at least one event within the time range to perform a successful calculation. Two events are required for standard deviation. In handling events at the boundary of the calculation, the AFSDK uses following rules:
          'TimeWeightedContinuous', // Apply weighting as in TimeWeighted, but do all interpolation between values as if they represent continuous data, (standard interpolation) regardless of whether the attribute is stepped.
          'TimeWeightedDiscrete', // Apply weighting as in TimeWeighted but interpolation between values is performed as if they represent discrete, unrelated values (stair step plot) regardless of the attribute is stepped.
          'EventWeightedExcludeMostRecentEvent', // The calculation behaves the same as _EventWeighted_, except in the handling of events at the boundary of summary intervals in a multiple intervals calculation. Use this option to prevent events at the intervals boundary from being double count at both intervals. With this option, events at the end time (most recent time) of an interval is not used in that interval.
          'EventWeightedExcludeEarliestEvent', // Similar to the option _EventWeightedExcludeMostRecentEvent_. Events at the start time(earliest time) of an interval is not used in that interval.
          'EventWeightedIncludeBothEnds' // Events at both ends of the interval boundaries are included in the event weighted calculation.
          ];

          _this.noDataReplacementSegment = _this.uiSegmentSrv.newSegment('Null');
          _this.noDataReplacement = ['Null', // replace with nulls
          'Drop', // drop items
          'Previous', // use previous value if available
          '0'];

          _this.target.summary = _this.target.summary || { types: [], basis: 'EventWeighted', interval: '5m', nodata: 'Null' };
          _this.target.summary.types = _this.target.summary.types || [];
          _this.target.summary.basis = _this.target.summary.basis || 'EventWeighted';
          _this.target.summary.nodata = _this.target.summary.nodata || 'Null';
          _this.target.summary.interval = _this.target.summary.interval || '5m';

          _this.target.target = _this.target.target || ';';

          _this.target.interpolate = _this.target.interpolate || { enable: false };
          if (_this.target.interpolate === false || _this.target.interpolate === true) {
            _this.target.interpolate = { enable: _this.target.interpolate };
          }
          _this.target.interpolate.enable = _this.target.interpolate.enable || false;

          if (_this.segments.length === 0) {
            _this.segments.push(_this.uiSegmentSrv.newSelectMetric());
          }

          _this.textEditorChanged();
          return _this;
        }

        /**
         * Queries PI Web API for child elements and attributes when the query segments or options are changed.
         *  
         * @memberOf PiWebApiDatasourceQueryCtrl
         */


        _createClass(PiWebApiDatasourceQueryCtrl, [{
          key: 'targetChanged',
          value: function targetChanged() {
            var _this2 = this;

            if (this.error) {
              return;
            }

            var ctrl = this;

            var oldTarget = this.target.target;
            var oldElement = this.target.target.split(';')[0];
            var element = this.getSegmentPathUpTo(this.segments.length);
            var attributes = this.getAttributes();
            var target = element + ';' + attributes;
            this.target.target = target; // _.reduce(this.functions, this.wrapFunction, target)
            this.target.elementPath = element;
            this.target.attributes = attributes.split(';');
            this.target.summary.types = this.getSummaries().split(',');

            this.target.webids = _.map(ctrl.target.attributes, function (attrib) {
              return ctrl.availableAttributes[attrib];
            });

            if (element !== oldElement || this.target.webid === undefined) {
              var segmentValue = this.segments[this.segments.length - 1].value;
              if (!segmentValue.startsWith('Select AF')) {
                this.panelCtrl.refresh();
              }

              this.datasource.getElement(element).then(function (results) {
                _this2.target.webid = results.WebId;
              });
            }
          }
        }, {
          key: 'textEditorChanged',
          value: function textEditorChanged() {
            var ctrl = this;
            var splitAttributes = ctrl.target.target.split(';');
            var splitElements = splitAttributes[0].split('\\');

            // remove element hierarchy from attribute collection
            splitAttributes.splice(0, 1);

            var segments = [];
            var attributes = [];

            _.each(splitElements, function (item) {
              segments.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }));
            });

            _.each(splitAttributes, function (item) {
              attributes.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }));
            });

            ctrl.segments = segments;
            ctrl.checkOtherSegments(segments.length - 1);

            ctrl.attributes = attributes;
            ctrl.checkAttributeSegments().then(function (r) {
              ctrl.targetChanged();
              ctrl.refresh();
            });

            var summaries = [];
            _.each(ctrl.target.summary.types, function (item) {
              if (item) {
                summaries.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }));
              }
            });
            ctrl.summaries = summaries;
          }
        }, {
          key: 'toggleEditorMode',
          value: function toggleEditorMode() {
            this.target.textEditor = !this.target.textEditor;
          }
        }, {
          key: 'getSegmentPathUpTo',
          value: function getSegmentPathUpTo(index) {
            var arr = this.segments.slice(0, index);

            return _.reduce(arr, function (result, segment) {
              if (!segment.value.startsWith('Select AF')) {
                return result ? result + '\\' + segment.value : segment.value;
              }
              return result;
            }, '');
          }
        }, {
          key: 'getAttributes',
          value: function getAttributes() {
            var arr = this.attributes.slice(0, this.attributes.length);

            return _.reduce(arr, function (result, segment) {
              if (!segment.value.startsWith('Select AF')) {
                return result ? result + ';' + segment.value : segment.value;
              }
              return result;
            }, '');
          }
        }, {
          key: 'getSummaries',
          value: function getSummaries() {
            var arr = this.summaries.slice(0, this.summaries.length);

            return _.reduce(arr, function (result, segment) {
              if (segment && !segment.value.startsWith('Select AF')) {
                return result ? result + ',' + segment.value : segment.value;
              }
              return result;
            }, '');
          }
        }, {
          key: 'getCollapsedText',
          value: function getCollapsedText() {
            return '[' + (this.target.display || '') + ']: ' + this.target.target;
          }
        }, {
          key: 'isElementSegmentExpandable',
          value: function isElementSegmentExpandable(element) {
            return element.HasChildren === undefined || element.HasChildren === true || element.Path.split('\\').length <= 3;
          }
        }, {
          key: 'checkAttributeSegments',
          value: function checkAttributeSegments() {
            var ctrl = this;
            var query = {
              path: this.getSegmentPathUpTo(this.segments.length),
              type: 'attributes'
            };

            return this.datasource.metricFindQuery(angular.toJson(query)).then(function (attributes) {
              var validAttributes = {};

              _.each(attributes, function (attribute) {
                validAttributes[attribute.Path.substring(attribute.Path.indexOf('|') + 1)] = attribute.WebId;
              });

              var filteredAttributes = _.filter(ctrl.attributes, function (attrib) {
                return validAttributes[attrib.value] !== undefined;
              });

              ctrl.availableAttributes = validAttributes;
              ctrl.attributes = filteredAttributes;
            }).catch(function (err) {
              ctrl.error = err.message || 'Failed to issue metric query';
              ctrl.attributes = [];
            });
          }
        }, {
          key: 'checkOtherSegments',
          value: function checkOtherSegments(fromIndex) {
            var ctrl = this;
            var query = { path: ctrl.getSegmentPathUpTo(fromIndex + 1) };

            if (ctrl.segments.length === 0) {
              ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Database"));
            }

            return ctrl.datasource.metricFindQuery(angular.toJson(query)).then(function (children) {
              if (children.length === 0) {
                if (query.path !== '') {
                  ctrl.segments = ctrl.segments.splice(0, fromIndex + 1);
                  if (ctrl.segments[ctrl.segments.length - 1].expandable) {
                    ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Database"));
                  }
                }
              } else /* if (this.isElementSegmentExpandable(segments[0])) */{
                  if (ctrl.segments.length === fromIndex) {
                    ctrl.segments = ctrl.segments.splice(0, fromIndex);
                    if (ctrl.segments[ctrl.segments.length - 1].expandable) {
                      ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Element"));
                    }
                  } else {
                    return ctrl.checkOtherSegments(fromIndex + 1);
                  }
                }
            }).catch(function (err) {
              ctrl.segments = ctrl.segments.splice(0, fromIndex);
              if (ctrl.segments[ctrl.segments.length - 1].expandable) {
                ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Element"));
              }
              ctrl.error = err.message || 'Failed to issue metric query';
            });
          }
        }, {
          key: 'setSegmentFocus',
          value: function setSegmentFocus(segmentIndex) {
            _.each(this.segments, function (segment, index) {
              segment.focus = false;
              segment.focus = segmentIndex === index;
            });
          }
        }, {
          key: 'wrapFunction',
          value: function wrapFunction(target, func) {
            return func.render(target);
          }
        }, {
          key: 'isValueEmpty',
          value: function isValueEmpty(value) {
            return value === undefined || value === null || value === '' || value === '-REMOVE-';
          }
        }, {
          key: 'calcBasisValueChanged',
          value: function calcBasisValueChanged(segment, index) {
            this.target.summary.basis = this.calculationBasisSegment.value;
            this.targetChanged();
            this.panelCtrl.refresh();
          }
        }, {
          key: 'calcNoDataValueChanged',
          value: function calcNoDataValueChanged(segment, index) {
            this.target.summary.nodata = this.noDataReplacementSegment.value;
            this.targetChanged();
            this.panelCtrl.refresh();
          }
        }, {
          key: 'getCalcBasisSegments',
          value: function getCalcBasisSegments() {
            var ctrl = this;
            var segments = _.map(this.calculationBasis, function (item) {
              return ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true });
            });
            return this.$q.when(segments);
          }
        }, {
          key: 'getNoDataSegments',
          value: function getNoDataSegments() {
            var ctrl = this;
            var segments = _.map(this.noDataReplacement, function (item) {
              return ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true });
            });
            return this.$q.when(segments);
          }
        }, {
          key: 'removeSummary',
          value: function removeSummary(part) {
            this.summaries = _.filter(this.summaries, function (item) {
              return item !== part;
            });
            this.panelCtrl.refresh();
          }
        }, {
          key: 'summaryAction',
          value: function summaryAction() {
            // if value is not empty, add new attribute segment
            if (!this.isValueEmpty(this.summarySegment.value)) {
              this.summaries.push(this.uiSegmentSrv.newSegment({ value: this.summarySegment.value, expandable: true }));
              this.targetChanged();
            }

            // reset the + button
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.summarySegment.value = plusButton.value;
            this.summarySegment.html = plusButton.html;
            this.panelCtrl.refresh();
          }
        }, {
          key: 'summaryValueChanged',
          value: function summaryValueChanged(segment, index) {
            this.summaries[index].value = segment.value;
            if (this.isValueEmpty(segment.value)) {
              this.summaries.splice(index, 1);
            }
            this.targetChanged();
            this.panelCtrl.refresh();
          }
        }, {
          key: 'getSummarySegments',
          value: function getSummarySegments() {
            var ctrl = this;
            // var segments = _.map(_.difference(ctrl.summaryTypes, _.map(ctrl.summaries, seg => { return seg.value || '' })), item => {
            var segments = _.map(ctrl.summaryTypes, function (item) {
              return ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true });
            });

            segments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));

            return this.$q.when(segments);
          }
        }, {
          key: 'removeAttribute',
          value: function removeAttribute(part) {
            this.attributes = _.filter(this.attributes, function (item) {
              return item !== part;
            });
            this.panelCtrl.refresh();
          }
        }, {
          key: 'attributeAction',
          value: function attributeAction() {
            // if value is not empty, add new attribute segment
            if (!this.isValueEmpty(this.attributeSegment.value)) {
              this.attributes.push(this.uiSegmentSrv.newSegment({ value: this.attributeSegment.value, expandable: true }));
              this.targetChanged();
            }

            // reset the + button
            var plusButton = this.uiSegmentSrv.newPlusButton();
            this.attributeSegment.value = plusButton.value;
            this.attributeSegment.html = plusButton.html;
            this.panelCtrl.refresh();
          }
        }, {
          key: 'attributeValueChanged',
          value: function attributeValueChanged(segment, index) {
            this.attributes[index].value = segment.value;
            if (this.isValueEmpty(segment.value)) {
              this.attributes.splice(index, 1);
            }
            this.targetChanged();
            this.panelCtrl.refresh();
          }
        }, {
          key: 'getAttributeSegments',
          value: function getAttributeSegments() {
            var ctrl = this;
            var segments = [];

            _.forOwn(ctrl.availableAttributes, function (val, key) {
              segments.push(ctrl.uiSegmentSrv.newSegment({ value: key, expandable: true }));
            });
            segments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));

            return this.$q.when(segments);
          }
        }, {
          key: 'getElementSegments',
          value: function getElementSegments(index) {
            var ctrl = this;
            var query = { path: this.getSegmentPathUpTo(index) };

            return this.datasource.metricFindQuery(angular.toJson(query)).then(function (items) {
              var altSegments = _.map(items, function (item) {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
              });

              if (altSegments.length === 0) {
                return altSegments;
              }

              // add template variables
              _.each(ctrl.templateSrv.variables, function (variable) {
                altSegments.unshift(ctrl.uiSegmentSrv.newSegment({
                  type: 'template',
                  value: '$' + variable.name,
                  expandable: true
                }));
              });

              altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));

              // add wildcard option
              // altSegments.unshift(ctrl.uiSegmentSrv.newSegment('*'))
              return altSegments;
            }).catch(function (err) {
              ctrl.error = err.message || 'Failed to issue metric query';
              return [];
            });
          }
        }, {
          key: 'segmentValueChanged',
          value: function segmentValueChanged(segment, segmentIndex) {
            var ctrl = this;
            ctrl.error = null;

            if (ctrl.isValueEmpty(segment.value)) {
              ctrl.segments.length = segmentIndex;
              ctrl.checkOtherSegments(segmentIndex);
            }

            if (segment.expandable) {
              ctrl.checkOtherSegments(segmentIndex + 1);
              ctrl.setSegmentFocus(segmentIndex + 1);
              ctrl.targetChanged();
            } else {
              ctrl.segments = ctrl.segments.splice(0, segmentIndex + 1);
            }

            ctrl.setSegmentFocus(segmentIndex + 1);
            ctrl.checkAttributeSegments();
            ctrl.targetChanged();
          }
        }]);

        return PiWebApiDatasourceQueryCtrl;
      }(QueryCtrl));

      _export('PiWebApiDatasourceQueryCtrl', PiWebApiDatasourceQueryCtrl);

      PiWebApiDatasourceQueryCtrl.templateUrl = 'partials/query.editor.html';
    }
  };
});
//# sourceMappingURL=query_ctrl.js.map
