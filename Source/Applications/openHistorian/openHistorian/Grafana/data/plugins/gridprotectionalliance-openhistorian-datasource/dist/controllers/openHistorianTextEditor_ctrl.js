//******************************************************************************************************
//  openHistorianTextEditor.ts - Gbtc
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
System.register([], function(exports_1) {
    var OpenHistorianTextEditorCtrl;
    return {
        setters:[],
        execute: function() {
            OpenHistorianTextEditorCtrl = (function () {
                function OpenHistorianTextEditorCtrl($scope, templateSrv) {
                    this.$scope = $scope;
                    this.templateSrv = templateSrv;
                    this.$scope = $scope;
                    this.targetText = ($scope.target.targetText == undefined ? '' : $scope.target.targetText);
                    this.setTargetWithText();
                    delete $scope.target.segments;
                    delete $scope.target.functionSegments;
                    delete $scope.target.wheres;
                    delete $scope.target.topNSegment;
                    delete $scope.target.functions;
                    delete $scope.target.orderBys;
                    delete $scope.target.whereSegment;
                    delete $scope.target.filterSegment;
                    delete $scope.target.orderBySegment;
                    delete $scope.target.functionSegment;
                }
                OpenHistorianTextEditorCtrl.prototype.setTargetWithText = function () {
                    this.$scope.target.targetText = this.targetText;
                    this.$scope.target.target = this.targetText;
                    this.$scope.target.queryType = 'Text Editor';
                    this.$scope.panel.refresh(); // Asks the panel to refresh data.
                };
                return OpenHistorianTextEditorCtrl;
            })();
            exports_1("OpenHistorianTextEditorCtrl", OpenHistorianTextEditorCtrl);
        }
    }
});
//# sourceMappingURL=openHistorianTextEditor_ctrl.js.map