using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaAltaLoteResultDto : Resultado
    {
        public string altaIdLote;
        public int altaEstado;
        public bool altaEstadoSpecified;
        public object altaEstadoDetalleError;
        public int altaEstadoLote;
        public bool altaEstadoLoteSpecified;
        public List<altaItemDto> altaItem;
        public ConfirmaAltaEstadoDto confirmaAltaEstado;
        public ConfirmaAltaEstadoLoteDto confirmaAltaEstadoLote;
    }

    public class altaItemDto
    {
        public string altaIdDocumento;
        public int altaEstadoDocumento;
        public List<string> altaErrores;
        public string altaIdDocumentoExistenteLote;
        public string altaIdDocumentoExistente;
        public string codigo;
        public ConfirmaAltaEstadoDocumentoDto confirmaAltaEstadoDocumento;
    }

    public class ConfirmaAltaEstadoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstado { get; set; }
    }

    public class ConfirmaAltaEstadoLoteDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstadoLote { get; set; }
    }

    public class ConfirmaAltaEstadoDocumentoDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int CodigoConfirmaAltaEstadoDocumento { get; set; }
    }

    public class EstadosConfirmaDto
    {
        public List<ConfirmaAltaEstadoDto> ConfirmaAltaEstadoDto;
        public List<ConfirmaAltaEstadoLoteDto> ConfirmaAltaEstadoLoteDto;
        public List<ConfirmaAltaEstadoDocumentoDto> ConfirmaAltaEstadoDocumentoDto;
    }

    public class ConfirmaAltaLoteBorradorResultDto : Resultado
    {
        public string altaIdLote;
        public int altaEstado;
        public bool altaEstadoSpecified;
        public object altaEstadoDetalleError;
        public int altaEstadoLote;
        public bool altaEstadoLoteSpecified;
        public List<altaItemBorradorDto> altaItem;
        public ConfirmaAltaEstadoDto confirmaAltaEstado;
        public ConfirmaAltaEstadoLoteDto confirmaAltaEstadoLote;
    }

    public class altaItemBorradorDto
    {
        public string altaIdLote;
        public string altaIdBolsa;
        public int altaEstadoDocumento;
        public List<string> altaErrores;
        public string altaIdDocumentoExistenteLote;
        public string altaIdDocumentoExistente;
        public string codigo;
        public ConfirmaAltaEstadoDocumentoDto confirmaAltaEstadoDocumento;
    }
}