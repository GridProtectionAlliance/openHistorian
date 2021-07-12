//******************************************************************************************************
//  Site.js - Gbtc
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
//  01/15/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Hub connectivity and site specific scripts
"use strict";

// Declare page scoped SignalR variables
var dataHub, dataHubClient;
var sharedHub, sharedHubClient;
var securityHub, securityHubClient;
var serviceHub, serviceHubClient;
var hubIsConnecting = false;
var hubIsConnected = false;

const defaultInfoMessageTimeout = 2000;
const defaultErrorMessageTimeout = 6000;
const defaultHighEmphasisInfoMessageTimeout = 4000;
const defaultHighEmphasisErrorMessageTimeout = -1;

const messagePrefix = "<div><p>";
const messageSuffix = "</p></div><div><a href=\"#\" class=\"close\" aria-label=\"close\">×</a></div>";

const messagePanel = {
    template: jsPanel.tplContentOnly,
    paneltype: "hint",
    position: `right-top -5 ${Math.ceil($("#menuBar").height()) + 5} DOWN`,
    border: "2px solid",
    contentSize: "350 auto",
    show: "animated slideInUp",
    callback: function (panel) {
        this.content.css({ display: "flex" });

        $("div", this.content).eq(0).css({
            fontSize: "16px",
            textAlign: "center",
            width: "100%"
        });

        $("p", this.content).eq(0).css({
            margin: "auto",
            padding: "4px 4px"
        });

        $("div", this.content).eq(1).css({
            width: "5px",
            padding: "0 4px 0 12px"
        });
    }
};

function repositionPanels() {
    let index = Math.ceil($("#menuBar").height()) + 5;

    $(jsPanel.activePanels.list).each(function () {
        const panel = jsPanel.activePanels.getPanel(this);
        panel.reposition(`right-top -25 ${index}`);
        index += panel.height() + 10;
    });
}

$(window).on("resize", repositionPanels);

function hideErrorMessage(options) {
    if (!options)
        options = { closeHeaderPanel: true, closeFloatingPanels: true };

    if (options.closeHeaderPanel) {
        const wasVisible = $("#error-msg-block").is(":visible");

        $("#error-msg-block").hide();

        // Raise "messageVisibiltyChanged" event
        if (wasVisible)
            $(window).trigger("messageVisibiltyChanged");
    }

    if (options.closeFloatingPanels) {
        const activePanels = jsPanel.activePanels;

        $(activePanels.list).each(function () {
            const panel = activePanels.getPanel(this);

            if (panel.isErrorPanel)
                panel.close();
        });
    }
}

function hideInfoMessage(options) {
    if (!options)
        options = { closeHeaderPanel: true, closeFloatingPanels: true };

    if (options.closeHeaderPanel) {
        const wasVisible = $("#info-msg-block").is(":visible");

        $("#info-msg-block").hide();

        // Raise "messageVisibiltyChanged" event
        if (wasVisible)
            $(window).trigger("messageVisibiltyChanged");
    }

    if (options.closeFloatingPanels) {
        const activePanels = jsPanel.activePanels;

        $(activePanels.list).each(function () {
            const panel = activePanels.getPanel(this);

            if (panel.isInfoPanel)
                panel.close();
        });
    }
}

function showErrorMessage(message, timeout, highEmphasis) {
    if (highEmphasis) {
        // Show high-emphasis messages in header block
        const wasVisible = $("#error-msg-block").is(":visible");

        $("#error-msg-text").html(message);
        $("#error-msg-block").show();

        if (timeout === undefined || timeout === null)
            timeout = defaultHighEmphasisErrorMessageTimeout;

        if (timeout > 0)
            setTimeout(() => hideErrorMessage({ closeHeaderPanel: true }), timeout);

        // Raise "messageVisibiltyChanged" event
        if (!wasVisible)
            $(window).trigger("messageVisibiltyChanged");
    }
    else {
        if (timeout === undefined || timeout === null)
            timeout = defaultErrorMessageTimeout;

        const errorPanel = $.jsPanel($.extend({}, messagePanel, {
            autoclose: timeout,
            theme: "red filledlight",
            content: `${messagePrefix}${message}${messageSuffix}`
        }));

        errorPanel.css("backgroundColor", errorPanel.content.css("backgroundColor"));

        const panelID = errorPanel.attr("id");
        const dismissPanelID = `dismissPanel-${panelID}`;

        errorPanel.isErrorPanel = true;
        errorPanel.content.find("a:first").attr("id", dismissPanelID);

        $(`#${dismissPanelID}`).click(() => {
            errorPanel.close();
            repositionPanels();
        });
    }
}

function showInfoMessage(message, timeout, highEmphasis) {
    if (highEmphasis) {
        // Show high-emphasis messages in header block
        const wasVisible = $("#info-msg-block").is(":visible");

        $("#info-msg-text").html(message);
        $("#info-msg-block").show();

        if (timeout === undefined || timeout === null)
            timeout = defaultHighEmphasisInfoMessageTimeout;

        if (timeout > 0)
            setTimeout(() => hideInfoMessage({ closeHeaderPanel: true }), timeout);

        // Raise "messageVisibiltyChanged" event
        if (!wasVisible)
            $(window).trigger("messageVisibiltyChanged");
    }
    else {
        if (timeout === undefined || timeout === null)
            timeout = defaultInfoMessageTimeout;

        const infoPanel = $.jsPanel($.extend({}, messagePanel, {
            autoclose: timeout,
            theme: "green filledlight",
            content: `${messagePrefix}${message}${messageSuffix}`
        }));

        infoPanel.css("backgroundColor", infoPanel.content.css("backgroundColor"));

        const panelID = infoPanel.attr("id");
        const dismissPanelID = `dismissPanel-${panelID}`;

        infoPanel.isInfoPanel = true;
        infoPanel.content.find("a:first").attr("id", dismissPanelID);

        $(`#${dismissPanelID}`).click(() => {
            infoPanel.close();
            repositionPanels();
        });
    }
}

function calculateRemainingBodyHeight() {
    // Calculation based on content in Layout.cshtml
    return $(window).height() -
        $("#menuBar").outerHeight(true) -
        $("#bodyContainer").paddingHeight() -
        $("#pageHeader").outerHeight(true) -
        ($(window).width() < 768 ? 30 : 5);
}

function hubConnected() {
    hideErrorMessage();

    if (hubIsConnecting)
        showInfoMessage("Reconnected to service.", null, true);

    hubIsConnecting = false;
    hubIsConnected = true;

    // Re-enable hub dependent controls
    updateHubDependentControlState(true);

    // Raise "hubConnected" event
    $(window).trigger("hubConnected");
}

function updateHubDependentControlState(enabled) {
    // This only controls style - not enabled element state
    if (enabled)
        $("[hub-dependent]").removeClass("disabled");
    else
        $("[hub-dependent]").addClass("disabled");
}

// Useful to call when dynamic data-binding adds new controls
function refreshHubDependentControlState() {
    updateHubDependentControlState(hubIsConnected);
}

function startHubConnection() {
    $.connection.hub.start().done(function() {
        hubConnected();
    }).fail(function (err) {
        if (!err || !err.context)
            return;

        if (err.context.status === 401) {
            if (isIE) {
                // Attempt to clear any credentials cached by browser
                clearCachedCredentials(null, function (success) {
                    window.location.reload();
                });
            }
            else {
                window.location.reload();
            }
        }            
    });
}

$(function () {
    $(".page-header").css("margin-bottom", "-5px");

    const searchHeaders = $("[search-header]");

    // Apply initial content-fill-height styles
    if (searchHeaders.length > 0)
        $("[content-fill-height]").addClass("fill-height-with-search");
    else
        $("[content-fill-height]").addClass("fill-height");

    $(window).on("messageVisibiltyChanged", function (event) {
        const contentWells = $("[content-fill-height]");
        const errorIsVisble = $("#error-msg-block").is(":visible");
        const infoIsVisible = $("#info-msg-block").is(":visible");

        if (searchHeaders.length > 0) {
            contentWells.removeClass("fill-height-with-search fill-height-one-message-with-search fill-height-two-messages-with-search");

            if (errorIsVisble && infoIsVisible)
                contentWells.addClass("fill-height-two-messages-with-search");
            else if (errorIsVisble || infoIsVisible)
                contentWells.addClass("fill-height-one-message-with-search");
            else
                contentWells.addClass("fill-height-with-search");
        } else {
            contentWells.removeClass("fill-height fill-height-one-message fill-height-two-messages");

            if (errorIsVisble && infoIsVisible)
                contentWells.addClass("fill-height-two-messages");
            else if (errorIsVisble || infoIsVisible)
                contentWells.addClass("fill-height-one-message");
            else
                contentWells.addClass("fill-height");
        }
    });

    $("#dismissInfoMsg").click(() => hideInfoMessage({ closeHeaderPanel: true }));
    $("#dismissErrorMsg").click(() => hideErrorMessage({ closeHeaderPanel: true }));

    // Prevent clicking on disabled anchors
    $("body").on("click", "a.disabled", function (event) {
        event.preventDefault();
    });

    // Set initial state of hub dependent controls
    updateHubDependentControlState(false);

    // Initialize proxy references to the SignalR hubs
    dataHub = $.connection.dataHub.server;
    dataHubClient = $.connection.dataHub.client;
    sharedHub = $.connection.sharedHub.server;
    sharedHubClient = $.connection.sharedHub.client;
    securityHub = $.connection.securityHub.server;
    securityHubClient = $.connection.securityHub.client;
    serviceHub = $.connection.serviceHub.server;
    serviceHubClient = $.connection.serviceHub.client;

    $.connection.hub.reconnecting(function() {
        hubIsConnecting = true;
        showInfoMessage("Attempting to reconnect to service&nbsp;&nbsp;<span class='glyphicon glyphicon-refresh glyphicon-spin'></span>", -1, true);

        // Disable hub dependent controls
        updateHubDependentControlState(false);

        // Raise "hubDisconnected" event
        $(window).trigger("hubDisconnected");
    });

    $.connection.hub.reconnected(function() {
        hubConnected();
    });

    $.connection.hub.disconnected(function() {
        hubIsConnected = false;

        if (hubIsConnecting)
            showErrorMessage("Disconnected from server", -1, true);

        // Disable hub dependent controls
        updateHubDependentControlState(false);

        // Raise "hubDisconnected" event
        $(window).trigger("hubDisconnected");

        setTimeout(startHubConnection, 5000); // Restart connection after 5 seconds
    });

    // Create hub client functions for message control
    function encodeInfoMessage(message, timeout) {
        // Html encode message
        const encodedMessage = $("<div />").text(message).html();
        showInfoMessage(encodedMessage, timeout, true);
    }

    function encodeErrorMessage(message, timeout) {
        // Html encode message
        const encodedMessage = $("<div />").text(message).html();
        showErrorMessage(encodedMessage, timeout, true);
    }

    // Register info and error message handlers for each hub client
    dataHubClient.sendInfoMessage = encodeInfoMessage;
    dataHubClient.sendErrorMessage = encodeErrorMessage;
    sharedHubClient.sendInfoMessage = encodeInfoMessage;
    sharedHubClient.sendErrorMessage = encodeErrorMessage;
    securityHubClient.sendInfoMessage = encodeInfoMessage;
    securityHubClient.sendErrorMessage = encodeErrorMessage;
    serviceHubClient.sendInfoMessage = encodeInfoMessage;
    serviceHubClient.sendErrorMessage = encodeErrorMessage;

    // Raise "beforeHubConnected" event - client pages should use
    // this event to register any needed SignalR client functions
    $(window).trigger("beforeHubConnected");

    // Start the connection
    startHubConnection();

    // Enable tool-tips on the page
    $("[data-toggle='tooltip']").tooltip();

    // Keep body vertical scroll bars after nested Bootstrap modal dialogs are closed
    $(document).on("hidden.bs.modal", ".modal", function () {
        $(".modal:visible").length && $(document.body).addClass("modal-open");
    });
});
