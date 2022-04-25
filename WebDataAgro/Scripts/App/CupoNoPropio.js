var _DefaultDateTemplate = "{0:dd/MM/yyyy tt}";


$(document).ready(function () {

    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    inicializarTodosKendoDate($(".filtroFecha"));

    CreateGridReporteCupoNoPropio();
    //InicializarElementos();
    InicializarElementos();
    //CompletarTable();  

});

function InicializarElementos() {
    $("#fechaIngresoId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    $('#ContratoSAPId').click(function (e) {
        $('#ContratoSAPId').val("");
        CompletarTable();
    });
    $("#ContratoSAPId").bind("paste", function (e) {
        // access the clipboard using the api
        setTimeout(
            CompletarTable
            , 50)
    });
    $("#fechaEntrega").kendoDatePicker({
        value: kendo.parseDate(new Date()),
        min: kendo.parseDate(new Date()),
        change: function () {

        }
    });
    $('#ModalCrearCupoNoPropio').on('hidden.bs.modal', function () {
        BorrarContenidoModal();
    })
    $("#ContratoSAPId").on("keypress keyup blur", function (event) {
        var valor = $(this).val();
        $(this).val(valor.replace(' ', ''))
        if (!valor.split(';')[0]) {
            $(this).val(valor.replace(';', ''))
        }

    });
    inicializarPopUpCodigo("CuposNoPropio");
}

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}


function Filtrar() {
    $('#gridCupoNoPropio').data('kendoGrid').dataSource.read();
}

function CreateGridReporteCupoNoPropio() {
    kendo.ui.FilterMultiCheck.prototype.options.messages =
        $.extend(true, kendo.ui.FilterMultiCheck.prototype.options.messages, {
            "selectedItemsFormat": ""
        });

    var ds = {
        transport: {
            parameterMap: function (options, operation) {

                if (operation == "read") {
                    return JSON.stringify(options)
                }
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;

            },
            read: {
                type: 'post',
                dataType: 'json',
                contentType: "application/json",
                url: '/CupoNoPropio/BuscaDatosTabla',

                data: function () {

                    let filtroCompleto = TraerFiltrosConValores();

                    return filtroCompleto;
                }
            }

        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Centro: { type: "string" },
                    Material: { type: "string" },
                    FechaIngreso: { type: "date" },
                    Codigo: { type: "string" },
                    Estado: { type: "string" },
                    Disponible: { type: "boolean" },
                    Utilizado: { type: "boolean" }
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        //sort: [
        //    { field: "Material", dir: "desc" }
        //],

        pageSize: 10,
    };

    $("#gridCupoNoPropio").kendoGrid({
        toolbar: kendo.template($("#templateToolbar").html()),
        //toolbar: ["excel"],
        excel: {
            fileName: "Reporte Cupos no Propios.xlsx",
            allPages: true,
        },
        dataSource: ds,
        dataBound: function () {
            $("td:has(div.statuspendiente)").css('border-bottom', '5px solid #ffc100');
            $("td:has(div.statusconfirmado)").css('border-bottom', '5px solid #179e2b');
            $("td:has(div.statusoferta)").css('border-bottom', '5px solid #00adf5');
            $("td:has(div.statuserror)").css('border-bottom', '5px solid #d00707');
            $("td:has(div.statusfinalizado)").css('border-bottom', '5px solid #000000');
            $("td:has(div.statusborrado)").css('border-bottom', '5px solid #848484');
            $("td:has(div.statuspreaprobacion)").css('border-bottom', '5px solid #15deca');
            $("td:has(div.statuspreanulado)").css('border-bottom', 'border-grey');
            $("td:has(div.statusreconfirmarfinalizado)").css('border-bottom', '5px solid #ac67ca');
        },
        columns: [
            {
                field: "Centro", title: "Planta", width: 150, template: function (dataItem) {
                    return dataItem.Centro;
                }
            },
            {
                field: "Material", title: "Material", width: 130, template: "#=Material#"
            },
            {
                field: "FechaIngreso", title: "Fecha de Ingreso", width: 120, format: _DefaultDateTemplate
            },

            {
                field: "Codigo", title: "Cupo", width: 120
            },
            {
                field: "Estado", title: "Estado", width: 100
            },
            {
                field: "Disponible", type: "string", title: "Disponible", width: 80, template: function (dataItem) {
                    var disponible = dataItem.Disponible ? "checked" : "";
                    var utilizado = dataItem.CupoId != null ? " disabled" : ""
                    return "<label class='content-input' style='cursor:pointer;'><input" + utilizado + " id='idDisponible" + dataItem.Id + "' onclick='ModificarDisponible(" + dataItem.Id + ")' type='checkbox'" + disponible + "><i></i></label>";
                }
            },
            {
                field: "Utilizado", type: "string", title: "Utilizado", width: 80, template: function (dataItem) {
                    return dataItem.CupoId != null ? "Si" : "No";
                }
            }
        ],
        excelExport: function (e) {
            var sheet = e.workbook.sheets[0];
            for (var i = 0; i < sheet.rows[0].cells.length; i++) {
                sheet.rows[0].cells[i].value = sheet.rows[0].cells[i].value.replace("<br>", "").replace("<br>", "").replace("<br>", "");
            }
            //for (var i = 0; i < sheet.rows[1].cells.length; i++) {
            //    sheet.rows[1].cells[i].value = sheet.rows[1].cells[i].value.replace("<br>", "").replace("<br>", "").replace("<br>", "");
            //}

            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];


                var fechaDesde = row.cells[2].value;

                if (fechaDesde != null) {
                    fechaDesde.setHours(fechaDesde.getHours() + 1);
                    row.cells[2].value = fechaDesde;
                }

                row.cells[5].value = row.cells[5].value == true ? "Si" : "No";
                row.cells[6].value = (e.data[i - 1].CupoId == null || e.data[i - 1].CupoId == undefined) ? "No" : "Si";


            }
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
        scrollable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
        selectable: "row",
        height: 550,
        filterable: false,

    });

}

function ModificarDisponible(id) {

    //var grid = $("#gridCupoNoPropio").data("kendoGrid").dataSource.data();
    var disponible = $("#idDisponible" + id).is(":checked") ?? null;
    result = MSExecuteOnServer('/CupoNoPropio/ModificarDisponible', { id: id, disponible: disponible });

    $.unblockUI();
    var errores = new Array();
    if (result.HayError) {
        errores = errores.concat(result.ListaErrores);
    }
    if (errores.length == 0) {
        MensInfo("Se grabo correctamente.");
    } else {
        ShowErrorMessages(errores);
    }
    Filtrar();


}
function AbrirModalCrearCupo() {
    $('#ModalCrearCupoNoPropio').modal('show');
    BorrarContenidoModal();
}

function CompletarTable() {
    $("#ContratoSAPId").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#ContratoSAPId").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

    });
    CambioVariosCupos();
    $('#cupos-table').find('tr.seleccionado').addClass('hide');
    $('#cupos-table').find('td').find('button').addClass('hide');
    $('#cupos-table').find('td.contador').addClass('show');
}

function CambioVariosCupos() {
    if
        (parseInt($("#ContratoSAPHastaId").val()) - parseInt($("#ContratoSAPId").val()) > 1000) {
        PopUpError("Seleccione un rango de valores menor a 1000");
        return;
    }
    var lista = [];
    lista = $("#ContratoSAPId").val().split(';');
    lista = lista.filter(function (value, index, arr) {
        return value.length > 0;
    });
    if (lista.length > 1) {
        $("#ContratoSAPHastaId").attr('disabled', 'disabled');
        $("#ContratoSAPHastaId").val("")
        ArmarTablaConEstado(lista);
    } else if ($.isNumeric($("#ContratoSAPId").val()) && $.isNumeric($("#ContratoSAPHastaId").val())) {
        $("#ContratoSAPHastaId").removeAttr('disabled');
        lista = [];
        for (var i = parseInt($("#ContratoSAPId").val()); i <= parseInt($("#ContratoSAPHastaId").val()); i++) {
            lista.push(i);
        }
        ArmarTablaConEstado(lista);
    }
    else {
        $("#ContratoSAPHastaId").removeAttr('disabled');
        ArmarTablaConEstado(lista);
    }
}
function ArmarTablaConEstado(contratos) {
    $("#cupos-table").empty();
    var html = '<tr class="seleccionado hide" ><th ></th></tr>';

    if (contratos) {
        html += "<div id='div1'><tbody>";
        for (var i = 0; i < contratos.length; i++) {

            if (contratos[i] != "") {
                html += '<tr>';
                html += '<td style="width:40px; text-align: center">' + (i + 1) + '</td>';
                html += "<td>" + (contratos[i].Codigo != undefined ? contratos[i].Codigo : contratos[i]) + "</td>";
                if (contratos[i].EstadoId != undefined) {
                    html += (contratos[i].EstadoId == 1 ?
                        "<td style='width: 40px; text-align:center'><i  style='color:red' class='fa fa-times' aria-hidden='true'></i></td>" : "<td style='width: 40px; text-align: center'><i style='color:green' class='fa fa-check' aria-hidden='true'></i></td>");
                }
                html += "</tr > ";

            }

        }
        html += "</tbody>";
        html += "</div>";
    }
    $("#cupos-table").append(html);


    html += "</table>";
}


//function ArmarTablaConEstado(contratos) {
//    $("#contratos-table").empty();
//    var tabla = '<tr class="seleccionado" ><th ></th></tr>';
//    if (contratos) {
//        tabla += "<div id='div1'>"
//        for (var i = 0; i < contratos.length; i++) {
//            if (contratos[i] != "") {
//                tabla += '<tr><td class="hide contador" style="width: 40px; text-align: center">' + (i + 1) + '</td><td style="width: 100%">' + contratos[i].Codigo + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr>';
//            }
//        }
//        tabla += "</div>";
//    }
//    $("#contratos-table").append(tabla);
//}



function GrabarCupo() {
    $(".errorDiv").hide();
    BlockUi('Guardando...');
    var cupoDto = {
        MaterialId: $("#material").val(),
        Centro: $("#planta").val(),
        CentroId: $("#planta").val(),
        Codigo: $("#ContratoSAPId").val(),
        FechaIngreso: $("#fechaEntrega").val(),
    }
    var resultado = MSExecuteOnServer('/CupoNoPropio/CrearCupo', { cupoDto });
    if (resultado.HayError) {
        if (resultado.CupoNoPropios != null && resultado.CupoNoPropios.length > 0) {
            $(".errorDiv").show();
            $("#error").text(resultado.ListaErrores[0].Message);
            ArmarTablaConEstado(resultado.CupoNoPropios);
        } else {
            $(".errorDiv").show();
            $("#error").text(resultado.ListaErrores[0].Message);
        }
    } else {
        MensInfo("Los cupos se grabaron correctamente");
        $('#ModalCrearCupoNoPropio').modal('hide');
        $(".errorDiv").hide();


    }

    $.unblockUI();
    setTimeout(recargarGrilla, 200);

}
function recargarGrilla() {
    $('#gridCupoNoPropio').data('kendoGrid').dataSource.read();
}

function BorrarContenidoModal() {
    $("#ContratoSAPId").val("");
    $("#cupos-table").empty();
    $(".errorDiv").hide();
    $("#material").val("3")
    $("#fechaEntrega").data("kendoDatePicker").value(kendo.parseDate(new Date()));
    $("#planta").val("1074");
}


// +++++++++++++++    QUERYS DE FILTRO +++++++++++++++++++++++++++++++

function inicializarPopUpCodigo(nombrePopUp) {

    crearPopUpCodigo(nombrePopUp);

    ModalCodigo();


    $("#Codigo").bind("paste", function (e) {
        e.preventDefault();
        if (e.originalEvent.clipboardData !== undefined) {
            clipText = e.originalEvent.clipboardData.getData('text/plain');
        } else {
            clipText = window.clipboardData.getData('text');
        }
        $("#Codigo").val(clipText.replace(/(\r\n|\n|\r)/gm, ";"));

        CambioVariosCodigo();
    });



    $("#Codigo").change(CambioVariosCodigo);

    $(document).on("click", ".agregarContrato", function () {
        var num = $("#nuevoNumCodigo").val();
        //if ($.isNumeric(num)) {
        if (num.trim() != "" && num.trim() != null) {
            $("#contratos-tableCodigo").append('<tr><td>' + num + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr></td></tr>');
            $(".nuevoNumCodigo").val('');
            $(".nuevoNumCodigo").focus();
            GenerarCodigoDesde();
        }

        //}
    });

    $(document).on("click", ".borrarContrato", function () {
        $(this).parent().parent().remove();
        GenerarCodigoDesde();
    });
}

function CambioVariosCodigo() {

    var lista = [];
    lista = $("#Codigo").val().split(';');
    if (lista.length >= 1) {

        ArmarTablaCodigo(lista);
    }
}

function GenerarCodigoDesde() {
    var arr = "";
    $("#contratos-tableCodigo tr").each(function () {
        if (arr != "") {
            arr += ";";
        }
        arr += $(this).find("td:first").text(); //put elements into array
    });
    $("#Codigo").val(arr);
}

function ArmarTablaCodigo(contratos) {
    $("#contratos-tableCodigo").empty();
    var tabla = '<tr class="seleccionado" ><th >Seleccionados:</th></tr>';
    if (contratos) {
        tabla += "<div id='divCodigoTabla'>"
        for (var i = 0; i < contratos.length; i++) {
            if (contratos[i] != "") {
                tabla += '<tr><td style="width: 100%">' + contratos[i] + '<button class="k-button k-button-icontext fa fa-trash borrarContrato" style="height: 34px;float: right" type="button"></button></td></tr>';
            }
        }
        tabla += "</div>";
    }
    $("#contratos-tableCodigo").append(tabla);
}


function ModalCodigo() {
    $("#abrirPopUpCargarValores").click(function () {
        $("#popUpCargarValoresCodigo").modal('toggle');
    });
}

function crearPopUpCodigo(nombrePopUp) {

    if ($("#popUpCargarValoresCodigo").children().length == 0) {

        $("#popUpCargarValoresCodigo").prepend('<div class="modal-dialog" id="01" role="document"></div>');
        $("#01").prepend('<div id="02" class="modal-content">');

        $("#02").prepend('<div id="04" class="modal-body"></div>');
        $("#02").prepend('<div id="03" class="modal-header"></div>');

        //tabla
        $("#04").prepend('<div id="07" class="table-responsive">');
        $("#07").prepend('<table id="contratos-tableCodigo" class="table table-striped"></table>');

        //body
        $("#07").prepend('<div id="08" class="row">');
        $("#08").prepend('<div id="010" class="col-xs-2">');
        $("#08").prepend('<div id="09" class="col-xs-4"></div>');

        $("#010").prepend('<button class="k-button k-button-icontext fa fa-plus agregarContrato" style="height: 34px;" type="button"></button>');
        $("#09").prepend('<input type="text" id="nuevoNumCodigo" class="nuevoNumCodigo">');

        //header
        $("#05").prepend('<span aria-hidden="true">&times;</span>');
        $("#03").prepend('<div id="06" class="status confirmado-modal"></div>');
        $("#06").prepend('<div>Seleccione ' + nombrePopUp + '</div>');
        $("#03").prepend('<button id="05" type="button" class="cerrar close" data-dismiss="modal" aria-label="Close"></button>');
    }
}