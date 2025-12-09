using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DeclaracionCampoSustentable
    {
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public string Fecha { get; set; }
        public string Cosecha { get; set; }
        public double CantidadParteSoja { get; set; }
        public List<CamposSustentableReporte> Campos { get; set; }
        public string Totalidad { get { return CantidadParteSoja == 0 ? "X" : ""; } }
        public string Parcial { get { return CantidadParteSoja > 0 ? "X" : ""; } }
        public string TotalidadMensaje { get { return "La totalidad de la soja correspondiente a la cosecha " + Cosecha + " proveniente de mi tierra y entregada a Molinos Agro S.A. ha sido cultivada en campos que ya se encontraban bajo agricultura antes del 1 de enero de 2008, esto es: proviene de “tierras cultivables”"; } }
        public string ParcialMensaje { get { return "Parte de la soja correspondiente a la cosecha " + Cosecha + " producida en mi tierra y entregada a Molinos Agro S.A., es procedente de " + (CantidadParteSoja > 0 ? CantidadParteSoja.ToString() : "_______") + " hectáreas que ya se encontraban bajo agricultura antes del 1 de enero de 2008, esto es: proviene de “tierras cultivables”"; } }
        public string CosechaMensaje { get { return "Cosecha " + Cosecha; } }
        public string FechaFormateada { get { return "Fecha: " + Fecha; } }
    }

    public class CamposSustentableReporte
    {
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string HectareasTotales { get; set; }
        public string HectareasSoja { get; set; }
        public string Coordenadas { get; set; }
        public int N { get; set; }
        public string Partido { get; set; }
    }
}