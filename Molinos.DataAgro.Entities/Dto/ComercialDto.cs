namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ComercialDto
    {
        public int ComercialId { get; set; }
        public string Apellido { get; set; }
        public string Nombres { get; set; }
        public int PerfilId { get; set; }
        public int? EmpleadorACargoId { get; set; }        
        public string IdActiveDirectory { get; set; }
        public int? GrupoDeComprasId { get; set; }        
        public bool? Administrador { get; set; }
    }
}


