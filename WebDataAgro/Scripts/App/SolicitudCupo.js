var externo;
$(document).ready(function () {
    $('#menuproveedor').hide();
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    })

    CargarGrilla();
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
    });
    AutoRecargarSolicitudes();
    InicializarElementos();
});
function CargarGrilla() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/AdministracionCupo/BuscarDatosSolicitudCupo',
                data: additionalInfo()
            },
            parameterMap: function (options, operation) {
                if (options.filter) {
                    KendoGrid_FixFilter(ds, options.filter);
                }
                return options;
            },
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    Fecha: { type: "date" },
                    LimiteCupo: { type: "number" },
                    Proveedor: { type: "string" },
                    Comercial: { type: "string" },
                    Material: { type: "string" },
                    Zona: { type: "string" },
                    Centro: { type: "string" },
                    Fecha: { type: "date" },
                    Estado: { type: "string" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "EstadoId", dir: "desc" }, { field: "Fecha", dir: "asc" }],
        serverFiltering: true,
        pageSize: 20,
    };
    var classExterno = externo ? "hide" : "";
    $("#gridInformeCompraNet").kendoGrid({
        dataSource: ds,
        parameterMap: function (options, operation) {

            if (operation == "read") {
                return JSON.stringify(options)
            }
            if (options.filter) {
                KendoGrid_FixFilter(ds, options.filter);
            }
            return options;
        },
        dataBound: function () {
            $("td:has(div.statuspendiente)").attr('id', 'border-orange');
            $("td:has(div.statusconfirmado)").attr('id', 'border-green');
            $("td:has(div.statuseliminado)").attr('id', 'border-grey');
        },
        columns: [
            {
                field: "Proveedor", type: "string", width: 150,
                editable: function (dataItem) {
                    return false;
                },
                headerAttributes: { "class": classExterno }, attributes: { "id": "line", "class": classExterno },
                template: function (dataItem) {
                    if (dataItem.EstadoId == 3) {
                        return '<div class="statuspendiente "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 1) {
                        return '<div class="statusconfirmado "></div>' + dataItem.Proveedor;
                    } else if (dataItem.EstadoId == 2) {
                        return '<div class="statuseliminado "></div>' + dataItem.Proveedor;
                    }
                },
                filterable: { ui: createMultiSelectProveedor }
            },
            {
                field: "Comercial", type: "string", title: "Comercial", width: 70, editable: function (dataItem) {
                    return false;
                }, filterable: { ui: createMultiSelectComercial }, headerAttributes: {
                    "class": classExterno
                },
                attributes: { "class": "mobile-xs " + classExterno }
            },
            {
                field: "Material", type: "string", editable: function (dataItem) {
                    return false;
                }, filterable: {
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
                }, width: 95, attributes: {
                    "class": "mobile-xs"
                }, itemTemplate: function (e) {
                    return "<span><label><span>#= data.Material|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.Material#'/></label></span>";
                }, template: "#=Material#"
            },
            {
                field: "Zona", title: "Zona", type: "string", editable: function (dataItem) {
                    return false;
                }, width: 150,
                filterable: {
                    multi: true,

                    dataSource: [
                        { ZonaCupo: "CORREDOR BS AS" },
                        { ZonaCupo: "CORREDOR ROSARIO" },
                        { ZonaCupo: "Fasones CAGSA/MOLCA, YPF y AMAGGI" },
                        { ZonaCupo: "MAT-ROFEX" },
                        { ZonaCupo: "ORIG INTERIOR CENTRO" },
                        { ZonaCupo: "ORIG INTERIOR NORTE" },
                        { ZonaCupo: "ORIG INTERIOR SUR" },
                        { ZonaCupo: "PRODUCCION PROPIA" },
                        { ZonaCupo: "REDESPACHOS" },
                        { ZonaCupo: "SOLIDARIDAD" }],
                    itemTemplate: function (e) {

                        return "<span><label><input type='checkbox' name='" + e.field + "' value='#= data.ZonaCupo#'/><span>#= data.ZonaCupo|| data.all #</span></label></span><br>";
                    }
                },
            },
            {
                field: "Centro", type: "string", title: "Destino", editable: function (dataItem) {
                    return false;
                }, attributes: { "class": "mobile-xs mobile-md" }
            },
            {
                field: "Fecha", title: "Fecha Solicitud", type: "date", editable: function (dataItem) {
                    return false;
                }, format: _DefaultDateTemplate
            },
            {
                field: "CantidadDeCupo", title: "Cantidad de Cupos", width: "110px",

            },
            {
                field: "CantidadFleteProcedencia", title: "Cantidad Flete Procedencia",
            },
            {
                field: "Estado", title: "Estado", editable: function (dataItem) {
                    return false;
                },
                filterable: {
                    multi: true,

                    dataSource: [
                        { Estado: "Aceptado" },
                        { Estado: "Rechazado" },
                        { Estado: "Pendiente" }]
                },
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.EstadoId || data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.EstadoId#'/></label></span>";
                }, template: function (dataItem) {
                    if (dataItem.EstadoId == 3) { //pendiente
                        return '<div class="status pendiente">Pendiente</div>';
                    }
                    if (dataItem.EstadoId == 1) { //confirmado                       
                        return '<div class="status confirmado">Confirmado</div>';
                    }
                    if (dataItem.EstadoId == 2) { //Rechazado
                        return '<div class="status borrado">Rechazado</div>';
                    }
                }
            }
        ],
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

        //,
        //filter: defaultFilter
    });
    var fecha = new Date();
    var grilla = $('#gridInformeCompraNet').data("kendoGrid");
    addOrRemoveFilter(grilla, "Fecha", "gte", fecha);
    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };
    function createMultiSelect(element, textField, valueField, url, columna) {
        element.removeAttr("data-bind");
        columna = columna == null ? valueField : columna;
        console.log(columna);
        element.kendoMultiSelect({
            itemTemplate: "<input type='checkbox'/> #:data." + textField + "#",
            dataBound: function () {
                var items = this.ul.find("li");
                setTimeout(function () {
                    checkInputs(items);
                });
            },

            dataTextField: textField,
            dataValueField: valueField,
            autoClose: false,
            autoBind: false,
            delay: 300,
            dataSource: {
                serverFiltering: true,
                filter: [],
                transport: {
                    read: {
                        url: url,
                        data: function () {
                            return {
                                text: element.data("kendoMultiSelect").input.val()
                            };
                        },
                        prefix: ""
                    }
                },
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);
                var grilla = $('#gridInformeCompraNet').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, columna, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, columna, "eq", null);
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }

    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/CompraNet/ListarProveedor");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "ComercialId", "/CompraNet/ListarComercial");
    }
}


function addOrRemoveFilter(grid, field, operator, value) {

    var newFilter = { field: field, operator: operator, value: value };
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }

    if (value != null && (value.length > 0 || value != undefined)) {
        //Add filter
        if (filters == null) {
            filters = [newFilter];
        }
        else {
            var isNew = true;
            var index = 0;
            for (index = 0; index < filters.length; index++) {
                if (filters[index].field == field) {
                    isNew = false;
                    break;
                }
            }
            if (isNew) {
                filters.push(newFilter);
            }
            else {
                filters[index] = newFilter;
            }
        }
    }
    else {
        //Remove filter 
        var removeIndex = -1;
        if (filters != null) {
            for (var x = 0; x < filters.length; x++) {
                var temp = filters[x];
                if (temp.field == field) {
                    removeIndex = x;
                    break;
                }
            }
            if (removeIndex != -1)
                filters.splice(removeIndex, 1);
        }
    }
    dataSource.filter(filters);
}

function recargarAceptar() {
    recargarGrilla();
}
function recargarGrilla() {
    $('#gridInformeCompraNet').data('kendoGrid').dataSource.read();
}
function AutoRecargarSolicitudes() {
    setInterval(function () {
        if (document.getElementById('checkRecarga').checked == true) {
            recargarGrilla();
        }
    }, 30000);

}

function InicializarElementos() {
    $("#CalidadIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value"
    });
    $("#MaterialIdSE").kendoDropDownList({
        change: function () {
            checkSoja();
            MostrarVisualizarStock();
        }
    });
    $("#CentroIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        change: function () {
            MostrarVisualizarStock();
        }
    });


    $("#ComercialIdSE").kendoDropDownList({
        dataTextField: "Text",
        dataValueField: "Value",
        filter: "contains",
        popup: {
            appendTo: $("#modalSolicitudExtraordinaria"),
            //origin: "bottom right"
        }
    });

    $("#FechaSE").kendoDatePicker({
        min: new Date()
    });


    $("#MaterialIdSE").change(function () {
        checkSoja();
    });
}
function LimpiarModalSolicitudExtraordinaria() {
    //$("#CalidadIdSE").data("kendoDropDownList").value('');
    //$("#MaterialIdSE").data("kendoDropDownList").value('');
    $("#buscadorProveedorSE").click();
    $("#ProveedorIdSE").val('');
    $("#FechaSE").val('');
    //$("#ComercialIdSE").data("kendoDropDownList").value('');
    $("#ObservacionSE").val('');
    $("#CantidadCupoSE").data("kendoNumericTextBox").value('');
    $("#CuitES").val('');
    $("#FasonES").prop("checked", false);
    $("#FleteAcarreoES").prop("checked", false);
    checkSoja();
}

function AbrirModalSolicitudExtraordinaria() {
    LimpiarModalSolicitudExtraordinaria();
    $("#modalSolicitudExtraordinaria").modal("show");
}

function checkSoja() {
    if ($("#MaterialIdSE").val() !== "3") {
        $("#calidadDivSE").hide();
        $("#CalidadIdSE").data("kendoDropDownList").value("");
    } else {
        $("#calidadDivSE").show();
    }
}

function grabarSolicitudExtraordinaria() {
    if ($("#MaterialIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Material"); return;
    }
    if ($("#CentroIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Destino"); return;
    }
    if ($("#MaterialIdSE").data("kendoDropDownList").value() == "3" && $("#CalidadIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione la Calidad"); return;
    }
    if ($("#FechaSE").data("kendoDatePicker").value() == null) {
        MensErr("Seleccione la Fecha"); return;
    }
    if ($("#ProveedorIdSE").val() == "") {
        $("#buscadorProveedorSE").click();
        MensErr("Seleccione un Proveedor"); return;
    }
    if ($("#ObservacionSE").val().length > 500) {
        MensErr("La Observacion es muy larga. 500 caracteres maximo"); return;
    }
    if ($("#ComercialIdSE").data("kendoDropDownList").value() == "") {
        MensErr("Seleccione el Comercial"); return;
    }
    if ($("#CantidadCupoSE").data("kendoNumericTextBox").value() == null || $("#CantidadCupoSE").data("kendoNumericTextBox").value() == 0) {
        MensErr("Ingrese la Cantidad"); return;
    }
    if ($("#FasonES").is(':checked') && $("#CuitES").val() == "") {
        MensErr("Complete el CUIT del destinatario"); return;
    }
    
    BlockUi('Grabando...');
    setTimeout(function () {
        var solicitud = {
            ProveedorId: $("#ProveedorIdSE").val(),
            ComercialId: $("#ComercialIdSE").data("kendoDropDownList").value(),
            Fecha: $("#FechaSE").data("kendoDatePicker").value(),
            MaterialId: $("#MaterialIdSE").data("kendoDropDownList").value(),
            CantidadCupo: $("#FleteAcarreoES").is(':checked') ? 0 : $("#CantidadCupoSE").data("kendoNumericTextBox").value(),
            CantidadFleteProcedencia: $("#FleteAcarreoES").is(':checked') ? $("#CantidadCupoSE").data("kendoNumericTextBox").value() : 0,
            TipoAdministracionCupoId: 2,
            Fason: $("#FasonES").is(':checked'),
            Destinatario: $("#CuitES").val(),
            Observacion: $("#ObservacionSE").val(),
            CentroId: $("#CentroIdSE").data("kendoDropDownList").value(),
            Calidad: $("#MaterialIdSE").data("kendoDropDownList").value() == 3 ? $("#CalidadIdSE").data("kendoDropDownList").text() : ""
        };
        result = MSExecuteOnServer('/SugerenciaCupo/GenerarSolicitudExtraordinaria', solicitud);
        ListarRespuesta(result);

        $.unblockUI();
    }, 250);
}
function ListarRespuesta(result) {
    $.unblockUI();
    var erroresTabla = new Array();
    var cuposGeneradosTabla = new Array();

    if (result.ListaCupos != null && result.ListaCupos.length > 0) {
        cuposGeneradosTabla = cuposGeneradosTabla.concat(result.ListaCupos);
        $("#modalSolicitudExtraordinaria").modal("hide");
    }

    else if (result.ListaErrores != null && result.ListaErrores.length > 0) {
        for (var i = 0; i < result.ListaErrores.length; i++) {
            cuposGeneradosTabla = cuposGeneradosTabla.concat(result.ListaErrores[i].Message);
            //$("#modalSolicitudExtraordinaria").modal("hide");
        }

    } else {
        $("#modalSolicitudExtraordinaria").modal("hide");
        MensInfo("La solicitud se genero correctamente.");
        //click panel de solicitudes y reresh de grilla
    }
    if (result.ListaCupos != null && result.ListaCupos.length == 0 &&
        result.ListaErrores != null && result.ListaErrores.length > 0) {
        MensErr(result.ListaErrores[0].Message);
    } else {
        if (cuposGeneradosTabla.length > 0) {
            cuposCreados(cuposGeneradosTabla);
        }
    }
    //if (result.HayError && cuposGeneradosTabla.length < 0) {
    //    for (var i = 0; i < result.ListaErrores.length; i++) {
    //        erroresTabla = erroresTabla.concat(result.ListaErrores[i].Message);
    //    }

    //}
    //if (erroresTabla.length > 0 && cuposGeneradosTabla.length < 0) {
    //    ShowErrorMessages(erroresTabla);
    //}
}

function checkFason() {
    if ($("#FasonES").is(':checked')) {
        $("#CuitES").show();
        var cuitAux = $("#buscadorProveedorSE").val().split('(');
        if (cuitAux[1] != null) {
            var cuit = cuitAux[1].split(')');
        }
        else {
            cuit = cuitAux;
        }
        $("#CuitES").val(cuit[0]);
    }
    else {
        $("#CuitES").hide();
        $("#CuitES").val("");
    }
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

    var cuitProv = $("#buscadorProveedorSE").val().split('(');
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

        table += '<td>' + result[i].Establecimiento + '</td>';
        table += '<td>' + kendo.toString(result[i].Cantidad, "n0") + '</td>';
        table += "</tr>";
    }

    $("#cargarDatosEstablecimiento").html(table);
    $("#modalEstablecimientos").modal("show");

}

function MostrarVisualizarStock() {
    if ($("#buscadorProveedorSE").val() != "" && $("#CentroIdSE").val() == "1600" && $("#MaterialIdSE").val() == "3") {
        $("#stock").show();
    } else {
        $("#stock").hide();
    }
}