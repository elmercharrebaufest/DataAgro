var viewModel;
var fecha;
var zonaSeleccionada;
var anularCupo;
var modificarCupo; 

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    
    InicializarCargaCupos();
    $.unblockUI();
    checkFason();
    checkSoja();
});



function InicializarCargaCupos() {
    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#Proveedor").val("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });

    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="buscar-nomb">#: data.RazonSocial#(#: data.Cuit#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        change: function () {
            if ($("#buscadorProveedor").val().split('|').length > 1) {
                $("#buscadorProveedor").val($("#buscadorProveedor").val().split('|')[1]);

            }
        },
        select: function (e) {
            $("#Proveedor").val(e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Cupo/BuscarProveedor"
                },
                parameterMap: function (data, type) {
                    return { filtroProveedor: $('#buscadorProveedor').val() };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#fechaEntrega").kendoDatePicker({
    });

    $("#cantidad").kendoNumericTextBox({
        optionLabel: "SELECCIONE CANTIDAD DE CUPOS...",
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });
    $("#cuit").mask("00000000000");
    $("#fason").click(function () {
        checkFason();
    });
    $("#material").change(function () {
        checkSoja();
    });

    if ($("#Id").val() != null && $("#Id").val() != "0") {

        $("#material").attr('disabled', 'disabled');
        $("#planta").attr('disabled', 'disabled');
        $("#fechaEntrega").data('kendoDatePicker').enable(false);
        $("#cantidad").data('kendoNumericTextBox').enable(false);
        $("#zona").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        if ($("#flete").is(':checked')) {
            MensInfo("Cupo con condición de Flete");
        }
    }

}
function checkFason() {
    if ($("#fason").is(':checked')) {
        $("#cuit").show();
    }
    else {
        $("#cuit").hide("hidden");
        $("#cuit").val("");
    }
}
function checkSoja() {
    if ($("#material").val() !== "3") {
        $("#calidadDiv").hide();
        $("#calidad").val("");
    } else {
        $("#calidadDiv").show();
    }
}