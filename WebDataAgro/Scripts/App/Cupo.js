var viewModel;
var fecha;
var zonaSeleccionada;
var anularCupo;
var modificarCupo;
var filasSeleccionadas = {};
var modificarCentro;
var modificarOtrasZonas;
var modificarSur;
var modificarNorte;
var modificarRosario;
var modificarBsAs;
var anularCentro;
var anularOtrasZonas;
var anularSur;
var anularNorte;
var anularRosario;
var anularBsAs;
var externo;
var materialDisponible;


$(document).ready(function () {
    anularCupo = ConvertirStringABool(anularCupo);
    modificarCupo = ConvertirStringABool(modificarCupo);
    modificarCentro = ConvertirStringABool(modificarCentro);
    modificarOtrasZonas = ConvertirStringABool(modificarOtrasZonas);;
    modificarSur = ConvertirStringABool(modificarSur);;
    modificarNorte = ConvertirStringABool(modificarNorte);;
    modificarRosario = ConvertirStringABool(modificarRosario);
    modificarBsAs = ConvertirStringABool(modificarBsAs);
    anularCupo = ConvertirStringABool(anularCupo);
    anularCentro = ConvertirStringABool(anularCentro);
    anularOtrasZonas = ConvertirStringABool(anularOtrasZonas);;
    anularSur = ConvertirStringABool(anularSur);;
    anularNorte = ConvertirStringABool(anularNorte);;
    anularRosario = ConvertirStringABool(anularRosario);
    anularBsAs = ConvertirStringABool(anularBsAs);
    externo = ConvertirStringABool(externo);
    materialDisponible = ConvertirStringABool(materialDisponible);
    kendo.culture("es-AR");
    $('#menuproveedor').hide();
    $("#demo").on("hide.bs.collapse", function () {
        $(".btn").html('<span class="icono"><i class="fa fa-plus-square-o" aria-hidden="true"></i></span>');
    });
    $("#demo").on("show.bs.collapse", function () {
        $(".btn").html('<span class="icono"><i class="fa fa-minus-square-o" aria-hidden="true"></i></span>');
    });
    if (externo) {
        HabilitarAltaCupoExterno();
    }
    $("#crearCupo").click(function () {
        window.location.href = window.location.origin + "/Cupo/CrearCupo";
    });

    $.unblockUI();

    InicializarCuposIndex();
    AutoRecargar();


    reordenarPorEstadoCupo();

   


});


function InicializarCuposIndex() {

    var estados = new Array();
       estados = [
            { EstadoCupo: "Sin CTG" },
            { EstadoCupo: "Activado" },
            { EstadoCupo: "Arribado" },
            { EstadoCupo: "Descargado" },
            { EstadoCupo: "Anulado" },
            { EstadoCupo: "Disponible" },
            { EstadoCupo: "Sin STOP" },
           { EstadoCupo: "Error STOP" },
           { EstadoCupo: "Rechazado" } ];

    var estadoExterno = new Array();
    estadoExterno = [
        { EstadoCupo: "Pendiente" },
        { EstadoCupo: "Activado" },
        { EstadoCupo: "Arribado" },
        { EstadoCupo: "Descargado" },
        { EstadoCupo: "Aceptado" },
        { EstadoCupo: "Anulado" },
        { EstadoCupo: "Disponible" },
        { EstadoCupo: "Error STOP" },
        { EstadoCupo: "Rechazado" } ];


    var defaultFilter = [
        { field: "FechaIngreso", operator: "gte", value: new Date()},
    ];
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
                url: '/Cupo/BuscaDatosTabla',
                data: function (filtrosEnviados) {

                    if (filtrosEnviados.filter == null) {
                        filtrosEnviados.filter = new FiltroPadre("and", defaultFilter);
                    }

                    //let fechaRegistroFiltro = filtrosEnviados.filter.filters.find(function (f) { return f.field == "FechaRegistro" });

                    //if (fechaRegistroFiltro != null) {

                    //    fechaRegistroFiltro.value = deFechaAString(fechaRegistroFiltro.value);
                    //}
                }
            }
        },
        schema: {
            data: 'Data',
            total: 'Total',
            model: {
                id: 'Id',
                fields: {
                    FechaIngreso: { type: "date" },
                    //FechaGeneracion: { type: "date" },
                    FechaRegistro: { type: "date" },
                    FleteProcedencia: { type: "boolean" }
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [
            { field: "FechaIngreso", dir: "asc" },
            { field: "EstadoOrden", dir: "asc" }

        ],
        serverFiltering: true,
        pageSize: 20
        //,
        //filter: defaultFilter
    };

    $("#gridCupo").kendoGrid({
        dataSource: ds,
        dataBound: function () {           
            $("td:has(div.statusexterno)").attr('id', 'border-turquoise');
            var grid = $("#gridCupo").data("kendoGrid");
            if (externo) {
                grid.hideColumn("ZonaCupo");
            }  
            var view = grid.dataSource.view();
            for (var i = 0; i < view.length; i++) {
                if (view[i].FleteProcedencia) {
                    grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                        .addClass("flete-procedencia");
                }                
                for (var j = 0; j < filasSeleccionadas.length; j++) {
                    if (filasSeleccionadas[j].Id == view[i].Id) {
                        grid.tbody.find("tr[data-uid='" + view[i].uid + "']")
                            .addClass("k-state-selected")
                            .find(".k-checkbox")
                            .prop('checked', true);
                    }
                }
                $('[data-toggle="tooltip"]').tooltip();
            }
        },
        sortable: true,
        columns: [
            { selectable: true },
            {
                field: "FechaIngreso", title: externo ? "Fecha de Cupo": "Fecha de ingreso", type: "date", width: 150, format: _DefaultDateTemplate,
                template: function (dataItem) {
                    if (dataItem.UsuarioCreador != null) {
                        return '<div class="statusexterno "></div>' + kendo.toString(dataItem.FechaIngreso, "dd/MM/yyyy");
                    } else {
                        return kendo.toString(dataItem.FechaIngreso, "dd/MM/yyyy");
                    }
                }

            },
            

            { field: "CupoSap", title: "Cupo", type: "string", width: 150 },
            {
                field: "Material", type: "string", width: 150,

                filterable: {
                    multi: true,
                    dataSource: [
                        { MaterialNombre: "Maiz" },
                        { MaterialNombre: "Trigo" },
                        { MaterialNombre: "Soja" },
                        { MaterialNombre: "Girasol" },
                        { MaterialNombre: "Girasol AO" }],
                    itemTemplate: function (e) {
                        return "<span><label><input type='checkbox' name='" + e.field + "' value='#= data.MaterialNombre#'/><span>#= data.MaterialNombre|| data.all #</span></label></span><br>";
                    }
                },
            },
            { field: "Proveedor", type: "string", width: 150, filterable: { ui: createMultiSelectProveedor } },
            { field: "Destinatario", type: "string", width: 150 },
            { field: "Centro", type: "string", width: 150 },
            { field: "Calidad", type: "string", width: 150 },
            //{ field: "FechaGeneracion", title: "Fecha de registro", type: "date", width: 50, format: _DefaultDateTemplate },
            { field: "FechaRegistro", title: "Fecha de Registro", type: "date", width: 50, format: _DefaultDateTemplate },
            { field: "Hora", title: "Hora", type: "date", width: 150 },
            {
                field: "ZonaCupo", title: "Zona", type: "string", width: 150,
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
                        //return "<span><label><input type='checkbox' name='" + e.field + "' value='#= data.codigoSap == 0 ? null : data.codigoSap #'/><span class='multiFilter'>#= data.ZonaCupo || data.all #</span></label></span><br>";
                    }
                },
            },
            {
                field: "FleteProcedencia", title: "Flete", type: "string", width: 150, template: function (dataItem) { return dataItem.FleteProcedencia ? "Si" : "No"; }
            },
            { field: "Observaciones", type: "string", width: 150, hidden: externo },
            { field: "Comercial", type: "string", width: 150, filterable: { ui: createMultiSelectComercial } },
            { field: "EstadoOrden", type: "number", hidden: true },
            {
                field: "EstadoCupo", title: "Estado",
                filterable: {
                    multi: true,
                    dataSource: externo ? estadoExterno : estados
                },

                width: 200,
                itemTemplate: function (e) {
                    return "<span><label><span>#= data.EstadoCupo|| data.all #</span><input type='checkbox' name='" + e.field + "' value='#= data.EstadoCupo#'/></label></span>";
                },
                template: function (dataItem) {


                    if (dataItem.EstadoCupoId == 1) {

                        return '<div class="status sinctg"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonModificar(dataItem, 'fa-pencil ctg') +
                            botonBorrar(dataItem, 'fa-trash ctg');
                    }
                    else if (dataItem.EstadoCupoId == 8) {
                        return '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonModificar(dataItem, 'fa-pencil sto') +
                            botonBorrar(dataItem, 'fa-trash sto');
                    }
                    else if (dataItem.EstadoCupoId == 2) {
                        return '<div class="status activado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 3) {
                        return '<div class="status arribado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 5) {
                        return '<div class="status descargado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    } else if (dataItem.EstadoCupoId == 4) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    }
                    else if (dataItem.EstadoCupoId == 6) {
                        var descripcion = "";
                        if (dataItem.UsuarioCreador != null) {

                            if (dataItem.CentroId == 1) {
                                descripcion = '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                                    botonRechazarCupo(dataItem, 'fa-trash sto') +
                                    botonAceptarCupo(dataItem, 'fa-check pend sto')
                            } else {
                                 descripcion = '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                                    botonAceptarCupo(dataItem, 'fa-check pend sto')
                            }
                         
                        } else {
                            descripcion = '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                                botonModificar(dataItem, 'fa-pencil sto') +
                                botonBorrar(dataItem, 'fa-trash sto') +
                                botonRetransmitir(dataItem, 'fa-mail-forward sto')
                        }

                        if (externo) {
                            descripcion = '<div class="status sinstop"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                        }

                        return descripcion;
                    } else if (dataItem.EstadoCupoId == 7) {
                        return '<div class="status error" data-toggle="tooltip" data-placement="top" title="' + dataItem.MensajeError + '"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>' +
                            botonBorrar(dataItem, 'fa-trash err') +
                            botonRetransmitir(dataItem, 'fa-mail-forward err');

                    } else if (dataItem.EstadoCupoId == 9) {
                        return '<div class="status anulado"><span style:"display:inline-block;">' + dataItem.EstadoCupo + '</span></div>';
                    }

                }
            },


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
            checkAll: true,
            height: 350,
            extra: false,
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
                    eq: "Igual",
                    neq: "Distinto",
                    startswith: "Comienza con",
                    contains: "Contiene",
                    endswith: "Finaliza con"
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
        filterMenuInit: function (e) {
            if (e.field == "Proveedor" || e.field == "Comercial") {
                $(e.container).css("width", "300px");
            }
        }
    });



    var checkInputs = function (elements) {
        elements.each(function () {
            var element = $(this);
            var input = element.children("input");

            input.prop("checked", element.hasClass("k-state-selected"));
        });
    };

    function createMultiSelect(element, textField, valueField, url) {
        element.removeAttr("data-bind");

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
                }
            },
            change: function (e) {
                var items = this.ul.find("li");
                checkInputs(items);
                var grilla = $('#gridCupo').data("kendoGrid");
                var values = this.value();
                $.each(values, function (i, v) {
                    if (v !== '') {
                        addOrRemoveFilter(grilla, valueField, "eq", v);
                    }
                });

                if (values.length === 0) {
                    addOrRemoveFilter(grilla, valueField, "eq", "");
                }
            }
        });
        setTimeout(function () {
            $(".k-multiselect").parent().children(".k-dropdown").remove();
            $(".k-multiselect").parent().children("div").find('button').remove();
        }, 200);
    }
    function createMultiSelectProveedor(element) {
        return createMultiSelect(element, "Proveedor", "Proveedor", "/Cupo/ListarProveedorTodos");
    }
    function createMultiSelectComercial(element) {
        return createMultiSelect(element, "Comercial", "Comercial", "/Cupo/ListarComercial");
    }

    function addOrRemoveFilter(grid, field, operator, value) {

        var newFilter = { field: field, operator: operator, value: value };
        var dataSource = grid.dataSource;
        var filters = null;
        if (dataSource.filter() != null) {
            filters = dataSource.filter().filters;
        }

        if (value && value.length > 0) {
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

    $("#finalizarBorrar").click(function () {
        ObtenerDatosModalBorrado();
    });
}
function botonBorrar(dataItem, icono) {
    if (anularCupo) {
        return '<button data-toggle="tooltip" title="Anular" onclick="ModalBorrar(' +
            "'" + dataItem.Id + "'" + ',' +
            "'" + dataItem.CupoSap + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return "<div></div>";
    }
}

function botonRechazarCupo(dataItem, icono) {
    if (anularCupo) {
        return '<button data-toggle="tooltip" title="Rechazar" onclick="ModalRechazar(' +
            "'" + dataItem.Id + "'" + ',' +
            "'" + dataItem.CupoSap + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else {
        return "<div></div>";
    }
}

function botonRetransmitir(dataItem, icono) {
    if (!dataItem.Acopio) {
        return '<button data-toggle="tooltip" title="Transmitir a STOP" onclick="Retransmitir(' +
            "'" + dataItem.CupoSap + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else if (anularCentro == true && dataItem.ZonaCupoSap == "OIC" ||
        anularNorte == true && dataItem.ZonaCupoSap == "OIN" ||
        anularRosario == true && dataItem.ZonaCupoSap == "CRO" ||
        anularBsAs == true && dataItem.ZonaCupoSap == "CBA" ||
        anularSur == true && dataItem.ZonaCupoSap == "OIS" ||
        anularOtrasZonas == true && dataItem.ZonaCupoSap == "FAS" ||
        anularOtrasZonas == true && dataItem.ZonaCupoSap == "MAT" ||
        anularOtrasZonas == true && dataItem.ZonaCupoSap == "PPR" ||
        anularOtrasZonas == true && dataItem.ZonaCupoSap == "RED" ||
        anularOtrasZonas == true && dataItem.ZonaCupoSap == "SOL") {
        return '<button data-toggle="tooltip" title="Transmitir a STOP" onclick="Retransmitir(' +
            "'" + dataItem.CupoSap + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    }
    else {
        return '<div></div>';
    }

}

function botonAceptarCupo(dataItem, icono) {
    if (!dataItem.Acopio) {
        return '<button data-toggle="tooltip" title="Aprobar" onclick="AceptarCupo(' +
            "'" + dataItem.Id + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    }
    else {
        return '<div></div>';
    }

}
function botonModificar(dataItem, icono) {
    if (modificarCupo) {
        return '<button data-toggle="tooltip" title="Modificar Cupo" onclick="ModificarCupo(' +
            "'" + dataItem.Id + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else if (modificarCentro == true && dataItem.ZonaCupoSap == "OIC" ||
        modificarNorte == true && dataItem.ZonaCupoSap == "OIN" ||       
        modificarRosario == true && dataItem.ZonaCupoSap == "CRO" ||
        modificarBsAs == true && dataItem.ZonaCupoSap == "CBA" ||
        modificarSur == true && dataItem.ZonaCupoSap == "OIS" ||
        modificarOtrasZonas == true && dataItem.ZonaCupoSap == "FAS" ||
        modificarOtrasZonas == true && dataItem.ZonaCupoSap == "MAT" ||
        modificarOtrasZonas == true && dataItem.ZonaCupoSap == "PPR" ||
        modificarOtrasZonas == true && dataItem.ZonaCupoSap == "RED" ||
        modificarOtrasZonas == true && dataItem.ZonaCupoSap == "SOL") {
        return '<button data-toggle="tooltip" title="Modificar Cupo" onclick="ModificarCupo(' +
            "'" + dataItem.Id + "'" +
            ')"><i class="fa  ' + icono + '" aria-hidden="true"></i></button>';
    } else { return "<div></div>"; }
}
function ModalBorrar(id, cupo) {
    $("#cupo_a_borrar").text(cupo);
    $("#cupoBorrar").val(id);

    $("#modalBorrar").modal('show');
}
function RetransmitirSeleccionados() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if ((selectedItem.EstadoCupoId == 6 || selectedItem.EstadoCupoId == 7) && selectedItem.CentroId == 1 && selectedItem.UsuarioCreador == null) {
            obj.push(selectedItem.CupoSap);
        }
    });
    Retransmitir(obj);
}

function CopiarSeleccionados() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if (selectedItem.EstadoCupoId != 4)
            obj.push(selectedItem.CupoSap);
    });
    var listaCupos = obj.join("\n");
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

function Retransmitir(listaCupos) {
    var a = [];
    if (!Array.isArray(listaCupos)) {
        a.push(listaCupos);
    } else {
        for (var i = 0; i < listaCupos.length; i++) {
            a.push(listaCupos[i]);
        }
    }
    if (a.length > 0) {
        var result = MSExecuteOnServer('/Cupo/TransmitirCupos', { cupos: a });

        if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            recargarGrilla();
        }
    } else {
        MensErr("Seleccione cupos válidos");
    }
}

function ObtenerDatosModalBorrado() {
    var objBorrar = {};
    var result;
    objBorrar.id = $("#cupoBorrar").val();
    result = MSExecuteOnServer('/Cupo/EliminarCupo', objBorrar);
    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}
function recargarGrilla() {
    $('#gridCupo').data('kendoGrid').dataSource.read();
}

function ModificarCupo(cupoId) {
    window.location.href = window.location.origin + "/Cupo/CrearCupo?id=" + cupoId;
}

function AnularSeleccionados() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    var obj = [];
    var borrar = [];
    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if ((selectedItem.EstadoCupoId == 1 || selectedItem.EstadoCupoId == 6 || selectedItem.EstadoCupoId == 7 ||
            selectedItem.EstadoCupoId == 8) && (anularCentro && selectedItem.ZonaCupoSap == "OIC" ||
            anularNorte && selectedItem.ZonaCupoSap == "OIN" ||
            anularRosario && selectedItem.ZonaCupoSap == "CRO" ||
            anularBsAs && selectedItem.ZonaCupoSap == "CBA" ||
            anularSur && selectedItem.ZonaCupoSap == "OIS" ||
            anularOtrasZonas && selectedItem.ZonaCupoSap == "FAS" ||
            anularOtrasZonas && selectedItem.ZonaCupoSap == "MAT" ||
            anularOtrasZonas && selectedItem.ZonaCupoSap == "PPR" ||
            anularOtrasZonas && selectedItem.ZonaCupoSap == "RED" ||
            anularOtrasZonas && selectedItem.ZonaCupoSap == "SOL" || anularCupo)) {
            obj.push(selectedItem.CupoSap);
            borrar.push(selectedItem.Id);
        }
    });
    var result;
    if (obj.length == 0) {
        $("#cupos-seleccionados-a-borrar").text("No hay cupos seleccionados");
        $("#BorrarVarios").hide();
    } else {
        $("#cupos-seleccionados-a-borrar").text(obj.join(", "));
        $("#BorrarVarios").show();
        $("#BorrarVarios").click(function () {
            result = MSExecuteOnServer('/Cupo/EliminarVarios', { listaCupos: borrar });
            $("#modalBorrarVarios").modal('toggle');
            recargarGrilla();
            ErrorAnulacion(result);
        });
    }

    $("#modalBorrarVarios").on("hidden.bs.modal", function () {
        $("#BorrarVarios").unbind('click');
    });
    $("#modalBorrarVarios").modal('toggle');
}
function ErrorAnulacion(resultado) {
    if (resultado != null) {
        if (ExistsErrorMessages(resultado.Errores)) {
            MensErr(makeUL(resultado.Errores));
        }
        else {
            MensInfo("Anulación exitosa");
        }
    }
}
function makeUL(array) {
    var list = document.createElement('ul');
    for (var i = 0; i < array.length; i++) {
        var item = document.createElement('li');
        item.appendChild(document.createTextNode(array[i].Message));
        list.appendChild(item);
    }
    return list;
}

function ModificarSeleccionados() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    var obj = [];
    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        if ((selectedItem.EstadoCupoId == 1 || selectedItem.EstadoCupoId == 6 || selectedItem.EstadoCupoId == 8) &&
            (modificarCentro && selectedItem.ZonaCupoSap == "OIC" ||
            modificarNorte  && selectedItem.ZonaCupoSap == "OIN" ||
            modificarRosario  && selectedItem.ZonaCupoSap == "CRO" ||
            modificarBsAs && selectedItem.ZonaCupoSap == "CBA" ||
            modificarSur && selectedItem.ZonaCupoSap == "OIS" ||
            modificarOtrasZonas && selectedItem.ZonaCupoSap == "FAS" ||
            modificarOtrasZonas && selectedItem.ZonaCupoSap == "MAT" ||
            modificarOtrasZonas && selectedItem.ZonaCupoSap == "PPR" ||
            modificarOtrasZonas && selectedItem.ZonaCupoSap == "RED" ||
            modificarOtrasZonas && selectedItem.ZonaCupoSap == "SOL" || modificarCupo))
            obj.push(selectedItem.Id);
    });
    if (obj.length == 0) {
        $("#texto").text("Error");
        $("#cupos-seleccionados-a-borrar").text("No hay cupos seleccionados");
        $("#BorrarVarios").hide();
        $("#modalBorrarVarios").on("hidden.bs.modal", function () {
            $("#BorrarVarios").unbind('click');
        });
        $("#modalBorrarVarios").modal('toggle');
    } else {
        var siguientes = obj.slice(1);
        window.location.href = window.location.origin + "/Cupo/CrearCupo?id=" + obj[0] + (siguientes != undefined ? "&siguientes=" + JSON.stringify(siguientes) : "");

    }

}
function AutoRecargar() {
    setInterval(function () {
        filasSeleccionadas = SeleccionarElementos();
        recargarGrilla();
    }, 30000);

}

function SeleccionarElementos() {
    var grid = $("#gridCupo").data("kendoGrid");
    var selectedRows = grid.select();
    obj = [];

    selectedRows.each(function (index, row) {
        var selectedItem = grid.dataItem(row);
        obj.push(selectedItem);
    });
    return obj;
}

function reordenarPorEstadoCupo() {

    let grid = $("#gridCupo").data("kendoGrid");

    grid.bind("sort", function (e) {

        $.each($(".k-link"), function (numeroDeColumna) {
            if ($(".k-link")[numeroDeColumna].innerText == "EstadoOrden") {
                $(".k-link")[numeroDeColumna].click();
            }
        });
    });
}

function filtrarMesa() {
    //FILTRO MANUAL
    var grilla = $('#gridCupo').data("kendoGrid");
    if (!$("#cupoPropiosDiv").hasClass("selected")) {
        var currentFilters =  { filters: [], logic: 'and' };        
        grilla.dataSource.filter(currentFilters);
        var fecha = new Date();
        var ayer = new Date(fecha.getTime() - 24 * 60 * 60 * 1000);
        addOrRemoveFilter(grilla, "FechaIngreso", "gte", ayer);
        addOrRemoveFilter(grilla, "ComercialId", "eq", parseInt(comercialId));
        $("#cupoPropiosDiv").addClass("selected");
        $("#cupoPropio").addClass("selected").removeClass("varios");        
    } else {
        addOrRemoveFilter(grilla, "FechaIngreso", "gte", "");
        addOrRemoveFilter(grilla, "ComercialId", "eq", "");
        $("#cupoPropiosDiv").removeClass("selected");
        $("#cupoPropio").addClass("varios").removeClass("selected");
    }
    recargarGrilla();   
}

function addOrRemoveFilter(grid, field, operator, value) {

    var newFilter = { field: field, operator: operator, value: value };
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }

    if (value && (value.length > 0 || value != undefined)) {
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
function HabilitarAltaCupoExterno (){
    $("#crearCupo").hide()
    var habilitaciones = MSExecuteOnServer('/Cupo/TraerTodasHabilitacionesActivas');
    if (materialDisponible) {
        $("#crearCupo").show();

    } 
        var habilitacionesPorMaterial = MSExecuteOnServer('/Cupo/TraerTodoMaterialRetirado');

        var table = '<tr>';
        for (var i = 0; i < habilitacionesPorMaterial.length; i++) {
            table += '<th class="col-xs-2">' + habilitacionesPorMaterial[i].DescripcionMaterial + '</th>';
        }
        table += '</tr>';
        for (i = 0; i < habilitacionesPorMaterial.length; i++) {
            table += '<td>';
            table += '<span class="precio">' + habilitacionesPorMaterial[i].Descripcion; + '</span><br/>';
            table += '</td>';
        }
        $("#tabla-cupo").html(table);


}

function ModalRechazarCupoConMotivo() {
    var motivoRechazo = $("#motivo-rechazo").val();
    var objConfirmado = {};
    objConfirmado.MotivoRechazo = motivoRechazo;
    if (motivoRechazo == null || motivoRechazo == "" || motivoRechazo == undefined) {
        MensErr("Ingrese un motivo de rechazo");
        return;
    } else {
        if (motivoRechazo.length > 1000) {
            MensErr("El motivo de rechazo es demasiado largo.");
            return;
        }
    }
    var result;
    objConfirmado.Id = $("#cupoId").val();
    result = MSExecuteOnServer('/Cupo/RechazarCupo', objConfirmado);
       

    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }
}

function ModalRechazar(id) {
    
    $("#cupoId").val(id);
    $("#motivo-rechazo").val("");
    $("#motivo-rechazo").show();
    $("#modalRechazarCupo").modal('show');
}

function AceptarCupo(id) {
    var objConfirmado = {};
    objConfirmado.Id = id;
    result = MSExecuteOnServer('/Cupo/AceptarCupo', objConfirmado);


    if (result != null && result.Errores != null && ExistsErrorMessages(result.Errores)) {
        MensErr(result.Errores[0].Message);
    }
    else {
        recargarGrilla();
    }

}

function Filtrar() {
    var grid = $('#gridCupo').data('kendoGrid');
    var currentFilters = grid.dataSource.filter();
    let filtroSap = TraerFiltrosConValores();
    currentFilters = { filters: [], logic: 'and' };
    if (filtroSap.filter != null) {
        //-----------------------------------------
        currentFilters.filters = currentFilters.filters.filter(function (x) {
            return x.field != 'CupoSap' && x.field != undefined
        });
        var contratoSapFilters = { logic: 'and', filters: [] };

        contratoSapFilters.filters.push({ field: 'CupoSap', operator: 'contains', value: filtroSap.filter.filters[0].value });
        currentFilters.filters.push(contratoSapFilters);
        grid.dataSource.filter(currentFilters);
    } else {
        BorrarFiltro();
    }
}


function BorrarFiltro() {
    $("#CupoSAPId").val("");
    var grid = $('#gridCupo').data('kendoGrid');
    var dataSource = grid.dataSource;
    var filters = null;
    if (dataSource.filter() != null) {
        filters = dataSource.filter().filters;
    }
    //Remove filter 
    var removeIndex = -1;
    if (filters != null) {
        for (var x = 0; x < filters.length; x++) {
            var temp = filters[x];
            if (temp.filters != undefined) {

                for (var i = 0; i < temp.filters.length; i++) {
                    if (temp.filters[i].field == 'CupoSap') {
                        removeIndex = x;
                        break;
                    }
                }
                break;
            }
        }
        if (removeIndex != -1)
            filters.splice(removeIndex, 1);

    }
    dataSource.filter(filters);
}
  
   

