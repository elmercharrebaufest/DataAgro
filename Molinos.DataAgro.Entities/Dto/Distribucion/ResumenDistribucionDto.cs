namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ResumenDistribucionDto
    {
        public string Material { get; set; }
        public string Operador { get; set; }
        public string Clase { get; set; }
        public int Limite { get; set; }
        public int CuposAsignados { get; set; }
        public int CuposRestantes { get; set; }
        public int ContratosAsignados { get; set; }
        public int ContratosSinCupos { get; set; }
    }
}
