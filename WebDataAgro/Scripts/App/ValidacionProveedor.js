function validar() {
    if (!$("#cuit").val()) {
        MensErr("El CUIT debe estar cargado.");
        return false;
    }
    var tonsProduccion = 0;
    for (var i = 0; i < aGuardar.length; i++) {
        for (var j = 0; j < aGuardar[i].granos.length; j++) {
            tonsProduccion += aGuardar[i].granos[j].toneladas;
        }
    }
    var tonsAlmacenamiento = 0;
    for (var i = 0; i < aGuardarAlmacenamiento.length; i++) {
        for (var j = 0; j < aGuardarAlmacenamiento[i].granosAlmacenamientoGrano.length; j++) {
            tonsAlmacenamiento += aGuardarAlmacenamiento[i].granosAlmacenamientoGrano[j].toneladasAlmacenamiento;
        }
    }
    console.log(aGuardarAlmacenamiento, tonsAlmacenamiento);
    if ((!$("#segmentacion").val() || $("#segmentacion").val() === "null") && (tonsProduccion == 0 && tonsAlmacenamiento == 0)) {
        MensErr("El campo Segmentacion debe estar cargado.");
        return false;
    }
    /*if (!$("#nomReferente").val()) {
        MensErr("El campo Nombre Referente debe estar cargado.");
        return false;
    }*/

    if ($("#campañaObjetivo" + cantGranoObjetivo).val() != "null" || ($("#granoObjetivo" + cantGranoObjetivo).val() != "null"
        || $("#toneladasObjetivo" + cantGranoObjetivo).val().trim().length > 0)) {
        var id = $(".ObjetivoGranos:last-of-type").attr("class").split(" ")[0].split("lineaObjetivos")[1];
        if (!ValidarGranoObjetivo(id))
            return false;
    }

    if ($("#volumen-anual-total-tns").val()) {
        if (isNaN($("#volumen-anual-total-tns").val().trim().split(",").join("."))) {
            MensErr("El formato del Total de Toneladas es invalido");
            return false;
        }
    }

    if ($("#capacidad-almacenamiento-prop").val()) {
        if (isNaN($("#capacidad-almacenamiento-prop").val().trim().split(",").join("."))) {
            MensErr("El formato del Almacenamiento propio es invalido");
            return false;
        }
    }

    if ($("#tons-max-aprob-sojasust").val()) {
        if (isNaN($("#tons-max-aprob-sojasust").val().trim().split(",").join("."))) {
            MensErr("El formato del maximo de Toneladas aprobada de soja sustentable es invalido");
            return false;
        }
    }

    if ($("#has-aprob-sojasust").val()) {
        if (isNaN($("#has-aprob-sojasust").val().trim().split(",").join("."))) {
            MensErr("El formato de las Hectareas aprobadas de soja sustentable es invalido");
            return false;
        }
    }

    return true;
}

function ValidarGranoProduccion(cantGrano) {
    if ($("#campaña" + cantGrano).val() && $("#campaña" + cantGrano).val() != "null") {
        if (!$("#grano" + cantGrano).val() || $("#grano" + cantGrano).val() == "null") {
            MensErr("Debe ingresar un grano");
            return false;
        }
    }

    /*
    if (!$("#hectareas" + cantGrano).val() || $("#hectareas" + cantGrano).val().trim().length == 0) {
        MensErr("Debe ingresar una cantidad de hectareas");
        return false;
    }
    else*/
    if (isNaN($("#hectareas" + cantGrano).val().trim().split(",").join("."))) {
        MensErr("El formato de las Hectareas es invalido");
        return false;
    }
    /*if (!$("#toneladas" + cantGrano).val() || $("#toneladas" + cantGrano).val().trim().length == 0) {
        MensErr("Debe ingresar una cantidad de toneladas");
        return false;
    }
    else */if (isNaN($("#toneladas" + cantGrano).val().trim().split(",").join("."))) {
        MensErr("El formato de las Toneladas es invalido");
        return false;
    }

    return true;
}

function ValidarGranoAlmacenamiento(cantGranoAlmacenamiento) {
    if ($("#toneladasAlmacenamiento" + cantGranoAlmacenamiento).val() && isNaN($("#toneladasAlmacenamiento" + cantGranoAlmacenamiento).val().trim().split(",").join("."))) {
        MensErr("El formato de las Toneladas es invalido");
        return false;
    }
    return true;
}

function ValidarGranoObjetivo(cantGranoObjetivo) {
    if (!$("#campañaObjetivo" + cantGranoObjetivo).val() || $("#campañaObjetivo" + cantGranoObjetivo).val() == "null") {
        MensErr("Debe ingresar una campaña");
        return false;
    }
    if (!$("#granoObjetivo" + cantGranoObjetivo).val() || $("#granoObjetivo" + cantGranoObjetivo).val() == "null") {
        MensErr("Debe ingresar un grano");
        return false;
    }

    /*if (!$("#toneladasObjetivo" + cantGranoObjetivo).val() || $("#toneladasObjetivo" + cantGranoObjetivo).val().trim().length == 0) {
        MensErr("Debe ingresar las tonelas objetivos");
        return false;
    }
    else*/ if (isNaN($("#toneladasObjetivo" + cantGranoObjetivo).val().trim().split(",").join("."))) {
        MensErr("El formato de las Toneladas es invalido");
        return false;
    }

    return true;
}

function ValidarContactoComercial() {
    if (!$("#concom-nombre").val() || $("#concom-nombre").val() === "") {
        MensErr("Se debe ingresar el Nombre de Contacto Comercial");
        return false;
    }

    if (!$("#concom-apellido").val() || $("#concom-apellido").val() === "") {
        MensErr("Se debe ingresar el Apellido de Contacto Comercial");
        return false;
    }

    if ($("#concom-fechanacimientodia").val() && (!$("#concom-fechanacimientomes").val() || !$("#concom-anionacimientomes").val())) {
        MensErr("La fecha tiene formato incorrecto.");
        return false;
    }
    if ($("#concom-fechanacimientomes").val() && (!$("#concom-fechanacimientodia").val() || !$("#concom-anionacimientomes").val())) {
        MensErr("La fecha tiene formato incorrecto.");
        return false;
    }
    if ($("#concom-anionacimientomes").val() && (!$("#concom-fechanacimientodia").val() || !$("#concom-fechanacimientomes").val())) {
        MensErr("La fecha tiene formato incorrecto.");
        return false;
    }
    var email = $('#concom-email1').val();
    for (var i = 1; i <= 3; i++) {
        email = $('#concom-email'+i).val();
        if (email && email !== "") {
            if (!validateEmail(email)) {
                MensErr("El Email "+ i +" no es válido");
                return false;
            }
            var mail = email.split('@');
            if (mail[1].toLowerCase().startsWith('molinosagro')) {
                MensErr("El Email"+i+" no debe ser de MolinosAgro");
                return false;
            }
        }
    }
    /*
if (!$("#concom-email1").val() || $("#concom-email1").val() === "") {
    MensErr("El contacto comercial debe tener al menos un Email");
    return false;
} else if (!validateEmail($("#concom-email1").val())) {
    MensErr("El primer Email no es válido");
    return false;
}

if (!$("#concom-TipoTelefono1").val() || $("#concom-TipoTelefono1").val() === "null"){
    MensErr("El contacto comercial debe tener al menos un Tipo de Telefono");
    return false;
}
if (!$("#concom-Telefono1").val() || $("#concom-Telefono1").val() === "") {
    MensErr("El contacto comercial debe tener al menos un Telefono");
    return false;
}

if ($("#concom-email2") && $("#concom-email2").length > 0 && $("#concom-email2").val()) {
    if (!validateEmail($("#concom-email2").val())) {
        MensErr("El segundo Email no es válido");
        return false;
    }
}

if ($("#concom-email3") && $("#concom-email3").length > 0 && $("#concom-email3").val()) {
    if (!validateEmail($("#concom-email3").val())) {
        MensErr("El tercer Email no es válido");
        return false;
    }
}

if (!$("#concom-cargo").val() || $("#concom-cargo").val() === "") {
    MensErr("Se debe ingresar el Cargo de Contacto Comercial");
    return false;
}

if (!$("#concom-puesto").val() || $("#concom-puesto").val() === "") {
    MensErr("Se debe ingresar el Puesto de Contacto Comercial");
    return false;
}
*/

    return true;
}