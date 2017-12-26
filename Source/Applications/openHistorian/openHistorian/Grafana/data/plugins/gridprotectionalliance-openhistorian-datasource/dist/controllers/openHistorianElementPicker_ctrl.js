System.register(['./../js/openHistorianConstants', 'lodash'], function(exports_1) {
    var openHistorianConstants_1, lodash_1;
    var OpenHistorianElementPickerCtrl;
    return {
        setters:[
            function (openHistorianConstants_1_1) {
                openHistorianConstants_1 = openHistorianConstants_1_1;
            },
            function (lodash_1_1) {
                lodash_1 = lodash_1_1;
            }],
        execute: function() {
            OpenHistorianElementPickerCtrl = (function () {
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
                        var altSegments = lodash_1.default.map(data, function (item) {
                            return ctrl.uiSegmentSrv.newSegment({ value: item.text, expandable: item.expandable });
                        });
                        altSegments.sort(function (a, b) {
                            if (a.value < b.value)
                                return -1;
                            if (a.value > b.value)
                                return 1;
                            return 0;
                        });
                        lodash_1.default.each(ctrl.$scope.datasource.templateSrv.variables, function (item, index, list) {
                            if (item.type == "query")
                                altSegments.unshift(ctrl.uiSegmentSrv.newCondition('$' + item.name));
                        });
                        if (!newSegment)
                            altSegments.unshift(ctrl.uiSegmentSrv.newSegment('-REMOVE-'));
                        return lodash_1.default.filter(altSegments, function (segment) {
                            return lodash_1.default.find(ctrl.segments, function (x) {
                                return x.value == segment.value;
                            }) == undefined;
                        });
                    });
                };
                OpenHistorianElementPickerCtrl.prototype.addElementSegment = function () {
                    // if value is not empty, add new attribute segment
                    if (event.target['text'] != null) {
                        this.segments.push(this.uiSegmentSrv.newSegment({ value: event.target['text'], expandable: true }));
                        this.setTargetWithElements();
                    }
                    // reset the + button
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
                    lodash_1.default.each(ctrl.functions, function (element, index, list) {
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
                    lodash_1.default.each(Object.keys(openHistorianConstants_1.FunctionList), function (element, index, list) {
                        array.push(ctrl.uiSegmentSrv.newSegment(element));
                    });
                    if (this.functions.length == 0)
                        array = array.slice(2, array.length);
                    array.sort(function (a, b) {
                        var nameA = a.value.toUpperCase(); // ignore upper and lowercase
                        var nameB = b.value.toUpperCase(); // ignore upper and lowercase
                        if (nameA < nameB) {
                            return -1;
                        }
                        if (nameA > nameB) {
                            return 1;
                        }
                        // names must be equal
                        return 0;
                    });
                    return Promise.resolve(lodash_1.default.filter(array, function (segment) {
                        return lodash_1.default.find(_this.functions, function (x) {
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
                        var fi = lodash_1.default.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
                        if (func.Function == 'Slice')
                            this.functionSegments[fi + 1].Parameters = this.functionSegments[fi + 1].Parameters.slice(1, this.functionSegments[fi + 1].Parameters.length);
                        else if (fi > 0 && (this.functionSegments[fi - 1].Function == 'Set' || this.functionSegments[fi - 1].Function == 'Slice')) {
                            --fi;
                            ++l;
                        }
                        this.functionSegments.splice(fi, l);
                    }
                    else if (func.Type != 'Function') {
                        var fi = lodash_1.default.findIndex(this.functionSegments, function (segment) { return segment.Function == func.Function; });
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
                    // reset the + button
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
                    lodash_1.default.each(ctrl.functionSegments, function (element, index, list) {
                        var newElement = ctrl.uiSegmentSrv.newSegment(element.Function);
                        newElement.Type = 'Function';
                        newElement.Function = element.Function;
                        ctrl.functions.push(newElement);
                        if (newElement.value == 'Set' || newElement.value == 'Slice')
                            return;
                        var operator = ctrl.uiSegmentSrv.newOperator('(');
                        operator.Type = 'Operator';
                        ctrl.functions.push(operator);
                        lodash_1.default.each(element.Parameters, function (param, i, j) {
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
                    this.typingTimer = setTimeout(function () { ctrl.functionValueChanged(func, index); }, 3000);
                    event.target['focus']();
                };
                return OpenHistorianElementPickerCtrl;
            })();
            exports_1("OpenHistorianElementPickerCtrl", OpenHistorianElementPickerCtrl);
        }
    }
});
//# sourceMappingURL=openHistorianElementPicker_ctrl.js.map