namespace Molinos.DataAgro.Entities.Dto
{
    public class InicializarContratoDto
    {
        public DatosIniContrato Datos { get; set; }

        public InicializarContratoDto()
        {
            this.Datos = new DatosIniContrato();
        }
    }
}