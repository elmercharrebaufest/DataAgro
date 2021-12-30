var pausado;
var material = [];
var centros = [];
var fila = 0;
$(document).ready(function () {
    kendo.culture("es-AR");

    kendo.culture();
    InicializarElementos();
  
});


function InicializarElementos() {
    var hoy = new Date();
    pausado = ConvertirStringABool(pausado);

    $("#DesdeVigencia").kendoDateTimePicker();
    $("#HastaVigencia").kendoDateTimePicker();
    $("#DesdeVigenciaPago").kendoDateTimePicker();
    $("#HastaVigenciaPago").kendoDateTimePicker();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "23:59");
    $("#Precio").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    $("#CantidadDia").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });
    $("#Tasa").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#PrecioSustentable").kendoNumericTextBox({ culture: "es-AR", format: "n2", spinners: false, min: 0 });
    $("#DesdeVigenciaSustentable").kendoDateTimePicker();
    $("#HastaVigenciaSustentable").kendoDateTimePicker();
    $("#DesdeEntregaSustentable").kendoDatePicker();
    $("#HastaEntregaSustentable").kendoDatePicker();

    $("#DiaPizarra").kendoDatePicker();
    $("#DiaPizarraHasta").kendoDatePicker();
    $("#DesdeEntregaPizarra").kendoDatePicker();
    $("#HastaEntregaPizarra").kendoDatePicker();
    $("#DesdeEntrega").kendoDatePicker();
    $("#HastaEntrega").kendoDatePicker();
    $("#DesdeFijacion").kendoDatePicker();
    $("#HastaFijacion").kendoDatePicker();
    $("#PizarraDesde").kendoTimePicker();
    $("#PizarraHasta").kendoTimePicker();
    $("#FijacionDia").kendoDatePicker();
    $("#PrecioTab").click(function () {
        DeseleccionarForms();
        $("#PrecioTab").children().addClass("whc-selected");
        $("#PrecioMoa").show();
    });
    $("#PizarraTab").click(function () {
        DeseleccionarForms();
        $("#PizarraTab").children().addClass("whc-selected");
        $("#HabilitacionPizarra").show();
    });
    $("#CampañaTab").click(function () {
        DeseleccionarForms();
        $("#CampañaTab").children().addClass("whc-selected");
        $("#HabilitacionCampaña").show();
    });
    $("#FijacionTab").click(function () {
        DeseleccionarForms();
        $("#FijacionTab").children().addClass("whc-selected");
        $("#HabilitacionFijacion").show();
    });
    $("#PagoTab").click(function () {
        DeseleccionarForms();
        $("#PagoTab").children().addClass("whc-selected");
        $("#HabilitacionPagoDiferido").show();
    });
    $("#SustentableTab").click(function () {
        DeseleccionarForms();
        $("#SustentableTab").children().addClass("whc-selected");
        $("#HabilitacionSustentable").show();
    });

    $("#TipoNegocioId").change(function () {
        LimpiarPrecioForm();
        var value = $("#TipoNegocioId").val();
        if (value == 1) {
            $("#divPrecioFijacion").show();
            $("#divPrecioEntrega").show();
            $("#Precio").data("kendoNumericTextBox").enable(false);
            $("#MonedaId").prop('disabled', 'disabled');
        }
        if (value == 2) {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").show();
            $("#Precio").data("kendoNumericTextBox").enable(true);
            $("#MonedaId").prop('disabled', false);
        }
        if (value == 3) {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").hide();
            $("#Precio").data("kendoNumericTextBox").enable(true);
            $("#MonedaId").prop('disabled', false);

        }
        if (value == undefined || value == null || value == "") {
            $("#divPrecioFijacion").hide();
            $("#divPrecioEntrega").hide();
        }
    });

    $("#TipoNegocioIdPizarra").change(function () {
        LimpiarPizarraForm();
        var value = $("#TipoNegocioIdPizarra").val();
        if (value == 2) {
            $("#divEntregaPizarra").show();
        }
        if (value == 3) {
            $("#divEntregaPizarra").hide();
        }
        if (value == undefined || value == null || value == "") {
            $("#divEntregaPizarra").hide();
        }
    });
    $("#TipoNegocioId").change();
    $("#TipoNegocioIdPizarra").change();

    if (pausado == true) {
        $(".pausado").prop("checked", this.checked);
    }

    //CargarTablePrecio();   
}

function CargarTablePrecio() {
    var datos = result = MSExecuteOnServer('/Centro/Buscar', null);

    for (var i = 0; i < datos.Datos.length; i++) {
        if (datos.Datos[i].Id != 6 && datos.Datos[i].Id != 7 && datos.Datos[i].Id != 9 && datos.Datos[i].Id != 10 && datos.Datos[i].Id != 13) {
            centros.push({ Destino: datos.Datos[i].Descripcion });
        }
    }
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/ConfiguracionInterna/TraerPrecios'
            },
            update: {
                url: '/ConfiguracionInterna/UpdatePrecio',
                type: 'post',
                dataType: 'json',
                // dataType: "jsonp"
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    DesdeVigencia: { type: "date" },
                    HastaVigencia: { type: "date" },
                    DesdeEntrega: { type: "date" },
                    HastaEntrega: { type: "date" },
                    DesdeFijacion: { type: "date" },
                    HastaFijacion: { type: "date" },
                    Precio: { type: "number" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "Material", dir: "asc" }, { field: "DesdeVigencia", dir: "asc" }],
        serverFiltering: true,
        pageSize: 20,
        batch: true,
        autoSync: true,
        error: function (e) {
            $("#tablePrecio").data("kendoGrid").dataSource.read();
            MensErr("Ha ocurrido un error. Por favor intente nuevamente");
        }
    };

    $("#tablePrecio").kendoGrid({
        dataSource: ds,
        editable: true,
        columns: [
            //{ selectable: true, width: "50px" },
            {
                field: "TipoNegocio", type: "string", title: "Tipo <br> Negocio", filterable: {
                    multi: true, dataSource: [{
                        TipoNegocio: "A FIJAR"
                    }, {
                        TipoNegocio: "A PRECIO"
                    }, {
                        TipoNegocio: "FIJACION"
                    }]
                }, width: 100, template: "#=TipoNegocio#", editable: true,
            },
            {
                field: "Material", title: "Material", filterable: {
                    multi: true, dataSource: [{
                        Material: "Maiz"
                    }, {
                        Material: "Trigo"
                    }, {
                        Material: "Soja"
                    }, {
                        Material: "Girasol"
                    }, {
                        Material: "Girasol AO"
                    }]
                }, width: 100, template: "#=Material#", editable: true,
            },
            {
                field: "Destino", title: "Destino", filterable: {
                    multi: true, dataSource: centros
                }, width: 130, template: "#= (Destino == null) ? 'Todos' : Destino #", editable: true
            },
            { field: "Precio", format: "{0:n2}", width: 100, editable: false, editor: numberEditor },
            { field: "MonedaId", title: "Moneda", editable: true, width: 70 },
            { field: "DesdeEntrega", title: "Desde <br> Entrega", type: "date", format: "{0:dd-MMM-yy}", editable: true, width: 70 },
            { field: "HastaEntrega", title: "Hasta <br> Entrega", type: "date", format: "{0:dd-MMM-yy}", editable: true },
            { field: "DesdeFijacion", title: "Desde <br> Fijacion", type: "date", format: "{0:dd-MMM-yy}", editable: true, width: 70 },
            { field: "HastaFijacion", title: "Hasta <br> Fijacion", type: "date", format: "{0:dd-MMM-yy}", editable: true },
            { field: "DesdeVigencia", title: "Desde <br> Vigencia", type: "date", format: "{0:dd-MMM-yy}", editable: true, width: 70 },
            { field: "HastaVigencia", title: "Hasta <br> Vigencia", type: "date", format: "{0:dd-MMM-yy}", editable: true },
            {
                field: "Id", title: " ", editable: true, filterable: false, sortable: false, width: 80, template: function (dataItem) {

                    return (((dataItem.DesdeVigencia <= new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()) && dataItem.HastaVigencia >= new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()))
                        || dataItem.DesdeVigencia >= new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate())) ?
                        '<a data-toggle="tooltip" title="Limite Cupo" class="abrirModalLimite links-grid" onclick="EliminarPrecio(' + dataItem.Id + ')">' +
                        '<span> <i class="fa fa-minus-circle danger"></i> </span ></a >' : "")
                        +
                        '<a href="#" class="fa fa-copy danger" onclick="copiarPrecioMOA(' + dataItem.Id + ')"></a>'


                }
            }
        ],
        dataBound: function (e) {
            $(".cerrado").each(function (index) {
                var dataItem = e.sender.dataItem($(this).parent());
                if (dataItem.BloquearCupera == "Si") {
                    $(this).addClass('line');
                }
            });
            var items = this._data;
            var tableRows = $(this.table).find("tbody tr");
            tableRows.each(function (index) {
                var row = $(this);
                var Item = items[index];
                if (Item != null) {
                    if (Item.Material == "Maiz") {
                        row.addClass('maiz');
                    }
                    if (Item.Material == "Soja") {
                        row.addClass('soja');
                    }
                    if (Item.Material == "Trigo") {
                        row.addClass('trigo');
                    }
                    if (Item.Material == "Girasol") {
                        row.addClass('girasol');
                    }
                    if (Item.Material == "Girasol AO") {
                        row.addClass('girasolAO');
                    }
                }
            });
            changeVerDiaAnterior();
            changeVerVigencia();
        },
        pageable: {
            messages: {
                display: "{2} elementos",
                empty: "No hay elementos para mostrar",
                page: "P&aacute;gina",
                allPages: "Todas",
                of: "de {0}",
                itemsPerPage: "Elementos por p&aacute;gina",
                first: "Ir a la primer p&aacute;gina",
                previous: "Ir a la p&aacute;gina anterior",
                next: "Ir a la p&aacute;gina siguiente",
                last: "Ir a la &uacute;ltima p&aacute;gina",
                refresh: "Recargar"
            },
            input: true,
            numeric: true
        },
        scrollable: false,
        //sortable: {
        //    mode: "multiple",
        //    allowUnsort: true,
        //    showIndexes: false
        //},
        sortable: false,
        filterable: {
            height: 350,
            extra: false,
            checkAll: false,

            messages: {
                info: "Filtros:",
                filter: "Filtrar",
                clear: "Limpiar",
                isTrue: "SI",
                isFalse: "NO",
                and: "Y",
                or: "O"
            },
            operators: {
                string: {
                    contains: "Contains"
                },
                date: {
                    eq: "Igual",
                    gte: "Despu&eacute;s o igual a",
                    lte: "Antes o igual a"
                },
                number: {
                    eq: "Igual a",
                    gte: "Mayor que o igual a",
                    lte: "Menor que o igual a"
                }
            }
        },
        //edit: function (e) {
        //    //var index = e.sender.current().parent().index();

        //    //if (rowIndex === null) {
        //    //    rowIndex = index;
        //    //} else {
        //    //    if (rowIndex != index) {
        //    //        rowIndex = null;
        //    //        editCell = true;

        //            e.sender.dataSource.sync();
        //    //    }
        //    //}
        //},
    });

}
function numberEditor(container, options) {
    var hoy = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
    var readonly = options.model.TipoNegocio == "A FIJAR" ||
        !((options.model.DesdeVigencia <= hoy && options.model.HastaVigencia >= hoy) || options.model.DesdeVigencia >= hoy);
    $('<input ' + (readonly == true ? 'disabled="disabled" ' : '') + ' data-bind="value:' + options.field + '"/>')
        .appendTo(container)
        .kendoNumericTextBox({
            decimals: 2,
            format: "n2",
            step: 0.5,
            change: function () {
                console.log("Change :: " + this.value());
            },
        });
}

function changeVerVigencia() {
    if ($('#verVigencia').prop("checked")) {
        $("#tablePrecio").data("kendoGrid").showColumn("DesdeVigencia");
        $("#tablePrecio").data("kendoGrid").showColumn("HastaVigencia");
    } else {
        $("#tablePrecio").data("kendoGrid").hideColumn("DesdeVigencia");
        $("#tablePrecio").data("kendoGrid").hideColumn("HastaVigencia");
    }
}
function changeVerDiaAnterior() {
    if ($('#verDiaAnterior').prop("checked")) {
        var items = $('#tablePrecio').data('kendoGrid').dataSource.data();
        for (var i = 0; i < items.length; i++) {
            var $row = $('#tablePrecio').find("[data-uid='" + items[i].uid + "']"); // find grid row by uid
            $row.show();

        }
    } else {
        var items = $('#tablePrecio').data('kendoGrid').dataSource.data();
        var hoy = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
        for (var i = 0; i < items.length; i++) {
            var visible = (items[i].DesdeVigencia <= hoy && items[i].HastaVigencia >= hoy) || items[i].DesdeVigencia >= hoy;
            if (!visible) {
                var $row = $('#tablePrecio').find("[data-uid='" + items[i].uid + "']"); // find grid row by uid
                $row.hide();
            }
        }
    }
}

function EliminarPrecio(id) {
    var datos = MSExecuteOnServer('/ConfiguracionInterna/EliminarPrecio', { id: id });
    if (datos.HayError == true && datos.Errores[0].Message != "Se eliminó correctamente") {
        ShowErrorMessages(datos.Errores);
    } else {
        MensInfo("Se eliminó correctamente");
        $("#tablePrecio").data("kendoGrid").dataSource.read();
    }
}
function LimpiarPrecioForm() {
    $("#Precio").data("kendoNumericTextBox").value("0");
    $("#MonedaId").val("");
    $("#MaterialId").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigencia").val(stringDia + " " + "00:00");
    $("#HastaVigencia").val(stringDia + " " + "19:00");
    $("#DesdeEntrega").data("kendoDatePicker").value("");
    $("#HastaEntrega").data("kendoDatePicker").value("");
    $("#DesdeFijacion").data("kendoDatePicker").value("");
    $("#HastaFijacion").data("kendoDatePicker").value("");
}
function LimpiarPagoForm() {
    $("#CantidadDia").data("kendoNumericTextBox").value("0");
    $("#Tasa").data("kendoNumericTextBox").value("0");
    $("#TipoNegocioIdPago").val("");
    $("#MaterialIdPago").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "19:00");
    LimpiarDiaDiferido();
}
function LimpiarPizarraForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DiaPizarra").val(stringDia);
    $("#DiaPizarraHasta").val(stringDia);
    $("#PizarraDesde").val("00:00");
    $("#PizarraHasta").val("23:59");
}
function LimpiarFijacionForm() {
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#FijacionDia").val(stringDia);
    $("#MaterialFijacionId").val("");
}
function LimpiarSustentableForm() {
    $("#MonedaId").val("");
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#PrecioSustentable").data("kendoNumericTextBox").value("0");
    $("#DesdeVigenciaSustentable").val(stringDia + " " + "00:00");
    $("#HastaVigenciaSustentable").val(stringDia + " " + "23:59");
    $("#DesdeEntregaSustentable").data("kendoDatePicker").value("");
    $("#HastaEntregaSustentable").data("kendoDatePicker").value("");
}
function DeseleccionarForms() {
    $("#PrecioTab").children().removeClass("whc-selected");
    $("#PrecioMoa").hide();
    $("#PizarraTab").children().removeClass("whc-selected");
    $("#HabilitacionPizarra").hide();
    $("#FijacionTab").children().removeClass("whc-selected");
    $("#HabilitacionFijacion").hide();
    $("#CampañaTab").children().removeClass("whc-selected");
    $("#HabilitacionCampaña").hide();
    $("#PagoTab").children().removeClass("whc-selected");
    $("#HabilitacionPagoDiferido").hide();
    $("#SustentableTab").children().removeClass("whc-selected");
    $("#HabilitacionSustentable").hide();
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

function copiarPrecioMOA(id) {
    var configuracion = null;
    var data = $("#tablePrecio").data("kendoGrid").dataSource.data();
    for (item in data) {
        if (data[item].Id == id) {
            configuracion = data[item];
            break;
        }
    }
    if (configuracion == null) {
        return;
    }

    $("#TipoNegocioId").val(configuracion.TipoNegocioId);
    $("#TipoNegocioId").change();
    $("#MonedaId").val(configuracion.MonedaId);
    $("#MaterialId").val(configuracion.MaterialId);
    $("#DestinoId").val(configuracion.DestinoId);
    if (configuracion.Precio > 0) {
        $("#Precio").data("kendoNumericTextBox").value(configuracion.Precio);
    }
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigencia").val(stringDia + " " + "00:00");
    $("#HastaVigencia").val(stringDia + " " + "23:59");
    $("#DesdeEntrega").data("kendoDatePicker").value(new Date(configuracion.DesdeEntrega));
    $("#HastaEntrega").data("kendoDatePicker").value(new Date(configuracion.HastaEntrega));
    if (configuracion.DesdeFijacion != null) {
        $("#DesdeFijacion").data("kendoDatePicker").value(new Date(configuracion.DesdeFijacion));
    }
    if (configuracion.HastaFijacion != null) {
        $("#HastaFijacion").data("kendoDatePicker").value(new Date(configuracion.HastaFijacion));
    }
}

function copiarPago(configuracion) {
    configuracion = JSON.parse(configuracion);
    $("#TipoNegocioId").val(configuracion.TipoNegocioId);
    $("#TipoNegocioId").change();
    $("#MaterialId").val(configuracion.MaterialId);
    if (configuracion.Importe > 0) {
        $("#Importe").data("kendoNumericTextBox").value(configuracion.Importe);
    }
    if (configuracion.CantidadDia > 0) {
        $("#CantidadDia").data("kendoNumericTextBox").value(configuracion.CantidadDia);
    }
    var hoy = new Date();
    var stringDia = hoy.getDate().toString() + "/" + (hoy.getMonth() + 1).toString() + "/" + hoy.getFullYear().toString();
    $("#DesdeVigenciaPago").val(stringDia + " " + "00:00");
    $("#HastaVigenciaPago").val(stringDia + " " + "23:59");
    if (configuracion.Tasa > 0) {
        $("#Tasa").data("kendoNumericTextBox").value(configuracion.Tasa);
    }
}

//function Pausar() {  
//    MSExecuteOnServer("ConfiguracionInterna/PausarPrecios", { pausa: $("#pausar").is(":checked") ? true : false});
//    MensInfo("El cambio se guardó correctamente");
//}

function ObtenerDatosMaterialHabilitado() {

    var lista = [];
    lista.push({ MaterialId: 0, TipoNegocioId: 1, Habilitado: $("#AFIJAR").is(":checked") });
    lista.push({ MaterialId: 0, TipoNegocioId: 2, Habilitado: $("#APRECIO").is(":checked") });
    lista.push({ MaterialId: 0, TipoNegocioId: 3, Habilitado: $("#FIJACION").is(":checked") });
    for (var j = 1; j <= 3; j++) {
        for (var i = 0; i < material.length; i++) {
            lista.push({ MaterialId: material[i].Id, TipoNegocioId: j, Habilitado: $("#" + material[i].Descripcion.replace(" ", "") + j).is(":checked") });
        }
    }

    MSExecuteOnServer("ConfiguracionInterna/PausarPrecios", { lista: lista });
    MensInfo("El cambio se guardó correctamente");
}

function AbrirModal() {
    $("#modalHabilitarMaterial").modal("show");
}

function cancelarEstado() {
    $("#modalHabilitarMaterial").modal("hide");

    var data = MSExecuteURLOnServer("ConfiguracionInterna/ModalHabilitarMaterial");
    setTimeout(function () { $("#estadoHabilitacion").html(data); }, 500)
}

function AgregarPagoDiferido() {     
        if (fila <= 3) {

            fila = fila + 1;
            // Añadir caja de texto.            
            $(".agregarTasa" + fila).append('<div class="col-md-2"> <label>Días Hasta</label></div > <div class="col-sm-3"><input name="DiasPagoDiferido[' + fila + '].Cantidad" value="' + (fila == 3 ? 0 : fila + 1) + '0" type=text class="input w100" id=dia' + fila + '></div>');
            $(".agregarTasa" + fila).append('<div class="col-md-1"><label> Tasa</label > </div> <div class="col-sm-3"><input name="DiasPagoDiferido[' + fila + '].Tasa" value="0" type=text class="input w100" id=tasa' + fila + '></div>');
            $(".agregarTasa" + fila).append('<div class="col-md-1" onclick="EliminarPagoDiferido(' + fila + ')" id="icono' + fila + '"><i class="fa fa-minus-circle" aria-hidden="true"></i></div> ');
            $("#icono" + fila).show();
            $(".agregarTasa" + fila).show();
            $("#dia" + fila).kendoNumericTextBox({
                culture: "es-AR",
                format: "n0",
                spinners: false,
                min: 0
            });
            $("#tasa" + fila).kendoNumericTextBox({
                culture: "es-AR",
                format: "n2",
                spinners: false,
                min: 0
            });
            $(".agregarTasa" + fila).show();
            if (fila == 2) {
                $("#icono" + 1).hide();
            } else if (fila == 3) {
                $("#icono" + 1).hide();
                $("#icono" + 2).hide();               
            }
            if (fila > 3) {
                fila = 3;
            }
    }

        
}

function EliminarPagoDiferido(item) {      
    $("#dia" + item).data("kendoNumericTextBox").destroy();
    $("#tasa" + item).data("kendoNumericTextBox").destroy();
    $(".agregarTasa" + fila).html('');
    fila = fila - 1;
    $("#icono" + fila).show();
}

function LimpiarDiaDiferido() {
    var filas = fila;
    for (var i = filas; i <= filas; i--) {   
        if (i == 0) {
            break;
        }
        EliminarPagoDiferido(i);
    }
}


