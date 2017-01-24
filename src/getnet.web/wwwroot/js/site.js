/// <reference path="../lib/jquery/dist/jquery.js"/>
/// <reference path="../lib/jquery-form/jquery-form.js"/>
/// <reference path="../lib/material-design-lite/material.js"/>
/// <reference path="../lib/c3/c3.js"/>

//
//                 GETNET Main JS
//
//           -o/.             -/.`                  
//           Gooo/.`          -ooo/`                
//           Eoooooo/`        -ooooo/-`             
//           Toooooooo+:`     -oooooooo/`           
//           Nooooooooooo:/   -oooooooooo.          
//           Eoooooooooooooo:`-oooooooooo.          
//           Toooooooooooooooo+oooooooooo.          
//           `:+ooooooooooooooooooooooooo.          
//              -+ooooooooooooooooooooooo.          
//                 -+oooooooooooooooooooo.          
//                  `:++ooooooooooooooooo.          
//                     `-oooooooooooooooo.          
//                --      `:ooooooooooooo.          
//            .+oooooo+.    `:+oooooooooo.          
//           `/oooooooo:`      `:oooooooo.          
//           :soooooooo+`        ``/ooooo.          
//            :oooooooo-            `/soo.          
//             .--++--`                ./.  


window.getnet = (function () {

    var registeredRefreshes = {};

    function init(options) {
        getnet.options = options;
        getnet.Pings = 0;
    }

    //#region Refresh
    function registerRefresh(name, callback, interval, paused) {
        var refreshData = {
            name: name,
            func: callback,
            interval: interval,
            paused: paused
        };
        registeredRefreshes[name] = refreshData;
        refreshData.timer = setTimeout(function () { execRefresh(refreshData); }, refreshData.interval);
    }

    function runRefresh(name) {
        pauseRefresh(name);
        resumeRefresh(name);
    }

    function execRefresh(refreshData) {
        if (refreshData.paused) {
            return;
        }
        refreshData.func();
        refreshData.timer = setTimeout(function () { execRefresh(refreshData); }, refreshData.interval);
    }

    function pauseRefresh(name) {

        function pauseSingleRefresh(r) {
            r.paused = true;
            if (r.timer) {
                clearTimeout(r.timer);
                r.timer = 0;
            }
        }

        if (name && registeredRefreshes[name]) {
            getnet.log('Refresh paused for: ' + name);
            pauseSingleRefresh(registeredRefreshes[name]);
            return;
        }

        getnet.log('Refresh paused');
        for (var key in registeredRefreshes) {
            if (registeredRefreshes.hasOwnProperty(key)) {
                pauseSingleRefresh(registeredRefreshes[key]);
            }
        }
    }

    function resumeRefresh(name) {

        function resumeSingleRefresh(r) {
            if (r.timer) {
                clearTimeout(r.timer);
            }
            r.paused = false;
            execRefresh(r);
        }

        if (name && registeredRefreshes[name]) {
            getnet.log('Refresh resumed for: ' + name);
            resumeSingleRefresh(registeredRefreshes[name]);
            return;
        }

        getnet.log('Refresh resumed');
        for (var key in registeredRefreshes) {
            if (registeredRefreshes.hasOwnProperty(key)) {
                resumeSingleRefresh(registeredRefreshes[key]);
            }
        }
    }
    //#endregion Refresh

    function ShowSnack(data) {
        componentHandler.upgradeElement(document.querySelector('#global-snack'));
        document.querySelector('#global-snack').MaterialSnackbar.showSnackbar(data);
    }

    function PingAllFromServer() {
        $(".pingme").each(function (e) {
            if ($(this).is(":visible")) {
                $(this).trigger("click");
            }
        });
    }

    function PingFromServer(target) {
        var address = $(target).data("ip");
        var colorTarget = $(target).data("colortarget");
        getnet.log("pinging " + address);
        $("#fullpage-loading").show();
        getnet.Pings++;
        $.getJSON("/ping/" + encodeURIComponent(address)).done(
                function (data) {
                    if (data.success) {
                        getnet.log(address + " is up");
                        $(colorTarget).fadeOut(100);
                        $(colorTarget).removeClass("mdl-color--red-100");
                        $(colorTarget).addClass("mdl-color--green-100");
                        $(colorTarget).fadeIn(100);
                    } else {
                        getnet.log(address + " is down");
                        $(colorTarget).fadeOut(100);
                        $(colorTarget).removeClass("mdl-color--green-100");
                        $(colorTarget).addClass("mdl-color--red-100");
                        $(colorTarget).fadeIn(100);
                    }
                }
            ).always(
                function () {
                    getnet.Pings--;
                    if (getnet.Pings == 0) {
                        $("#fullpage-loading").hide();
                    }
                }
            );
    }

    function ToggleDrawer() {
        var layout = document.querySelector('.mdl-layout');
        layout.MaterialLayout.toggleDrawer();
    }

    function GetServerData(url) {
        $("#fullpage-loading").show();
        $.get(url, function (data) {
            return data;
        }).fail(function () {
            ShowSnack({
                message: "An error occured",
                timer: 10000
            });
        }).always(function () {
            $("#fullpage-loading").hide();
        });
    }

    function HipkuEncodeToSnack(ip) {
        ShowSnack({
            message: Hipku.encode(ip),
            timeout: 10000
        });
    }

    function CopyToClipboard(elem) {
        var targetId = "_hiddenCopyText_";
        var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
        var elemdata = elem.getAttribute("data-text");
        getnet.log(elem);
        var hasData = elemdata.length > 0;
        var origSelectionStart, origSelectionEnd;
        if (hasData) {
            target = document.getElementById(targetId);
            if (!target) {
                var target = document.createElement("textarea");
                target.style.position = "absolute";
                target.style.left = "-9999px";
                target.style.top = "0";
                target.id = targetId;
                document.body.appendChild(target);
            }
            target.textContent = elemdata;
        } else if (isInput) {
            target = elem;
            origSelectionStart = elem.selectionStart;
            origSelectionEnd = elem.selectionEnd;
        } else {
            var target = document.getElementById(targetId);
            if (!target) {
                target = document.createElement("textarea");
                target.style.position = "absolute";
                target.style.left = "-9999px";
                target.style.top = "0";
                target.id = targetId;
                document.body.appendChild(target);
            }
            target.textContent = elem.textContent;
        }
        var currentFocus = document.activeElement;
        target.focus();
        target.setSelectionRange(0, target.value.length);

        var succeed;
        try {
            succeed = document.execCommand("copy");
        } catch (e) {
            succeed = false;
        }
        if (currentFocus && typeof currentFocus.focus === "function") {
            currentFocus.focus();
        }

        if (isInput) {
            elem.setSelectionRange(origSelectionStart, origSelectionEnd);
        } else {
            target.textContent = "";
        }
        return succeed;
    }

    function CloseAlert(id) {
        $("#" + id).animate({ height: 0, paddingTop: 0, paddingBottom: 0, marginTop: 0, marginBottom: 0 }, 500);
        setTimeout(function () { $("#" + id).remove(); }, 500);
    }

    function SelectElementText(element) {
        var doc = document
            , text = doc.getElementById(element)
            , range, selection
        ;
        if (doc.body.createTextRange) {
            range = document.body.createTextRange();
            range.moveToElementText(text);
            range.select();
        } else if (window.getSelection) {
            selection = window.getSelection();
            range = document.createRange();
            range.selectNodeContents(text);
            selection.removeAllRanges();
            selection.addRange(range);
        }
    }

    function LogMessage(message) {
        if (getnet.options.enableLogging == true) {
            console.log(message);
        }
    }

    return {
        init: init,
        refresh: {
            register: registerRefresh,
            pause: pauseRefresh,
            resume: resumeRefresh,
            run: runRefresh,
            registered: registeredRefreshes
        },
        copy: CopyToClipboard,
        selectText: SelectElementText,
        showSnack: ShowSnack,
        closeAlert: CloseAlert,
        toggleDrawer: ToggleDrawer,
        log: LogMessage,
        ping: PingFromServer,
        pingall: PingAllFromServer,
        get: GetServerData,
        hipkuSnack: HipkuEncodeToSnack
    }
})();

getnet.Forms = (function () {
    function init(options) {
        //requires jquery-form
        getnet.Forms.options = options;
        EnableForms();
        EnableLinks();
    }

    function EnableForms() {
        $(".getnet-ajax-form").each(function (i) {
            EnableForm($(this).attr("id"));
        });

        $(".getnet-group-selector").each(function (i) {
            $(this).on("click", function () {
                $(".getnet-selection-group input").prop("disabled", true);
                $(".getnet-selection-group .mdl-textfield").addClass("is-disabled");
                $("#" + $(this).data("group") + " input").prop("disabled", false);
                $("#" + $(this).data("group") + " .mdl-textfield").removeClass("is-disabled");
            });
        });
    }

    function EnableLinks() {
        $(".getnet-ajax-link").each(function (i) {
            EnableLink($(this).attr("id"));
        });
    }

    function EnableForm(formId) {
        if (formId.length === 0) {
            return;
        }

        //discover attributes            
        var updateTarget = GetIdOrData(formId + "-results", formId, "ajax-results");
        var loadingTarget = GetIdOrData("fullpage-loading", formId, "ajax-loading");
        var reset = true;
        if ($("#" + formId).attr("data-ajax-reset") === "false")
        {
            reset = false;
        }
        var successAction = function () {
            getnet.log("success form");
            if ($(loadingTarget).length > 0) {
                $(loadingTarget).hide();
            }
            if (reset) {
                EnableForm(formId);
            }
        };
        var errorAction = function (data) {
            getnet.log("error form");
            $(updateTarget).html(data.responseText);
            if ($(loadingTarget).length > 0) {
                $(loadingTarget).hide();
            }
            if (reset) {
                EnableForm(formId);
            }
        };
        if ($("#" + formId).attr("data-ajax-success-action")) {
            successAction = function (e) {
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).hide();
                }
                eval($("#" + formId).data("ajax-success-action"));
                if (reset) {
                    EnableForm(formId);
                }
            }
        }
        if ($("#" + formId).attr("data-ajax-error-action")) {
            errorAction = function (data, status) {
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).hide();
                }
                eval($("#" + formId).data("ajax-error-action"));
                if (reset) {
                    EnableForm(formId);
                }
            }
        }

        //add ajax event
        $("#" + formId).ajaxForm({
            error: errorAction,
            target: updateTarget,
            type: 'POST',
            success: successAction,
            beforeSubmit: function () {
                getnet.log("Submitting form");
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).show();
                }
            }
        });
        getnet.log("Ajax form loaded for " + formId);
    }

    function EnableLink(formId) {
        if (formId.length === 0) {
            return;
        }

        //discover attributes            
        var updateTarget = GetIdOrData(formId + "-results", formId, "ajax-results");
        var loadingTarget = GetIdOrData("fullpage-loading", formId, "ajax-loading");
        var reset = true;
        if ($("#" + formId).attr("data-ajax-reset") === "false") {
            reset = false;
        }
        var successAction = function () {
            getnet.log("success link");
            if ($(loadingTarget).length > 0) {
                $(loadingTarget).hide();
            }
            if (reset) {
                EnableForm(formId);
            }
        };
        var errorAction = function (data) {
            getnet.log("error link");
            $(updateTarget).html(data.responseText);
            if ($(loadingTarget).length > 0) {
                $(loadingTarget).hide();
            }
            if (reset) {
                EnableForm(formId);
            }
        };
        if ($("#" + formId).attr("data-ajax-success-action")) {
            successAction = function (e) {
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).hide();
                }
                eval($("#" + formId).data("ajax-success-action"));
                if (reset) {
                    EnableForm(formId);
                }
            }
        }
        if ($("#" + formId).attr("data-ajax-success-action")) {
            errorAction = function (data, status) {
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).hide();
                }
                eval($("#" + formId).data("ajax-error-action"));
                if (reset) {
                    EnableForm(formId);
                }
            }
        }

        //add ajax event
        $("#" + formId).on("click", function (e) {
            e.preventDefault();
            $("#" + formId).ajaxSubmit({
                url: $("#" + formId).attr("href"),
                error: errorAction,
                target: updateTarget,
                type: 'GET',
                success: successAction,
                beforeSubmit: function () {
                    getnet.log("Submitting link");
                    if ($(loadingTarget).length > 0) {
                        $(loadingTarget).show();
                    }
                }
            });
        });        
        getnet.log("Ajax link submit loaded for " + formId);
    }

    function GetIdOrData(id, dataTarget, dataid) {
        if ($("#" + id).length > 0)
        {
            return "#" + id;
        } else if ($("#" + dataTarget).length > 0 && $("#" + dataTarget).attr("data-" + dataid)) {
            return "#" + $("#" + dataTarget).data(dataid);
        } else {
            return "";
        }
    }

    function DisplaySuccess(target, message) {
        var successAlert = " \
            <div class ='getnet-alert getnet-alert-success getnet-alert-closable' id='SuccessAlertJs'> \
                <button class ='mdl-button mdl-js-button mdl-button--icon getnet-alert-close' onclick='getnet.closeAlert(\"SuccessAlertJs\")'> \
                    <i class ='material-icons'>close</i> \
                </button>";
        successAlert = successAlert + message + "</div>";
        $("#" + target).html(successAlert);
    }

    function Shutdown() {
        
    }

    return {
        init: init,
        success: DisplaySuccess,
        enableLinks: EnableLinks,
        enableLink: EnableLink
    }
})();

getnet.Snacks = (function () {
    function init(options) {
        getnet.Snacks.options = options;
        getnet.Snacks.failureCount = 0;
        getnet.Snacks.retryCount = 3;
        LoadSnacks();
        if (options.refresh) {
            getnet.refresh.register("Snacks", function () { LoadSnacks(); }, getnet.Snacks.options.refresh * 1000);
        }
    }
    
    function LoadSnacks() {
        $.ajax({
            dataType: 'json',
            type: 'GET',
            url: '/a/getsnacks',
            success: function (data) {
                $.each(data, function (i, item) {
                    if (item.actionHandler) {
                        var action = item.actionHandler;
                        item.actionHandler = function () {
                            eval(action);
                        };
                    }
                    getnet.showSnack(item);
                });
            },
            error: function (reason) {
                getnet.log(reason.responseText);
                if (getnet.Snacks.failureCount >= getnet.Snacks.retryCount) {
                    Shutdown();
                } else {
                    getnet.Snacks.failureCount++;
                }
            }
        })
    }

    function Shutdown() {
        getnet.refresh.pause("Snacks");
    }

    return {
        init: init,
        loadSnacks: LoadSnacks
    }
})();


getnet.Dash = (function () {
    function init(options) {
        getnet.Dash.options = options;
        getnet.Dash.failureCount = 0;
        getnet.Dash.retryCount = 3;
        LoadDash();
        if (options.refresh) {
            getnet.refresh.register("Dash", function () { UpdateDash(); }, getnet.Dash.options.refresh * 1000);
        }
    }

    function LoadDash() {
        getnet.Dash.StatusChart = c3.generate({
            bindto: "#statuschart",
            data: {
                columns: [
                ],
                type: 'donut'
            },
            donut: {
                title: 'Site Status'
            }
        });
        UpdateDash();        
    }

    function UpdateDash() {
        $("#statuschart-loading").show();
        getnet.log("loading status chart data");
        getnet.Dash.StatusChart.unload();
        $.getJSON('/a/getsitestatus').success(function (data) {
            getnet.Dash.StatusChart.load(data);
        }).error(function (reason) {
            getnet.log(reason.responseText);
            if (getnet.Dash.failureCount >= getnet.Dash.retryCount) {
                Shutdown();
            } else {
                getnet.Dash.failureCount++;
            }
        }).always(function () {
            $("#statuschart-loading").hide();
        });
    }

    function Shutdown() {
        getnet.refresh.pause("Dash");
    }

    return {
        init: init
    }
})();