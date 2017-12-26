System.register(["lodash", './../js/openHistorianConstants'], function(exports_1) {
    var lodash_1, openHistorianConstants_1;
    var OpenHistorianQueryOptionsCtrl;
    return {
        setters:[
            function (lodash_1_1) {
                lodash_1 = lodash_1_1;
            },
            function (openHistorianConstants_1_1) {
                openHistorianConstants_1 = openHistorianConstants_1_1;
            }],
        execute: function() {
            OpenHistorianQueryOptionsCtrl = (function () {
                // #endregion
                function OpenHistorianQueryOptionsCtrl($scope, $compile) {
                    var _this = this;
                    this.$scope = $scope;
                    this.$compile = $compile;
                    this.$scope = $scope;
                    var value = JSON.parse(JSON.stringify($scope.return));
                    this.dataFlags = this.hex2flags(parseInt(value.Excluded));
                    this.dataFlags['Normal'].Value = value.Normal;
                    this.return = $scope.return;
                    this.flagArray = lodash_1.default.map(Object.keys(this.dataFlags), function (a) {
                        return { key: a, order: _this.dataFlags[a].Order };
                    }).sort(function (a, b) {
                        return a.order - b.order;
                    });
                }
                // #region Methods
                OpenHistorianQueryOptionsCtrl.prototype.calculateFlags = function (flag) {
                    var ctrl = this;
                    var flagVarExcluded = ctrl.return.Excluded;
                    if (flag == 'Select All') {
                        lodash_1.default.each(Object.keys(ctrl.dataFlags), function (key, index, list) {
                            if (key == "Normal")
                                ctrl.dataFlags[key].Value = false;
                            else
                                ctrl.dataFlags[key].Value = ctrl.dataFlags['Select All'].Value;
                        });
                        if (ctrl.dataFlags['Select All'].Value)
                            flagVarExcluded = 0xFFFFFFFF;
                        else
                            flagVarExcluded = 0;
                    }
                    else {
                        ctrl.dataFlags['Select All'].Value = false;
                        flagVarExcluded ^= ctrl.dataFlags[flag].Flag;
                    }
                    ctrl.return.Excluded = flagVarExcluded;
                    ctrl.return.Normal = ctrl.dataFlags['Normal'].Value;
                };
                OpenHistorianQueryOptionsCtrl.prototype.hex2flags = function (hex) {
                    var ctrl = this;
                    var flag = hex;
                    var flags = JSON.parse(JSON.stringify(openHistorianConstants_1.DefaultFlags));
                    lodash_1.default.each(Object.keys(flags), function (key, index, list) {
                        if (key == 'Select All')
                            return;
                        flags[key].Value = (flags[key].Flag & flag) != 0;
                    });
                    return flags;
                };
                return OpenHistorianQueryOptionsCtrl;
            })();
            exports_1("OpenHistorianQueryOptionsCtrl", OpenHistorianQueryOptionsCtrl);
        }
    }
});
//# sourceMappingURL=openHistorianQueryOptions_ctrl.js.map