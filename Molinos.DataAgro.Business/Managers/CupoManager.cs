using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class CupoManager:ICupoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ICrearCupoAgent crearCupoAgent;
        private readonly IEliminarCupoAgent eliminarCupoAgent;

        public CupoManager(IRepositorio repositorio,ILogger logger,ICrearCupoAgent crearCupoAgent, IEliminarCupoAgent eliminarCupoAgent) {
            this.repositorio = repositorio;
            this.logger = logger;
            this.crearCupoAgent = crearCupoAgent;
            this.eliminarCupoAgent = eliminarCupoAgent;
        }
        public Resultado GrabarCupo(Cupo cupo, int cantidadCupos)
        {
            var error = Validar(cupo, cantidadCupos);
            if (error.HayError)
            {
                return error;
            }
            try
            {
                cupo.Proveedor = repositorio.Obtener<Proveedor>(cupo.ProveedorId);
                cupo.Material = repositorio.Obtener<Material>(cupo.MaterialId);
                cupo.Centro = repositorio.Obtener<Centro>(cupo.CentroId);
                cupo.ZonaCupo = repositorio.Obtener<ZonaCupo>(cupo.ZonaCupoId);

                var listaCupos = crearCupoAgent.Crear(cupo, cantidadCupos);
                var cuposConSap = new List<Cupo>();
                
                cupo.EstadoCupoId = 1;
                foreach (var cupoSap in listaCupos)
                {
                    var nuevoCupo = (Cupo)cupo.Clone();
                    nuevoCupo.CupoSap = cupoSap;

                    cuposConSap.Add(nuevoCupo);
                }
                repositorio.AgregarTodos(cuposConSap);
                repositorio.GuardarCambios();
                return error;
            }
            catch(Exception e)
            {
                logger.Error(e);
                error.Errores.Add(new ErrorMessage(400,e.Message));
                return error;
            }
        }
        private Resultado Validar(Cupo cupo,int cantidadCupos)
        {
            var error = new Resultado();
           
            if (cupo.ProveedorId != 0)
            {
                var cuit = repositorio.Obtener<Proveedor, string>(y => y.ProveedorId == cupo.ProveedorId, y => y.CUIT);
                var sisa = repositorio.Obtener<SISA>(x => x.CUIT == cuit);
                if ((sisa != null && (sisa.EstadoCuit == 0 || sisa.EstadoCuit == 3)) || sisa == null)
                {
                    error.Errores.Add(new ErrorMessage(400, "Proveedor con CUIT en estado No Operable"));
                }
            }                                   
            if (cupo.MaterialId == 3 && (cupo.Calidad == ""|| cupo.Calidad == null))
            {
                error.Errores.Add(new ErrorMessage(400, "La calidad no debe estar vacia para Soja"));
            }
            if (cupo.Fason == true && (cupo.Destinatario == ""|| cupo.Destinatario == null))
            {
                error.Errores.Add(new ErrorMessage(400, "CUIT Destinatario no debe estar vacio cuando elige Fasón"));
            }
            var limiteCupo = repositorio.Obtener<LimiteCupo>(x => x.ConfiguracionCupo.CentroId == cupo.CentroId && x.ConfiguracionCupo.MaterialId == cupo.MaterialId
           && x.ZonaCupoId == cupo.ZonaCupoId && x.ConfiguracionCupo.Fecha == cupo.FechaIngreso);
            if (limiteCupo == null)
            {
                error.Errores.Add(new ErrorMessage(400, "La Zona no esta dada de alta en Administracion de Cupos"));
            }
            var cuposOtorgados = repositorio.Contar<Cupo>(x => x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.FechaIngreso == cupo.FechaIngreso && x.ZonaCupoId == cupo.ZonaCupoId);
            if (limiteCupo!= null && limiteCupo.CantidadCupo < cantidadCupos + cuposOtorgados)
            {
                error.Errores.Add(new ErrorMessage(400, "Limite de cupos alcanzado"));
            }
            return error;
        }
        public KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosCupos(request, equipo));
        }
        public Resultado EliminarCupo(int id)
        {
            try
            {
                var nuevoResultado = new Resultado();
                var cupoSap = repositorio.Obtener<Cupo>(id);
                var resultado = eliminarCupoAgent.Eliminar(cupoSap.CupoSap);
                if (resultado == "OK")
                {
                    cupoSap.EstadoCupoId = 5;
                    repositorio.GuardarCambios();
                }
                else
                {
                    nuevoResultado.Error("","Error al anular cupo"); ;
                }
                return nuevoResultado;
            }catch (Exception e)
            {
                throw e;
            }
        }
    }
}
