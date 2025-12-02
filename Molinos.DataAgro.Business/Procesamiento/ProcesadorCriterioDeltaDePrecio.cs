using NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Linq;

namespace Molinos.DataAgro.Business
{
    public class ProcesadorCriterioDeltaDePrecio : ProcesadorCriterio<CriterioDeltaDePrecio>
    {

        private readonly ITipoDeCambioAgent tipoDeCambio;
        public ProcesadorCriterioDeltaDePrecio(IRepositorio repositorio, ILogger log, ITipoDeCambioAgent tipoDeCambio)
           : base(repositorio, log)
        {
            this.tipoDeCambio = tipoDeCambio;
        }
        public override decimal Calcular(CriterioDeltaDePrecio criterio)
        {
            if (String.IsNullOrEmpty(criterio.Dto.MonedaId) || criterio.Dto.Precio == null || criterio.Dto.Precio == 0)
            {
                return 0;
            }
            DateTime hoy = DateTime.Now.Date;
            var pizarraLista = Repositorio.Listar<PrecioPizarra>(x => x.FechaDesde <= hoy && x.FechaHasta >= hoy);

            string typeOfRate = null;
            if (criterio.Dto != null && criterio.Dto.NegocioId != null && criterio.Dto.NegocioId != 0)
            {
                Negocio negocio = Repositorio.Obtener<Negocio>(criterio.Dto.NegocioId);
                typeOfRate = negocio.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND ? "Z" : "M";
            }
            decimal dolarCotizacion = tipoDeCambio.TraerTipoDeCambio(null, typeOfRate);
            var pizarra = pizarraLista.Where(a => a.MaterialId == criterio.Dto.MaterialId).SingleOrDefault();
            decimal precioPizarra = 1;
            if (pizarra == null)
            {
                pizarra = Repositorio.Listar<PrecioPizarra>(a => a.MaterialId == criterio.Dto.MaterialId).OrderByDescending(a => a.FechaDesde).Take(1).Single();
                pizarraLista.Add(pizarra);
            }

            precioPizarra = pizarra.MonedaId == "ARP  " ? pizarra.Precio : pizarra.Precio * dolarCotizacion;

            criterio.Dto.PrecioPizarra = precioPizarra;
            var precio = criterio.Dto.MonedaId == "ARP  " ? criterio.Dto.Precio : criterio.Dto.Precio * dolarCotizacion;
            decimal puntos = (precio.Value - precioPizarra) / -precioPizarra;

            return puntos;
        }
    }
}
