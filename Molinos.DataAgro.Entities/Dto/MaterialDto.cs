namespace Molinos.DataAgro.Entities.Dto
{
    public partial class MaterialDto
    {
        public int MaterialId { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
        public int? CampañaId { get; set; }
        public string Campana { get; set; }
        public int? CampaniaTableroId { get; set; }
        public string CampaniaTablero { get; set; }
    }
}
   



