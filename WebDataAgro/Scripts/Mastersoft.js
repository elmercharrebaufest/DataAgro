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
        error: function (error) {
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información tecnica: " + JSON.stringify(datos));
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

    //alert(MSGetUrl(url));

    if (iswait) {
        $('#myPleaseWait').modal('show');
    }

    $.ajax({
        timeout: 600000,
        async: true,
        url: MSGetUrl(url),
        type: 'POST',
        data: kendo.stringify(datos),
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            var owait = $('#myPleaseWait');
            if (owait != null) {
                owait.modal('hide');
            }
            if (fncallback) {
                fncallback(data);
            }
        },
        error: function (error) {
            var owait = $('#myPleaseWait');
            if (owait != null) {
                owait.modal('hide');
            }
            MensErr("No se pudieron enviar los datos al servidor \n URL: " + url + "\n Información técnica: " + JSON.stringify(datos));

            //alert(kendo.stringify(error));
        }
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
        message: "\n"+mensaje,
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
        message: "\n"+mensaje,
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
        message: "\n"+mensaje,
        type: BootstrapDialog.TYPE_WARNING, // <-- Default value is BootstrapDialog.TYPE_PRIMARY
        closable: false, // <-- Default value is false
        draggable: false, // <-- Default value is false
    });
}

function MensAlerta(mensaje) {

    BootstrapDialog.show({
        title: 'Alerta',
        message: "\n"+mensaje,
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
        message: "\n"+mensaje,
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
        message: "\n"+mensaje,
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

            $("#" + prefix + arrayDeErrores[i].Source).css("display", "inline");;

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
        else if(arrayDeErrores[i].Source == "aviso") {
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
    $.blockUI({ blockMsgClass: 'alertBox', message: '<h3>' + mensaje +'</h3>' });
}

function ConvertirStringABool(valor) {
    return valor == "True" ? true : valor == "False" ? false : valor;    
}