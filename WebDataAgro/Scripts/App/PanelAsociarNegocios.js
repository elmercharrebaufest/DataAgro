var viewModelAsociados;
$(document).ready(function () {
    kendo.culture("es-AR");
    viewModelAsociados = kendo.observable({
        Asociados: []
    });
    $("#precioPonderadoId").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#asociadoId").kendoAutoComplete({
        footerTemplate: 'Total #: instance.dataSource.total() # Negocios encontrados',
        template: '<span class="k-state-default"></span>' +
            '<span class="k-state-default"><p>#: data.Identificador #</p></span>',
        dataTextField: "Identificador",
        dataValueField: "Identificador",
        placeholder: "Seleccione Negocio...",
        autoWidth: true,
        filter: "contains",
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/CompraNet/DevolverContratosParaAsociar"
                },
                parameterMap: function (data, type) {

                    return { contratoId: $("#paseId").val(), numero: $('#asociadoId').val() };
                }
            }

        },
        select: function (e) {

        }
    });


    $("#cerrarAsociarNegocios").click(function (e) {
        Cerrar();
    });
    $("#asociadoId").click(function (e) {
        $("#asociadoId").val("");
        $("#asociadoId").data("kendoAutoComplete").value("");
        $("#asociadoId").data("kendoAutoComplete").search("");

    });
    $("#asociadoId").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if (event.which < 48 || event.which > 57) {
            event.preventDefault();
        }
    });
    $("#gridAsociarNegocios").kendoGrid({
        dataSource: {
            data: [],
        },
        height: 200,
        pageable: false,
        columns: [
            {

                field: "Contrato", title: "Contrato", template: function (dataItem) {

                    return "<label  style='color: " + (dataItem.Color != null ? dataItem.Color : "") + "'> " + dataItem.Contrato + "</label>"

                }
            },
            {

                field: "TipoNegocioDesc", title: "Tipo", template: function (dataItem) {

                    return "<label  style='color: " + (dataItem.Color != null ? dataItem.Color : "") + "'> " + dataItem.TipoNegocioDesc + "</label>"

                }
            },
            {
                field: "Precio", title: "Precio", format: "{0:n2}", template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + kendo.toString(dataItem.Precio, "n2") + "</label>"
                    } else {
                        return kendo.toString(dataItem.Precio, "n2");
                    }
                }
            },
            {
                field: "MonedaDesc", title: "Moneda", attributes: { style: 'white-space: nowrap ' }, template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + dataItem.MonedaDesc + "</label>"
                    } else {
                        return dataItem.MonedaDesc;
                    }
                }
            },
            {
                field: "MaterialDesc", title: "Material", attributes: { style: 'white-space: nowrap ' }, template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + dataItem.MaterialDesc + "</label>"
                    } else {
                        return dataItem.MaterialDesc;
                    }
                }
            },
            {
                field: "Cantidad", title: "Cantidad (Kg)", width: 120, format: "{0:n0}", template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + kendo.toString(dataItem.Cantidad, "n0") + "</label>"
                    } else {
                        return kendo.toString(dataItem.Cantidad, "n0");
                    }
                }
            },
            {
                field: "Campania", title: "Campaña", template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + dataItem.Campania + "</label>"
                    } else {
                        return dataItem.Campania;
                    }
                }
            },
            {
                field: "Posicion", title: "Posicion", template: function (dataItem) {
                    if (dataItem.Color != "") {
                        return "<label  style='color: " + dataItem.Color + "'> " + dataItem.Posicion + "</label>"
                    } else {
                        return dataItem.Posicion;
                    }
                }
            },
            {
                field: "", template: function (dataItem) {
                    var data = dataItem;
                    if ($("#sePuedeEditar").val() == true) {
                        return '<button class="botones seleccionar" data-toggle="tooltip" title="Eliminar" onclick="Eliminar(\'' + data.Id + '\')"> Eliminar</button>';
                    } else {
                        return "";
                    }
                }
            }
        ],
    }).data("kendoGrid");

});

function Eliminar(data) {
    data = Number(data);
    if (viewModelAsociados.Asociados.length > 0) {
        for (var i = 0; i < viewModelAsociados.Asociados.length; i++) {
            if (viewModelAsociados.Asociados[i].Id == data) {
                viewModelAsociados.Asociados.remove(viewModelAsociados.Asociados[i]);
            }
        }
    }
    CalcularPonderado();
}

function AgregarAViewModel() {
    var autocomplete = $("#asociadoId").data("kendoAutoComplete");
    var dataItem = autocomplete.dataItem();
    var mensaje = ValidarViewModel(dataItem);
    
    if (mensaje == "") {
        var datos = { contratoSAP: dataItem.ContratoSap, TipoNegocioId: dataItem.TipoNegocioId };
        result = MSExecuteOnServer('/CompraNet/EstaConfirmadoEnSAP', datos);
        if (result != true) {
            MensErr("El contrato no esta confirmado en SAP.");
            return;
        }
        viewModelAsociados.Asociados.push(dataItem);
    } else {
        MensErr(mensaje)
    }
    $("#asociadoId").val("");
    $("#asociadoId").data("kendoAutoComplete").value("");
    CalcularPonderado();
}

function ValidarViewModel(dataItem) {
    var mensaje = "";
    var asociados = viewModelAsociados.Asociados;
    if (dataItem) {
        if (asociados && asociados.length > 0) {
            for (var i = 0; i < asociados.length; i++) {
                if (asociados[i].Id == dataItem.Id) {
                    mensaje = "Este negocio ya ha sido asociado";
                }
            }
        }
    } else {
        return mensaje = "Seleccione un negocio";
    }
    return mensaje;
}





function GrabarCambios() {

    BlockUi('Guardando...');
    CalcularPonderado();
    var datos = { negocio: viewModelAsociados.Asociados, contratoId: $("#paseId").val(), precioPonderado: $("#precioPonderadoId").val() };
    result = MSExecuteOnServer('/CompraNet/ActualizarNegociosAsociados', datos);
    $("#PanelAsociados").modal('hide');
    $("#asociadoId").val("");
    $("#asociadoId").data("kendoAutoComplete").value("");
    setTimeout(function () { $.unblockUI() }, 1000);

}


function Cerrar() {
    $("#asociadoId").val("");
    $("#asociadoId").data("kendoAutoComplete").value("");
    ActualizarAsociados($("#paseId").val());
}

function CalcularPonderado() {
    var asociados = viewModelAsociados.Asociados;
    var precioPonderado = 0;
    var totalidad = 0;

    if (asociados != null && asociados.length > 0) {
        for (var i = 0; i < asociados.length; i++) {
            totalidad = totalidad + asociados[i].Cantidad;
        }
        for (var i = 0; i < asociados.length; i++) {
            var calculo = asociados[i].Cantidad / totalidad * asociados[i].Precio;
            precioPonderado = precioPonderado + calculo;
        }
    }
    $("#precioPonderadoId").data("kendoNumericTextBox").value(precioPonderado);
}
