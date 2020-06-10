$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    CargarGrillaConfig();
    CargarEventos();
   

});
$(document).on('click', '.list > li ', function () {
    $(this).next('ul').toggle(200);
    if (($(this).next('ul').length)) {
        $(this).toggleClass('multi-opened');
    }
})
function CargarEventos() {
    $("#btnCancelar").click(function () {
        $("#motivoRechazo").val("");
        $("#ModalRolPermiso").modal('hide');
        recargarGrilla();
    });

    $("#btnRechazar").click(function () {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');
        if (grid.selectedKeyNames().length == 0) {
            MensErr("Debe seleccionar al menos una sugerencia.");

        } else {
            $("#ModalRechazo").modal('show');          
            
        }

    });
    $("#btnAceptarRechazar").click(function () {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');
        var motivo = $.trim($("#motivoRechazo").val());
        if (motivo == "") {
            MensErr("Ingrese un motivo de Rechazo");
            return false;
        }
        if (motivo.length > 500) {
            MensErr("El texto ingresado es demasiado largo");
            return false;
        }
        BlockUi("Grabando...");
        result = MSExecuteOnServer('/SugerenciaCupo/Rechazar', { ids: grid.selectedKeyNames(), motivo: motivo });
        grid._selectedIds = {};
        grid.clearSelection();
        grid.dataSource.read();
        $("#ModalRechazo").modal('hide');
        $.unblockUI();
        var errores = new Array();
        for (var i = 0; i < result.length; i++) {
            if (result[i].HayError) {
                errores = errores.concat(result[i].ListaErrores);
            }
        }

        if (errores.length == 0) {
            MensInfo("Se grabo correctamente.");
        } else {
            //alert(errores.join());
            ShowErrorMessages(errores);
        }
    });


    function ListarSugerencias() {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');

        var list = new Array();
        for (var i = 0; i < grid.dataSource.data().length; i++) {
            var item = grid.dataSource.data()[i];
            for (var j = 0; j < grid.selectedKeyNames().length; j++) {
                if (grid.selectedKeyNames()[j] == item.Id) {
                    list.push(item);
                    break;
                }
            }
        }
        return list;
    }

    $("#btnAceptar").click(function () {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');
        if (grid.selectedKeyNames().length == 0) {
            MensErr("Debe seleccionar al menos una sugerencia.");

        }else {
            $("#fleteProcedenciaModal").modal("show");
        }

    });
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
    });

    $("#boton-si").click(function () {
        var sugerencias = ListarSugerencias();
        var fila = '';
        for (var i = 0; i < sugerencias.length; i++) {
            fila = '<tr><td>' + sugerencias[i].ProveedorDesc + '</td> <td>'
                + kendo.toString(sugerencias[i].FechaSugerida, "dd/MM/yyyy")
                + '</td> <td><input id="fleteProcedencia' + i + '" name="' + sugerencias[i].CantidadFleteProcedencia + '" min="1" max="' + sugerencias[i].CantidadDeCupos
                + '" class="cantidad" value="' + sugerencias[i].CantidadFleteProcedencia + '"/> </td> <td>'
                + 'Max. de cupos: ' + sugerencias[i].CantidadDeCupos + '</td></tr>'
            $("#cuerpo-carga-cupos").append(fila);
        }
        $(".cantidad").kendoNumericTextBox({
            culture: "es-AR",
            format: "n0",
            spinners: false,
            min: 0
        });
        $("#fleteProcedenciaModal").modal("hide");
        $("#CargaCupos").modal("show");       
    });

    $("#aceptar").click(function () {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');
        var sugerencias = ListarSugerencias();
        for (var j = 0; j < sugerencias.length; j++) {
            sugerencias[j].CantidadFleteProcedencia = $("#fleteProcedencia" + j).val();
            sugerencias[j].CantidadDeCupos = sugerencias[j].CantidadDeCupos - sugerencias[j].CantidadFleteProcedencia;

        }
        BlockUi("Grabando...");
        result = MSExecuteOnServer('/SugerenciaCupo/Aceptar', sugerencias);// grid.selectedKeyNames());
        $("#CargaCupos").modal("hide");

        $.unblockUI();
        var errores = new Array();
        var cuposGenerados = new Array();
        for (var i = 0; i < result.length; i++) {
            if (result[i].HayError) {
                errores = errores.concat(result[i].ListaErrores);
            }
            if (result[i].ListaCupos != null && result[i].ListaCupos.length > 0) {
                cuposGenerados = cuposGenerados.concat(result[i].ListaCupos);
            }
        }
        if (cuposGenerados.length > 0) {
            cuposCreados(cuposGenerados);
            grid._selectedIds = {};
            grid.clearSelection();
            grid.dataSource.read();
        }
        if (errores.length > 0) {
            ShowErrorMessages(errores);
        }
    });

    $("#boton-no").click(function () {
        var grid = $('#gridSugerenciaCupo').data('kendoGrid');
        $("#fleteProcedenciaModal").modal("hide");
        BlockUi("Grabando...");
        var lista = ListarSugerencias();
        result = MSExecuteOnServer('/SugerenciaCupo/Aceptar', lista);
        $.unblockUI();
        var error = new Array();
        var cupos = new Array();
        for (var i = 0; i < result.length; i++) {
            if (result[i].HayError) {
                error = error.concat(result[i].ListaErrores);
            }
            if (result[i].ListaCupos != null && result[i].ListaCupos.length > 0) {
                cupos = cupos.concat(result[i].ListaCupos);
            }
        }
        if (cupos.length > 0) {
            //MensInfo("Cupos generados: " + cuposGenerados.join());
            cuposCreados(cupos);
            grid._selectedIds = {};
            grid.clearSelection();
            grid.dataSource.read();
        }
        if (error.length > 0) {
            ShowErrorMessages(error);
        }
    });
}

function cuposCreados(lista) {
    $("#cupos-generados-modal").html(lista.join("</br>"));
    $('#resultadoCupo').modal('toggle');

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

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/SugerenciaCupo/DatosConfiguracion'
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
                    Id: { type: "number", editable: false },
                    PuntuacionTotal: { type: "number", editable: false },
                    FechaSugerida: { type: "date", editable: false },
                    MaterialDesc: { type: "string", editable: false },
                    TipoNegocioDesc: { type: "string", editable: false },
                    Precio: { type: "number", editable: false },
                    MonedaId: { type: "string", editable: false },
                    ProveedorCUIT: { type: "number", editable: false },
                    ProveedorDesc: { type: "string", editable: false },
                    ContratoSAP: { type: "string", editable: false },
                    ZonaDescrip: { type: "string", editable: false },
                    CantidadDeCupos: { type: "number", editable: true, validation: { required: true, min: 1 } },
                    CantidadDeCuposMax: { type: "number", editable: false },
                }
            }
        },
        serverPaging: false,
        serverSorting: false,
        sort: [{ field: "PuntuacionTotal", dir: "desc" }],
        serverFiltering: false,
        pageSize: 20
    };

    $("#gridSugerenciaCupo").kendoGrid({
        dataSource: ds,
        columns: [
            //{ field: "Centro", type: "string" },
            { selectable: true, width: "40px", title: "Todos" },
            { field: "MaterialDesc", title: "Material", width: "150px" },
            { field: "FechaSugerida", type: "date", format: _DefaultDateTemplate, title: "Fecha <br> Sugerida", width: "110px" },
            { field: "ProveedorCUIT", title: "CUIT", width: "100px" },
            { field: "ProveedorDesc", title: "Razon Social" },
            { field: "MonedaId", title: "Moneda", width: "88px"},            
            { field: "Precio", width: "88px", format: "{0:n0}" },
            { field: "TipoNegocioDesc", title: "Negocio", width: "100px" },
            { field: "ContratoSAP", title: "ContratoSAP", width: "125px" },
            {
                field: "CantidadDeCupos", title: "Cantidad <br> de Cupos", width: "110px",
                editor: function (container, options) {
                    // create an input element
                    var input = $("<input name='" + options.field + "'/>");
                    // append it to the container
                    input.appendTo(container);
                    // initialize a Kendo UI numeric text box and set max value
                    input.kendoNumericTextBox({
                        max: options.model.CantidadDeCuposMaximo,
                        min: 1
                    });
                }
            },
            { field: "PuntuacionTotal", title: "Puntuacion", width: "120px" },
            //{
            //    field: "Id", title: " ", filterable: false, sortable: false, width: 75, template: function (dataItem) {
            //        return '<a data-toggle="tooltip" title="Ver Detalle" class="abrirModalLimite links-grid" onclick="VerDetalle(' + dataItem.Id + ')">' +
            //            '<span> <i class="fa fa-list"></i> </span ></a >';
            //    }
            //},
        ],
        persistSelection: true,
        //pageable: {
        //    numeric: false,
        //    previousNext: false,
        //    messages: {
        //        display: "Mostrando {2}  items"
        //    }
        //},
        pageable: true,
        groupable: true,
        //scrollable: {
        //    virtual: true
        //},
        editable: true,
        sortable: {
            mode: "multiple",
            allowUnsort: true,
            showIndexes: false
        },
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
                    eq: "Igual"
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
        }
    });

    $("#gridSugerenciaCupo").kendoTooltip({
        filter: "td:nth-child(11)",
        //filter: "td", 
        position: "left",
        content: function (e) {
            var dataItem = $("#gridSugerenciaCupo").data("kendoGrid").dataItem(e.target.closest("tr"));
            var datos = JSON.stringify(dataItem.Puntuaciones, undefined, 4).replace(/,/gi, '<br>').replace(/"/gi, '').replace(/{/gi, '').replace(/}/gi, '');
            var datos2 = JSON.stringify(dataItem.Puntuaciones, undefined, 4).replace(/,/gi, ',').replace(/"/gi, '').replace(/{/gi, '').replace(/}/gi, '');
            datos2 = datos2.split(",");
            var result = "<div style='text-align: left;'>";
            
            result = result + datos + "</div>";
            console.log(result);
            return result;
        }
    }).data("kendoTooltip");
}

function recargarGrilla() {
    $('#gridSugerenciaCupo').data('kendoGrid').dataSource.read();
}



