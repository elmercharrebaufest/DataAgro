using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class CartaDePresentacionManager : ICartaDePresentacionManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public CartaDePresentacionManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public RptCartaDePresentacionInfo GenerarCartaDePresentacion(RptCartaDePresentacionInfo informe, List<NuevoProduccion> nuevosCampos, List<NuevoAcopio> nuevosAcopios)
        {
            if (nuevosCampos != null || nuevosAcopios != null)
            {
                var materiales = repositorio.Listar<Material>();
                var campañas = repositorio.Listar<Campaña>();
                var localidades = repositorio.Listar<Localidad>();

                if (nuevosCampos != null)
                {
                    foreach (var item in nuevosCampos)
                    {
                        informe.CapProduccion.Add(new CartaDePresentacionAcopiadores
                        {
                            Grano = materiales.Where(a => a.MaterialId == item.MaterialId).Single().Descripcion,
                            Localidad = localidades.Where(a => a.LocalidadId == item.LocalidadId).Single().Nombre,
                            Provincia = localidades.Where(a => a.LocalidadId == item.LocalidadId).Single().Provincia.Nombre,
                            Toneladas = item.Toneladas,
                            Superficie = item.Hectareas,
                            Alquilado = !item.ArrendaPropia ? "X" : String.Empty,
                            Propio = item.ArrendaPropia ? "X" : String.Empty
                        });
                    }

                    //// Maiz
                    var TonMaiz = nuevosCampos.Where(a => a.MaterialId == 1).Sum(a => a.Toneladas);
                    //// Soja
                    var TonSoja = nuevosCampos.Where(a => a.MaterialId == 3).Sum(a => a.Toneladas);
                    //// Trigo
                    var TonTrigo = nuevosCampos.Where(a => a.MaterialId == 2).Sum(a => a.Toneladas);
                    //// Girasol
                    var TonGira = nuevosCampos.Where(a => a.MaterialId == 4).Sum(a => a.Toneladas);
                    //// Girasol Alto
                    var TonGiraAlto = nuevosCampos.Where(a => a.MaterialId == 5).Sum(a => a.Toneladas);

                    informe.ToneladasTodo = "Maiz " + TonMaiz.ToString("N2") + " Tn. / Soja " + TonSoja.ToString("N2") + " Tn. / Trigo " + TonTrigo.ToString("N2") +
                        " Tn. / Girasol " + TonGira.ToString("N2") + " Tn. / Girasol A. O. " + TonGiraAlto.ToString("N2") + " Tn. ";
                }
                else
                {
                    informe.ToneladasTodo = "";
                }

                if (nuevosAcopios != null)
                {
                    foreach (var item in nuevosAcopios)
                    {
                        informe.CapAlmacenaje.Add(new CartaDePresentacionAcopiadores
                        {
                            Localidad = localidades.Where(a => a.LocalidadId == item.LocalidadId).Single().Nombre,
                            Provincia = localidades.Where(a => a.LocalidadId == item.LocalidadId).Single().Provincia.Nombre,
                            Toneladas = item.Toneladas,
                            Alquilado = !item.ArrendaPropia ? "X" : String.Empty,
                            Propio = item.ArrendaPropia ? "X" : String.Empty
                        });
                    }
                }
            }
            

            return informe;
        }
    }

}
