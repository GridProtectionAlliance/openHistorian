import angular from 'angular'
import _ from 'lodash'
import { QueryCtrl } from 'app/plugins/sdk'
import './css/query-editor.css!'

export class PiWebApiDatasourceQueryCtrl extends QueryCtrl {

  constructor ($scope, $injector, uiSegmentSrv, templateSrv, $q) {
    super($scope, $injector)
    this.uiSegmentSrv = uiSegmentSrv
    this.templateSrv = templateSrv
    this.$q = $q
    this.segments = []
    this.attributes = []
    this.availableAttributes = {}
    this.attributeSegment = this.uiSegmentSrv.newPlusButton()

    this.summaries = []
    this.summarySegment = this.uiSegmentSrv.newPlusButton()
    this.summaryTypes = [
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
    ]

    this.calculationBasisSegment = this.uiSegmentSrv.newSegment('EventWeighted')
    this.calculationBasis = [
      'TimeWeighted', // Weight the values in the calculation by the time over which they apply. Interpolation is based on whether the attribute is stepped. Interpolated events are generated at the boundaries if necessary.
      'EventWeighted', // Evaluate values with equal weighting for each event. No interpolation is done. There must be at least one event within the time range to perform a successful calculation. Two events are required for standard deviation. In handling events at the boundary of the calculation, the AFSDK uses following rules:
      'TimeWeightedContinuous', // Apply weighting as in TimeWeighted, but do all interpolation between values as if they represent continuous data, (standard interpolation) regardless of whether the attribute is stepped.
      'TimeWeightedDiscrete', // Apply weighting as in TimeWeighted but interpolation between values is performed as if they represent discrete, unrelated values (stair step plot) regardless of the attribute is stepped.
      'EventWeightedExcludeMostRecentEvent', // The calculation behaves the same as _EventWeighted_, except in the handling of events at the boundary of summary intervals in a multiple intervals calculation. Use this option to prevent events at the intervals boundary from being double count at both intervals. With this option, events at the end time (most recent time) of an interval is not used in that interval.
      'EventWeightedExcludeEarliestEvent', // Similar to the option _EventWeightedExcludeMostRecentEvent_. Events at the start time(earliest time) of an interval is not used in that interval.
      'EventWeightedIncludeBothEnds' // Events at both ends of the interval boundaries are included in the event weighted calculation.
    ]

    this.noDataReplacementSegment = this.uiSegmentSrv.newSegment('Null')
    this.noDataReplacement = [
      'Null', // replace with nulls
      'Drop', // drop items
      'Previous', // use previous value if available
      '0', // replace with 0      
    ]


    this.target.summary = this.target.summary || { types: [], basis: 'EventWeighted', interval: '5m', nodata: 'Null' }
    this.target.summary.types = this.target.summary.types || []
    this.target.summary.basis = this.target.summary.basis || 'EventWeighted'
    this.target.summary.nodata = this.target.summary.nodata || 'Null'
    this.target.summary.interval = this.target.summary.interval || '5m'


    this.target.target = this.target.target || ';'

    this.target.interpolate = this.target.interpolate || {enable: false}
    if (this.target.interpolate === false || this.target.interpolate === true) {
      this.target.interpolate = { enable: this.target.interpolate }
    }
    this.target.interpolate.enable = this.target.interpolate.enable || false

    if (this.segments.length === 0) {
      this.segments.push(this.uiSegmentSrv.newSelectMetric())
    }

    this.textEditorChanged()
  }


  /**
   * Queries PI Web API for child elements and attributes when the query segments or options are changed.
   *  
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  targetChanged () {
    if (this.error) { return }

    var ctrl = this

    var oldTarget = this.target.target
    var oldElement = this.target.target.split(';')[0]
    var element = this.getSegmentPathUpTo(this.segments.length)
    var attributes = this.getAttributes()
    var target = element + ';' + attributes
    this.target.target = target // _.reduce(this.functions, this.wrapFunction, target)
    this.target.elementPath = element
    this.target.attributes = attributes.split(';')
    this.target.summary.types = this.getSummaries().split(',')

    this.target.webids = _.map(ctrl.target.attributes, attrib => { return ctrl.availableAttributes[attrib] })

    if (element !== oldElement || this.target.webid === undefined) {
      var segmentValue = this.segments[this.segments.length - 1].value
      if (!segmentValue.startsWith('Select AF')) {
        this.panelCtrl.refresh()
      }

      this.datasource.getElement(element).then(results => {
        this.target.webid = results.WebId
      })
    }
  }

  
  /**
   * Queries PI Web API for child elements and attributes when the query text editor is changed.
   * 
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  textEditorChanged () {
    var ctrl = this
    var splitAttributes = ctrl.target.target.split(';')
    var splitElements = splitAttributes[0].split('\\')

    // remove element hierarchy from attribute collection
    splitAttributes.splice(0, 1)

    var segments = []
    var attributes = []

    _.each(splitElements, function (item) {
      segments.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }))
    })

    _.each(splitAttributes, function (item) {
      attributes.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }))
    })

    ctrl.segments = segments
    ctrl.checkOtherSegments(segments.length - 1)

    ctrl.attributes = attributes
    ctrl.checkAttributeSegments().then(r => {
      ctrl.targetChanged()
      ctrl.refresh()
    })

    var summaries = []
    _.each(ctrl.target.summary.types, item => {
      if (item) {
        summaries.push(ctrl.uiSegmentSrv.newSegment({ value: item, expandable: true }))
      }
    })
    ctrl.summaries = summaries
  }

  /**
   * Toggle between segment queries and text editor queries.
   * 
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  toggleEditorMode () {
    this.target.textEditor = !this.target.textEditor
  }

  /**
   * Gets the segment information and parses it to a string.
   * 
   * @param {any} index - Last index of segment to use.
   * @returns - AF Path or PI Point name.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  getSegmentPathUpTo (index) {
    var arr = this.segments.slice(0, index)

    return _.reduce(arr, function (result, segment) {
      if (!segment.value.startsWith('Select AF')) {
        return result ? (result + '\\' + segment.value) : segment.value
      }
      return result
    }, '')
  }

  /**
   * Gets the currently selected child attributes.
   * 
   * @returns - String of selected attributes.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  getAttributes () {
    var arr = this.attributes.slice(0, this.attributes.length)

    return _.reduce(arr, function (result, segment) {
      if (!segment.value.startsWith('Select AF')) {
        return result ? (result + ';' + segment.value) : segment.value
      }
      return result
    }, '')
  }

  /**
   * Gets the currently selected summaries.
   * 
   * @returns - Selected summaries as a string.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  getSummaries () {
    var arr = this.summaries.slice(0, this.summaries.length)

    return _.reduce(arr, function (result, segment) {
      if (segment && !segment.value.startsWith('Select AF')) {
        return result ? (result + ',' + segment.value) : segment.value
      }
      return result
    }, '')
  }

  /**
   * Used in converting a query to a string.
   * 
   * @returns - Query string.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  getCollapsedText () {
    return '[' + (this.target.display || '') + ']: ' + this.target.target
  }

  /**
   * Detects if a metric segment has children.
   * 
   * @param {any} element - The metric segment to check.
   * @returns - True if has children or is a server or database, otherwise false.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  isElementSegmentExpandable (element) {
    return element.HasChildren === undefined || element.HasChildren === true || element.Path.split('\\').length <= 3
  }

  /**
   * Get the current AF Element's child attributes. Validates when the element selection changes.
   * 
   * @returns - Collection of attributes.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  checkAttributeSegments () {
    var ctrl = this
    var query = {
      path: this.getSegmentPathUpTo(this.segments.length),
      type: 'attributes'
    }

    return this.datasource.metricFindQuery(angular.toJson(query))
    .then(attributes => {
      var validAttributes = {}

      _.each(attributes, attribute => {
        validAttributes[attribute.Path.substring(attribute.Path.indexOf('|') + 1)] = attribute.WebId
      })

      var filteredAttributes = _.filter(ctrl.attributes, attrib => {
        return validAttributes[attrib.value] !== undefined
      })

      ctrl.availableAttributes = validAttributes
      ctrl.attributes = filteredAttributes
    })
    .catch(err => {
      ctrl.error = err.message || 'Failed to issue metric query'
      ctrl.attributes = []
    })
  }

  /**
   * When changing a metric (AF Element) in the query, validates all the children paths.
   * If the children are different, queries the server for new children and removes any elements after the selected one.
   * Also validates attributes.
   * 
   * @param {any} fromIndex
   * @returns
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  checkOtherSegments (fromIndex) {
    var ctrl = this
    var query = { path: ctrl.getSegmentPathUpTo(fromIndex + 1) }
    
    if (ctrl.segments.length === 0) {
      ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Database"))
    }

    return ctrl.datasource.metricFindQuery(angular.toJson(query)).then(children => {
      if (children.length === 0) {
        if (query.path !== '') {
          ctrl.segments = ctrl.segments.splice(0, fromIndex + 1)
          if (ctrl.segments[ctrl.segments.length - 1].expandable) {
            ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Database"))
          }
        }
      } else /* if (this.isElementSegmentExpandable(segments[0])) */ {
        if (ctrl.segments.length === fromIndex) {
          ctrl.segments = ctrl.segments.splice(0, fromIndex)
          if (ctrl.segments[ctrl.segments.length - 1].expandable) {
            ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Element"))
          }
        } else {
          return ctrl.checkOtherSegments(fromIndex + 1)
        }
      }
    }).catch(err => {
      ctrl.segments = ctrl.segments.splice(0, fromIndex)
      if (ctrl.segments[ctrl.segments.length - 1].expandable) {
        ctrl.segments.push(ctrl.uiSegmentSrv.getSegmentForValue(null, "Select AF Element"))
      }
      ctrl.error = err.message || 'Failed to issue metric query'
    })
  }

  /**
   * Focus the selected segment.
   * 
   * @param {any} segmentIndex - The currently selected metric.
   * 
   * @memberOf PiWebApiDatasourceQueryCtrl
   */
  setSegmentFocus (segmentIndex) {
    _.each(this.segments, (segment, index) => {
      segment.focus = false
      segment.focus = segmentIndex === index
    })
  }
  // wrap a function for grafana
  wrapFunction (target, func) {
    return func.render(target)
  }
  // is selected segment empty
  isValueEmpty (value) {
    return value === undefined || value === null || value === '' || value === '-REMOVE-'
  }
  // get summary calculation basis
  calcBasisValueChanged (segment, index) {
    this.target.summary.basis = this.calculationBasisSegment.value
    this.targetChanged()
    this.panelCtrl.refresh()
  }

 calcNoDataValueChanged (segment, index) {
    this.target.summary.nodata = this.noDataReplacementSegment.value
    this.targetChanged()
    this.panelCtrl.refresh()
  }

  // get summary calculation basis user interface segments
  getCalcBasisSegments () {
    var ctrl = this
    var segments = _.map(this.calculationBasis, item => {
      return ctrl.uiSegmentSrv.newSegment({value: item, expandable: true})
    })
    return this.$q.when(segments)
  }

 // get summary calculation basis user interface segments
  getNoDataSegments () {
    var ctrl = this
    var segments = _.map(this.noDataReplacement, item => {
      return ctrl.uiSegmentSrv.newSegment({value: item, expandable: true})
    })
    return this.$q.when(segments)
  }

  // remove a summary from the user interface and the query
  removeSummary (part) {
    this.summaries = _.filter(this.summaries, function (item) { return item !== part })
    this.panelCtrl.refresh()
  }
  // add a new summary to the query
  summaryAction () {
    // if value is not empty, add new attribute segment
    if (!this.isValueEmpty(this.summarySegment.value)) {
      this.summaries.push(this.uiSegmentSrv.newSegment({value: this.summarySegment.value, expandable: true}))
      this.targetChanged()
    }

    // reset the + button
    var plusButton = this.uiSegmentSrv.newPlusButton()
    this.summarySegment.value = plusButton.value
    this.summarySegment.html = plusButton.html
    this.panelCtrl.refresh()
  }
  // change a summary query
  summaryValueChanged (segment, index) {
    this.summaries[index].value = segment.value
    if (this.isValueEmpty(segment.value)) {
      this.summaries.splice(index, 1)
    }
    this.targetChanged()
    this.panelCtrl.refresh()
  }
  // get the list of summaries available
  getSummarySegments () {
    var ctrl = this
    // var segments = _.map(_.difference(ctrl.summaryTypes, _.map(ctrl.summaries, seg => { return seg.value || '' })), item => {
    var segments = _.map(ctrl.summaryTypes, item => {
      return ctrl.uiSegmentSrv.newSegment({value: item, expandable: true})
    })

    segments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'))

    return this.$q.when(segments)
  }
  // remove an af attribute from the query
  removeAttribute (part) {
    this.attributes = _.filter(this.attributes, function (item) { return item !== part })
    this.panelCtrl.refresh()
  }
  // add an attribute to the query
  attributeAction () {
    // if value is not empty, add new attribute segment
    if (!this.isValueEmpty(this.attributeSegment.value)) {
      this.attributes.push(this.uiSegmentSrv.newSegment({value: this.attributeSegment.value, expandable: true}))
      this.targetChanged()
    }

    // reset the + button
    var plusButton = this.uiSegmentSrv.newPlusButton()
    this.attributeSegment.value = plusButton.value
    this.attributeSegment.html = plusButton.html
    this.panelCtrl.refresh()
  }
  // change an attribute
  attributeValueChanged (segment, index) {
    this.attributes[index].value = segment.value
    if (this.isValueEmpty(segment.value)) {
      this.attributes.splice(index, 1)
    }
    this.targetChanged()
    this.panelCtrl.refresh()
  }
  // get the list of attributes for the user interface
  getAttributeSegments () {
    var ctrl = this
    var segments = []

    _.forOwn(ctrl.availableAttributes, (val, key) => {
      segments.push(ctrl.uiSegmentSrv.newSegment({value: key, expandable: true}))
    })
    segments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'))

    return this.$q.when(segments)
  }
  // get a ui segment for the attributes
  getElementSegments (index) {
    var ctrl = this
    var query = { path: this.getSegmentPathUpTo(index) }

    return this.datasource.metricFindQuery(angular.toJson(query))
    .then(items => {
      var altSegments = _.map(items, item => {
        return ctrl.uiSegmentSrv.newSegment({value: item.text, expandable: item.expandable})
      })

      if (altSegments.length === 0) { return altSegments }
     
      // add template variables
      _.each(ctrl.templateSrv.variables, variable => {
        altSegments.unshift(ctrl.uiSegmentSrv.newSegment({
          type: 'template',
          value: '$' + variable.name,
          expandable: true
        }))
      })
      
      altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'))

      // add wildcard option
      // altSegments.unshift(ctrl.uiSegmentSrv.newSegment('*'))
      return altSegments
    }).catch(err => {
      ctrl.error = err.message || 'Failed to issue metric query'
      return []
    })
  }
  // changes the selecte af element segment
  segmentValueChanged (segment, segmentIndex) {
    var ctrl = this;
    ctrl.error = null

    if (ctrl.isValueEmpty(segment.value)) {
      ctrl.segments.length = segmentIndex;
      ctrl.checkOtherSegments(segmentIndex);
    }

    if (segment.expandable) {
      ctrl.checkOtherSegments(segmentIndex + 1)
      ctrl.setSegmentFocus(segmentIndex + 1)
      ctrl.targetChanged()
    } else {
      ctrl.segments = ctrl.segments.splice(0, segmentIndex + 1)
    }

    ctrl.setSegmentFocus(segmentIndex + 1)
    ctrl.checkAttributeSegments()
    ctrl.targetChanged()
  }
}

PiWebApiDatasourceQueryCtrl.templateUrl = 'partials/query.editor.html'
