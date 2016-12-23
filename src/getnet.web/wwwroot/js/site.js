window.getnet = (function () {

    var registeredRefreshes = {};

    function init(options) {
        getnet.options = options;
    }

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

    function copyToClipboard(elem) {
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
        copy: copyToClipboard,
        selectText: SelectElementText
    }
})();

unite.Forms = (function () {
    function init(options) {
        //requires jquery-form
        getnet.Forms.options = options;
        EnableForms();
    }

    function EnableForms() {
        $(".getnet-ajax-form").each(function (i) {
            var formId = $(this).attr("id");
            var updateTarget = formId + "-container";
            var loadingTarget = formid + "-loading";
            var errorTarget = formId + "-error";
            var successTarget = formid + "-success";
            $(this).ajaxSubmit({
                target: formId,
                error: function (e) {

                },
                replaceTarget: true,
                target: updateTarget,
                type: 'POST',
                success: function (data) {

                }
            })
        });
    }

    function Shutdown() {
        
    }

    return {
        init: init
    }
})();
