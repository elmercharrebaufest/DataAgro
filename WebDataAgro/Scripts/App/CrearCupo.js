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
    $("#fechaHasta").kendoDatePicker({
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
        $("#fechaHasta").data('kendoDatePicker').enable(false);
        $("#cantidad").data('kendoNumericTextBox').enable(false);
        $("#zona").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        if ($("#flete").is(':checked')) {
            MensInfo("Cupo con condición de Flete");
        }
        $("#guardarBtn").attr('type', 'button');
        $("#guardarBtn").click(function () {
            $('#fleteProcedenciaModal').modal('toggle');
        });
        $("#boton-si").click(function () {
            $("#flete").removeAttr('disabled');
            $("#flete").prop('checked', true);
            $("form").submit();
        });
        $("#boton-no").click(function () {
            $("#flete").removeAttr('disabled');
            $("#flete").prop('checked', false);
            $("form").submit();
        });
        $("#boton-cancelar").click(function () {
            window.location.href = window.location.origin + "/Cupo/";
        });
    }
    $("#fleteProcedenciaModal").draggable({
        handle: ".modal-header"
    }); 
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

function cuposCreados(error, lista) {
    $(document).ready(function () {
        var listaError = JSON.parse(error);
        if (listaError.length>0) {
            $("#error-modal").html(makeUL(listaError));
            $("#error-modal").show();
        }

        $("#cupos-generados-modal").html(lista.join(", "));
        $('#resultadoCupo').modal('toggle');

        $(".modal").on("hidden.bs.modal", function () {
            window.location.href = window.location.origin + "/Cupo/";
        });
    });
}
function makeUL(array) {
    var list = document.createElement('ul');
    for (var i = 0; i < array.length; i++) {
        var item = document.createElement('li');
        item.appendChild(document.createTextNode(array[i]));
        list.appendChild(item);
    }
    return list;
}

function copiarGenerados() {
    var listaCupos = $("#cupos-generados-modal").html().split(/[, ]/g).filter(e => e.trim().length > 0).join("\n");
    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', listaCupos);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', listaCupos);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}