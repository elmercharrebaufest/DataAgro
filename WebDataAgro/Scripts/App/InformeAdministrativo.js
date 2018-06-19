var checkedIds = {};

$(document).ready(function () {
	
	InicializarElementos();

	CreateGridInformes();

	CargarGrilla();
	
});

var InicializarElementos = function(){
	
	kendo.culture("es-AR");  
	
	$("#butDescargar").click(function(){
		DescargarElementos();
	});	
	
}

var DescargarElementos = function(){
	var checked = [];
	for (var i in checkedIds) {
		if (checkedIds[i]) {
			checked.push(i);
		}
	}
	var funcReturn = function (data) {

		if (data != null) {
			if (data.Errores.length > 0)
			{
				MensErr("No se encontraron resultados para los filtros elegidos.");
				return false;
			}
			else
			{
				CargarGrilla();
				if (data.DownloadKey.length > 0) {
					var url = MSGetUrl('/DownLoad/Excel?key=' + data.DownloadKey);
					window.location = url;
				}
			}
		}
		else
		{
			MensErr("No se encontraron resultados para los filtros elegidos.");
			return false;
		}
	}

	if (checked.length > 0) {
		var datos = { Informes: checked.join(',')};
		MSExecuteOnServerAsync('/InformeComercial/GenerarExcel', datos, funcReturn, true);
	}
} 

var CreateGridInformes = function(){
	
	$("#grilla-informes").kendoGrid({
        columns: [
            //{ template: '<input type="checkbox" #= Seleccionado ? \'checked="checked"\' : "" # class="chkbx" />', width: 110 }, 
			{
				title: 'Select All',
				headerTemplate: "<input type='checkbox' id='header-chb' class='k-checkbox header-checkbox'><label class='k-checkbox-label' for='header-chb'></label>",
				template: function (dataItem) {
					return "<input type='checkbox' id='" + dataItem.InformeComercialId + "' class='k-checkbox row-checkbox'><label class='k-checkbox-label' for='" + dataItem.InformeComercialId + "'></label>";
				},
				width: 80
			},
            { field: "Cuit", width: 250 },
            { field: "RazonSocial", title: "Razón Social", width: 250 },
			{ field: "Campaña"},
			{ field: "Materiales", template: "#=Materiales#"},
			{ field: "Comercial" }
        ],       
        sortable: true,
        selectable: "row"
    });
	
	var grid = $("#grilla-informes").data("kendoGrid");
	grid.table.on("click", ".row-checkbox", selectRow);
	
	$('#header-chb').change(function (ev) {
		var checked = ev.target.checked;
		$('.row-checkbox').each(function (idx, item) {
			if (checked) {
				if (!($(item).closest('tr').is('.k-state-selected'))) {
					$(item).click();
				}
			} else {
				if ($(item).closest('tr').is('.k-state-selected')) {
					$(item).click();
				}
			}
		});
	});

	
	
    /*$("#grilla-informes").on("change", "input.chkbx", function (e) {
        var grid = $("#grilla-informes").data("kendoGrid"),
            dataItem = grid.dataItem($(e.target).closest("tr"));

        dataItem.set("Seleccionado", this.checked);
    });*/

	function selectRow() {
        var checked = this.checked,
            row = $(this).closest("tr"),
            grid = $("#grilla-informes").data("kendoGrid"),
            dataItem = grid.dataItem(row);

        checkedIds[dataItem.InformeComercialId] = checked;

        if (checked) {
            //-select the row
            row.addClass("k-state-selected");

            var checkHeader = true;

            $.each(grid.items(), function (index, item) {
                if (!($(item).hasClass("k-state-selected"))) {
                    checkHeader = false;
                }
            });

            $("#header-chb")[0].checked = checkHeader;
        } else {
            //-remove selection
            row.removeClass("k-state-selected");
            $("#header-chb")[0].checked = false;
        }
    }

    //on dataBound event restore previous selected rows:
    function onDataBound(e) {
        var view = this.dataSource.view();
        for (var i = 0; i < view.length; i++) {
            if (checkedIds[view[i].InformeComercialId]) {
                this.tbody.find("tr[data-uid='" + view[i].uid + "']")
                    .addClass("k-state-selected")
                    .find(".k-checkbox")
                    .attr("checked", "checked");
            }
        }
    }
		
}

var CargarGrilla = function(){
	var funcreturn = function(datos){
		if (datos.length > 0){
			var grid = $("#grilla-informes").data("kendoGrid");
			var dataSource = new kendo.data.DataSource({
				data: datos
			});
			
			grid.setDataSource(dataSource);
		}else{
			var div = $("#grilla-informes");
			div.css({
				'background-color':'transparent',
				'text-align':'center',
				border: 'none',
				'margin-top': 20
			});
			div.empty();
			$("<img src='../Content/Images/agro.png'>").appendTo(div).css({
				width: 30,
				height: 30				
			});
			$("<span>").appendTo(div).html("No se encontraron resultados").css({
				'padding-left': 10,
				'padding-right': 10,
				'font-size': 18
			});
			$("<img src='../Content/Images/agro.png'>").appendTo(div).css({
				width: 30,
				height: 30
			});			
		
			$("#butDescargar").hide();
			
			var grid = $("#grilla-informes").data("kendoGrid");
			var dataSource = new kendo.data.DataSource({
				data: []
			});
			
			grid.setDataSource(dataSource);
			
		}
	}
	
	MSExecuteOnServerAsync('/InformeComercial/ListarInformes', {}, funcreturn, true);
	
}

