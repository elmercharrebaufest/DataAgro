var SegmentacionMultiple = [];

$(document).ready(function () {
    google.charts.load('current', {
        'packages': ['geochart', 'corechart', 'gauge'],
        'mapsApiKey': 'AIzaSyD-9tSrke72PouQMnMX-a7eZSW0jkFMBWY'
    });
    inicializarTabs();
    inicializarTabsGrafico();
    inicializarElementos();
    cargarCombos();
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $(".page-content").prop("style", "padding-left:0!important;padding-right:0!important;");
    }
});

function inicializarTabs() {
    var tabs = $("#tabs-container").children();
    for (var i = 0; i < tabs.length; i++) {
        $(tabs[i])
            .click(function () {
                $("#sindatos").remove();
                LimpiarFiltros();
                if ($(".filtro").is(":visible"))
                    $(".filtro").animate({ width: 'toggle' }, 400);
                $("#graficoR").empty();
                $("#graficoGR").empty().hide();
                $(".botones").slideUp();
                $(".grafico").slideUp();
                for (var j = 0; j < tabs.length; j++) {
                    $(tabs[j]).removeClass("whc-selected");
                }
                $(this).addClass("whc-selected");
                var graficos = $(".grafico .wizard-header-contenedor-grupo").children();
                setTimeout(function () {
                    for (var j = 0; j < graficos.length; j++) {
                        $(graficos[j]).removeClass("whcsub-selected").hide();
                    }
                }, 400);
                var id = $(this).prop("id");
                switch (id) {
                    case "compras":
                        setTimeout(function () {
                            $("#mapa").show();
                            $("#torta").show();
                            $("#barras").show();
                            $(".grafico").slideDown();
                        }, 600);
                        break;
                    case "capacidadproductiva":
                    case "capacidadacopio":
                        setTimeout(function () {
                            $("#mapa").show();
                            $("#barras").show();
                            $(".grafico").slideDown();
                        }, 600);
                        break;
                    case "objetivoscomercial":
                        setTimeout(function () {
                            $("#gaudge").show();
                            $(".grafico").slideDown();
                        }, 600);
                        break;
                    case "basededatos":
                        setTimeout(function () {
                            $("#torta").show();
                            $(".grafico").slideDown();
                        }, 600);
                        break;
                    default:
                        break;
                }
            });
    }
}

function inicializarTabsGrafico() {
    var graficos = $(".grafico .wizard-header-contenedor-grupo").children();
    for (var i = 0; i < graficos.length; i++) {
        $(graficos[i])
            .click(function () {
                LimpiarFiltros();
                $("#sindatos").remove();
                $("#graficoR").empty();
                $(".botones").slideUp();
                if ($(".filtro").is(":visible"))
                    $(".filtro").animate({ width: 'toggle' }, 400);
                for (var j = 0; j < graficos.length; j++) {
                    $(graficos[j]).removeClass("whcsub-selected");
                }
                $(this).addClass("whcsub-selected");
                armarFiltro();
            });
    }
}

function LimpiarFiltros() {
    $("#txtdesde").val("");
    $("#txthasta").val("");
    $("#txtgrano").val("null");
    $("#txtcosecha").val("null");
    $("#txtprovincia").val("null");
    $("#txtsegmentacion").val("0");
    $("#txtcomercial").val("null");
    $("#txtmes").val("null");
    $("#txttoneladas").val("null");
    SegmentacionMultiple = [];
    SegmentacionMultiple.push([0]);
    $("#txtsegmentacion:selected").prop("selected", false);
    $("#txtsegmentacion").multiselect('refresh');
}

function inicializarElementos() {
    kendo.culture("es-AR");

    $("#compras").click();

    $("#txtdesde").kendoDatePicker();
    $("#txthasta").kendoDatePicker();

    $("#btn-filtrar").click(function () {
        $("#sindatos").remove();

        var filtro = getFiltros();

        if (filtro.Indicadores == "objetivoscomercial" && filtro.Grafico == "gaudge") {
            if (filtro.Cosecha == "null") {
                MensErr("La cosecha debe estar definida");
                return false;
            }
        }

        if (filtro.Indicadores == "basededatos" && filtro.Grafico == "torta") {
            if (!filtro.FechaDesde || !filtro.FechaHasta) {
                MensErr("La fecha debe estar definida");
                return false;
            }

            o = {};
            o.fechaDesde = $("#txtdesde").val().split('/');
            o.fechaHasta = $("#txthasta").val().split('/');

            if (+new Date(o.fechaDesde[2], o.fechaDesde[1], o.fechaDesde[0]) > +new Date(o.fechaHasta[2], o.fechaHasta[1], o.fechaHasta[0])) {
                MensErr("La fecha Desde no debe ser mayor a la fecha Hasta");
                return false;
            }
        }

        funcReturn = function (datos) {
            if ((filtro.Indicadores == "basededatos" && (datos.valoresGrilla.length > 0)) || (filtro.Indicadores != "basededatos" && datos.length > 0)) {
                switch (filtro.Indicadores) {
                    case "compras":
                        switch (filtro.Grafico) {
                            case "mapa":
                                var arrData = [];
                                arrData.push(["Ciudad", "TNs", "CUITS"]);
                                var objAux = datos.concat();
                                objAux = objAux.filter(function (x) { return x.Provincia != null });
                                var capfed = objAux.filter(function (x) { return x.Provincia == "CAPITAL FEDERAL" });
                                if (capfed.length == 0) {
                                    capfed = {
                                        Cuit: 0,
                                        Estado: null,
                                        Grano: 3,
                                        Provincia: "BUENOS AIRES",
                                        Segmentacion: null,
                                        Tonelada: 0
                                    };
                                } else {
                                    capfed = capfed[0];
                                }
                                objAux = objAux.filter(function (x) { return x.Provincia != "CAPITAL FEDERAL" });
                                var bsas = objAux.filter(function (x) { return x.Provincia == "BUENOS AIRES" });
                                if (bsas.length == 0) {
                                    bsas = {
                                        Cuit: 0,
                                        Estado: null,
                                        Grano: 3,
                                        Provincia: "BUENOS AIRES",
                                        Segmentacion: null,
                                        Tonelada: 0
                                    };
                                } else {
                                    bsas = bsas[0];
                                }
                                objAux = objAux.filter(function (x) { return x.Provincia != "BUENOS AIRES" });
                                var bsasall = {
                                    Cuit: capfed.Cuit + bsas.Cuit,
                                    Estado: null,
                                    Grano: 3,
                                    Provincia: "BUENOS AIRES",
                                    Segmentacion: null,
                                    Tonelada: capfed.Tonelada + bsas.Tonelada
                                };
                                if (bsas.Tonelada != 0 || capfed.Tonelada != 0)
                                    objAux.push(bsasall);

                                for (var ii in objAux) {
                                    (function (i) {
                                        arrData.push([objAux[i].Provincia, objAux[i].Tonelada, objAux[i].Cuit]);
                                    })(ii);
                                }
                                armarGraficoMapa(arrData);
                                break;
                            case "torta":
                                var arrData = [];
                                //arrData.push(["Segmentación", "Clientes"]);
                                //arrData.push(["Segmentación", "983","Tooltip"]);
                                var objAux = datos.concat();
                                objAux = objAux.filter(function (x) { return x.Provincia != null });
                                var total = 0;

                                //Saco el total de cuits
                                for (var ii in objAux) {
                                    (function (i) {
                                        total += objAux[i].Tonelada;
                                    })(ii);
                                }
                                var valor = [];
                                var sumaValor = 0;
                                var MayorValor = 0;

                                //Verifico si da el 100% por redondeo
                                for (var ii in objAux) {
                                    (function (i) {
                                        sumaValor += parseFloat(((objAux[i].Tonelada / total) * 100).toFixed(2));
                                        if (objAux[i].Tonelada > MayorValor) {
                                            MayorValor = objAux[i].Tonelada;
                                        }
                                    })(ii);
                                }

                                if (sumaValor == 100) {
                                    for (var ii in objAux) {
                                        (function (i) {
                                            //valor.push([((objAux[i].Cuit * 100)/total).toFixed(2)]);
                                            var mensaje = "<strong>" + objAux[i].Provincia + "</strong></br> <strong>Tn:</strong> " + objAux[i].Tonelada;
                                            mensaje += " (" + ((objAux[i].Tonelada * 100) / total).toFixed(2) + "%)";

                                            var segmentacion = objAux[i].Provincia + ' - ' + ((objAux[i].Tonelada * 100) / total).toFixed(2) + ' % - Tn.: ' + objAux[i].Tonelada.toLocaleString() + ' CUITs : ' + objAux[i].Cuit;

                                            arrData.push([segmentacion, objAux[i].Cuit, mensaje]);
                                        })(ii);
                                    }
                                }
                                else {
                                    var sumarizado = 0;
                                    for (var ii in objAux) {
                                        (function (i) {
                                            //valor.push([((objAux[i].Cuit * 100)/total).toFixed(2)]);
                                            var segmentacion = '';
                                            if ((MayorValor == objAux[i].Cuit) && (sumarizado == 0)) {
                                                var mensaje = "<strong>" + objAux[i].Provincia + "</strong></br> <strong>Tn:</strong> " + objAux[i].Tonelada;
                                                mensaje += " (" + (((objAux[i].Tonelada * 100) / total) + parseFloat(100 - sumaValor)).toFixed(2) + " %)";
                                                segmentacion = objAux[i].Provincia + ' - ' + (((objAux[i].Tonelada * 100) / total) + parseFloat(100 - sumaValor)).toFixed(2) + ' % - Tn.: ' + objAux[i].Tonelada.toLocaleString() + ' CUITs : ' + objAux[i].Cuit;
                                                sumarizado = 1;
                                            }
                                            else {
                                                var mensaje = "<strong>" + objAux[i].Provincia + "</strong></br> <strong>Tn:</strong> " + objAux[i].Tonelada;
                                                mensaje += " (" + ((objAux[i].Tonelada * 100) / total).toFixed(2) + "%)";
                                                segmentacion = objAux[i].Provincia + ' - ' + ((objAux[i].Tonelada * 100) / total).toFixed(2) + ' % - Tn.: ' + objAux[i].Tonelada.toLocaleString() + ' CUITs : ' + objAux[i].Cuit;
                                            }

                                            arrData.push([segmentacion, objAux[i].Tonelada, mensaje]);

                                            //arrData.push([objAux[i].Provincia, objAux[i].Cuit, mensaje]);
                                        })(ii);
                                    }
                                }

                                /*for (var ii in objAux) {
                                    (function (i) {
                                        arrData.push([objAux[i].Provincia, objAux[i].Cuit,"Cantidad de Cuit 111 (25%)"]);
                                    })(ii);
                                }*/

                                armarGraficoTorta(arrData, "Segmentación de Proveedores por compras");
                                break;
                            case "barras":

                                if (!datos[0].MasCl100 && !datos[0].MasCl1000 && !datos[0].MasCl5000 && !datos[0].MasCl10000 && !datos[0].MasCl20000 && !datos[0].MasCl40000 && !datos[0].MasTn100 && !datos[0].MasTn1000 && !datos[0].MasTn5000 && !datos[0].MasTn10000 && !datos[0].MasTn20000 && !datos[0].MasTn40000 && !datos[0].MenosCl10000 && !datos[0].MenosTn10000) {
                                    $("#graficoGR").empty();
                                    $("#graficoGR").css("display", "none");
                                    $("#graficoR").empty();
                                    var graficoid = $("#graficoR");
                                    var sd = $("<div id='sindatos'>").appendTo(graficoid).css({
                                        padding: 25,
                                        'font-size': 20,
                                        'text-align': 'center',
                                        backgroundColor: '#f0f0f0',
                                        'margin-top': 30
                                    });
                                    $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                        width: 30,
                                        height: 30,
                                        'vertical-align': 'top',
                                        backgroundColor: '#f0f0f0'
                                    });
                                    $("<span>").appendTo(sd).html("No se encontraron resultados").css({
                                        'vertical-align': 'top',
                                        'padding-left': 10,
                                        'padding-right': 10
                                    });
                                    $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                        width: 30,
                                        height: 30,
                                        backgroundColor: '#f0f0f0',
                                        'vertical-align': 'top'
                                    });
                                }
                                else {
                                    armarGraficoBarrasCompras(datos);
                                }
                                break;
                        }
                        break;
                    case "capacidadproductiva":
                    case "capacidadacopio":
                        switch (filtro.Grafico) {
                            case "mapa":
                                var arrData = [];
                                arrData.push(["Ciudad", "TNs", "CUITS"]);
                                var objAux = datos.concat();
                                objAux = objAux.filter(function (x) { return x.Provincia != null });
                                var capfed = objAux.filter(function (x) { return x.Provincia == "CAPITAL FEDERAL" });
                                if (capfed.length == 0) {
                                    capfed = {
                                        Cuit: 0,
                                        Estado: null,
                                        Grano: 3,
                                        Provincia: "BUENOS AIRES",
                                        Segmentacion: null,
                                        Tonelada: 0//null
                                    };
                                } else {
                                    capfed = capfed[0];
                                }
                                objAux = objAux.filter(function (x) { return x.Provincia != "CAPITAL FEDERAL" });
                                var bsas = objAux.filter(function (x) { return x.Provincia == "BUENOS AIRES" });
                                if (bsas.length == 0) {
                                    bsas = {
                                        Cuit: 0,
                                        Estado: null,
                                        Grano: 3,
                                        Provincia: "BUENOS AIRES",
                                        Segmentacion: null,
                                        Tonelada: 0//null
                                    };
                                } else {
                                    bsas = bsas[0];
                                }
                                objAux = objAux.filter(function (x) { return x.Provincia != "BUENOS AIRES" });
                                var bsasall = {
                                    Cuit: capfed.Cuit + bsas.Cuit,
                                    Estado: null,
                                    Grano: 3,
                                    Provincia: "BUENOS AIRES",
                                    Segmentacion: null,
                                    Tonelada: capfed.Tonelada + bsas.Tonelada
                                };
                                //if (bsas.Tonelada !== null || capfed.Tonelada !== null)
                                objAux.push(bsasall);
                                for (var ii in objAux) {
                                    (function (i) {
                                        arrData.push([objAux[i].Provincia, objAux[i].Tonelada, objAux[i].Cuit]);
                                    })(ii);
                                }
                                armarGraficoMapa(arrData);
                                break;
                            case "barras":
                                if (filtro.Indicadores == "capacidadacopio") {
                                    if (!datos[0].MasCl100 && !datos[0].MasCl1000 && !datos[0].MasCl5000 && !datos[0].MasCl10000 && !datos[0].MasCl20000 && !datos[0].MasCl40000 && !datos[0].MasTn100 && !datos[0].MasTn1000 && !datos[0].MasTn5000 && !datos[0].MasTn10000 && !datos[0].MasTn20000 && !datos[0].MasTn40000 && !datos[0].MenosCl10000 && !datos[0].MenosTn10000) {
                                        $("#graficoGR").empty();
                                        $("#graficoGR").css("display", "none");
                                        $("#graficoR").empty();
                                        var graficoid = $("#graficoR");
                                        var sd = $("<div id='sindatos'>").appendTo(graficoid).css({
                                            padding: 25,
                                            'font-size': 20,
                                            'text-align': 'center',
                                            backgroundColor: '#f0f0f0',
                                            'margin-top': 30
                                        });
                                        $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                            width: 30,
                                            height: 30,
                                            backgroundColor: '#f0f0f0',
                                            'vertical-align': 'top'
                                        });
                                        $("<span>").appendTo(sd).html("No se encontraron resultados").css({
                                            'vertical-align': 'top',
                                            'padding-left': 10,
                                            backgroundColor: '#f0f0f0',
                                            'padding-right': 10
                                        });
                                        $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                            width: 30,
                                            height: 30,
                                            backgroundColor: '#f0f0f0',
                                            'vertical-align': 'top'
                                        });
                                    }
                                    else {
                                        armarGraficoBarrasAcopio(datos);
                                    }
                                }
                                else if (filtro.Indicadores == "capacidadproductiva") {
                                    if (!datos[0].MasCl100 && !datos[0].MasCl1000 && !datos[0].MasCl5000 && !datos[0].MasCl10000 && !datos[0].MasCl20000 && !datos[0].MasCl40000 && !datos[0].MasTn100 && !datos[0].MasTn1000 && !datos[0].MasTn5000 && !datos[0].MasTn10000 && !datos[0].MasTn20000 && !datos[0].MasTn40000 && !datos[0].MenosCl10000 && !datos[0].MenosTn10000) {
                                        $("#graficoGR").empty();
                                        $("#graficoGR").css("display", "none");
                                        $("#graficoR").empty();
                                        var graficoid = $("#graficoR");
                                        var sd = $("<div id='sindatos'>").appendTo(graficoid).css({
                                            padding: 25,
                                            'font-size': 20,
                                            'text-align': 'center',
                                            backgroundColor: '#f0f0f0',
                                            'margin-top': 30
                                        });
                                        $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                            width: 30,
                                            height: 30,
                                            backgroundColor: '#f0f0f0',
                                            'vertical-align': 'top'
                                        });
                                        $("<span>").appendTo(sd).html("No se encontraron resultados").css({
                                            'vertical-align': 'top',
                                            'padding-left': 10,
                                            backgroundColor: '#f0f0f0',
                                            'padding-right': 10
                                        });
                                        $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                                            width: 30,
                                            height: 30,
                                            backgroundColor: '#f0f0f0',
                                            'vertical-align': 'top'
                                        });
                                    }
                                    else {
                                        armarGraficoBarrasProduccion(datos);
                                    }
                                }

                                break;
                        }
                        break;
                    case "objetivoscomercial":
                        switch (filtro.Grafico) {
                            case "gaudge":
                                armarGraficoGaudge(datos);
                                break;
                        }
                        break;
                    case "basededatos":
                        switch (filtro.Grafico) {
                            case "torta":
                                crearGrilla(datos.valoresGrilla);
								/*
								var arrData = [];
									arrData.push(["Segmentación", "Toneladas"]);
									var objAux = datos.graficoBaseDatos.concat();
									for (var ii in objAux) {
										(function (i) {
											arrData.push([objAux[i].segmentacion, objAux[i].Toneladas]);
										})(ii);
									}

									*/
                                var arrData = [];
                                var total = 0;
                                var objAux = datos.graficoBaseDatos.concat();
                                //Saco el total de cuits
                                for (var ii in objAux) {
                                    (function (i) {
                                        total += objAux[i].Toneladas;
                                    })(ii);
                                }
                                var valor = [];
                                var sumaValor = 0;
                                var MayorValor = 0;

                                //Verifico si da el 100% por redondeo
                                for (var ii in objAux) {
                                    (function (i) {
                                        sumaValor += parseFloat(((objAux[i].Toneladas / total) * 100).toFixed(2));
                                        if (objAux[i].Toneladas > MayorValor) {
                                            MayorValor = objAux[i].Toneladas;
                                        }
                                    })(ii);
                                }

                                if (sumaValor == 100) {
                                    for (var ii in objAux) {
                                        (function (i) {
                                            //valor.push([((objAux[i].Cuit * 100)/total).toFixed(2)]);
                                            var mensaje = "<strong>" + objAux[i].segmentacion + "</strong></br> <strong>Cuits.:</strong> " + objAux[i].Toneladas;
                                            mensaje += " (" + ((objAux[i].Toneladas * 100) / total).toFixed(2) + "%)";

                                            var segmentacion = objAux[i].segmentacion + ' - ' + ((objAux[i].Toneladas * 100) / total).toFixed(2) + ' % Cuits : ' + objAux[i].Toneladas;

                                            arrData.push([segmentacion, objAux[i].Toneladas, mensaje]);
                                        })(ii);
                                    }
                                }
                                else {
                                    var sumarizado = 0;
                                    for (var ii in objAux) {
                                        (function (i) {
                                            //valor.push([((objAux[i].Cuit * 100)/total).toFixed(2)]);
                                            var segmentacion = '';
                                            if ((MayorValor == objAux[i].Toneladas) && (sumarizado == 0)) {
                                                var mensaje = "<strong>" + objAux[i].segmentacion + "</strong></br> <strong>Cuits.:</strong> " + objAux[i].Toneladas;
                                                mensaje += " (" + (((objAux[i].Toneladas * 100) / total) + parseFloat(100 - sumaValor)).toFixed(2) + " %)";
                                                segmentacion = objAux[i].segmentacion + ' - ' + (((objAux[i].Toneladas * 100) / total) + parseFloat(100 - sumaValor)).toFixed(2) + ' % Cuits : ' + objAux[i].Toneladas;
                                                sumarizado = 1;
                                            }
                                            else {
                                                var mensaje = "<strong>" + objAux[i].segmentacion + "</strong></br> <strong>Cuits.:</strong> " + objAux[i].Toneladas;
                                                mensaje += " (" + ((objAux[i].Toneladas * 100) / total).toFixed(2) + "%)";
                                                segmentacion = objAux[i].segmentacion + ' - ' + ((objAux[i].Toneladas * 100) / total).toFixed(2) + ' % Cuits : ' + objAux[i].Toneladas;
                                            }

                                            arrData.push([segmentacion, objAux[i].Toneladas, mensaje]);

                                            //arrData.push([objAux[i].Provincia, objAux[i].Cuit, mensaje]);
                                        })(ii);
                                    }
                                }

                                armarGraficoTorta(arrData, "Segmentación por capacidad productiva");

                                break;
                        }
                        break;
                }
            } else {
                $("#graficoGR").empty();
                $("#graficoGR").css("display", "none");
                $("#graficoR").empty();
                var graficoid = $("#graficoR");
                var sd = $("<div id='sindatos'>").appendTo(graficoid).css({
                    padding: 25,
                    'font-size': 20,
                    'text-align': 'center',
                    backgroundColor: '#f0f0f0',
                    'margin-top': 30
                });
                $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                    width: 30,
                    height: 30,
                    backgroundColor: '#f0f0f0',
                    'vertical-align': 'top'
                });
                $("<span>").appendTo(sd).html("No se encontraron resultados").css({
                    'vertical-align': 'top',
                    'padding-left': 10,
                    backgroundColor: '#f0f0f0',
                    'padding-right': 10
                });
                $("<img src='../Content/Images/agro.png'>").appendTo(sd).css({
                    width: 30,
                    height: 30,
                    backgroundColor: '#f0f0f0',
                    'vertical-align': 'top'
                });
            }
        }
        if (filtro.Grafico == "barras" && filtro.Indicadores == "compras") {
            MSExecuteOnServerAsync('/Reportes/TraerDatosReporteComprasBarra', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "gaudge") {
            MSExecuteOnServerAsync('/Reportes/TraerDatosReporteObjetivoGauge', filtro, funcReturn, false);
        }
        else if (filtro.Indicadores == "basededatos") {
            MSExecuteOnServerAsync('/Reportes/TraerDatosGrillaBD', filtro, funcReturn, false);
        } else {
            MSExecuteOnServerAsync('/Reportes/TraerDatosReporteCompras', filtro, funcReturn, false);
        }
    });

    function DescargarExcel(param, data) {
        var funcReturn = function (data) {
            if (data != null) {
                if (data.DownloadKey.length > 0) {
                    var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
                    window.location = url;
                }
            }
        }

        MSExecuteOnServerAsync('/Home/ExportarContactosExcel', param, funcReturn, true);
    }

    $("#btn-exportar").click(function () {
        var filtro = getFiltros();

        if (filtro.Indicadores == "objetivoscomercial" && filtro.Grafico == "gaudge") {
            if (filtro.Cosecha == "null") {
                MensErr("La cosecha debe estar definida");
                return false;
            }
        }

        if (filtro.Indicadores == "basededatos" && filtro.Grafico == "torta") {
            if (!filtro.FechaDesde || !filtro.FechaHasta) {
                MensErr("La fecha debe estar definida");
                return false;
            }
        }

        var funcReturn = function (data) {
            if (data != null) {
                if (data.Errores.length > 0) {
                    MensErr("No se encontraron resultados para los filtros elegidos.");
                    return false;
                }
                else {
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

        if (filtro.Grafico == "barras" && filtro.Indicadores == "compras") {
            MSExecuteOnServerAsync('/Reportes/ExportarComprasBarrasIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "mapa" && filtro.Indicadores == "compras") {
            MSExecuteOnServerAsync('/Reportes/ExportarCompraMapaIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "torta" && filtro.Indicadores == "compras") {
            MSExecuteOnServerAsync('/Reportes/ExportarCompraTortaIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "gaudge" && filtro.Indicadores == "objetivoscomercial") {
            MSExecuteOnServerAsync('/Reportes/ExportarObjetivosGaugeIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "torta" && filtro.Indicadores == "basededatos") {
            MSExecuteOnServerAsync('/Reportes/ExportarBD', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "mapa" && filtro.Indicadores == "capacidadproductiva") {
            MSExecuteOnServerAsync('/Reportes/ExportarProduccionMapaIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "barras" && filtro.Indicadores == "capacidadproductiva") {
            MSExecuteOnServerAsync('/Reportes/ExportarProduccionBarraIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "mapa" && filtro.Indicadores == "capacidadacopio") {
            MSExecuteOnServerAsync('/Reportes/ExportarAcopioMapaIndicadores', filtro, funcReturn, false);
        }
        else if (filtro.Grafico == "barras" && filtro.Indicadores == "capacidadacopio") {
            MSExecuteOnServerAsync('/Reportes/ExportarAcopioBarraIndicadores', filtro, funcReturn, false);
        }
        else {
            MSExecuteOnServerAsync('/Reportes/ExportarIndicadores', filtro, funcReturn, false);
        }
    });
}

function armarFiltro() {
    var filtros = $(".filtro .wizard-header-contenedor-grupo").children();
    setTimeout(function () {
        for (var i = 0; i < filtros.length; i++) {
            if ($(filtros[i]).prop("id") != "filtros-titulo")
                $(filtros[i]).hide();
        }
    }, 400);
    var indicador = $(".whc-selected").prop("id");
    var grafico = $(".whcsub-selected").prop("id");

    switch (indicador) {
        case "compras":
            switch (grafico) {
                case "mapa":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#provincia").show();
                        $("#segmentacion").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 600);
                    break;
                case "torta":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 600);
                    break;
                case "barras":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#segmentacion").show();
                        $("#mes").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 600);
                    break;
            }
            break;
        case "capacidadproductiva":
        case "capacidadacopio":
            switch (grafico) {
                case "mapa":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#provincia").show();
                        $("#segmentacion").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 800);
                    break;
                case "barras":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#segmentacion").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 800);
                    break;
            }
            break;
        case "objetivoscomercial":
            switch (grafico) {
                case "gaudge":
                    setTimeout(function () {
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 800);
                    break;
            }
            break;
        case "basededatos":
            switch (grafico) {
                case "torta":
                    setTimeout(function () {
                        $("#fdesde").show();
                        $("#fhasta").show();
                        $("#grano").show();
                        $("#cosecha").show();
                        $("#cuit").show();
                        $("#toneladas").show();
                        $("#mes").show();
                        $("#segmentacion").show();
                        $("#comercial").show();
                        $(".filtro").animate({ width: 'toggle' }, 400);
                        $(".botones").slideDown();
                    }, 800);
                    break;
            }
            break;
        default:
            break;
    }
}

function armarGraficoMapa(arrData) {
    var data = google.visualization.arrayToDataTable(arrData);

    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    var options = {
        region: 'AR',
        displayMode: 'regions',
        resolution: 'provinces',
        backgroundColor: '#f0f0f0',
        colorAxis: { colors: ['yellow', 'orange', 'green'] },
        legend: {
            numberFormat: "###.### TNs."
        }
    };

    var formatter = new google.visualization.NumberFormat({ groupingSymbol: '.', fractionDigits: 0 });
    formatter.format(data, 1); // Apply formatter to second column

    var chart = new google.visualization.GeoChart(document.getElementById('graficoR'));

    chart.draw(data, options);
}

function armarGraficoTorta(arrData, titulo) {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    //var data = google.visualization.arrayToDataTable(arrData);

    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Year');
    data.addColumn('number', 'Sales');
    // A column for custom tooltip content
    data.addColumn({ type: 'string', role: 'tooltip', p: { html: true } });

    for (var ii in arrData) {
        (function (i) {
            data.addRows([[arrData[i][0], arrData[i][1], arrData[i][2]]]);
        })(ii);
    }

    var total = google.visualization.data.group(data, [{
        type: 'boolean',

        column: 0,

        modifier: function () { return true; }
    }], [{
        type: 'number',

        column: 1,

        aggregation: google.visualization.data.sum
    }], [{
        type: 'boolean',

        column: 2,

        modifier: function () { return true; }
    }]);

    data.addRow(['Total: ' + total.getValue(0, 1).toLocaleString() + " Tn", 0, 'Total: ' + total.getValue(0, 1).toLocaleString() + " Tn"]);

    var colores = ['#f6e58d', '#ff7979', '#badc58', '#c7ecee', '#e056fd', '#686de0', '#30336b', '#ff3f34', '#05c46b', '#ffa801', '#B33771', '#58B19F', '#a4b0be'];

    var options = null;
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        options = {
            title: titulo ? titulo : "",
            //is3D: true,
            //pieSliceText: 'value-and-percentage'
            pieSliceText: 'none',
            sliceVisibilityThreshold: 0,
            tooltip: { isHtml: true },
            chartArea: { left: 0, width: '100%', height: '70%' },
            //legend:{ position:{left:-10}, textStyle: { fontSize: 12, bold : true}},
            backgroundColor: '#f0f0f0',
            colors: colores,
            'legend': { 'position': 'none' }
        };
    } else {
        options = {
            title: titulo ? titulo : "",
            //is3D: true,
            //pieSliceText: 'value-and-percentage'
            pieSliceText: 'none',
            sliceVisibilityThreshold: 0,
            tooltip: { isHtml: true },
            chartArea: { left: 0, width: '100%', height: '70%' },
            colors: colores,
            backgroundColor: '#f0f0f0',
            legend: { position: { left: -10 }, textStyle: { fontSize: 12, bold: true } }
        };
    }

    //var formatter = new google.visualization.NumberFormat({pattern:'Cant. de Cuit - ###,###', fractionDigits: 2} );

    //var formatter = new google.visualization.NumberFormat({pattern:'Cuits: ###', fractionDigits: 2} );
    //formatter.format(data, 1);

    var formatter = new google.visualization.NumberFormat({
        fractionDigits: 2,
        suffix: '%'
    });
	/*
	setTimeout(function(){
			var textos =  $('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > g > text');

			var circulo = $('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > circle');

			for (var i = 0; i < circulo.length; i++){
				//$($("#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > circle")[i]).attr("cx", "485");
				$($("#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > circle")[i]).attr("cx", ($($("#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > circle")[i]).attr("cx") - 70));
				$($("#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > circle")[i]).attr("r", "6");
			}

			for (var i = 0; i < textos.length; i++){
				//$($('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > g > text')[i]).attr("x", "496");
				$($('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > g > text')[i]).attr("x", ($($('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > g > text')[i]).attr("x")-60));
			    $($('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g > g > text')[i]).attr("font-size","12");
			}

			//$('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g:nth-child(2) > g > text').attr("x", "496");
			//$('#graficoR > div > div:nth-child(1) > div > svg > g:nth-child(4) > g:nth-child(2) > circle').attr("cx", "485");
		},300);

		*/
    var chart = new google.visualization.PieChart(document.getElementById('graficoR'));

    chart.draw(data, options);

    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        var grafico = $("#graficoR");
        var div = $("<div>").appendTo(grafico)
        var tabla = $("<div class='tabla'>").appendTo(div);
        var trHead = $("<div class='thead'>").appendTo(tabla);
        $("<div class='tdhead'>").html("Color").appendTo(trHead);
        $("<div class='tdhead'>").html("Leyenda").appendTo(trHead);
        var cant = 0;
        for (var i = 0; i < arrData.length; i++) {
            var trBody = $("<div class='tbody'>").appendTo(tabla);
            $("<div class='tdbodyuno'>").css({
                'background': colores[i]
            }).appendTo(trBody);
            $("<div class='tdbodydos'>").html(arrData[i][0]).appendTo(trBody);
            cant = i;
        }

        var trBody = $("<div  class='tbody'>").appendTo(tabla);
        $("<div class='tdbodyuno'>").css({
            'background': '#8b008b'
        }).appendTo(trBody);
        $("<div class='tdbodydos'>").html('Total: ' + total.getValue(0, 1).toLocaleString() + " Tn").appendTo(trBody);
    }
}

function armarGraficoBarrasCompras(datos) {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    var data = google.visualization.arrayToDataTable([
        ['Cantidad', 'TNs', 'Proveedores'],
        ['Menor a 5000 TNs', (datos[0].MasTn100 ? datos[0].MasTn100 : 0), (datos[0].MasCl100 ? datos[0].MasCl100 : 0)],
        ['5000 a 10000 TNs', (datos[0].MasTn1000 ? datos[0].MasTn1000 : 0), (datos[0].MasCl1000 ? datos[0].MasCl1000 : 0)],
        ['Más de 10000 TNs', (datos[0].MasTn5000 ? datos[0].MasTn5000 : 0), (datos[0].MasCl5000 ? datos[0].MasCl5000 : 0)]

    ]);

    var options = {
        title: 'Perfil de Proveedores',
        // multiple axis (you can have different labels, colors, etc.)
        vAxes: [
            { title: "Tns.", format: '###.###', viewWindowMode: 'explicit', formatOptions: { format: '###.###', groupingSymbol: '.' } },
            { title: "Cuits" }
        ],
        vAxis: { formatOptions: { format: '###.###', groupingSymbol: '.' } },
        // hAxis: {title: "date"},
        seriesType: "bars",
        backgroundColor: '#f0f0f0',

        series: {
            1: { type: "line", targetAxisIndex: 1 }
        }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('graficoR'));

    var formatter = new google.visualization.NumberFormat({ groupingSymbol: '.', fractionDigits: 0 });
    formatter.format(data, 1); // Apply formatter to second column
    formatter.format(data, 2);
    chart.draw(data, options);
}

function armarGraficoBarrasAcopio(datos) {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    var data = google.visualization.arrayToDataTable([
        ['Cantidad', 'TNs', 'Proveedores'],
        ['Menor a 10000 TNs', (datos[0].MenosTn10000 ? datos[0].MenosTn10000 : 0), (datos[0].MenosCl10000 ? datos[0].MenosCl10000 : 0)],
        ['10000 a 20000 TNs', (datos[0].MasTn10000 ? datos[0].MasTn10000 : 0), (datos[0].MasCl10000 ? datos[0].MasCl10000 : 0)],
        ['20000 a 40000 TNs', (datos[0].MasTn20000 ? datos[0].MasTn20000 : 0), (datos[0].MasCl20000 ? datos[0].MasCl20000 : 0)],
        ['Más de 40000  TNs', (datos[0].MasTn40000 ? datos[0].MasTn40000 : 0), (datos[0].MasCl40000 ? datos[0].MasCl40000 : 0)]

    ]);

    var options = {
        title: 'Perfil de Proveedores',
        // multiple axis (you can have different labels, colors, etc.)
        vAxes: [
            { title: "Tns.", format: '###.###', viewWindowMode: 'explicit', formatOptions: { format: '###.###', groupingSymbol: '.' } },
            { title: "Cuits" }
        ],
        vAxis: { formatOptions: { format: '###.###', groupingSymbol: '.' } },
        // hAxis: {title: "date"},
        seriesType: "bars",
        backgroundColor: '#f0f0f0',

        series: {
            1: { type: "line", targetAxisIndex: 1 }
        }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('graficoR'));

    var formatter = new google.visualization.NumberFormat({ groupingSymbol: '.', fractionDigits: 0 });
    formatter.format(data, 1); // Apply formatter to second column
    formatter.format(data, 2);
    chart.draw(data, options);
}

function armarGraficoBarrasProduccion(datos) {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    var data = google.visualization.arrayToDataTable([
        ['Cantidad', 'TNs', 'Proveedores'],
        ['Menor a 5000 TNs', (datos[0].MasTn100 ? datos[0].MasTn100 : 0), (datos[0].MasCl100 ? datos[0].MasCl100 : 0)],
        ['5000 a 10000 TNs', (datos[0].MasTn1000 ? datos[0].MasTn1000 : 0), (datos[0].MasCl1000 ? datos[0].MasCl1000 : 0)],
        ['10000 a 20000 TNs', (datos[0].MasTn10000 ? datos[0].MasTn10000 : 0), (datos[0].MasCl10000 ? datos[0].MasCl10000 : 0)],
        ['Más de 20000  TNs', (datos[0].MasTn20000 ? datos[0].MasTn20000 : 0), (datos[0].MasCl20000 ? datos[0].MasCl20000 : 0)]

    ]);

    var options = {
        title: 'Perfil de Proveedores',
        // multiple axis (you can have different labels, colors, etc.)
        vAxes: [
            { title: "Tns.", format: '###.###', viewWindowMode: 'explicit', formatOptions: { format: '###.###', groupingSymbol: '.' } },
            { title: "Cuits" }
        ],
        vAxis: { formatOptions: { format: '###.###', groupingSymbol: '.' } },
        // hAxis: {title: "date"},
        seriesType: "bars",
        backgroundColor: '#f0f0f0',

        series: {
            1: { type: "line", targetAxisIndex: 1 }
        }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('graficoR'));

    var formatter = new google.visualization.NumberFormat({ groupingSymbol: '.', fractionDigits: 0 });
    formatter.format(data, 1); // Apply formatter to second column
    formatter.format(data, 2);
    chart.draw(data, options);
}

function armarGraficoBarras(datos) {
    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 400
        });
    }

    var data = google.visualization.arrayToDataTable([
        ['Cantidad', 'TNs', 'Proveedores'],
        ['Menor a 2500 TNs', (datos[0].MasTn100 ? datos[0].MasTn100 : 0), (datos[0].MasCl100 ? datos[0].MasCl100 : 0)],
        ['2500 a 5000 TNs', (datos[0].MasTn1000 ? datos[0].MasTn1000 : 0), (datos[0].MasCl1000 ? datos[0].MasCl1000 : 0)],
        ['Más de 5000 TNs', (datos[0].MasTn5000 ? datos[0].MasTn5000 : 0), (datos[0].MasCl5000 ? datos[0].MasCl5000 : 0)]

    ]);
    /*
        var columnRange = data.getColumnRange(1);
        var ticks = [];
        for (var i = columnRange.min; i <= columnRange.max; i=i+1000000) {
          ticks.push({
            v: i,
            f: formatNumber.formatValue(i)
          });
        }*/
    //,format:'###.###',legend:'###.###',groupingSymbol: '.'
    var options = {
        title: 'Perfil de Proveedores',
        // multiple axis (you can have different labels, colors, etc.)
        vAxes: [
            { title: "Tns.", format: '###.###', viewWindowMode: 'explicit', formatOptions: { format: '###.###', groupingSymbol: '.' } },
            { title: "Cuits" }
        ],
        vAxis: { formatOptions: { format: '###.###', groupingSymbol: '.' } },
        // hAxis: {title: "date"},
        seriesType: "bars",
        backgroundColor: '#f0f0f0',

        series: {
            1: { type: "line", targetAxisIndex: 1 }
        }
    };

    var chart = new google.visualization.ComboChart(document.getElementById('graficoR'));

    var formatter = new google.visualization.NumberFormat({ groupingSymbol: '.', fractionDigits: 0 });
    formatter.format(data, 1); // Apply formatter to second column
    formatter.format(data, 2);
    chart.draw(data, options);
}

function armarGraficoGaudge(datos) {
    var arrData = [];
    arrData.push(['Label', 'Value']);
    for (var ii in datos) {
        (function (i) {
            arrData.push([datos[i].Material, datos[i].Porcentajes]);
        })(ii);
    }

    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
        $("#graficoR").css({
            height: 1200
        });
    }

    var data = google.visualization.arrayToDataTable(arrData);

    var options = {
        width: 800, height: 300,
        redFrom: 0, redTo: 40,
        yellowFrom: 40, yellowTo: 70,
        greenFrom: 70, greenTo: 100,
        minorTicks: 5,
        backgroundColor: '#f0f0f0'
    };

    var chart = new google.visualization.Gauge(document.getElementById('graficoR'));

    setTimeout(function () {
        var textos = $("#graficoR svg g g text");

        for (var i = 0; i < textos.length; i++) {
            $($("#graficoR svg g g text")[i]).html($($("#graficoR svg g g text")[i]).html() + " %");
        }

        for (var i = 0; i < 6; i++) {
            $($("#graficoR svg text:first-of-type")[i]).attr('style', 'font-size:18px!important;font-weight:bold!important;');
        }
    }, 300);

    chart.draw(data, options);
    /*
            setInterval(function() {
              data.setValue(0, 1, 40 + Math.round(60 * Math.random()));
              chart.draw(data, options);
            }, 13000);
            setInterval(function() {
              data.setValue(1, 1, 40 + Math.round(60 * Math.random()));
              chart.draw(data, options);
            }, 5000);
            setInterval(function() {
              data.setValue(2, 1, 60 + Math.round(20 * Math.random()));
              chart.draw(data, options);
            }, 26000);
            */
    $("#graficoR").css({
        padding: 25
    });
}

function cargarCombos() {
    function funcReturn(datos) {
        //Grano

        var granos = $("#txtgrano")
        granos.empty();
        $("<option>").text('- Elegir').val('null').appendTo(granos);
        for (var ii in datos.mat) {
            (function (i) {
                $("<option>").text(datos.mat[i].Descripcion).val("" + datos.mat[i].MaterialId).appendTo(granos);
            })(ii);
        }

        var cosecha = $("#txtcosecha")
        cosecha.empty();
        $("<option>").text('- Elegir').val('null').appendTo(cosecha);
        for (var ii in datos.camp) {
            (function (i) {
                $("<option>").text("Campaña " + datos.camp[i].Descripcion).val("" + datos.camp[i].CampañaId).appendTo(cosecha);
            })(ii);
        }

        var provincia = $("#txtprovincia")
        provincia.empty();
        $("<option>").text('- Elegir').val('null').appendTo(provincia);
        for (var ii in datos.provs) {
            (function (i) {
                $("<option>").text(datos.provs[i].Nombre).val("" + datos.provs[i].Provinciaid).appendTo(provincia);
            })(ii);
        }

        var comercial = $("#txtcomercial2")
        comercial.empty();
        $("<option>").text('- Elegir').val('null').appendTo(comercial);
        for (var ii in datos.come) {
            (function (i) {
                $("<option>").text(datos.come[i].Nombre).val(datos.come[i].ComercialId).appendTo(comercial);
            })(ii);
        }

        var grupos = {};
        for (var jj in datos.segm) {
            (function (j) {
                grupos[datos.segm[j].Grupo] = grupos[datos.segm[j].Grupo] || [];
                grupos[datos.segm[j].Grupo].push({
                    SegmentacionId: datos.segm[j].SegmentacionId,
                    Descripcion: datos.segm[j].Descripcion
                });
            })(jj);
        }

        var htmlSegmentacion = "";
        htmlSegmentacion += '<select id="txtsegmentacion" class="excluir">';
        htmlSegmentacion += '<option value="0">Todos</option>';
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

        $(".contenedor-segundo-filtro-campo-segmentacion").append(htmlSegmentacion);
        //SegmentacionMultiple

        $("#txtsegmentacion").multiselect({
            header: false,
            //selectedList: 1,
            multiselect: true,
            noneSelectedText: "Elegir",
            click: function (event, ui) {
                if (ui.checked) {
                    SegmentacionMultiple.push([parseInt(ui.value)]);
                }
                else {
                    var index = -1
                    for (var i = 0; i < SegmentacionMultiple.length; i++) {
                        if (SegmentacionMultiple[i] == parseInt(ui.value)) {
                            index = i;
                        }
                    }
                    if (index !== -1) SegmentacionMultiple.splice(index, 1);
                }
            },
        });
		/*
			var segmentacion = $("#txtsegmentacion")
			segmentacion.empty();
			$("<option>").text('- Elegir').val('null').appendTo(segmentacion);
			for (var ii in datos.segm) {
				(function (i) {
					$("<option>").text(datos.segm[i].Descripcion).val("" + datos.segm[i].SegmentacionId).appendTo(segmentacion);
				})(ii);
			}
		*/
        var comercial = $("#txtcomercial")
        comercial.empty();
        $("<option>").text('- Elegir').val('null').appendTo(comercial);
        for (var ii in datos.come) {
            (function (i) {
                $("<option>").text(datos.come[i].IdActiveDirectory).val("" + datos.come[i].ComercialId).appendTo(comercial);
            })(ii);
        }
    }

    MSExecuteURLOnServerAsync('/Reportes/TraerDatosCombo', funcReturn, false);
}

function getFiltros() {
    var obj = {
        Indicadores: $(".whc-selected").prop("id"),
        Grafico: $(".whcsub-selected").prop("id"),
        Grano: $("#txtgrano").val(),
        Mes: $("#txtmes").val(),
        Provincia: $("#txtprovincia").val(),
        Cosecha: $("#txtcosecha").val(),
        Segmentacion: SegmentacionMultiple.join(','), //$("#txtsegmentacion").val(),
        Comercial: $("#txtcomercial").val(),
        Toneladas: $("#txttoneladas").val(),
        Objetivos: $("#txtobjetivos").val(),
        FechaDesde: $("#txtdesde").val(),
        FechaHasta: $("#txthasta").val()
    }
    return obj;
}

function crearGrilla(obj) {
    $("#graficoGR").empty();
    $("#graficoGR").show();
    $("#graficoGR").kendoGrid({
        dataSource: {
            data: obj
        },
        height: 300,
        sortable: true,
        scrolleable: true,
        columns: [{
            field: "Cuit",
            title: "CUIT",
            width: 240
        }, {
            field: "RazonSocial",
            title: "Razón Social",
            width: 240
        }, {
            field: "Estado",
            title: "Estado",
            width: 240
        }, {
            field: "Segmentación",
            title: "Segmentación",
            width: 240
        }, {
            field: "Grano",
            title: "Grano",
            width: 240
        }, {
            field: "Toneladas",
            title: "Toneladas",
            width: 240
        }, {
            field: "FechaAlta",
            title: "FechaAlta",
            width: 240,
            template: '#= kendo.toString(kendo.parseDate(FechaAlta), "dd/MM/yyyy") #'
        }, {
            field: "Comercial",
            title: "Comercial",
            width: 240
        }]
    });
}