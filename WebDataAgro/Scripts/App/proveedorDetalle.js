var cantGrano = 0;
var capProdCant = 0;
var produccionCampoGuardar = [];
var resultDatos = {};
var grupoacopio = {};
var grupocampoacopio = {};
var campaniaSeleccionada;
var grupoestablecimiento = {};

$(document).ready(function () {
    kendo.culture("es-AR");
    //inicializarGrafico();
    setChangeChecks();
    modificarHeader();
    armarContacto();
    armarEstilosyFuncionesDetalle();
    //armarEstilosyFuncionesEditable();
    InicializarDatos();
    $("#Imprimir").click(function () {
        ImprimirReporte();
    });
    $("#ExportarPdf").click(function () {
        ExportarPdf();
    });

    $(window).resize(function () {
        if ($("#campaña").val() && $("#campaña").val() !== "null") {
            if ($("#campaña").val() != "-1") {
                inicializarGrafico(val[$("#campaña").val()], $("#campaña").val());
            } else {

                inicializarGrafico(campaniaSeleccionada);
            }
        }
    });

});

function createChart(val) {
    var cant = 0;
    var mes = 1;
    var axis = [];
    var series = [];
    var anio = 0;
    var minaño = 0;
    var maxaño = 0;

    if ($("#campaña").val() != "-1") {
        var obj = {};
        obj[$("#campaña").val()] = val;
        val = obj;
    }

    for (var ii in val) {
        (function (i) {
            for (var jj in val[i].Campañas) {
                (function (j) {
                    if (minaño == 0) {
                        minaño = val[i].Campañas[j].Año;
                    } else {
                        if (minaño > val[i].Campañas[j].Año) {
                            minaño = val[i].Campañas[j].Año;
                        }
                    }
                    if (maxaño == 0) {
                        maxaño = val[i].Campañas[j].Año;
                    } else {
                        if (maxaño < val[i].Campañas[j].Año) {
                            maxaño = val[i].Campañas[j].Año;
                        }
                    }
                })(jj);
            }
        })(ii);
    }

    var año = minaño;

    var valoresaux = {};
    var x = año;
    for (var jj in val) {
        (function (j) {
            while (x <= maxaño) {
                for (var i = 1; i <= 12; i++) {
                    valoresaux[j] = valoresaux[j] || [];
                    valoresaux[j].push({
                        año: x,
                        mes: i,
                        total: 0
                    });
                }
                x++;
            }
            x = año;
        })(jj);
    }
    for (var ii in val) {
        (function (i) {
            for (var jj in val[i].Campañas) {
                (function (j) {
                    if (valoresaux[i].filter(function (x) { return x.año == val[i].Campañas[j].Año && x.mes == val[i].Campañas[j].Mes; }).length > 0) {
                        var index = -1;
                        for (var r = 0; r < valoresaux[i].length; ++r) {
                            if (valoresaux[i][r].año == val[i].Campañas[j].Año && valoresaux[i][r].mes == val[i].Campañas[j].Mes) {
                                index = r;
                                break;
                            }
                        }

                        valoresaux[i][/*valoresaux[i].findIndex(function (x) { return x.año == val[i].Campañas[j].Año && x.mes == val[i].Campañas[j].Mes; })*/index].total = val[i].Campañas[j].Total;//ToneladasAux;
                    }
                })(jj);
            }
        })(ii);
    }
    var obj = {};
    var vals = [];
    for (var ii in valoresaux) {
        (function (i) {
            obj = {};
            vals = [];
            obj.name = i;
            for (var jj in valoresaux[i]) {
                (function (j) {
                    vals.push(valoresaux[i][j].total);
                    switch (valoresaux[i][j].mes) {
                        case 1:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - ene"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - ene");
                            break;
                        case 2:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - feb"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - feb");
                            break;
                        case 3:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - mar"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - mar");
                            break;
                        case 4:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - abr"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - abr");
                            break;
                        case 5:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - may"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - may");
                            break;
                        case 6:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - jun"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - jun");
                            break;
                        case 7:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - jul"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - jul");
                            break;
                        case 8:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - ago"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - ago");
                            break;
                        case 9:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - sep"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - sep");
                            break;
                        case 10:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - oct"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - oct");
                            break;
                        case 11:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - nov"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - nov");
                            break;
                        case 12:
                            if ($.inArray((valoresaux[i][j].año.toString().substring(2, 4) + " - dic"), axis) == -1)
                                axis.push(valoresaux[i][j].año.toString().substring(2, 4) + " - dic");
                            break;
                    }
                })(jj);
            }
            obj.data = vals.concat();
            series.push(obj);
        })(ii);
    }

    var minindex = -1;

    for (var ii in series) {
        (function (i) {
            var minindexaux;
            for (var r = 0; r < series[i].data.length; ++r) {
                if (series[i].data[r] > 0) {
                    minindexaux = r;
                    break;
                }
            }

            if (minindex == -1) {
                minindex = minindexaux;
                //minindex = series[i].data.findIndex(function(x){return x > 0});
            } else if (minindex > minindexaux) {
                minindex = minindexaux;//series[i].data.findIndex(function(x){return x > 0});
            }
        })(ii);
    }

    for (var ii in series) {
        (function (i) {
            for (var j = 0; j < minindex; j++) {
                series[i].data.splice(0, 1);
            }
        })(ii);
    }

    for (var j = 0; j < minindex; j++) {
        axis.splice(0, 1);
    }

    var maxindex = -1;

    for (var ii in series) {
        (function (i) {
            var aux = series[i].data.concat().reverse().concat();

            var maxindexaux;
            for (var r = 0; r < aux.length; ++r) {
                if (aux[r] > 0) {
                    maxindexaux = r;
                    break;
                }
            }

            if (maxindex == -1) {
                maxindex = maxindexaux;//aux.findIndex(function (x) { return x > 0 });
            } else if (maxindex > maxindexaux) {//aux.findIndex(function (x) { return x > 0 })) {
                maxindex = maxindexaux;//aux.findIndex(function (x) { return x > 0 });
            }
        })(ii);
    }

    for (var ii in series) {
        (function (i) {
            series[i].data = series[i].data.reverse();
            for (var j = 0; j < maxindex; j++) {
                series[i].data.splice(0, 1);
            }
            series[i].data = series[i].data.reverse();
        })(ii);
    }

    axis.reverse();
    for (var j = 0; j < maxindex; j++) {
        axis.splice(0, 1);
    }
    axis.reverse();

    $("#chart").kendoChart({
        legend: {
            position: "bottom"
        },
        chartArea: {
            background: "",
            height: 200
        },
        seriesDefaults: {
            type: "line",
            style: "smooth"
        },
        seriesColors: ["orange", "green", "skyblue", "red", "black", "gray"],
        series: series,
        valueAxis: {
            labels: {
                format: "{0}"
            },
            line: {
                visible: false
            },
            axisCrossingValue: 0
        },
        categoryAxis: {
            categories: axis.length > 0 ? axis : ["ene", "feb", "mar", "abr", "may", "jun", "jul", "ago", "sep", "oct", "nov", "dic", "ene"],
            majorGridLines: {
                visible: false
            },
            labels: {
                rotation: "auto"
            }
        },
        tooltip: {
            visible: true,
            format: "{0}%",
            template: "#= series.name #: #= value #"
        }
    });
}

function inicializarGrafico(val, campaña) {
    if (val && val !== null && val !== undefined) {
        $(".contenedor-grafico-sin-resultados").remove();
        $(".contenedor-principal-campanas-grupo").empty();

        var campa = $("#campaña").val();
        if (campa == -1)
            campa = "0";

        if (campa != "0") {
            var campfiltr = resultDatos.Historial.camp.filter(function (x) { return x.Nombre == campa });

            var htmlg = "";

            if (campfiltr && campfiltr[0]) {
                htmlg += '<div class="contenedor-principal-campanas-titulo contenedor-principal-campanas-grano">'
                    + "Campaña " + campfiltr[0].Nombre;

                for (var ii in campfiltr[0].grano) {
                    (function (i) {
                        var ToneladasAux = campfiltr[0].grano[i].Total.toString().split(".");
                        if (ToneladasAux.length > 1) {
                            ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            ToneladasAux[1] = ToneladasAux[1].lenght > 0 ? ToneladasAux[1].substr(0, 2) : "";
                            ToneladasAux = ToneladasAux.join(",");
                        }
                        else {
                            ToneladasAux[0] = ToneladasAux[0].toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                            ToneladasAux = ToneladasAux.join("");
                        }

                        htmlg += '<div class="contenedor-principal-campanas-detalle">' +
                            '<div class="contenedor-principal-campanas-grano">' +
                            campfiltr[0].grano[i].Grano +
                            '</div>' +
                            '<div class="contenedor-principal-campanas-cantidad">' +
                            ToneladasAux +
                            '</div>' +
                            '</div>';
                    })(ii);
                }
                htmlg += '</div>';
            }
        } else {
            var campfiltr = resultDatos.Historial.camp.filter(function (x) { return x.Nombre != campa });

            var htmlg = "";

            if (campfiltr && campfiltr[0]) {
                for (var ii in campfiltr) {
                    (function (i) {
                        htmlg += '<div class="contenedor-principal-campanas-titulo contenedor-principal-campanas-grano">'
                            + "Campaña " + campfiltr[i].Nombre;

                        for (var rr in campfiltr[i].grano) {
                            (function (r) {
                                var ToneladasAux = campfiltr[i].grano[r].Total.split(".");
                                if (ToneladasAux.length > 1) {
                                    ToneladasAux[0] = ToneladasAux[0].replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                                    ToneladasAux[1] = ToneladasAux[1].lenght > 0 ? ToneladasAux[1].substr(0, 2) : "";
                                    ToneladasAux = ToneladasAux.join(",");
                                }
                                else {
                                    ToneladasAux[0] = ToneladasAux[0].replace(/\B(?=(\d{3})+(?!\d))/g, ".");
                                    ToneladasAux = ToneladasAux.join("");
                                }

                                htmlg += '<div class="contenedor-principal-campanas-detalle">' +
                                    '<div class="contenedor-principal-campanas-grano">' +
                                    campfiltr[i].grano[r].Grano +
                                    '</div>' +
                                    '<div class="contenedor-principal-campanas-cantidad">' +
                                    ToneladasAux +
                                    '</div>' +
                                    '</div>';
                            })(rr);
                        }

                        htmlg += "</div>";
                    })(ii);
                }
            }
        }

        $(".contenedor-principal-campanas-grupo").append(htmlg);

        createChart(val);
        $(document).bind("kendo:skinChange", createChart);
    } else {
        var html = "";
        html += '<div class="contenedor-grafico-sin-resultados">' +
            'No hay compras para el contacto' +
            '</div>';
        $(".campañagrafico").append(html);
    }
}

function modificarHeader() {
    $(".page-sidebar-menu").attr('style', 'display:none!important');
    $(".page-sidebar").attr('style', 'display:none!important');
    $(".page-content").css({
        'margin-left': 0
    })
    $(".atras-nav").show();

    $(".atras-nav").click(function () {
        window.location.href = window.location.origin;
    });
    $(".navbarsegundo-bread").html("Ficha Contacto");
}

function armarCamposEditables() {
    //armarEditable();
    //$("#detalle").hide();
    //$("#editor").show();
}

function armarDetalle() {
    $("#editor").hide();
    $("#detalle").show();
}

function armarContacto() {
    var datos = {
        ProveedorId: ProveedorId
    };
    var result = resultDatos = MSExecuteOnServer('/Proveedor/TraerProveedor', datos);
    var actividad = result.ActividadTraerPorProveedores;
    var actividadhistoria = result.ActividadHistoriaTraerPorProveedores;
    var basico = result.BasicoProveedorTraerPorProveedores;
    var campoacopio = result.CampoProduccionAcopioPorProveedores;
    var comerciales = result.ContactosComercialesTraerPorProveedores;
    var acopio = result.Acopio;
    var historial = result.Historial ? result.Historial.HistorialGrano : result.Historial;
    var objetivos = result.ObjetivosTraerPorProveedorId;
    var acopiomaterial = result.AcopioMaterialPorProveedores;
    var establecimiento = result.ProveedorCampoDetalle;

    armarSelectHeader(historial);

    $(".detalle-contacto-header-estado-span").html(basico[0].Estado);
    var estrellas = "";
    for (var i = 0; i < basico[0].Calificacion; i++) {
        estrellas += '<img src="../Content/Images/estrellacalificacion.png" />';
    }
    $(".detalle-contacto-header-estado-cantestrellas").append(estrellas);
    $(".contacto-detalle-basico-contenedor-razonsocial-span").html(basico[0].RazonSocial).attr('title', basico[0].RazonSocial);
    $(".contacto-detalle-basico-contenedor-cuit-span").html("(CUIT " + basico[0].CUIT + ")");
    if (basico[0].NoOperable) {
        $(".span-contacto-no-operable-tooltip").html(basico[0].TooltipNoOperable);
        $(".contacto-detalle-basico-contenedor-operable").show();
    } else {
        $(".contacto-detalle-basico-contenedor-operable").hide();
    }
    $(".contacto-detalle-basico-contenedor-datos-nomref").html(basico[0].NombreReferente ? "Ref: " + basico[0].NombreReferente : "Ref: -");
    $(".contacto-detalle-basico-contenedor-datos-tipo").html(basico[0].Segmentacion ? "Seg: " + basico[0].Segmentacion : "Seg: -");
    var basicoEmails = "Emails: <br>";
    if (basico[0].Email1) {
        basicoEmails += '<span><a target="_blank" href="mailto:' + basico[0].Email1 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + basico[0].Email1 + '</i></a></span>';
    }
    if (basico[0].Email2) {
        basicoEmails += ',<br><span><a target="_blank" href="mailto:' + basico[0].Email2 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + basico[0].Email2 + '</i></a></span>';
    }
    if (basico[0].Email3) {
        basicoEmails += ',<br><span><a target="_blank" href="mailto:' + basico[0].Email3 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + basico[0].Email3 + '</i></a></span>';
    }
    if (basico[0].Email4) {
        basicoEmails += ',<br><span><a target="_blank" href="mailto:' + basico[0].Email4 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + basico[0].Email4 + '</i></a></span>';
    }
    if (basicoEmails === "Emails: <br>") {
        basicoEmails = "Emails: -";
    }
    $(".contacto-detalle-basico-contenedor-datos-mail").html(basicoEmails);

    var basicoTelefonos = "Tels: <br>";
    if (basico[0].Telefono1) {
        basicoTelefonos += '<span><a target="_blank" href="tel:' + basico[0].Telefono1 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + basico[0].Telefono1 + '</i></a></span>';
    }
    if (basico[0].Telefono2) {
        basicoTelefonos += ',<br><span><a target="_blank" href="tel:' + basico[0].Telefono2 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + basico[0].Telefono2 + '</i></a></span>';
    }
    if (basico[0].Telefono3) {
        basicoTelefonos += ',<br><span><a target="_blank" href="tel:' + basico[0].Telefono3 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + basico[0].Telefono3 + '</i></a></span>';
    }
    if (basico[0].Telefono4) {
        basicoTelefonos += ',<br><span><a target="_blank" href="tel:' + basico[0].Telefono4 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + basico[0].Telefono4 + '</i></a></span>';
    }
    if (basicoTelefonos === "Tels: <br>") {
        basicoTelefonos = "Tels: -";
    }

    $(".contacto-detalle-basico-contenedor-datos-telefono").html(basicoTelefonos);
    $(".contacto-detalle-basico-contenedor-datos-comentarios").html(basico[0].Observaciones ? "Obs: " + basico[0].Observaciones : "Sin Observaciones");

    var contactocomercialprincipal = comerciales.filter(function (x) { return x.EsPrincipal });
    if (contactocomercialprincipal.length > 0) {
        $(".contacto-comercial-principal-nombre").html((contactocomercialprincipal[0].Nombres ? contactocomercialprincipal[0].Nombres + " " : "") + (contactocomercialprincipal[0].Apellido ? contactocomercialprincipal[0].Apellido : ""))
        $(".contacto-principal-cargo").html((contactocomercialprincipal[0].Cargo ? contactocomercialprincipal[0].Cargo + " - " : ""));
        $(".contacto-principal-puesto").html((contactocomercialprincipal[0].Puesto ? contactocomercialprincipal[0].Puesto : ""));
        var comercialTelefonos = "";
        if (contactocomercialprincipal[0].Telefono1) {
            comercialTelefonos += '<span><a target="_blank" href="tel:' + contactocomercialprincipal[0].Telefono1 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + contactocomercialprincipal[0].Telefono1 + '</i></a></span>';
        }
        if (contactocomercialprincipal[0].Telefono2) {
            comercialTelefonos += ',<br><span><a target="_blank" href="tel:' + contactocomercialprincipal[0].Telefono2 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + contactocomercialprincipal[0].Telefono2 + '</i></a></span>';
        }
        if (contactocomercialprincipal[0].Telefono3) {
            comercialTelefonos += ',<br><span><a target="_blank" href="tel:' + contactocomercialprincipal[0].Telefono3 + '">' + '<i class="fa fa-skype" aria-hidden="true">  ' + contactocomercialprincipal[0].Telefono3 + '</i></a></span>';
        }
        if (comercialTelefonos === "") {
            comercialTelefonos = "- Telefonos";
        }

        $(".contacto-principal-telefonos").html(comercialTelefonos);

        var comercialEmails = "";
        if (contactocomercialprincipal[0].Email1) {
            comercialEmails += '<span><a target="_blank" href="mailto:' + contactocomercialprincipal[0].Email1 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + contactocomercialprincipal[0].Email1 + '</i></a></span>';
        }
        if (contactocomercialprincipal[0].Email2) {
            comercialEmails += ',<br><span><a target="_blank" href="mailto:' + contactocomercialprincipal[0].Email2 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + contactocomercialprincipal[0].Email2 + '</i></a></span>';
        }
        if (contactocomercialprincipal[0].Email3) {
            comercialEmails += ',<br><span><a target="_blank" href="mailto:' + contactocomercialprincipal[0].Email3 + '">' + '<i class="fa fa-envelope-o" aria-hidden="true">  ' + contactocomercialprincipal[0].Email3 + '</i></a></span>';
        }

        if (comercialEmails === "") {
            comercialEmails = "Emails: -";
        }

        $(".contacto-principal-mails").html(comercialEmails);
        $(".contacto-principal-fechanacimiento").html((contactocomercialprincipal[0].FechaNacimiento ? kendo.toString(kendo.parseDate(contactocomercialprincipal[0].FechaNacimiento), "dd/MM/yy") : "No especifica"))

        $(".contacto-principal-intereses").html((contactocomercialprincipal[0].Interes ? contactocomercialprincipal[0].Interes.split(",").join("<br>") + "<br>" : "") + (contactocomercialprincipal[0].OtrosIntereses ? contactocomercialprincipal[0].OtrosIntereses : "No especifica"));
    }
    else {
        $(".contacto-comercial-principal").hide();
    }

    $(".contacto-detalle-moa-ultimo-contacto-span").html(basico[0].FechaUltimoContacto ? kendo.toString(kendo.parseDate(basico[0].FechaUltimoContacto), "m") : "-");
    $(".cliente-moa-val").html("<b>" + (basico[0].ClienteMOA ? "Si" : "No") + "</b>");
    $(".comecial-a-cargo").html((basico[0].Nombres !== "" ? basico[0].Nombres + ' ' : '') + (basico[0].Apellido !== "" ? basico[0].Apellido : ''));
    $(".grupo-de-compras").html(basico[0].GrupoDeCompras ? basico[0].GrupoDeCompras : "-");

    var domActividad = [];
    if (!basico[0].Direccion && !basico[0].CodigoPostal && !basico[0].Provincia && !basico[0].Localidad) {
        domActividad.push("-");
    } else {
        if (basico[0].Direccion) {
            domActividad.push(basico[0].Direccion + (basico[0].CodigoPostal ? " (CP" + basico[0].CodigoPostal + ")" : ""));
        }
        if (basico[0].Localidad) {
            domActividad.push(basico[0].Localidad);
        }
        if (basico[0].Provincia) {
            domActividad.push(basico[0].Provincia);
        }
    }

    $("#daco-domact").html(domActividad.join(", "));

    var canope = [];
    for (var ii in resultDatos.CanalesDeOperacion) {
        (function (i) {
            canope.push(resultDatos.CanalesDeOperacion[i].Descripcion);
        })(ii);
    }
    $("#daco-caope").html((canope.length > 0 ? canope.join("<br>") : "-"));

    var enta = [];
    for (var ii in resultDatos.ProveedorDestinatario) {
        (function (i) {
            enta.push(resultDatos.ProveedorDestinatario[i].Descripcion);
        })(ii);
    }
    $("#daco-enta").html((enta.length > 0 ? enta.join(", ") : "-"));

    var condpre = [];
    for (var ii in resultDatos.ProveedorCondicion) {
        (function (i) {
            condpre.push(resultDatos.ProveedorCondicion[i].Descripcion);
        })(ii);
    }

    $("#daco-condpre").html(condpre.length > 0 ? condpre.join("<br>") : "-");

    $("#daco-arin").html(basico[0].AreaInfluencia == null ? "-" : basico[0].AreaInfluencia);
    $("#daco-inte").html(basico[0].Intermediario == null ? "-" : basico[0].Intermediario);
    $("#daco-come").html(basico[0].Observaciones == null ? "-" : basico[0].Observaciones);

    $("#daco-clascomnet").html(basico[0].ClasificacionCompraNet == null ? "-" : basico[0].ClasificacionCompraNet);
    $("#daco-bolecomnet").html(basico[0].BoletoCompraNet == null ? "-" : basico[0].BoletoCompraNet);
    $("#daco-bolscomnet").html(basico[0].BolsaCompraNet == null ? "-" : basico[0].BolsaCompraNet);
    $("#daco-provcomnet").html((basico[0].ProvinciaCompraNet == null || basico[0].ProvinciaCompraNet == "") ? "-" : basico[0].ProvinciaCompraNet);
    $("#daco-loccomnet").html((basico[0].LocalidadCompraNet == null || basico[0].ProvinciaCompraNet == "") ? "-" : basico[0].LocalidadCompraNet);
    $("#daco-consigcomnet").html((basico[0].Consignatario == 1) ? "Si" : "No");
    $("#daco-planCanjecomnet").html((basico[0].PlanCanje == 1) ? "Si" : "No");
    $("#daco-comisioncomnet").html(basico[0].Comision);

    if ($("#daco-clascomnet").html() != 'Acopiador') {
        $("#rowConsignatario").hide();
        $("#rowPlanCanje").hide();
    }


    if (comerciales.length == 0) {
    } else {
        var htmlComerciales = '';
        for (var ii in comerciales) {
            (function (i) {
                var telComerciales = [];
                if (comerciales[i].Telefono1) {
                    telComerciales.push('<a href="tel: ' + comerciales[i].Telefono1 + '" target: "_blank"><i class="fa fa-skype" aria-hidden="true"> ' + comerciales[i].Telefono1 + '</i></a>');
                }
                if (comerciales[i].Telefono2) {
                    telComerciales.push('<a href="tel: ' + comerciales[i].Telefono2 + '" target: "_blank"><i class="fa fa-skype" aria-hidden="true"> ' + comerciales[i].Telefono2 + '</i></a>');
                }
                if (comerciales[i].Telefono3) {
                    telComerciales.push('<a href="tel: ' + comerciales[i].Telefono3 + '" target: "_blank"><i class="fa fa-skype" aria-hidden="true">  ' + comerciales[i].Telefono3 + '</i></a>');
                }

                var emailComerciales = [];
                if (comerciales[i].Email1) {
                    emailComerciales.push('<a href="mailto: ' + comerciales[i].Email1 + '" target: "_blank"><i class="fa fa-envelope-o" aria-hidden="true"> ' + comerciales[i].Email1 + '</i></a>');
                }
                if (comerciales[i].Email2) {
                    emailComerciales.push('<a href="mailto: ' + comerciales[i].Email2 + '" target: "_blank"><i class="fa fa-envelope-o" aria-hidden="true"> ' + comerciales[i].Email2 + '</i></a>');
                }
                if (comerciales[i].Email3) {
                    emailComerciales.push('<a href="mailto: ' + comerciales[i].Email3 + '" target: "_blank"><i class="fa fa-envelope-o" aria-hidden="true"> ' + comerciales[i].Email3 + '</i></a>');
                }

                htmlComerciales += '<div class="contenedor-contacto-comercial">' +
                    '<div class="contenedor-contacto-comercial-titulo">' +
                    '<img class="img-contacto-comercial" src="../Content/Images/contprinc-cont4.png" /> ' +
                    '<span class="span-contacto-comercial"> ' +
                    comerciales[i].Nombres + ' ' + comerciales[i].Apellido +
                    '</span>' +
                    '<span class="align-right">' +
                    /*'<img class="contacto-edit-img" src="../Content/Images/contacto-edit.png" /> ' +
                    '<span class="editar-contacto editar-contacto-comercial">' +
                    'Editar' +
                    '</span>' +*/
                    '</span>' +
                    '</div>' +
                    '<div class="contenedor-contacto-comercial-posicion">' +
                    '<span class="contenedor-contacto-comercial-posicion-izq">' +
                    (comerciales[i].Cargo ? comerciales[i].Cargo + ' - ' : "") +
                    '</span>' +
                    '<span class="contenedor-contacto-comercial-posicion-der">' +
                    (comerciales[i].Puesto ? comerciales[i].Puesto : "") +
                    '</span>' +
                    '</div>' +
                    '<div class="contenedor-contacto-comercial-telefonos">' +
                    (telComerciales.length > 0 ? telComerciales.join(" - ") : "") +
                    '</div>' +
                    '<div class="contenedor-contacto-comercial-mails">' +
                    (emailComerciales.length > 0 ? ' ' + emailComerciales.join(" - ") : "") +
                    (comerciales[i].CompraNet == true ? " &#10004;" : "") +
                    (comerciales[i].Cupo == true ? '<i class="fa fa-truck"></i>' : "") +
                    '</div>' +
                    '<div class="contenedor-contacto-comercial-extras">' +
                    '<div class="row">' +
                    '<div class="col-lg-6">' +
                    '<span class="contenedor-contacto-comercial-extras-label">' +
                    'Cumpleaños' +
                    '</span>' +
                    '</div>' +
                    '<div class="col-lg-6">' +
                    '<span class="contenedor-contacto-comercial-extras-value">' +
                    (comerciales[i].FechaNacimiento ? kendo.toString(kendo.parseDate(comerciales[i].FechaNacimiento), "m") : "No especifica") +
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
                    (comerciales[i].Interes ? comerciales[i].Interes.split(",").join("\n") + (comerciales[i].OtrosIntereses ? "\n" + comerciales[i].OtrosIntereses : "") : (comerciales[i].OtrosIntereses ? comerciales[i].OtrosIntereses : "No especifica")) +
                    '</span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';
            })(ii);
        }

        $(".contenedor-grupo-contactos-comerciales").append(htmlComerciales);
        $(".editar-contacto-comercial").click(function () {
            { ProveedorId: ProveedorId }
            editarProveedor(ProveedorId);
        });
    }

    $("#16-17").html("");
    $("#15-16").html("");
    $("#14-15").html("");
    $("#19-20").html("");
    $("#20-21").html("");

    if (establecimiento.length > 0) {
        armarEstablecimiento(establecimiento);
    } else {
        var html = "";
        var html = "";
        html += '<div class="contenedor-produccion-sin-resultados">' +
            'No se encontraron resultados de Establecimientos' +
            '</div>';
        $("#datos-establecimiento").append(html);
        $("#datos-establecimiento").hide();
    }

    if (campoacopio.length > 0) {
        //var grupocampoacopio = {};
        for (var ii in campoacopio) {
            (function (i) {
                grupocampoacopio[campoacopio[i].Campaña] = grupocampoacopio[campoacopio[i].Campaña] || {};
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id] = grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id] || {};
                //grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].push(campoacopio[i]);
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].ArrendadoPropio = campoacopio[i].ArrendadoPropio;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].EsCampoProduccion = campoacopio[i].EsCampoProduccion;

                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Id = campoacopio[i].Id;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Localidad = campoacopio[i].Localidad;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Provincia = campoacopio[i].Provincia;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Partido = campoacopio[i].Partido;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Nombre = campoacopio[i].Nombre;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Comercial = campoacopio[i].Comercial;

                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].KMZnombre = campoacopio[i].KMZnombre;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].KMZfile = campoacopio[i].KMZfile;

                //grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].AlmacCapacidadPropia = campoacopio[i].AlmacCapacidadPropia;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].AlmacHabilitadoSojaSust = campoacopio[i].AlmacHabilitadoSojaSust;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].AlmacHectSojaSust = campoacopio[i].AlmacHectSojaSust;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].AlmacTonsMaxSojaSust = campoacopio[i].AlmacTonsMaxSojaSust;
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].AlmacVolAnualTotal = campoacopio[i].AlmacVolAnualTotal;

                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Granos = grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Granos || [];
                grupocampoacopio[campoacopio[i].Campaña]["Campo" + campoacopio[i].Id].Granos.push({
                    HectareasPorcentaje: campoacopio[i].HectareasPorcentaje,
                    Material: campoacopio[i].Material,
                    MaterialId: campoacopio[i].MaterialId,
                    Toneladas: campoacopio[i].Toneladas,
                    Campaña: campoacopio[i].Campaña,
                    CampañaId: campoacopio[i].CampañaId
                });
            })(ii);
        }

        grupocampoacopio = [grupocampoacopio];

        var cont = 0;
        for (var ii in grupocampoacopio[0]) {
            (function (i) {
                switch (cont) {
                    case 0:
                        armarCampañaProduccion($("#16-17"), i, grupocampoacopio, $("#campana16-17"));
                        $("#datos-produccion .col-lg-12:first-of-type").show();
                        break;
                    case 1:
                        armarCampañaProduccion($("#15-16"), i, grupocampoacopio, $("#campana15-16"));
                        $("#datos-produccion .col-lg-12:first-of-type").show();
                        break;
                    case 2:
                        armarCampañaProduccion($("#14-15"), i, grupocampoacopio, $("#campana14-15"));
                        $("#datos-produccion .col-lg-12:first-of-type").show();
                        break;
                    case 3:
                        armarCampañaProduccion($("#19-20"), i, grupocampoacopio, $("#campana19-20"));
                        $("#datos-produccion .col-lg-12:first-of-type").show();
                        break;
                    case 4:
                        armarCampañaProduccion($("#20-21"), i, grupocampoacopio, $("#campana20-21"));
                        $("#datos-produccion .col-lg-12:first-of-type").show();
                        break;
                }
                cont++;
            })(ii);
        }

        campañaLength = Object.keys(grupocampoacopio[0]).length;
        if (campañaLength == 0) {
            $("#16-17").remove();
            $("#campana16-17").remove();
            $("#15-16").remove();
            $("#campana15-16").remove();
            $("#14-15").remove();
            $("#campana14-15").remove();
            $("#19-20").remove();
            $("#campana19-20").remove();
            $("#20-21").remove();
            $("#campana20-21").remove();
        } else if (campañaLength == 1) {
            $("#15-16").remove();
            $("#campana15-16").remove();
            $("#14-15").remove();
            $("#campana14-15").remove();
            $("#19-20").remove();
            $("#campana19-20").remove();
            $("#20-21").remove();
            $("#campana20-21").remove();
        } else if (campañaLength == 2) {
            $("#14-15").remove();
            $("#campana14-15").remove();
            $("#19-20").remove();
            $("#campana19-20").remove();
            $("#20-21").remove();
            $("#campana20-21").remove();
        } else if (campañaLength == 3) {
            $("#19-20").remove();
            $("#campana19-20").remove();
            $("#20-21").remove();
            $("#campana20-21").remove();
        } else if (campañaLength == 4) {
            $("#20-21").remove();
            $("#campana20-21").remove();
        }
    } else {
        var html = "";
        var html = "";
        html += '<div class="contenedor-produccion-sin-resultados">' +
            'No se encontraron resultados de Producción' +
            '</div>';
        $("#datos-produccion").append(html);
    }



    $("#a16-17").html("");
    $("#a15-16").html("");
    $("#a14-15").html("");
    $("#a19-20").html("");
    $("#a20-21").html("");

    if (acopio.length > 0) {
        //var grupoacopio = {};
        for (var ii in acopio) {
            (function (i) {
                grupoacopio[acopio[i].Campaña] = grupoacopio[acopio[i].Campaña] || {};
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id] = grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id] || [];
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].ArrendadoPropio = acopio[i].ArrendadoPropio;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].EsCampoProduccion = acopio[i].EsCampoProduccion;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Id = acopio[i].Id;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Localidad = acopio[i].Localidad;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Provincia = acopio[i].Provincia;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Partido = acopio[i].Partido;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Nombre = acopio[i].Nombre;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Comercial = acopio[i].Comercial;

                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].KMZnombre = acopio[i].KMZnombre;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].KMZfile = acopio[i].KMZfile;

                //grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].AlmacCapacidadPropia = acopio[i].AlmacCapacidadPropia;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].AlmacHabilitadoSojaSust = acopio[i].AlmacHabilitadoSojaSust;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].AlmacHectSojaSust = acopio[i].AlmacHectSojaSust;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].AlmacTonsMaxSojaSust = acopio[i].AlmacTonsMaxSojaSust;
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].AlmacVolAnualTotal = acopio[i].AlmacVolAnualTotal;

                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Granos = grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Granos || [];
                grupoacopio[acopio[i].Campaña]["Acopio" + acopio[i].Id].Granos.push({
                    //HectareasPorcentaje: acopio[i].HectareasPorcentaje,
                    Material: acopio[i].Material,
                    MaterialId: acopio[i].MaterialId,
                    Toneladas: acopio[i].Toneladas,
                    Campaña: acopio[i].Campaña,
                    CampañaId: acopio[i].CampañaId,
                    HasArrendadas: acopio[i].HasArrendadas
                });
            })(ii);
        }

        for (var ii in acopiomaterial) {
            (function (i) {
                grupoacopio[acopiomaterial[i].Campaña] = grupoacopio[acopiomaterial[i].Campaña] || {};
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId] = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId] || [];
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Localidad = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Localidad || acopiomaterial[i].Localidad;
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Provincia = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Provincia || acopiomaterial[i].Provincia;
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Partido = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Partido || acopiomaterial[i].Partido;
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Nombre = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].Nombre || acopiomaterial[i].Nombre;

                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].Id].KMZnombre = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].Id].KMZnombre || acopiomaterial[i].KMZnombre;
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].Id].KMZfile = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].Id].KMZfile || acopiomaterial[i].KMZfile;

                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento = grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento || [];
                grupoacopio[acopiomaterial[i].Campaña]["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento.push({
                    campañaId: acopiomaterial[i].CampañaId,
                    campaña: acopiomaterial[i].Campaña,
                    granoId: acopiomaterial[i].MaterialId,
                    grano: acopiomaterial[i].Material,
                    toneladasAlmacenamiento: acopiomaterial[i].Toneladas,
                    AcopioId: acopiomaterial[i].AcopioId,
                });
            })(ii);
        }

        grupoacopio = [grupoacopio];

        var acont = 0;
        for (var ii in grupoacopio[0]) {
            (function (i) {
                if (i != "null") {
                    switch (acont) {
                        case 0:
                            armarCampañaAlmacenamiento($("#a16-17"), i, grupoacopio, $("#acampana16-17"));
                            $("#datos-almacenamiento .col-lg-12:first-of-type").show();
                            break;
                        case 1:
                            armarCampañaAlmacenamiento($("#a15-16"), i, grupoacopio, $("#acampana15-16"));
                            $("#datos-almacenamiento .col-lg-12:first-of-type").show();
                            break;
                        case 2:
                            armarCampañaAlmacenamiento($("#a14-15"), i, grupoacopio, $("#acampana14-15"));
                            $("#datos-almacenamiento .col-lg-12:first-of-type").show();
                            break;
                        case 3:
                            armarCampañaAlmacenamiento($("#a19-20"), i, grupoacopio, $("#acampana19-20"));
                            $("#datos-almacenamiento .col-lg-12:first-of-type").show();
                            break;
                        case 4:
                            armarCampañaAlmacenamiento($("#a20-21"), i, grupoacopio, $("#acampana20-21"));
                            $("#datos-almacenamiento .col-lg-12:first-of-type").show();
                            break;
                    }
                    acont++;
                }
            })(ii);
        }

        acampañaLength = Object.keys(grupoacopio[0]).length;
        if (acampañaLength == 0) {
            $("#a16-17").remove();
            $("#acampana16-17").remove();
            $("#a15-16").remove();
            $("#acampana15-16").remove();
            $("#a14-15").remove();
            $("#acampana14-15").remove();
            $("#a19-20").remove();
            $("#acampana19-20").remove();
            $("#a20-21").remove();
            $("#acampana20-21").remove();
        } else if (acampañaLength == 1) {
            $("#a15-16").remove();
            $("#acampana15-16").remove();
            $("#a14-15").remove();
            $("#acampana14-15").remove();
            $("#a19-20").remove();
            $("#acampana19-20").remove();
            $("#a20-21").remove();
            $("#acampana20-21").remove();
        } else if (acampañaLength == 2) {
            $("#a14-15").remove();
            $("#acampana14-15").remove();
            $("#a19-20").remove();
            $("#acampana19-20").remove();
            $("#a20-21").remove();
            $("#acampana20-21").remove();
        } else if (acampañaLength == 3) {
            $("#a19-20").remove();
            $("#acampana19-20").remove();
            $("#a20-21").remove();
            $("#acampana20-21").remove();
        } else if (acampañaLength == 4) {
            $("#a20-21").remove();
            $("#acampana20-21").remove();
        }
    } else {
        var html = "";
        var html = "";
        html += '<div class="contenedor-produccion-sin-resultados">' +
            'No se encontraron resultados de Almacenamiento' +
            '</div>';
        $("#datos-almacenamiento").append(html);
    }

    var contAct = 0;
    var htmlProxAct = "";
    var htmlHist = "";

    if (!actividad.length) {
        htmlProxAct = "<div class='nohayrecordatorios'>No hay recordatorios registrados.</div>";
    } else {
        armarProxActividad(actividad, function (fhtmlHist, fhtmlProxAct) {
            htmlProxAct = fhtmlProxAct;
            //htmlHist = fhtmlHist;
        })
    }
    if (!actividadhistoria.length) {
        htmlHist = "<div class='nohayrecordatorios'>No hay actividades registradas.</div>";
    } else {
        armarActividad(actividadhistoria, function (fhtmlHist, fhtmlProxAct) {
            //htmlProxAct = fhtmlProxAct;
            htmlHist = fhtmlHist;
        });
    }

    $(".proximas-actividades-contenedor").append(htmlProxAct);
    $(".actividad-ultimo-contacto").html(basico[0].FechaUltimoContacto ? kendo.toString(kendo.parseDate(basico[0].FechaUltimoContacto), "dd/MM") : "-");
    $(".historial-actividad-grupo-contenedor").append(htmlHist);

    if (objetivos && objetivos.length) {
        var htmlCamp = '<div class="noCorredor"><div class="datos-contacto-titular noCorredor"> ' +
            '<span>Objetivos</span>' +
            '</div>' +
            '<div class="datos-produccion-cap-prod-editor-contenedor">' +
            '<div class="datos-produccion-cap-prod-editor-granos-contenedor sin-borde">' +
            '<div class="datos-produccion-cap-prod-editor-granos-titulos">' +
            '<div class="produccion-titulo-grano">Campaña</div>' +
            '<div class="produccion-titulo-grano">Grano</div>' +
            '<div class="produccion-titulo-grano">Objetivo</div>' +
            '</div>' +
            '<div class="datos-produccion-cap-prod-editor-granos-cantidades-contenedor">' +
            '<div class="datos-produccion-cap-prod-editor-granos-cantidades-grupo"></div>';

        for (var ii in objetivos) {
            (function (i) {
                htmlCamp += '<div class="noCorredor"><div class="lineaObjetivos campo-granos-objetivo">' +
                    '<div class="campo-input-text toneladasObjetivo"><span style="text-align:center;">' + objetivos[i].Campaña + '</span></div>' +
                    '<div class="campo-input-text toneladasObjetivo"><span style="text-align:center;">' + objetivos[i].Material + '</span></div>' +
                    '<div class="campo-input-text toneladasObjetivo"><span style="text-align:center;">' + (objetivos[i].ToneladasObjetivos ? objetivos[i].ToneladasObjetivos : "0") + ' TNs</span></div>' +
                    '</div></div>';
            })(ii);
        }
    } else {
        var htmlCamp = '<div class="noCorredor"><div class="datos-contacto-titular">' +
            '<span>Objetivos</span>' +
            '</div>' +
            '<div class="datos-produccion-cap-prod-editor-contenedor">' +
            '<div class="datos-produccion-cap-prod-editor-granos-contenedor sin-borde">' +
            '<div class="datos-produccion-cap-prod-editor-granos-titulos">' +
            '<div class="produccion-titulo-grano" style="width:100%!important;">No hay Objetivos cargados</div>'
        '</div>' +
            '<div class="datos-produccion-cap-prod-editor-granos-cantidades-contenedor">' +
            '<div class="datos-produccion-cap-prod-editor-granos-cantidades-grupo"></div>S';
    }

    htmlCamp += '</div>' +
        '</div>' +
        '</div>' +
        '</div>';
    $("#datos-contacto .wd65p").append(htmlCamp);

    if (MostrarAgenda) {
        setTimeout(function () {
            $("#agenda").trigger("click");
        }, 300);
    }
    if (basico[0].GrupoSegmentacion === "Corredores") {
        armarDetalleCorredor(datos);
    }
}

function armarCampañaProduccion(elem, i, grupocampoacopio, elem2) {
    elem.html("Campaña " + (i != "null" ? i : "sin especificar"));
    var htmlCamp = "";
    htmlCamp += '<div class="datos-contacto-titular">' +
        '<span>Campaña ' + (i != "null" ? i : "sin especificar") + '</span>' +
        '</div>' +
        '<div class="datos-contacto-subtitular">' +
        '<span>Capacidad productiva</span>' +
        /*'<a><img src="../Content/Images/contacto-edit.png" /> <span class="editar-contacto editar-contacto-cp"> Editar</span></a>' +*/
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Detalle Producción' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">';
    var entreCampo = 0;
    var hasCampo = 0;
    var hasPropias = 0;
    var hasArrendadas = 0;
    for (var jj in grupocampoacopio[0][i]) {
        (function (j) {
            htmlCamp += '<div class="value-contacto ' + j + '">' +
                '<div class="contenedor-campo-grupo">' +
                '<div class="row">';
            if (grupocampoacopio[0][i][j].Provincia && grupocampoacopio[0][i][j].Provincia != null) {
                htmlCamp += '<div class="contenedor-campo-grupo-ubicacion">' +
                    grupocampoacopio[0][i][j].Provincia + ", " + grupocampoacopio[0][i][j].Localidad + ", " + grupocampoacopio[0][i][j].Partido +
                    '</div>';
            }
            if (grupocampoacopio[0][i][j].Nombre && grupocampoacopio[0][i][j].Nombre != null && grupocampoacopio[0][i][j].Nombre != "") {
                htmlCamp += '<br><div class="contenedor-campo-grupo-ubicacion">' +
                    '<b>Nombre: </b>' + grupocampoacopio[0][i][j].Nombre +
                    '</div>';
            }
            if (grupocampoacopio[0][i][j].Comercial && grupocampoacopio[0][i][j].Comercial != null && grupocampoacopio[0][i][j].Comercial != "") {
                htmlCamp += '<br><div class="contenedor-campo-grupo-ubicacion">' +
                    '<b>Comercial: </b>' + grupocampoacopio[0][i][j].Comercial +
                    '</div>';
            }
            if (grupocampoacopio[0][i][j].KMZfile && grupocampoacopio[0][i][j].KMZfile != null) {
                var aux = grupocampoacopio[0][i][j].KMZnombre.split("\\").length - 1;
                var nomb = grupocampoacopio[0][i][j].KMZnombre.split("\\")[aux];

                htmlCamp += '<div class="contenedor-campo-grupo-kmz"><span onclick="descargarKMZ(this)" id="id_' + grupocampoacopio[0][i][j].Id + '_' + i + '">' +
                    nomb +
                    '</span></div>';
            }
            htmlCamp += '</div>' +
                '<div class="contenedor-campo-grupo-hectareas">' +
                ' ​​​​​​(Has ' + (grupocampoacopio[0][i][j].ArrendadoPropio ? "Propias" : "Arrendadas") + ')' +
                '</div>';

            htmlCamp += '<div class="contenedor-campo-grupo-granos-contenedor">';
            for (var ll in grupocampoacopio[0][i][j].Granos) {
                (function (l) {
                    hasCampo += grupocampoacopio[0][i][j].Granos[l].HectareasPorcentaje;
                    if (grupocampoacopio[0][i][j].ArrendadoPropio) {
                        hasPropias += grupocampoacopio[0][i][j].Granos[l].HectareasPorcentaje;
                    } else {
                        hasArrendadas += grupocampoacopio[0][i][j].Granos[l].HectareasPorcentaje;
                    }

                    htmlCamp += '<div class="contenedor-campo-grupo-granos-titulo">' +
                        (grupocampoacopio[0][i][j].Granos[l].Material ? grupocampoacopio[0][i][j].Granos[l].Material : "No especifica material") +
                        '</div>' +
                        '<div class="contenedor-campo-grupo-granos-hastns">' +
                        '<span>' + (grupocampoacopio[0][i][j].Granos[l].HectareasPorcentaje ? grupocampoacopio[0][i][j].Granos[l].HectareasPorcentaje : "No especifica ") + ' Has</span> <span>-</span> <span>' + (grupocampoacopio[0][i][j].Granos[l].Toneladas ? grupocampoacopio[0][i][j].Granos[l].Toneladas : "No especifica") + ' TNs</span>' +
                        '</div>';
                })(ll);
            }
            htmlCamp += '</div>' +
                '</div>' +
                '</div>';
        })(jj);
    }

    htmlCamp += '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +

        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Totales para siembra' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + hasCampo + '</b> Has' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Totales para siembra propio' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + hasPropias + '</b> Has' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Totales para siembra arrendado' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + hasArrendadas + '</b> Has' +
        '</div>' +
        '</div>' +
        '</div>' +
        /*
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Establecimiento propio / arrendado' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + hasCampo + '</b> Has' +
        '</div>' +
        '</div>' +
        '</div>' +
        */
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Tons. Máx. Aprobadas Soja Sustentable' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + (grupocampoacopio[0][i][Object.keys(grupocampoacopio[0][i])[0]].AlmacHectSojaSust ? grupocampoacopio[0][i][Object.keys(grupocampoacopio[0][i])[0]].AlmacHectSojaSust + " Has" : "-") + '</b> ' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Has. Aprobadas Soja Sustentable' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + (grupocampoacopio[0][i][Object.keys(grupocampoacopio[0][i])[0]].AlmacTonsMaxSojaSust ? grupocampoacopio[0][i][Object.keys(grupocampoacopio[0][i])[0]].AlmacTonsMaxSojaSust + " Has" : "-") + '</b>' +
        '</div>' +
        '</div>' +
        '</div>';
    /* +
        '<div class="linea-separador-produccion"></div>';*/
    elem2.append(htmlCamp);
}

function descargarKMZ(elem) {
    var id = $(elem).prop("id").split("_")[1];
    var campaña = $(elem).prop("id").split("_")[2];
    var val = grupocampoacopio[0][campaña]["Campo" + id];
    if (!val)
        val = grupoacopio[0][campaña]["Acopio" + id];    
    
    var file = val.KMZfile;
    var aux = val.KMZnombre.split("\\").length - 1;
    var nomb = val.KMZnombre.split("\\")[aux];
    download(nomb, file);
}
function descargarKMZEstablecimiento(elem) {
    var id = $(elem).prop("id").split("_")[1];
    var campaña = $(elem).prop("id").split("_")[2];
    var val = grupoestablecimiento[0]["Establecimiento" + id];

    var file = val.KMZfile;
    var aux = val.KMZnombre.split("\\").length - 1;
    var nomb = val.KMZnombre.split("\\")[aux];
    download(nomb, file);
}

function download(name, uri) {
    var link = document.createElement("a");
    link.download = name;
    link.href = uri;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    delete link;
}

function armarCampañaAlmacenamiento(elem, i, grupoacopio, elem2) {
    elem.html("Campaña " + (i != "null" ? i : "sin especificar"));
    var htmlCamp = "";
    htmlCamp += '<div class="datos-contacto-titular">' +
        '<span>Campaña ' + (i != "null" ? i : "sin especificar") + '</span>' +
        '</div>';
    htmlCamp += '<div class="datos-contacto-subtitular">' +
        '<span>Almacenamiento</span>' +
        /*'<a><img src="../Content/Images/contacto-edit.png" /> <span class="editar-contacto editar-contacto-alm"> Editar</span></a>' +*/
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Volumen Anual Total' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + (grupoacopio[0][i][Object.keys(grupoacopio[0][i])[0]].AlmacVolAnualTotal ? grupoacopio[0][i][Object.keys(grupoacopio[0][i])[0]].AlmacVolAnualTotal + " TNs" : "-") + '</b>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Habilitado para Soja Sustentable' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">' +
        '<div class="value-contacto">' +
        '<b>' + (grupoacopio[0][i][Object.keys(grupoacopio[0][i])[0]].AlmacHabilitadoSojaSust ? "Si" : "No") + '</b>' +
        '</div>' +
        '</div>' +
        '</div>' +

        '<div class="row">' +
        '<div class="col-lg-5">' +
        '<div class="label-contacto">' +
        'Detalle volumen (movimiento físico)' +
        '</div>' +
        '</div>' +
        '<div class="col-lg-7">';

    for (var jj in grupoacopio[0][i]) {
        (function (j) {
            htmlCamp += '<div class="value-contacto ' + j + '">' +
                '<div class="contenedor-campo-grupo">' +
                '<div class="row">';
            if (grupoacopio[0][i][j].Provincia && grupoacopio[0][i][j].Provincia != null) {
                htmlCamp += '<div class="contenedor-campo-grupo-ubicacion">' +
                    grupoacopio[0][i][j].Provincia + ", " + grupoacopio[0][i][j].Localidad + ", " + grupoacopio[0][i][j].Partido +
                    '</div>';
            }
            if (grupoacopio[0][i][j].Nombre && grupoacopio[0][i][j].Nombre != null && grupoacopio[0][i][j].Nombre != "") {
                htmlCamp += '<div class="contenedor-campo-grupo-ubicacion">' +
                    '<br><b>Nombre: </b>' + grupoacopio[0][i][j].Nombre +
                    '</div>';
            }
            if (grupoacopio[0][i][j].Comercial && grupoacopio[0][i][j].Comercial != null && grupoacopio[0][i][j].Comercial != "") {
                htmlCamp += '<div class="contenedor-campo-grupo-ubicacion">' +
                    '<br><b>Comercial: </b>' + grupoacopio[0][i][j].Comercial +
                    '</div>';
            }
            if (grupoacopio[0][i][j].KMZfile && grupoacopio[0][i][j].KMZfile != null) {
                var aux = grupoacopio[0][i][j].KMZnombre.split("\\").length - 1;
                var nomb = grupoacopio[0][i][j].KMZnombre.split("\\")[aux];

                htmlCamp += '<div class="contenedor-campo-grupo-kmz"><span onclick="descargarKMZ(this)" id="id_' + grupoacopio[0][i][j].Id + '_' + i + '">' +
                    nomb +
                    '</span></div>';
            }

            htmlCamp += '<div class="contenedor-campo-grupo-abm">' +
                /*'<a><span class="editar-contacto editar-contacto-cp">x Editar</span></a>' +
                '<a><img src="../Content/Images/contacto-edit.png" /> <span class="editar-contacto editar-contacto-cp"> Editar</span></a>' +*/
                '</div>' +
                '</div>';
            /*'<div class="contenedor-campo-grupo-hectareas">' +
            ' ​​​​​​(Has ' + (grupoacopio[0][i][j].ArrendadoPropio ? "Propias" : "Arrendadas") + ')' +
            '</div>'*/
            htmlCamp += '<div class="contenedor-campo-grupo-granos-contenedor">';

            for (var ll in grupoacopio[0][i][j].Granos) {
                (function (l) {
                    if (grupoacopio[0][i][j].Granos[l].Toneladas) {
                        htmlCamp += /*'<div class="contenedor-campo-grupo-granos-titulo">' +
                     grupoacopio[0][i][j].Granos[l].Material +
                     '</div>' +*/
                            '<div class="contenedor-campo-grupo-granos-hastns">' +
                            '<span>' + grupoacopio[0][i][j].Granos[l].Toneladas + ' TNs - </span>' + (grupoacopio[0][i][j].Granos[l].HasArrendadas == 1 ? "Alquiladas" : "Propias") + '<span>' +
                            '</div>';
                    }
                })(ll);
            }

            for (var ll in grupoacopio[0][i][j].GranosAlmacenamiento) {
                (function (l) {
                    htmlCamp += '<div class="granos-contenedor-grupo">'
                        + '<div class="granos-contenedor-titulo">'
                        + 'Campaña ' + (grupoacopio[0][i][j].GranosAlmacenamiento[l].campañaId != "null" ? grupoacopio[0][i][j].GranosAlmacenamiento[l].campaña : "no especifica") + ": " //+ obj.GranosAlmacenamiento[l].granoAlmacenamiento
                        + '</div>'
                        + '<div class="granos-contenedor-has-tns">'
                        + '<b>' + (grupoacopio[0][i][j].GranosAlmacenamiento[l].granoId ? grupoacopio[0][i][j].GranosAlmacenamiento[l].grano + " - " : "No especifica material - ") + (grupoacopio[0][i][j].GranosAlmacenamiento[l].toneladasAlmacenamiento ? grupoacopio[0][i][j].GranosAlmacenamiento[l].toneladasAlmacenamiento : "No especifica ") + "</b> Tns"
                        + '</div>'
                        + '</div>'
                })(ll);
            }

            htmlCamp += '</div>' +
                '</div>' +
                '</div>';
        })(jj);
    }
    htmlCamp += '</div>' +
        '</div>' +
        '</div>' +
        '</div>';
    elem2.append(htmlCamp);
}

function armarEstilosyFuncionesDetalle() {
    $('textarea').each(function () {
        $(this).val($(this).val().trim());
    }
    );

    $(".lista-contacto-no-operable").mouseover(function () {
        $(".lista-contacto-no-operable-tooltip").show();
        $(".lista-contacto-no-operable-tooltip-arrow").show();
    }).mouseout(function () {
        $(".lista-contacto-no-operable-tooltip").hide();
        $(".lista-contacto-no-operable-tooltip-arrow").hide();
    });

    $("body").click(function (e) {
        /*e.stopPropagation();
        e.preventDefault();*/
        if ($(".lista-contacto-no-operable-tooltip").is("visible")) {
            $(".lista-contacto-no-operable-tooltip").hide();
            $(".lista-contacto-no-operable-tooltip-arrow").hide();
        }
    });

    $(".contacto-detalle-basico-contenedor-operable").click(function (e) {
        /*e.stopPropagation();
        e.preventDefault();*/
        $(".lista-contacto-no-operable-tooltip").show();
        $(".lista-contacto-no-operable-tooltip-arrow").show();
    })

    $(".contacto-detalle-basico-editar").mouseover(function () {
        $(".contacto-detalle-basico-editar span").css({
            'text-decoration': 'underline'
        });
    }).mouseout(function () {
        $(".contacto-detalle-basico-editar span").css({
            'text-decoration': 'none'
        });
    }).click(function () {
        { ProveedorId: ProveedorId }
        editarProveedor(ProveedorId);
    });

    $(".editar-contacto-dc").mouseover(function () {
        $(".editar-contacto-dc").css({
            'text-decoration': 'underline'
        });
    }).mouseout(function () {
        $(".editar-contacto-dc").css({
            'text-decoration': 'none'
        });
    }).click(function () {
        { ProveedorId: ProveedorId }
        editarProveedor(ProveedorId);
    });
    $("#agenda").click(function () {
        $("#agenda").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#establecimiento").removeClass("whc-selected");
        $("#agenda").addClass("whc-selected");
        if ($("#datos-contacto").is(":visible")) {
            $("#datos-contacto").fadeOut("slow", function () {
                $("#datos-agenda").fadeIn("slow", function () { });
            });
        } else if ($("#datos-produccion").is(":visible")) {
            $("#datos-produccion").fadeOut("slow", function () {
                $("#datos-agenda").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento").is(":visible")) {
            $("#datos-almacenamiento").fadeOut("slow", function () {
                $("#datos-agenda").fadeIn("slow", function () { });
            });
        } else if ($("#datos-establecimiento").is(":visible")) {
            $("#datos-establecimiento").fadeOut("slow", function () {
                $("#datos-agenda").fadeIn("slow", function () { });
            });
        }
    });
    $("#contacto").click(function () {
        $("#agenda").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#establecimiento").removeClass("whc-selected");
        $("#proveedoresCorredor").removeClass("whc-selected");
        $("#contacto").addClass("whc-selected");
        if ($("#datos-produccion").is(":visible")) {
            $("#datos-produccion").fadeOut("slow", function () {
                $("#datos-contacto").fadeIn("slow", function () { });
            });
        } else if ($("#datos-agenda").is(":visible")) {
            $("#datos-agenda").fadeOut("slow", function () {
                $("#datos-contacto").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento").is(":visible")) {
            $("#datos-almacenamiento").fadeOut("slow", function () {
                $("#datos-contacto").fadeIn("slow", function () { });
            });
        } else if ($("#datos-proveedorescorredor").is(":visible")) {
            $("#datos-proveedorescorredor").fadeOut("slow", function () {
                $("#datos-contacto").fadeIn("slow", function () { });
            });
        } else if ($("#datos-establecimiento").is(":visible")) {
            $("#datos-establecimiento").fadeOut("slow", function () {
                $("#datos-contacto").fadeIn("slow", function () { });
            });
        }
    });
    $("#produccion").click(function () {
        $("#agenda").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#establecimiento").removeClass("whc-selected");
        $("#produccion").addClass("whc-selected");
        if ($("#datos-contacto").is(":visible")) {
            $("#datos-contacto").fadeOut("slow", function () {
                $("#datos-produccion").fadeIn("slow", function () { });
            });
        } else if ($("#datos-agenda").is(":visible")) {
            $("#datos-agenda").fadeOut("slow", function () {
                $("#datos-produccion").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento").is(":visible")) {
            $("#datos-almacenamiento").fadeOut("slow", function () {
                $("#datos-produccion").fadeIn("slow", function () { });
            });
        } else if ($("#datos-establecimiento").is(":visible")) {
            $("#datos-establecimiento").fadeOut("slow", function () {
                $("#datos-produccion").fadeIn("slow", function () { });
            });
        }
    });
    $("#almacenamiento").click(function () {
        $("#agenda").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#establecimiento").removeClass("whc-selected");
        $("#almacenamiento").addClass("whc-selected");
        if ($("#datos-contacto").is(":visible")) {
            $("#datos-contacto").fadeOut("slow", function () {
                $("#datos-almacenamiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-agenda").is(":visible")) {
            $("#datos-agenda").fadeOut("slow", function () {
                $("#datos-almacenamiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-produccion").is(":visible")) {
            $("#datos-produccion").fadeOut("slow", function () {
                $("#datos-almacenamiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-establecimiento").is(":visible")) {
            $("#datos-establecimiento").fadeOut("slow", function () {
                $("#datos-almacenamiento").fadeIn("slow", function () { });
            });
        }
    });
    $("#establecimiento").click(function () {
        $("#agenda").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#produccion").removeClass("whc-selected");
        $("#almacenamiento").removeClass("whc-selected");
        $("#establecimiento").addClass("whc-selected");
        if ($("#datos-contacto").is(":visible")) {
            $("#datos-contacto").fadeOut("slow", function () {
                $("#datos-establecimiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-agenda").is(":visible")) {
            $("#datos-agenda").fadeOut("slow", function () {
                $("#datos-establecimiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-produccion").is(":visible")) {
            $("#datos-produccion").fadeOut("slow", function () {
                $("#datos-establecimiento").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento").is(":visible")) {
            $("#datos-almacenamiento").fadeOut("slow", function () {
                $("#datos-establecimiento").fadeIn("slow", function () { });
            });
        }
    });

    $("#proveedoresCorredor").click(function () {
        $("#proveedoresCorredor").removeClass("whc-selected");
        $("#contacto").removeClass("whc-selected");
        $("#proveedoresCorredor").addClass("whc-selected");
        if ($("#datos-contacto").is(":visible")) {
            $("#datos-contacto").fadeOut("slow", function () {
                $("#datos-proveedorescorredor").fadeIn("slow", function () { });
            });
        }
    });

    $("#16-17").click(function () {
        $("#16-17").removeClass("whc-selected");
        $("#15-16").removeClass("whc-selected");
        $("#14-15").removeClass("whc-selected");
        $("#19-20").removeClass("whc-selected");
        $("#20-21").removeClass("whc-selected");
        $("#16-17").addClass("whc-selected");
        if ($("#campana15-16").is(":visible")) {
            $("#campana15-16").fadeOut("slow", function () {
                $("#campana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#campana14-15").is(":visible")) {
            $("#campana14-15").fadeOut("slow", function () {
                $("#campana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#campana19-20").is(":visible")) {
            $("#campana19-20").fadeOut("slow", function () {
                $("#campana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#campana20-21").is(":visible")) {
            $("#campana20-21").fadeOut("slow", function () {
                $("#campana16-17").fadeIn("slow", function () { });
            });
        }
    });
    $("#15-16").click(function () {
        $("#16-17").removeClass("whc-selected");
        $("#15-16").removeClass("whc-selected");
        $("#14-15").removeClass("whc-selected");
        $("#19-20").removeClass("whc-selected");
        $("#20-21").removeClass("whc-selected");
        $("#15-16").addClass("whc-selected");
        if ($("#campana16-17").is(":visible")) {
            $("#campana16-17").fadeOut("slow", function () {
                $("#campana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#campana14-15").is(":visible")) {
            $("#campana14-15").fadeOut("slow", function () {
                $("#campana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#campana19-20").is(":visible")) {
            $("#campana19-20").fadeOut("slow", function () {
                $("#campana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#campana20-21").is(":visible")) {
            $("#campana20-21").fadeOut("slow", function () {
                $("#campana15-16").fadeIn("slow", function () { });
            });
        }
    });
    $("#14-15").click(function () {
        $("#16-17").removeClass("whc-selected");
        $("#15-16").removeClass("whc-selected");
        $("#14-15").removeClass("whc-selected");
        $("#19-20").removeClass("whc-selected");
        $("#20-21").removeClass("whc-selected");
        $("#14-15").addClass("whc-selected");
        if ($("#campana16-17").is(":visible")) {
            $("#campana16-17").fadeOut("slow", function () {
                $("#campana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#campana15-16").is(":visible")) {
            $("#campana15-16").fadeOut("slow", function () {
                $("#campana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#campana20-21").is(":visible")) {
            $("#campana20-21").fadeOut("slow", function () {
                $("#campana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#campana20-21").is(":visible")) {
            $("#campana20-21").fadeOut("slow", function () {
                $("#campana14-15").fadeIn("slow", function () { });
            });
        }
    });
    $("#19-20").click(function () {
        $("#16-17").removeClass("whc-selected");
        $("#15-16").removeClass("whc-selected");
        $("#14-15").removeClass("whc-selected");
        $("#20-21").removeClass("whc-selected");
        $("#19-20").addClass("whc-selected");
        if ($("#campana16-17").is(":visible")) {
            $("#campana16-17").fadeOut("slow", function () {
                $("#campana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#campana15-16").is(":visible")) {
            $("#campana15-16").fadeOut("slow", function () {
                $("#campana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#campana14-15").is(":visible")) {
            $("#campana14-15").fadeOut("slow", function () {
                $("#campana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#campana20-21").is(":visible")) {
            $("#campana20-21").fadeOut("slow", function () {
                $("#campana19-20").fadeIn("slow", function () { });
            });
        }
    });

    $("#20-21").click(function () {
        $("#16-17").removeClass("whc-selected");
        $("#15-16").removeClass("whc-selected");
        $("#14-15").removeClass("whc-selected");
        $("#19-20").removeClass("whc-selected");
        $("#20-21").addClass("whc-selected");
        if ($("#campana16-17").is(":visible")) {
            $("#campana16-17").fadeOut("slow", function () {
                $("#campana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#campana15-16").is(":visible")) {
            $("#campana15-16").fadeOut("slow", function () {
                $("#campana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#campana14-15").is(":visible")) {
            $("#campana14-15").fadeOut("slow", function () {
                $("#campana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#campana19-20").is(":visible")) {
            $("#campana19-20").fadeOut("slow", function () {
                $("#campana20-21").fadeIn("slow", function () { });
            });
        }
    });


    $("#a16-17").click(function () {
        $("#a16-17").removeClass("whc-selected");
        $("#a15-16").removeClass("whc-selected");
        $("#a14-15").removeClass("whc-selected");
        $("#a19-20").removeClass("whc-selected");
        $("#a20-21").removeClass("whc-selected");
        $("#a16-17").addClass("whc-selected");
        if ($("#acampana15-16").is(":visible")) {
            $("#acampana15-16").fadeOut("slow", function () {
                $("#acampana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#acampana14-15").is(":visible")) {
            $("#acampana14-15").fadeOut("slow", function () {
                $("#acampana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#acampana19-20").is(":visible")) {
            $("#acampana19-20").fadeOut("slow", function () {
                $("#acampana16-17").fadeIn("slow", function () { });
            });
        } else if ($("#acampana20-21").is(":visible")) {
            $("#acampana20-21").fadeOut("slow", function () {
                $("#acampana16-17").fadeIn("slow", function () { });
            });
        }

    });
    $("#a15-16").click(function () {
        $("#a16-17").removeClass("whc-selected");
        $("#a15-16").removeClass("whc-selected");
        $("#a14-15").removeClass("whc-selected");
        $("#a19-20").removeClass("whc-selected");
        $("#a20-21").removeClass("whc-selected");
        $("#a15-16").addClass("whc-selected");
        if ($("#acampana16-17").is(":visible")) {
            $("#acampana16-17").fadeOut("slow", function () {
                $("#acampana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#acampana14-15").is(":visible")) {
            $("#acampana14-15").fadeOut("slow", function () {
                $("#acampana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#acampana19-20").is(":visible")) {
            $("#acampana19-20").fadeOut("slow", function () {
                $("#acampana15-16").fadeIn("slow", function () { });
            });
        } else if ($("#acampana20-21").is(":visible")) {
            $("#acampana20-21").fadeOut("slow", function () {
                $("#acampana15-16").fadeIn("slow", function () { });
            });
        }
    });
    $("#a14-15").click(function () {
        $("#a16-17").removeClass("whc-selected");
        $("#a15-16").removeClass("whc-selected");
        $("#a14-15").removeClass("whc-selected");
        $("#a19-20").removeClass("whc-selected");
        $("#a20-21").removeClass("whc-selected");
        $("#a14-15").addClass("whc-selected");
        if ($("#acampana16-17").is(":visible")) {
            $("#acampana16-17").fadeOut("slow", function () {
                $("#acampana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#acampana15-16").is(":visible")) {
            $("#acampana15-16").fadeOut("slow", function () {
                $("#acampana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#acampana19-20").is(":visible")) {
            $("#acampana19-20").fadeOut("slow", function () {
                $("#acampana14-15").fadeIn("slow", function () { });
            });
        } else if ($("#acampana20-21").is(":visible")) {
            $("#acampana20-21").fadeOut("slow", function () {
                $("#acampana14-15").fadeIn("slow", function () { });
            });
        }
    });

    $("#a19-20").click(function () {
        $("#a16-17").removeClass("whc-selected");
        $("#a15-16").removeClass("whc-selected");
        $("#a14-15").removeClass("whc-selected");
        $("#a19-20").removeClass("whc-selected");
        $("#a20-21").removeClass("whc-selected");
        $("#a19-20").addClass("whc-selected");
        if ($("#acampana16-17").is(":visible")) {
            $("#acampana16-17").fadeOut("slow", function () {
                $("#acampana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#acampana15-16").is(":visible")) {
            $("#acampana15-16").fadeOut("slow", function () {
                $("#acampana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#acampana14-15").is(":visible")) {
            $("#acampana14-15").fadeOut("slow", function () {
                $("#acampana19-20").fadeIn("slow", function () { });
            });
        } else if ($("#acampana20-21").is(":visible")) {
            $("#acampana20-21").fadeOut("slow", function () {
                $("#acampana19-20").fadeIn("slow", function () { });
            });
        }
    });

    $("#a20-21").click(function () {
        $("#a16-17").removeClass("whc-selected");
        $("#a15-16").removeClass("whc-selected");
        $("#a14-15").removeClass("whc-selected");
        $("#a19-20").removeClass("whc-selected");
        $("#a20-21").removeClass("whc-selected");
        $("#a20-21").addClass("whc-selected");
        if ($("#acampana16-17").is(":visible")) {
            $("#acampana16-17").fadeOut("slow", function () {
                $("#acampana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#acampana15-16").is(":visible")) {
            $("#acampana15-16").fadeOut("slow", function () {
                $("#acampana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#acampana14-15").is(":visible")) {
            $("#acampana14-15").fadeOut("slow", function () {
                $("#acampana20-21").fadeIn("slow", function () { });
            });
        } else if ($("#acampana19-20").is(":visible")) {
            $("#acampana19-20").fadeOut("slow", function () {
                $("#acampana20-21").fadeIn("slow", function () { });
            });
        }
    });
    $("#timepicker").kendoTimePicker({
        dateInput: true,
        value: new Date(),
        format: 'HH:mm'
    });
    $("#datepicker").kendoDatePicker({
        value: new Date(),
        min: new Date(),
        /*change: function () {
            var valor = this.value();
            if (valor <= new Date()) {
                $("#timepicker").data("kendoTimePicker").setOptions({
                    min: new Date(),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59),
                    format: 'HH:mm'
                });
            } else {
                $("#timepicker").data("kendoTimePicker").setOptions({
                    min: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    format: 'HH:mm'
                });
            }
        }*/
    });

    $("#timepicker-hasta").kendoTimePicker({
        dateInput: true,
        value: new Date(),
        format: 'HH:mm'
    });
    $("#datepicker-hasta").kendoDatePicker({
        value: new Date(),
        min: new Date(),
        /*change: function () {
            var valor = this.value();
            if (valor <= new Date()) {
                $("#timepicker-hasta").data("kendoTimePicker").setOptions({
                    min: new Date(),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59),
                    format: 'HH:mm'
                });
            } else {
                $("#timepicker-hasta").data("kendoTimePicker").setOptions({
                    min: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    format: 'HH:mm'
                });
            }
        }*/
    });

    $(".agenda-contenedor-recordatorio-span").click(function () {
        $(".agenda-contenedor-recordatorio-span-cancelar").show();
        $(".agenda-contenedor-recordatorio-fechayhora").show();
    });
    $(".agenda-contenedor-recordatorio-span-cancelar").click(function () {
        $(".agenda-contenedor-recordatorio-span-cancelar").hide();
        //$(".agenda-contenedor-recordatorio-fechayhora").hide();
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

    $(".guardar-agenda").click(function () {
        guardarActividad();
    });

    $(document).ready(function () {
        $(window).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });
    });

    $("#palabra-clave").change(function () {
        updateFiltro();
    });
}

function armarEstilosyFuncionesEditable() {
    $("#contacto-edit").click(function () {
        $("#almacenamiento-edit").removeClass("whc-selected");
        $("#contacto-edit").removeClass("whc-selected");
        $("#produccion-edit").removeClass("whc-selected");
        $("#contacto-edit").addClass("whc-selected");
        if ($("#datos-produccion-edit").is(":visible")) {
            $("#datos-produccion-edit").fadeOut("slow", function () {
                $("#datos-contacto-edit").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento-edit").is(":visible")) {
            $("#datos-almacenamiento-edit").fadeOut("slow", function () {
                $("#datos-contacto-edit").fadeIn("slow", function () { });
            });
        }
    });
    $("#produccion-edit").click(function () {
        $("#almacenamiento-edit").removeClass("whc-selected");
        $("#contacto-edit").removeClass("whc-selected");
        $("#produccion-edit").removeClass("whc-selected");
        $("#produccion-edit").addClass("whc-selected");
        if ($("#datos-contacto-edit").is(":visible")) {
            $("#datos-contacto-edit").fadeOut("slow", function () {
                $("#datos-produccion-edit").fadeIn("slow", function () { });
            });
        } else if ($("#datos-almacenamiento-edit").is(":visible")) {
            $("#datos-almacenamiento-edit").fadeOut("slow", function () {
                $("#datos-produccion-edit").fadeIn("slow", function () { });
            });
        }
    });
    $("#almacenamiento-edit").click(function () {
        $("#almacenamiento-edit").removeClass("whc-selected");
        $("#contacto-edit").removeClass("whc-selected");
        $("#produccion-edit").removeClass("whc-selected");
        $("#almacenamiento-edit").addClass("whc-selected");
        if ($("#datos-contacto-edit").is(":visible")) {
            $("#datos-contacto-edit").fadeOut("slow", function () {
                $("#datos-almacenamiento-edit").fadeIn("slow", function () { });
            });
        } else if ($("#datos-produccion-edit").is(":visible")) {
            $("#datos-produccion-edit").fadeOut("slow", function () {
                $("#datos-almacenamiento-edit").fadeIn("slow", function () { });
            });
        }
    });
    $("#agregarMail").click(function () {
        if (!($("#Email2") && $("#Email2").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<input type="text" class="campo-input-text" id="Email2" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail2" />' +
                '</div>';
            $(".emailinput").append(div);
            $("#eliminarMail2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Email3") && $("#Email3").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<input type="text" class="campo-input-text" id="Email3" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail3" />' +
                '</div>';
            $(".emailinput").append(div);
            $("#eliminarMail3").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Email4") && $("#Email4").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<input type="text" class="campo-input-text" id="Email4" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarMail4" />' +
                '</div>';
            $(".emailinput").append(div);
            $("#eliminarMail4").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 4 correos electrónicos.");
        }
    });
    $("#agregarTelefono").click(function () {
        if (!($("#Telefono2") && $("#Telefono2").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<select class="campo-input-select" id="TipoTelefono2">' +
                '<option value="1">Oficina</option>' +
                '<option value="2">Movil</option>' +
                '</select>' +
                '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono2" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono2" />' +
                '</div>';
            $(".telefonoinput").append(div);
            $("#eliminarTelefono2").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Telefono3") && $("#Telefono3").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<select class="campo-input-select" id="TipoTelefono3">' +
                '<option value="1">Oficina</option>' +
                '<option value="2">Movil</option>' +
                '</select>' +
                '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono3" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono3" />' +
                '</div>';
            $(".telefonoinput").append(div);
            $("#eliminarTelefono3").click(function () {
                $(this).parent().remove();
            });
        } else if (!($("#Telefono4") && $("#Telefono4").length > 0)) {
            var div = "";
            div += '<div class="formulario-campo">' +
                '<select class="campo-input-select" id="TipoTelefono4">' +
                '<option value="1">Oficina</option>' +
                '<option value="2">Movil</option>' +
                '</select>' +
                '<input type="text" placeholder="Telefono"  class="campo-input-text" id="Telefono4" style="margin-right:0px;" />' +
                '<img src="../Content/Images/eliminar-tel-mail.png" id="eliminarTelefono4" />' +
                '</div>';
            $(".telefonoinput").append(div);
            $("#eliminarTelefono4").click(function () {
                $(this).parent().remove();
            });
        } else {
            MensInfo("No se pueden agregar mas de 4 correos teléfonos.");
        }
    });
    $(".formulario-footer-cancelar").click(function () {
        armarDetalle();
    });
    $("#concom-intereses").multiselect({
        header: false
    });
    $("#calificacion").multiselect({
        header: false,
        multiple: false
    });

    $(".agregargrano").click(function () {
        if (!$("#grano" + cantGrano).val()) {
            MensErr("El grano debe estar seleccionado");
            return false;
        } else if (!$("#hectareas" + cantGrano).val()) {
            MensErr("Las hectareas deben estar cargadas");
            return false;
        } else if (!$("#toneladas" + cantGrano).val()) {
            MensErr("Las toneladas deben estar cargadas");
            return false;
        }

        cantGrano++;
        var html = "";
        html += '<div class="linea' + cantGrano + '">'
            + '<select class="campo-input-select campo-sin-span grano" id="grano' + cantGrano + '">'
            + '<option disabled selected value="">Seleccionar...</option>'
            + '<option value="1">Cebada</option>'
            + '<option value="2">Girasol</option>'
            + '<option value="3">Maiz</option>'
            + '<option value="4">Soja</option>'
            + '<option value="5">Trigo</option>'
            + '<option value="6">Otros</option>'
            + '</select>'
            + '<input type="text" class="campo-input-text hectareas" style="margin-left: 24px;margin-right: 4px;" id="hectareas' + cantGrano + '" />'
            + '<input type="text" class="campo-input-text toneladas" id="toneladas' + cantGrano + '" />'
            + '</div>';
        $(".linea" + (cantGrano - 1)).prepend(html);

        if (cantGrano > 0)
            for (var i = 0; i < cantGrano; i++) {
                var val = $("#grano" + i).val();
                $("#grano" + cantGrano + " option[value=" + val + "]").remove();
            }

        $("#eliminarGrano").show();
    });

    $("#eliminarGrano").click(function () {
        $(".linea" + cantGrano).remove();
        cantGrano--;
        if (cantGrano == 0)
            $("#eliminarGrano").hide();
    });

    $("#guardarCapProd").click(function () {
        var obj = {};

        obj.item = capProdCant;

        if ($("#provincia-produccion").val()) {
            obj.provincia = $("#provincia-produccion").val();
            obj.provinciaNom = $("#provincia-produccion option:selected").text();
        }

        if ($("#localidad-produccion").val()) {
            obj.localidad = $("#localidad-produccion").val();
            obj.localidadNom = $("#localidad-produccion option:selected").text();
        }

        if ($("#coordenadas").val())
            obj.coordenadas = $("#coordenadas").val();

        if ($("#hectareas-arrendadas").is(":checked")) {
            obj.hectareas = $("#hectareas-arrendadas").prop("value");
            obj.hectareasNom = $("label[for='hectareas-arrendadas']").text().trim()
        } else if ($("#hectareas-propias").is(":checked")) {
            obj.hectareas = $("#hectareas-propias").prop("value");
            obj.hectareasNom = $("label[for='hectareas-propias']").text().trim()
        }

        obj.granos = [];

        for (var i = 0; i < (cantGrano + 1); i++) {
            obj.granos.push({
                granoId: $("#grano" + i).val(),
                grano: $("#grano" + i).find('option:selected').text(),
                hectareas: $("#hectareas" + i).val(),
                toneladas: $("#toneladas" + i).val()
            });
        }

        var html = "";
        html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="granocontenedor' + capProdCant + '">'
            + '<div>'
            + '<div class="datos-produccion-cap-prod-guardados-zona">'
            + obj.provinciaNom + ", " + obj.localidadNom
            + '</div>'
            /*+ '<div class="editar-produccion" id="editarProd' + capProdCant + '">'
                + '<img src="../Content/Images/contacto-edit.png" /> Editar'
            + '</div>'
            + '<div class="eliminar-produccion"  onclick="eliminarCampoProduccion(this)" id="eliminarProd' + capProdCant + '">'
                + 'x Eliminar'
            + '</div>'*/
            + '</div>'
            + '<div>'
            + '<div class="datos-produccion-cap-prod-guardados-hectareas">'
            + '(Has ' + obj.hectareasNom + ')'
            + '</div>'
            + '<div class="granos-contenedor">';

        for (var jj in obj.granos) {
            (function (j) {
                html += '<div class="granos-contenedor-grupo">'
                    + '<div class="granos-contenedor-titulo">'
                    + obj.granos[j].grano
                    + '</div>'
                    + '<div class="granos-contenedor-has-tns">'
                    + '<b>' + obj.granos[j].hectareas + "</b> Has - <b>" + obj.granos[j].toneladas + "</b> TNs"
                    + '</div>'
                    + '</div>'
            })(jj);
        }

        html += '</div>'
            + '</div>';

        $(".datos-produccion-cap-prod-guardados").append(html);
        $(".datos-produccion-cap-prod-guardados").show();
        produccionCampoGuardar.push(obj);

        capProdCant++;
    });
}

function eliminarCampoProduccion(val) {
    var item = $(val).attr("id").split("eliminarProd")[1];
    $("#granocontenedor" + item).remove();
    produccionCampoGuardar = produccionCampoGuardar.filter(function (el) {
        return el.item !== parseInt(item);
    });
}

function guardarCambios() {
    var obj = {};
    obj.basico = {};
    obj.contacto = {};
    obj.contactoComercial = {};
    obj.produccion = {};
    obj.almacenamiento = {};
    obj.establecimiento = {};
    obj.cuit = $("#CUIT").val();
}

function armarEditable() {
    var actividad = resultDatos.ActividadTraerPorProveedores;
    var basico = resultDatos.BasicoProveedorTraerPorProveedores;
    var campoacopio = resultDatos.CampoProduccionAcopioPorProveedores;
    var comerciales = resultDatos.ContactosComercialesTraerPorProveedores;
    var contacto = resultDatos.DatosContacto;
    $("#CUIT").val(basico[0].CUIT);
    $("#RazonSocial").val(basico[0].RazonSocial);
    var idSegmentacion = $('#segmentacion option').filter(function () { return $(this).html() == basico[0].Segmentacion; }).val();

    $("#segmentacion").val([idSegmentacion]);
    $("#segmentacion").multiselect("refresh");
    $("#Email1").val(basico[0].Email1); //recorrer Emails
    $("#TipoTelefono1").val(basico[0].TipoTelefono1Id);
    $("#Telefono1").val(basico[0].Telefono1); //recorrer telefonos
    $("#calificacion").val(basico[0].Calificacion);

    //$("#concom-nombre").val(comerciales[0].Nombres);
    //$("#concom-apellido").val(comerciales[0].Apellido);
    //$("#concom-email1").val(comerciales[0].Email1);
    //$("#Telefono1").val(comerciales[0].Telefono1);
    //$("#concom-cargo").val(comerciales[0].Cargo);
    //$("#concom-puesto").val(comerciales[0].Puesto);
    //$("#concom-intereses").val(comerciales[0].Interes);
    //$("#concom-otrosintereses").val(comerciales[0].OtrosIntereses);

    $("#datcon-direccion").val(basico[0].Direccion);
    $("#datcon-codigopostal").val(basico[0].CodigoPostal);
    $("#datcon-canaloperacion").val(basico[0].CanalOperacion);
    $("#datcon-entregaa").val(basico[0].Destinatario);
    $("#Condicion").val(basico[0].Destinatario);
    $("#datcon-intermediario").val(basico[0].Intermediario);
    $("#datcon-comentarios").val(basico[0].Intermediario);
}

function InicializarDatos() {
    var resultDatos = MSExecuteOnServer('/Proveedor/Iniciliazar', { ProveedorId: ProveedorId });

    if (resultDatos != null) {
        armarSelectsCombos(resultDatos);
    }
}

function buscarLocalidad(val) {
    var data = { Id: val };
    var resultDatos = MSExecuteOnServer('/Proveedor/TraerLocalidad', data);
    $(".datcon-provincia").empty();
    var htmlLocalidad = "";
    htmlLocalidad += '<select multiple class="" id="datcon-localidad"';
    htmlLocalidad += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos) {
        (function (i) {
            htmlLocalidad += '<option value="' + resultDatos[i].LocalidadId + '">' + resultDatos[i].Nombre + '</option>';
        })(ii);
    }
    htmlLocalidad += '</select>';
    $(".datcon-provincia").append(htmlLocalidad);
    $("#datcon-localidad").multiselect({
        header: false,
        multiple: false,
        selectedList: 1
    })
}

function armarSelectsCombos(resultDatos) {
    var grupos = {};
    for (var jj in resultDatos.segm) {
        (function (j) {
            grupos[resultDatos.segm[j].Grupo] = grupos[resultDatos.segm[j].Grupo] || [];
            grupos[resultDatos.segm[j].Grupo].push({
                SegmentacionId: resultDatos.segm[j].SegmentacionId,
                Descripcion: resultDatos.segm[j].Descripcion
            });
        })(jj);
    }

    var htmlSegmentacion = "";
    htmlSegmentacion += '<select class="col-lg-7 segmentacioncombo campo-segmentacion" id="segmentacion">';
    var nulo = grupos[null];
    htmlSegmentacion += '<option value="null">Seleccione...</option>';
    //  htmlSegmentacion += '<option value="' + nulo[0].SegmentacionId + '">' + nulo[0].Descripcion + '</option>';
    for (var ii in grupos) {
        (function (i) {
            if (i != "null") {
                htmlSegmentacion += '<optgroup label=' + i + '>';
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
    htmlTipoTelefono += '<select class="campo-input-select" id="TipoTelefono1"';
    htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.tiptel) {
        (function (i) {
            htmlTipoTelefono += '<option value="' + resultDatos.tiptel[i].TipoTelefonoId + '">' + resultDatos.tiptel[i].Descripcion + '</option>';
        })(ii);
    }
    htmlTipoTelefono += '<input type="text" placeholder="Telefono" class="campo-input-text" id="Telefono1" />'
        + '<img src="../Content/Images/agregar-tel-mail.png" id="agregarTelefono" />'
        + '</select>';
    $(".telefonoinput").append(htmlTipoTelefono);

    var htmlProvincia = "";
    htmlProvincia += '<select class="" id="datcon-provincia"';
    //htmlProvincia += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.prov) {
        (function (i) {
            htmlProvincia += '<option value="' + resultDatos.prov[i].Provinciaid + '">' + resultDatos.prov[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvincia += '</select>';
    htmlProvincia += '<select class="" id="datcon-localidad"';
    //htmlProvincia += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.loc) {
        (function (i) {
            htmlProvincia += '<option value="' + resultDatos.loc[i].LocalidadId + '">' + resultDatos.loc[i].Nombre + '</option>';
        })(ii);
    }
    htmlProvincia += '</select>';
    htmlProvincia += '<input type="text" placeholder="direccion" id="datcon-direccion" />'
        + '<input type="text" placeholder="cod. postal" id="datcon-codigopostal" />';
    $(".campoprovincialocalidad").append(htmlProvincia);

    var htmlCanalOperacion = "";
    htmlCanalOperacion += '<select style="width: 258px!important" id="datcon-canaloperacion"';
    htmlCanalOperacion += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.cope) {
        (function (i) {
            htmlCanalOperacion += '<option value="' + resultDatos.cope[i].CanalOperacionId + '">' + resultDatos.cope[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCanalOperacion += '</select>';
    $(".campo-canaloperacion").append(htmlCanalOperacion);

    var htmlDestinatario = "";
    htmlDestinatario += '<select class="" style="width: 258px!important" id="datcon-entregaa"';
    htmlDestinatario += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.dest) {
        (function (i) {
            htmlDestinatario += '<option value="' + resultDatos.dest[i].DestinatarioId + '">' + resultDatos.dest[i].Descripcion + '</option>';
        })(ii);
    }
    htmlDestinatario += '</select>';
    $(".campo-entregaa").append(htmlDestinatario);

    var htmlCondicion = "";
    htmlCondicion += '<select class="campo-input-select" style="width: 258px!important" id="datcon-condpref"';
    htmlCondicion += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.cond) {
        (function (i) {
            htmlCondicion += '<option value="' + resultDatos.cond[i].CondicionId + '">' + resultDatos.cond[i].Descripcion + '</option>';
        })(ii);
    }
    htmlCondicion += '</select>';
    $(".campo-condpref").append(htmlCondicion);

    var htmlTipoTelefono = "";
    htmlTipoTelefono += '<select class="campo-input-select concom-campo-tiptelefono" id="concom-TipoTelefono1"';
    htmlTipoTelefono += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.tiptel) {
        (function (i) {
            htmlTipoTelefono += '<option value="' + resultDatos.tiptel[i].TipoTelefonoId + '">' + resultDatos.tiptel[i].Descripcion + '</option>';
        })(ii);
    }
    htmlTipoTelefono += '<input type="text" placeholder="Telefono" class="campo-input-text" id="Telefono1" />'
        + '<img src="../Content/Images/agregar-tel-mail.png" id="agregarTelefono" />'
        + '</select>';
    $(".concom-campo-telefono").append(htmlTipoTelefono);

    var htmlInteres = "";
    htmlInteres += '<select class="" id="concom-intereses"';
    //htmlInteres += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.inte) {
        (function (i) {
            htmlInteres += '<option value="' + resultDatos.inte[i].InteresId + '">' + resultDatos.inte[i].Descripcion + '</option>';
        })(ii);
    }
    htmlInteres += '</select>';
    $(".campo-intereses").append(htmlInteres);

    var htmlTipoActividad = "";
    htmlTipoActividad += '<select class"campo-tipoactividad" id="tipo-actividad">';
    htmlTipoActividad += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.tipoact) {
        (function (i) {
            htmlTipoActividad += '<option value="' + resultDatos.tipoact[i].TipoActividadId + '">' + resultDatos.tipoact[i].Descripcion + '</option>';
        })(ii);
    }
    htmlTipoActividad += '</select>';
    $(".campo-tipoactividad").append(htmlTipoActividad);

    $("#tipo-actividad").change(function () {
        $(".titulo-recordatorio").hide();
        $(".campoasunto").hide();
        $(".campofechadesde").hide();
        $(".campofechahasta").hide();
        $(".campoenviar").hide();
        var valor = $("#tipo-actividad").val();
        /*if (valor == extras.AgendaTareas) {
            $(".titulo-recordatorio").show();
            $(".campoasunto").show();
            $(".campoenviar").show();
        } else*/ if (valor == extras.AgendaCita) {
            $(".titulo-recordatorio").show();
            $(".campoasunto").show();
            $(".campofechadesde").show();
            $(".campofechahasta").show();
            $(".campoenviar").show();
        }
    });

    var htmlContactoComercial = "";
    htmlContactoComercial += '<select class"campo-tipoactividad" id="contacto-actividad">';
    htmlContactoComercial += '<option value = "null">Seleccione...</option>';
    for (var ii in resultDatos.concom) {
        (function (i) {
            htmlContactoComercial += '<option value="' + resultDatos.concom[i].ContactoComercialId + '">' + resultDatos.concom[i].Nombres + '</option>';
        })(ii);
    }
    htmlContactoComercial += '</select>';
    $(".campo-contactocomercial").append(htmlContactoComercial);

    $("#segmentacion").multiselect({
        header: false,
        multiple: false,
        selectedList: 1,
        noneSelectedText: "Seleccione...",
    });

    $("#datcon-canaloperacion").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Canal Operacion",
    });

    $("#datcon-entregaa").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Entrega A",
    });

    $("#datcon-condpref").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "CondicionPreferente",
    });

    $("#concom-intereses").multiselect({
        header: false,
        selectedList: 1,
        noneSelectedText: "Interes",
    });

    $("#datcon-provincia").change(function () {
        var valor = $("#datcon-provincia").val();
        buscarLocalidad(valor);
    });
}

$("#formulario-footer-guardar-contacto").click(function () {
    guardarProveedor();
});

function guardarProveedor() {
    var obj = {};

    if ($("#CUIT").val()) {
        obj.cuitBasico = $("#CUIT").val();
        obj.cuitNom = $("#CUIT option:selected").text();
    }

    if ($("#RazonSocial").val()) {
        obj.razonSocialBasico = $("#RazonSocial").val();
        obj.razonSocialNom = $("#RazonSocial option:selected").text();
    }

    if ($("#RazonSocial").val()) {
        obj.razonSocialBasico = $("#RazonSocial").val();
        obj.razonSocialNom = $("#RazonSocial option:selected").text();
    }

    if ($("#segmentacion").val()) {
        obj.segmentacionBasico = $("#segmentacion").val();
        obj.segmentacionNom = $("#segmentacion option:selected").text();
    }

    if ($("#Email1").val()) {
        obj.emailBasico = $("#Email1").val();
        obj.emailNom = $("#Email1 option:selected").text();
    }

    if ($("#TipoTelefono1").val()) {
        obj.tipotelefonoBasico = $("#TipoTelefono1").val();
        obj.tipotelefonoNom = $("#TipoTelefono1 option:selected").text();
    }

    if ($("#Telefono1").val()) {
        obj.telefonoBasico = $("#Telefono1").val();
        obj.telefonoNom = $("#Telefono1 option:selected").text();
    }

    if ($("#calificacion").val()) {
        obj.calificacionBasico = $("#calificacion").val();
        obj.calificacionNom = $("#calificacion option:selected").text();
    }

    aGuardarBasico.push(obj);
}

function editarActividad(elem) {
    var id = $(elem).prop("id").split("editar")[1];
    var obj = resultDatos.ActividadHistoriaTraerPorProveedores.filter(function (x) { return x.ActividadId == id });
    if (obj) {
        obj = obj[0];
    } else {
        return false;
    }

    $("#actividadId").val(obj.ActividadId);
    $("#fechaHoraActividad").val(obj.FechaHoraActividad);
    $("#tipo-actividad").val($('#tipo-actividad option').filter(function () { return $(this).html() == obj.TipoActividad; }).val());
    $("#detalle-actividad").val(obj.Detalle);
    $("#agendaAsunto").val(obj.asunto);

    if (obj.ContactoComercialId)
        $("#contacto-actividad").val(obj.ContactoComercialId);
    else
        $("#contacto-actividad").val("null");

    if (obj.TipoActividad == "Agenda") {
        $(".titulo-recordatorio").show();
        $(".campoasunto").show();
        $(".campofechadesde").show();
        $(".campofechahasta").show();
        $(".campoenviar").show();
    }

    if (obj.FechaHoraRecordatorio) {
        obj.FechaHoraRecordatorio = new Date(kendo.parseDate(obj.FechaHoraRecordatorio));
        $("#datepicker").data("kendoDatePicker").value(obj.FechaHoraRecordatorio);
        $("#timepicker").data("kendoTimePicker").value(obj.FechaHoraRecordatorio.getHours() + ":" + obj.FechaHoraRecordatorio.getMinutes());
    }
    if (obj.FechaHoraRecordatorioFin) {
        obj.FechaHoraRecordatorioFin = new Date(kendo.parseDate(obj.FechaHoraRecordatorioFin));
        $("#datepicker-hasta").data("kendoDatePicker").value(obj.FechaHoraRecordatorioFin);
        $("#timepicker-hasta").data("kendoTimePicker").value(obj.FechaHoraRecordatorioFin.getHours() + ":" + obj.FechaHoraRecordatorioFin.getMinutes());
    }
}

function eliminarActividad(elem) {
    var id = $(elem).prop("id").split("eliminar")[1];
    var result = MSExecuteOnServer('/Proveedor/EliminarRecordatorio', { id: id });

    $(elem).parent().parent().remove();
}

function armarActividad(actividad, ret) {
    var contAct = 0;
    var htmlHist = "";
    var htmlProxAct = "";
    for (var ii in actividad) {
        (function (i) {
            htmlHist += '<div class="historial-actividad-grupo-detalle">' +
                '<div class="historial-actividad-grupo-detalle-titulo">' +
                actividad[i].TipoActividad +
                '</div>';

            htmlHist += '<div class="historial-actividad-grupo-detalle-abm">' +
                '<img src="../Content/Images/eliminar-agenda.png" onclick="eliminarActividad(this)" id="eliminar' + actividad[i].ActividadId + '" />' +
                '<img src="../Content/Images/contacto-edit.png" onclick="editarActividad(this)" id="editar' + actividad[i].ActividadId + '" />' +
                '</div>';

            if (actividad[i].ContactoComercial) {
                htmlHist += '<div class="historial-contactocomercial">' +
                    actividad[i].ContactoComercial +
                    '</div>';
            }

            htmlHist += '<div class="historial-actividad-grupo-detalle-comentario">' +
                '<img src="../Content/Images/mensaje-agenda.png" /> <span>' + actividad[i].Detalle + '</span>' +
                '</div>' +
                '<div class="historial-actividad-grupo-detalle-fecha">' +
                kendo.toString(kendo.parseDate(actividad[i].FechaHoraActividad), "ddd dd") + " de " + kendo.toString(kendo.parseDate(actividad[i].FechaHoraActividad), "MMMM HH:mm") +
                '</div>';
            if (actividad[i].FechaHoraRecordatorio) {
                htmlHist += '<div class="historial-actividad-grupo-detalle-recordatorio">' +
                    '<img src="../Content/Images/notificaciones-reloj.png" /><span>Recordatorio: ' + kendo.toString(kendo.parseDate(actividad[i].FechaHoraRecordatorio), "ddd dd") + " de " + kendo.toString(kendo.parseDate(actividad[i].FechaHoraRecordatorio), "MMMM HH:mm") + '</span>' +
                    '</div>';
            }
            htmlHist += '</div>';
            contAct++;
        })(ii);
    }

    ret(htmlHist, htmlProxAct);
}

function armarProxActividad(actividad, ret) {
    var htmlProxAct = "";
    var contAct = 0;
    var htmlHist = "";
    for (var ii in actividad) {
        (function (i) {
            actividad[i].FechaHoraActividad = new Date(new Date(kendo.parseDate(actividad[i].FechaHoraActividad)).setMonth(new Date(kendo.parseDate(actividad[i].FechaHoraActividad)).getMonth()));
            htmlProxAct += '<div class="proximas-actividades-detalle">' +
                '<div class="proximas-actividades-detalle-horario">' +
                '<img src="../Content/Images/notificaciones-reloj.png" />' +
                '<span class="proximas-actividades-horario">' + kendo.toString(kendo.parseDate(actividad[i].FechaHoraActividad), "ddd dd") + " de " + kendo.toString(kendo.parseDate(actividad[i].FechaHoraActividad), "MMMM HH:mm") + '</span>' +
                '</div>' +
                '<div class="proximas-actividades-titulo">' +
                actividad[i].TipoActividad +
                '</div>' +
                '<div class="proximas-contactocomercial">' +
                actividad[i].ContactoComercial +
                '</div>' +
                '<div class="proximas-actividades-observacion">' +
                actividad[i].Detalle +
                '</div>' +
                '</div>';
            if (actividad[i].FechaHoraRecordatorio) {
                htmlProxAct += '<div class="historial-actividad-grupo-detalle-recordatorio">' +
                    '<img src="../Content/Images/notificaciones-reloj.png" /><span>Recordatorio: ' + kendo.toString(kendo.parseDate(actividad[i].FechaHoraRecordatorio), "ddd dd") + " de " + kendo.toString(kendo.parseDate(actividad[i].FechaHoraRecordatorio), "MMMM HH:mm") + '</span>' +
                    '</div>';
            }
            htmlProxAct += '</div>';
            contAct++;
        })(ii);
    }
    ret(htmlHist, htmlProxAct);
}

function guardarActividad() {
    var obj = {};
    if ((!$("#contacto-actividad").val() || $("#contacto-actividad").val() === "null")) {
        MensErr("La actividad debe tener un contacto");
        return false;
    }

    obj.ActividadId = $("#actividadId").val();
    obj.fechaActividad = $("#fechaHoraActividad").val();

    obj.tipoactividad = $("#tipo-actividad").val();
    obj.detalle = $("#detalle-actividad").val().trim();
    obj.contacto = $("#contacto-actividad").val();
    obj.enviar = $("#enviarActividad").is(":checked") ? 1 : 0;
    obj.ProveedorId = ProveedorId;
    //if ($(".agenda-contenedor-recordatorio-fechayhora").is(":visible")) {
    obj.fecha = $("#datepicker").val();
    obj.hora = $("#timepicker").val();
    if (obj.fecha && obj.hora) {
        var fechaAux = obj.fecha.split("/");
        var horaAux = obj.hora.split(":");
        if (obj.tipoactividad == "3")
            obj.fechaYHoraRecordatorio = new Date(fechaAux[2], (fechaAux[1] - 1), fechaAux[0], horaAux[0], horaAux[1]);
        else
            obj.fechaYHoraRecordatorio = null;
    }
    else {
        obj.fecha = null;
        obj.hora = null;
        obj.fechaYHoraRecordatorio = null;
    }

    /*if ($(".agenda-contenedor-recordatorio-fechayhora").is(":visible")) {*/
    obj.fechaFin = $("#datepicker-hasta").val();
    obj.horaFin = $("#timepicker-hasta").val();
    if (obj.fechaFin && obj.horaFin) {
        var fechaAux = obj.fechaFin.split("/");
        var horaAux = obj.horaFin.split(":");
        if (obj.tipoactividad == "3")
            obj.fechaYHoraRecordatorioFin = new Date(fechaAux[2], (fechaAux[1] - 1), fechaAux[0], horaAux[0], horaAux[1]);
        else
            obj.fechaYHoraRecordatorioFin = null;
    } else {
        obj.fechaFin = null;
        obj.horaFin = null;
        obj.fechaYHoraRecordatorioFin = null;
    }

    obj.asunto = $("#agendaAsunto").val();

    if (obj.fechaYHoraRecordatorio && obj.fechaYHoraRecordatorioFin) {
        if (+obj.fechaYHoraRecordatorioFin < +obj.fechaYHoraRecordatorio) {
            MensErr("La fecha de fin de recordatorio no puede ser mayor a la fecha de inicio");
            return false;
        }
    }

    var resultActividad = MSExecuteOnServer('/Proveedor/CrearActividad', obj);

    $("#actividadId").val(null);
    $("#fechaHoraActividad").val(null);
    $("#tipo-actividad").val(null);
    $("#detalle-actividad").val("");
    $("#contacto-actividad").val("");
    $("#enviarActividad").prop('checked', false);
    //$(".agenda-contenedor-recordatorio-fechayhora").hide();
    $(".agenda-contenedor-recordatorio-span-cancelar").hide();
    $("#agendaAsunto").val("")

    $("#timepicker").kendoTimePicker({
        dateInput: true,
        value: new Date(),
        format: 'HH:mm'
    });
    $("#datepicker").kendoDatePicker({
        value: new Date(),
        min: new Date(),
        /*change: function () {
            var valor = this.value();
            if (valor <= new Date()) {
                $("#timepicker").data("kendoTimePicker").setOptions({
                    min: new Date(),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59),
                    format: 'HH:mm'
                });
            } else {
                $("#timepicker").data("kendoTimePicker").setOptions({
                    min: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    format: 'HH:mm'
                });
            }
        }*/
    });

    $("#timepicker-hasta").kendoTimePicker({
        dateInput: true,
        value: new Date(),
        format: 'HH:mm'
    });
    $("#datepicker-hasta").kendoDatePicker({
        value: new Date(),
        min: new Date(),
        /*change: function () {
            var valor = this.value();
            if (valor <= new Date()) {
                $("#timepicker-hasta").data("kendoTimePicker").setOptions({
                    min: new Date(),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 23, 59, 59),
                    format: 'HH:mm'
                });
            } else {
                $("#timepicker-hasta").data("kendoTimePicker").setOptions({
                    min: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    max: new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 0, 0, 0),
                    format: 'HH:mm'
                });
            }
        }*/
    });

    var result = MSExecuteOnServer('/Proveedor/TraerProveedor', { ProveedorId: ProveedorId });
    var actividad = result.ActividadTraerPorProveedores;
    var basico = result.BasicoProveedorTraerPorProveedores;
    var actividadhistoria = result.ActividadHistoriaTraerPorProveedores;

    $(".proximas-actividades-contenedor").empty();
    $(".historial-actividad-grupo-contenedor").empty();

    var contAct = 0;
    var htmlProxAct = "";
    var htmlHist = "";

    if (!actividad.length) {
        htmlProxAct = "<div class='nohayrecordatorios'>No hay recordatorios registrados.</div>";
    } else {
        armarProxActividad(actividad, function (fhtmlHist, fhtmlProxAct) {
            htmlProxAct = fhtmlProxAct;
            //htmlHist = fhtmlHist;
        })
    }
    if (!actividadhistoria.length) {
        htmlHist = "<div class='nohayrecordatorios'>No hay actividades registradas.</div>";
    } else {
        armarActividad(actividadhistoria, function (fhtmlHist, fhtmlProxAct) {
            //htmlProxAct = fhtmlProxAct;
            htmlHist = fhtmlHist;
        });
    }

    $(".proximas-actividades-contenedor").append(htmlProxAct);
    $(".actividad-ultimo-contacto").html(basico[0].FechaUltimoContacto ? kendo.toString(kendo.parseDate(basico[0].FechaUltimoContacto), "dd/MM") : "-");
    $(".historial-actividad-grupo-contenedor").append(htmlHist);
    ArmarNotificaciones();
}

function setChangeChecks() {
    $('form :input').change(function () {
        updateFiltro();
    });
    $("#tipo-act-Reunion-personal").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Todas").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Llamada").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Evento-Molinos").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Mail-chat").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Cupones").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Notas").change(function () {
        updateFiltro();
    });
    $("#tipo-act-Cierre-de-operaciones").change(function () {
        updateFiltro();
    });
}

function updateFiltro() {
    filtro = {};

    if ($("#palabra-clave").val() && $("#palabra-clave").val() != "null")
        filtro.detalle = $("#palabra-clave").val();

    /*if ($("#periodo-tiempo").val() && $("#periodo-tiempo").val() != "null")
        filtro.Segmentacion = $("#periodo-tiempo").val();
    */

    if ($("#tipo-act-Todas").is(":checked") ||
        $("#tipo-act-Llamada").is(":checked") ||
        $("#tipo-act-Reunion-personal").is(":checked") ||
        $("#tipo-act-Evento-Molinos").is(":checked") ||
        $("#tipo-act-Mail-chat").is(":checked") ||
        $("#tipo-act-Cierre-de-operaciones").is(":checked") ||
        $("#tipo-act-Cupones").is(":checked") ||
        $("#tipo-act-Notas").is(":checked")
    ) {
        if ($("#tipo-act-Todas").is(":checked")) {
            $("#tipo-act-Llamada").prop("checked", false);
            $("#tipo-act-Reunion-personal").prop("checked", false);
            $("#tipo-act-Evento-Molinos").prop("checked", false);
            $("#tipo-act-Mail-chat").prop("checked", false);
            $("#tipo-act-Cierre-de-operaciones").prop("checked", false);
            $("#tipo-act-Cupones").prop("checked", false);
            $("#tipo-act-Notas").prop("checked", false);
        }

        filtro.Actividad = [];
        $("#tipo-act-Todas").is(":checked") && filtro.Actividad.push($("#tipo-act-Todas").prop("value"));
        $("#tipo-act-Llamada").is(":checked") && filtro.Actividad.push($("#tipo-act-Llamada").prop("value"));
        $("#tipo-act-Reunion-personal").is(":checked") && filtro.Actividad.push($("#tipo-act-Reunion-personal").prop("value"));
        $("#tipo-act-Evento-Molinos").is(":checked") && filtro.Actividad.push($("#tipo-act-Evento-Molinos").prop("value"));
        $("#tipo-act-Mail-chat").is(":checked") && filtro.Actividad.push($("#tipo-act-Mail-chat").prop("value"));
        $("#tipo-act-Cierre-de-operaciones").is(":checked") && filtro.Actividad.push($("#tipo-act-Cierre-de-operaciones").prop("value"));
        $("#tipo-act-Cupones").is(":checked") && filtro.Actividad.push($("#tipo-act-Cupones").prop("value"));
        $("#tipo-act-Notas").is(":checked") && filtro.Actividad.push($("#tipo-act-Notas").prop("value"));
        filtro.TipoActividadId = filtro.Actividad.join("|");
        if (filtro.Actividad === "null")
            filtro.TipoActividadId = null;
    }

    filtro.ProveedorId = ProveedorId;
    var result = resultDatos = MSExecuteOnServer('/Proveedor/TraerFiltros', filtro);

    $(".historial-actividad-grupo-contenedor").empty();
    var htmlHist = "";
    armarActividad(result.ActividadHistoriaTraerPorProveedores, function (fhtmlHist, fhtmlProxAct) {
        //htmlProxAct = fhtmlProxAct;
        htmlHist = fhtmlHist;
    });
    $(".historial-actividad-grupo-contenedor").append(htmlHist);
}

function editarProveedor(ProveedorId) {
    window.location.href = window.location.origin + "/Proveedor/Agregar?ProveedorId=" + ProveedorId;
}

function armarSelectHeader(granos) {
    var aux = {};
    for (var ii in granos) {
        (function (i) {
            aux[granos[i].Nombre] = aux[granos[i].Nombre] || {};
            for (var jj in granos[i].campañas) {
                (function (j) {
                    aux[granos[i].Nombre][granos[i].campañas[j].Nombre] = aux[granos[i].Nombre][granos[i].campañas[j].Nombre] || {};
                    aux[granos[i].Nombre][granos[i].campañas[j].Nombre].Campañas = granos[i].campañas[j].Campañas;
                })(jj);
            }
        })(ii);
    }

    var granosArr = Object.keys(aux);

    if (granos && granos.length > 0) {
        var htmlGranos = "";
        htmlGranos += '<select id="grano" class="detalle-contacto-header-campana-filtro-select">';
        //htmlGranos += '<option value="null">- Elegir Grano</option>';
        for (var ii in granosArr) {
            (function (i) {
                htmlGranos += '<option value="' + granosArr[i] + '">' + granosArr[i] + '</option>';
            })(ii);
        }
        htmlGranos += "</select>";
        htmlGranos += '<select id="campaña" class="detalle-contacto-header-campana-filtro-select">' +
            //'<option value="null">- Elegir Campaña</option>' +
            '</select>'

        $(".detalle-contacto-header-campana-filtro").append(htmlGranos);
        $("#grano").change(function () {
            actualizarCampaña(aux[$("#grano").val()]);
        });

        $("#grano").trigger("change");
    } else {
        var htmlGranos = "";
        htmlGranos += '<select id="grano" class="detalle-contacto-header-campana-filtro-select">';
        htmlGranos += '<option value="null">- Elegir Grano</option>';
        htmlGranos += "</select>";
        htmlGranos += '<select id="campaña" class="detalle-contacto-header-campana-filtro-select">' +
            '<option value="null">- Elegir Campaña</option>' +
            '</select>'

        $(".detalle-contacto-header-campana-filtro").append(htmlGranos);

        var html = "";
        html += '<div class="contenedor-grafico-sin-resultados">' +
            'No hay compras para el contacto' +
            '</div>';
        $(".campañagrafico").append(html);
    }
}

function actualizarCampaña(val) {
    if (val && val !== "null" && val !== null && val !== undefined) {
        $("#campaña").find('option').remove();
        /*$('#campaña').append($('<option>', {
            value: "null",
            text: "- Elegir Campaña"
        }));*/
        $('#campaña').append($('<option>', {
            value: "-1",
            text: "Últimas tres campañas"
        }));
        for (var ii in val) {
            (function (i) {
                $('#campaña').append($('<option>', {
                    value: i,
                    text: i
                }));
            })(ii);
        }
        $("#campaña").change(function () {
            if ($("#campaña").val() && $("#campaña").val() !== "null") {
                if ($("#campaña").val() != "-1") {
                    inicializarGrafico(val[$("#campaña").val()], $("#campaña").val());
                } else {
                    campaniaSeleccionada = val;
                    inicializarGrafico(val);
                }
            }
        });

        $("#campaña").trigger("change");
    } else {
        $("#campaña").find('option').remove();
        $('#campaña').append($('<option>', {
            value: "null",
            text: "- Elegir Campaña"
        }));
    }
}

function ImprimirReporte() {
    var oParam = {
        "ProveedorId": ProveedorId,
    }
    var result = MSExecuteOnServer('/Proveedor/ImprimirReporteProveedor', oParam);

    if (result != null) {
        if (result.DownloadKey.length > 0) {
            var url = MSGetUrl('/DownLoad/Reporte?key=' + result.DownloadKey);
            window.location = url;
        }
    }
}

function ExportarPdf() {
    var oParam = {
        "ProveedorId": ProveedorId,
    }
    var result = MSExecuteOnServer('/Proveedor/ImprimirReporteProveedor', oParam);

    if (result != null) {
        if (result.DownloadKey.length > 0) {
            var url = MSGetUrl('/DownLoad/Reporte?key=' + result.DownloadKey);
            window.location = url;
        }
    }
}

function armarEstablecimiento(establecimiento) {
   

    for (var ii in establecimiento) {
        (function (i) {
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId] = grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId] || {};

            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].CampoId = establecimiento[i].CampoId;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].Localidad = establecimiento[i].localidadNom;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].Provincia = establecimiento[i].provinciaNom;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].Partido = establecimiento[i].Partido;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].LocalidadId = establecimiento[i].localidad;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].ProvinciaId = establecimiento[i].provincia;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].latitud = establecimiento[i].latitud;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].longitud = establecimiento[i].longitud;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].nombre = establecimiento[i].nombre;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].comercialId = establecimiento[i].comercialId;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].comercialNom = establecimiento[i].comercialNom;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].KMZnombre = establecimiento[i].archivo;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].KMZfile = establecimiento[i].archivoFileResult;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].rinde = establecimiento[i].rinde;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].htotales = establecimiento[i].htotales;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].hcultivables = establecimiento[i].hcultivables;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].materialId = establecimiento[i].materialId;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].materialNom = establecimiento[i].materialNom;
            grupoestablecimiento["Establecimiento" + establecimiento[i].CampoId].partidoNom = establecimiento[i].partidoNom;



        })(ii);
    }


    grupoestablecimiento = [grupoestablecimiento];
    var capProdCantEstablecimiento = 0;
    for (ii in grupoestablecimiento[0]) {
        (function (i) {
            var obj = {};

            obj.item = capProdCantEstablecimiento;

            obj.localidad = grupoestablecimiento[0][i].LocalidadId;
            obj.localidadNom = grupoestablecimiento[0][i].Localidad + "(" + grupoestablecimiento[0][i].Provincia + ")";
            obj.partido = grupoestablecimiento[0][i].Partido;


            obj.archivo = grupoestablecimiento[0][i].KMZnombre;

            obj.archivoFileResult = grupoestablecimiento[0][i].KMZfile;

            obj.latitud = grupoestablecimiento[0][i].latitud;
            obj.longitud = grupoestablecimiento[0][i].longitud;
            obj.nombre = grupoestablecimiento[0][i].nombre;
            obj.comercialId = grupoestablecimiento[0][i].comercialId;
            obj.comercialNom = grupoestablecimiento[0][i].comercialNom;
            obj.partido = grupoestablecimiento[0][i].partidoNom;

            obj.CampoId = grupoestablecimiento[0][i].CampoId;

            obj.materialNom = grupoestablecimiento[0][i].materialNom;
            obj.materialId = grupoestablecimiento[0][i].materialId;
            obj.rinde = grupoestablecimiento[0][i].rinde;
            obj.htotales = grupoestablecimiento[0][i].htotales;
            obj.hcultivables = grupoestablecimiento[0][i].hcultivables;
            var file = "";
            if (obj.archivoFileResult && obj.archivoFileResult != null) {
                var aux = obj.archivo.split("\\").length - 1;
                var nomb = obj.archivo.split("\\")[aux];
                file = '<div class="contenedor-campo-grupo-kmz"><span onclick="descargarKMZEstablecimiento(this)" id="id_' + obj.CampoId + '_' + i + '">' + nomb + '</span></div>';
            }
            var html = "";
            html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="establecimientocontenedor' + capProdCantEstablecimiento + '">'
                + '<div>'
                + (obj.nombre != "" ? ('<div class="datos-produccion-cap-prod-guardados-zona"><b>Nombre</b>: ' + obj.nombre.toUpperCase() + '</div><br>') : "")
                + '<div class="datos-produccion-cap-prod-guardados-zona">'
                + obj.localidadNom + " - " + obj.partido
                + (obj.latitud != "" && obj.longitud != "" ? ('<span class="datos-produccion-cap-prod-guardados-hectareas"> &nbsp;&nbsp;&nbsp;&nbsp; <b>Latitud</b>:' + obj.latitud + " &nbsp;&nbsp;&nbsp;&nbsp;<b>Longitud</b>: " + obj.longitud + '</span>') : "")
                + '</div>'
                + file
                + '</div>'
                + '<div>'
                + '<div class="datos-produccion-cap-prod-guardados-hectareas">'
                
                + (obj.comercialId > 0 ? ' <b>Comercial</b>:' + obj.comercialNom : "")
                //+ "<br>"
                + (obj.materialId > 0 ? '&nbsp;&nbsp;&nbsp;&nbsp; <b>Material</b>:' + obj.materialNom : "")
                + (obj.rinde > 0 ? '&nbsp;&nbsp;&nbsp;&nbsp; <b>Rinde</b>:' +obj.rinde : "")
                + (obj.htotales > 0 ? '&nbsp;&nbsp;&nbsp;&nbsp; <b>Has Totales</b>:' + obj.htotales : "")
                + (obj.hcultivables > 0 ? '&nbsp;&nbsp;&nbsp;&nbsp; <b>Has Cultivables</b>:' +obj.hcultivables : "")
                + '</div>'
                + '<div class="granos-contenedor">';

            html += '</div>'
                + '</div>';

            $("#datos-establecimiento").append(html);
            $("#datos-establecimiento").hide();
            capProdCantEstablecimiento++;
        })(ii);
    }
}