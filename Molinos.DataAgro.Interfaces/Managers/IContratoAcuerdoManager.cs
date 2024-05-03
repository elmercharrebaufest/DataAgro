using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoAcuerdoManager
    {
        DatosIniComboContratoAcuerdo TraerDatosCombo();
        ResultIniContratoAcuerdo TraerTodoContratoAcuerdo();
        GrabarAcuerdoResult FinalizarAcuerdo(int id);
        GrabarAcuerdoResult GrabarAcuerdo(ContratoAcuerdo oContratoAcuerdo, List<CupoConDescargaFechasDto> listCupoConDescargaFechas = null);
        BasicoContrato TraerAcuerdo(int contratoId);
        GrabarAcuerdoResult BorrarAcuerdo(ContratoAcuerdo oAcuerdo);
        ContratoAcuerdoDto ObtenerContratoAcuerdoParaAsociar(DateTime fecha, int destinoId, int materialId, int proveedorId);
        Resultado ConfirmarContratoAcuerdo(int id, int comercialId);
        void AnularAcuerdos();
    }
}