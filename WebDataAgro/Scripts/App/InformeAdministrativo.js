var checkedIds = {};

$(document).ready(function () {
    kendo.culture("es-AR");
    inicializarTodosKendoDate($(".filtroFecha"));
    //$("#fechaCargaId").data("kendoDatePicker").value(new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate()));
    CreateGridInformeAdministrativo();
    InicializarElementos2();

    //InicializarElementos();
    //CreateGridInformes();
    //CargarGrilla();
});
function onChange(arg) {
    console.log("The selected product ids are: [" + this.selectedKeyNames().join(", ") + "]");
}

function CreateGridInformeAdministrativo() {
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
                url: '/InformeComercial/BuscaDatosTabla',

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
                id: 'InformeComercialId',
                fields: {
                    InformeComercialId: { type: "number" },
                    Cuit: { type: "string" },
                    RazonSocial: { type: "string" },
                    Campaña: { type: "string" },
                    Materiales: { type: "string" },
                    Comercial: { type: "string" },
                    FechaAlta: { type: "date" },
                    FechaDescarga: { type: "date" },
                    OrigenDA: { type: "string" },
                    Seleccionado: { type: "boolean" },
                }
            }
        },

        serverPaging: true,
        serverSorting: true,
        serverFiltering: false,
        pageSize: 20,

        batch: true,
    };

    //Grid definition
    $("#grilla-informes").kendoGrid({
        toolbar: ["excel"],
        //toolbar: ['Export'],
        excel: {
            fileName: "Informe_Administrativo.xlsx",
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
            //define template column with checkbox and attach click event handler
            //{
            //    title: 'Select All',
            //    //headerTemplate: "<input type='checkbox' id='header-chb' class='k-checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
            //    headerTemplate: "<input type='checkbox' id='header-chb' class='k-checkbox k-checkbox-md k-rounded-md header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
            //    template: function (dataItem) {
            //        //return "<input type='checkbox' id='" + dataItem.InformeComercialId + "' class='k-checkbox row-checkbox'><label class='k-checkbox-label' for='" + dataItem.InformeComercialId + "'></label>";
            //        return "<input type='checkbox' id='" + dataItem.InformeComercialId + "' class='k-checkbox k-checkbox-md k-rounded-md row-checkbox'><label class='k-checkbox-label' for='" + dataItem.InformeComercialId + "'></label>";
            //    },
            //    width: 80,
            //    attributes: { class: "k-text-center" },
            //    headerAttributes: { class: "k-text-center" },
            //},
            { field: "InformeComercialId", selectable: true, exportable: { excel: false }, width: 50, title: "Id" },
            //{ field: "InformeComercialId", title: "InformeComercialId"},
            { field: "Cuit", width: 150 },
            { field: "RazonSocial", title: "Razón Social", width: 250 },
            { field: "Campaña", width: 100 },
            { field: "Materiales", template: "#=Materiales#" },
            { field: "Comercial" },
            { field: "FechaAlta", title: "Fecha de Generación", width: 150, format: _DefaultDateTemplate },
            { field: "FechaDescarga", title: "Fecha de Descarga", width: 150, format: _DefaultDateTemplate },
            { field: "OrigenDA", title: "Origen DA", width: 100 }
        ],
        persistSelection: true,
        excelExport: function (e) {
            var mes = new Date().getMonth() + 1;
            var sheet = e.workbook.sheets[0];
            for (var i = 1; i < sheet.rows.length; i++) {
                var row = sheet.rows[i];
                row.cells[7].value = new Date().getDate() + "/" + mes + "/" + new Date().getFullYear();
            }

            var grid = $("#grilla-informes").data("kendoGrid");

            if (grid.selectedKeyNames().length === 0) {
                MensErr("Por favor, seleccione filas antes de exportar.");
                return;
            }


            sheet.rows = sheet.rows.filter(x => x.type == "header" || grid.selectedKeyNames().some(y => x.cells[0].value == y));
            

            if (grid.selectedKeyNames().length > 0) {
                var objInforme = { ids: grid.selectedKeyNames() };
                MSExecuteOnServer('/InformeComercial/ActualizarFechaDescargaInformeComercial', objInforme);
            }
            grid.dataSource.page(1);

        },
        change: onChange,

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
        //selectable: "row",
        selectable: "multiple",
        height: 550,
        filterable: false,
        batch: true,
        pageSize: 20,
        pageable: true,
        //dataBound: onDataBound,
    });

    $(".k-grid-Export").on('click', function (e) {

        var grid = $("#grilla-informes").data("kendoGrid");
        var view = grid.dataSource.data();

        var arrInformeComercial = [];
        var arrInformeComercial = new Array();

        for (var i = 0; i < view.length; i++) {
            if (checkedIds[view[i].InformeComercialId]) {
                console.log(view[i].InformeComercialId);
                view[i].Seleccionado = true;
                arrInformeComercial.push(view[i].InformeComercialId);
            }
        };

        if (arrInformeComercial.length > 0) {
            var objInforme = { ids: arrInformeComercial };
            MSExecuteOnServer('/InformeComercial/ActualizarFechaDescargaInformeComercial', objInforme);
        }

        var grid = $("#grilla-informes").getKendoGrid();

        var rows = [{
            cells: [
                { value: "Cuit" },
                { value: "RazonSocial" },
                { value: "Campaña" },
                { value: "Materiales" },
                { value: "Comercial" },
                { value: "FechaAlta" },
                { value: "FechaDescarga" },
                { value: "OrigenDA" },
            ]
        }];

        var fechaDescarga = new Date().getDate() + "/" + new Date().getMonth() + "/" + new Date().getFullYear();

        var trs = $("#grilla-informes").find('tr');

        for (var i = 0; i < trs.length; i++) {
            if ($(trs[i]).find(":checkbox").is(":checked")) {
                var dataItem = grid.dataItem(trs[i]);
                rows.push({
                    cells: [
                        { value: dataItem.Cuit },
                        { value: dataItem.RazonSocial },
                        { value: dataItem.Campaña },
                        { value: dataItem.Materiales },
                        { value: dataItem.Comercial },
                        { value: dataItem.FechaAlta },
                        { value: fechaDescarga }, //{ value: dataItem.FechaDescarga },
                        { value: dataItem.OrigenDA },
                    ]
                });
            }
        }

        grid.dataSource.pageSize(20);
        grid.refresh();

        excelExport(rows);
    })

    function excelExport(rows) {

        var workbook = new kendo.ooxml.Workbook({
            sheets: [
                {
                    columns: [
                        { width: 150 }, //{ autoWidth: true },
                        { width: 250 },
                        { width: 100 },
                        { width: 200 },
                        { width: 200 },
                        { width: 150 },
                        { width: 150 },
                        { width: 100 },
                    ],
                    title: "Informe_Administrativo",
                    rows: rows
                }
            ]
        });

        kendo.saveAs({ dataURI: workbook.toDataURL(), fileName: "Informe_Administrativo.xlsx" });
    }

    var grid = $("#grilla-informes").data("kendoGrid");
    //bind click event to the checkbox
    grid.table.on("click", ".row-checkbox", selectRow);

    $('#header-chb').change(function (ev) {
        var checked = ev.target.checked;
        $('.row-checkbox').each(function (idx, item) {
            if (checked) {
                if (!($(item).closest('tr').is('.k-selected'))) {
                    $(item).click();
                }
            } else {
                if ($(item).closest('tr').is('.k-selected')) {
                    $(item).click();
                }
            }
        });
    });

    $("#showSelection").bind("click", function () {
        var checked = [];
        for (var i in checkedIds) {
            if (checkedIds[i]) {
                checked.push(i);
            }
        }

        alert(checked);
    });

    //on click of the checkbox:
    function selectRow() {
        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#grilla-informes").data("kendoGrid"),
            dataItem = grid.dataItem(row);

        checkedIds[dataItem.InformeComercialId] = checked;

        if (checked) {
            //-select the row
            row.addClass("k-selected");

            var checkHeader = true;

            $.each(grid.items(), function (index, item) {
                if (!($(item).hasClass("k-selected"))) {
                    checkHeader = false;
                }
            });

            $("#header-chb")[0].checked = checkHeader;
        } else {
            //-remove selection
            row.removeClass("k-selected");
            $("#header-chb")[0].checked = false;
        }
    }

    //on dataBound event restore previous selected rows:
    function onDataBound(e) {
        var view = this.dataSource.view();
        for (var i = 0; i < view.length; i++) {
            if (checkedIds[view[i].InformeComercialId]) {
                this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                    .addClass("k-selected")
                    .find(".k-checkbox")
                    .attr("checked", "checked");
            }
        }
    }
}

function InicializarElementos2() {
    $(".number").kendoNumericTextBox({
        culture: "es-AR",
        format: "n0",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });
    kendo.culture("es-AR");

    $("#nuevoNumContrato").kendoNumericTextBox({
        culture: "es-AR",
        format: "######################",
        value: " ",
        decimals: 0,
        restrictDecimals: true,
        spinners: false,
        min: 0
    });

    CrearMultiSelectFiltro("#buscadorProveedor", "Proveedor", "ProveedorId", "/Contrato/ListarProveedor");
}

function Filtrar() {
    $('#grilla-informes').data('kendoGrid').dataSource.read();
}

//var InicializarElementos = function () {
//    kendo.culture("es-AR");

//    $("#butDescargar").click(function () {
//        DescargarElementos();
//    });
//}

var DescargarElementos = function () {
    var checked = [];
    for (var i in checkedIds) {
        if (checkedIds[i]) {
            checked.push(i);
        }
    }
    var funcReturn = function (data) {
        if (data != null) {
            if (data.Errores.length > 0) {
                MensErr("No se encontraron resultados para los filtros elegidos.");
                return false;
            }
            else {
                CargarGrilla();
                if (data.DownloadKey.length > 0) {
                    var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                    window.location = url;
                }
            }
        }
        else {
            MensErr("No se encontraron resultados para los filtros elegidos.");
            return false;
        }
    }

    if (checked.length > 0) {
        var datos = { Informes: checked.join(',') };
        MSExecuteOnServerAsync('/InformeComercial/GenerarExcel', datos, funcReturn, true);
    }
}

var CreateGridInformes = function () {
    $("#grilla-informes").kendoGrid({
        columns: [
            //{ template: '<input type="checkbox" #= Seleccionado ? \'checked="checked"\' : "" # class="chkbx" />', width: 110 },
            {
                title: 'Select All',
                headerTemplate: "<input type='checkbox' id='header-chb' class='k-checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
                template: function (dataItem) {
                    return "<input type='checkbox' id='" + dataItem.InformeComercialId + "' class='k-checkbox row-checkbox'><label class='k-checkbox-label' for='" + dataItem.InformeComercialId + "'></label>";
                },
                width: 80
            },
            { field: "Cuit", width: 250 },
            { field: "RazonSocial", title: "Razón Social", width: 250 },
            { field: "Campaña" },
            { field: "Materiales", template: "#=Materiales#" },
            { field: "Comercial" }
        ],
        sortable: true,
        selectable: "row"
    });

    var grid = $("#grilla-informes").data("kendoGrid");
    grid.table.on("click", ".row-checkbox", selectRow);

    $('#header-chb').change(function (ev) {
        var checked = ev.target.checked;
        $('.row-checkbox').each(function (idx, item) {
            if (checked) {
                if (!($(item).closest('tr').is('.k-state-selected'))) {
                    $(item).click();
                }
            } else {
                if ($(item).closest('tr').is('.k-state-selected')) {
                    $(item).click();
                }
            }
        });
    });

    /*$("#grilla-informes").on("change", "input.chkbx", function (e) {
        var grid = $("#grilla-informes").data("kendoGrid"),
            dataItem = grid.dataItem($(e.target).closest("tr"));

        dataItem.set("Seleccionado", this.checked);
    });*/

    function selectRow() {
        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#grilla-informes").data("kendoGrid"),
            dataItem = grid.dataItem(row);

        checkedIds[dataItem.InformeComercialId] = checked;

        if (checked) {
            //-select the row
            row.addClass("k-state-selected");

            var checkHeader = true;

            $.each(grid.items(), function (index, item) {
                if (!($(item).hasClass("k-state-selected"))) {
                    checkHeader = false;
                }
            });

            $("#header-chb")[0].checked = checkHeader;
        } else {
            //-remove selection
            row.removeClass("k-state-selected");
            $("#header-chb")[0].checked = false;
        }
    }

    //on dataBound event restore previous selected rows:
    function onDataBound(e) {
        var view = this.dataSource.view();
        for (var i = 0; i < view.length; i++) {
            if (checkedIds[view[i].InformeComercialId]) {
                this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                    .addClass("k-state-selected")
                    .find(".k-checkbox")
                    .attr("checked", "checked");
            }
        }
    }
}

var CargarGrilla = function () {
    var funcreturn = function (datos) {
        if (datos.length > 0) {
            var grid = $("#grilla-informes").data("kendoGrid");
            var dataSource = new kendo.data.DataSource({
                data: datos
            });

            grid.setDataSource(dataSource);
        } else {
            var div = $("#grilla-informes");
            div.css({
                'background-color': 'transparent',
                'text-align': 'center',
                border: 'none',
                'margin-top': 20
            });
            div.empty();
            $("<img src='../Content/Images/agro.png'>").appendTo(div).css({
                width: 30,
                height: 30
            });
            $("<span>").appendTo(div).html("No se encontraron resultados").css({
                'padding-left': 10,
                'padding-right': 10,
                'font-size': 18
            });
            $("<img src='../Content/Images/agro.png'>").appendTo(div).css({
                width: 30,
                height: 30
            });

            $("#butDescargar").hide();

            var grid = $("#grilla-informes").data("kendoGrid");
            var dataSource = new kendo.data.DataSource({
                data: []
            });

            grid.setDataSource(dataSource);
        }
    }

    MSExecuteOnServerAsync('/InformeComercial/ListarInformes', {}, funcreturn, true);
}