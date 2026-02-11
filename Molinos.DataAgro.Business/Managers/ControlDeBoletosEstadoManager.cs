using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
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
    public class ControlDeBoletosEstadoManager : IControlDeBoletosEstadoManager
    {
        private readonly IRepositorio _repositorio;

        public ControlDeBoletosEstadoManager(IRepositorio repositorio)
        {
            this._repositorio = repositorio;
        }

        public List<ControlDeBoletosEstadoDto> ListarTodo()
        {
            List<ControlDeBoletosEstadoDto> listarEstados = new List<ControlDeBoletosEstadoDto>();
            listarEstados = _repositorio.Listar<ControlDeBoletosEstado, ControlDeBoletosEstadoDto>(x => new ControlDeBoletosEstadoDto()
            {
                Id = x.Id,
                Descripcion = x.Descripcion
            });
            return listarEstados;
        }
    }
}
