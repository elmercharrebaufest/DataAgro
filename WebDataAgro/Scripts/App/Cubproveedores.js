var viewModel;
var datosIniCubProveedores;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });

    InicializarElementos();

    CrearPivotGrid();

    CrearConfigurator();

    CrearViewModel();

    AssignTabEvents();

    InicializarDatos();
});

function InicializarElementos() {
    kendo.culture("es-AR");

    $("#proveedorId").kendoDropDownList({
        optionLabel: "SELECCIONE UN PROVEEDOR...",
        dataTextField: "RazonSocial",
        dataValueField: "ProveedorId"
    });

    $("#proveedorId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#proveedorId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#provinciaId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA PROVINCIA...",
        dataTextField: "Nombre",
        dataValueField: "ProvinciaId"
    });

    $("#provinciaId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#provinciaId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#LocalidadId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA LOCALIDAD...",
        dataTextField: "Nombre",
        dataValueField: "LocalidadId"
    });

    $("#LocalidadId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#LocalidadId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#EstadoId").kendoDropDownList({
        optionLabel: "SELECCIONE UN ESTADO...",
        dataTextField: "Descripcion",
        dataValueField: "EstadoId"
    });

    $("#EstadoId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#EstadoId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#SegmentacionId").kendoDropDownList({
        optionLabel: "SELECCIONE UNA SEGMENTACION...",
        dataTextField: "Descripcion",
        dataValueField: "SegmentacionId"
    });

    $("#SegmentacionId").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#SegmentacionId").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#material").kendoDropDownList({
        optionLabel: "SELECCIONE UN MATERIAL...",
        dataTextField: "Descripcion",
        dataValueField: "MaterialId"
    });

    $("#material").closest('.k-dropdown.k-widget').keydown(function (e) {
        if (e.keyCode == 46) {
            var dropdownlist = $("#material").data("kendoDropDownList");
            dropdownlist.text("");
        }
    });

    $("#butAceptar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Aceptar.png")
    });

    $("#butAceptar").click(function () {
        ReporteListar();
    });

    $("#butCancelar").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Cancelar.png")
    });

    $("#butCancelar").click(function () {
        Cancelar();
    });

    $("#butExcelExport").kendoButton({
        imageUrl: MSGetUrl("/Content/Images/Excel.png")
    });

    $("#butExcelExport").click(function () {
        ExportarAExcel();
    });
}

function CrearPivotGridDataSource(datos) {
    var dataSource = new kendo.data.PivotDataSource({
        data: datos,
        schema: {
            model: {
                fields: {
                    CUIT: { type: "string" },
                    RazonSocial: { type: "string" },
                    Sergmentacion: { type: "string" },
                    Estado: { type: "string" },
                    ContactoApellido: { type: "string" },
                    AreaDeInfluencia: { type: "string" },
                    MaterialCampaña: { type: "string" },
                    Campaña: { type: "string" },
                    ToneladasCompradas: { type: "number" },
                    ToneladasObjetivo: { type: "number" },
                    CampañaCampo: { type: "string" },
                    MaterialCampo: { type: "string" },
                    HectCampo: { type: "number" },
                    TonCampo: { type: "number" },
                    LocalidadCampo: { type: "string" },
                    ProvinciaCampo: { type: "string" },
                    CampañaAcopio: { type: "string" },
                    PorcentajeAcopio: { type: "number" },
                    TonAcopio: { type: "number" },
                    LocalidadAcopio: { type: "string" },
                    ProvinciaAcopio: { type: "string" },
                }
            },
            cube: {
                dimensions: {
                    CUIT: { caption: "CUIT" },
                    RazonSocial: { caption: "Razón Social" },
                    Sergmentacion: { caption: "Sergmentación" },
                    Estado: { caption: "Estado" },
                    ContactoApellido: { caption: "Contacto" },
                    AreaDeInfluencia: { caption: "Area de Influencia" },
                    MaterialCampaña: { caption: "Material Campaña" },
                    Campaña: { caption: "Campaña" },
                    ToneladasCompradas: { caption: "Tn. Compradas" },
                    ToneladasObjetivo: { caption: "Tn. Objetivo" },
                    CampañaCampo: { caption: "Campaña Campo" },
                    MaterialCampo: { caption: "Material Campo" },
                    HectCampo: { caption: "Hectareas Campo" },
                    TonCampo: { caption: "Toneladas Campo" },
                    LocalidadCampo: { caption: "Localidad Campo" },
                    ProvinciaCampo: { caption: "Provincia Campo" },
                    CampañaAcopio: { caption: "Campaña Acopio" },
                    PorcentajeAcopio: { caption: "% Acopio" },
                    TonAcopio: { caption: "Toneladas Acopio" },
                    LocalidadAcopio: { caption: "Localidad Acopio" },
                    ProvinciaAcopio: { caption: "Provincia Acopio" },
                },
                measures: {
                    "Hectareas Campo": { field: "HectCampo", format: "{0:n0}", aggregate: "sum" },
                    "Toneladas Campo": { field: "TonCampo", format: "{0:n0}", aggregate: "sum" },
                    "% Acopio": { field: "PorcentajeAcopio", format: "{0:n2}", aggregate: "sum" },
                    "Toneladas Acopio": { field: "TonAcopio", format: "{0:n0}", aggregate: "sum" },
                    "Tn. Compradas": { field: "ToneladasCompradas", format: "{0:n0}", aggregate: "sum" },
                    "Tn. Objetivo": { field: "ToneladasObjetivo", format: "{0:n0}", aggregate: "sum" },

                    "% Cumplimiento": {
                        field: "PorcentCumpl",
                        format: "{0:n2}",
                        aggregate: function (value, state, context) {
                            var dataItem = context.dataItem;
                            var ToneladasCompradas = dataItem.ToneladasCompradas;
                            var ToneladasObjetivo = dataItem.ToneladasObjetivo;

                            state.ToneladasCompradas = (state.ToneladasCompradas || 0) + ToneladasCompradas;
                            state.ToneladasObjetivo = (state.ToneladasObjetivo || 0) + ToneladasObjetivo;
                        },
                        result: function (state) {
                            if (state.ToneladasCompradas == 0 || state.ToneladasObjetivo == 0) {
                                return 0;
                            } else {
                                return ((state.ToneladasCompradas * 100) / state.ToneladasObjetivo);
                            }
                        }
                    },
                }
            }
        },

        columns: [{ name: "MaterialCampaña", expand: true }, { name: "Campaña", expand: true }],

        rows: [{ name: "CUIT", expand: true },
        { name: "RazonSocial", expand: false },
        { name: "Estado", expand: false }],

        //measures: ["Hectareas Campo", "Toneladas Campo", "Toneladas Compradas", "Toneladas Objetivo", "% Cumplimiento"]

        measures: ["Tn. Compradas", "Tn. Objetivo", "% Cumplimiento"]
    });

    return dataSource;
}

function CrearPivotGrid() {
    var ds = CrearPivotGridDataSource([]);

    $("#pivotgrid").kendoPivotGrid({
        filterable: true,
        sortable: false,
        columnWidth: 140,
        //height: "60vh",
        dataSource: ds,

        excel: {
            fileName: "Reporte Dinamico.xlsx"
        },

        dataBound: function () {
            var fields = this.columnFields.add(this.rowFields).add(this.measureFields);

            fields.find(".k-button")
                .each(function (_, item) {
                    item = $(item);
                    var text = item.data("name");

                    if (text == "RazonSocial") {
                        text = "Razón Social"
                    }
                    else if (text == "Sergmentacion") {
                        text = "Sergmentación"
                    }
                    else if (text == "ContactoApellido") {
                        text = "Contacto"
                    }
                    else if (text == "AreaDeInfluencia") {
                        text = "Area de Influencia"
                    }
                    else if (text == "MaterialCampaña") {
                        text = "Material Campaña"
                    }
                    else if (text == "Campaña") {
                        text = "Campaña"
                    }
                    else if (text == "ToneladasCompradas") {
                        text = "Tn Compradas"
                    }
                    else if (text == "ToneladasObjetivo") {
                        text = "Tn Objetivo"
                    }
                    else if (text == "Campaña Campo") {
                        text = "Campaña Campo"
                    }
                    else if (text == "MaterialCampo") {
                        text = "Material Campo"
                    }
                    else if (text == "HectCampo") {
                        text = "Hectareas Campo"
                    }
                    else if (text == "TonCampo") {
                        text = "Toneladas Campo"
                    }
                    else if (text == "Localidad Campo") {
                        text = "Localidad Campo"
                    }
                    else if (text == "ProvinciaCampo") {
                        text = "Provincia Campo"
                    }
                    else if (text == "CampañaAcopio") {
                        text = "Campaña Acopio"
                    }
                    else if (text == "PorcentajeAcopio") {
                        text = "% Acopio"
                    }
                    else if (text == "TonAcopio") {
                        text = "Toneladas Acopio"
                    }
                    else if (text == "LocalidadAcopio") {
                        text = "Localidad Acopio"
                    }
                    else if (text == "ProvinciaAcopio") {
                        text = "Provincia Acopio"
                    }

                    item.contents().eq(0).replaceWith(text);
                });
        },

        messages: {
            measureFields: "Suelte Campos de Datos Aquí",
            columnFields: "Suelte Campos de Columna Aquí",
            rowFields: "Suelte Campos de Fila Aquí",
            fieldMenu: {
                info: "Mostrar elementos con estos valores:",
                sortAscending: "Orden Ascendente",
                sortDescending: "Orden Descendente",
                filterFields: "Filtro de campos",
                filter: "Filtro",
                include: "Incluir campos...",
                title: "Campos a incluir",
                clear: "Limpiar",
                ok: "Aceptar",
                cancel: "Cancelar",
                operators: {
                    contains: "Contiene",
                    doesnotcontain: "No Contiene",
                    startswith: "Comienza Con",
                    endswith: "Termina Con",
                    eq: "Es Igual a",
                    neq: "Es Distinto a"
                }
            }
        },
    });
}

function CrearConfigurator() {
    var ds = CrearPivotGridDataSource([]);

    var configurator = $("#configurator").kendoPivotConfigurator({
        dataSource: ds,
        filterable: true,
        sortable: false,
        //height: "60vh",
        messages: {
            measures: "Suelte Campos de Datos Aquí",
            columns: "Suelte Campos de Columna Aquí",
            rows: "Suelte Campos de Fila Aquí",
            measuresLabel: "Datos",
            columnsLabel: "Columnas",
            rowsLabel: "Filas",
            fieldsLabel: "Campos",
            fieldMenu: {
                info: "Mostrar elementos con estos valores:",
                sortAscending: "Orden Ascendente",
                sortDescending: "Orden Descendente",
                filterFields: "Filtro de campos",
                filter: "Filtro",
                include: "Incluir campos...",
                title: "Campos a incluir",
                clear: "Limpiar",
                ok: "Aceptar",
                cancel: "Cancelar",
                operators: {
                    contains: "Contiene",
                    doesnotcontain: "No Contiene",
                    startswith: "Comienza Con",
                    endswith: "Termina Con",
                    eq: "Es Igual a",
                    neq: "Es Distinto a"
                }
            }
        },
    }).getKendoPivotConfigurator();

    var source = configurator.treeView.dataSource;

    source.one("change", function () {
        source.get("Measures").set("caption", "Campos Totalizables");
    });
}

function AssignTabEvents() {
    $('.nav-tabs a').on('shown.bs.tab', function (event) {
        var titulo = $(event.target).text();

        if (titulo == "Reporte Dinámico") {
            $("#pivotgrid").data("kendoPivotGrid").refresh();
        }

        if (titulo == "Configuración") {
            $("#configurator").data("kendoPivotConfigurator").refresh();
        }
    });
}

function CrearViewModel() {
    var param = {
        "proveedorId": null,
        "provinciaId": null,
        "provinciaIdDesc": null,
        "LocalidadId": null,
        "LocalidadIdDesc": null,
        "EstadoId": null,
        "EstadoIdDesc": null,
        "SegmentacionId": null,
        "SegmentacionIdDesc": null,
        "material": null,
        "materialDesc": null,
    };

    viewModel = kendo.observable({
        Parametros: param,

        ProveedorCombo: [],
        ProvinciaCombo: [],
        LocalidadCombo: [],
        EstadoCombo: [],
        SegmentacionCombo: [],
        MaterialCombo: [],
        ComercialCombo: [],

        isControlDisabled: true,
    });

    kendo.bind($("#CubProveedores"), viewModel);
}

function InicializarDatos() {
    var funcReturn = function (data) {
        if (ExistsErrorMessages(data.Errores)) {
            ShowErrorMessages(data.Errores);
        }
        else {
            datosIniCubProveedores = data;
            AsignarDatos();
            RefrescarWidgets();
        }
    }

    MSExecuteURLOnServerAsync('/CubProveedores/Inicializar', funcReturn, '');
}

function AsignarDatos() {
    viewModel.set("Parametros", datosIniCubProveedores.Param);
    viewModel.set("Parametros", datosIniCubProveedores.Param);
    viewModel.set("ProveedorCombo", datosIniCubProveedores.Datos.Proveedor);
    viewModel.set("ProvinciaCombo", datosIniCubProveedores.Datos.Provincia);
    viewModel.set("LocalidadCombo", datosIniCubProveedores.Datos.Localidad);
    viewModel.set("EstadoCombo", datosIniCubProveedores.Datos.Estado);
    viewModel.set("SegmentacionCombo", datosIniCubProveedores.Datos.Segmentacion);
    viewModel.set("MaterialCombo", datosIniCubProveedores.Datos.Material);
    viewModel.set("ComercialCombo", datosIniCubProveedores.Datos.Comercial);

    viewModel.set("isControlDisabled", false);
}

function RefrescarWidgets() {
    viewModel.Parametros.proveedorId = $("#proveedorId").data("kendoDropDownList").dataItem();
    viewModel.Parametros.provinciaId = $("#provinciaId").data("kendoDropDownList").dataItem();
    viewModel.Parametros.LocalidadId = $("#LocalidadId").data("kendoDropDownList").dataItem();
    viewModel.Parametros.EstadoId = $("#EstadoId").data("kendoDropDownList").dataItem();
    viewModel.Parametros.SegmentacionId = $("#SegmentacionId").data("kendoDropDownList").dataItem();
    viewModel.Parametros.material = $("#material").data("kendoDropDownList").dataItem();
}

function LimpiarValidaciones() {
    $("#errproveedorId").css("display", "none");
    $("#errprovinciaId").css("display", "none");
    $("#errLocalidadId").css("display", "none");
    $("#errEstadoId").css("display", "none");
    $("#errSegmentacionId").css("display", "none");
    $("#errmaterial").css("display", "none");
}

function ReporteListar() {
    LimpiarValidaciones();

    var param = {
        "proveedorId": GetDropDownValue(viewModel, "Parametros.proveedorId.ProveedorId"),
        "proveedorId": GetDropDownValue(viewModel, "Parametros.proveedorId.ProveedorId"),
        "provinciaId": GetDropDownValue(viewModel, "Parametros.provinciaId.ProvinciaId"),
        "provinciaIdDesc": $("#provinciaId").data("kendoDropDownList").text(),
        "LocalidadId": GetDropDownValue(viewModel, "Parametros.LocalidadId.LocalidadId"),
        "LocalidadIdDesc": $("#LocalidadId").data("kendoDropDownList").text(),
        "EstadoId": GetDropDownValue(viewModel, "Parametros.EstadoId.EstadoId"),
        "EstadoIdDesc": $("#EstadoId").data("kendoDropDownList").text(),
        "SegmentacionId": GetDropDownValue(viewModel, "Parametros.SegmentacionId.SegmentacionId"),
        "SegmentacionIdDesc": $("#SegmentacionId").data("kendoDropDownList").text(),
        "material": GetDropDownValue(viewModel, "Parametros.material.MaterialId"),
        "materialDesc": $("#material").data("kendoDropDownList").text(),
    };

    var result = MSExecuteOnServer('/CubProveedores/Validar', param);

    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            ShowTooltipMessages("err", result.Errores);
        }
        else {
            EmitirReporte(param);
        }
    }
}

function EmitirReporte(param) {
    var funcReturn = function (data) {
        if (data != null) {
            if (ExistsErrorMessages(data.Errores)) {
                ShowTooltipMessages("err", data.Errores);
            }
            else {
                var ds = CrearPivotGridDataSource(data.Proveedores);

                var pivotgrid = $("#pivotgrid").data("kendoPivotGrid");

                pivotgrid.setDataSource(ds);

                $('#rootwizard').bootstrapWizard('show', 'tab2');

                $('.nav-tabs a[href="#menu1"]').tab('show');

                pivotgrid.refresh();

                var configurator = $("#configurator").data("kendoPivotConfigurator");

                configurator.setDataSource(ds);
            }
        }
    }

    MSExecuteOnServerAsync('/CubProveedores/Listar', param, funcReturn, true);
}

function Cancelar() {
    $('#rootwizard').bootstrapWizard('show', 'tab1');
}

function ExportarAExcel() {
    var pivotgrid = $("#pivotgrid").data("kendoPivotGrid");

    pivotgrid.saveAsExcel();
}