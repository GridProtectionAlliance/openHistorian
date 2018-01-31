//******************************************************************************************************
//  config_ctrl.js - Gbtc
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
//  10/30/2017 - Billy Ernest
//       Generated original version of source code.
//
//******************************************************************************************************

import _ from "lodash";
import { DefaultFlags } from './../js/openHistorianConstants'

export class OpenHistorianConfigCtrl{
    static templateUrl:string = 'partial/config.html';
    current: any;
    flagArray: Array<any>;
    constructor($scope) {
        var ctrl = this;

        ctrl.current.jsonData.Excluded = this.current.jsonData.Excluded || 0;
        ctrl.current.jsonData.Normal = this.current.jsonData.Normal || false;

    }
}

