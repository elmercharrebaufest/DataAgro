var viewModel;
var fecha;
var zonaSeleccionada;
var fleteProcedencia;
var reasignarCupo;
var stockDisponible = true;

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
            '<p class="#:data.Corredor# buscar-nomb #: data.Estado != "" ? \'k-state-disabled\': \'\' #"  style="color:#: data.Color#" value="#:data.RazonSocial#" >#: data.RazonSocial#(#: data.Cuit#)#if(data.Estado != null) {# ' +
            ' #: data.Estado #    #}else{# #}# </p > ',
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
            if (e.dataItem.Deshabilitar) {
                if (e.dataItem.CuposConRiesgo == true && e.dataItem.RiesgoComercialSap == 'A') {
                    $("#Proveedor").val(e.dataItem.Id);
                } else {
                    $("#buscadorProveedor").val("")
                    e.preventDefault();
                }
            } else {
                $("#Proveedor").val(e.dataItem.Id);
            }

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

        var codigoSap = $(this).val();
        var result = MSExecuteOnServer("/Centro/ObtenerCentroPorCodigoSap", { codigoSap });
        if (result != null) {
            if (result.Acopio) {
                $("#flete").prop('checked', true);
                $("#zona").val("9");
            } else {
                $("#flete").prop('checked', false);
                $("#zona").val("0");
            }
        }

        //if ($(this).val() == "1074") {
        //    $("#flete").attr('disabled', 'disabled');
        //    $("#flete").prop('checked', false);
        //} else {
        //    $("#flete").removeAttr("disabled");
        //}

    });
    $("#Sustentable").change(function () {
        if ($("#Sustentable").is(':checked')) {
            $("#EPA").prop("checked", false).prop("disabled", false);
            $("#EUDR").prop("checked", false).prop("disabled", false);
        }
        MostrarVisualizarStock();
    });
    $("#EPA").change(function () {
        if ($("#EPA").is(':checked')) {
            $("#EUDR").prop("checked", true).prop("disabled", true);
            $("#Sustentable").prop("checked", false);
        } else {
            $("#EUDR").prop("checked", false).prop("disabled", false);
        }
        MostrarVisualizarStock();
    });
    $("#EUDR").change(function () {
        if ($("#EUDR").is(':checked')) {
            $("#EPA").prop("checked", true).prop("disabled", true);
            $("#Sustentable").prop("checked", false);
        } else {
            $("#EPA").prop("checked", false).prop("disabled", false);
        }
        MostrarVisualizarStock();
    });
    if ($("#Id").val() != null && $("#Id").val() != "0") {

        $("#material").attr('disabled', 'disabled');
        $("#planta").attr('disabled', 'disabled');
        $("#fechaEntrega").data('kendoDatePicker').enable(false);
        $("#fechaHasta").data('kendoDatePicker').enable(false);
        $("#cantidad").data('kendoNumericTextBox').enable(false);
        $("#cantidad").data('kendoNumericTextBox').value(1);
        $("#zona").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        if ($("#NoPropio").val() == "True") {
            $("#flete").prop('checked', false);
        }
        if (reasignarCupo == false) {
            $("#buscadorProveedor").attr('disabled', 'disabled');
        }
        if ($("#flete").is(':checked')) {
            MensInfo("Cupo con condición de Flete");
        }
        $("#guardarBtn").attr('type', 'button');
        $("#guardarBtn").click(function () {
            if (fleteProcedencia && $("#NoPropio").val() == "False") {
                if ($("#flete").is(':checked')) {
                    $("#msjConfirmacion").html("El cupo tiene flete procedencia, ¿desea mantener esta condición?");
                } else {
                    $("#msjConfirmacion").html("El cupo no tiene flete procedencia, ¿desea mantener esta condición?");
                }
                $('#fleteProcedenciaModal').modal('toggle');
            } else {
                $("#flete").removeAttr('disabled');
                //$("form").submit();
                GuardarCupo();
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
            //$("form").submit();
            GuardarCupo();
        });
        $("#boton-no").click(function () {
            $("#flete").removeAttr('disabled');
            if ($("#flete").is(':checked')) {
                $("#flete").prop('checked', false);
            } else {
                $("#flete").prop('checked', true);
            }
            $('#fleteProcedenciaModal').modal('toggle');
            //$("form").submit();
            GuardarCupo();
        });
        $("#boton-cancelar").click(function () {
            window.location.href = window.location.origin + "/Cupo/";
        });
    } else {
        $("#guardarBtn").click(function () {
            GuardarCupo();
        });
    }
    $("#fleteProcedenciaModal").draggable({
        handle: ".modal-header"
    });

    if ($("#fechaHasta").val() != $("#fechaEntrega").val()) {
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
                    return { cuitProveedor: cuitP[0], materialId: $('#material').val(), centro: $('#planta').val(), filtro: $('#contratoId').val(), desde: $('#fechaEntrega').val(), hasta: $('#fechaHasta').val() };
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
    MostrarVisualizarStock();
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
        $("#sustentableDiv").hide();
        $("#Sustentable").prop("checked", false);
        $("#EPADiv").hide();
        $("#EPA").prop("checked", false);
        $("#EUDRDiv").hide();
        $("#EUDR").prop("checked", false);
    } else {
        $("#calidadDiv").show();
        if ($("#planta").val() == "1029") {
            $("#sustentableDiv").show();
            $("#EPADiv").show();
            $("#EUDRDiv").show();
        } else {
            $("#sustentableDiv").hide();
            $("#Sustentable").prop("checked", false);
            $("#EPADiv").hide();
            $("#EPA").prop("checked", false);
            $("#EUDRDiv").hide();
            $("#EUDR").prop("checked", false);
        }
    }
}

function cuposCreados(error, lista) {
    $(document).ready(function () {
        var listaError = JSON.parse(error);
        if (listaError.length > 0) {
            $("#error-modal").html(makeUL(listaError));
            $("#error-modal").show();
            MostrarVisualizarStock();
        }
        if ($("#MaterialId").val() == 2 && lista.length > 0) {
            lista.unshift("Trigo libre de HB4");
        }
        //if ($("#buscadorProveedor").val() != "" && $("#planta").val() == "1600" && $("#material").val() == "3") {
        if ($("#buscadorProveedor").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#planta").val() == "1029" && $("#material").val() == "3") {
            MostrarVisualizarStock();
            VisualizarStock(true);
            $("#copy_btn2").show();
        } else {
            $("#copy_btn2").hide();
        }
        $("#cupos-generados-modal").html(lista.join("</br>"));
        $('#resultadoCupo').modal('toggle');

        $("#resultadoCupo").on("hidden.bs.modal", function () {
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
    var esEPAoEUDR = $("#EPA").is(':checked') || $("#EUDR").is(':checked');
    $("#copiar-cupos-generados-modal").empty();
    $("#copiar-cupos-generados-modal").append($("#cupos-generados-modal").html());

    //if ($("#buscadorProveedor").val() != "" && $("#planta").val() == "1600" && $("#material").val() == "3") {
    if ($("#buscadorProveedor").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#planta").val() == "1029" && $("#material").val() == "3") {
        var cuitProv = $("#buscadorProveedor").val().split('(');
        if (cuitProv[1] != null) {
            var cuitP = cuitProv[1].split(')');
        }
        else {
            cuitP = cuitProv;
        }
        var result = MSExecuteOnServer('/Cupo/TraerEstablecimientos', { cuitProveedor: cuitP[0], esEPAoEUDR: esEPAoEUDR });
        if (result != null && result.length > 0) {
            var table = "<pre style='border: 0;    background-color: transparent;'>";
            table += '<div colspan = "">Cosecha ' + result[0].Cosecha + '</div>';
            table += "<div>" + "Establecimiento".padEnd(25, ' ') + "Cantidad (Kg)".padEnd(25, ' ') + "Localidad (Provincia)" + " </div>"
            for (var i = 0; i < result.length; i++) {
                table += "<div>";
                table += '<div>' + result[i].Establecimiento.padEnd(25, ' ') + kendo.toString(result[i].Cantidad, "n0").padEnd(25, ' ') + result[i].Localidad + ' (' + result[i].Provincia + ')' + '</div>';
                table += "</div>";
            }
            table += "</pre>";

            $("#copiar-cupos-generados-modal").append(table);
        }
    }

    //var listaCupos = $("#copiar-cupos-generados-modal").html().replace(/<br>/g, "\n");
    //var copy = function (e) {
    //    e.preventDefault();
    //    console.log('copy');

    //    if (e.clipboardData) {
    //        e.clipboardData.setData('text/plain', listaCupos);
    //    } else if (window.clipboardData) {
    //        window.clipboardData.setData('Text', listaCupos);
    //    }
    //};
    //window.addEventListener('copy', copy);
    //document.execCommand('copy');
    //window.removeEventListener('copy', copy);c
    selectElementContents(document.getElementById('copiar-cupos-generados-modal'));
    setTimeout(function () {
        $("#copiar-cupos-generados-modal").empty();
    }, 50);
}
function selectElementContents(el) {
    var body = document.body,
        range, sel;
    if (document.createRange && window.getSelection) {
        range = document.createRange();
        sel = window.getSelection();
        sel.removeAllRanges();
        range.selectNodeContents(el);
        sel.addRange(range);
    }
    document.execCommand("Copy");
}
function MostrarCarga() {
    $("#CargaCupos").modal('toggle');
}

function CrearTablaFechaHasta() {
    $(".fila-carga").remove();

    var date1 = $("#fechaEntrega").val();
    var date2 = $("#fechaHasta").val();
    var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

    for (var i = 0; i <= diffDays; i++) {

        var fila = '<tr class="fila-carga"><input name="Dias[' + i + '].Fecha" value="' + date1 + '" type="hidden"/><td>' + date1 + '</td><td><input name="Dias[' + i + '].Cantidad" class="cantidad-masiva" value="' + $("#cantidad").data('kendoNumericTextBox').value() + '"/></td></tr>';
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
        for (var i = 0; i < cantidadDias.length; i++) {
            $('[name="Dias[' + i + '].Cantidad"]').data('kendoNumericTextBox').value(cantidadDias[i].Cantidad);
        }
    });
}

function copiarTablaEstablecimiento() {
    //var copiarEstablecimientos = document.getElementById("cargarDatosEstablecimiento").innerText;
    //var copy = function (e) {
    //    e.preventDefault();
    //    console.log('copy');

    //    if (e.clipboardData) {
    //        e.clipboardData.setData('text/plain', copiarEstablecimientos);
    //    } else if (window.clipboardData) {
    //        window.clipboardData.setData('Text', copiarEstablecimientos);
    //    }
    //};
    //window.addEventListener('copy', copy);
    //document.execCommand('copy');
    //window.removeEventListener('copy', copy);
    copiarImagen();
}

function VisualizarStock(noabrir) {
    var esEPAoEUDR = $("#EPA").is(':checked') || $("#EUDR").is(':checked');
    var cuitProv = $("#buscadorProveedor").val().split('(');
    if (cuitProv[1] != null) {
        var cuitP = cuitProv[1].split(')');
    }
    else {
        cuitP = cuitProv;
    }
    var result = MSExecuteOnServer('/Cupo/TraerEstablecimientos', { cuitProveedor: cuitP[0], esEPAoEUDR: esEPAoEUDR });
    if (result != null && result.length > 0) {
        stockDisponible = true;
        var table = "<tr>";
        table += '<th colspan = "3">Cosecha ' + result[0].Cosecha + '</th>';
        table += "</tr>";
        table += "<tr>";
        table += "<th> Establecimiento</th>"
        table += "<th> Cantidad (Kg)</th>"
        table += "<th> Localidad (Provincia) </th>"
        table += "</tr>";
        for (var i = 0; i < result.length; i++) {
            table += "<tr>";

            table += '<td>' + result[i].Establecimiento + '</td>';
            table += '<td>' + kendo.toString(result[i].Cantidad, "n0") + '</td>';
            table += '<td>' + result[i].Localidad + ' (' + result[i].Provincia + ')' + '</td>';
            table += "</tr>";
        }

        $("#cargarDatosEstablecimiento").html(table);
        if (noabrir != true) {
            $("#modalEstablecimientos").modal("show");
        }
    } else {
        stockDisponible = false;
        if (noabrir != true) {
            MensErr("No se encontraron establecimientos con stock disponible")
        }
    }

}

function MostrarVisualizarStock() {
    if ($("#buscadorProveedor").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#planta").val() == "1029" && $("#material").val() == "3") {
        $("#stock").show();
    } else {
        $("#stock").hide();
    }
}
function imageToBlob(imageURL) {
    const img = new Image;
    const c = document.createElement("canvas");
    const ctx = c.getContext("2d");
    img.crossOrigin = "";
    img.src = imageURL;
    return new Promise(resolve => {
        img.onload = function () {
            c.width = this.naturalWidth;
            c.height = this.naturalHeight;
            ctx.drawImage(this, 0, 0);
            c.toBlob((blob) => {
                // here the image is a blob
                resolve(blob)
            }, "image/png", 0.75);
        };
    })
}

async function copyImage(imageURL) {
    const blob = await imageToBlob(imageURL)
    const item = new ClipboardItem({ "image/png": blob });
    navigator.clipboard.write([item]);
}

function copiarImagen() {
    html2canvas($("#cargarDatosEstablecimiento")[0]).then(function (canvas) {
        let image = new Image();
        image.src = canvas.toDataURL();
        $("#out_image").append(image);
        copyImage(image.src);
        $("#out_image").empty();
    }
    );
}


function GuardarCupo() {
    if ($("#buscadorProveedor").val() != "" && ($("#Sustentable").is(':checked') || $("#EPA").is(':checked') || $("#EUDR").is(':checked')) && $("#planta").val() == "1029" && $("#material").val() == "3") {
        VisualizarStock(true);
    } else {
        stockDisponible = true;
    }

    if (stockDisponible) {

        BlockUi("Grabando...");
        setTimeout(function () {
            var date1 = $("#fechaEntrega").val();
            var date2 = $("#fechaHasta").val();
            var diffDays = parseInt((kendo.parseDate(date2) - kendo.parseDate(date1)) / (1000 * 60 * 60 * 24), 10);

            var dias = [];
            for (var i = 0; i <= diffDays; i++) {
                dias.push({ Fecha: $('[name="Dias[' + i + '].Fecha"]').val(), Cantidad: $('[name="Dias[' + i + '].Cantidad"]').val() })
            }



            var cupo = {
                Id: $("#Id").val(),
                Siguientes: $("#Siguientes").val(),
                ProveedorDescripcion: $("#buscadorProveedor").val(),
                Proveedor: $("#Proveedor").val(),
                PlantaId: $("#planta").val(),

                FechaEntrega: $("#fechaEntrega").val(),
                FechaHastaEntrega: $("#fechaHasta").val(),
                CantidadCupos: $("#cantidad").val(),

                Zona: $("#Zona").val(),
                ZonaId: $("#zona").val(),
                FleteAcarreo: $("#flete").is(':checked'),
                CalidadId: $("#calidad").val(),
                Observacion: $("#observacion").val(),
                FasonId: $("#fason").is(':checked'),
                CuitId: $("#cuit").val(),
                ConDescarga: $("#ConDescarga").is(':checked'),
                Dias: dias,
                //CupoResult Resultado :
                MaterialId: $("#material").val(),
                Negocio: $("#Negocio").val(),
                NegocioId: $("#NegocioId").val(),

                NoPropio: $("#NoPropio").val(),
                FechaIngreso: $("#FechaIngreso").val(),
                CuposNoPropios: $("#CuposNoPropios").val(),

                Sustentable: $("#Sustentable").is(':checked'),
                EPA: $("#EPA").is(':checked'),
                EUDR: $("#EUDR").is(':checked')
            };

            var resultado = MSExecuteOnServer('/Cupo/GuardarCupo', { cupo });
            if (resultado.Result.Resultado != null) {
                if (resultado.irA == "") {
                    //var errorCantidadCuposSAP = resultado.Result.Resultado.ListaErrores.filter(x => x.Source == "CantidadCuposSAP");
                    var errores = [];
                    //if (errorCantidadCuposSAP != undefined) {
                    //    errores.push(errorCantidadCuposSAP.Message);
                    //}
                    for (var i = 0; i < resultado.Error.ListaErrores.length; i++) {
                        errores.push(resultado.Error.ListaErrores[i].Message);
                    }
                    if (resultado.Result.Resultado.ListaCupos.length > 0) {
                        cuposCreados(JSON.stringify(errores), resultado.Result.Resultado.ListaCupos);
                    } else {
                        var listaErr = [];
                        for (var i = 0; i < resultado.Error.ListaErrores.length; i++) {
                            listaErr.push(resultado.Error.ListaErrores[i].Message);
                        }
                        $("#errorDiv1").html(makeUL(listaErr));
                        $("#errorDiv").show();
                        $.unblockUI();
                    }
                }
                else {
                    window.location.href = window.location.origin + resultado.irA;
                }
                $.unblockUI();
            } else {
                var listaErr = [];
                for (var i = 0; i < resultado.Error.ListaErrores.length; i++) {
                    listaErr.push(resultado.Error.ListaErrores[i].Message);
                }
                $("#errorDiv1").html(makeUL(listaErr));
                $("#errorDiv").show();
                $.unblockUI();
                //MostrarVisualizarStock();
            }
        }, 1000);

    } else {
        MensErr("No se pudo guardar porque no existen establecimientos con stock disponible");
    }
}