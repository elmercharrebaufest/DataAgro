cuposAsignadosPorDia = [];

$(document).ready(function () {
    $('[data-toggle="popover"]').popover({
        delay: { "show": 50, "hide": 10 }
    });
    inicializarElementos();
});
function CalcularTotal(elem) {

    if (elem != null) {
        var fila = elem.getAttribute("data-row");
        var col = elem.getAttribute("data-column");
        var cantidadCambio = $('label[data-row="' + fila + '"][data-column="' + col + '"]').text();
        var cantidadSinCambio = $('span[data-rowContenido="' + fila + '"][data-columnContenido="' + col + '"]').text();
        if (cantidadCambio != cantidadSinCambio) {
            $('span[data-mostrarCruzFila="' + fila + '"][data-mostrarCruzColumna="' + col + '"]').show();
            $('label[data-row="' + fila + '"][data-column="' + col + '"]').attr('contenteditable', false);
            $('input[data-row="' + fila + '"][data-column="' + col + '"]').prop('checked', true);
            $('input[data-row="' + fila + '"][data-column="' + col + '"]').prop('disabled', true);
            $('td[data-row="' + fila + '"][data-column="' + col + '"]').addClass("blueClass");

        }
    }
   

    var $filasEncabezado = $("#tabla tr:not('.encabezado')");
    var nColumnas = $("#tabla tr:last td").length;
    var totales = [];
    var cuposDisponibles = 0;
    var cuposExcedidos = 0;
    var cuposProveedor = [];
    var cuposExcedidosPorFila = 0;
    var cuposDisponiblesPorFila = 0;

    for (var i = 1; i < nColumnas; i++) {
        totales.push(0);
    }
    
    $filasEncabezado.each(function () {
        var valorFila = 0
        $(this).find('td').each(function (i) {
            if (i != 0) {
                totales[i - 1] += parseInt($(this).context.innerText);
                valorFila += parseInt($(this).context.innerText);
            }
        });
        cuposProveedor.push(valorFila);
    });

    var habilita = true;

    for (var i = 0; i < totales.length; i++) {
        $(".col" + (i + 1)).removeClass("redClass");
        $('#check' + (i + 1)).prop('disabled', false);
        if (totales[i] > cuposAsignadosPorDia[i] || isNaN(totales[i])) {             
            cuposExcedidos = totales[i] - cuposAsignadosPorDia[i];
            if (!isNaN(cuposExcedidos)) {           

                $('#popover' + "col" + (i + 1))[0].dataset.content = "Cupos pendientes a Confirmar: " + cuposExcedidos;
                $('#popover' + "col" + (i + 1)).popover("show");                
                $("." + (i + 1)).prop('disabled', false);
                habilita = true;

            } else {
                $('#popover' + "col" + (i + 1))[0].dataset.content = "Los campos no deben estar vacíos";               
                $('#popover' + "col" + (i + 1)).popover("show");
                $('#check' + (i + 1)).prop('disabled', true);
                $(".col" + (i + 1)).addClass("redClass");
                $("." + (i + 1)).prop('disabled', true);
                $("#label" + (i + 1)).attr('contenteditable', true);
                OcultarPopover($('#popover' + "col" + (i + 1)));
                habilita = false;
            }
           
        } else if (totales[i] < cuposAsignadosPorDia[i] || isNaN(totales[i])) {
            cuposDisponibles = cuposAsignadosPorDia[i] - totales[i];
            $(".col" + (i + 1)).removeClass("redClass");

            if (!isNaN(cuposDisponibles)) {
                $('#popover' + "col" + (i + 1))[0].dataset.content = "Cupos disponibles:" + cuposDisponibles;
                $('#popover' + "col" + (i + 1)).popover("show");
                $("." + (i + 1)).prop('disabled', false);
                OcultarPopover($('#popover' + "col" + (i + 1)));
                habilita = true;
            } else {
                $('#popover' + "col" + (i + 1))[0].dataset.content = "Los campos no deben estar vacíos";
                $('#popover' + "col" + (i + 1)).popover("show");
                $(".col" + (i + 1)).addClass("redClass");
                $("." + (i + 1)).prop('disabled', true);
                $("#label" + (i + 1)).attr('contenteditable', true);
                OcultarPopover($('#popover' + "col" + (i + 1)));
                habilita = false;
            }

        } else {           
            $('#popover' + "col" + (i + 1)).popover('hide');
            $(".col" + (i + 1)).removeClass("redClass");
            $("." + (i + 1)).prop('disabled', false);
        }
    }

    for (var j = 0; j < cuposProveedor.length; j++) {
        $(".fila" + (j + 1) + " > td").removeClass("redClass");   
        $('.checkFila' + (j + 1)).prop('disabled', false);
        if (cuposAsignados[j] < cuposProveedor[j] || isNaN(cuposProveedor[j])) {           
            cuposExcedidosPorFila = cuposProveedor[j] - cuposAsignados[j];
            if (!isNaN(cuposExcedidosPorFila)) {          
                $('#popoverFila' + "fila" + (j + 1))[0].dataset.content = "Cupos excedidos: " + cuposExcedidosPorFila;
                $('#popoverFila' + "fila" + (j + 1)).popover("show");    
                $(".fila" + (j + 1) + " > td").addClass("redClass");
                $('.checkFila' + (j + 1)).prop('disabled', true);
                $("." + (j + 1)).prop('disabled', true);
                $("#label" + (j + 1)).attr('contenteditable', true);
                OcultarPopover($('#popoverFila' + "col" + (j + 1)));
            } else {               
                $('#popoverFila' + "fila" + (j + 1))[0].dataset.content = "Los campos no deben estar vacíos";
                $('#popoverFila' + "fila" + (j + 1)).popover("show");
                $(".fila" + (j + 1) + " > td").addClass("redClass");
                $('.checkFila' + (j + 1)).prop('disabled', true);
                $("#label" + (j + 1)).attr('contenteditable', true);
                $("." + (j + 1)).prop('disabled', true);

                OcultarPopover($('#popoverFila' + "fila" + (j + 1)));
            }
                habilita = false;
        } else if (cuposAsignados[j] > cuposProveedor[j] || isNaN(cuposAsignados[j])) {           
            cuposDisponiblesPorFila = cuposAsignados[j] - cuposProveedor[j];
            if (!isNaN(cuposDisponiblesPorFila)) {
                $('#popoverFila' + "fila" + (j + 1))[0].dataset.content = "Cupos disponibles:" + cuposDisponiblesPorFila;
                $('#popoverFila' + "fila" + (j + 1)).popover("show");
                $("." + (j + 1)).prop('disabled', false);

                OcultarPopover($('#popoverFila' + "fila" + (j + 1)));
            } else {                
                $('#popoverFila' + "fila" + (j + 1))[0].dataset.content = "Los campos no deben estar vacíos";
                $('#popoverFila' + "fila" + (j + 1)).popover("show");
                $(".fila" + (j + 1) + " > td").addClass("redClass");
                $("." + (j + 1)).prop('disabled', true);
                OcultarPopover($('#popoverFila' + "fila" + (j + 1)));
                $("#label" + (j + 1)).attr('contenteditable', false);
                habilita = false;
            }

        }else {          
            $('#popoverFila' + "fila" + (j + 1)).popover('hide');
            $(".fila" + (j + 1) + " > td").removeClass("redClass");
            $("." + (j + 1)).prop('disabled', false);
            
        }
    }

    if (habilita == false) {
        $('.btnConfirmar').attr("disabled", true);
    } else {
        $('.btnConfirmar').attr("disabled", false);
    }
}

function OcultarPopover($popover) {
    $popover.on('shown.bs.popover', function () {
        setTimeout(function () {
            $popover.popover('hide');
            
        }, 10000);
    });
}

function BloquearCelda(elem) {
    var fila = elem.getAttribute("data-row");
    var col = elem.getAttribute("data-column");
    if (elem.checked) {
        $('label[data-row="' + fila + '"][data-column="' + col + '"]').attr('contenteditable', false);
    } else {
        $('label[data-row="' + fila + '"][data-column="' + col + '"]').attr('contenteditable', true);
    }
}

function Reestablecer(elem) {
    var fila = elem.getAttribute("data-reestablecerFila");
    var col = elem.getAttribute("data-reestablecerColumna");
    $('span[data-mostrarCruzFila="' + fila + '"][data-mostrarCruzColumna="' + col + '"]').hide();
    $('label[data-row="' + fila + '"][data-column="' + col + '"]').text($('span[data-rowContenido="' + fila + '"][data-columnContenido="' + col + '"]').text());  
    $('input[data-row="' + fila + '"][data-column="' + col + '"]').prop('disabled', false);
    $('input[data-row="' + fila + '"][data-column="' + col + '"]').prop('checked', false);
    $('label[data-row="' + fila + '"][data-column="' + col + '"]').attr('contenteditable', true);
    $('td[data-row="' + fila + '"][data-column="' + col + '"]').removeClass("blueClass");
    CalcularTotal(null);
}

function MarcarFila(elem) {
    if (elem.checked) {
        $(".fila" + elem.className + "> td").addClass("verdeClassFila");
        $('input[data-row="' + elem.className + '"]').prop('checked', true);
        $('label[data-row="' + elem.className + '"]').attr('contenteditable', false);
    } else {

        $(".fila" + elem.className + "> td").removeClass("verdeClassFila");
        $('label[data-row="' + elem.className + '"]').attr('contenteditable', true);
        $('input[data-row="' + elem.className + '"]').prop('checked', false);
    }
}

function MarcarColumna(elem) {
    if (elem.checked) {
        $(".col" + elem.className).addClass("verdeClassColumna");
        $('input[data-column="' + elem.className + '"]').prop('checked', true);
        $('label[data-column="' + elem.className + '"]').attr('contenteditable', false);
    } else {
        $(".col" + elem.className).removeClass("verdeClassColumna");
        $('label[data-column="' + elem.className + '"]').attr('contenteditable', true);
        $('input[data-column="' + elem.className + '"]').prop('checked', false);
    }
}

function ConfirmarSeleccionados() {
    var datosTabla = [];
    var id = 0;
   
    var obj = {};
    var $filas = $("#tabla tr:not('.encabezado')");
    for (var j = 1; j <= $filas.length; j++) {
        var cantidadCupos = "";
        var fecha = "";
        obj = {};
        var detalleCupo = [];
        var k = $('label[data-row=' + j + ']').length;
        for (var i = 0; i < k; i++) {            
            idProveedor = $('label[data-row=' + j + ']')[0].attributes["data-proveedor"].textContent;
            idComercial = $('label[data-rowComercial=' + j + ']')[0].attributes["data-comercial"].textContent;
            proveedorDesc = $('label[data-proveedorDesc=' + j + ']')[0].attributes["data-proveedorDescripcion"].textContent;
            if ($('input[data-row="' + j + '"][data-column="' + i + '"]').length > 0 && $('input[data-row="' + j + '"][data-column="' + i + '"]').prop('checked') && $('input[data-row="' + j + '"][data-column="' + i + '"]').length > 0) {
                //$('div[data-row="' + j + '"][data-column="' + i + '"]').remove();
                fecha = $('.fecha' + i).text();
                cantidadCupos = $('label[data-row="' + j + '"][data-column="' + i + '"]')[0].innerText;
                if (fecha != "" && cantidadCupos != "") {
                    detalleCupo.push({ Fecha: fecha, Cantidad: cantidadCupos, CantidadFleteProcedencia: 0, CantidadSugerencia: cantidadCupos });
                   
                }
            }           
        }
        if (detalleCupo.length > 0) {
            obj.ProveedorId = idProveedor;
            obj.ComercialId = idComercial;
            obj.ProveedorDesc = proveedorDesc;
            obj.Detalles = detalleCupo;
            datosTabla.push(obj);
        }
    
    }
    return datosTabla;
}

function inicializarElementos() {

    $("#confirmarSugerencia").click(function () {
        var lista = ConfirmarSeleccionados();
        if (lista.length == 0) {
            MensErr("Debe seleccionar al menos una sugerencia.");
        } else {
            $("#fleteProcedenciaTabla").modal("show");
        }
    });

    $("#boton-siTabla").click(function () {
        $("#fleteProcedenciaTabla").modal("hide");
        var sugerencias = ConfirmarSeleccionados();
        var fila = '';
        for (var i = 0; i < sugerencias.length; i++) {
            for (var j = 0; j < sugerencias[i].Detalles.length; j++) {
                fila = '<tr><td>' + sugerencias[i].ProveedorDesc + '</td> <td>'
                    + kendo.toString(sugerencias[i].Detalles[j].Fecha, "dd/MM/yyyy")
                    + '</td> <td><input id="flete' + i + '" name="' + sugerencias[i].Detalles[j].CantidadFleteProcedencia + '" min="0" max="' + sugerencias[i].Detalles[j].Cantidad
                    + '" class="cantidad-masiva" value="' + sugerencias[i].Detalles[j].CantidadFleteProcedencia + '"/> </td> <td>'
                    + 'Max. de cupos: ' + sugerencias[i].Detalles[j].Cantidad + '</td></tr>'
                $("#cuerpo-carga-cupos-tabla").append(fila);
            }
        }
        $(".cantidad-masiva").kendoNumericTextBox({
            culture: "es-AR",
            format: "n0",
            spinners: false,
            min: 0
        });
        $("#fleteProcedenciaModal").modal("hide");
        $("#CargaCuposTabla").modal("show");
    });

    $("#boton-noTabla").click(function () {
        $("#fleteProcedenciaTabla").modal("hide");
        var sugerencias = ConfirmarSeleccionados();
        result = MSExecuteOnServer('/SugerenciaCupo/DatosConfirmar', { datosTabla: sugerencias, materialId: $("#MaterialId").val(), centroId: $("#CentroId").val() });

        $.unblockUI();
        var erroresTabla = new Array();
        var cuposGeneradosTabla = new Array();
        if (result.HayError) {
            erroresTabla = erroresTabla.concat(result.ListaErrores);
        }
        if (result.ListaCupos != null && result.ListaCupos.length > 0) {
            cuposGeneradosTabla = cuposGeneradosTabla.concat(result.ListaCupos);
        }
        if (cuposGeneradosTabla.length > 0) {
            cuposCreados(cuposGeneradosTabla);
        }
        if (erroresTabla.length > 0) {
            ShowErrorMessages(errores);
        }
    });

    $("#aceptarTabla").click(function () {
        var sugerencias = ConfirmarSeleccionados();       

        for (var j = 0; j < sugerencias.length; j++) {
            for (var a = 0; a < sugerencias[j].Detalles.length; a++) {
                sugerencias[j].Detalles[a].CantidadFleteProcedencia = $("#flete" + j).val();
                sugerencias[j].Detalles[a].Cantidad = sugerencias[j].Detalles[a].Cantidad - sugerencias[j].Detalles[a].CantidadFleteProcedencia;

            }
        }
        result = MSExecuteOnServer('/SugerenciaCupo/DatosConfirmar', { datosTabla: sugerencias, materialId: $("#MaterialId").val(), centroId : $("#CentroId").val() });

        $.unblockUI();
        var errores = new Array();
        var cuposGenerados = new Array();
        if (result.HayError) {
            errores = errores.concat(result.ListaErrores);
        }
        if (result.ListaCupos != null && result.ListaCupos.length > 0) {
            cuposGenerados = cuposGenerados.concat(result.ListaCupos);
        }
        if (cuposGenerados.length > 0) {
            cuposCreados(cuposGenerados);
            
        }
        if (errores.length > 0) {
            ShowErrorMessages(errores);
        }
    });

    $("#MaterialId").change(function () {
        $("#filtrarMaterial").trigger("click");
        
    });
    $("#CentroId").change(function () {
        $("#filtrarMaterial").trigger("click");

    });

    $("#resultadoCupo").on('hidden.bs.modal', function () {
        $("#filtrarMaterial").trigger("click");
    });

}
   




