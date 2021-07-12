var viewModel;
var fecha;
var zonaSeleccionada;
var fleteProcedencia;
var reasignarCupo;

$(document).ready(function () {
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    fleteProcedencia = ConvertirStringABool(fleteProcedencia);
    reasignarCupo = ConvertirStringABool(reasignarCupo);
    InicializarCargaCupos();
    $.unblockUI();
    checkFason();
    checkSoja();  
  
});
$(document).submit(function () {
    BlockUi("Grabando...");
});

function InicializarCargaCupos() {
    copiarTablaEstablecimiento();
    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#Proveedor").val("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
    });

    $("#buscadorProveedor").kendoAutoComplete({
        template: '<img class="buscar-cont" src="..' + MSGetUrl("/Content/Images/usuario-busqueda.png") + '" /> ' +
            '<p class="#:data.Corredor# buscar-nomb" value="#:data.RazonSocial#" >#: data.RazonSocial#(#: data.Cuit#)</p>',
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
            MostrarVisualizarStock();
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

        min: kendo.parseDate(new Date()),
        change: function () {
            $("#fechaHasta").data("kendoDatePicker").value("");
            var datepicker = $("#fechaHasta").data("kendoDatePicker");
            datepicker.min(kendo.parseDate($("#fechaEntrega").val()));
            datepicker.value(kendo.parseDate($("#fechaEntrega").val()));
            $("#contratoId").val("");
            CrearTablaFechaHasta();
        }
    });

    $("#fechaHasta").kendoDatePicker({
        min: kendo.parseDate($("#fechaEntrega").val()),
        change: function () {
            $("#contratoId").val("");
            CrearTablaFechaHasta();
            $("#boton-carga-masiva").show();
        }
    });
    $("#cantidad").kendoNumericTextBox({
        optionLabel: "SELECCIONE CANTIDAD DE CUPOS...",
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0,
        change: function () {
            $(".cantidad-masiva").val($("#cantidad").data('kendoNumericTextBox').value());
        }
    });
    CrearTablaFechaHasta();
    $("#cuit").mask("00000000000");
    $("#fason").click(function () {
        checkFason();
    });
    $("#material").change(function () {
        checkSoja();
        MostrarVisualizarStock();
    });
    $("#planta").change(function () {
        checkSoja();
        MostrarVisualizarStock();
    });
    if ($("#Id").val() != null && $("#Id").val() != "0") {

        $("#material").attr('disabled', 'disabled');
        $("#planta").attr('disabled', 'disabled');
        $("#fechaEntrega").data('kendoDatePicker').enable(false);
        $("#fechaHasta").data('kendoDatePicker').enable(false);
        $("#cantidad").data('kendoNumericTextBox').enable(false);        
        $("#zona").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        if (reasignarCupo == false) {
            $("#buscadorProveedor").attr('disabled', 'disabled');     
        }
        if ($("#flete").is(':checked')) {
            MensInfo("Cupo con condición de Flete");
        }
        $("#guardarBtn").attr('type', 'button');
        $("#guardarBtn").click(function () {
            if (fleteProcedencia) {
                if ($("#flete").is(':checked')) {
                    $("#msjConfirmacion").html("El cupo tiene flete procedencia, desea mantener esta condición?");
                } else {
                    $("#msjConfirmacion").html("El cupo no tiene flete procedencia, desea mantener esta condición?");
                }
                $('#fleteProcedenciaModal').modal('toggle');
            } else {
                $("#flete").removeAttr('disabled');
                $("form").submit();
            }
        });
        $("#boton-si").click(function () {
            $("#flete").removeAttr('disabled');
            if ($("#flete").is(':checked')) {
                $("#flete").prop('checked', true);
            } else {
                $("#flete").prop('checked', false);
            }
            $('#fleteProcedenciaModal').modal('toggle');
            $("form").submit();
        });
        $("#boton-no").click(function () {
            $("#flete").removeAttr('disabled');
            if ($("#flete").is(':checked')) {
                $("#flete").prop('checked', false);
            } else {
                $("#flete").prop('checked', true);
            }
            $('#fleteProcedenciaModal').modal('toggle');
            $("form").submit();
        });
        $("#boton-cancelar").click(function () {
            window.location.href = window.location.origin + "/Cupo/";
        });
    }
    $("#fleteProcedenciaModal").draggable({
        handle: ".modal-header"
    }); 

    if ($("#fechaHasta").val() != $("#fechaEntrega").val() ) {
        $("#boton-carga-masiva").show();
    }

    $("#contratoId").kendoAutoComplete({
        template: 
        '<p class="buscar-nomb" >#: data.TipoNegocio# - #: data.Negocio# KGs: #: data.Cantidad#</p>',
        dataTextField: "Negocio",
        dataValueField: "Id",
        autoWidth: true,
        filter: "contains",
        //change: function () {
        //    if ($("#contratoId").val().split('|').length > 1) {
        //        $("#contratoId").val($("#contratoId").val().split('|')[1]);
        //    }
        //},
        select: function (e) {
            $("#cantidad").data("kendoNumericTextBox").max(e.dataItem.CantidadMaximaCupo);
            $("#cantidad").data("kendoNumericTextBox").value(e.dataItem.CantidadMaximaCupo);
            $("#cantidad").data("kendoNumericTextBox").trigger("change");
            $("#Negocio").val(e.dataItem.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Cupo/TraerNegocioConCupoDisponible"
                },
                parameterMap: function (data, type) {
                    var cuitProv = $("#buscadorProveedor").val().split('(');
                    if (cuitProv[1] != null) {
                        var cuitP = cuitProv[1].split(')');
                    }
                    else {
                        cuitP = cuitProv;
                    }                   
                    return { cuitProveedor: cuitP[0], materialId: $('#material').val(), centro: $('#planta').val(), filtro: $('#contratoId').val(), desde: $('#fechaEntrega').val(), hasta: $('#fechaHasta').val()};
                }
            }

        }
    });
    $('#contratoId').click(function (e) {
        $('#contratoId').val("");
        $("#contratoId").data("kendoAutoComplete").search("");
    });
    $("#contratoId").on("keypress keyup blur", function (event) {
        $(this).val($(this).val().replace(/[^\d].+/, ""));
        if (event.which < 48 || event.which > 57) {
            event.preventDefault();
        }
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

        $("#cupos-generados-modal").html(lista.join("</br>"));
        $('#resultadoCupo').modal('toggle');

        $(".modal").on("hidden.bs.modal", function () {
            window.location.href = window.location.origin + "/Cupo/";
        });
    });
}

function avisoCuposCreados() {
    $(document).ready(function () {
       window.location.href = window.location.origin + "/Cupo/";         
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
    var listaCupos = $("#cupos-generados-modal").html().replace(/<br>/g, "\n");
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
function MostrarCarga() {
    $("#CargaCupos").modal('toggle');
}

function CrearTablaFechaHasta(){
    $(".fila-carga").remove();

    var date1 = $("#fechaEntrega").val();
    var date2 = $("#fechaHasta").val();
    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

    for (var i = 0; i <= diffDays; i++) {

        var fila = '<tr class="fila-carga"><input name="Dias[' + i + '].Fecha" value="' + date1 + '" type="hidden"/><td>' + date1 + '</td><td><input name="Dias['+i+'].Cantidad" class="cantidad-masiva" value="' + $("#cantidad").data('kendoNumericTextBox').value() + '"/></td></tr>';
        $("#carga-cupos-table").append(fila);
        var newdate = kendo.parseDate(date1);

        newdate.setDate(newdate.getDate() + 1); var dd = newdate.getDate();
        var mm = newdate.getMonth() + 1;
        var y = newdate.getFullYear();

        date1 = dd + '/' + mm + '/' + y;
    }
    $(".cantidad-masiva").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        spinners: false,
        min: 0
    });

    $("#cancelar-carga").click(function () {
        $(".cantidad-masiva").val($("#cantidad").data('kendoNumericTextBox').value());
        $("#cancelar-carga").unbind('click');
    });
    $('[name="Dias[0].Cantidad"]').change(function () {
        if ($("#cantidad").val() == 0) {
            $("#cantidad").data('kendoNumericTextBox').value($('[name="Dias[0].Cantidad"]').val());
            $("#cantidad").data("kendoNumericTextBox").trigger("change");
        }
    });
}
function ActualizarCantidad(cantidadDias) {
    $(document).ready(function () {
        for (var i=0; i < cantidadDias.length; i++) {
            $('[name="Dias[' + i + '].Cantidad"]').data('kendoNumericTextBox').value(cantidadDias[i].Cantidad);
        }
    });
}

function copiarTablaEstablecimiento() {
    var copiarEstablecimientos = document.getElementById("cargarDatosEstablecimiento").innerText;

    var copy = function (e) {
        e.preventDefault();
        console.log('copy');

        if (e.clipboardData) {
            e.clipboardData.setData('text/plain', copiarEstablecimientos);
        } else if (window.clipboardData) {
            window.clipboardData.setData('Text', copiarEstablecimientos);
        }
    };
    window.addEventListener('copy', copy);
    document.execCommand('copy');
    window.removeEventListener('copy', copy);
}

function VisualizarStock() {   

    var cuitProv = $("#buscadorProveedor").val().split('(');
    if (cuitProv[1] != null) {
        var cuitP = cuitProv[1].split(')');
    }
    else {
        cuitP = cuitProv;
    }  
    var result = MSExecuteOnServer('/Cupo/TraerEstablecimientos', { cuitProveedor: cuitP[0] });

    var table = "<tr>";   
    table += '<th colspan = "2">Cosecha ' + result[0].Cosecha + '</th>';
    table += "</tr>";
    table += "<tr>"; 
    table += "<th> Establecimiento</th>"
    table += "<th> Cantidad (Kg)</th>"
    table += "</tr>";
    for (var i = 0; i < result.length; i++) {
        table += "<tr>";   
   
        table += '<td>'+ result[i].Establecimiento +'</td>';   
        table += '<td>' + kendo.toString(result[i].Cantidad, "n0") + '</td>';   
        table += "</tr>";
    }        

    $("#cargarDatosEstablecimiento").html(table);
    $("#modalEstablecimientos").modal("show");
   
}

function MostrarVisualizarStock() {
    if ($("#buscadorProveedor").val() != "" && $("#planta").val() == "1600" && $("#material").val() == "3") {
        $("#stock").show();
    } else {
        $("#stock").hide();
    }    
}
