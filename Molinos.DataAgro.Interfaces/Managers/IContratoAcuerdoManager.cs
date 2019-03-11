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
        DatosIniComboContratoAcuerdo TraerDatosCombo(int perfil);
        ResultIniContratoAcuerdo TraerTodoContratoAcuerdo();
        ContratoAcuerdoDto TraerContratoAcuerdo(int id);
        Resultado GrabarContratoAcuerdo(ContratoAcuerdo oCentro, EnumPerfil perfil);
        Resultado EliminarContratoAcuerdo(int id);
        ContratoAcuerdoDto ObtenerContratoAcuerdoParaAsociar(DateTime fecha, int destinoId, int materialId, int proveedorId);
        Resultado ConfirmarContratoAcuerdo(int id);
    }
}
