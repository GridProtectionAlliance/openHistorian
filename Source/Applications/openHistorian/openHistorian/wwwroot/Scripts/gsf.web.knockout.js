//******************************************************************************************************
//  gsf.web.knockout.js - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
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
//  07/22/2016 - Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Grid Solutions Framework Core Web Knockout Extension Script Functions
"use strict";

ko.bindingHandlers.numeric = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).on("keydown", function(event) {
            // Allow backspace, delete, tab, escape, and enter
            if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
                // Ctrl+A
                (event.keyCode === 65 && event.ctrlKey) ||
                // E + - . ,
                (event.keyCode === 69 || event.keyCode === 107 || event.keyCode === 109 || event.keyCode === 110 || event.keyCode === 190 || event.keyCode === 188) ||
                // home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                return;
            }
            else {
                // If value is not a number, stop key-press
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });
    }
};

ko.bindingHandlers.integer = {
    init: function(element, valueAccessor, allBindings, viewModel, bindingContext) {
        $(element).on("keydown", function(event) {
            // Allow backspace, delete, tab, escape, and enter
            if (event.keyCode === 46 || event.keyCode === 8 || event.keyCode === 9 || event.keyCode === 27 || event.keyCode === 13 ||
                // Ctrl+A
                (event.keyCode === 65 && event.ctrlKey) ||
                // home, end, left, right
                (event.keyCode >= 35 && event.keyCode <= 39)) {
                return;
            }
            else {
                // If value is not a number, stop key-press
                if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                    event.preventDefault();
                }
            }
        });
    }
};
