//******************************************************************************************************
//  module.js - Gbtc
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
//  11/02/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************

import { OpenHistorianDataSource } from './openHistorianDatasource'
import { OpenHistorianDataSourceQueryCtrl } from './controllers/openHistorianQuery_ctrl'
import { OpenHistorianConfigCtrl } from './controllers/openHistorianConfig_ctrl'
import { OpenHistorianQueryOptionsCtrl } from './controllers/openHistorianQueryOptions_ctrl'
import { OpenHistorianAnnotationsQueryCtrl } from './controllers/openHistorianAnnotations_ctrl'
import { OpenHistorianElementPickerCtrl } from './controllers/openHistorianElementPicker_ctrl'
import { OpenHistorianTextEditorCtrl } from './controllers/openHistorianTextEditor_ctrl'
import { OpenHistorianFilterExpressionCtrl } from './controllers/openHistorianFilterExpression_ctrl'

import angular from "angular"

export {
    OpenHistorianDataSource as Datasource,
    OpenHistorianDataSourceQueryCtrl as QueryCtrl,
    OpenHistorianConfigCtrl as ConfigCtrl,
    OpenHistorianQueryOptionsCtrl as QueryOptionsCtrl,
    OpenHistorianAnnotationsQueryCtrl as AnnotationsQueryCtrl
}

angular.module('grafana.directives').directive("queryOptions", function() {
  return {
    templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.options.html',
    restrict: 'E',
    controller: OpenHistorianQueryOptionsCtrl,
    controllerAs: 'queryOptionCtrl',
    scope: {
      flags: "=",
      return: "=",
    }
  };
});

angular.module('grafana.directives').directive("elementPicker", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.elementPicker.html',
        restrict: 'E',
        controller: OpenHistorianElementPickerCtrl,
        controllerAs: 'openHistorianElementPickerCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});

angular.module('grafana.directives').directive("textEditor", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.textEditor.html',
        restrict: 'E',
        controller: OpenHistorianTextEditorCtrl,
        controllerAs: 'openHistorianTextEditorCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});

angular.module('grafana.directives').directive("filterExpression", function () {
    return {
        templateUrl: 'public/plugins/gridprotectionalliance-openhistorian-datasource/partial/query.filterExpression.html',
        restrict: 'E',
        controller: OpenHistorianFilterExpressionCtrl,
        controllerAs: 'openHistorianFilterExpressionCtrl',
        scope: {
            target: "=",
            datasource: "=",
            panel: "="
        }
    };
});