var resultEdit = {};
$(document).ready(function () {
    if (ProveedorId) {
        InicializarEdit();
    }
});

function InicializarEdit() {
    var datos = { ProveedorId: ProveedorId };
    resultEdit = MSExecuteOnServer('/Proveedor/TraerProveedor', datos);
    var actividad = resultEdit.ActividadTraerPorProveedores;
    var actividadhistoria = resultEdit.ActividadHistoriaTraerPorProveedores;
    var basico = resultEdit.BasicoProveedorTraerPorProveedores;
    var campoacopio = resultEdit.CampoProduccionAcopioPorProveedores;
    var comerciales = resultEdit.ContactosComercialesTraerPorProveedores;
    var acopio = resultEdit.Acopio;
    var objetivo = resultEdit.ObjetivosTraerPorProveedorId;
    var acopiomaterial = resultEdit.AcopioMaterialPorProveedores;
    armarBasico(basico[0]);
    armarComercial(comerciales);
    armarProduccion(campoacopio);
    armarAlmacenamiento(acopio, acopiomaterial);
    armarObjetivos(objetivo);
}

function armarBasico(basico) {
    $("#cuit").val(basico.CUIT);
    $("#cuit").trigger("keyup");
    $("#razonsocial").val(basico.RazonSocial);
    $("#segmentacion").val($('#segmentacion option').filter(function () { return $(this).html() == basico.Segmentacion; }).val());
    $("#Operable-agregar").val(basico.Operable);
    if (basico.Estado === "Sin interés de operar") {
        $("#nocliente").attr("checked", true);
    } else
        $("#nocliente").attr("checked", false);

    $("#calificacion").val(basico.Calificacion);
    $("#calificacion").multiselect("refresh");
    $("#comentario").val(basico.Observaciones);

    if (basico.Provincia) {
        $("#provincia").val($("#provincia option").filter(function () { return $(this).html() == basico.Provincia; }).val());
        $("#provincia").trigger("change");
    }

    $("#localidad").val($("#localidad option").filter(function () { return $(this).html() == basico.Localidad; }).val());
    $("#direccion").val(basico.Direccion);
    $("#codpost").val(basico.CodigoPostal);

    var canalesOperacionId = [];
    for (var ii in resultEdit.CanalesDeOperacion) {
        (function (i) {
            canalesOperacionId.push(resultEdit.CanalesDeOperacion[i].CanalOperacionId);
        })(ii);
    }
    $("#canopera").val(canalesOperacionId);
    $("#canopera").multiselect("refresh");

    var entregaAId = [];
    for (var ii in resultEdit.ProveedorDestinatario) {
        (function (i) {
            entregaAId.push(resultEdit.ProveedorDestinatario[i].DestinatarioId);
        })(ii);
    }

    $("#entregaA").val(entregaAId);
    $("#entregaA").multiselect("refresh");

    var condPreferenteId = [];
    for (var ii in resultEdit.ProveedorCondicion) {
        (function (i) {
            condPreferenteId.push(resultEdit.ProveedorCondicion[i].CondicionId);
        })(ii);
    }
    $("#condPrefer").val(condPreferenteId);
    $("#condPrefer").multiselect("refresh");

    if (basico.AreaInfluencia == "unica") {
        $("#influencia-zona-unica").prop("checked", true);
    } else if (basico.AreaInfluencia == "multizona") {
        $("#influencia-multizona").prop("checked", true);
    } else {
        $("#influencia-zona-unica").prop("checked", false);
        $("#influencia-multizona").prop("checked", false);
    }

    if (basico.ProvinciaCompraNet) {
        $("#provincia-compranet").val($("#provincia option").filter(function () { return $(this).html() == basico.ProvinciaCompraNet; }).val());
        $("#provincia-compranet").trigger("change");
    }
    if (basico.LocalidadCompraNet) {
        $("#localidad-compranet").val($("#localidad-compranet option").filter(function () { return $(this).html() == basico.LocalidadCompraNet; }).val());
        $("#localidad-compranet").trigger("change");
    }
    if (basico.ClasificacionCompraNet) {
        $("#clasificacion-compranet").val($("#clasificacion-compranet option").filter(function () { return $(this).html() == basico.ClasificacionCompraNet; }).val());
        $("#clasificacion-compranet").trigger("change");
    }

    if (basico.BoletoCompraNet) {
        $("#boleto-compranet").val($("#boleto-compranet option").filter(function () { return $(this).html() == basico.BoletoCompraNet; }).val());
        $("#boleto-compranet").trigger("change");
    }
    if (basico.BolsaCompraNet) {
        $("#bolsa-compranet").val($("#bolsa-compranet option").filter(function () { return $(this).html() == basico.BolsaCompraNet; }).val());
    } $("#bolsa-compranet").trigger("change");

    if (basico.Consignatario) {
        $("#consignatario-compranet").prop("checked", true);
    }
}

function armarComercial(comerciales) {
    var htmlComerciales = "";
    for (var ii in comerciales) {
        (function (i) {
            cantContactoComercial++;

            var obj = {};
            obj.contactoComercialId = comerciales[i].ContactoComercialId; //TODO: Poner el id que va
            obj.nombre = comerciales[i].Nombres;
            obj.apellido = comerciales[i].Apellido;
            obj.emails = [];
            if (comerciales[i].Email1)
                obj.emails.push(comerciales[i].Email1);
            else
                obj.emails.push(null);
            if (comerciales[i].Email2)
                obj.emails.push(comerciales[i].Email2);
            else
                obj.emails.push(null);
            if (comerciales[i].Email3)
                obj.emails.push(comerciales[i].Email3);
            else
                obj.emails.push(null);
            obj.telefonos = [];
            if (comerciales[i].Telefono1)
                obj.telefonos.push({
                    tipoTelefono: comerciales[i].TipoTelefono1Id,
                    telefono: comerciales[i].Telefono1
                });
            else
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            if (comerciales[i].Telefono2)
                obj.telefonos.push({
                    tipoTelefono: comerciales[i].TipoTelefono2Id,
                    telefono: comerciales[i].Telefono2
                });
            else
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            if (comerciales[i].Telefono3)
                obj.telefonos.push({
                    tipoTelefono: comerciales[i].TipoTelefono1Id,
                    telefono: comerciales[i].Telefono3
                });
            else
                obj.telefonos.push({
                    tipoTelefono: null,
                    telefono: null
                });
            obj.fechaNacimiento = comerciales[i].FechaNacimiento ? kendo.parseDate(comerciales[i].FechaNacimiento) : null;

            obj.dia = obj.fechaNacimiento ? obj.fechaNacimiento.getDate() : "null";
            obj.mes = obj.fechaNacimiento ? obj.fechaNacimiento.getMonth() + 1 : "null";
            obj.anio = obj.fechaNacimiento ? obj.fechaNacimiento.getFullYear() : "null";
            obj.cargo = comerciales[i].Cargo;
            obj.puesto = comerciales[i].Puesto;
            obj.intereses = comerciales[i].InteresId ? comerciales[i].InteresId.split(", ") : [];
            obj.interesesNombre = comerciales[i].Interes ? comerciales[i].Interes.split(", ") : "";
            obj.otrosIntereses = comerciales[i].OtrosIntereses ? comerciales[i].OtrosIntereses : "";
            obj.principal = comerciales[i].EsPrincipal;
            obj.CompraNet = comerciales[i].CompraNet;
            obj.item = cantContactoComercial;
            aGuardarContactoComercial.push(obj);

            htmlComerciales += '<div class="contenedor-contacto-comercial" id="comercial' + cantContactoComercial + '">' +
                '<div class="contenedor-contacto-comercial-titulo">' +
                '<img class="img-contacto-comercial" src="../Content/Images/contprinc-cont4.png" /> ' +
                '<span class="span-contacto-comercial"> ' +
                comerciales[i].Nombres + ' ' + comerciales[i].Apellido +
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
                (comerciales[i].Cargo ? comerciales[i].Cargo : "No especifica cargo") + ' - ' +
                '</span>' +
                '<span class="contenedor-contacto-comercial-posicion-der">' +
                (comerciales[i].Puesto ? comerciales[i].Puesto : "No especifica puesto") +
                '</span>' +
                '</div>' +
                '<div class="contenedor-contacto-comercial-telefonos">' +
                (comerciales[i].Telefono1 ? comerciales[i].Telefono1 + (comerciales[i].Telefono2 ? " - " + comerciales[i].Telefono2 : "") + (comerciales[i].Telefono3 ? " - " + comerciales[i].Telefono3 : "") : "No especifica teléfono") +
                '</div>' +
                '<div class="contenedor-contacto-comercial-mails">' +
                (comerciales[i].Email1 ? comerciales[i].Email1 + (comerciales[i].Email2 ? " - " + comerciales[i].Email2 : "") + (comerciales[i].Email3 ? " - " + comerciales[i].Email3 : "") : "No especifica mails") +
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
                (comerciales[i].FechaNacimiento ? kendo.toString(kendo.parseDate(comerciales[i].FechaNacimiento), "m") + " de " + kendo.toString(kendo.parseDate(comerciales[i].FechaNacimiento), "yyyy") : "no especifica") +
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
                (comerciales[i].Interes ? comerciales[i].Interes.split(",").join("<br>") + "<br>" + (comerciales[i].OtrosIntereses ? comerciales[i].OtrosIntereses : "") : (comerciales[i].OtrosIntereses ? comerciales[i].OtrosIntereses : "")) +
                '</span>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>';
        })(ii);
    }
    $(".datos-contactocomercial-guardados").append(htmlComerciales);
}

function armarProduccion(campoacopio) {
    var grupocampoacopio = {};

    if (campoacopio.length > 0) {
        $("#tons-max-aprob-sojasust").val(campoacopio[0].AlmacTonsMaxSojaSust);

        $("#has-aprob-sojasust").val(campoacopio[0].AlmacHectSojaSust);
    }

    for (var ii in campoacopio) {
        (function (i) {
            var obj = {};
            obj.provincia = campoacopio[i].Provincia;
            obj.localidad = campoacopio[i].Localidad;

            grupocampoacopio["Campo" + campoacopio[i].Id] = grupocampoacopio["Campo" + campoacopio[i].Id] || {};

            grupocampoacopio["Campo" + campoacopio[i].Id].ArrendadoPropio = campoacopio[i].ArrendadoPropio;
            grupocampoacopio["Campo" + campoacopio[i].Id].EsCampoProduccion = campoacopio[i].EsCampoProduccion;
            grupocampoacopio["Campo" + campoacopio[i].Id].HabilitadoSojaSustentable = campoacopio[i].HabilitadoSojaSustentable;
            grupocampoacopio["Campo" + campoacopio[i].Id].CampoId = campoacopio[i].Id;
            grupocampoacopio["Campo" + campoacopio[i].Id].Localidad = campoacopio[i].Localidad;
            grupocampoacopio["Campo" + campoacopio[i].Id].Provincia = campoacopio[i].Provincia;
            grupocampoacopio["Campo" + campoacopio[i].Id].LocalidadId = campoacopio[i].LocalidadId;
            grupocampoacopio["Campo" + campoacopio[i].Id].ProvinciaId = campoacopio[i].ProvinciaId;
            grupocampoacopio["Campo" + campoacopio[i].Id].Coordenadas = campoacopio[i].Coordenadas;
            grupocampoacopio["Campo" + campoacopio[i].Id].Granos = grupocampoacopio["Campo" + campoacopio[i].Id].Granos || [];
            grupocampoacopio["Campo" + campoacopio[i].Id].Granos.push({
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

    for (var ii in grupocampoacopio[0]) {
        (function (i) {
            var obj = {};

            console.log("asdasdf", obj);

            obj.item = capProdCant;

            obj.provincia = grupocampoacopio[0][i].ProvinciaId;
            obj.provinciaNom = grupocampoacopio[0][i].Provincia;

            obj.localidad = grupocampoacopio[0][i].LocalidadId;
            obj.localidadNom = grupocampoacopio[0][i].Localidad;

            obj.archivo = grupocampoacopio[0][i].KMZnombre;

            obj.archivoFileResult = grupocampoacopio[0][i].KMZfile;

            obj.coordenadas = grupocampoacopio[0][i].Coordenadas;

            obj.CampoId = grupocampoacopio[0][i].CampoId;

            obj.hectareas = grupocampoacopio[0][i].ArrendadoPropio == true ? 1 : 0;

            obj.granos = [];

            var html = "";
            html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="granocontenedor' + capProdCant + '">'
                + '<div>'
                + '<div class="datos-produccion-cap-prod-guardados-zona">'
                + grupocampoacopio[0][i].Provincia + ", " + grupocampoacopio[0][i].Localidad
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
                + '(Has ' + (grupocampoacopio[0][i].ArrendadoPropio == true ? "Propias" : (grupocampoacopio[0][i].ArrendadoPropio === false ? "Arrendadas" : "no especificadas")) + ')'
                + '</div>'
                + '<div class="granos-contenedor">';

            for (var jj in grupocampoacopio[0][i].Granos) {
                (function (j) {
                    if (grupocampoacopio[0][i].Granos[j].CampañaId || grupocampoacopio[0][i].Granos[j].MaterialId) {
                        obj.granos.push({
                            granoId: grupocampoacopio[0][i].Granos[j].MaterialId,
                            grano: grupocampoacopio[0][i].Granos[j].Material,
                            campañaId: grupocampoacopio[0][i].Granos[j].CampañaId,
                            campaña: grupocampoacopio[0][i].Granos[j].Campaña,
                            hectareas: grupocampoacopio[0][i].Granos[j].HectareasPorcentaje,
                            toneladas: grupocampoacopio[0][i].Granos[j].Toneladas
                        });
                    }

                    html += '<div class="granos-contenedor-grupo">'
                        + '<div class="granos-contenedor-titulo">'
                        + "Campaña " + (grupocampoacopio[0][i].Granos[j].Campaña ? grupocampoacopio[0][i].Granos[j].Campaña : "no especificada") + ": " + (grupocampoacopio[0][i].Granos[j].Material ? grupocampoacopio[0][i].Granos[j].Material : "no se especificó material")
                        + '</div>'
                        + '<div class="granos-contenedor-has-tns">'
                        + '<b>' + (grupocampoacopio[0][i].Granos[j].HectareasPorcentaje ? grupocampoacopio[0][i].Granos[j].HectareasPorcentaje : "No especifica") + "</b> Has - <b>" + (grupocampoacopio[0][i].Granos[j].Toneladas ? grupocampoacopio[0][i].Granos[j].Toneladas : "No especifica") + "</b> TNs"
                        + '</div>'
                        + '</div>'
                })(jj);
            }

            html += '</div>'
                + '</div>';

            $(".datos-produccion-cap-prod-guardados").append(html);
            $(".datos-produccion-cap-prod-guardados").show();
            aGuardar.push(obj);
            capProdCant++;
        })(ii);
    }
}

function armarAlmacenamiento(acopio, acopiomaterial) {
    var grupoacopio = {};

    if (acopio.length > 0) {
        $("#volumen-anual-total-tns").val(acopio[0].AlmacVolAnualTotal);

        if (acopio[0].AlmacHabilitadoSojaSust == true) {
            $("#sojasust-si").prop("checked", true);
            $("#sojasust-no").prop("checked", false);
        } else {
            $("#sojasust-si").prop("checked", false);
            $("#sojasust-no").prop("checked", true);
        }
    }

    for (var ii in acopio) {
        (function (i) {
            grupoacopio["Acopio" + acopio[i].Id] = grupoacopio["Acopio" + acopio[i].Id] || {};
            grupoacopio["Acopio" + acopio[i].Id].HasArrendadas = acopio[i].HasArrendadas;

            grupoacopio["Acopio" + acopio[i].Id].EsCampoProduccion = acopio[i].EsCampoProduccion;
            grupoacopio["Acopio" + acopio[i].Id].HabilitadoSojaSustentable = acopio[i].HabilitadoSojaSustentable;
            grupoacopio["Acopio" + acopio[i].Id].CampoId = acopio[i].Id;
            grupoacopio["Acopio" + acopio[i].Id].Localidad = acopio[i].Localidad;
            grupoacopio["Acopio" + acopio[i].Id].Provincia = acopio[i].Provincia;
            grupoacopio["Acopio" + acopio[i].Id].LocalidadId = acopio[i].LocalidadId;
            grupoacopio["Acopio" + acopio[i].Id].ProvinciaId = acopio[i].ProvinciaId;
            grupoacopio["Acopio" + acopio[i].Id].Coordenadas = acopio[i].Coordenadas;
            grupoacopio["Acopio" + acopio[i].Id].Granos = grupoacopio["Acopio" + acopio[i].Id].Granos || [];
            grupoacopio["Acopio" + acopio[i].Id].Granos.push({
                HectareasPorcentaje: acopio[i].HectareasPorcentaje,
                Material: acopio[i].Material,
                MaterialId: acopio[i].MaterialId,
                Toneladas: acopio[i].Toneladas,
                Campaña: acopio[i].Campaña,
                CampañaId: acopio[i].CampañaId,
                hasArrendadas: acopio[i].HasArrendadas
            });
        })(ii);
    }

    for (var ii in acopiomaterial) {
        (function (i) {
            grupoacopio["Acopio" + acopiomaterial[i].AcopioId] = grupoacopio["Acopio" + acopiomaterial[i].AcopioId] || {};
            grupoacopio["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento = grupoacopio["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento || [];
            grupoacopio["Acopio" + acopiomaterial[i].AcopioId].GranosAlmacenamiento.push({
                campañaId: acopiomaterial[i].CampañaId,
                campaña: acopiomaterial[i].Campaña,
                granoId: acopiomaterial[i].MaterialId,
                grano: acopiomaterial[i].Material,
                toneladasAlmacenamiento: acopiomaterial[i].Toneladas,
                CampoId: acopiomaterial[i].AcopioId
            });
        })(ii);
    }

    grupoacopio = [grupoacopio];

    console.log("GURpo", grupoacopio);

    for (var ii in grupoacopio[0]) {
        (function (i) {
            var obj = {};

            obj.hectareas = grupoacopio[0][i].ArrendadoPropio;
            obj.item = capProdCantAlmacenamiento;

            obj.localidad = grupoacopio[0][i].LocalidadId;
            obj.localidadNom = grupoacopio[0][i].Localidad;

            obj.provincia = grupoacopio[0][i].ProvinciaId;
            obj.provinciaNom = grupoacopio[0][i].Provincia;

            obj.archivo = grupoacopio[0][i].KMZnombre;

            obj.archivoFileResult = grupoacopio[0][i].KMZfile;

            obj.coordenadasAlmacenamiento = grupoacopio[0][i].Coordenadas;

            obj.CampoId = grupoacopio[0][i].CampoId;

            obj.granosAlmacenamiento = [];
            obj.granosAlmacenamientoGrano = [];

            var html = "";
            html += '<div class="datos-produccion-cap-prod-guardados-contenedor" id="granocontenedoralmacenamiento' + capProdCantAlmacenamiento + '">'
                + '<div>'
                + '<div class="datos-produccion-cap-prod-guardados-zona">'
                + grupoacopio[0][i].Provincia + ", " + grupoacopio[0][i].Localidad
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

            console.log(grupoacopio[0][i].Granos);
            for (var jj in grupoacopio[0][i].Granos) {
                (function (j) {
                    if (grupoacopio[0][i].Granos[j].CampañaId) {
                        obj.granosAlmacenamiento.push({
                            campañaId: grupoacopio[0][i].Granos[j].CampañaId,
                            campaña: grupoacopio[0][i].Granos[j].Campaña,
                            toneladasAlmacenamiento: grupoacopio[0][i].Granos[j].Toneladas,
                            hasArrendadas: grupoacopio[0][i].Granos[j].hasArrendadas ? 1 : 0
                        });

                        html += '<div class="granos-contenedor-grupo">'
                            + '<div class="granos-contenedor-titulo">'
                            + 'Campaña ' + grupoacopio[0][i].Granos[j].Campaña + ": " + grupoacopio[0][i].Granos[j].Material
                            + '</div>'
                            + '<div class="granos-contenedor-has-tns">'
                            + '<b>' + grupoacopio[0][i].Granos[j].Toneladas + "</b> Tns - " + (grupoacopio[0][i].Granos[j].hasArrendadas == 1 ? "Alquiladas" : "Propias")
                            + '</div>'
                            + '</div>'
                    }
                })(jj);
            }

            for (var jj in grupoacopio[0][i].GranosAlmacenamiento) {
                (function (j) {
                    if (grupoacopio[0][i].GranosAlmacenamiento[j].campañaId) {
                        obj.granosAlmacenamientoGrano.push({
                            granoId: grupoacopio[0][i].GranosAlmacenamiento[j].granoId,
                            granoAlmacenamiento: grupoacopio[0][i].GranosAlmacenamiento[j].granoAlmacenamiento,
                            campañaId: grupoacopio[0][i].GranosAlmacenamiento[j].campañaId,
                            campaña: grupoacopio[0][i].GranosAlmacenamiento[j].campaña,
                            toneladasAlmacenamiento: grupoacopio[0][i].GranosAlmacenamiento[j].toneladasAlmacenamiento,
                        });
                        console.log("grupoacopio[0][i].GranosAlmacenamiento[j]", grupoacopio[0][i].GranosAlmacenamiento[j]);
                        html += '<div class="granos-contenedor-grupo">'
                            + '<div class="granos-contenedor-titulo">'
                            + 'Campaña ' + (grupoacopio[0][i].GranosAlmacenamiento[j].campañaId != "null" ? grupoacopio[0][i].GranosAlmacenamiento[j].campaña : "no especifica") + ": " //+ obj.granosAlmacenamiento[j].granoAlmacenamiento
                            + '</div>'
                            + '<div class="granos-contenedor-has-tns">'
                            + '<b>' + (grupoacopio[0][i].GranosAlmacenamiento[j].granoId ? grupoacopio[0][i].GranosAlmacenamiento[j].grano + " - " : "No especifica material - ") + (grupoacopio[0][i].GranosAlmacenamiento[j].toneladasAlmacenamiento ? grupoacopio[0][i].GranosAlmacenamiento[j].toneladasAlmacenamiento : "No especifica ") + "</b> Tns"
                            + '</div>'
                            + '</div>'
                    }
                })(jj);
            }

            html += '</div>'
                + '</div>';

            $(".datos-almacenamiento-cap-prod-guardados").append(html);
            $(".datos-almacenamiento-cap-prod-guardados").show();
            aGuardarAlmacenamiento.push(obj);
            capProdCantAlmacenamiento++;
        })(ii);
    }
}

function armarObjetivos(objetivo) {
    var htmlCampañaGranoObjetivo = "";
    var cant = objetivo.length ? objetivo.length : 0;
    for (var z = 0; z < cant; z++) {
        $("#granoObjetivo" + cantGranoObjetivo).val(objetivo[z].MaterialId);
        var obj = {
            MaterialId: objetivo[z].MaterialId,
            elemId: cantGranoObjetivo
        };
        armarSelectGranoObjetivo(obj);
        $("#campañaObjetivo" + cantGranoObjetivo).val(objetivo[z].CampañaId);
        $("#toneladasObjetivo" + cantGranoObjetivo).val(objetivo[z].ToneladasObjetivos);

        if (z < objetivo.length - 1) {
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

            $("#eliminarObjetivo").show();
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
        }
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