// Generar Boleto
const GenerarBoleto = (() => {
    "use strict";

    const config = {
        urls: {
            getMateriales: "/Boleto/GetMateriales",
            getProveedores: "/Boleto/GetProveedores",
            getComerciales: "/Boleto/GetComerciales",
            getBolsaCompraNet: "/Boleto/GetBolsaCompraNet",
            buscaDatosTabla: "/Boleto/BuscaDatosTabla",
            buscaContratosPendientes: "/Boleto/BuscaContratosPendientes",
            generarBoletos: "/Boleto/GenerarBoletos",
            validarNegocio: "/Boleto/ValidarNegocio",
            gestionarClausulas: "/Boleto/GestionarClausulas"
        }
    };

    const state = {
        grid: null,
        datosInicializados: false,
        seleccionarSoloPendientes: false
    };

    const el = {
        negocioSAP: () => $("#NegocioSAP"),
        fechaConfirmacionDesde: () => $("#fechaConfirmacionDesde"),
        fechaConfirmacionHasta: () => $("#fechaConfirmacionHasta"),
        materialId: () => $("#materialId"),
        proveedorId: () => $("#proveedorId"),
        comercialId: () => $("#comercialId"),
        bolsaCompraNetId: () => $("#bolsaCompraNetId"),
        mailId: () => $("#mailId"),
        contratosPendientesCheck: () => $("#ContratosPendientesCheck"),
        filtrarBtn: () => $("#filtrarBoletosCarOferta"),
        grid: () => $("#contratos-grid"),
        modalBoleto: () => $("#ModalBoleto")
    };

    // ── Utilidades ───────────────────────────────────────────────────────────────

    const trimEnd = cadena =>
        cadena && cadena.endsWith(';') ? cadena.slice(0, -1) : (cadena || '');

    const getDatePicker = $el => $el.data("kendoDatePicker")?.value() ?? null;

    const spinner = mostrar => mostrar ? BlockUi('Consultando...') : $.unblockUI();

    function esNegocioSAPValido() {
        const val = el.negocioSAP().val()?.trim();
        return !val || /^[0-9;]+$/.test(val);
    }
    function validarFiltros() {

        const filtros = {
            NegocioSAP: el.negocioSAP().val() || "",
            FechaConfirmacionDesde: getDatePicker(el.fechaConfirmacionDesde())?.toISOString() ?? null,
            FechaConfirmacionHasta: getDatePicker(el.fechaConfirmacionHasta())?.toISOString() ?? null,
            ProveedorId: el.proveedorId().val(),
            ComercialId: el.comercialId().val(),
            BolsaCompraNetId: el.bolsaCompraNetId().val(),
            MaterialId: el.materialId().val(),
            EsSoloPendientes: el.contratosPendientesCheck().is(":checked")
        };

        const sinFiltrosPrincipales =
            !filtros.NegocioSAP &&
            !filtros.FechaConfirmacionDesde &&
            !filtros.FechaConfirmacionHasta &&
            !filtros.EsSoloPendientes;

        const sinFiltrosSecundarios =
            !filtros.ProveedorId &&
            !filtros.ComercialId &&
            !filtros.MaterialId;

        if (sinFiltrosPrincipales && sinFiltrosSecundarios) {
            return false;
        }
        return true;
    }

    function validarFechas() {
        if (trimEnd(el.negocioSAP().val()) !== '') return true;
        const desde = getDatePicker(el.fechaConfirmacionDesde());
        const hasta = getDatePicker(el.fechaConfirmacionHasta());
        return !(desde && hasta && desde > hasta);
    }
    function validarRangoFechas(desde, hasta, maxMeses = 3) {
        if (!desde || !hasta) return true;
        const limite = new Date(desde);
        limite.setMonth(limite.getMonth() + maxMeses);
        return hasta <= limite;
    }
    function validarFechasConfirmacion() {
        if (trimEnd(el.negocioSAP().val()) !== '') return true;
        const desde = getDatePicker(el.fechaConfirmacionDesde());
        const hasta = getDatePicker(el.fechaConfirmacionHasta());
        if (!validarRangoFechas(desde, hasta)) return false;
        return true;
    }
    function cargarDropdown(url, $selector, textoDefault) {
        $.ajax({
            url,
            type: "GET",
            dataType: "json",
            success(data) {
                $selector.empty().append(`<option value="">${textoDefault}</option>`);
                if (Array.isArray(data)) {
                    data.forEach(item =>
                        $selector.append($("<option>", { value: item.Value, text: item.Text }))
                    );
                }
            },
            error(xhr, status, error) {
                console.error(`Error al cargar ${url}:`, error);
                $selector.html('<option value="">Error al cargar datos</option>');
            }
        });
    }

    // ── Auto-ajuste de columnas (header + content sincronizados) ─────────────────

    // Canvas reutilizable para medir texto sin tocar el DOM
    const _canvas = document.createElement("canvas");
    function medirTexto(texto, fuente) {
        const ctx = _canvas.getContext("2d");
        ctx.font = fuente;
        return Math.ceil(ctx.measureText(texto).width);
    }

    // Ajusta columnas con field al contenido; las columnas sin field (acciones/checkbox)
    // conservan el width fijo declarado en la definición de columns[].
    // El header NO tiene scroll: solo el body hace overflow-x.
    function autoFitColumnas(grid) {
        const $wrapper = grid.element;
        const $headerCols = $wrapper.find(".k-grid-header-wrap colgroup col");
        const $contentCols = $wrapper.find(".k-grid-content   colgroup col");
        const $headerCells = $wrapper.find(".k-grid-header-wrap tr:first th");
        const $rows = $wrapper.find(".k-grid-content tbody tr");
        const columns = grid.columns;

        $headerCells.each(function (colIdx) {
            const colDef = columns[colIdx];
            const hasField = colDef && colDef.field && colDef.field !== "Select";

            if (!hasField) {
                // Columna de acciones o checkbox: respetar width original
                const fixedW = (colDef && colDef.width) ? colDef.width : 40;
                $headerCols.eq(colIdx).css("width", fixedW + "px");
                $contentCols.eq(colIdx).css("width", fixedW + "px");
                return;
            }

            // Medir header (negrita + espacio para icono de sort)
            const headerText = $(this).find(".k-link").text().trim() || $(this).text().trim();
            let maxPx = medirTexto(headerText, "bold 13px Arial") + 32;

            // Medir celdas de esa columna
            $rows.each(function () {
                const cellPx = medirTexto($(this).find("td").eq(colIdx).text().trim(), "13px Arial") + 24;
                if (cellPx > maxPx) maxPx = cellPx;
            });

            maxPx = Math.max(maxPx, 60);

            $headerCols.eq(colIdx).css("width", maxPx + "px");
            $contentCols.eq(colIdx).css("width", maxPx + "px");
        });

        // Sincronizar ancho total de ambas tablas para evitar descuadre
        const totalWidth = Array.from($headerCols).reduce(
            (sum, col) => sum + (parseInt($(col).css("width")) || 0), 0
        );
        $wrapper.find(".k-grid-header-wrap table, .k-grid-content table").css("width", totalWidth + "px");
    }

    // ── Estilos globales del grid ────────────────────────────────────────────────

    function inyectarEstilosGrid() {
        if ($("#grid-boleto-styles").length) return;
        $("<style id='grid-boleto-styles'>").text(`
            #contratos-grid .k-grid-header th {
                font-weight: bold !important;
                white-space: nowrap;
                background-color: #f5f5f5;
            }
            #contratos-grid .k-grid-header-wrap {
                overflow: hidden !important;
            }
            #contratos-grid .k-grid-content {
                overflow-x: auto !important;
                overflow-y: auto !important;
            }
            #contratos-grid .k-grid-header-wrap table,
            #contratos-grid .k-grid-content table {
                table-layout: fixed;
            }
            #contratos-grid .k-grid-content tr:hover td,
            #contratos-grid .k-grid-content tr.k-state-hover td {
                color: #333 !important;
            }
            #contratos-grid .k-grid-content tr.k-state-selected td {
                color: #333 !important;
            }

        `).appendTo("head");
    }

    // ── Inicialización ───────────────────────────────────────────────────────────

    function inicializarFechas() {
        const hoy = new Date();
        const ayer = new Date(hoy);
        ayer.setDate(hoy.getDate() - 30);

        [
            { $el: el.fechaConfirmacionDesde(), value: ayer },
            { $el: el.fechaConfirmacionHasta(), value: hoy },
        ].forEach(({ $el, value }) => {
            $el.data("kendoDatePicker")?.destroy();
            $el.kendoDatePicker({ weekNumber: true, format: "dd/MM/yyyy", value });
        });
    }

    function inicializarCombos() {
        cargarDropdown(config.urls.getMateriales, el.materialId(), "Seleccione Material");
        cargarDropdown(config.urls.getProveedores, el.proveedorId(), "Seleccione Proveedor");
        cargarDropdown(config.urls.getComerciales, el.comercialId(), "Seleccione Comercial");
        cargarDropdown(config.urls.getBolsaCompraNet, el.bolsaCompraNetId(), "Seleccione Bolsa");
    }

    function construirFiltros() {
        const parseId = $el => { const v = $el.val(); return v ? parseInt(v) : null; };

        return {
            NegocioSAP: el.negocioSAP().val() || "",
            FechaConfirmacionDesde: getDatePicker(el.fechaConfirmacionDesde())?.toISOString() ?? null,
            FechaConfirmacionHasta: getDatePicker(el.fechaConfirmacionHasta())?.toISOString() ?? null,
            ProveedorId: parseId(el.proveedorId()),
            ComercialId: parseId(el.comercialId()),
            BolsaCompraNetId: parseId(el.bolsaCompraNetId()),
            MaterialId: parseId(el.materialId()),
            EsSoloPendientes: el.contratosPendientesCheck().is(":checked")
        };
    }

    function inicializarGrilla() {
        const $grid = el.grid();

        if (!$grid.length) {
            console.error("Grid container #contratos-grid not found in DOM");
            return;
        }

        if ($grid.data("kendoGrid")) {
            $grid.data("kendoGrid").destroy();
            $grid.empty();
        }

        inyectarEstilosGrid();

        const dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    url: config.urls.buscaDatosTabla,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8"
                },
                parameterMap(options, operation) {
                    if (operation !== "read") return kendo.stringify(options);
                    return kendo.stringify({
                        page: options.page || 1,
                        pageSize: 10,
                        skip: options.skip || 0,
                        take: 10,
                        sort: options.sort || [],
                        ...construirFiltros()
                    });
                }
            },
            schema: {
                data: "Data",
                total: "Total",
                parse(response) {
                    // Convierte "/Date(ticks)/" a Date en todos los campos de fecha
                    const fechas = ["FechaGeneracion", "FechaOperacion", "FechaConfirmadoSAP", "FechaAnulacion"];
                    (response.Data || []).forEach(item => {
                        fechas.forEach(f => {
                            if (item[f] && typeof item[f] === "string") {
                                const ms = parseInt(item[f].replace(/\/Date\((\d+)\)\//, "$1"));
                                item[f] = isNaN(ms) ? null : new Date(ms);
                            }
                        });
                    });
                    return response;
                },
                model: {
                    id: "Id",
                    fields: {
                        NegocioSAP: { type: "string" },
                        TipoBoleto: { type: "string" },
                        Material: { type: "string" },
                        Estado_Version: { type: "string" },
                        FechaConfirmacion: { type: "date" },
                        FechaGeneracion: { type: "date" },
                        Comercial: { type: "string" },
                        Vendedor: { type: "string" },
                        Bolsa: { type: "string" }
                    }
                }
            },
            serverPaging: true, serverSorting: true, serverFiltering: false,
            pageSize: 10,
            error(e) {
                console.error("Error cargando datos del grid:", e);
                MensErr("Error al cargar los datos: " + (e.errors || "Error desconocido"));
                spinner(false);
            },
            requestStart: () => spinner(true),
            requestEnd: () => spinner(false)
        });

        const columnasExcel = [
            { field: "NegocioSAP", title: "Contrato" },
            { field: "Material", title: "Material" },
            { field: "TipoBoleto", title: "Tipo Boleto" },
            { field: "Bolsa", title: "Bolsa" },
            { field: "Version", title: "Vers." },
            { field: "Estado_Version", title: "Estado V." },
            { field: "FechaGeneracion", title: "Fecha Generación" },
            { field: "FechaOperacion", title: "Fecha Operación" },
            { field: "FechaConfirmadoSAP", title: "Fecha Confirmación" },
            { field: "FechaAnulacion", title: "Fecha Anulación" },
            { field: "ContratoVendedor", title: "Contrato Vendedor" },
            { field: "ContratoCorredor", title: "Contrato Corredor" },
            { field: "Corredor", title: "Corredor" },
            { field: "Vendedor", title: "Vendedor" },
            { field: "Comercial", title: "Comercial" },
            { field: "Precio", title: "Precio" },
            { field: "Moneda", title: "Moneda" },
            { field: "TipoNegocio", title: "Tipo de Contrato" },
            { field: "UsuarioAnulacion", title: "Usuario Anulación" }
        ];

        state.grid = $grid.kendoGrid({
            toolbar: ["excel"],
            excel: { fileName: "Reporte Seleccionado.xlsx", allPages: false },
            excelExport(e) {
                const selectedRows = [];
                $grid.find("tbody input.row-checkbox:checked").each(function () {
                    selectedRows.push(state.grid.dataItem($(this).closest("tr")));
                });

                if (!selectedRows.length) {
                    MensAlerta("No hay filas seleccionadas para exportar.");
                    e.preventDefault();
                    return;
                }

                const sheet = e.workbook.sheets[0];
                sheet.name = "Datos Seleccionados";
                sheet.rows = [];
                sheet.columns = columnasExcel.map(() => ({ width: 150 }));
                sheet.rows.push({
                    cells: columnasExcel.map(c => ({ value: c.title, background: "#f4f4f4", bold: true }))
                });
                selectedRows.forEach(item => {
                    sheet.rows.push({
                        cells: columnasExcel.map(c => {
                            const v = item[c.field];
                            return { value: v instanceof Date ? kendo.toString(v, "dd/MM/yyyy") : v };
                        })
                    });
                });
            },
            dataSource,
            height: 550,
            scrollable: { virtual: false },
            resizable: true,
            pageable: {
                refresh: true,
                pageSizes: false,
                buttonCount: 5,
                messages: {
                    display: "{0} - {1} de {2} elementos",
                    empty: "No hay elementos para mostrar",
                    page: "Página", of: "de {0}",
                    itemsPerPage: "elementos por página",
                    first: "Primero", last: "Último",
                    next: "Siguiente", previous: "Anterior"
                }
            },
            sortable: { mode: "single", allowUnsort: false },
            filterable: false,
            columns: [
                {
                    field: "Select",
                    title: "<input type='checkbox' id='select-all'>",
                    template: "<input type='checkbox' class='row-checkbox'/>",
                    width: 40, sortable: false, filterable: false
                },
                {
                    title: "",
                    template: ({ NegocioSAP }) =>
                        `<div style="display:flex;flex-direction:column;gap:4px;">
                            <a onclick="GenerarBoletoIndividual('${NegocioSAP}')" title="Generar Boleto">
                                Generar Boleto <img style="width:16px;height:16px;vertical-align:middle;" src="/Content/Images/agregar-tel-mail.png">
                            </a>
                            <a onclick="GestionarClausulasBoleto('${NegocioSAP}')" title="Editar Clausulas">
                                Editar Clausulas <img style="width:16px;height:16px;vertical-align:middle;" src="/Content/Images/contacto-edit.png">
                            </a>
                        </div>`,
                    width: 160, sortable: false, filterable: false
                },
                { field: "NegocioSAP", title: "Contrato", type: "string", headerAttributes: { title: "Contrato" } },
                { field: "Material", title: "Material", type: "string", headerAttributes: { title: "Material" } },
                { field: "TipoBoleto", title: "Tipo Boleto", type: "string", headerAttributes: { title: "Tipo Boleto" } },
                { field: "Bolsa", title: "Bolsa", type: "string", headerAttributes: { title: "Bolsa" } },
                { field: "Version", title: "Vers.", type: "number", headerAttributes: { title: "Versión" } },
                { field: "Estado_Version", title: "Estado V.", type: "string", headerAttributes: { title: "Estado Versión" } },
                { field: "FechaGeneracion", title: "F. Generación", type: "date", headerAttributes: { title: "Fecha Generación" }, format: "{0:dd/MM/yyyy}" },
                { field: "FechaConfirmadoSAP", title: "F. Confirmación", type: "date", headerAttributes: { title: "Fecha Confirmación" }, format: "{0:dd/MM/yyyy}" },
                { field: "FechaAnulacion", title: "F. Anulación", type: "date", headerAttributes: { title: "Fecha Anulación" }, format: "{0:dd/MM/yyyy}" },
                { field: "ContratoVendedor", title: "C. Vendedor", type: "string", headerAttributes: { title: "Contrato Vendedor" } },
                { field: "ContratoCorredor", title: "C. Corredor", type: "string", headerAttributes: { title: "Contrato Corredor" } },
                { field: "Vendedor", title: "Vendedor", headerAttributes: { title: "Vendedor" } },
                { field: "Corredor", title: "Corredor", headerAttributes: { title: "Corredor" } },
                { field: "Comercial", title: "Comercial", headerAttributes: { title: "Comercial" } },
                { field: "Precio", title: "Precio", type: "number", headerAttributes: { title: "Precio" }, format: "{0:#,##0.00}" },
                { field: "Moneda", title: "Moneda", type: "string", headerAttributes: { title: "Moneda" } },
                { field: "TipoNegocio", title: "Tipo Contrato", type: "string", headerAttributes: { title: "Tipo Contrato" } },
                { field: "UsuarioAnulacion", title: "Usu. Anulación", type: "string", headerAttributes: { title: "Usuario Anulación" } }
            ],
            dataBound(e) {
                autoFitColumnas(e.sender);
                configurarEventosGrid();
            }
        }).data("kendoGrid");
    }

    function configurarEventosGrid() {
        $("#select-all").off("change").on("change", function () {
            el.grid().find("input.row-checkbox").prop("checked", $(this).is(":checked"));
            const esSoloPendientes = el.contratosPendientesCheck().is(":checked")
            if (esSoloPendientes)
                state.seleccionarSoloPendientes = $(this).is(":checked");
            else
                state.seleccionarSoloPendientes = false;
        });

        $(".row-checkbox").off("change").on("change", function () {
            const total = $(".row-checkbox").length;
            const seleccionados = $(".row-checkbox:checked").length;
            //$("#select-all")
            //    .prop("indeterminate", seleccionados > 0 && seleccionados < total)
            //    .prop("checked", seleccionados === total);
        });
    }

    function inicializarEventos() {
        const $btn = el.filtrarBtn();

        if (!$btn.length) {
            console.error("Button #filtrarBoletosCarOferta not found in DOM");
            return;
        }

        $btn.off("click").on("click", e => {
            e.preventDefault();
            filtrarBoletos();
        });

        el.negocioSAP()
            .on("paste", function (e) {

                e.preventDefault();

                let texto = (e.originalEvent.clipboardData || window.clipboardData)
                    .getData("text");

                // Separar por saltos de línea
                let valores = texto
                    .split(/\r?\n/)           // soporta Excel / Windows / Linux
                    .map(v => v.trim())       // quitar espacios
                    .filter(v => v !== "");   // eliminar vacíos

                // eliminar duplicados
                valores = [...new Set(valores)];

                // unir en una sola línea con ;
                $(this).val(valores.join(";"));
                el.contratosPendientesCheck().prop("checked", false);
            })
            .on("keypress", e => {
                el.contratosPendientesCheck().prop("checked", false);
                if (e.which === 32) e.preventDefault(); // bloquear espacios
            });
    }

    // ── Lógica principal ─────────────────────────────────────────────────────────

    function filtrarBoletos() {
        spinner(true);
        setTimeout(() => {
            try {

                if (!validarFiltros()) {
                    MensErr("No se ha seleccionado ningun filtro para la busqueda.");
                    spinner(false);
                    return;
                }

                if (!esNegocioSAPValido()) {
                    MensErr("Solo se admiten números y el ';' en el campo Negocio SAP.");
                    spinner(false);
                    return;
                }
                if (!validarFechas()) {
                    MensErr("El Rango de Fechas no es válido. El campo Desde debe tener un valor menor al campo Hasta.");
                    spinner(false);
                    return;
                }
                if (!validarFechasConfirmacion()) {
                    MensErr("El rango de fechas de confirmación no puede superar los 3 meses.");
                    spinner(false);
                    return;
                }
                if (state.grid) {
                    state.grid.dataSource.page(1);
                    state.grid.dataSource.read();
                } else {
                    inicializarGrilla();
                    spinner(false);
                }
            } catch (e) {
                console.error("Error al filtrar boletos:", e);
                MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
                spinner(false);
            }
        }, 200);
    }

    function generarBoleto(negocioSAP) {
        BlockUi('Cargando...');
        setTimeout(() => {
            MSExecuteOnServerAsync(config.urls.generarBoletos, {
                ContratoSAP: negocioSAP,
                Mail: el.mailId().is(":checked")
            })
                .then(function (result) {
                    $.unblockUI();
                    if (!result?.length) {
                        MensErr("No se generó ningún boleto.");
                    } else {
                        kendo.bind(el.modalBoleto(), { ObtenerBoletos: result });
                        el.modalBoleto().modal("show");
                    }
                })
                .catch(function (e) {
                    $.unblockUI();
                    console.error("Error al Generar Boleto:", e);
                    MensErr("Ocurrió un error inesperado. Inténtelo nuevamente.");
                });
        }, 200);
    }

    function generarBoletosSeleccionados() {
        const negocioSAPList = [];

        const esSoloPendientes = el.contratosPendientesCheck().is(":checked")
        if (esSoloPendientes && state.seleccionarSoloPendientes) {

            const parseId = $el => { const v = $el.val(); return v ? parseInt(v) : null; };
            const filtros = {
                NegocioSAP: el.negocioSAP().val() || "",
                FechaConfirmacionDesde: getDatePicker(el.fechaConfirmacionDesde())?.toISOString() ?? null,
                FechaConfirmacionHasta: getDatePicker(el.fechaConfirmacionHasta())?.toISOString() ?? null,
                ProveedorId: parseId(el.proveedorId()),
                ComercialId: parseId(el.comercialId()),
                BolsaCompraNetId: parseId(el.bolsaCompraNetId()),
                MaterialId: parseId(el.materialId()),
                EsSoloPendientes: el.contratosPendientesCheck().is(":checked")
            };

            var response = MSExecuteOnServer(config.urls.buscaContratosPendientes, { filtrosBusqueda: filtros });
            response.Data.forEach(function (item) {
                negocioSAPList.push(item.ContratoSAP);
            });

            if (negocioSAPList.length)
                generarBoleto(negocioSAPList.join(';'));

        } else {
            el.grid().find("tbody input.row-checkbox:checked").each(function () {
                negocioSAPList.push(state.grid.dataItem($(this).closest("tr")).NegocioSAP);
            });

            if (negocioSAPList.length) {
                generarBoleto(negocioSAPList.join(';'));
            } else {
                MensErr("No hay ningún negocio seleccionado.");
            }
        }

    }

    function gestionarClausulas(negocioSAP) {
        MSExecuteOnServerAsync(config.urls.validarNegocio, { NegocioSAP: negocioSAP })
            .then(function (response) {
                if (response?.Mensaje) {
                    MensErr(response.Mensaje);
                } else {
                    window.location.href = `${config.urls.gestionarClausulas}?numeroSap=${encodeURIComponent(negocioSAP)}`;
                }
            })
            .catch(function (error) {
                console.error("Error validando negocio:", error);
                MensErr("Error al validar el negocio");
            });
    }

    // ── API pública ──────────────────────────────────────────────────────────────

    return {
        init() {
            if (state.datosInicializados) return;

            el.contratosPendientesCheck().prop('checked', true);
            kendo.culture("es-AR");

            inicializarFechas();
            inicializarCombos();
            inicializarGrilla();
            inicializarEventos();

            state.datosInicializados = true;
        },
        filtrarBoletos,
        generarBoleto,
        generarBoletosSeleccionados,
        gestionarClausulas
    };
})();

$(document).ready(() => GenerarBoleto.init());

// Funciones globales para compatibilidad con HTML
function FiltrarBoletos() { GenerarBoleto.filtrarBoletos(); }
function GenerarBoletoIndividual(negocio) { GenerarBoleto.generarBoleto(negocio); }
function GenerarBoletos() { GenerarBoleto.generarBoletosSeleccionados(); }
function GestionarClausulasBoleto(negocio) { GenerarBoleto.gestionarClausulas(negocio); }
