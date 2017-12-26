//******************************************************************************************************
//  queryOptions_ctrl.js - Gbtc
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
//  10/31/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************
import _ from "lodash";
import { DefaultFlags } from './../js/openHistorianConstants'

export class OpenHistorianQueryOptionsCtrl{
     // #region Members

    dataFlags: any;
    return: any;
    flagArray: Array<any>;
    // #endregion

    constructor(private $scope,private $compile) {

        this.$scope = $scope;
        var value = JSON.parse(JSON.stringify($scope.return));

        this.dataFlags = this.hex2flags(parseInt(value.Excluded));
        this.dataFlags['Normal'].Value = value.Normal;

        this.return = $scope.return;

        this.flagArray = _.map(Object.keys(this.dataFlags), a => {
            return { key: a, order: this.dataFlags[a].Order };
        }).sort((a, b) => {
            return a.order - b.order;
        });

    }

    // #region Methods
    calculateFlags(flag) {
        var ctrl = this;
        var flagVarExcluded = ctrl.return.Excluded;

        if (flag == 'Select All') {
            _.each(Object.keys(ctrl.dataFlags), function (key, index, list) {

                if(key == "Normal") 
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
    }

    
    hex2flags(hex){
        var ctrl = this;
        var flag = hex;
        var flags = JSON.parse(JSON.stringify(DefaultFlags));

        _.each(Object.keys(flags), function (key, index, list) {
            if (key == 'Select All') return;
            
            flags[key].Value = (flags[key].Flag & flag) != 0
        });
        
        return flags;
    }

    // #endregion

}
