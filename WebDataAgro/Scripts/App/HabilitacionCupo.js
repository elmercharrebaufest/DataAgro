var cantidadZonas;
$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarElementos();
    CargarGrillaConfig();
});

function mostrarocultar(element) {
    if ($(element).text() == "Mostrar") {
        $(element).text("Ocultar");
    } else {
        $(element).text("Mostrar");
    }
}

$(".alert").ready(function () {
    setTimeout(function () { $(".alert").hide(); }, 5000);
});

function InicializarElementos() {
    kendo.culture("es-AR");;
    $("#FechaDesde").kendoDateTimePicker({
        value: new Date(),
        dateInput: true
    });
    $("#FechaHasta").kendoDateTimePicker({
        value: new Date(),
        dateInput: true
    });

    $("#desdeMes").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 1,
        max: 12
    });
    $("#hastaMes").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 1,
        max: 12
    });
    $("#desdeAnio").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 0
    });
    $("#hastaAnio").kendoNumericTextBox({
        culture: "es-AR",
        format: "#",
        spinners: false,
        min: 0
    });
  
    
}

function CargarGrillaConfig() {
    var ds = {
        transport: {
            read: {
                type: 'post',
                dataType: 'json',
                url: '/HabilitacionCupo/DatosConfiguracion'
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
                    Zona: { type: "string" },
                    Material: {type: "string"},
                    FechaDesde: { type: "date" },
                    FechaHasta: { type: "date"},
                }
            }
        },
        serverPaging: true,
        serverSorting: true,
        sort: [{ field: "FechaDesde", dir: "desc" }],
        serverFiltering: true,
        pageSize: 20
    };

    $("#gridHabilitacionCupo").kendoGrid({
        dataSource: ds,
        columns: [
            {
                field: "Zona", title: "Zona", type: "string", width: 150,
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
                field: "Material", title: "Cultivo", filterable: {
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
                }, width: 130, template: "#=Material#"
            },
            {
                field: "FechaDesde", title: "Fecha Desde", type: "date", format: _DefaultDateTemplate,
                template: "#= kendo.toString(kendo.parseDate(FechaDesde, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },

            {
                field: "FechaHasta", title: "Fecha Hasta", type: "date", format: _DefaultDateTemplate,
                 template: "#= kendo.toString(kendo.parseDate(FechaHasta, 'yyyy-MM-dd'), 'dd/MM/yyyy HH:mm') #"
            },
            
            {
                field: "Id", title: " ", filterable: false, sortable: false, width: 75, template: function (dataItem) { 
                    return '<a data-toggle="tooltip" title="Editar Configuracion"f class="abrirModalLimite links-grid" onclick="Editar(' + dataItem.Id + ')"><span> <i class="fa fa-pencil"></i> </span ></a >';
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
    });
}
function recargarGrilla() {
    $('#gridConfiguracionCupo').data('kendoGrid').dataSource.read();
}

function Editar(id) {
    var cupo = MSExecuteOnServer("/HabilitacionCupo/EditarHabilitacionCupo", { id: id });
    $("#Id").val(cupo.Id);
    $("#ZonaCupoId").val(cupo.ZonaCupoId);  
    $("#MaterialId").val(cupo.MaterialId);  
    var fechaDesde = kendo.toString(kendo.parseDate(cupo.FechaDesde), "dd-MM-yyyy"); 
    var fechaHasta = kendo.toString(kendo.parseDate(cupo.FechaHasta), "dd-MM-yyyy"); 
    $("#FechaDesde").val(fechaDesde);
    $("#FechaHasta").val(fechaHasta);
}

