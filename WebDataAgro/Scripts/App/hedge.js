var diaCerrado;
var matLen;
var objLen;

$(document).ready(function () {
    $('#rootwizard').bootstrapWizard({
        'withVisible': false
    });
    InicializarElementos();  
});

function TimeOut() {
    window.setTimeout(function () {
        $(".alert").fadeTo(1000, 0, function () {
            $(this).remove();
        });
    }, 3000);
}


function InicializarElementos() {
    $(".number-input").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });

    $("#HedgeMaterialTab").click(function (){
        DeseleccionarForms();
        $("#HedgeMaterialTab").children().addClass("whc-selected");
        $("#hedgeMaterial").show();
    });
    $("#ObjetivosTab").click(function (){
        DeseleccionarForms();
        $("#ObjetivosTab").children().addClass("whc-selected");
        $("#objetivos").show();
    });
    $("#HedgeTCTab").click(function (){
        DeseleccionarForms();
        $("#HedgeTCTab").children().addClass("whc-selected");
        $("#hedgeTC").show();
    });
    if (diaCerrado === "True") {
        $("#tc").data("kendoNumericTextBox").enable(false);
        $("#hedgepesos").data("kendoNumericTextBox").enable(false);
        for (var i = 0; i < matLen; i++) {
            $("#disponible" + i).data("kendoNumericTextBox").enable(false);
            $("#forward" + i).data("kendoNumericTextBox").enable(false);
            $("#new-crop" + i).data("kendoNumericTextBox").enable(false);
        }
        for (var h = 0; h < objLen; h++) {
            $("#pricing" + h).data("kendoNumericTextBox").enable(false);
            $("#remitir" + h).data("kendoNumericTextBox").enable(false);
        }
        $(".number-input").addClass("inhabilitado");
        $(".number-input").children().addClass("inhabilitado");
        $(".fin-dia").hide();
        $("#fin-dia").hide();
        $("#reabrir-dia").show();

        $("#row-botones-material").hide();
        $("#row-botones-objetivo").hide();
    } else {
        $("#row-botones-material").show();
        $("#row-botones-objetivo").show();
        $("#reabrir-dia").hide();
    }
}

function DeseleccionarForms() {
    $("#HedgeMaterialTab").children().removeClass("whc-selected");
    $("#hedgeMaterial").hide();
    $("#ObjetivosTab").children().removeClass("whc-selected");
    $("#objetivos").hide();
    $("#HedgeTCTab").children().removeClass("whc-selected");
    $("#hedgeTC").hide();
}

function materialInput() {
    $(".number-input-material").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    TimeOut();
    if (diaCerrado === "True") {
        $("#tc").data("kendoNumericTextBox").enable(false);
        $("#hedgepesos").data("kendoNumericTextBox").enable(false);
        for (var i = 0; i < matLen; i++) {
            $("#disponible" + i).data("kendoNumericTextBox").enable(false);
            $("#forward" + i).data("kendoNumericTextBox").enable(false);
            $("#new-crop" + i).data("kendoNumericTextBox").enable(false);
        }
        $(".number-input").addClass("inhabilitado");
        $(".number-input").children().addClass("inhabilitado");
    }    
}
function objetivoInput() {
    $(".number-input-objetivo").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        min: 0
    });
    TimeOut();
    if (diaCerrado === "True") {
        $("#tc").data("kendoNumericTextBox").enable(false);
        $("#hedgepesos").data("kendoNumericTextBox").enable(false);
        for (var h = 0; h < objLen; h++) {
            $("#pricing" + h).data("kendoNumericTextBox").enable(false);
            $("#remitir" + h).data("kendoNumericTextBox").enable(false);
        }
        $(".number-input").addClass("inhabilitado");
        $(".number-input").children().addClass("inhabilitado");
    }    
}
function tcInput() {
    $(".number-input-tc").kendoNumericTextBox({
        culture: "es-AR",
        format: "n2",
        spinners: false,
        value:"",
        min: 0        
    });
    TimeOut();
}
