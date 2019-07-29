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
    public partial class EstadioManager : IEstadioManager
    {
        private readonly IRepositorio repositorio;
        public EstadioManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public List<EstadioDto> TraerTodoEstadioPorMaterial(int materialId)
        {
            return repositorio.Listar<Estadio, EstadioDto>(x => new EstadioDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                MaterialId = x.MaterialId
            }, x => x.MaterialId == materialId);
        }
    }
}
