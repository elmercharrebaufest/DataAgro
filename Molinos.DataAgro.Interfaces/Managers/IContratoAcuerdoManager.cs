using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoAcuerdoManager
    {
        DatosIniAbmContratoAcuerdo TraerDatosIniciales();
        DatosIniComboContratoAcuerdo TraerDatosCombo();
        ResultIniContratoAcuerdo TraerTodoContratoAcuerdo();
        GrabarAcuerdoResult FinalizarAcuerdo(int id);
        GrabarAcuerdoResult GrabarAcuerdo(ContratoAcuerdo oContratoAcuerdo);
        BasicoContrato TraerAcuerdo(int contratoId);
        GrabarAcuerdoResult BorrarAcuerdo(ContratoAcuerdo oAcuerdo);
        ContratoAcuerdoDto ObtenerContratoAcuerdoParaAsociar(DateTime fecha, int destinoId, int materialId, int proveedorId);
        Resultado ConfirmarContratoAcuerdo(int id);
    }
}
