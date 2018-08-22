var cantGrano = 0;
var capProdCant = 0;
var aGuardar = [];
var cantGranoAlmacenamiento = 0;
var cantGranoAlmacenamientoGrano = 0;
var capProdCantAlmacenamiento = 0;
var aGuardarAlmacenamiento = [];
var mantenerArchivos = [];
var mantenerArchivosAlmacenamiento = [];
var resultInit = {};
var cantContactoComercial = 0;
var aGuardarContactoComercial = [];
var resultCampaña = [];
var resultGranos = [];
var cantGranoObjetivo = 0;
var aEliminarObjetivos = [];
var aEliminarGranos = [];
var aEliminarGranosAlmacenamiento = [];
var aEliminarGranosAlmacenamientoGrano = [];

function eliminarGrano(elem) {
    var val = $(elem).prop("id").split("eliminarGrano")[1];

    var obj = {
        granoId: $("#grano" + val).val(),
        grano: $("#grano" + val).find('option:selected').text(),
        campañaId: $("#campaña" + val).val(),
        campaña: $("#campaña" + val).find('option:selected').text(),
        hectareas: $("#hectareas" + val).val(),
        toneladas: $("#toneladas" + val).val(),
        CampoId: $("#campoid").val()
    };

    if (obj.granoId != "null" && obj.campañaId != "null")
        aEliminarGranos.push(obj);

    if (val > 0) {
        $(".linea" + val).remove();
    } else {
        $("#grano" + val).val("null");
        $("#campaña" + val).val("null");
        $("#toneladas" + val).val("");
        $("#hectareas" + val).val("");
    }
}

function eliminarObjetivo(elem) {
    var val = $(elem).prop("id").split("eliminarObjetivo")[1];

    var obj = {
        granoId: $("#granoObjetivo" + val).val(),
        grano: $("#granoObjetivo" + val).find('option:selected').text(),
        campañaId: $("#campañaObjetivo" + val).val(),
        campaña: $("#campañaObjetivo" + val).find('option:selected').text(),
        toneladasObjetivo: $("#toneladasObjetivo" + val).val()
    };

    if (obj.granoId != "null" && obj.campañaId != "null")
        aEliminarObjetivos.push(obj);

    if (val > 0) {
        $(".lineaObjetivos" + val).remove();
    } else {
        $("#granoObjetivo" + val).val("null"),
            $("#campañaObjetivo" + val).val("null"),
            $("#toneladasObjetivo" + val).val("")
    }
}

function eliminarGranoAlmacenamiento(elem) {
    var val = $(elem).prop("id").split("eliminarGrano-almacenamiento")[1];

    var obj = {
        campañaId: $("#campañaAlmacenamiento" + val).val(),
        campaña: $("#campañaAlmacenamiento" + val).find('option:selected').text(),
        toneladasAlmacenamiento: $("#toneladasAlmacenamiento" + val).val(),
        hasArrendadas: $("#hectareas-propias" + val).is(":checked") ? 0 : ($("#hectareas-arrendadas" + val).is(":checked") ? 1 : null),
        hasArrendadasNombre: $("#hectareas-propias" + val).is(":checked") ? "Propias" : ($("#hectareas-arrendadas" + val).is(":checked") ? "Alquiladas" : "no especifica"),
        CampoId: $("#campoid").val()
    };

    aEliminarGranosAlmacenamiento.push(obj);

    if (val > 0) {
        $(".lineaAlmacenamiento" + val).remove();
        val--;
    } else {
        $("#campañaAlmacenamiento" + val).val("null");
        $("#toneladasAlmacenamiento" + val).val("");
        $("#hectareas-arrendadas" + val).prop("checked", false);
        $("#hectareas-propias" + val).prop("checked", false);
    }
}

function eliminarGranoAlmacenamientoGrano(elem) {
    var val = $(elem).prop("id").split("eliminarGrano-almacenamientograno")[1];
    var obj = {
        campañaId: $("#campañaAlmacenamientoGranos" + val).val(),
        campaña: $("#campañaAlmacenamientoGranos" + val).find('option:selected').text(),
        granoId: $("#granoAlmacenamientoGranos" + val).val(),
        grano: $("#granoAlmacenamientoGranos" + val).find('option:selected').text(),
        toneladasAlmacenamiento: $("#toneladasAlmacenamientoGrano" + val).val(),
        CampoId: $("#campoid").val()
    };

    aEliminarGranosAlmacenamientoGrano.push(obj);

    if (val > 0) {
        $(".lineaAlmacenamientoGranos" + val).remove();
        val--;
    } else {
        $("#campañaAlmacenamientoGranos" + val).val("null");
        $("#granoAlmacenamientoGranos" + val).val("null");
        $("#toneladasAlmacenamientoGrano" + val).val("");
    }
}

var mostrarTooltip = function (el) {
    $(el).parent().find($(".lista-contacto-no-operable-tooltip")).show();
    $(el).parent().find($(".lista-contacto-no-operable-tooltip-arrow")).show();
}
var ocultarTooltip = function (el) {
    $(el).parent().find($(".lista-contacto-no-operable-tooltip")).hide();
    $(el).parent().find($(".lista-contacto-no-operable-tooltip-arrow")).hide();
}

$(document).ready(function () {
    kendo.culture("es-AR");
    armarFuncionalidades();
    InicializarDatos();
});

function buscarLocalidad(val) {
    var data = { Id: val };
    var result = MSExecuteOnServer('/Proveedor/TraerLocalidad', data);

    $(".campo-localidad").empty();
    var htmlLocalidad = "";
    htmlLocalidad += '<select class="campo-input-select campo-sin-span" id="localidad">';
    htmlLocalidad += '<option value = "null">Seleccione...</option>';
    for (var ii in result) {
        (function (i) {
            htmlLocalidad += '<option value="' + result[i].LocalidadId + '">' + result[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidad += '</select>';
    $(".campo-localidad").append(htmlLocalidad);
}

function buscarLocalidadProduccion(val) {
    var data = { Id: val };
    var result = MSExecuteOnServer('/Proveedor/TraerLocalidad', data);
    $(".campo-localidad-produccion").empty();
    var htmlLocalidadProduccion = "";
    htmlLocalidadProduccion += '<select class="campo-input-select campo-sin-span-produccion" id="localidad-produccion">';
    for (var ii in result) {
        (function (i) {
            htmlLocalidadProduccion += '<option value="' + result[i].LocalidadId + '">' + result[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidadProduccion += '</select>';
    $(".campo-localidad-produccion").append(htmlLocalidadProduccion);
}

function buscarLocalidadAlmacenamiento(val) {
    var data = { Id: val };
    var result = MSExecuteOnServer('/Proveedor/TraerLocalidad', data);
    $(".campo-localidad-almacenamiento").empty();
    var htmlLocalidadAlmacenamiento = "";
    htmlLocalidadAlmacenamiento += '<select  class="campo-input-select campo-sin-span-produccion" id="localidad-almacenamiento">';
    for (var ii in result) {
        (function (i) {
            htmlLocalidadAlmacenamiento += '<option value="' + result[i].LocalidadId + '">' + result[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidadAlmacenamiento += '</select>';
    $(".campo-localidad-almacenamiento").append(htmlLocalidadAlmacenamiento);
}

function buscarLocalidadCompraNet(val) {
    var data = { Id: val };
    var result = MSExecuteOnServer('/Proveedor/TraerLocalidad', data);
    $(".campo-localidad-compranet").empty();
    var buscarLocalidadCompraNet = "";
    buscarLocalidadCompraNet += '<span class="campo-span">Localidad CompraNet</span> <select  class="campo-input-select" id="localidad-compranet">';
    buscarLocalidadCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result) {
        (function (i) {
            buscarLocalidadCompraNet += '<option value="' + result[i].LocalidadId + '">' + result[i].Nombre + '</option>';
        })(ii);
    }
    buscarLocalidadCompraNet += '</select>';
    $(".campo-localidad-compranet").append(buscarLocalidadCompraNet);
}

function buscarRazonSocial(val) {
    var data = { cuit: val };
    var result = MSExecuteOnServer('/Proveedor/TraerRazonSocial', data);
    if (result) {
        if (result.Existe == 1) {
            MensInfo("El CUIT ya existe");
        }
    }
    $("#razonsocial").val(result.razonSocial);
    $(".campo-estadoafip-span-operable").html(result.Operable ? "Operable" : "No Operable");
    if (result.Operable === 0) {
        $(".span-contacto-no-operable-tooltip").html(result.Condicion);
        $("#Operable-agregar").show();
    }
    if (result.Operable) {
        $("#imgOperable").attr("src", "../Content/Images/operableafip.png");
        $(".campo-estadoafip").css({
            'background-color': 'rgba(150, 235, 198, 0.45)'
        });
    }
    else {
        $("#imgOperable").attr("src", "../Content/Images/cancelar.png");
        $(".campo-estadoafip").css({
            'background-color': 'rgba(239, 105, 105, 0.45)'
        });
    }
}

function armarSelects(result) {
    var grupos = {};
    for (var jj in result.segm) {
        (function (j) {
            grupos[result.segm[j].Grupo] = grupos[result.segm[j].Grupo] || [];
            grupos[result.segm[j].Grupo].push({
                SegmentacionId: result.segm[j].SegmentacionId,
                Descripcion: result.segm[j].Descripcion
            });
        })(jj);
    }

    var htmlSegmentacion = "";
    htmlSegmentacion += '<select class="campo-input-select" id="segmentacion">';
    if (grupos[null]) {
        var nulo = grupos[null];
        for (var ii in nulo) {
            (function (i) {
                htmlSegmentacion += '<option value="' + nulo[i].SegmentacionId + '">' + nulo[i].Descripcion + '</option>';
            })(ii);
        }
    }
    for (var ii in grupos) {
        (function (i) {
            if (i != "null") {
                htmlSegmentacion += '<optgroup label="' + i + '">';
                for (var jj in grupos[i]) {
                    (function (j) {
                        htmlSegmentacion += '<option value="' + grupos[i][j].SegmentacionId + '">' + grupos[i][j].Descripcion + '</option>';
                    })(jj);
                }
                htmlSegmentacion += '</optgroup>';
            }
        })(ii);
    }
    htmlSegmentacion += '</select>';

    $(".campo-segmentacion").append(htmlSegmentacion);

    var htmlTipoTelefono = "";
    htmlTipoTelefono += '<select  class="campo-input-select" id="TipoTelefono1">';
    htmlTipoTelefono += '<option value = "null">Seleccione...</option>';

    var htmlTipoTelefonocc = "";
    htmlTipoTelefonocc += '<select  class="campo-input-select" id="concom-TipoTelefono1">';
    htmlTipoTelefonocc += '<option value = "null">Seleccione...</option>';

    for (var ii in result.tiptel) {
        (function (i) {
            htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
            if (result.tiptel[i].Descripcion != "Otros" && result.tiptel[i].Descripcion != "Emergencia") {
                htmlTipoTelefonocc += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
            }
        })(ii);
    }
    htmlTipoTelefono += '<input type="text" placeholder="Telefono" class="campo-input-text" id="Telefono1" />'
        + '<img src="../Content/Images/agregar-tel-mail.png" id="agregarTelefono" />'
        + '</select>';
    $(".campo-tiptelefono").append(htmlTipoTelefono);

    htmlTipoTelefonocc += '<input type="text" placeholder="Telefono" class="campo-input-text" id="concom-Telefono1" />'
        + '<img src="../Content/Images/agregar-tel-mail.png" id="concom-agregarTelefono" />'
        + '</select>';
    $(".concom-campo-telefono").append(htmlTipoTelefonocc);

    var anioActual = new Date().getFullYear();
    var htmlAnio = "";
    htmlAnio += '<option value="null">Año</option>';
    for (var i = 1950; i <= anioActual; i++) {
        htmlAnio += '<option value="' + i + '">' + i + '</option>';
    }
    $("#concom-anionacimientomes").append(htmlAnio);

    var htmlProvincia = "";
    htmlProvincia += '<select  class="campo-input-select campo-sin-span" id="provincia">';
    htmlProvincia += '<option value = "null">Seleccione...</option>';
    result.prov.sort(function (a, b) {
        var textA = a.Nombre.toUpperCase();
        var textB = b.Nombre.toUpperCase();
        return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
    });
    for (var ii in result.prov) {
        (function (i) {
            htmlProvincia += '<option value="' + result.prov[i].Provinciaid + '">' + result.prov[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvincia += '</select>';
    $(".campo-provincia").append(htmlProvincia);

    var htmlLocalidad = "";
    htmlLocalidad += '<select   class="campo-input-select campo-sin-span" id="localidad">';
    htmlLocalidad += '<option value = "null">Seleccione...</option>';
    for (var ii in result.loc) {
        (function (i) {
            htmlLocalidad += '<option value="' + result.loc[i].LocalidadId + '">' + result.loc[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidad += '</select>';
    $(".campo-localidad").append(htmlLocalidad);

    var htmlCanalOperacion = "";
    htmlCanalOperacion += '<select multiple class="campo-input-select" id="canopera">';
    for (var ii in result.cope) {
        (function (i) {
            htmlCanalOperacion += '<option value="' + result.cope[i].CanalOperacionId + '">' + result.cope[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCanalOperacion += '</select>';
    $(".campo-operacion").append(htmlCanalOperacion);

    var htmlDestinatario = "";
    htmlDestinatario += '<select multiple class="campo-input-select"  id="entregaA">';
    for (var ii in result.dest) {
        (function (i) {
            htmlDestinatario += '<option value="' + result.dest[i].DestinatarioId + '">' + result.dest[i].Descripcion + '</option>';
        })(ii);
    }
    htmlDestinatario += '</select>';
    $(".campo-destinatario").append(htmlDestinatario);

    var htmlCondicion = "";
    htmlCondicion += '<select multiple class="campo-input-select"  id="condPrefer">';
    for (var ii in result.cond) {
        (function (i) {
            htmlCondicion += '<option value="' + result.cond[i].CondicionId + '">' + result.cond[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCondicion += '</select>';
    $(".campo-condicion").append(htmlCondicion);

    resultCampaña = MSExecuteOnServer('/Proveedor/TraerCampañasActivas');

    var htmlCampañaGrano = "";
    htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="grano0">';
    htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
    for (var ii in result.gran) {
        (function (i) {
            htmlCampañaGrano += '<option value="' + result.gran[i].MaterialId + '">' + result.gran[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCampañaGrano += '</select>';
    htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="campaña0">';
    htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
    htmlCampañaGrano += '</select>'
        + '<input type="text" class="campo-input-text hectareas" id="hectareas0" />'
        + '<input type="text" class="campo-input-text toneladas" id="toneladas0" />';
    $(".campo-granos").append(htmlCampañaGrano);

    $("#grano0").change(function (x) {
        var obj = {
            MaterialId: $(this).val(),
            elemId: $(this).prop("id").split("grano")[1]
        };
        armarSelectGrano(obj);
    });

    var htmlProvinciaProduccion = "";
    htmlProvinciaProduccion += '<select class="campo-input-select campo-sin-span-produccion" id="provincia-produccion">';
    htmlProvinciaProduccion += '<option value = "null">Seleccione...</option>';
    for (var ii in result.prov) {
        (function (i) {
            htmlProvinciaProduccion += '<option value="' + result.prov[i].Provinciaid + '">' + result.prov[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvinciaProduccion += '</select>';
    $(".campo-provincia-produccion").append(htmlProvinciaProduccion);

    var htmlLocalidadProduccion = "";
    htmlLocalidadProduccion += '<select  class="campo-input-select campo-sin-span-produccion" id="localidad-produccion">';
    htmlLocalidadProduccion += '<option value = "null">Seleccione...</option>';
    for (var ii in result.loc) {
        (function (i) {
            htmlLocalidadProduccion += '<option value="' + result.loc[i].LocalidadId + '">' + result.loc[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidadProduccion += '</select>';
    $(".campo-localidad-produccion").append(htmlLocalidadProduccion);

    var htmlCampañaGranoAlmacenamiento = "";
    htmlCampañaGranoAlmacenamiento += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamiento0">';
    htmlCampañaGranoAlmacenamiento += '<option value = "null">Seleccione...</option>';
    for (var ii in resultCampaña) {
        (function (i) {
            htmlCampañaGranoAlmacenamiento += '<option value="' + resultCampaña[i].CampañaId + '">' + resultCampaña[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCampañaGranoAlmacenamiento += '</select>'
        + '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamiento0" />';
    htmlCampañaGranoAlmacenamiento += '<div class="almacenamiento-hectareas-linea">' +
        '<div class="formulario-campo-radio">' +
        '<label for="hectareas-arrendadas0"><input id="hectareas-arrendadas0" type="radio" name="hectareas0" value="1"> Alquiladas</label>' +
        '<label for="hectareas-propias0"><input id="hectareas-propias0" type="radio" name="hectareas0" value="0"> Propias</label>' +
        '</div>' +

        '</div>' +
        '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamiento" id="eliminarGrano-almacenamiento0" />' +
        '</div>';

    $(".campo-granos-almacenamiento").append(htmlCampañaGranoAlmacenamiento);

    $("#campañaAlmacenamiento0").change(function (x) {
        var obj = {
            CampañaId: $(this).val(),
            elemId: $(this).prop("id").split("campañaAlmacenamiento")[1]
        };
        armarSelectGranoAlmacenamiento(obj);
    });

    var htmlCampañaGranoAlmacenamientoGrano = "";
    htmlCampañaGranoAlmacenamientoGrano += '<select class="campo-input-select campo-sin-span grano" id="granoAlmacenamientoGranos0">';
    htmlCampañaGranoAlmacenamientoGrano += '<option value = "null">Seleccione...</option>';
    for (var ii in resultInit.gran) {
        (function (i) {
            htmlCampañaGranoAlmacenamientoGrano += '<option value="' + resultInit.gran[i].MaterialId + '">' + resultInit.gran[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCampañaGranoAlmacenamientoGrano += '</select>';
    htmlCampañaGranoAlmacenamientoGrano += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamientoGranos0">';
    htmlCampañaGranoAlmacenamientoGrano += '<option value = "null">Seleccione...</option>';
    htmlCampañaGranoAlmacenamientoGrano += '</select>'
        + '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamientoGrano0" />'
        + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamientograno" id="eliminarGrano-almacenamientograno0" />';

    $(".campo-granos-almacenamientograno").append(htmlCampañaGranoAlmacenamientoGrano);

    $("#granoAlmacenamientoGranos0").change(function (x) {
        var obj = {
            MaterialId: $(this).val(),
            elemId: $(this).prop("id").split("granoAlmacenamientoGranos")[1]
        };
        armarSelectGranoAlmacenamientoGrano(obj);
    });

    $("#eliminarGrano-almacenamientograno0").click(function () {
        console.log("clickeo");
        var val = 0;
        var obj = {
            campañaId: $("#campañaAlmacenamientoGranos" + val).val(),
            campaña: $("#campañaAlmacenamientoGranos" + val).find('option:selected').text(),
            granoId: $("#granoAlmacenamientoGranos" + val).val(),
            grano: $("#granoAlmacenamientoGranos" + val).find('option:selected').text(),
            toneladasAlmacenamiento: $("#toneladasAlmacenamientoGrano" + val).val(),
            CampoId: $("#campoid").val()
        };

        aEliminarGranosAlmacenamientoGrano.push(obj);

        if (val > 0) {
            $(".lineaAlmacenamientoGranos" + val).remove();
            val--;
        } else {
            $("#campañaAlmacenamientoGranos" + val).val("null");
            $("#granoAlmacenamientoGranos" + val).val("null");
            $("#toneladasAlmacenamientoGrano" + val).val("");
        }
    });

    $("#eliminarGrano-almacenamiento0").click(function () {
        var val = 0;
        var obj = {
            campañaId: $("#campañaAlmacenamiento" + val).val(),
            campaña: $("#campañaAlmacenamiento" + val).find('option:selected').text(),
            toneladasAlmacenamiento: $("#toneladasAlmacenamiento" + val).val(),
            hasArrendadas: $("#hectareas-propias" + val).is(":checked") ? 0 : ($("#hectareas-arrendadas" + val).is(":checked") ? 1 : null),
            hasArrendadasNombre: $("#hectareas-propias" + val).is(":checked") ? "Propias" : ($("#hectareas-arrendadas" + val).is(":checked") ? "Alquiladas" : "no especifica"),
            CampoId: $("#campoid").val()
        }

        aEliminarGranosAlmacenamiento.push(obj);

        if (val > 0) {
            $(".lineaAlmacenamiento" + val).remove();
            val--;
        } else {
            $("#campañaAlmacenamiento" + val).val("null");
            $("#toneladasAlmacenamiento" + val).val("");
            $("#hectareas-arrendadas" + val).prop("checked", false);
            $("#hectareas-propias" + val).prop("checked", false);
        }
    });

    var htmlProvinciaAlmacenamiento = "";
    htmlProvinciaAlmacenamiento += '<select class="campo-input-select campo-sin-span-produccion" id="provincia-almacenamiento">';
    htmlProvinciaAlmacenamiento += '<option value = "null">Seleccione...</option>';
    for (var ii in result.prov) {
        (function (i) {
            htmlProvinciaAlmacenamiento += '<option value="' + result.prov[i].Provinciaid + '">' + result.prov[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvinciaAlmacenamiento += '</select>';
    $(".campo-provincia-almacenamiento").append(htmlProvinciaAlmacenamiento);

    var htmlLocalidadAlmacenamiento = "";
    htmlLocalidadAlmacenamiento += '<select class="campo-input-select campo-sin-span-produccion" id="localidad-almacenamiento">';
    htmlLocalidadAlmacenamiento += '<option value = "null">Seleccione...</option>';
    for (var ii in result.loc) {
        (function (i) {
            htmlLocalidadAlmacenamiento += '<option value="' + result.loc[i].LocalidadId + '">' + result.loc[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidadAlmacenamiento += '</select>';
    $(".campo-localidad-almacenamiento").append(htmlLocalidadAlmacenamiento);

    var htmlProvinciaCompraNet = "";
    htmlProvinciaCompraNet += '<select class="campo-input-select" id="provincia-compranet">';
    htmlProvinciaCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result.prov) {
        (function (i) {
            htmlProvinciaCompraNet += '<option value="' + result.prov[i].Provinciaid + '">' + result.prov[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvinciaCompraNet += '</select>';
    $(".campo-provincia-compranet").append(htmlProvinciaCompraNet);

    var htmlLocalidadCompraNet = "";
    htmlLocalidadCompraNet += '<select class="campo-input-select" id="localidad-compranet">';
    htmlLocalidadCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result.loc) {
        (function (i) {
            htmlLocalidadCompraNet += '<option value="' + result.loc[i].LocalidadId + '">' + result.loc[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidadCompraNet += '</select>';
    $(".campo-localidad-compranet").append(htmlLocalidadCompraNet);

    var htmlClasificacionCompraNet = "";
    htmlClasificacionCompraNet += '<select class="campo-input-select" id="clasificacion-compranet">';
    htmlClasificacionCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result.ClasComNet) {
        (function (i) {
            htmlClasificacionCompraNet += '<option value="' + result.ClasComNet[i].Id + '">' + result.ClasComNet[i].Descripcion + '</option>';
        })(ii);
    }
    htmlClasificacionCompraNet += '</select>';
    $(".campo-clasificacion-compranet").append(htmlClasificacionCompraNet);

    var htmlBoletoCompraNet = "";
    htmlBoletoCompraNet += '<select class="campo-input-select" id="boleto-compranet">';
    htmlBoletoCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result.BoleComNet) {
        (function (i) {
            htmlBoletoCompraNet += '<option value="' + result.BoleComNet[i].Id + '">' + result.BoleComNet[i].Descripcion + '</option>';
        })(ii);
    }
    htmlBoletoCompraNet += '</select>';
    $(".campo-tipo-boleto-compranet").append(htmlBoletoCompraNet);

    var htmlBolsaCompraNet = "";
    htmlBolsaCompraNet += '<select class="campo-input-select" id="bolsa-compranet">';
    htmlBolsaCompraNet += '<option value = "null">Seleccione...</option>';
    for (var ii in result.BolsComNet) {
        (function (i) {
            htmlBolsaCompraNet += '<option value="' + result.BolsComNet[i].Id + '">' + result.BolsComNet[i].Descripcion + '</option>';
        })(ii);
    }
    htmlBolsaCompraNet += '</select>';
    $(".campo-bolsa-compranet").append(htmlBolsaCompraNet);

    $("#agregarTelefono").click(function () {
        if (!($("#Telefono2") && $("#Telefono2").length > 0)) {
            if (!validateNumber($("#Telefono1").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }

            var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
            htmlTipoTelefono += '<select  class="campo-input-select" id="TipoTelefono2">';
            htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
            for (var ii in resultInit.tiptel) {
                (function (i) {
                    htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
                })(ii);
            }
            htmlTipoTelefono += '</select>'
                + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono2" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono2" />'
                + '</div>';

            $(".contacto-basico-telefonos").append(htmlTipoTelefono);

            $("#eliminarTelefono2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Telefono3") && $("#Telefono3").length > 0)) {
            if (!validateNumber($("#Telefono2").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }

            var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
            htmlTipoTelefono += '<select  class="campo-input-select" id="TipoTelefono3">';
            htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
            for (var ii in resultInit.tiptel) {
                (function (i) {
                    htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
                })(ii);
            }
            htmlTipoTelefono += '</select>'
                + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono3" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono3" />'
                + '</div>';

            $(".contacto-basico-telefonos").append(htmlTipoTelefono);

            $("#eliminarTelefono3").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Telefono4") && $("#Telefono4").length > 0)) {
            if (!validateNumber($("#Telefono3").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }

            var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
            htmlTipoTelefono += '<select  class="campo-input-select" id="TipoTelefono4">';
            htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
            for (var ii in resultInit.tiptel) {
                (function (i) {
                    htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
                })(ii);
            }
            htmlTipoTelefono += '</select>'
                + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono4" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono4" />'
                + '</div>';

            $(".contacto-basico-telefonos").append(htmlTipoTelefono);

            $("#eliminarTelefono4").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 4 correos teléfonos.");
        }
    });

    $("#concom-agregarTelefono").click(function () {
        if (!($("#concom-Telefono2") && $("#concom-Telefono2").length > 0)) {
            if (!validateNumber($("#concom-Telefono1").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }

            var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
            htmlTipoTelefono += '<select  class="campo-input-select" id="concom-TipoTelefono2">';
            htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
            for (var ii in resultInit.tiptel) {
                (function (i) {
                    if (result.tiptel[i].Descripcion != "Otros" && result.tiptel[i].Descripcion != "Emergencia") {
                        htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
                    }
                })(ii);
            }
            htmlTipoTelefono += '</select>'
                + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="concom-Telefono2" style="margin-right: 64px;margin-top: 5px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarTelefono2" />'
                + '</div>';

            $(".concom-campo-telefono").append(htmlTipoTelefono);

            $("#concom-eliminarTelefono2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#concom-Telefono3") && $("#concom-Telefono3").length > 0)) {
            if (!validateNumber($("#concom-Telefono2").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }

            var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
            htmlTipoTelefono += '<select  class="campo-input-select" id="concom-TipoTelefono3">';
            htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
            for (var ii in resultInit.tiptel) {
                (function (i) {
                    if (result.tiptel[i].Descripcion != "Otros" && result.tiptel[i].Descripcion != "Emergencia") {
                        htmlTipoTelefono += '<option value="' + result.tiptel[i].TipoTelefonoId + '">' + result.tiptel[i].Descripcion + '</option>';
                    }
                })(ii);
            }
            htmlTipoTelefono += '</select>'
                + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="concom-Telefono3" style="margin-right: 64px;margin-top: 5px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarTelefono3" />'
                + '</div>';

            $(".concom-campo-telefono").append(htmlTipoTelefono);

            $("#concom-eliminarTelefono3").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 3 teléfonos.");
        }
    });

    var htmlInteres = "";
    htmlInteres += '<select multiple class="" id="concom-intereses">';
    for (var ii in result.inte) {
        (function (i) {
            htmlInteres += '<option value="' + result.inte[i].InteresId + '">' + result.inte[i].Descripcion + '</option>';
        })(ii);
    }
    htmlInteres += '</select>';
    $(".concom-intereses").append(htmlInteres);

    var htmlCampañaObjetivo = "";
    htmlCampañaObjetivo += '<select class="campo-input-select campo-sin-span grano" id="granoObjetivo0">';
    htmlCampañaObjetivo += '<option value = "null">Seleccione...</option>';
    for (var ii in result.gran) {
        (function (i) {
            htmlCampañaObjetivo += '<option value="' + result.gran[i].MaterialId + '">' + result.gran[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCampañaObjetivo += '</select>'
    htmlCampañaObjetivo += '<select class="campo-input-select campo-sin-span grano" id="campañaObjetivo0">';
    htmlCampañaObjetivo += '<option value = "null">Seleccione...</option>';

    htmlCampañaObjetivo += '</select>'
        + '<input type="text" class="campo-input-text toneladasObjetivo" id="toneladasObjetivo0" />';

    $(".campo-granos-objetivo").append(htmlCampañaObjetivo);

    $("#granoObjetivo0").change(function (x) {
        var obj = {
            MaterialId: $(this).val(),
            elemId: $(this).prop("id").split("granoObjetivo")[1]
        };
        armarSelectGranoObjetivo(obj);
    });

    $("#calificacion").multiselect({
        header: false,
        multiple: false,
        selectedList: 1
    });

    $("#canopera").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Canal Operacion",
    });

    $("#entregaA").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Entrega A",
    });

    $("#condPrefer").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "CondicionPreferente",
    });

    $("#concom-intereses").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Interes",
    });

    $("#guardarCapProd").click(function () {
        var obj = {};

        obj.item = capProdCant;
        obj.CampoId = $("#campoid").val();

        if (!$("#kmz").val()) {
            if ($("#provincia-produccion").val() == "null") {
                MensErr("Debe ingresar una provincia");
                return false;
            } else {
                obj.provincia = $("#provincia-produccion").val();
                obj.provinciaNom = $("#provincia-produccion option:selected").text();
            }
        }

        if (!$("#kmz").val()) {
            if ($("#localidad-produccion").val() == "null") {
                MensErr("Debe ingresar una localidad");
                return false;
            } else {
                obj.localidad = $("#localidad-produccion").val();
                obj.localidadNom = $("#localidad-produccion option:selected").text();
            }
        }

        obj.coordenadas = $("#coordenadas").val();

        if ($("#hectareas-arrendadas").is(":checked")) {
            obj.hectareas = $("#hectareas-arrendadas").prop("value");
            obj.hectareasNom = $("label[for='hectareas-arrendadas']").text().trim();
        } else if ($("#hectareas-propias").is(":checked")) {
            obj.hectareas = $("#hectareas-propias").prop("value");
            obj.hectareasNom = $("label[for='hectareas-propias']").text().trim();
        } else {
            obj.hectareas = null;
            obj.hectareasNom = "";
        }
        obj.granos = [];

        for (var i = 0; i < (cantGrano + 1); i++) {
            if (($("#campaña" + i).val() && $("#campaña" + i).val() != "null") || ($("#grano" + i).val() && $("#grano" + i).val() != "null")) {
                obj.granos.push({
                    granoId: $("#grano" + i).val(),
                    grano: $("#grano" + i).find('option:selected').text(),
                    campañaId: $("#campaña" + i).val(),
                    campaña: $("#campaña" + i).find('option:selected').text(),
                    hectareas: $("#hectareas" + i).val(),
                    toneladas: $("#toneladas" + i).val(),
                    CampoId: $("#campoid").val()
                });
            }
        }

        if (!ValidarGranoProduccion(cantGrano))
            return false;

        obj.archivo = $("#kmz").val();
        obj.archivoFile = document.getElementById("kmz").files[0];
        if (obj.archivo) {
            var input = document.getElementById("kmz");
            var file = input.files[0];
            fr = new FileReader();
            fr.readAsDataURL(file);
            obj.archivoFileReader = fr;
            obj.archivoFileResult = obj.archivoFileReader.result;
        } else if (mantenerArchivos.length) {
            obj.archivo = mantenerArchivos[0].archivo;
            obj.archivoFile = mantenerArchivos[0].archivoFile;
            obj.archivoFileReader = mantenerArchivos[0].archivoFileReader;
            obj.archivoFileResult = mantenerArchivos[0].archivoFileResult;
            mantenerArchivos.pop();
        }

        var html = "";
        html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="granocontenedor' + capProdCant + '">'
            + '<div>'
            + '<div class="datos-produccion-cap-prod-guardados-zona">'
            + obj.provinciaNom + ", " + obj.localidadNom
            + '</div>'
            + '<div class="editar-produccion" onclick="editarCampoProduccion(' + capProdCant + ')" id="editarProd' + capProdCant + '">'
            + '<img src="../Content/Images/contacto-edit.png" /> Editar'
            + '</div>'
            + '<div class="eliminar-produccion" onclick="eliminarCampoProduccion(this)" id="eliminarProd' + capProdCant + '">'
            + 'x Eliminar'
            + '</div>'
            + '</div>'
            + '<div>'
            + '<div class="datos-produccion-cap-prod-guardados-hectareas">'
            + '(Has ' + (obj.hectareasNom ? obj.hectareasNom : "no especificadas") + ')'
            + '</div>'
            + '<div class="granos-contenedor">';

        for (var jj in obj.granos) {
            (function (j) {
                html += '<div class="granos-contenedor-grupo">'
                    + '<div class="granos-contenedor-titulo">'
                    + "Campaña " + (obj.granos[j].campañaId != "null" ? obj.granos[j].campaña : "no especificada") + ": " + (obj.granos[j].granoId != "null" ? obj.granos[j].grano : "No se especificó material")
                    + '</div>'
                    + '<div class="granos-contenedor-has-tns">'
                    + '<b>' + (obj.granos[j].hectareas ? obj.granos[j].hectareas : "No especifica ") + "</b> Has - <b>" + (obj.granos[j].toneladas ? obj.granos[j].toneladas : "No especifica ") + "</b> TNs"
                    + '</div>'
                    + '</div>'
            })(jj);
        }

        html += '</div>'
            + '</div>';

        $("#coordenadas").val("");
        $("#provincia-produccion").val("null");
        $("#localidad-produccion").val("null");
        $("#hectareas-arrendadas").prop('checked', false);
        $("#hectareas-propias").prop('checked', false);
        $("#campoid").val(0);
        for (var i = cantGrano; i > 0; i--) {
            $(".linea" + i).remove();
        }

        $("#grano0").val("null");
        $("#grano0").trigger("change");
        $("#campaña0").val("null");
        $("#hectareas0").val("");
        $("#toneladas0").val("");
        $("#eliminarGrano").hide();
        $("#kmz").val("");
        $(".label-field .produccion").html("Subir un Archivo");
        cantGrano = 0;

        $(".datos-produccion-cap-prod-guardados").append(html);
        $(".datos-produccion-cap-prod-guardados").show();

        obj.eliminarproduccion = aEliminarGranos.concat();
        aGuardar.push(obj);

        aEliminarGranos = [];

        capProdCant++;
    });

    $("#guardarCapProd-almacenamiento").click(function () {
        var obj = {};

        obj.item = capProdCantAlmacenamiento;
        obj.CampoId = $("#campo-almacenamientoid").val();

        if (!$("#kmz-almacenamiento").val()) {
            if ($("#provincia-almacenamiento").val() == "null") {
                MensErr("Debe ingresar una provincia");
                return false;
            } else {
                obj.provincia = $("#provincia-almacenamiento").val();
                obj.provinciaNom = $("#provincia-almacenamiento option:selected").text();
            }
        }

        if (!$("#kmz-almacenamiento").val()) {
            if ($("#localidad-almacenamiento").val() == "null") {
                MensErr("Debe ingresar una localidad");
                return false;
            } else {
                obj.localidad = $("#localidad-almacenamiento").val();
                obj.localidadNom = $("#localidad-almacenamiento option:selected").text();
            }
        }

        if ($("#coordenadas-almacenamiento").val())
            obj.coordenadasAlmacenamiento = $("#coordenadas-almacenamiento").val();

        obj.granosAlmacenamiento = [];

        for (var i = 0; i < (cantGranoAlmacenamiento + 1); i++) {
            if ($("#campañaAlmacenamiento" + i).val() && $("#campañaAlmacenamiento" + i).val() != "null") {
                obj.granosAlmacenamiento.push({
                    campañaId: $("#campañaAlmacenamiento" + i).val(),
                    campaña: $("#campañaAlmacenamiento" + i).find('option:selected').text(),
                    toneladasAlmacenamiento: $("#toneladasAlmacenamiento" + i).val(),
                    hasArrendadas: $("#hectareas-propias" + i).is(":checked") ? 0 : ($("#hectareas-arrendadas" + i).is(":checked") ? 1 : null),
                    hasArrendadasNombre: $("#hectareas-propias" + i).is(":checked") ? "Propias" : ($("#hectareas-arrendadas" + i).is(":checked") ? "Alquiladas" : "no especifica"),
                    CampoId: $("#campoid").val()
                });
            }
        }

        obj.granosAlmacenamientoGrano = [];
        for (var i = 0; i < (cantGranoAlmacenamientoGrano + 1); i++) {
            if ($("#campañaAlmacenamientoGranos" + i).val() && $("#campañaAlmacenamientoGranos" + i).val() != "null") {
                obj.granosAlmacenamientoGrano.push({
                    campañaId: $("#campañaAlmacenamientoGranos" + i).val(),
                    campaña: $("#campañaAlmacenamientoGranos" + i).find('option:selected').text(),
                    granoId: $("#granoAlmacenamientoGranos" + i).val(),
                    grano: $("#granoAlmacenamientoGranos" + i).find('option:selected').text(),
                    toneladasAlmacenamiento: $("#toneladasAlmacenamientoGrano" + i).val(),
                    CampoId: $("#campoid").val()
                });
            }
        }

        if (!ValidarGranoAlmacenamiento(cantGranoAlmacenamiento))
            return false;

        obj.archivo = $("#kmz-almacenamiento").val();
        obj.archivoFile = document.getElementById("kmz-almacenamiento").files[0];
        if (obj.archivo) {
            var input = document.getElementById("kmz-almacenamiento");
            var file = input.files[0];
            fr = new FileReader();
            fr.readAsDataURL(file);
            obj.archivoFileReader = fr;
            obj.archivoFileResult = obj.archivoFileReader.result;
        } else if (mantenerArchivosAlmacenamiento.length) {
            obj.archivo = mantenerArchivosAlmacenamiento[0].archivo;
            obj.archivoFile = mantenerArchivosAlmacenamiento[0].archivoFile;
            obj.archivoFileReader = mantenerArchivosAlmacenamiento[0].archivoFileReader;
            obj.archivoFileResult = mantenerArchivosAlmacenamiento[0].archivoFileResult;
            mantenerArchivosAlmacenamiento.pop();
        }

        var html = "";
        html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="granocontenedoralmacenamiento' + capProdCantAlmacenamiento + '">'
            + '<div>'
            + '<div class="datos-produccion-cap-prod-guardados-zona">'
            + obj.provinciaNom + ", " + obj.localidadNom
            + '</div>'
            + '<div class="editar-produccion" onclick="editarAlmacenamiento(' + capProdCantAlmacenamiento + ')" id="editarAlm' + capProdCantAlmacenamiento + '">'
            + '<img src="../Content/Images/contacto-edit.png" /> Editar'
            + '</div>'
            + '<div class="eliminar-produccion" onclick="eliminarAlmacenamiento(this)" id="eliminarAlm' + capProdCantAlmacenamiento + '">'
            + 'x Eliminar'
            + '</div>'
            + '</div>'
            + '<div>'
            + '<div class="granos-contenedor">';

        for (var jj in obj.granosAlmacenamiento) {
            (function (j) {
                html += '<div class="granos-contenedor-grupo">'
                    + '<div class="granos-contenedor-titulo">'
                    + 'Campaña ' + (obj.granosAlmacenamiento[j].campañaId != "null" ? obj.granosAlmacenamiento[j].campaña : "no especifica") + ": " //+ obj.granosAlmacenamiento[j].granoAlmacenamiento
                    + '</div>'
                    + '<div class="granos-contenedor-has-tns">'
                    + '<b>' + (obj.granosAlmacenamiento[j].toneladasAlmacenamiento ? obj.granosAlmacenamiento[j].toneladasAlmacenamiento : "No especifica ") + "</b> Tns - " + obj.granosAlmacenamiento[j].hasArrendadasNombre
                    + '</div>'
                    + '</div>'
            })(jj);
        }

        for (var jj in obj.granosAlmacenamientoGrano) {
            (function (j) {
                html += '<div class="granos-contenedor-grupo">'
                    + '<div class="granos-contenedor-titulo">'
                    + 'Campaña ' + (obj.granosAlmacenamientoGrano[j].campañaId != "null" ? obj.granosAlmacenamientoGrano[j].campaña : "no especifica") + ": " //+ obj.granosAlmacenamiento[j].granoAlmacenamiento
                    + '</div>'
                    + '<div class="granos-contenedor-has-tns">'
                    + '<b>' + (obj.granosAlmacenamientoGrano[j].granoId ? obj.granosAlmacenamientoGrano[j].grano + " - " : "No especifica material - ") + (obj.granosAlmacenamientoGrano[j].toneladasAlmacenamiento ? obj.granosAlmacenamientoGrano[j].toneladasAlmacenamiento : "No especifica ") + "</b> Tns"
                    + '</div>'
                    + '</div>'
            })(jj);
        }

        html += '</div>'
            + '</div>';
        $("#coordenadas-almacenamiento").val("");
        $("#provincia-almacenamiento").val("null");
        $("#localidad-almacenamiento").val("null");
        for (var i = cantGranoAlmacenamiento; i > 0; i--) {
            $(".lineaAlmacenamiento" + i).remove();
        }
        for (var i = cantGranoAlmacenamientoGrano; i > 0; i--) {
            $(".lineaAlmacenamientoGranos" + i).remove();
        }

        $("#campo-almacenamientoid").val(0);
        $("#campañaAlmacenamiento0").val("null");
        $("#campañaAlmacenamiento0").trigger("change");
        $("#toneladasAlmacenamiento0").val("");
        $("#eliminarGrano-almacenamiento").hide();
        $("#kmz-almacenamiento").val("");
        $(".label-field .almacenamiento").html("Subir un Archivo");
        $("#hectareas-arrendadas0").prop("checked", false);
        $("#hectareas-propias0").prop("checked", false);

        $("#granoAlmacenamientoGranos0").val("null");
        $("#granoAlmacenamientoGranos0").trigger("change");
        $("#campañaAlmacenamientoGranos0").val("null");
        $("#toneladasAlmacenamientoGrano0").val("");
        $("#eliminarGrano-almacenamientograno").hide();

        cantGranoAlmacenamiento = 0;
        cantGranoAlmacenamientoGrano = 0;

        $(".datos-almacenamiento-cap-prod-guardados").append(html);
        $(".datos-almacenamiento-cap-prod-guardados").show();

        obj.eliminargranoalmacenamiento = aEliminarGranosAlmacenamiento.concat();
        obj.eliminargranoalmacenamientograno = aEliminarGranosAlmacenamientoGrano.concat();

        aGuardarAlmacenamiento.push(obj);

        aEliminarGranosAlmacenamiento = [];
        aEliminarGranosAlmacenamientoGrano = [];
        capProdCantAlmacenamiento++;
    });

    $("#provincia").change(function () {
        var valor = $("#provincia").val();
        buscarLocalidad(valor);
    });

    $("#provincia-produccion").change(function () {
        var valor = $("#provincia-produccion").val();
        buscarLocalidadProduccion(valor);
    });

    $("#provincia-almacenamiento").change(function () {
        var valor = $("#provincia-almacenamiento").val();
        buscarLocalidadAlmacenamiento(valor);
    });

    $("#provincia-compranet").change(function () {
        var valor = $("#provincia-compranet").val();
        buscarLocalidadCompraNet(valor);
    });
}

function eliminarCampoProduccion(val) {
    var item = $(val).attr("id").split("eliminarProd")[1];
    $("#granocontenedor" + item).remove();
    aGuardar = aGuardar.filter(function (el) {
        return el.item !== parseInt(item);
    });
}

function eliminarAlmacenamiento(val) {
    var item = $(val).attr("id").split("eliminarAlm")[1];
    $("#granocontenedoralmacenamiento" + item).remove();
    aGuardarAlmacenamiento = aGuardarAlmacenamiento.filter(function (el) {
        return el.item !== parseInt(item);
    });
}

function editarCampoProduccion(id) {
    var obj = aGuardar.filter(function (el) {
        return el.item === parseInt(id);
    });
    obj = obj[0];

    $("#coordenadas").val(obj.coordenadas ? obj.coordenadas : "");
    $("#provincia-produccion").val(obj.provincia ? obj.provincia : "null");
    $("#provincia-produccion").trigger("change");
    $("#localidad-produccion").val(obj.localidad ? obj.localidad : "null");
    if (obj.hectareas == 1) {
        $("#hectareas-propias").prop('checked', true);
    } else if (obj.hectareas == 0) {
        $("#hectareas-arrendadas").prop('checked', true);
    }

    if (obj.archivo) {
        $(".label-field .produccion").html("Archivo Subido");
        mantenerArchivos.push({
            archivo: obj.archivo,
            archivoFile: obj.archivoFile,
            archivoFileReader: obj.archivoFileReader,
            archivoFileResult: obj.archivoFileResult
        });
    }

    $("#campoid").val(obj.CampoId ? obj.CampoId : 0);

    var cantGranos = obj.granos.length;
    $("#grano0").val(obj.granos.length ? obj.granos[0].granoId : "null");
    $("#grano0").trigger("change");
    $("#campaña0").val(obj.granos.length > 0 ? obj.granos[0].campañaId : "null");

    $("#hectareas0").val(obj.granos.length ? obj.granos[0].hectareas : "");
    $("#toneladas0").val(obj.granos.length ? obj.granos[0].toneladas : "");
    for (var i = 1; i < cantGranos; i++) {
        cantGrano++;

        var htmlCampañaGrano = "";
        htmlCampañaGrano += '<div class="linea' + i + ' campo-granos" style="position:relative">';
        htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="grano' + i + '">';
        htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
        for (var jj in resultGranos) {
            (function (j) {
                htmlCampañaGrano += '<option value="' + resultGranos[j].MaterialId + '">' + resultGranos[j].Descripcion + '</option>';
            })(jj);
        }
        htmlCampañaGrano += '</select>'
            + '<select class="campo-input-select campo-sin-span grano" id="campaña' + i + '">'
            + '<option disabled selected value="">Seleccionar...</option>'
            + '</select>'
            + '<input type="text" class="campo-input-text hectareas" style="margin-left: 20px;" id="hectareas' + i + '" />'
            + '<input type="text" class="campo-input-text toneladas" id="toneladas' + i + '" />'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano" id="eliminarGrano' + i + '" />'
            + '</div>';

        $("#formulario-produccion .datos-produccion-cap-prod-editor-granos-cantidades-grupo").append(htmlCampañaGrano);

        $("#grano" + i + "").change(function (x) {
            var obj = {
                MaterialId: $(this).val(),
                elemId: $(this).prop("id").split("grano")[1]
            };
            armarSelectGrano(obj);
        });

        $("#eliminarGrano" + i).click(function () {
            eliminarGrano(this);
        });

        $("#grano" + i).val(obj.granos[i].granoId);
        $("#grano" + i).trigger("change");
        $("#campaña" + i).val(obj.granos[i].campañaId);

        $("#hectareas" + i).val(obj.granos[i].hectareas);
        $("#toneladas" + i).val(obj.granos[i].toneladas);
    }

    $("#granocontenedor" + id).remove();
    aGuardar = aGuardar.filter(function (el) {
        return el.item !== parseInt(id);
    });
}

function editarAlmacenamiento(id) {
    var obj = aGuardarAlmacenamiento.filter(function (el) {
        return el.item === parseInt(id);
    });
    obj = obj[0];

    $("#coordenadas-almacenamiento").val(obj.coordenadasAlmacenamiento ? obj.coordenadasAlmacenamiento : "");
    $("#provincia-almacenamiento").val(obj.provincia ? obj.provincia : "null");
    $("#provincia-almacenamiento").trigger("change");
    $("#localidad-almacenamiento").val(obj.localidad ? obj.localidad : "null");

    if (obj.archivo) {
        $(".label-field .almacenamiento").html("Archivo Subido");
        mantenerArchivosAlmacenamiento.push({
            archivo: obj.archivo,
            archivoFile: obj.archivoFile,
            archivoFileReader: obj.archivoFileReader,
            archivoFileResult: obj.archivoFileResult
        });
    }

    $("#campo-almacenamientoid").val(obj.CampoId ? obj.CampoId : 0);

    var cantGranosAlmacenamiento = obj.granosAlmacenamiento.length;
    $("#campañaAlmacenamiento0").val(obj.granosAlmacenamiento.length > 0 ? obj.granosAlmacenamiento[0].campañaId : "null");
    $("#campañaAlmacenamiento0").trigger("change");
    $("#granoAlmacenamiento0").val(obj.granosAlmacenamiento.length > 0 ? obj.granosAlmacenamiento[0].granoId : "null");
    $("#toneladasAlmacenamiento0").val(obj.granosAlmacenamiento.length > 0 ? obj.granosAlmacenamiento[0].toneladasAlmacenamiento : "");

    if (obj.granosAlmacenamiento.length > 0 && obj.granosAlmacenamiento[0].hasArrendadas == 1) {
        $("#hectareas-arrendadas0").prop("checked", true);
    } else if (obj.granosAlmacenamiento.length > 0 && obj.granosAlmacenamiento[0].hasArrendadas == 0) {
        $("#hectareas-propias0").prop("checked", true);
    }
    for (var i = 1; i < cantGranosAlmacenamiento; i++) {
        cantGranoAlmacenamiento++;

        var htmlCampañaGrano = "";
        htmlCampañaGrano += '<div class="lineaAlmacenamiento' + i + ' campo-granos-almacenamiento" style="position:relative;">';
        htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamiento' + i + '">';
        htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
        for (var jj in resultCampaña) {
            (function (j) {
                htmlCampañaGrano += '<option value="' + resultCampaña[j].CampañaId + '">' + resultCampaña[j].Descripcion + '</option>';
            })(jj);
        }
        htmlCampañaGrano += '</select>'
            + '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamiento' + i + '" />'
            + '<div class="almacenamiento-hectareas-linea">'
            + '<div class="formulario-campo-radio">'
            + '<label for="hectareas-arrendadas' + i + '"><input id="hectareas-arrendadas' + i + '" type="radio" name="hectareas' + i + '" value="1"> Alquiladas</label>'
            + '<label for="hectareas-propias' + i + '"><input id="hectareas-propias' + i + '" type="radio" name="hectareas' + i + '" value="0"> Propias</label>'
            + '</div>'
            + '</div>'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamiento" id="eliminarGrano-almacenamiento' + i + '" />'
            + '</div>';

        $("#formulario-almacenamiento .datos-almacenamiento-cap-prod-editor-granos-cantidades-grupo").append(htmlCampañaGrano);

        $("#campañaAlmacenamiento" + i + "").change(function (x) {
            var obj = {
                CampañaId: $(this).val(),
                elemId: $(this).prop("id").split("campañaAlmacenamiento")[1]
            };
            armarSelectGranoAlmacenamiento(obj);
        });

        $("#eliminarGrano-almacenamiento" + i).click(function () {
            eliminarGranoAlmacenamiento(this);
        });

        $("#campañaAlmacenamiento" + i).val(obj.granosAlmacenamiento[i].campañaId);
        $("#campañaAlmacenamiento" + i).trigger("change");
        $("#granoAlmacenamiento" + i).val(obj.granosAlmacenamiento[i].granoId);
        $("#toneladasAlmacenamiento" + i).val(obj.granosAlmacenamiento[i].toneladasAlmacenamiento);
        $("#porcentajeAlmacenamiento" + i).val(obj.granosAlmacenamiento[i].porcentajeAlmacenamiento);
        if (obj.granosAlmacenamiento[i].hasArrendadas == 0) {
            $("#hectareas-propias" + i).prop("checked", true);
        } else if (obj.granosAlmacenamiento[i].hasArrendadas == 1) {
            $("#hectareas-arrendadas" + i).prop("checked", true);
        }
    }

    var cantGranosAlmacenamientoGranos = obj.granosAlmacenamientoGrano.length;
    $("#granoAlmacenamientoGranos0").val(obj.granosAlmacenamientoGrano.length > 0 ? obj.granosAlmacenamientoGrano[0].granoId : "null");
    $("#granoAlmacenamientoGranos0").trigger("change");
    $("#campañaAlmacenamientoGranos0").val(obj.granosAlmacenamientoGrano.length > 0 ? obj.granosAlmacenamientoGrano[0].campañaId : "null");
    $("#toneladasAlmacenamientoGrano0").val(obj.granosAlmacenamientoGrano.length > 0 ? obj.granosAlmacenamientoGrano[0].toneladasAlmacenamiento : "");

    for (var i = 1; i < cantGranosAlmacenamientoGranos; i++) {
        cantGranoAlmacenamientoGrano++;

        var htmlCampañaGrano = "";

        htmlCampañaGrano += '<div class="lineaAlmacenamientoGranos' + i + ' campo-granos-almacenamientograno" style="position:relative;">';
        htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="granoAlmacenamientoGranos' + i + '">';
        htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
        for (var ii in resultInit.gran) {
            (function (i) {
                htmlCampañaGrano += '<option value="' + resultInit.gran[i].MaterialId + '">' + resultInit.gran[i].Descripcion + '</option>';
            })(ii);
        }
        htmlCampañaGrano += '</select>'
        htmlCampañaGrano += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamientoGranos' + i + '">';
        htmlCampañaGrano += '<option value = "null">Seleccione...</option>';

        htmlCampañaGrano += '</select>' +
            '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamientoGrano' + i + '" />'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamientograno" id="eliminarGrano-almacenamientograno' + i + '" />'
            + '</div>';

        $("#formulario-almacenamiento .datos-almacenamiento-cap-prod-editor-granosalm-cantidades-grupo").append(htmlCampañaGrano);

        $("#granoAlmacenamientoGranos" + i + "").change(function (x) {
            var obj = {
                MaterialId: $(this).val(),
                elemId: $(this).prop("id").split("granoAlmacenamientoGranos")[1]
            };
            armarSelectGranoAlmacenamientoGrano(obj);
        });

        $("#eliminarGrano-almacenamientograno" + i).click(function () {
            console.log("clickeo");
            eliminarGranoAlmacenamientoGrano(this);
        });

        $("#granoAlmacenamientoGranos" + i).val(obj.granosAlmacenamientoGrano[i].granoId);
        $("#granoAlmacenamientoGranos" + i).trigger("change");
        $("#campañaAlmacenamientoGranos" + i).val(obj.granosAlmacenamientoGrano[i].campañaId);
        $("#toneladasAlmacenamientoGrano" + i).val(obj.granosAlmacenamientoGrano[i].toneladasAlmacenamiento);
    }

    $("#granocontenedoralmacenamiento" + id).remove();
    aGuardarAlmacenamiento = aGuardarAlmacenamiento.filter(function (el) {
        return el.item !== parseInt(id);
    });
}

function armarFuncionalidades() {
    $(".page-sidebar-menu").attr('style', 'display:none!important');
    $(".page-sidebar").attr('style', 'display:none!important');
    $(".page-content").css({
        'margin-left': 0
    });
    $(".atras-nav").show();
    $(".nav-bar-segundo a span").css({
        'vertical-align': 'middle'
    });
    $(".navbarsegundo-bread").html("Agregar contacto");
    $("#AgregarContacto").hide();

    $(".atras-nav").click(function () {
        $("#modalSalir").modal();
    });

    $($(".logoheader-nav").parent()).click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $($(".logoheader-nav").parent()).blur();
        $("#modalSalir").modal();
    });

    $(".miscontactos-nav").parent().click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $($(".logoheader-nav").parent()).blur();
        $("#modalSalir").modal();
    });

    $(".formulario-footer-cancelar").click(function () {
        $("#modalSalir").modal();
    });

    $(".modal-salir-sin-guardar").click(function () {
        if (ProveedorId) {
            window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + ProveedorId;
        } else {
            window.location.href = "http://" + window.location.href.split("\/")[2];
        }
    });

    $(".modal-cancelar").click(function () {
        $("#modalSalir").modal('hide');
    });

    $("label[for='kmz']").hover(function () {
        $("label[for='kmz']").css({
            'background-color': 'white',
            border: '1px solid #017940'
        });
        $("label[for='kmz'] img").attr('src', "../content/images/upload.png");
        $("label[for='kmz'] .campo-span").css({
            color: '#017940'
        })
    }, function () {
        $("label[for='kmz']").css({
            'background-color': '#017940'
        });
        $("label[for='kmz'] img").attr('src', "../content/images/uploadb.png");
        $("label[for='kmz'] .campo-span").css({
            color: '#fff',
            border: 'none'
        })
    });

    $("#kmz").change(function (x) {
        $(".label-field").html("Archivo subido");
    });

    $("label[for='kmz-almacenamiento']").hover(function () {
        $("label[for='kmz-almacenamiento']").css({
            'background-color': 'white',
            border: '1px solid #017940'
        });
        $("label[for='kmz-almacenamiento'] img").attr('src', "../content/images/upload.png");
        $("label[for='kmz-almacenamiento'] .campo-span").css({
            color: '#017940'
        })
    }, function () {
        $("label[for='kmz-almacenamiento']").css({
            'background-color': '#017940'
        });
        $("label[for='kmz-almacenamiento'] img").attr('src', "../content/images/uploadb.png");
        $("label[for='kmz-almacenamiento'] .campo-span").css({
            color: '#fff',
            border: 'none'
        })
    });

    $("#kmz-almacenamiento").change(function (x) {
        $(".label-field").html("Archivo subido");
    });

    $("#contacto").click(function () {
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#contactocomercial").removeClass("whc-selected");
        $("#contacto").addClass("whc-selected");

        if ($("#formulario-basico").is(":visible")) {
            $("#formulario-basico").fadeOut("slow", function () {
                $("#formulario-contacto").fadeIn("slow");
            });
        } else if ($("#formulario-produccion").is(":visible")) {
            $("#formulario-produccion").fadeOut("slow", function () {
                $("#formulario-contacto").fadeIn("slow");
            });
        } else if ($("#formulario-almacenamiento").is(":visible")) {
            $("#formulario-almacenamiento").fadeOut("slow", function () {
                $("#formulario-contacto").fadeIn("slow");
            });
        } else if ($("#formulario-contactocomercial").is(":visible")) {
            $("#formulario-contactocomercial").fadeOut("slow", function () {
                $("#formulario-contacto").fadeIn("slow");
            });
        }

        $(".formulario-footer-guardar-contacto").html("Guardar").removeClass("invertir-boton-Guardar");
        $(".formulario-footer-siguiente").show();
    });

    $("#contactocomercial").click(function () {
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#contactocomercial").removeClass("whc-selected");
        $("#contactocomercial").addClass("whc-selected");

        if ($("#formulario-basico").is(":visible")) {
            $("#formulario-basico").fadeOut("slow", function () {
                $("#formulario-contactocomercial").fadeIn("slow");
            });
        } else if ($("#formulario-produccion").is(":visible")) {
            $("#formulario-produccion").fadeOut("slow", function () {
                $("#formulario-contactocomercial").fadeIn("slow");
            });
        } else if ($("#formulario-almacenamiento").is(":visible")) {
            $("#formulario-almacenamiento").fadeOut("slow", function () {
                $("#formulario-contactocomercial").fadeIn("slow");
            });
        } else if ($("#formulario-contacto").is(":visible")) {
            $("#formulario-contacto").fadeOut("slow", function () {
                $("#formulario-contactocomercial").fadeIn("slow");
            });
        }

        $(".formulario-footer-guardar-contacto").html("Guardar").removeClass("invertir-boton-Guardar");
        $(".formulario-footer-siguiente").show();
    });

    $("#produccion").click(function () {
        $("#basico").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#contactocomercial").removeClass("whc-selected");
        $("#produccion").addClass("whc-selected");

        if ($("#formulario-basico").is(":visible")) {
            $("#formulario-basico").fadeOut("slow", function () {
                $("#formulario-produccion").fadeIn("slow");
            });
        } else if ($("#formulario-contacto").is(":visible")) {
            $("#formulario-contacto").fadeOut("slow", function () {
                $("#formulario-produccion").fadeIn("slow");
            });
        } else if ($("#formulario-almacenamiento").is(":visible")) {
            $("#formulario-almacenamiento").fadeOut("slow", function () {
                $("#formulario-produccion").fadeIn("slow");
            });
        } else if ($("#formulario-contactocomercial").is(":visible")) {
            $("#formulario-contactocomercial").fadeOut("slow", function () {
                $("#formulario-produccion").fadeIn("slow");
            });
        }

        $(".formulario-footer-guardar-contacto").html("Guardar").removeClass("invertir-boton-Guardar");
        $(".formulario-footer-siguiente").show();
    });

    $("#almacenamiento").click(function () {
        $("#basico").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#contactocomercial").removeClass("whc-selected");
        $("#almacenamiento").addClass("whc-selected");

        if ($("#formulario-basico").is(":visible")) {
            $("#formulario-basico").fadeOut("slow", function () {
                $("#formulario-almacenamiento").fadeIn("slow");
            });
        } else if ($("#formulario-contacto").is(":visible")) {
            $("#formulario-contacto").fadeOut("slow", function () {
                $("#formulario-almacenamiento").fadeIn("slow");
            });
        } else if ($("#formulario-produccion").is(":visible")) {
            $("#formulario-produccion").fadeOut("slow", function () {
                $("#formulario-almacenamiento").fadeIn("slow");
            });
        } else if ($("#formulario-contactocomercial").is(":visible")) {
            $("#formulario-contactocomercial").fadeOut("slow", function () {
                $("#formulario-almacenamiento").fadeIn("slow");
            });
        }

        $(".formulario-footer-guardar-contacto").html("Guardar y Finalizar").addClass("invertir-boton-Guardar");
        $(".formulario-footer-siguiente").hide();
    });

    $("#agregarMail").click(function () {
        if (!($("#Email2") && $("#Email2").length > 0)) {
            if (!validateEmail($("#Email1").val())) {
                MensErr("El Email no es válido");
                return false;
            }

            var div = "";
            div += '<div class="formulario-campo">'
                + '<input type="text" class="campo-input-text" id="Email2" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail2" />'
                + '</div>';
            $(".contacto-basico-emails").append(div);

            $("#eliminarMail2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Email3") && $("#Email3").length > 0)) {
            if (!validateEmail($("#Email2").val())) {
                MensErr("El Email no es válido");
                return false;
            }

            var div = "";
            div += '<div class="formulario-campo">'
                + '<input type="text" class="campo-input-text" id="Email3" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail3" />'
                + '</div>';
            $(".contacto-basico-emails").append(div);

            $("#eliminarMail3").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Email4") && $("#Email4").length > 0)) {
            if (!validateEmail($("#Email3").val())) {
                MensErr("El Email no es válido");
                return false;
            }

            var div = "";
            div += '<div class="formulario-campo">'
                + '<input type="text" class="campo-input-text" id="Email4" style="margin-right:0px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail4" />'
                + '</div>';
            $(".contacto-basico-emails").append(div);

            $("#eliminarMail4").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 4 correos electrónicos.");
        }
    });

    $(".formulario-footer-siguiente").click(function () {
        if ($("#formulario-basico").is(":visible")) {
            $("#formulario-basico").fadeOut("slow", function () {
                $("#formulario-contacto").fadeIn("slow");
            });

            $("#basico").removeClass("whc-selected");
            $("#contacto").addClass("whc-selected");
        } else if ($("#formulario-contacto").is(":visible")) {
            $("#formulario-contacto").fadeOut("slow", function () {
                $("#formulario-contactocomercial").fadeIn("slow");
            });

            $("#contacto").removeClass("whc-selected");
            $("#contactocomercial").addClass("whc-selected");
        } else if ($("#formulario-contactocomercial").is(":visible")) {
            $("#formulario-contactocomercial").fadeOut("slow", function () {
                $("#formulario-produccion").fadeIn("slow");
            });

            $("#contactocomercial").removeClass("whc-selected");
            $("#produccion").addClass("whc-selected");
        } else if ($("#formulario-produccion").is(":visible")) {
            $("#formulario-produccion").fadeOut("slow", function () {
                $("#formulario-almacenamiento").fadeIn("slow");
            });

            $("#produccion").removeClass("whc-selected");
            $("#almacenamiento").addClass("whc-selected");

            $(".formulario-footer-guardar-contacto").html("Guardar y Finalizar").addClass("invertir-boton-Guardar");
            $(".formulario-footer-siguiente").hide();
        }
    });

    $(".agregargrano").click(function () {
        var id = $(".campo-granos:last-of-type").attr("class").split(" ")[0].split("linea")[1];
        if (!ValidarGranoProduccion(id))
            return false;

        cantGrano++;

        var htmlCampañaGrano = "";

        htmlCampañaGrano += '<div class="linea' + cantGrano + ' campo-granos"  style="position:relative;">'
            + '<select class="campo-input-select campo-sin-span grano" id="grano' + cantGrano + '">'
            + '<option disabled selected value="">Seleccionar...</option>';
        for (var ii in resultInit.gran) {
            (function (i) {
                htmlCampañaGrano += '<option value="' + resultInit.gran[i].MaterialId + '">' + resultInit.gran[i].Descripcion + '</option>';
            })(ii);
        }
        htmlCampañaGrano += '</select>' +
            '<select class="campo-input-select campo-sin-span grano" id="campaña' + cantGrano + '">';
        htmlCampañaGrano += '<option value = "null">Seleccione...</option>';
        htmlCampañaGrano += '</select>'
            + '<input type="text" class="campo-input-text hectareas" style="margin-left: 20px;" id="hectareas' + cantGrano + '" />'
            + '<input type="text" class="campo-input-text toneladas" id="toneladas' + cantGrano + '" />'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano" id="eliminarGrano' + cantGrano + '" />'
            + '</div>';

        $("#formulario-produccion .datos-produccion-cap-prod-editor-granos-cantidades-grupo").append(htmlCampañaGrano);

        $("#grano" + cantGrano + "").change(function (x) {
            var obj = {
                MaterialId: $(this).val(),
                elemId: $(this).prop("id").split("grano")[1]
            };
            armarSelectGrano(obj);
        });

        $("#eliminarGrano" + cantGrano).click(function () {
            eliminarGrano(this);
        });
    });

    $(".agregargrano-almacenamiento").click(function () {
        var id = $(".campo-granos-almacenamiento:last-of-type").attr("class").split(" ")[0].split("lineaAlmacenamiento")[1];
        if (!ValidarGranoAlmacenamiento(cantGranoAlmacenamiento))
            return false;

        cantGranoAlmacenamiento++;

        var htmlCampañaGranoAlmacenamiento = "";
        htmlCampañaGranoAlmacenamiento += '<div class="lineaAlmacenamiento' + cantGranoAlmacenamiento + ' campo-granos-almacenamiento"  style="position:relative;">';
        htmlCampañaGranoAlmacenamiento += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamiento' + cantGranoAlmacenamiento + '">';
        htmlCampañaGranoAlmacenamiento += '<option value = "null">Seleccione...</option>';
        for (var ii in resultCampaña) {
            (function (i) {
                htmlCampañaGranoAlmacenamiento += '<option value="' + resultCampaña[i].CampañaId + '">' + resultCampaña[i].Descripcion + '</option>';
            })(ii);
        }
        htmlCampañaGranoAlmacenamiento += '</select>' +
            '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamiento' + cantGranoAlmacenamiento + '" />';

        htmlCampañaGranoAlmacenamiento += '<div class="almacenamiento-hectareas-linea">' +
            '<div class="formulario-campo-radio">' +
            '<label for="hectareas-arrendadas' + cantGranoAlmacenamiento + '"><input id="hectareas-arrendadas' + cantGranoAlmacenamiento + '" type="radio" name="hectareas' + cantGranoAlmacenamiento + '" value="1"> Alquiladas</label>' +
            '<label for="hectareas-propias' + cantGranoAlmacenamiento + '"><input id="hectareas-propias' + cantGranoAlmacenamiento + '" type="radio" name="hectareas' + cantGranoAlmacenamiento + '" value="0"> Propias</label>' +
            '</div>' +

            '</div>' +
            '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamiento" id="eliminarGrano-almacenamiento' + cantGranoAlmacenamiento + '" />' +
            '</div>';

        $("#formulario-almacenamiento .datos-almacenamiento-cap-prod-editor-granos-cantidades-grupo").append(htmlCampañaGranoAlmacenamiento);

        $("#campañaAlmacenamiento" + cantGranoAlmacenamiento + "").change(function (x) {
            var obj = {
                CampañaId: $(this).val(),
                elemId: $(this).prop("id").split("campañaAlmacenamiento")[1]
            };
            armarSelectGranoAlmacenamiento(obj);
        });

        $("#eliminarGrano-almacenamiento" + cantGranoAlmacenamiento).click(function () {
            eliminarGranoAlmacenamiento(this);
        });
    });

    $(".agregargrano-almacenamientograno").click(function () {
        var id = $(".campo-granos-almacenamientograno:last-of-type").attr("class").split(" ")[0].split("lineaAlmacenamientoGranos")[1];

        cantGranoAlmacenamientoGrano++;

        var htmlCampañaGranoAlmacenamiento = "";
        htmlCampañaGranoAlmacenamiento += '<div class="lineaAlmacenamientoGranos' + cantGranoAlmacenamientoGrano + ' campo-granos-almacenamientograno"   style="position:relative;">';
        htmlCampañaGranoAlmacenamiento += '<select class="campo-input-select campo-sin-span grano" id="granoAlmacenamientoGranos' + cantGranoAlmacenamientoGrano + '">';
        htmlCampañaGranoAlmacenamiento += '<option value = "null">Seleccione...</option>';
        for (var ii in resultInit.gran) {
            (function (i) {
                htmlCampañaGranoAlmacenamiento += '<option value="' + resultInit.gran[i].MaterialId + '">' + resultInit.gran[i].Descripcion + '</option>';
            })(ii);
        }
        htmlCampañaGranoAlmacenamiento += '</select>'
        htmlCampañaGranoAlmacenamiento += '<select class="campo-input-select campo-sin-span grano" id="campañaAlmacenamientoGranos' + cantGranoAlmacenamientoGrano + '">';
        htmlCampañaGranoAlmacenamiento += '<option value = "null">Seleccione...</option>';

        htmlCampañaGranoAlmacenamiento += '</select>' +
            '<input type="text" class="campo-input-text toneladasAlmacenamiento" id="toneladasAlmacenamientoGrano' + cantGranoAlmacenamientoGrano + '" />'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarGrano-almacenamientograno" id="eliminarGrano-almacenamientograno' + cantGranoAlmacenamientoGrano + '" />' +
            '</div>';

        $("#formulario-almacenamiento .datos-almacenamiento-cap-prod-editor-granosalm-cantidades-grupo").append(htmlCampañaGranoAlmacenamiento);

        $("#granoAlmacenamientoGranos" + cantGranoAlmacenamientoGrano + "").change(function (x) {
            var obj = {
                MaterialId: $(this).val(),
                elemId: $(this).prop("id").split("granoAlmacenamientoGranos")[1]
            };
            armarSelectGranoAlmacenamientoGrano(obj);
        });

        $("#eliminarGrano-almacenamientograno" + cantGranoAlmacenamientoGrano).click(function () {
            eliminarGranoAlmacenamientoGrano(this);
        });
    });

    $(".agregargranoObjetivo").click(function () {
        var id = $(".ObjetivoGranos:last-of-type").attr("class").split(" ")[0].split("lineaObjetivos")[1];
        if (!ValidarGranoObjetivo(id))
            return false;

        cantGranoObjetivo++;

        var htmlCampañaGranoObjetivo = "";

        htmlCampañaGranoObjetivo += '<div class="lineaObjetivos' + cantGranoObjetivo + ' ObjetivoGranos" style="position:relative;">'
            + '<select class="campo-input-select campo-sin-span grano" id="granoObjetivo' + cantGranoObjetivo + '">'
            + '<option disabled selected value="">Seleccionar...</option>';
        for (var ii in resultInit.gran) {
            (function (i) {
                htmlCampañaGranoObjetivo += '<option value="' + resultInit.gran[i].MaterialId + '">' + resultInit.gran[i].Descripcion + '</option>';
            })(ii);
        }

        htmlCampañaGranoObjetivo += '</select>'
            + '<select class="campo-input-select campo-sin-span grano" id="campañaObjetivo' + cantGranoObjetivo + '">';
        htmlCampañaGranoObjetivo += '<option value = "null">Seleccione...</option>';

        htmlCampañaGranoObjetivo += '</select>'
            + '<input type="text" class="campo-input-text toneladasObjetivo" style="margin-left: 20px;" id="toneladasObjetivo' + cantGranoObjetivo + '" />'
            + '<img src="../Content/Images/eliminar-tel-mail.png" class="eliminarObjetivos" id="eliminarObjetivo' + cantGranoObjetivo + '" />'
            + '</div>';

        $("#formulario-contacto .datos-produccion-cap-prod-editor-granos-cantidades-grupo").append(htmlCampañaGranoObjetivo);

        $("#granoObjetivo" + cantGranoObjetivo + "").change(function (x) {
            var obj = {
                MaterialId: $(this).val(),
                elemId: $(this).prop("id").split("granoObjetivo")[1]
            };
            armarSelectGranoObjetivo(obj);
        });

        $("#eliminarObjetivo" + cantGranoObjetivo).click(function () {
            eliminarObjetivo(this);
        });
    });

    $("#eliminarObjetivo0").click(function () {
        var val = 0;
        var obj = {
            granoId: $("#granoObjetivo" + val).val(),
            grano: $("#granoObjetivo" + val).find('option:selected').text(),
            campañaId: $("#campañaObjetivo" + val).val(),
            campaña: $("#campañaObjetivo" + val).find('option:selected').text(),
            toneladasObjetivo: $("#toneladasObjetivo" + val).val()
        };

        if (obj.granoId != "null" && obj.campañaId != "null")
            aEliminarObjetivos.push(obj);

        if (val > 0) {
            $(".lineaObjetivos" + val).remove();
            val--;
        } else {
            $("#granoObjetivo" + val).val("null"),
                $("#campañaObjetivo" + val).val("null"),
                $("#toneladasObjetivo" + val).val("")
        }
    });
    $("#eliminarGrano0").click(function () {
        var val = 0;
        var obj = {
            granoId: $("#grano" + val).val(),
            grano: $("#grano" + val).find('option:selected').text(),
            campañaId: $("#campaña" + val).val(),
            campaña: $("#campaña" + val).find('option:selected').text(),
            hectareas: $("#hectareas" + val).val(),
            toneladas: $("#toneladas" + val).val(),
            CampoId: $("#campoid").val()
        };

        if (obj.granoId != "null" && obj.campañaId != "null")
            aEliminarGranos.push(obj);

        if (val > 0) {
            $(".linea" + val).remove();
            val--;
        } else {
            $("#grano" + val).val("null");
            $("#campaña" + val).val("null");
            $("#toneladas" + val).val("");
            $("#hectareas" + val).val("");
        }
    });

    $("#cuit").blur(function (ev) {
        var valor = $("#cuit").val();
        if (valor.length > 10)
            buscarRazonSocial(valor);
    });

    $(".formulario-footer-guardar-contacto").click(function () {
        if (comprobarInputs()) {
            if (validar()) {
                ObtenerDatos();
            }
        }
    });

    $("#concom-agregarMail").click(function () {
        if (!($("#concom-email2") && $("#concom-email2").length > 0)) {
            if (!validateEmail($("#concom-email1").val())) {
                MensErr("El Email no es válido");
                return false;
            }

            var div = "";
            div += '<div class="formulario-campo">'
                + '<input type="text" class="campo-input-text" id="concom-email2" style="margin-right: 50px;margin-top: 5px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarMail2" />'
                + '</div>';
            $(".concom-campo-email").append(div);

            $("#concom-eliminarMail2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#concom-email3") && $("#concom-email3").length > 0)) {
            if (!validateEmail($("#concom-email2").val())) {
                MensErr("El segundo Email no es válido");
                return false;
            }
            var div = "";
            div += '<div class="formulario-campo">'
                + '<input type="text" class="campo-input-text" id="concom-email3" style="margin-right: 50px;margin-top: 5px;" />'
                + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarMail3" />'
                + '</div>';
            $(".concom-campo-email").append(div);

            $("#concom-eliminarMail3").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 3 correos electrónicos.");
        }
    });

    $("#GuardarContactoComercial").click(function () {
        if (!ValidarContactoComercial())
            return false;

        var obj = {};
        obj.nombre = $("#concom-nombre").val();
        obj.apellido = $("#concom-apellido").val();

        obj.emails = [];
        if (($("#concom-email1") && $("#concom-email1").length > 0 && $("#concom-email1").val()) ||
            ($("#concom-email2") && $("#concom-email2").length > 0 && $("#concom-email2").val()) ||
            ($("#concom-email3") && $("#concom-email3").length > 0 && $("#concom-email3").val())) {
            if ($("#concom-email1") && $("#concom-email1").length > 0 && $("#concom-email1").val()) {
                if (!validateEmail($("#concom-email1").val())) {
                    MensErr("El primer Email no es válido");
                    return false;
                }
                obj.emails.push($("#concom-email1").val());
            } else {
                obj.emails.push(null);
            }

            if ($("#concom-email2") && $("#concom-email2").length > 0 && $("#concom-email2").val()) {
                if (!validateEmail($("#concom-email2").val())) {
                    MensErr("El segundo Email no es válido");
                    return false;
                }
                obj.emails.push($("#concom-email2").val());
            } else {
                obj.emails.push(null);
            }

            if ($("#concom-email3") && $("#concom-email3").length > 0 && $("#concom-email3").val()) {
                if (!validateEmail($("#concom-email3").val())) {
                    MensErr("El tercer Email no es válido");
                    return false;
                }
                obj.emails.push($("#concom-email3").val());
            } else {
                obj.emails.push(null);
            }
        } else {
            obj.emails.push(null);
            obj.emails.push(null);
            obj.emails.push(null);
        }

        obj.telefonos = [];
        if (($("#concom-Telefono1") && $("#concom-Telefono1").length > 0 && $("#concom-Telefono1").val()) ||
            ($("#concom-Telefono2") && $("#concom-Telefono2").length > 0 && $("#concom-Telefono2").val()) ||
            ($("#concom-Telefono3") && $("#concom-Telefono3").length > 0 && $("#concom-Telefono3").val())) {
            if ($("#concom-Telefono1") && $("#concom-Telefono1").length > 0 && $("#concom-Telefono1").val()) {
                if (((!$("#concom-TipoTelefono1").val() || $("#concom-TipoTelefono1").val() === "null") && ($("#concom-Telefono1").val() && $("#concom-Telefono1").val() !== "")) || (($("#concom-TipoTelefono1").val() && $("#concom-TipoTelefono1").val() !== "null") && (!$("#concom-Telefono1").val() && $("#concom-Telefono1").val() === ""))) {
                    MensErr("El contacto comercial debe tener bien cargado el primer telefono");
                    return false;
                }

                if (!validateNumber($("#concom-Telefono1").val())) {
                    MensErr("El primer Telefono no es válido");
                    return false;
                }

                obj.telefonos.push({
                    tipoTelefono: $("#concom-TipoTelefono1").val(),
                    telefono: $("#concom-Telefono1").val()
                });
            } else {
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            }
            if ($("#concom-Telefono2") && $("#concom-Telefono2").length > 0 && $("#concom-Telefono2").val()) {
                if (((!$("#concom-TipoTelefono2").val() || $("#concom-TipoTelefono2").val() === "null") && ($("#concom-Telefono2").val() && $("#concom-Telefono2").val() !== "")) || (($("#concom-TipoTelefono2").val() && $("#concom-TipoTelefono2").val() !== "null") && (!$("#concom-Telefono2").val() && $("#concom-Telefono2").val() === ""))) {
                    MensErr("El contacto comercial debe tener bien cargado el segundo telefono");
                    return false;
                }

                if (!validateNumber($("#concom-Telefono2").val())) {
                    MensErr("El segundo Telefono no es válido");
                    return false;
                }

                obj.telefonos.push({
                    tipoTelefono: $("#concom-TipoTelefono2").val(),
                    telefono: $("#concom-Telefono2").val()
                });
            } else {
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            }
            if ($("#concom-Telefono2").val() > 0) {
                if (!validateNumber($("#concom-Telefono2").val())) {
                    MensErr("El Telefono no es válido");
                    return false;
                }
            }

            if ($("#concom-Telefono3") && $("#concom-Telefono3").length > 0 && $("#concom-Telefono3").val()) {
                if (((!$("#concom-TipoTelefono3").val() || $("#concom-TipoTelefono3").val() === "null") && ($("#concom-Telefono3").val() && $("#concom-Telefono3").val() !== "")) || (($("#concom-TipoTelefono3").val() && $("#concom-TipoTelefono3").val() !== "null") && (!$("#concom-Telefono3").val() && $("#concom-Telefono3").val() === ""))) {
                    MensErr("El contacto comercial debe tener bien cargado el tercer telefono");
                    return false;
                }

                if (!validateNumber($("#concom-Telefono1").val())) {
                    MensErr("El tercer Telefono no es válido");
                    return false;
                }

                obj.telefonos.push({
                    tipoTelefono: $("#concom-TipoTelefono3").val(),
                    telefono: $("#concom-Telefono3").val()
                });
            } else {
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            }
        } else {
            obj.telefonos.push({
                tipoTelefono: null,
                telefono: null
            });
            obj.telefonos.push({
                tipoTelefono: null,
                telefono: null
            });
            obj.telefonos.push({
                tipoTelefono: null,
                telefono: null
            });
        }
        if ($("#concom-Telefono3").val()) {
            if (!validateNumber($("#concom-Telefono3").val())) {
                MensErr("El Telefono no es válido");
                return false;
            }
        }

        cantContactoComercial++;
        obj.contactoComercialId = $("#concom-id").val() ? $("#concom-id").val() : 0;
        obj.dia = $("#concom-fechanacimientodia").val();
        obj.mes = $("#concom-fechanacimientomes").val();
        obj.anio = $("#concom-anionacimientomes").val();

        if (obj.anio != "null" && obj.mes != "null" && obj.dia != "null")
            obj.fechaNacimiento = new Date(obj.anio, (obj.mes - 1), obj.dia);
        else
            obj.fechaNacimiento = null;
        obj.cargo = $("#concom-cargo").val();
        obj.puesto = $("#concom-puesto").val();
        obj.intereses = $("#concom-intereses").val() != "" ? $("#concom-intereses").val() : [];
        obj.interesesNombre = [];
        $("#concom-intereses option:selected").each(function () {
            var $this = $(this);
            if ($this.length) {
                var selText = $this.text();
                obj.interesesNombre.push(selText);
            }
        });

        obj.otrosIntereses = $("#concom-otrosintereses").val();
        obj.principal = $("#concom-principal").is(":checked") ? 1 : 0;
        obj.item = cantContactoComercial;

        if (obj.principal) {
            for (var ii in aGuardarContactoComercial) {
                (function (i) {
                    aGuardarContactoComercial[i].principal = 0;
                })(ii);
            }
        }

        aGuardarContactoComercial.push(obj);

        var htmlComerciales = "";
        htmlComerciales += '<div class="contenedor-contacto-comercial" id="comercial' + cantContactoComercial + '">' +
            '<div class="contenedor-contacto-comercial-titulo">' +
            '<img class="img-contacto-comercial" src="../Content/Images/contprinc-cont4.png" /> ' +
            '<span class="span-contacto-comercial"> ' +
            obj.nombre + ' ' + obj.apellido +
            '</span>' +
            '<span class="align-right" onclick="editarContactoComercial(' + cantContactoComercial + ')" id="concom-editar' + cantContactoComercial + '">' +
            '<img class="contacto-edit-img" src="../Content/Images/contacto-edit.png" /> ' +
            '<span class="editar-contacto editar-contacto-comercial">' +
            'Editar' +
            '</span>' +
            '</span>' +
            '<span style="margin-right:5px;" onclick="eliminarContactoComercial(this)" class="align-right" id="concom-eliminar' + cantContactoComercial + '">' +
            '<span class="editar-contacto editar-contacto-comercial">' +
            'x Eliminar' +
            '</span>' +
            '</span>' +
            '</div>' +
            '<div class="contenedor-contacto-comercial-posicion">' +
            '<span class="contenedor-contacto-comercial-posicion-izq">' +
            (obj.cargo ? obj.cargo : "No se especifica cargo") + ' - ' +
            '</span>' +
            '<span class="contenedor-contacto-comercial-posicion-der">' +
            (obj.puesto ? obj.puesto : "No se especifica puesto") +
            '</span>' +
            '</div>' +
            '<div class="contenedor-contacto-comercial-telefonos">' +
            (obj.telefonos[0].telefono ? obj.telefonos[0].telefono + (obj.telefonos[1].telefono ? " - " + obj.telefonos[1].telefono : "") + (obj.telefonos[2].telefono ? " - " + obj.telefonos[2].telefono : "") : "No especifica teléfono") +
            '</div>' +
            '<div class="contenedor-contacto-comercial-mails">' +
            (obj.emails[0] ? obj.emails[0] + (obj.emails[1] ? " - " + obj.emails[1] : "") + (obj.emails[2] ? " - " + obj.emails[2] : "") : "No especifica mails") +
            '</div>' +
            '<div class="contenedor-contacto-comercial-extras">' +
            '<div class="row">' +
            '<div class="col-lg-6">' +
            '<span class="contenedor-contacto-comercial-extras-label">' +
            'Fecha de nacimiento:' +
            '</span>' +
            '</div>' +
            '<div class="col-lg-6">' +
            '<span class="contenedor-contacto-comercial-extras-value">' +
            (obj.fechaNacimiento ? kendo.toString(obj.fechaNacimiento, "m") + " de " + kendo.toString(obj.anio) : "no especifica") +
            '</span>' +
            '</div>' +
            '</div>' +
            '<div class="row">' +
            '<div class="col-lg-6">' +
            '<span class="contenedor-contacto-comercial-extras-label">' +
            'Intereses' +
            '</span>' +
            '</div>' +
            '<div class="col-lg-6">' +
            '<span class="contenedor-contacto-comercial-extras-value">' +
            (obj.interesesNombre ? obj.interesesNombre.join("<br>") + "<br>" + (obj.otrosIntereses ? obj.otrosIntereses : "") : (obj.otrosIntereses ? obj.otrosIntereses : "No especifica")) +
            '</span>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

        $(".datos-contactocomercial-guardados").append(htmlComerciales);

        $("#concom-nombre").val("");
        $("#concom-apellido").val("");
        $("#concom-email1").val("");
        if ($("#concom-email2").length)
            $("#concom-email2").parent().remove();
        if ($("#concom-email3").length)
            $("#concom-email3").parent().remove();
        $("#concom-email1").val("");
        $("#concom-TipoTelefono1").val("null");
        $("#concom-Telefono1").val("");
        if ($("#concom-TipoTelefono2").length) {
            $("#concom-TipoTelefono2").parent().remove();
        }
        if ($("#concom-TipoTelefono3").length) {
            $("#concom-TipoTelefono3").parent().remove();
        }
        $("#concom-fechanacimientodia").val("null");
        $("#concom-fechanacimientomes").val("null");
        $("#concom-anionacimientomes").val("null");
        $("#concom-cargo").val("");
        $("#concom-puesto").val("");
        $("#concom-intereses").multiselect("uncheckAll");
        $("#concom-otrosintereses").val("");
        $("#concom-id").val(0);
        $("#concom-principal").prop("checked", false);
    });
}

function eliminarContactoComercial(val) {
    var item = $(val).attr("id").split("concom-eliminar")[1];
    $("#comercial" + item).remove();
    aGuardarContactoComercial = aGuardarContactoComercial.filter(function (el) {
        return el.item !== parseInt(item);
    });
}

function editarContactoComercial(id) {
    var obj = aGuardarContactoComercial.filter(function (el) {
        return el.item === parseInt(id);
    });
    obj = obj[0];

    $("#concom-nombre").val(obj.nombre);
    $("#concom-apellido").val(obj.apellido);
    $("#concom-fechanacimientodia").val(obj.dia);
    $("#concom-fechanacimientomes").val(obj.mes);
    $("#concom-anionacimientomes").val(obj.anio);
    $("#concom-cargo").val(obj.cargo);
    $("#concom-puesto").val(obj.puesto);
    $("#concom-intereses").val(obj.intereses);
    $("#concom-intereses").multiselect("refresh");
    $("#concom-otrosintereses").val(obj.otrosIntereses);

    if (obj.principal == 1) {
        $("#concom-principal").prop("checked", true);
    } else {
        $("#concom-principal").prop("checked", false);
    }

    var cantEmails = obj.emails.length;
    $("#concom-email1").val(obj.emails[0]);
    for (var i = 2; i <= cantEmails; i++) {
        if (obj.emails[i - 1] != null) {
            if (!($("#concom-email" + i + "") && $("#concom-email" + i + "").length > 0)) {
                var div = "";
                div += '<div class="formulario-campo">'
                    + '<input type="text" class="campo-input-text" id="concom-email' + i + '" style="margin-right: 50px;margin-top: 5px;" />'
                    + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarMail' + i + '" />'
                    + '</div>';
                $(".concom-campo-email").append(div);

                $("#concom-eliminarMail" + i + "").click(function () {
                    $(this).parent().remove();
                });

                $("#concom-email" + i + "").val(obj.emails[i - 1]);
            }
        }
    }

    var cantTelefonos = obj.telefonos.length;
    $("#concom-TipoTelefono1").val(obj.telefonos[0].tipoTelefono);
    $("#concom-Telefono1").val(obj.telefonos[0].telefono);
    for (var i = 2; i <= cantTelefonos; i++) {
        if (obj.telefonos[i - 1].TipoTelefono != null && obj.telefonos[i - 1].telefono) {
            if (!($("#concom-Telefono" + i + "") && $("#concom-Telefono" + i + "").length > 0)) {
                var htmlTipoTelefono = '<div class="formulario-campo campo-tiptelefono">';
                htmlTipoTelefono += '<select  class="campo-input-select" id="concom-TipoTelefono' + i + '">';
                htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
                for (var jj in resultInit.tiptel) {
                    (function (j) {
                        htmlTipoTelefono += '<option value="' + resultInit.tiptel[j].TipoTelefonoId + '">' + resultInit.tiptel[j].Descripcion + '</option>';
                    })(jj);
                }
                htmlTipoTelefono += '</select>'
                    + '<input type="text" placeholder="Telefono"  class="campo-input-text" id="concom-Telefono' + i + '" style="margin-right: 64px;margin-top: 5px;" />'
                    + '<img src="../Content/Images/eliminar-tel-mail.png" id="concom-eliminarTelefono' + i + '" />'
                    + '</div>';

                $(".concom-campo-telefono").append(htmlTipoTelefono);

                $("#concom-eliminarTelefono" + i + "").click(function () {
                    $(this).parent().remove();
                });

                $("#concom-TipoTelefono" + i + "").val(obj.telefonos[i - 1].tipoTelefono);
                $("#concom-Telefono" + i + "").val(obj.telefonos[i - 1].telefono);
            }
        }
    }
    $("#concom-id").val(obj.contactoComercialId ? obj.contactoComercialId : 0);
    $("#comercial" + id).remove();
    aGuardarContactoComercial = aGuardarContactoComercial.filter(function (el) {
        return el.item !== parseInt(id);
    });
}

function InicializarDatos() {
    var Datos = {
        ProveedorId: 0
    };
    var result = resultInit = MSExecuteOnServer('/Proveedor/Iniciliazar', Datos);

    if (result != null) {
        resultGranos = result.gran;
        armarSelects(result);
    }
}

function ObtenerDatos() {
    var obj = {};
    obj.basicos = {};
    obj.contacto = {};
    obj.produccion = {};
    obj.almacenamiento = {};

    obj.basicos.cuit = $("#cuit").val();

    obj.basicos.razonsocial = $("#razonsocial").val();

    obj.basicos.nocliente = $("#nocliente").is(":checked") ? "1" : "0";

    obj.basicos.segmentacion = $("#segmentacion").val();
    obj.basicos.calificacion = $("#calificacion").val();
    if (obj.basicos.calificacion)
        obj.basicos.calificacion = obj.basicos.calificacion[0];

    obj.basicos.ClasificacionCompraNet = $("#clasificacion-compranet").val();
    obj.basicos.BoletoCompraNet = $("#boleto-compranet").val();
    obj.basicos.BolsaCompraNet = $("#bolsa-compranet").val();
    obj.basicos.ProvinciaCompraNet = $("#provincia-compranet").val();
    obj.basicos.LocalidadCompraNet = $("#localidad-compranet").val();

    obj.basicos.comentario = $("#comentario").val();

    obj.contacto.provincia = $("#provincia").val();

    obj.contacto.localidad = $("#localidad").val();

    obj.contacto.direccion = $("#direccion").val();

    obj.contacto.codpost = $("#codpost").val();

    obj.contacto.canalesOperacion = $("#canopera").val();

    obj.contacto.entregaA = $("#entregaA").val();

    obj.contacto.condPreferentes = $("#condPrefer").val();

    obj.contacto.intermediario = $("#intermediario").val();

    if ($("#influencia-zona-unica").is(":checked")) {
        obj.contacto.areaDeInfluencia = $("#influencia-zona-unica").prop("value");
    } else if ($("#influencia-multizona").is(":checked")) {
        obj.contacto.areaDeInfluencia = $("#influencia-multizona").prop("value");
    } else
        obj.contacto.areaDeInfluencia = null;

    obj.produccion.CamposProduccion = aGuardar != null ? aGuardar.concat() : [];
    for (var ii in obj.produccion.CamposProduccion) {
        (function (i) {
            obj.produccion.CamposProduccion[i].archivoFileResult = (obj.produccion.CamposProduccion[i].archivoFileReader ? obj.produccion.CamposProduccion[i].archivoFileReader.result : null);
        })(ii);
    }

    obj.produccion.objetivos = [];
    for (var i = 0; i < (cantGranoObjetivo + 1); i++) {
        if ($("#granoObjetivo" + i).length > 0 && $("#granoObjetivo" + i).val() != "null" && $("#campañaObjetivo" + i).val() != "null") {
            obj.produccion.objetivos.push({
                granoId: $("#granoObjetivo" + i).val(),
                grano: $("#granoObjetivo" + i).find('option:selected').text(),
                campañaId: $("#campañaObjetivo" + i).val(),
                campaña: $("#campañaObjetivo" + i).find('option:selected').text(),
                toneladasObjetivo: $("#toneladasObjetivo" + i).val()
            });
        }
    }

    obj.produccion.eliminarobjetivos = aEliminarObjetivos.concat();

    obj.produccion.volumenAnualTotalTns = $("#volumen-anual-total-tns").val();

    obj.produccion.TonsMaxAprobSojaSust = $("#tons-max-aprob-sojasust").val();

    obj.produccion.hasAprobSojaSust = $("#has-aprob-sojasust").val();

    if ($("#sojasust-si").is(":checked")) {
        obj.produccion.habilitaoSojaSust = $("#sojasust-si").prop("value");
    } else if ($("#sojasust-no").is(":checked")) {
        obj.produccion.habilitaoSojaSust = $("#sojasust-no").prop("value");
    } else {
        obj.produccion.habilitaoSojaSust = null;
    }
    obj.almacenamiento.CamposAlmacenamiento = aGuardarAlmacenamiento;
    for (var ii in obj.almacenamiento.CamposAlmacenamiento) {
        (function (i) {
            obj.almacenamiento.CamposAlmacenamiento[i].archivoFileResult = (obj.almacenamiento.CamposAlmacenamiento[i].archivoFileReader ? obj.almacenamiento.CamposAlmacenamiento[i].archivoFileReader.result : null);
        })(ii);
    }
    obj.contactocomercial = aGuardarContactoComercial;

    GrabarProveedor(obj)
}

function GrabarProveedor(nuevoProveedor) {
    if (ProveedorId)
        nuevoProveedor.ProveedorId = ProveedorId;

    var result = MSExecuteOnServer('/Proveedor/GrabarProveedor', nuevoProveedor);
    if (result != null) {
        if (ExistsErrorMessages(result.Errores)) {
            MensErr(result.Errores[0].Message);
        }
        else {
            MensInfo("Se ha realizado la operacion con exito");
            window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
        }
    }
}

function armarSelectGrano(obj) {
    var elem = $("#campaña" + obj.elemId + "");
    elem.empty();
    elem.append($('<option>', {
        value: "null",
        text: "Seleccione..."
    }));
    if (obj.MaterialId && obj.MaterialId !== "null") {
        var resultGrano = MSExecuteOnServer('/Proveedor/TraerCampañaPorMaterial', { MaterialId: obj.MaterialId });
        if (resultGrano.Errores) {
        } else {
            for (var ii in resultGrano) {
                (function (i) {
                    var esta = 0;
                    for (var j = 0; j < obj.elemId; j++) {
                        if ($("#campaña" + j).val() == resultGrano[i].CampañaId && $("#grano" + j).val() == obj.MaterialId)
                            esta = 1;
                    }
                    if (!esta) {
                        elem.append($('<option>', {
                            value: resultGrano[i].CampañaId,
                            text: resultGrano[i].Descripcion
                        }));
                    }
                })(ii);
            }
        }
    }
}

function armarSelectGranoAlmacenamiento(obj) {
    var elem = $("#granoAlmacenamiento" + obj.elemId + "");
    elem.empty();
    elem.append($('<option>', {
        value: "null",
        text: "Seleccione..."
    }));
    if (obj.MaterialId && obj.MaterialId !== "null") {
        var resultGrano = MSExecuteOnServer('/Proveedor/TraerCampañaPorMaterial', { MaterialId: obj.MaterialId });
        if (resultGrano.Errores) {
        } else {
            for (var ii in resultGrano) {
                (function (i) {
                    var esta = 0;
                    for (var j = 0; j < obj.elemId; j++) {
                        if ($("#granoAlmacenamiento" + j).val() == resultGrano[i].MaterialId)
                            esta = 1;
                    }
                    if (!esta) {
                        elem.append($('<option>', {
                            value: resultGrano[i].MaterialId,
                            text: resultGrano[i].Descripcion
                        }));
                    }
                })(ii);
            }
        }
    }
}

function armarSelectGranoAlmacenamientoGrano(obj) {
    var elem = $("#campañaAlmacenamientoGranos" + obj.elemId + "");
    elem.empty();
    elem.append($('<option>', {
        value: "null",
        text: "Seleccione..."
    }));
    if (obj.MaterialId && obj.MaterialId !== "null") {
        var resultGrano = MSExecuteOnServer('/Proveedor/TraerCampañaPorMaterial', { MaterialId: obj.MaterialId });
        if (resultGrano.Errores) {
        } else {
            for (var ii in resultGrano) {
                (function (i) {
                    var esta = 0;
                    for (var j = 0; j < obj.elemId; j++) {
                        if ($("#campañaAlmacenamientoGranos" + j).val() == resultGrano[i].CampañaId && $("#granoAlmacenamientoGranos" + j).val() == obj.MaterialId)
                            esta = 1;
                    }
                    if (!esta) {
                        elem.append($('<option>', {
                            value: resultGrano[i].CampañaId,
                            text: resultGrano[i].Descripcion
                        }));
                    }
                })(ii);
            }
        }
    }
}

function armarSelectGranoObjetivo(obj) {
    var elem = $("#campañaObjetivo" + obj.elemId + "");
    elem.empty();
    elem.append($('<option>', {
        value: "null",
        text: "Seleccione..."
    }));
    var resultGrano = MSExecuteOnServer('/Proveedor/TraerCampañaPorMaterial', { MaterialId: obj.MaterialId });
    if (resultGrano.Errores) {
    } else {
        for (var ii in resultGrano) {
            (function (i) {
                var esta = 0;
                for (var j = 0; j < obj.elemId; j++) {
                    if ($("#campañaObjetivo" + j).val() == resultGrano[i].CampañaId && ($("#granoObjetivo" + j).val() == obj.MaterialId))
                        esta = 1;
                }
                if (!esta) {
                    elem.append($('<option>', {
                        value: resultGrano[i].CampañaId,
                        text: resultGrano[i].Descripcion
                    }));
                }
            })(ii);
        }
    }
}

function validateEmail(email) {
    var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

function validateNumber(number) {
    var re = /^[0-9]+$/;
    return re.test(number);
}

function comprobarInputs() {
    var hayErrores = 0;
    if ($("#cuit").val() && $("#cuit").val().length > 20) {
        mostrarError("#cuit", "error-elem-cuit", "No debe superar los 20 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#cuit", "error-elem-cuit");
    }

    if ($("#nomReferente").val() && $("#nomReferente").val().length > 100) {
        mostrarError("#nomReferente", "error-elem-nomref", "No debe superar los 100 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#nomReferente", "error-elem-nomref");
    }

    if ($("#Email1").val() && $("#Email1").val().length > 255) {
        mostrarError("#Email1", "error-elem-Email1", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Email1", "error-elem-Email1");
    }

    if ($("#Email2").val() && $("#Email2").val().length > 255) {
        mostrarError("#Email2", "error-elem-Email2", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Email2", "error-elem-Email2");
    }

    if ($("#Email3").val() && $("#Email3").val().length > 255) {
        mostrarError("#Email3", "error-elem-Email3", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Email3", "error-elem-Email3");
    }

    if ($("#Email4").val() && $("#Email4").val().length > 255) {
        mostrarError("#Email4", "error-elem-Email4", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Email4", "error-elem-Email4");
    }

    if ($("#Telefono1").val() && $("#Telefono1").val().length > 255) {
        mostrarError("#Telefono1", "error-elem-Telefono1", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Telefono1", "error-elem-Telefono1");
    }

    if ($("#Telefono2").val() && $("#Telefono2").val().length > 255) {
        mostrarError("#Telefono2", "error-elem-Telefono2", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Telefono2", "error-elem-Telefono2");
    }

    if ($("#Telefono3").val() && $("#Telefono3").val().length > 255) {
        mostrarError("#Telefono3", "error-elem-Telefono3", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Telefono3", "error-elem-Telefono3");
    }

    if ($("#Telefono4").val() && $("#Telefono4").val().length > 255) {
        mostrarError("#Telefono4", "error-elem-Telefono4", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#Telefono4", "error-elem-Telefono4");
    }

    if ($("#comentario").val() && $("#comentario").val().length > 500) {
        mostrarError("#comentario", "error-elem-comentario", "No debe superar los 500 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#comentario", "error-elem-comentario");
    }

    if ($("#direccion").val() && $("#direccion").val().length > 100) {
        mostrarError("#direccion", "error-elem-direccion", "No debe superar los 100 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#direccion", "error-elem-direccion");
    }

    if ($("#codpost").val() && $("#codpost").val().length > 10) {
        mostrarError("#codpost", "error-elem-codpost", "No debe superar los 10 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#codpost", "error-elem-codpost");
    }

    if ($("#intermediario").val() && $("#intermediario").val().length > 255) {
        mostrarError("#intermediario", "error-elem-intermediario", "No debe superar los 255 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#intermediario", "error-elem-intermediario");
    }
    if ($("#volumen-anual-total-tns").val() && $("#volumen-anual-total-tns").val().length > 15) {
        mostrarError("#volumen-anual-total-tns", "error-elem-volumen-anual-total-tns", "No debe superar los 15 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#volumen-anual-total-tns", "error-elem-volumen-anual-total-tns");
    }

    if ($("#tons-max-aprob-sojasust").val() && $("#tons-max-aprob-sojasust").val().length > 15) {
        mostrarError("#tons-max-aprob-sojasust", "error-elem-tons-max-aprob-sojasust", "No debe superar los 15 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#tons-max-aprob-sojasust", "error-elem-tons-max-aprob-sojasust");
    }

    if ($("#has-aprob-sojasust").val() && $("#has-aprob-sojasust").val().length > 15) {
        mostrarError("#has-aprob-sojasust", "error-elem-has-aprob-sojasust", "No debe superar los 15 caracteres");
        hayErrores = 1;
    } else {
        eliminarError("#has-aprob-sojasust", "error-elem-has-aprob-sojasust");
    }
    if (hayErrores) {
        return false;
    }
    else {
        return true;
    }
}

function mostrarError(elem, clase, textoError) {
    eliminarError(elem, clase);
    $(elem).addClass("input-error");
    $("<div>").addClass("tooltip-error").addClass("error-" + clase).html(textoError).appendTo($(elem).parent());
    $("<div>").addClass("triangulo-error").addClass("triangulo-" + clase).appendTo($(elem).parent());

    $(elem).hover(function () {
        $(".tooltip-error.error-" + clase).show();
        $(".triangulo-error.triangulo-" + clase).show();
    }, function () {
        $(".tooltip-error.error-" + clase).hide();
        $(".triangulo-error.triangulo-" + clase).hide();
    });

    $(".tooltip-error.error-" + clase).hover(function () {
        $(".tooltip-error.error-" + clase).show();
        $(".triangulo-error.triangulo-" + clase).show();
    }, function () {
        $(".tooltip-error.error-" + clase).hide();
        $(".triangulo-error.triangulo-" + clase).hide();
    });

    $(".triangulo-error.triangulo-" + clase).hover(function () {
        $(".tooltip-error.error-" + clase).show();
        $(".triangulo-error.triangulo-" + clase).show();
    }, function () {
        $(".tooltip-error.error-" + clase).hide();
        $(".triangulo-error.triangulo-" + clase).hide();
    });
}

function eliminarError(elem, clase) {
    $(elem).removeClass("input-error");
    $(".tooltip-error.error-" + clase).remove();
    $(".triangulo-error.triangulo-" + clase).remove();
}