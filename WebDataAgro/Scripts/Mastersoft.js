//--------------------------------------------------
//  Variables Globales
//--------------------------------------------------

var gblFechas = ["Hoy", "Semana Actual", "Mes Actual", "Año Actual", "Mes Anterior", "Año Anterior", "Personalizado", "Vacío"]
var gblPersDesc = "Personalizado";
var gblPersIndex = 6;

//--------------------------------------------------
//  Funciones JavaScript Genericas de Mastersoft
//--------------------------------------------------

function MSGetAppName() {

    return "";
}

function MSGetUrl(url) {

    var urlok = url;
    var appname = MSGetAppName();

    if (appname.length > 0) {
        urlok = '/' + appname + url
    }

    return urlok;
}


function MSShowLoading(htmlloading) {

    if (htmlloading != '') {
        $(htmlloading).showLoading();
    }
}


function MSHideLoading(htmlloading) {

    if (htmlloading != '') {
        $(htmlloading).hideLoading();
    }
}


function MSExecuteOnServer(url, datos, onCallBack) {

    var respuesta = null;

    //alert(MSGetUrl(url));

    $.ajax({
        async: false,
        url: MSGetUrl(url),
        type: 'POST',
        cache: false,
        data: kendo.stringify(datos),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            respuesta = data;
        },
        error: function (jqXHR) {
            if (jqXHR.status === 401 || jqXHR.status === 409) {
                return "error";
            } else {
                MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información tecnica: " + JSON.stringify(datos));
            }
        },
        complete: function () {
            if (onCallBack) {
                onCallBack();
            }
        }
    });

    return respuesta;
}


function MSExecuteOnServerAsync(url, datos, fncallback, iswait) {

    if (iswait) {
        $('#myPleaseWait').modal('show');
    }

    return new Promise(function (resolve, reject) {
        $.ajax({
            timeout: 600000,
            async: true,
            url: MSGetUrl(url),
            type: 'POST',
            data: kendo.stringify(datos),
            dataType: "text",   // evita parseerror cuando el servidor devuelve cuerpo vacío con 200
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var owait = $('#myPleaseWait');
                if (owait != null) {
                    owait.modal('hide');
                }

                var parsed = null;
                if (data && data.trim() !== '') {
                    try {
                        parsed = JSON.parse(data);
                    } catch (e) {
                        console.warn("MSExecuteOnServerAsync: respuesta no es JSON válido para URL: " + url, data);
                    }
                }

                if (fncallback) {
                    fncallback(parsed);
                }
                resolve(parsed);
            },
            error: function (xhr, status, error) {
                var owait = $('#myPleaseWait');
                if (owait != null) {
                    owait.modal('hide');
                }

                const errObj = {
                    xhr: xhr,
                    status: status,
                    error: error
                };

                MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

                if (fncallback) {
                    fncallback(null);
                }
                reject(errObj);
            }
        });
    });
}


function MSExecuteURLOnServer(url) {

    var respuesta = null;

    //alert(MSGetUrl(url));

    $.ajax({
        async: false,
        url: MSGetUrl(url),
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            respuesta = data;
        },
        error: function (error) {
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

        }
    });

    return respuesta;
}

function MSRedirectURLOnServer(url, datos) {

    var respuesta = null;

    //alert(MSGetUrl(url));

    $.ajax({
        async: false,
        url: MSGetUrl(url),
        type: 'POST',
        data: kendo.stringify(datos),
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data) {
                respuesta = data;

            }
        },
        error: function (error) {
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

        }
    });

    return respuesta;
}
function MSExecuteURLOnServerAsync(url, fncallback, htmlloading) {

    //alert(MSGetUrl(url));

    MSShowLoading(htmlloading);

    $.ajax({
        async: true,
        url: MSGetUrl(url),
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            MSHideLoading(htmlloading);
            fncallback(data);
        },
        error: function (error) {
            MSHideLoading(htmlloading);
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

            //alert(kendo.stringify(error));
        }
    });
}


function ExecuteURLOnServer(url, fncallback, htmlloading) {

    //alert(MSGetUrl(url));

    MSShowLoading(htmlloading);

    $.ajax({
        async: false,
        url: MSGetUrl(url),
        type: 'POST',
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            MSHideLoading(htmlloading);
            fncallback(data);
        },
        error: function (error) {
            MSHideLoading(htmlloading);
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

            //alert(kendo.stringify(error));
        }
    });
}


function MensErr(mensaje) {

    BootstrapDialog.show({
        title: 'Error',
        cssClass: 'error-dialog modal-superior',
        message: "\n" + mensaje,
        draggable: true,
        buttons: [{
            label: 'Cerrar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
            }
        }]
    });

}


function MensInfo(mensaje) {

    BootstrapDialog.show({
        title: 'Mensaje',
        message: "\n" + mensaje,
        draggable: true,
        buttons: [{
            label: 'Cerrar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
            }
        }]
    });
}

function MensAlerta(mensaje) {
    BootstrapDialog.confirm({
        title: 'Alerta',
        message: "\n" + mensaje,
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        closable: false, // <-- Default value is false
        draggable: false, // <-- Default value is false
    });
}

function MensAlerta(mensaje) {

    BootstrapDialog.show({
        title: 'Alerta',
        message: "\n" + mensaje,
        draggable: true,
        type: BootstrapDialog.TYPE_WARNING,
        buttons: [{
            label: 'Cerrar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
            }
        }]
    });
}

function MensInfoReload(mensaje) {

    BootstrapDialog.show({
        title: 'Mensaje',
        message: "\n" + mensaje,
        draggable: true,
        buttons: [{
            label: 'Cerrar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
                window.location.reload();
            }
        }]
    });
}


function Confirma(mensaje, fncallback) {

    BootstrapDialog.show({
        title: 'Confirmación',
        message: "\n" + mensaje,
        draggable: true,
        buttons: [{
            label: 'Aceptar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
                fncallback();
            }
        }, {
            label: 'Cancelar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
            }
        }]
    });

}

function ConfirmaConAdvertencia(mensaje, fncallback) {

    BootstrapDialog.show({
        title: 'Confirmación',
        message: "\n" + mensaje,
        type: BootstrapDialog.TYPE_WARNING,
        draggable: true,
        buttons: [{
            label: 'Aceptar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
                fncallback();
            }
        }, {
            label: 'Cancelar',
            cssClass: 'k-button',
            action: function (dialogItself) {
                dialogItself.close();
            }
        }]
    });

}

function parseJsonDate(jsonDate) {

    var offset = new Date().getTimezoneOffset() * 60000;
    var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);

    if (parts[2] == undefined)
        parts[2] = 0;

    if (parts[3] == undefined)
        parts[3] = 0;

    return new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
}


function GridSetEnabled(enabled) {

    if (enabled) {
        $(".k-add-button").removeClass("k-state-disabled").addClass("k-grid-add");
    }
    else {
        $(".k-add-button").addClass("k-state-disabled").removeClass("k-grid-add");
    }
}


function ExistsErrorMessages(arrayDeErrores) {

    if (arrayDeErrores.length == 0) {
        return false;
    }
    else {
        return true;
    }
}



function ShowErrorMessages(arrayDeErrores) {

    var mensaje = "";

    for (var i = 0; i < arrayDeErrores.length; i++) {
        mensaje = mensaje + arrayDeErrores[i].Message
    }

    if (mensaje.length > 0) {
        MensErr(mensaje);
    }
}


function ShowValidationMessages(viewModel, arrayDeErrores) {

    var mensaje = "";

    for (var i = 0; i < arrayDeErrores.length; i++) {

        if (arrayDeErrores[i].Source.length > 0) {
            viewModel.set("msg" + arrayDeErrores[i].Source, arrayDeErrores[i].Message);
        }
        else {
            mensaje = arrayDeErrores[i].Message
        }
    }

    if (mensaje.length > 0) {
        MensErr(mensaje);
    }
}


function ShowTooltipMessages(prefix, arrayDeErrores) {
    var mensaje = "";
    var aviso = "";

    for (var i = 0; i < arrayDeErrores.length; i++) {
        if (arrayDeErrores[i].Source.length > 0 && arrayDeErrores[i].Source != "aviso") {
            $("#" + prefix + arrayDeErrores[i].Source).css("display", "inline");

            var myTooltip = $("#" + prefix + arrayDeErrores[i].Source).data("kendoTooltip");

            if (myTooltip == null || typeof (myTooltip) == 'undefined') {
                $("#" + prefix + arrayDeErrores[i].Source).kendoTooltip({
                    content: arrayDeErrores[i].Message,
                    position: "bottom"
                });
            }
            else {
                myTooltip.options.content = arrayDeErrores[i].Message;
                myTooltip.refresh();
            }
        }
        else if (arrayDeErrores[i].Source == "aviso") {
            aviso = arrayDeErrores[i].Message;
        }
        else {
            mensaje = arrayDeErrores[i].Message
        }
    }

    if (mensaje.length > 0) {
        MensErr(mensaje);
    }
    else if (aviso.length > 0) {
        MensInfo(aviso);
    }
}


function EmptyValue(data) {

    if (typeof (data) == 'number') {

        if (data == 0) {
            return true;
        }
        else {
            return false;
        }
    }

    if (typeof (data) == 'boolean') {
        return false;
    }

    if (typeof (data) == 'undefined' || data === null) {
        return true;
    }

    if (typeof (data.length) != 'undefined') {

        if (/^[\s]*$/.test(data.toString())) {
            return true;
        }

        return data.length == 0;
    }

    return false;
}


function GetGridCurrentDataItem(id) {

    var grid = $("#" + id).data("kendoGrid");

    var editRow = grid.tbody.find("tr:has(.k-edit-cell)");

    return grid.dataItem(editRow);
}


function GetFechaDesde(opcion) {

    var fecha = new Date();

    fecha.setHours(0, 0, 0, 0);

    if (opcion == "Semana Actual") {
        fecha.setDate(fecha.getDate() + (1 - fecha.getDay()));
    }
    else if (opcion == "Mes Actual") {
        fecha = new Date(fecha.getFullYear(), fecha.getMonth(), 1, 0, 0, 0, 0);
    }
    else if (opcion == "Año Actual") {
        fecha = new Date(fecha.getFullYear(), 0, 1, 0, 0, 0, 0);
    }
    else if (opcion == "Mes Anterior") {
        fecha.setMonth(fecha.getMonth() - 1, 1)
    }
    else if (opcion == "Año Anterior") {
        fecha = new Date(fecha.getFullYear() - 1, 0, 1, 0, 0, 0, 0);
    }
    else if (opcion == "Vacío") {
        fecha = null;
    }

    return fecha;
}


function GetFechaHasta(opcion) {

    var fecha = new Date();

    fecha.setHours(23, 59, 59, 999);

    if (opcion == "Semana Actual") {
        fecha.setDate(fecha.getDate() + (1 - fecha.getDay()));
        fecha.setDate(fecha.getDate() + 6);
    }
    else if (opcion == "Mes Actual") {
        fecha = new Date(fecha.getFullYear(), fecha.getMonth() + 1, 0, 23, 59, 59, 999);

    }
    else if (opcion == "Año Actual") {
        fecha = new Date(fecha.getFullYear(), 11, 31, 23, 59, 59, 999);
    }
    else if (opcion == "Mes Anterior") {
        fecha.setMonth(fecha.getMonth(), 0)
    }
    else if (opcion == "Año Anterior") {
        fecha = new Date(fecha.getFullYear() - 1, 11, 31, 23, 59, 59, 999);
    }
    else if (opcion == "Vacío") {
        fecha = null;
    }

    return fecha;
}


function GetDropDownValue(viewModel, identificador) {

    var value = viewModel.get(identificador);

    if (typeof (value) == 'undefined') {

        var pos = identificador.lastIndexOf(".");

        var identificador = identificador.slice(0, pos);

        value = viewModel.get(identificador);
    }

    return value;
}


function AsignarRangoFecha(dropdownid, index, viewModel, identificadorDesde, identificadorHasta) {

    var dropdownlist = $(dropdownid).data("kendoDropDownList");

    dropdownlist.select(index);

    var opcion = dropdownlist.value();

    if (opcion != gblPersDesc) {
        viewModel.set(identificadorDesde, GetFechaDesde(opcion));
        viewModel.set(identificadorHasta, GetFechaHasta(opcion));
    }
}


function InitMaskedDatePicker() {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
        proxy = $.proxy,
        CHANGE = "change",
        PROGRESS = "progress",
        ERROR = "error",
        NS = ".generalInfo";

    var MaskedDatePicker = Widget.extend({
        init: function (element, options) {
            var that = this;
            Widget.fn.init.call(this, element, options);

            $(element).kendoMaskedTextBox({ mask: that.options.dateOptions.mask || "00/00/0000" })
                .kendoDatePicker({
                    format: that.options.dateOptions.format || "dd/MM/yyyy",
                    parseFormats: that.options.dateOptions.parseFormats || ["dd/MM/yyyy", "dd/MM/yy"]
                })
                .closest(".k-datepicker")
                .add(element)
                .removeClass("k-textbox");

            that.element.data("kendoDatePicker").bind("change", function () {
                that.trigger(CHANGE);
            });
        },
        options: {
            name: "MaskedDatePicker",
            dateOptions: {}
        },
        events: [
            CHANGE
        ],
        destroy: function () {
            var that = this;
            Widget.fn.destroy.call(that);

            kendo.destroy(that.element);
        },
        value: function (value) {
            var datepicker = this.element.data("kendoDatePicker");

            if (value === undefined) {
                return datepicker.value();
            }

            datepicker.value(value);
        }
    });

    ui.plugin(MaskedDatePicker);
}


function ValidDate(errores, id) {
    if ($("#" + id).data("kendoMaskedTextBox").value().length != 0) {
        if ($("#" + id).data("kendoDatePicker").value() == null) {
            errores.push({ Message: "Este campo debe ser una fecha valida", Source: id });
        }
    }
}


function ValidDateNoEmpty(errores, id) {
    if ($("#" + id).data("kendoMaskedTextBox").value().length == 0) {
        errores.push({ Message: "Esta fecha no debe ser vacía", Source: id });
    }
    else if ($("#" + id).data("kendoDatePicker").value() == null) {
        errores.push({ Message: "Este campo debe ser una fecha valida", Source: id });
    }
}


function AddIncorectMessage(errores) {
    errores.push({ Message: "Datos incorrectos, verifique el mensaje de error en cada campo", Source: "" });
}

function BlockUi(mensaje) {
    $.blockUI({ blockMsgClass: 'alertBox', message: '<h3>' + mensaje + '</h3>' });
}

function ConvertirStringABool(valor) {
    return valor == "True" ? true : valor == "False" ? false : valor;
}

function MSExecuteGetOnServer(url, datos, onCallBack) {

    var respuesta = null;

    $.ajax({
        async: false,
        url: MSGetUrl(url),
        type: 'GET',
        cache: false,
        data: datos,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            respuesta = data;
        },
        error: function (error) {
            MensErr("No se pudieron obtener los datos del servidor \n URL: " + url +
                "\n Información tecnica: " + JSON.stringify(error));
        },
        complete: function () {
            if (onCallBack) {
                onCallBack(respuesta);
            }
        }
    });

    return respuesta;
}

function MSExecuteGetOnServerAsync(url, datos) {

    datos = datos !== undefined ? datos : null;

    return new Promise(function (resolve, reject) {

        $.ajax({
            url: MSGetUrl(url),
            type: 'GET',
            cache: false,
            data: datos,
            dataType: "text",
            success: function (data) {
                if (!data || data.trim() === '') {
                    resolve(null);
                    return;
                }
                try {
                    resolve(JSON.parse(data));
                } catch (e) {
                    console.warn("MSExecuteGetOnServerAsync: respuesta no es JSON válido para URL: " + url, data);
                    resolve(null);
                }
            },
            error: function (xhr, status, error) {
                var errObj = {
                    xhr: xhr,
                    status: status,
                    error: error
                };

                console.error("Error AJAX GET:", errObj);
                MensErr("No se pudieron obtener los datos del servidor \n URL: " + url +
                    "\n Información técnica: " + JSON.stringify(errObj));
                reject(errObj);
            }
        });

    });
}
async function MSDownloadFileAsync(url, datos) {

    datos = datos !== undefined ? datos : null;

    return new Promise(function (resolve, reject) {

        $.ajax({
            url: MSGetUrl(url),
            type: 'GET',
            cache: false,
            data: datos,
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data, textStatus, xhr) {

                let fileName = "archivo";

                const disposition = xhr.getResponseHeader("Content-Disposition");

                if (disposition) {
                    const match = disposition.match(/filename="?([^"]+)"?/);
                    if (match) {
                        fileName = match[1];
                    }
                }

                const blobUrl = window.URL.createObjectURL(data);

                const link = document.createElement("a");
                link.href = blobUrl;
                link.download = fileName;

                document.body.appendChild(link);
                link.click();
                link.remove();

                window.URL.revokeObjectURL(blobUrl);

                resolve();
            },
            error: function (xhr, status, error) {

                var errObj = {
                    xhr: xhr,
                    status: status,
                    error: error
                };

                console.error("Error AJAX GET:", errObj);

                MensErr("No se pudo descargar el archivo.\nURL: " + url +
                    "\nInformación técnica: " + JSON.stringify(errObj));

                reject(errObj);
            }
        });

    });
}

const Materiales = {
    MAIZ: 1,
    TRIGO: 2,
    SOJA: 3,
    GIRASOL: 4,
    GIRASOL_AO: 5,
    SORGO: 6
};

const TIPO_NEGOCIO = {
    A_FIJAR: 1,
    A_PRECIO: 2,
    FIJACION: 3,
    FASON: 4,
    AGENTE_DE_COMPRAS: 5,
    CONTRATO_ACUERDO: 6,
    ESPACIO_DINAMICO: 7
}

const ESTADO_CONTRATO = {
    PENDIENTE: 1,
    CONFIRMADO: 2,
    OFERTA: 3,
    CON_ERROR: 4,
    FINALIZADO: 5,
    RECHAZADO: 6,
    RECONFIRMAR: 7,
    ELIMINADO: 8,
    PRE_APROBACION: 9,
    PRE_ANULADO: 10,
    RECONFIRMAR_FINALIZADO: 11
}

const ESTADO_INF_COMERIAL = {
    ACTUALIZADO: 1,
    DESACTUALIZADO: 2
}

const CLASIFICACION = {
    PRODUCTOR: 1,
    ACOPIADOR: 2,
    OTROS: 3
}

const BOLETO_COMPRANET = {
    CONFIRMA: 1,
    FISICO: 2,
    NINGUNO: 3,
    CARTA_OFERTA: 4,
    SIN_BOLETO: 5
}

const STANDARD_DE_CALIDAD = {
    CAMARA_COD_SAP_3: 1,
    ESPECIAL: 2,
    FABRICA: 3,
    CAMARA_COD_SAP_1: 4,
    CAMARA_COD_SAP_2: 5,
    MATERIA_EXTRANA: 6,
    GRADO_2: 7
}

const PORCENTAJE_PAGO = 97.5;
const PORCENTAJE_PAGO_TRIGO = 95.0;
