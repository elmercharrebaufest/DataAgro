$(document).ready(function () {
    $('#menuproveedor').hide();
    InicializarElementos();
});

Date.prototype.addDays = function (days) {
    var date = new Date(this.valueOf());
    date.setDate(date.getDate() + days);
    return date;
};

function InicializarElementos() {
    var fecha = new Date();
    $("#FechaDesde").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"],
        change: function () {
            var parts = $("#FechaDesde").val().split('-');
            var desde = new Date(parseInt(parts[2]), parseInt(parts[1]) - 1, parseInt(parts[0]) ).addDays(7);

            $("#FechaHasta").data("kendoDatePicker").value(desde);
        }
    }); 

    $("#FechaHasta").kendoDatePicker({
        format: "dd-MM-yyyy",
        parseFormats: ["dd-MM-yyyy", "dd/MM/yyyy"]
    });
}

$(".alert").ready(function () {
    setTimeout(function () { $(".alert").hide(); }, 5000);
});

function LimpiarForm() {
    fecha = new Date();
    $("#Id").val(0);
    $("#MaterialId").val("");
    $("#TipoResearchId").val("");
    $("#CampanaId").val("");
    $("#FechaDesde").data("kendoDatePicker").value(fecha);
    $("#FechaHasta").data("kendoDatePicker").value(fecha.addDays(7));
    $("#Mensaje").val("");
}