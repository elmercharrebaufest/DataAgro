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
    crearProcedenciaCompraNet();
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
function crearProcedenciaCompraNet() {
    $("#procedenciaCompranet").click(function () {
        $("#procedenciaCompranet").data("kendoAutoComplete").value("");
        $("#LocalidadCrearContrato").trigger("change");
    });

    $("#procedenciaCompranet").kendoAutoComplete({
        template: '<p class="buscar-nomb" >#: data.Localidad # (#: data.Provincia#)</p>',
        minLength: 3,
        enforceMinLength: true,
        dataTextField: "Filtro",
        dataValueField: "Filtro",
        filter: "contains",
        change: function (e) {
            if ($("#procedenciaCompranet").val().split('|').length > 1) {
                $("#procedenciaCompranet").val($("#procedenciaCompranet").val().split('|')[1]);
            }
        },
        select: function (e) {
            var item = e.dataItem;
            $("#provinciacorrCompranetId").val(item.ProvinciaId);
            $("#localidadcorrCompranetId").val(item.Id);
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
                    return { filtro: $('#procedenciaCompranet').val() };
                }
            }
        },
        filtering: function (e) {
            if (!e.filter.value) {
                e.preventDefault();
            }
        }
    });

    $("#procedenciaCompranet").removeClass("k-input");
    $("#procedenciaCompranet").addClass("campo-input-text");
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
    if (result.Operable === 0) {
        $(".span-contacto-no-operable-tooltip-corredor").html(result.Condicion);
        $("#Operable-agregar-corredor").show();
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
            $("#procedenciaCompranet").val(result.Proveedor.LocalidadCompraNet + " (" + result.Proveedor.ProvinciaCompraNet + ")");
            $("#provcorrId").val(result.Proveedor.ProveedorId);
            $("#clasificacion-proveedor-corredor").val(result.Proveedor.ClasificacionCompraNetId);
            $("#direccion-provcorr").val(result.Proveedor.Direccion);
            $("#codpost-provcorr").val(result.Proveedor.CodigoPostal);
            $("#provinciacorrId").val(result.Proveedor.ProvinciaId);
            $("#localidadcorrId").val(result.Proveedor.LocalidadId);
            $("#provinciacorrCompranetId").val(result.Proveedor.ProvinciaCompraNetId);
            $("#localidadcorrCompranetId").val(result.Proveedor.LocalidadCompraNetId);
            MostrarConsignatarioEnBuscarProveedor(result.Proveedor.ClasificacionCompraNetId);
            console.log(result.Proveedor.Consignatario);
            $("#consignatario-proveedor-compranet").prop("checked", result.Proveedor.Consignatario);

        }
    } else {
        limpiarCargaProveedor();
    }
}

function limpiarCargaProveedor() {
    $("#provcorr-cuit").val("");
    $("#provcorr-razonsocial").val("");
    $("#procedencia").val("");
    $("#procedenciaCompranet").val("");
    $("#provcorrId").val("");
    $("#clasificacion-proveedor-corredor").val("");
    $("#direccion-provcorr").val("");
    $("#codpost-provcorr").val("");
    $("#provinciacorrId").val("");
    $("#localidadcorrId").val("");
    $("#provinciacorrCompranetId").val("");
    $("#localidadcorrCompranetId").val("");
    $("#imgOperableCorr").attr("src", "");
    MostrarConsignatarioEnBuscarProveedor(0);
    $("#consignatario-proveedor-compranet").prop("checked", false);
    $(".campo-estadoafip-corredor").css({
        'background-color': 'rgba(150, 235, 198, 0.45)'
    });
}

function MostrarConsignatarioEnBuscarProveedor(clasificacionId) {
    console.log(clasificacionId);
    if (clasificacionId != 2) {
        $("#consignatarioProveedorDiv").hide();
    } else {
        $("#consignatarioProveedorDiv").show();
    }
}

function guardarProveedorCorredor() {
    $("#GuardarProveedorCorredor").click(function () {
        if (!ValidarProveedorCorredor()) {
            return false;
        } else {
            var obj = {};
            obj.basicos = {};
            obj.contacto = {};
            obj.basicos.cuit = $("#provcorr-cuit").val();
            obj.basicos.RazonSocial = $("#provcorr-razonsocial").val();
            obj.basicos.Alias = $("#Alias").val();
            obj.basicos.Deshabilitado = $("#deshabilitado").is(":checked");

            if ($("#procedencia").val() !== "") {
                var localidadAux = $("#procedencia").val().split(' (');
                obj.contacto.localidadDesc = localidadAux[0];
                var provinciaAux = localidadAux[1].split(')');
                obj.contacto.provinciaDesc = provinciaAux[0];
                obj.contacto.localidad = $("#localidadcorrId").val();
                obj.contacto.provincia = $("#provinciacorrId").val();
            }
            if ($("#procedenciaCompranet").val() !== "") {
                var localidadcnAux = $("#procedenciaCompranet").val().split(' (');
                obj.basicos.localidadCompraNetDesc = localidadcnAux[0];
                var provinciacnAux = localidadcnAux[1].split(')');
                obj.basicos.provinciaCompraNetDesc = provinciacnAux[0];
                obj.basicos.ProvinciaCompraNet = $("#provinciacorrCompranetId").val();
                obj.basicos.LocalidadCompraNet = $("#localidadcorrCompranetId").val();
            }
            obj.contacto.direccion = $("#direccion-provcorr").val();
            obj.contacto.codpost = $("#codpost-provcorr").val();
            obj.Clasificacion = $("#clasificacion-proveedor-corredor option:selected").text();
            obj.basicos.ClasificacionCompraNet = $("#clasificacion-proveedor-corredor").val();
            obj.ProveedorId = $("#provcorrId").val();
            obj.ProveedorCorredorId = $("#corredorId").val();
            obj.basicos.Consignatario = $("#consignatario-proveedor-compranet").is(":checked");

            crearContenedoresProveedores(obj);

            limpiarCargaProveedor();
        }
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
        '<span class="contenedor-contacto-comercial-posicion-der">' +
        'Procedencia CompraNet: <br />' + (obj.basicos.localidadCompraNetDesc ? obj.basicos.localidadCompraNetDesc + ', ' + obj.basicos.provinciaCompraNetDesc : "No se especifica") +
        '</span>' +
        '</div>' +
        '<div class="col-lg-6">' +
        '<span class="contenedor-contacto-comercial-posicion-der">' +
        'Clasificacion: ' + (obj.Clasificacion ? obj.Clasificacion : "no especifica") +
        '</span>' +
        '<div class="">' +
        '<span class="contenedor-contacto-comercial-posicion-der">' +
        'Consignatario: ' + (obj.basicos.Consignatario ? "SI" : "NO") +
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
    obj.contacto.localidad ? $("#procedencia").val(obj.contacto.localidadDesc + " (" + obj.contacto.provinciaDesc + ")") : "";
    obj.basicos.LocalidadCompraNet ? $("#procedenciaCompranet").val(obj.basicos.localidadCompraNetDesc + " (" + obj.basicos.provinciaCompraNetDesc + ")") : "";
    $("#provcorrId").val(obj.ProveedorId);
    $("#clasificacion-proveedor-corredor").val(obj.basicos.ClasificacionCompraNet);
    $("#consignatario-proveedor-compranet").prop('checked', obj.basicos.Consignatario);
    mostrarConsignatarioProveedor();
    $("#direccion-provcorr").val(obj.contacto.direccion);
    $("#codpost-provcorr").val(obj.contacto.codpost);
    $("#corredorId").val(obj.ProveedorCorredorId);
    $("#proveedor" + id).remove();
    $("#localidadcorrId").val(obj.contacto.localidad);
    $("#provinciacorrId").val(obj.contacto.provincia);
    $("#localidadcorrCompraNetId").val(obj.basicos.LocalidadCompraNet);
    $("#provinciacorrCompraNetId").val(obj.basicos.ProvinciaCompraNet);
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

    var repetidos = !proveedorCorredorGuardado.some(function (elemento) {
        if (elemento.basicos.cuit === $("#provcorr-cuit").val()) {
            MensErr("No se puede ingresar dos proveedores iguales");
        }
        return elemento.basicos.cuit === $("#provcorr-cuit").val();
    });

    return repetidos;
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
            proveedor.basicos.Alias = proveedores[i].Alias;
            proveedor.basicos.cuit = proveedores[i].CUIT;
            proveedor.contacto.direccion = proveedores[i].Direccion;
            proveedor.contacto.codpost = proveedores[i].CodigoPostal;
            proveedor.contacto.localidad = proveedores[i].LocalidadId;
            proveedor.contacto.provincia = proveedores[i].ProvinciaId;
            proveedor.contacto.localidadDesc = proveedores[i].Localidad;
            proveedor.contacto.provinciaDesc = proveedores[i].Provincia;
            proveedor.basicos.LocalidadCompraNet = proveedores[i].LocalidadCompraNetId;
            proveedor.basicos.ProvinciaCompraNet = proveedores[i].ProvinciaCompraNetId;
            proveedor.basicos.localidadCompraNetDesc = proveedores[i].LocalidadCompraNet;
            proveedor.basicos.provinciaCompraNetDesc = proveedores[i].ProvinciaCompraNet;
            proveedor.basicos.ClasificacionCompraNet = proveedores[i].ClasificacionCompraNetId;
            proveedor.basicos.Consignatario = proveedores[i].Consignatario;
            proveedor.Clasificacion = proveedores[i].ClasificacionDescripcion;
            proveedor.ProveedorId = proveedores[i].ProveedorId;
            proveedor.ProveedorCorredorId = proveedores[i].ProveedorCorredorId;
            crearContenedoresProveedores(proveedor);
        }
    }
}

function DatosCorredor() {
    if ($("#concom-nombre").val() !== ""
        || $("#concom-apellido").val() !== ""
        || $("#concom-email1").val() !== ""
        || !$("#concom-fechanacimientodia").val()
        || !$("#concom-fechanacimientomes").val()
        || !$("#concom-anionacimientomes").val()
        || $("#concom-cargo").val() !== ""
        || $("#concom-puesto").val() !== ""
        || $("#concom-cargo").val() !== "") {
        $("#GuardarContactoComercial").trigger("click");
    }
    if ($("#provcorr-cuit").val() !== ""
        || $("#provcorr-razonsocial").val() !== ""
        || $("#procedencia").val() !== ""
        || $("#direccion-provcorr").val() !== ""
        || $("#codpost-provcorr").val() !== ""
        || $("#procedenciaCompranet").val() !== ""
        || ($("#clasificacion-proveedor-corredor").val() !== "null"
            && $("#clasificacion-proveedor-corredor").val() !== null)) {
        $("#GuardarProveedorCorredor").trigger("click");
    }
    var obj = {};
    obj.basicos = {};
    obj.contacto = {};
    obj.contactocomercial = {};

    obj.basicos.cuit = $("#cuit").val();
    obj.basicos.razonsocial = $("#razonsocial").val();
    obj.basicos.Alias = $("#Alias").val();
    obj.basicos.Deshabilitado = $("#deshabilitado").is(":checked");
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
    obj.basicos.Comision = Number($("#comision-compranet").val().replace(',', '.'));
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
            if (result.CorredorId !== null || result.CorredorId !== 0) {
                BootstrapDialog.show({
                    title: 'Error !!!',
                    cssClass: 'error-dialog modal-superior',
                    message: "Corredor grabado correctamente. Revisar Proveedores: <br/>" + result.Errores[0].Message,
                    draggable: true,
                    buttons: [{
                        label: 'Cerrar',
                        cssClass: 'k-button',
                        action: function (dialogItself) {
                            dialogItself.close();
                        }
                    }],
                    onhide: function (dialogRef) {
                        if (result.Errores[0].Source == "SISA") {
                            window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
                        } else {
                            window.location.href = window.location.origin + "/Proveedor/Agregar?ProveedorId=" + result.ProveedorId;
                        }
                    }
                });
                
                setTimeout(function () {
                    if (result.Errores[0].Source == "SISA") {
                        window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
                    } else {
                        window.location.href = window.location.origin + "/Proveedor/Agregar?ProveedorId=" + result.ProveedorId;
                    }
                }, 5000);
                
            } else {
                MensErr(result.Errores[0].Message);
            }            
        }
        else {
            MensInfo("Se ha realizado la operacion con exito");
            window.location.href = window.location.origin + "/Proveedor/Detalle?ProveedorId=" + result.ProveedorId;
        }
    }
}