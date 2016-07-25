//******************************************************************************************************
//  gsf.web.primeui.js - Gbtc
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
//  04/08/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Grid Solutions Framework Core Web PrimeUI Extension Script Functions
"use strict";

// Get access to PrimeUI widget, e.g.: var autoCompletePanel = getPrimeUIWidget($("#myInputText"), "puiautocomplete").panel
function getPrimeUIWidget(jqueryElement, widgetType) {
    return jqueryElement.data("primeui-" + widgetType);
}

// Adds auto-complete lookup functionality to an input field element associated with a paged-view model 
function initializeAutoCompleteLookupField(fieldName, loadRecordsHubFunction, isObservable, addShowAllDropDown, limit) {
    if (isObservable === undefined)
        isObservable = true;

    if (addShowAllDropDown === undefined)
        addShowAllDropDown = true;

    if (limit === undefined)
        limit = 50;

    const inputFieldID = "input" + fieldName;
    const inputField = $("#" + inputFieldID);

    // Turn off browser remembered field values when doing auto-complete lookups
    inputField.attr("autocomplete", "off");

    if (isObservable)
        inputField.wrap("<div class=\"input-group\"></div>");

    if (addShowAllDropDown && viewModel.canEdit()) {
        // Insert drop-down button after input field
        inputField.after("<span id=\"" + inputFieldID + "ShowAll\" class=\"input-group-addon\" data-bind=\"style: {'cursor': ($root.recordMode()===RecordMode.View ? 'not-allowed' : 'pointer')}\"><i class=\"glyphicon glyphicon-triangle-bottom\"></i></span>");

        const inputFieldDropDown = $("#" + inputFieldID + "ShowAll");

        // Set on-click handler for drop-down button
        inputFieldDropDown.click(function () {
            // Search all records when user clicks drop-down button
            if (viewModel.dataHubIsConnected() && viewModel.recordMode() !== RecordMode.View)
                inputField.puiautocomplete("search", "");
        });

        // Apply data bindings to newly added drop-down button
        ko.applyBindings(viewModel, inputFieldDropDown[0]);
    }

    inputField.puiautocomplete({
        effect: "fade",
        effectSpeed: "fast",
        forceSelection: true,
        completeSource: function (request, response) {
            const self = this;

            if (viewModel.dataHubIsConnected()) {
                loadRecordsHubFunction(request.query, limit).done(function (records) {
                    if (limit > 0 && records.length >= limit) {
                        records.push({ id: null, label: "Search results truncated..." });
                    }

                    response.call(self, records);

                    // Set z-index of panel to be above pop-up dialog
                    self.panel.css("z-index", $("#addNewEditDialog").css("z-index") + 1);

                    // Auto-size panel to full width of input field area (including input group buttons)
                    setTimeout(function () { self.panel.width(inputField.parent().parent().width()); }, 25);
                });
            }
        }
    });

    if (isObservable) {
        inputField.puiautocomplete({
            select: function (event, item) {
                // Make sure knockout sees any selection - it can't always pickup non-user initiated changes
                if (item)
                    viewModel.currentRecord()[fieldName](item.text());
            }
        });
    }

    // Override jQuery UI display style added by puiautocomplete (messes with bootstrap form-element formatting)
    inputField.parent().css("display", "inline");
}
