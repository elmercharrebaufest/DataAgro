using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class PizarraManager : IPizarraManager
    {
        private readonly IRepositorio repositorio;
        public PizarraManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<PizarraDto> TraerTodoPizarra()
        {
            var hoy = DateTime.Now.Date;
            return repositorio.Listar<Pizarra, PizarraDto>(x => new PizarraDto
            {
                Id = x.Id,
                Codigo = x.Codigo,
                Descripcion = x.Descripcion
               
            });
        }
    }
}
