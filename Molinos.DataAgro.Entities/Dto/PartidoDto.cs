using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class PartidoDto
    {
        [DataMember] public int Id { get; set; }
        [DataMember] public string Descripcion { get; set; }
        [DataMember] public int ProvinciaId { get; set; }
        [DataMember] public virtual string Provincia { get; set; }
    }
}