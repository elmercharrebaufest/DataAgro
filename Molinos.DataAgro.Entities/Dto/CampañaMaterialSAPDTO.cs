namespace Molinos.DataAgro.Dto
{
    public class CampañaMaterialSAPDTO
    {
        public string CUIT { get; set; }
        public string Campaña { get; set; }
        public string Material { get; set; }
        public string Mes { get; set; }
        public int? Año { get; set; }
        public double? Toneladas { get; set; }
        public string Comercial { get; set; }

        public CampañaMaterialSAPDTO()
        {
            Toneladas = 0;
        }
    }

    public class CampaniaMaterialSAPDTO
    {
        public string CUIT { get; set; }
        public string Campania { get; set; }
        public string Material { get; set; }
        public string Mes { get; set; }
        public int? Anio { get; set; }
        public double? Toneladas { get; set; }
        public string Comercial { get; set; }
        
        public CampaniaMaterialSAPDTO()
        {
            Toneladas = 0;
        }
    }

    public class InformeComercialSAPDTO
    {
        public string CUIT { get; set; }
        public string RptSap { get; set; }
        public string Material { get; set; }
    }

}
