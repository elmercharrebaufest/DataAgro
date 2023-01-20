var listaCantidad = new Array();

$(document).ready(function () {
    $('#menuproveedor').hide();
    kendo.culture("es-AR");
    //CargarGrillaConfig();
    CargarEventos();


    $(".kNumeric").each(function (index) {
        var max = $(this).attr("max");
        if (!max) {
            max = 999;
        }
        $(this).kendoNumericTextBox({
            step: 1,
            min: 0,
            spinners: true,
            max: max,
            format: "0"
        });
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
            closeAll();
            $('.pall').hide();
            $('.p' + e.dataItem.Cuit).show();
            $('#proveedorCUIT').val(e.dataItem.Cuit);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarProveedoresConCorredor"
                },
                parameterMap: function (data, type) {
                    return { filtro: "", filtroProveedor: $('#buscadorProveedor').val(), corredor: 0 };
                }
            }

        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

});
$(document).on('click', '.list > li ', function () {
    $(this).next('ul').toggle(200);
    if (($(this).next('ul').length)) {
        $(this).toggleClass('multi-opened');
    }
})
var itemsConfirmados;
var aceptar;
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
            location.reload();
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

        } else {
            $("#fleteProcedenciaModal").modal("show");
        }

    });
    $('#CargaCupos').on('hidden.bs.modal', function () {
        $("#cuerpo-carga-cupos").empty();
    });

    $("#boton-si").click(function () {
        $("#fleteProcedenciaModal").modal("hide");
        $("#cuerpo-carga-cupos").empty();
        var fila = '';
        for (var i = 0; i < itemsConfirmados.length; i++) {
            fila = '<tr><td>' + itemsConfirmados[i].razonSocial + '</td> <td>'
                + kendo.toString(itemsConfirmados[i].fechaOriginal, "dd/MM/yyyy") + '</td> <td>'
                + kendo.toString(itemsConfirmados[i].fecha, "dd/MM/yyyy")
                + '</td> <td><input id="flete' + itemsConfirmados[i].nro + '" name="flete' + itemsConfirmados[i].nro + '" min="0" max="' + itemsConfirmados[i].cantidad
                + '" class="cantidad-masiva" value="' + itemsConfirmados[i].cantidadFleteProcedencia + '"/> </td> <td>'
                + 'Max. de cupos: ' + itemsConfirmados[i].cantidad + '</td></tr>'
            $("#cuerpo-carga-cupos").append(fila);
        }
        $(".cantidad-masiva").kendoNumericTextBox({
            culture: "es-AR",
            format: "n0",
            spinners: false,
            min: 0,
            step: 0
        });
        $("#fleteProcedenciaModal").modal("hide");
        $("#CargaCupos").modal("show");
    });
    $("#boton-no").click(function () {
        BlockUi('Cargando...');
        setTimeout(function () {
            $("#fleteProcedenciaModal").modal("hide");
            var url = '/SugerenciaCupo/AceptarSugerenciaCupo';
            var data = itemsConfirmados[0];
            if (aceptar != true) {
                url = '/SugerenciaCupo/ModificarSugerenciaCupo';
                data = itemsConfirmados;
            }
            var resultados = MSExecuteOnServer(url, data);
            $.unblockUI();
            mostrarResultados(resultados)
        }, 250);

    });
    $("#aceptar").click(function () {
        BlockUi('Cargando...');
        setTimeout(function () {
            for (var j = 0; j < itemsConfirmados.length; j++) {
                itemsConfirmados[j].cantidadFleteProcedencia = $("#flete" + j).val();
                itemsConfirmados[j].cantidad = itemsConfirmados[j].cantidad - itemsConfirmados[j].cantidadFleteProcedencia;
            }
            var url = '/SugerenciaCupo/AceptarSugerenciaCupo';
            var data = itemsConfirmados[0];
            if (aceptar != true) {//modificacion o aceptar sugerencias
                url = '/SugerenciaCupo/ModificarSugerenciaCupo';
                data = itemsConfirmados;
            }
            $("#CargaCupos").modal("hide");
            $("#cuerpo-carga-cupos").empty();
            var resultados = MSExecuteOnServer(url, data);
            mostrarResultados(resultados);
            $.unblockUI();
        }, 250);

    });

    $("#aceptarModificarPorNegocio").click(function () {
        aceptar = false;
        var maximo = $("#modCantidad").html();
        var total = 0;
        var fecha = $("#modFecha").html();
        itemsConfirmados = new Array();
        var i = 0;
        $("input:text.diaNeg").each(function (index) {
            if (this.id != "") {
                total = total + $(this).data("kendoNumericTextBox").value();
                var fechaSolicitud = this.id.substr(1, 2) + "/" + this.id.substr(3, 2) + "/" + this.id.substr(5, 4);
                if ($(this).data("kendoNumericTextBox").value() > 0) {
                    itemsConfirmados.push({
                        nro: i++,
                        cantidad: $(this).data("kendoNumericTextBox").value(),
                        idSugerencia: $("#modSugerenciaId").val(),
                        fecha: fechaSolicitud,
                        fechaOriginal: fecha,
                        proveedorId: $("#modProveedorId").val(),
                        razonSocial: $("#modProveedor").html(),
                        cantidadFleteProcedencia: 0,
                        materialId: $("#modMaterialId").val(),
                        comercialId: $("#ComercialSeleccionado1").val(),
                        centroId: $("#CentroId").val(),

                    });
                }

            }

        });
        if (total == 0) {
            MensErr("Ingrese la cantidad de sugerencias que desea cambiar de dia.");
            return;
        }
        if (total > maximo) {
            MensErr("El maximo de sugerencias a modificar es " + maximo);
        } else {
            $("#modificarSugerenciaNegocioModal").modal("hide");
            $("#fleteProcedenciaModal").modal("show");
        }
    });

    $("#aceptarModificarPorProveedor").click(function () {
        aceptar = false;
        var hayCambios = false;
        itemsConfirmados = new Array();
        var n = 0;
        for (var i = 0; i < listaCantidad.length; i++) {
            listaCantidad[i].cantidadAceptadaFila = 0;
        }
        $("input:text.diaProv").each(function (index) {
            if (this.id != "") {
                if ($(this).data("kendoNumericTextBox").value() > 0) {
                    hayCambios = true;
                    for (var i = 0; i < listaCantidad.length; i++) {
                        if (listaCantidad[i].fecha == this.id.substr(3, 2) + this.id.substr(5, 2) + this.id.substr(7, 4)) {
                            listaCantidad[i].cantidadAceptadaFila += $(this).data("kendoNumericTextBox").value();
                        }
                    }
                }

                var fechaOriginal = this.id.substr(3, 2) + "/" + this.id.substr(5, 2) + "/" + this.id.substr(7, 4);
                var fechaSolicitud = this.id.substr(12, 2) + "/" + this.id.substr(14, 2) + "/" + this.id.substr(16, 4);

                if ($(this).data("kendoNumericTextBox").value() > 0) {
                    itemsConfirmados.push({
                        nro: n++,
                        cantidad: $(this).data("kendoNumericTextBox").value(),
                        idSugerencia: $("#modSugerenciaId").val(),
                        fecha: fechaSolicitud,
                        fechaOriginal: fechaOriginal,
                        proveedorId: $("#modProveedorId").val(),
                        razonSocial: $(".modProveedor").html(),
                        cantidadFleteProcedencia: 0,
                        materialId: $("#modMaterialId").val(),
                        comercialId: $("#ComercialSeleccionado1").val(),
                        centroId: $("#CentroId").val(),

                    });
                }

            }

        });

        var errores = "";
        for (var i = 0; i < listaCantidad.length; i++) {
            if (listaCantidad[i].cantidad < listaCantidad[i].cantidadAceptadaFila) {
                errores += "Para la fecha " + listaCantidad[i].fecha.substr(0, 2) + "/" + listaCantidad[i].fecha.substr(2, 2) + "/" + listaCantidad[i].fecha.substr(4, 4) + " el limite de cupos es  " + listaCantidad[i].cantidad + ".\n";
            }
        }
        if (!hayCambios) {
            MensErr("Ingrese la cantidad de sugerencias que desea cambiar de dia.");
            return;
        }
        if (errores != "") {
            MensErr(errores);
        } else {
            $("#modificarSugerenciaProveedorModal").modal("hide");
            $("#fleteProcedenciaModal").modal("show");
        }
    });

    $("#resultadoCupo").on('hidden.bs.modal', function () {
        BlockUi("Cargando...");
        location.reload();
    });

    $(".ComercialSeleccionado").on('change', function () {
        var comercial = $(this).data("kendoDropDownList").value();
        var material = $("input[name='materialId']:checked").val();
        redirect(comercial, material)
    });

    $("input[name='materialId']").on('change', function () {
        var material = $("input[name='materialId']:checked").val();
        var comercial = $("#ComercialSeleccionado1").data("kendoDropDownList").value();
        redirect(comercial, material);
    });

    $("#buscadorProveedor").click(function () {
        $("#buscadorProveedor").data("kendoAutoComplete").value("");
        $("#buscadorProveedor").data("kendoAutoComplete").trigger("change");
        $(".pall").show();
        $('#proveedorCUIT').val("");
        closeAll();
    });
}
function redirect(comercial, material, mostrarResultado) {
    BlockUi("Cargando...");
    var search = "?ComercialSeleccionado=" + comercial + "&materialId=" + material;
    if (mostrarResultado == true) {
        search = search + "&muestraModal=true";
    }
    var newURL = window.location.protocol + "//" + window.location.host + window.location.pathname + search;
    window.location.href = newURL;
}

function AceptarSugerenciaProvDia(id, fecha, material, proveedorId, razonSocial, cantCupos) {
    aceptar = true;
    itemsConfirmados = new Array();
    itemsConfirmados.push({
        nro: 0,
        cantidad: $("#" + id).data("kendoNumericTextBox").value() != null ? $("#" + id).data("kendoNumericTextBox").value() : cantCupos,
        idSugerencia: 0,
        fecha: fecha,
        proveedorId: proveedorId,
        razonSocial: razonSocial,
        cantidadFleteProcedencia: 0,
        materialId: material,
        comercialId: $("#ComercialSeleccionado1").val(),
        centroId: $("#CentroId").val(),

    });
    $("#fleteProcedenciaModal").modal("show");
}

function AceptarSugerencia(id, fecha, proveedorId, razonSocial, cantCupos) {
    aceptar = true;
    itemsConfirmados = new Array();
    itemsConfirmados.push({
        nro: 0,
        cantidad: $("#" + id).data("kendoNumericTextBox").value() != null ? $("#" + id).data("kendoNumericTextBox").value() : cantCupos,
        idSugerencia: id.replace('s', ''),
        fecha: fecha,
        proveedorId: proveedorId,
        razonSocial: razonSocial,
        cantidadFleteProcedencia: 0
    });
    $("#fleteProcedenciaModal").modal("show");
}
function DevolverSugerencia(id) {
    BlockUi('Cargando...');
    setTimeout(function () {
        var cantidad = $("#" + id).data("kendoNumericTextBox").value();
        if (cantidad == null || cantidad == "" || cantidad == 0) {
            MensErr("Ingrese la cantidad de cupos a devolver");
            $.unblockUI();
            return;
        }
        id = id.replace('d', '');
        $("#fleteProcedenciaModal").modal("hide");
        var resultado = MSExecuteOnServer('/SugerenciaCupo/RechazarSugerenciaCupo', { idSugerencia: id, cantidad: cantidad });
        mostrarResultado(resultado);
        $.unblockUI();

    }, 250);
}

function ModificarSugerencia(id, fecha, proveedorId, razonSocial, cantidad, materialId, materialDesc) {
    aceptar = false;
    $("#modSugerenciaId").val(id);
    $("#modMaterialId").val(materialId);
    $("#modProveedorId").val(proveedorId);
    $("#modMaterial").html(materialDesc);
    $("#modProveedor").html(razonSocial);
    $("#modFecha").html(fecha);
    $("#modCantidad").html(cantidad);
    $("input:text.diaNeg").each(function (index) {
        if (this.id != "") {
            $(this).data("kendoNumericTextBox").value("");
        }
    });
    $("#modificarSugerenciaNegocioModal").modal("show");
}

function ModificarSugerenciaProv(id, fecha, proveedorId, razonSocial, materialId, materialDesc, cantidad) {
    aceptar = false;
    listaCantidad = new Array();
    var arrayDeCadenas = cantidad.split(",");
    for (var i = 0; i < arrayDeCadenas.length; i++) {
        var valores = arrayDeCadenas[i].split("-");
        listaCantidad.push({
            fecha: valores[0],
            cantidad: valores[1],
            cantidadAceptadaFila: 0,
        });
    }

    $(".filaProv").hide();
    for (var cupos of listaCantidad) {
        document.getElementById("modCantidad" + cupos.fecha).innerHTML = cupos.cantidad;
        $("#fila" + cupos.fecha).show();
    }
    $("#modSugerenciaId").val(0);
    $("#modMaterialId").val(materialId);
    $("#modProveedorId").val(proveedorId);
    $(".modMaterial").html(materialDesc);
    $(".modProveedor").html(razonSocial);
    $("input:text.diaProv").each(function (index) {
        if (this.id != "") {
            $(this).data("kendoNumericTextBox").value("");
        }
    });
    $("#modificarSugerenciaProveedorModal").modal("show");
}

function mostrarResultado(result) {
    if (result == null) {
        return;
    }
    var errores = new Array();
    for (var i = 0; i < result.length; i++) {
        if (result[i].HayError) {
            errores = errores.concat(result[i].ListaErrores);
        }
    }
    if (errores.length == 0) {
        MensInfoReload("Se grabo correctamente.");
    } else {
        ShowErrorMessages(errores);
    }
}
function mostrarResultados(result) {
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
        //grid._selectedIds = {};
        //grid.clearSelection();
        //grid.dataSource.read();
    }
    if (error.length > 0) {
        ShowErrorMessages(error);
    }
}

function cuposCreados(lista) {
    $("#cupos-generados-modal").html(lista.join("</br>"));
    $('#resultadoCupo').modal('toggle');
}


function copiarGenerados(idDiv) {
    var listaCupos = $("#"+idDiv).html().replace(/<br>/g, "\n");
    listaCupos = listaCupos.split('<strong>').join("");
    listaCupos = listaCupos.split('</strong>').join("");
    //como un replaceall 
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
                url: '/SugerenciaCupo/DatosConfiguracion',
                data: additionalInfo()
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
                    KgNegocio: { type: "number", editable: false },
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
            { field: "MonedaId", title: "Moneda", width: "88px" },
            { field: "Precio", width: "88px", format: "{0:n0}" },
            { field: "KgNegocio", title: "Kg", width: "88px", format: "{0:n0}" },
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

function additionalInfo() {
    return {
        ComercialId: $("#ComercialSeleccionado1").val()
    }
}

function copiarCuposGenerados(elem) {
    var listaCupos = $(elem).parent().children('div').children().html().replace(/<br>/g, "\n");
    listaCupos = listaCupos.split('<strong>').join("");
    listaCupos = listaCupos.split('</strong>').join("");
    //como un replaceall 
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

function MostrarMensaje() {
    $("#mensajes").modal("show");
}

function verOcultar(el, classname, verSugerencias) {

    var clase = "fa fa-caret-square-o-down";
    console.log(classname);
    if ($(el).hasClass(clase)) {
        //ver
        $("." + classname).show("slice");
        $(el).removeClass("fa fa-caret-square-o-down")
        $(el).addClass("fa fa-caret-square-o-right")
    } else {
        //ocultar
        $("." + classname).hide("slice");
        $(el).removeClass("fa fa-caret-square-o-right")
        $(el).addClass("fa fa-caret-square-o-down")
    }
    if (verSugerencias == false) {
        $(".sugerencia").hide();
        var ps = document.getElementsByName("p" + classname);
        ps.forEach((p) => {
            if ($(p).hasClass("fa fa-caret-square-o-right")) {
                $(p).removeClass("fa fa-caret-square-o-right");
                $(p).addClass("fa fa-caret-square-o-down");
            }

        })

    }
    if ($('#proveedorCUIT').val() != "") {
        $('.pall').hide();
        $('.p' + $('#proveedorCUIT').val()).show();
    }
}

function closeAll() {
    $(".fa .fa-caret-square-o-down").each(function (index) {
        $(this).removeClass("fa fa-caret-square-o-right");
        $(this).addClass("fa fa-caret-square-o-down");
    });

    $(".todo").hide();

}