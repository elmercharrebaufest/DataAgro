var ControlDeBoletosUI = window.ControlDeBoletosUI || (function () {
    "use strict";

    var notificationInstance = null;

    function getKendoDate($el) {
        var dp = $el && $el.data ? $el.data("kendoDatePicker") : null;
        return dp ? dp.value() : null;
    }

    function getKendoDateISO($el) {
        var date = getKendoDate($el);
        return date ? date.toISOString() : null;
    }

    function parseDateText(texto) {
        if (!texto) return null;
        var parsed = kendo.parseDate(texto, "dd/MM/yyyy") || kendo.parseDate(texto);
        return parsed && !isNaN(parsed.getTime()) ? parsed : null;
    }

    function parseServerDate(value) {
        if (!value) return null;

        if (typeof value === "string" && $.trim(value) === "") {
            return null;
        }

        var normalized = value;
        if (typeof normalized === "string") {
            normalized = normalized.replace(/\\/g, "");
        }

        var match = /Date\((\d+)\)/.exec(normalized);
        var parsed = match ? new Date(parseInt(match[1], 10)) : new Date(normalized);

        return isNaN(parsed.getTime()) ? null : parsed;
    }

    function setKendoDate($el, value) {
        var dp = $el && $el.data ? $el.data("kendoDatePicker") : null;
        if (!dp) return;

        if (!value) {
            dp.value(null);
            return;
        }

        var date = parseServerDate(value);
        dp.value(date);
    }

    function toggleSpinner(mostrar, spinnerId) {
        var id = spinnerId || "loadingSpinner";
        var spinner = document.getElementById(id);
        if (spinner) {
            spinner.style.display = mostrar ? "flex" : "none";
        }
    }

    function showModalMessage(titulo, mensaje, tipo) {
        var cssType = tipo || "info";
        $("#mensajeModalTitle").text(titulo);
        $("#mensajeModalBody").html('<div class="alert alert-' + cssType + '">' + mensaje + "</div>");
        $("#mensajeModal").modal("show");
    }

    function setButtonState($button, habilitado, texto) {
        $button.prop("disabled", !habilitado);
        if (texto) {
            $button.find("span").text(texto);
        }
    }

    function normalizeContractsPaste(texto) {
        if (!texto) return "";

        var values = String(texto)
            .split(/\r?\n/)
            .map(function (v) { return (v || "").trim(); })
            .filter(function (v) { return v !== ""; });

        values = values.filter(function (value, index) {
            return values.indexOf(value) === index;
        });

        return values.join(";");
    }

    function bindContractsPaste($input, onBeforeSet) {
        $input
            .on("paste", function (e) {
                e.preventDefault();

                if (typeof onBeforeSet === "function") {
                    onBeforeSet();
                }

                var texto = (e.originalEvent.clipboardData || window.clipboardData).getData("text");
                $(this).val(normalizeContractsPaste(texto));
            })
            .on("keypress", function (e) {
                if (e.which === 32) e.preventDefault();
            });
    }

    function loadDropdown(url, $select, textoCarga, textoDefault, mapFn) {
        var mapper = typeof mapFn === "function"
            ? mapFn
            : function (item) {
                return { value: item.Value, text: item.Text };
            };

        $select.html('<option value="">' + textoCarga + "</option>");

        return MSExecuteGetOnServerAsync(url)
            .then(function (data) {
                $select.empty().append('<option value="">' + textoDefault + "</option>");

                if (data && Array.isArray(data)) {
                    $.each(data, function (i, item) {
                        var mapped = mapper(item) || {};
                        $select.append('<option value="' + (mapped.value || "") + '">' + (mapped.text || "") + "</option>");
                    });
                } else {
                    $select.append('<option value="">Sin datos disponibles</option>');
                }
            })
            .catch(function () {
                $select.html('<option value="">Error al cargar datos</option>');
            });
    }

    function getNotification() {
        if (notificationInstance) {
            return notificationInstance;
        }

        var $notification = $("#notification");
        if (!$notification.length) {
            return null;
        }

        if (!$notification.data("kendoNotification")) {
            $notification.kendoNotification({
                position: {
                    pinned: true,
                    top: 50,
                    left: "50%"
                },
                autoHideAfter: 3000,
                stacking: "down"
            });
        }

        notificationInstance = $notification.data("kendoNotification");
        return notificationInstance;
    }

    function showNotification(mensaje, tipo) {
        var notification = getNotification();
        if (!notification) return;
        notification.show(mensaje, tipo || "info");
    }

    return {
        getKendoDate: getKendoDate,
        getKendoDateISO: getKendoDateISO,
        parseDateText: parseDateText,
        parseServerDate: parseServerDate,
        setKendoDate: setKendoDate,
        toggleSpinner: toggleSpinner,
        showModalMessage: showModalMessage,
        setButtonState: setButtonState,
        normalizeContractsPaste: normalizeContractsPaste,
        bindContractsPaste: bindContractsPaste,
        loadDropdown: loadDropdown,
        showNotification: showNotification
    };
})();
