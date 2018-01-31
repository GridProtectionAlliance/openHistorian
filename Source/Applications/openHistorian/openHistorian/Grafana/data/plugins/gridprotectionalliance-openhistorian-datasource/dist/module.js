//******************************************************************************************************
//  module.js - Gbtc
//
//  Copyright � 2017, Grid Protection Alliance.  All Rights Reserved.
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
//  11/02/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************
System.register(['./openHistorianDatasource', './controllers/openHistorianQuery_ctrl', './controllers/openHistorianConfig_ctrl', './controllers/openHistorianQueryOptions_ctrl', './controllers/openHistorianAnnotations_ctrl', './controllers/openHistorianElementPicker_ctrl', './controllers/openHistorianTextEditor_ctrl', './controllers/openHistorianFilterExpression_ctrl', "angular"], function(exports_1) {
    var openHistorianDatasource_1, openHistorianQuery_ctrl_1, openHistorianConfig_ctrl_1, openHistorianQueryOptions_ctrl_1, openHistorianAnnotations_ctrl_1, openHistorianElementPicker_ctrl_1, openHistorianTextEditor_ctrl_1, openHistorianFilterExpression_ctrl_1, angular_1;
    return {
        setters:[
            function (openHistorianDatasource_1_1) {
                openHistorianDatasource_1 = openHistorianDatasource_1_1;
            },
            function (openHistorianQuery_ctrl_1_1) {
                openHistorianQuery_ctrl_1 = openHistorianQuery_ctrl_1_1;
            },
            function (openHistorianConfig_ctrl_1_1) {
                openHistorianConfig_ctrl_1 = openHistorianConfig_ctrl_1_1;
            },
            function (openHistorianQueryOptions_ctrl_1_1) {
                openHistorianQueryOptions_ctrl_1 = openHistorianQueryOptions_ctrl_1_1;
            },
            function (openHistorianAnnotations_ctrl_1_1) {
                openHistorianAnnotations_ctrl_1 = openHistorianAnnotations_ctrl_1_1;
            },
            function (openHistorianElementPicker_ctrl_1_1) {
                openHistorianElementPicker_ctrl_1 = openHistorianElementPicker_ctrl_1_1;
            },
            function (openHistorianTextEditor_ctrl_1_1) {
                openHistorianTextEditor_ctrl_1 = openHistorianTextEditor_ctrl_1_1;
            },
            function (openHistorianFilterExpression_ctrl_1_1) {
                openHistorianFilterExpression_ctrl_1 = openHistorianFilterExpression_ctrl_1_1;
            },
            function (angular_1_1) {
                angular_1 = angular_1_1;
            }],
        execute: function() {
            exports_1("Datasource", openHistorianDatasource_1.OpenHistorianDataSource);
            exports_1("QueryCtrl", openHistorianQuery_ctrl_1.OpenHistorianDataSourceQueryCtrl);
            exports_1("ConfigCtrl", openHistorianConfig_ctrl_1.OpenHistorianConfigCtrl);
            exports_1("QueryOptionsCtrl", openHistorianQueryOptions_ctrl_1.OpenHistorianQueryOptionsCtrl);
            exports_1("AnnotationsQueryCtrl", openHistorianAnnotations_ctrl_1.OpenHistorianAnnotationsQueryCtrl);
            angular_1.default.module('grafana.directives').directive("queryOptions", function () {
                return {
                    templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.options.html',
                    restrict: 'E',
                    controller: openHistorianQueryOptions_ctrl_1.OpenHistorianQueryOptionsCtrl,
                    controllerAs: 'queryOptionCtrl',
                    scope: {
                        flags: "=",
                        return: "=",
                    }
                };
            });
            angular_1.default.module('grafana.directives').directive("elementPicker", function () {
                return {
                    templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.elementPicker.html',
                    restrict: 'E',
                    controller: openHistorianElementPicker_ctrl_1.OpenHistorianElementPickerCtrl,
                    controllerAs: 'openHistorianElementPickerCtrl',
                    scope: {
                        target: "=",
                        datasource: "=",
                        panel: "="
                    }
                };
            });
            angular_1.default.module('grafana.directives').directive("textEditor", function () {
                return {
                    templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.textEditor.html',
                    restrict: 'E',
                    controller: openHistorianTextEditor_ctrl_1.OpenHistorianTextEditorCtrl,
                    controllerAs: 'openHistorianTextEditorCtrl',
                    scope: {
                        target: "=",
                        datasource: "=",
                        panel: "="
                    }
                };
            });
            angular_1.default.module('grafana.directives').directive("filterExpression", function () {
                return {
                    templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.filterExpression.html',
                    restrict: 'E',
                    controller: openHistorianFilterExpression_ctrl_1.OpenHistorianFilterExpressionCtrl,
                    controllerAs: 'openHistorianFilterExpressionCtrl',
                    scope: {
                        target: "=",
                        datasource: "=",
                        panel: "="
                    }
                };
            });
        }
    }
});
//# sourceMappingURL=module.js.map