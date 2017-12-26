System.register(['./../js/openHistorianConstants', 'lodash', 'jquery'], function(exports_1) {
    var openHistorianConstants_1, lodash_1, jquery_1;
    var OpenHistorianFilterExpressionCtrl;
    return {
        setters:[
            function (openHistorianConstants_1_1) {
                openHistorianConstants_1 = openHistorianConstants_1_1;
            },
            function (lodash_1_1) {
                lodash_1 = lodash_1_1;
            },
            function (jquery_1_1) {
                jquery_1 = jquery_1_1;
            }],
        execute: function() {
            OpenHistorianFilterExpressionCtrl = (function () {
                function OpenHistorianFilterExpressionCtrl($scope, uiSegmentSrv) {
                    this.$scope = $scope;
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
                    lodash_1.default.each(this.wheres, function (element, index, list) {
                        where += element.value + ' ';
                    });
                    var orderby = '';
                    lodash_1.default.each(this.orderBys, function (element, index, list) {
                        orderby += (index == 0 ? 'ORDER BY ' : '') + element.value + (element.type == 'condition' && index < list.length - 1 ? ',' : '') + ' ';
                    });
                    var query = 'FILTER ' + topn + filter + where + orderby;
                    var functions = '';
                    lodash_1.default.each(this.functions, function (element, index, list) {
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
                // #region TOP N
                OpenHistorianFilterExpressionCtrl.prototype.topNValueChanged = function () {
                    var ctrl = this;
                    clearTimeout(ctrl.typingTimer);
                    ctrl.typingTimer = setTimeout(function () { ctrl.setTargetWithQuery(); }, 1000);
                    event.target['focus']();
                };
                // #endregion
                // #region Wheres
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
                            var altSegments = lodash_1.default.map(data, function (item) {
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
                    else if (where.type == 'value' && !jquery_1.default.isNumeric(where.value) && where.value.toUpperCase() != 'NULL')
                        this.wheres[index] = this.uiSegmentSrv.newSegment("'" + where.value + "'");
                    this.setTargetWithQuery();
                };
                OpenHistorianFilterExpressionCtrl.prototype.getWheresToAddNew = function () {
                    var ctrl = this;
                    return this.datasource.whereFindQuery(ctrl.filterSegment.value).then(function (data) {
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
                        return altSegments;
                    });
                };
                OpenHistorianFilterExpressionCtrl.prototype.addWhere = function () {
                    if (this.wheres.length > 0)
                        this.wheres.push(this.uiSegmentSrv.newCondition('AND'));
                    this.wheres.push(this.uiSegmentSrv.newSegment(event.target['text']));
                    this.wheres.push(this.uiSegmentSrv.newOperator('NOT LIKE'));
                    this.wheres.push(this.uiSegmentSrv.newFake("''", 'value', 'query-segment-value'));
                    // reset the + button
                    var plusButton = this.uiSegmentSrv.newPlusButton();
                    this.whereSegment.value = plusButton.value;
                    this.whereSegment.html = plusButton.html;
                    this.setTargetWithQuery();
                };
                // #endregion
                // #region Filters
                OpenHistorianFilterExpressionCtrl.prototype.getFilterToEdit = function () {
                    var ctrl = this;
                    return this.datasource.filterFindQuery().then(function (data) {
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
                        return altSegments;
                    });
                };
                OpenHistorianFilterExpressionCtrl.prototype.filterValueChanged = function () {
                    //this.target.policy = this.topNSegment;
                    this.orderBySegment = this.uiSegmentSrv.newPlusButton();
                    this.wheres = [];
                    this.setTargetWithQuery();
                    this.panelCtrl.refresh();
                };
                // #endregion
                // #region OrderBys
                OpenHistorianFilterExpressionCtrl.prototype.getOrderBysToAddNew = function () {
                    var ctrl = this;
                    return this.datasource.orderByFindQuery(ctrl.filterSegment.value).then(function (data) {
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
                        return lodash_1.default.filter(altSegments, function (segment) {
                            return lodash_1.default.find(ctrl.orderBys, function (x) {
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
                        if (orderBy.type !== 'plus-button')
                            altSegments.unshift(_this.uiSegmentSrv.newSegment('-REMOVE-'));
                        return lodash_1.default.filter(altSegments, function (segment) {
                            return lodash_1.default.find(ctrl.orderBys, function (x) {
                                return x.value == segment.value;
                            }) == undefined;
                        });
                    });
                };
                OpenHistorianFilterExpressionCtrl.prototype.addOrderBy = function () {
                    this.orderBys.push(this.uiSegmentSrv.newSegment(event.target['text']));
                    this.orderBys.push(this.uiSegmentSrv.newCondition('ASC'));
                    // reset the + button
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
                // #endregion
                // #region Functions
                OpenHistorianFilterExpressionCtrl.prototype.getFunctionsToAddNew = function () {
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
                    this.setTargetWithQuery();
                };
                OpenHistorianFilterExpressionCtrl.prototype.addFunctionSegment = function () {
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
                    this.setTargetWithQuery();
                };
                OpenHistorianFilterExpressionCtrl.prototype.buildFunctionArray = function () {
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
                    this.typingTimer = setTimeout(function () { ctrl.functionValueChanged(func, index); }, 3000);
                    event.target['focus']();
                };
                return OpenHistorianFilterExpressionCtrl;
            })();
            exports_1("OpenHistorianFilterExpressionCtrl", OpenHistorianFilterExpressionCtrl);
        }
    }
});
//# sourceMappingURL=openHistorianFilterExpression_ctrl.js.map