var proveedorCorredorGuardado = [];
var cantProveedorCorredor = 0;

function CrearCorredor() {
    if ($('#segmentacion :selected').parent().attr('label') === "Corredores") {        
        $(".noCorredor").hide();
        $("#datos").text("Datos corredor");
        $("#proveedores-corredor").show(); 
        armarFuncionalidadesProveedorCorredor();
        $(".formulario-footer-guardar-contacto").addClass("guardar-corredor");
        $(".formulario-footer-guardar-contacto").removeClass("guardar-proveedor");
    } else {
        $(".noCorredor").show();
        $("#datos").text("Datos proveedor");
        $("#proveedores-corredor").hide();
        $(".formulario-footer-guardar-contacto").removeClass("guardar-corredor");
        $(".formulario-footer-guardar-contacto").addClass("guardar-proveedor");
    }
}

function armarFuncionalidadesProveedorCorredor() {
    crearProcedencia();
    $("#provcorr-cuit").change(function () { buscarProveedor(); });
    guardarProveedorCorredor();
}

function crearProcedencia() {
    $("#procedencia").click(function () {
        $("#procedencia").data("kendoAutoComplete").value("");        
        $("#LocalidadCrearContrato").trigger("change");
    });

    $("#procedencia").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function (e) {
            if ($("#procedencia").val().split('|').length > 1) {
                $("#procedencia").val($("#procedencia").val().split('|')[1]);
            }
        },
        select: function (e) {
            var item = e.dataItem;
            $("#provinciacorrId").val(item.ProvinciaId);
            $("#localidadcorrId").val(item.Id);
        },
        dataSource: {
            severFiltering: true,
            serverPaging: true,
            transport: {
                read: {
                    type: 'post',
                    dataType: 'json',
                    url: "/Proveedor/BuscarLocalidades"
                },
                parameterMap: function (data, type) {
                    return { filtro: $('#procedencia').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#procedencia").removeClass("k-input");
    $("#procedencia").addClass("campo-input-text");
}
function buscarRazonSocialCorredor(val) {
    var data = { cuit: val };
    $("#provcorr-razonsocial").val("");
    $("#procedencia").val("");
    $("#provcorrId").val("");
    $("#clasificacion-proveedor-corredor").val("");
    $("#direccion-provcorr").val("");
    $("#codpost-provcorr").val("");
    $("#provinciacorrId").val("");
    $("#localidadcorrId").val("");
    var result = MSExecuteOnServer('/Proveedor/TraerRazonSocial', data);    
    $("#provcorr-razonsocial").val(result.razonSocial);
    $(".campo-estadoafip-span-operable-corredor").html(result.Operable ? "Operable" : "No Operable");
    if (result.Operable === 0) {
        $(".span-contacto-no-operable-tooltip-corredor").html(result.Condicion);
        $("#Operable-agregar").show();
    }
    if (result.Operable) {
        $("#imgOperableCorr").attr("src", "../Content/Images/operableafip.png");
        $(".campo-estadoafip-corredor").css({
            'background-color': 'rgba(150, 235, 198, 0.45)'
        });
    }
    else {
        $("#imgOperableCorr").attr("src", "../Content/Images/cancelar.png");
        $(".campo-estadoafip-corredor").css({
            'background-color': 'rgba(239, 105, 105, 0.45)'
        });
    }
}
function buscarProveedor() {
    var val = $("#provcorr-cuit").val();
    if (val !== "") {
        var data = { cuit: val };
        buscarRazonSocialCorredor(val);
        var result = MSExecuteOnServer('/Proveedor/TraerProveedorParaCorredor', data);
        if (result.Proveedor !== null) {
            $("#provcorr-razonsocial").val(result.Proveedor.RazonSocial);
            $("#procedencia").val(result.Proveedor.Localidad + " (" + result.Proveedor.Provincia + ")");
            $("#provcorrId").val(result.Proveedor.ProveedorId);
            $("#clasificacion-proveedor-corredor").val(result.Proveedor.ClasificacionCompraNetId);
            $("#direccion-provcorr").val(result.Proveedor.Direccion);
            $("#codpost-provcorr").val(result.Proveedor.CodigoPostal);
            $("#provinciacorrId").val(result.Proveedor.ProvinciaId);
            $("#localidadcorrId").val(result.Proveedor.LocalidadId);        
        }
    } else {
        limpiarCargaProveedor();
    }    
}

function limpiarCargaProveedor() {
    $("#provcorr-cuit").val("");
    $("#provcorr-razonsocial").val("");
    $("#procedencia").val("");
    $("#provcorrId").val("");
    $("#clasificacion-proveedor-corredor").val("");
    $("#direccion-provcorr").val("");
    $("#codpost-provcorr").val("");  
    $("#provinciacorrId").val("");
    $("#localidadcorrId").val("");
    $("#imgOperableCorr").attr("src", "");
    $(".campo-estadoafip-corredor").css({
        'background-color': 'rgba(150, 235, 198, 0.45)'
    });
}
function guardarProveedorCorredor() {
    $("#GuardarProveedorCorredor").click(function () {
        if (!ValidarProveedorCorredor()) {
            return false;
        }
        var obj = {};
        obj.basicos = {};
        obj.contacto = {};
        obj.basicos.cuit = $("#provcorr-cuit").val();
        obj.basicos.RazonSocial = $("#provcorr-razonsocial").val();
        if ($("#procedencia").val() !== "") {
            var localidadAux = $("#procedencia").val().split(' (');
            obj.contacto.localidadDesc = localidadAux[0];
            var provinciaAux = localidadAux[1].split(')');
            obj.contacto.provinciaDesc = provinciaAux[0];
            obj.contacto.localidad = $("#localidadcorrId").val();
            obj.contacto.provincia = $("#provinciacorrId").val();
        }
        obj.contacto.direccion = $("#direccion-provcorr").val();
        obj.contacto.codpost = $("#codpost-provcorr").val();
        obj.Clasificacion = $("#clasificacion-proveedor-corredor option:selected").text();
        obj.basicos.ClasificacionCompraNet = $("#clasificacion-proveedor-corredor").val();        
        obj.ProveedorId = $("#provcorrId").val();
        obj.ProveedorCorredorId = $("#corredorId").val();

        crearContenedoresProveedores(obj);
        
        limpiarCargaProveedor();
    });
}
function crearContenedoresProveedores(obj) {
    var htmlProveedores = "";
    htmlProveedores += '<div class="contenedor-proveedorescorredor-comercial" id="proveedor' + cantProveedorCorredor + '">' +
        '<div class="contenedor-proveedor-corredor-titulo">' +
        '<img class="img-contacto-comercial" src="../Content/Images/contprinc-cont4.png" /> ' +
        '<span class="span-contacto-comercial"> ' +
        obj.basicos.RazonSocial + ' (' + obj.basicos.cuit + ')</span>' +
        '<span class="align-right" onclick="editarProveedorComercial(' + cantProveedorCorredor + ')" id="provcorr-editar' + cantProveedorCorredor + '">' +
        '<img class="contacto-edit-img" src="../Content/Images/contacto-edit.png" /> ' +
        '<span class="editar-contacto editar-proveedor-corredor">' +
        'Editar' +
        '</span>' +
        '</span>' +
        '<span style="margin-right:5px;" onclick="eliminarProveedorComercial(this)" class="align-right" id="provcorr-eliminar' + cantProveedorCorredor + '">' +
        '<span class="editar-contacto editar-proveedor-corredor">' +
        'x Eliminar' +
        '</span>' +
        '</span>' +
        '</div>' +
        '<div class="contenedor-contacto-comercial-posicion">' +
        '<span class="contenedor-contacto-comercial-posicion-izq">' +
        (obj.contacto.direccion ? obj.contacto.direccion + ' (' + obj.contacto.codpost + ')' : "No se especifica dirección") +
        '</span>' +
        '</div>' +
        '<div class="contenedor-contacto-comercial-posicion">' +
        '<span class="contenedor-contacto-comercial-posicion-izq">' +
        (obj.contacto.localidad ? obj.contacto.localidadDesc + ', ' + obj.contacto.provinciaDesc : "No se especifica procedencia") +
        '</span>' +
        '</div>' +
        '<div class="contenedor-contacto-comercial-extras">' +
        '<div class="row">' +
        '<div class="col-lg-6">' +
        '<span class="contenedor-contacto-comercial-posicion-izq">' +
        'Clasificacion:' +
        '</span>' +
        '</div>' +
        '<div class="col-lg-6">' +
        '<span class="contenedor-contacto-comercial-posicion-der">' +
        (obj.Clasificacion ? obj.Clasificacion : "no especifica") +
        '</span>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>' +
        '</div>';
    $(".datos-proveedores-guardados").append(htmlProveedores);
    obj.item = cantProveedorCorredor;
    proveedorCorredorGuardado.push(obj);
    cantProveedorCorredor++;
}

function editarProveedorComercial(id) {
    var obj = proveedorCorredorGuardado.filter(function (el) {
        return el.item === parseInt(id);
    });
    obj = obj[0];
    $("#provcorr-cuit").val(obj.basicos.cuit);
    $("#provcorr-razonsocial").val(obj.basicos.RazonSocial);
    obj.contacto.localidad? $("#procedencia").val(obj.contacto.localidadDesc + " (" + obj.contacto.provinciaDesc + ")"):"";
    $("#provcorrId").val(obj.ProveedorId);
    $("#clasificacion-proveedor-corredor").val(obj.basicos.ClasificacionCompraNet);
    $("#direccion-provcorr").val(obj.contacto.direccion);
    $("#codpost-provcorr").val(obj.contacto.codpost);
    $("#corredorId").val(obj.ProveedorCorredorId);
    $("#proveedor" + id).remove();
    obj.contacto.localidad = $("#localidadcorrId").val();
    obj.contacto.provincia = $("#provinciacorrId").val();
    proveedorCorredorGuardado = proveedorCorredorGuardado.filter(function (el) {
        return el.item !== parseInt(id);
    });
}
function eliminarProveedorComercial(val) {
    var item = $(val).attr("id").split("provcorr-eliminar")[1];
    $("#proveedor" + item).remove();
    proveedorCorredorGuardado = proveedorCorredorGuardado.filter(function (el) {
        return el.item !== parseInt(item);
    });
}
function ValidarProveedorCorredor() {
    if ($("#provcorr-cuit").val() === "") {
        MensErr("El CUIT no debe estar vacio");
        return false;
    }
    if ($("#provcorr-razonsocial").val() === "") {
        MensErr("La Razón Social no debe estar vacia");
        return false;
    }
    return true;
}

function armarEditCorredor() {
    var datos = { ProveedorId: ProveedorId };
    var proveedores = MSExecuteOnServer('/Proveedor/TraerProveedoresCorredor', datos);
    if (proveedores !== null) {
        for (var i = 0; i < proveedores.length; i++) {
            var proveedor = {};
            proveedor.basicos = {};
            proveedor.contacto = {};
            proveedor.produccion = {};
            proveedor.basicos.RazonSocial = proveedores[i].RazonSocial;
            proveedor.basicos.cuit = proveedores[i].CUIT;
            proveedor.contacto.direccion = proveedores[i].Direccion;
            proveedor.contacto.codpost = proveedores[i].CodigoPostal;
            proveedor.contacto.localidad = proveedores[i].LocalidadId;
            proveedor.contacto.provincia = proveedores[i].ProvinciaId;
            proveedor.contacto.localidadDesc = proveedores[i].Localidad;
            proveedor.contacto.provinciaDesc = proveedores[i].Provincia;
            proveedor.basicos.ClasificacionCompraNet = proveedores[i].ClasificacionCompraNetId;
            proveedor.Clasificacion = proveedores[i].ClasificacionDescripcion;
            proveedor.ProveedorId = proveedores[i].ProveedorId;
            proveedor.ProveedorCorredorId = proveedores[i].ProveedorCorredorId;
            crearContenedoresProveedores(proveedor);
        }
    }
}

function DatosCorredor() {
    var obj = {};
    obj.basicos = {};
    obj.contacto = {};
    obj.contactocomercial = {};
    obj.proveedoresCorredor = {};

    obj.basicos.cuit = $("#cuit").val();
    obj.basicos.razonsocial = $("#razonsocial").val();
    obj.basicos.nocliente = $("#nocliente").is(":checked") ? "1" : "0";
    obj.basicos.segmentacion = $("#segmentacion").val();
    obj.basicos.calificacion = $("#calificacion").val();
    if (obj.basicos.calificacion) {
        obj.basicos.calificacion = obj.basicos.calificacion[0];
    }
    obj.basicos.ClasificacionCompraNet = $("#clasificacion-compranet").val();
    obj.basicos.BoletoCompraNet = $("#boleto-compranet").val();
    obj.basicos.BolsaCompraNet = $("#bolsa-compranet").val();
    obj.basicos.ProvinciaCompraNet = $("#provincia-compranet").val();
    obj.basicos.LocalidadCompraNet = $("#localidad-compranet").val();
    obj.basicos.Consignatario = $("#consignatario-compranet").is(":checked");
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
    } else {
        obj.contacto.areaDeInfluencia = null;
    }
    obj.contactocomercial = aGuardarContactoComercial;
    obj.proveedorCorredor = proveedorCorredorGuardado;
    GrabarCorredor(obj);
}
function GrabarCorredor(nuevoCorredor) {
    if (ProveedorId) {
        nuevoCorredor.CorredorId = ProveedorId;
    }
    var result = MSExecuteOnServer('/Proveedor/GrabarCorredor', nuevoCorredor);
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