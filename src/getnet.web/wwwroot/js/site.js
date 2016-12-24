/// <reference path="../lib/jquery/dist/jquery.js"/>
/// <reference path="../lib/jquery-form/jquery-form.js"/>
/// <reference path="../lib/material-design-lite/material.js"/>

window.getnet = (function () {

    var registeredRefreshes = {};

    function init(options) {
        getnet.options = options;
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
            console.log('Refresh paused for: ' + name);
            pauseSingleRefresh(registeredRefreshes[name]);
            return;
        }

        console.log('Refresh paused');
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
            console.log('Refresh resumed for: ' + name);
            resumeSingleRefresh(registeredRefreshes[name]);
            return;
        }

        console.log('Refresh resumed');
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

    function CopyToClipboard(elem) {
        var targetId = "_hiddenCopyText_";
        var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
        var elemdata = elem.getAttribute("data-text");
        console.log(elem);
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
            target = document.getElementById(targetId);
            if (!target) {
                var target = document.createElement("textarea");
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
        closeAlert: CloseAlert
    }
})();

getnet.Forms = (function () {
    function init(options) {
        //requires jquery-form
        getnet.Forms.options = options;
        EnableForms();
    }

    function EnableForms() {
        $(".getnet-ajax-form").each(function (i) {
            EnableForm($(this).attr("id"));
        });
    }

    function EnableForm(formId) {
        if (formId.length == 0) {
            return;
        }

        //discover attributes            
        var updateTarget = GetIdOrData(formId + "-results", formId, "ajax-results");
        var loadingTarget = GetIdOrData(formId + "-loading", formId, "ajax-loading");
        var successTarget = GetIdOrData(formId + "-success", formId, "ajax-success");
        var reset = false;
        if ($("#" + formId).attr("data-ajax-reset"))
        {
            reset = true;
        }
        var successAction = function () {
            console.log("success or error form");
            if ($(loadingTarget).length > 0) {
                $(loadingTarget).hide();
            }
            if (reset) {
                EnableForm(formId);
            }
        };
        var errorAction = successAction;
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
        $("#" + formId).ajaxForm({
            target: formId,
            error: errorAction,
            target: updateTarget,
            type: 'POST',
            success: successAction,
            beforeSubmit: function () {
                console.log("Submitting form");
                if ($(loadingTarget).length > 0) {
                    $(loadingTarget).show();
                }
            }
        });
        console.log("Ajax submit loaded for " + formId);
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

    function Shutdown() {
        
    }

    return {
        init: init
    }
})();
