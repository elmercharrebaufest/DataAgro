using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class DiferencialManager : IDiferencialManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public DiferencialManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DiferencialDto TraerDiferencial()
        {
            var diferencial = repositorio.ObtenerMayor<Diferencial, int, DiferencialDto>(x => true, x => x.Id, x =>
                                          new DiferencialDto
                                          {
                                              Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                                              DiferencialDefault = x.DiferencialDefault,
                                              Id = x.Id,
                                              Fecha = x.Fecha
                                          });
            if (diferencial != null)
            {
                diferencial.historialDiferencial = TraerHistorial();
            }
            return diferencial;
        }

        public Resultado GrabarDiferencial(Diferencial diferencial)
        {
            var resultado = new Resultado();
            if (diferencial.DiferencialDefault == 0)
            {
                resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser cero"));
                return resultado;
            }
            var anterior = repositorio.ObtenerMayor<Diferencial, int>(x => true, x => x.Id);
            if (anterior != null)
            {
                if (diferencial.DiferencialDefault == anterior.DiferencialDefault)
                {
                    resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser igual al activo"));
                    return resultado;
                }
            }
            repositorio.Agregar<Diferencial>(diferencial);
            repositorio.GuardarCambios();
            return resultado;
        }

        public Resultado EliminarDiferencial(int id)
        {
            var resultado = new Resultado();
            var diferencial = repositorio.Obtener<Diferencial>(x => x.Id == id);
            repositorio.Remover(diferencial);
            return resultado;
        }

        public Resultado ModificarDiferencial(Diferencial diferencial)
        {
            throw new NotImplementedException();
        }

        private List<DiferencialDto> TraerHistorial(int ultimosN = 20)
        {
            return repositorio.Listar<Diferencial, DiferencialDto>(x => new DiferencialDto
            {
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                DiferencialDefault = x.DiferencialDefault,
                Id = x.Id,
                Fecha = x.Fecha
            }, null, ultimosN, "Fecha", Entities.Helpers.DirOrden.Desc);
        }
    }
}

