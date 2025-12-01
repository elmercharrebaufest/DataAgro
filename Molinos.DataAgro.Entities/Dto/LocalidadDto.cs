using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public partial class LocalidadDto
    {
        [DataMember] public int LocalidadId { get; set; }
        [DataMember] public string Nombre { get; set; }
        [DataMember] public string CodLocalidad { get; set; }
        [DataMember] public string CodigoPostal { get; set; }
        [DataMember] public string SubCodigoPostal { get; set; }
        [DataMember] public int ProvinciaId { get; set; }
        [DataMember] public string Provincia_Nombre { get; set; }
        [DataMember] public string Partido_Nombre { get; set; }
        [DataMember] public int? PartidoId { get; set; }
        [DataMember] public string CodigoConfirma { get; set; }
    }
}