//******************************************************************************************************
//  openHistorianElementPicker_ctrl.ts - Gbtc
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
//  12/12/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************
import { FunctionList, Booleans, AngleUnits, TimeUnits, WhereOperators } from './../js/openHistorianConstants'
import _ from 'lodash'
import $ from 'jquery'

export class OpenHistorianElementPickerCtrl {

    elementSegment: any;
    segments: Array<any>;
    functionSegment: any;
    functionSegments: Array<any>;
    functions: Array<any>;
    typingTimer: any;


    constructor(private $scope, private uiSegmentSrv) {
        var ctrl = this;

        this.$scope = $scope;
        this.uiSegmentSrv = uiSegmentSrv;

        this.segments = (this.$scope.target.segments == undefined ? [] : this.$scope.target.segments.map(function (a) { return ctrl.uiSegmentSrv.newSegment({ value: a.text, expandable: true }) }));
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

    getElementSegments(newSegment) {
        var ctrl = this;
        var option = null;
        if (event.target['value'] != "") option = event.target['value'];

        return ctrl.$scope.datasource.metricFindQuery(option).then(data => {
            var altSegments = _.map(data, item => {
                return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable })
            });
            altSegments.sort((a, b) => {
                if (a.value < b.value)
                    return -1;
                if (a.value > b.value)
                    return 1;
                return 0;
            });

            _.each(ctrl.$scope.datasource.templateSrv.variables, (item, index, list) => {
                if (item.type == "query")
                    altSegments.unshift(ctrl.uiSegmentSrv.newCondition('$' + item.name));
            });

            if (!newSegment)
                altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));

            return _.filter(altSegments, segment => {
                return _.find(ctrl.segments, x => {
                    return x.value == segment.value
                }) == undefined;
            });
        });

    }

    addElementSegment() {
        // if value is not empty, add new attribute segment
        if (event.target['text'] != null) {
            this.segments.push(this.uiSegmentSrv.newSegment({ value: event.target['text'], expandable: true }))
            this.setTargetWithElements()
        }

        // reset the + button
        var plusButton = this.uiSegmentSrv.newPlusButton()
        this.elementSegment.value = plusButton.value
        this.elementSegment.html = plusButton.html
        this.$scope.panel.refresh()

    }

    segmentValueChanged(segment, index) {
        if (segment.value == "-REMOVE-") {
            this.segments.splice(index, 1);
        }
        else {
            this.segments[index] = segment;
        }

        this.setTargetWithElements()
    }

    setTargetWithElements() {
        var functions = '';
        var ctrl = this;
        _.each(ctrl.functions, function (element, index, list) {
            if (element.value == 'QUERY') functions += ctrl.segments.map(function (a) { return a.value }).join(';')
            else functions += element.value;
        });

        ctrl.$scope.target.target = (functions != "" ? functions : ctrl.segments.map(function (a) {
                return a.value
        }).join(';'));

        ctrl.$scope.target.functionSegments = ctrl.functionSegments;
        ctrl.$scope.target.segments = ctrl.segments;
        ctrl.$scope.target.queryType = 'Element List';
        this.$scope.panel.refresh()

    }

    getFunctionsToAddNew() {
        var ctrl = this;
        var array = []
        _.each(Object.keys(FunctionList), function (element, index, list) {
            array.push(ctrl.uiSegmentSrv.newSegment(element));
        });


        if (this.functions.length == 0) array = array.slice(2, array.length);

        array.sort(function(a, b) {
            var nameA = a.value.toUpperCase(); // ignore upper and lowercase
            var nameB = b.value.toUpperCase(); // ignore upper and lowercase
            if(nameA < nameB) {
                return -1;
            }
              if (nameA > nameB) {
                return 1;
            }

              // names must be equal
              return 0;        
        });

        return Promise.resolve(_.filter(array, segment => {
            return _.find(this.functions, x => {
                return x.value == segment.value;
            }) == undefined;
        }));
    }

    getFunctionsToEdit(func, index): any {
        var ctrl = this;
        var remove = [this.uiSegmentSrv.newSegment('-REMOVE-')];
        if (func.type == 'Operator') return Promise.resolve();
        else if (func.value == 'Set') return Promise.resolve(remove)

        return Promise.resolve(remove);
    }

    functionValueChanged(func, index) {
        var funcSeg = FunctionList[func.Function];

        if (func.value == "-REMOVE-") {
            var l = 1;
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function });
            if (func.Function == 'Slice')
                this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);
            else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                --fi;
                ++l;
            }

            this.functionSegments.splice(fi, l);
        }
        else if (func.Type != 'Function') {
            var fi = _.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function });
            this.functionSegments[fi].Parameters[func.Index].Default = func.value;
        }

        this.buildFunctionArray()

        this.setTargetWithElements()

    }

    addFunctionSegment() {
        var func = FunctionList[event.target['text']];

        if (func.Function == 'Slice') {
            this.functionSegments[0].Parameters.unshift(func.Parameters[0])
        }

        this.functionSegments.unshift(JSON.parse(JSON.stringify(func)));
        this.buildFunctionArray();

        // reset the + button
        var plusButton = this.uiSegmentSrv.newPlusButton()
        this.functionSegment.value = plusButton.value
        this.functionSegment.html = plusButton.html

        this.setTargetWithElements()
    }

    buildFunctionArray() {
        var ctrl = this;
        ctrl.functions = [];

        if (this.functionSegments.length == 0) return;

        _.each(ctrl.functionSegments, function (element, index, list) {
            var newElement = ctrl.uiSegmentSrv.newSegment(element.Function)
            newElement.Type = 'Function';
            newElement.Function = element.Function;

            ctrl.functions.push(newElement)

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
            })

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

    getBooleans() {
        return Promise.resolve(Booleans.map(value => { return this.uiSegmentSrv.newSegment(value) }));
    }

    getAngleUnits() {
        return Promise.resolve(AngleUnits.map(value => { return this.uiSegmentSrv.newSegment(value) }));
    }

    getTimeSelect() {
        return Promise.resolve(TimeUnits.map(value => { return this.uiSegmentSrv.newSegment(value) }));
    }

    inputChange(func, index) {
        var ctrl = this;
        clearTimeout(this.typingTimer);
        this.typingTimer = setTimeout(function () { ctrl.functionValueChanged(func, index) }, 3000);
        event.target['focus']();

    }

}