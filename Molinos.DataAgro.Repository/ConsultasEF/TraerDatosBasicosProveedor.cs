using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerDatosBasicosProveedor : IConsulta<BasicoProveedor>
    {
        private readonly int proveedorId;
        private readonly int comercialId;
        private readonly List<int> equipo;

        public TraerDatosBasicosProveedor(int proveedorId, int comercialId, List<int> equipo)
        {
            this.proveedorId = proveedorId;
            this.comercialId = comercialId;
            this.equipo = equipo;
        }

        private static List<BasicoProveedor> Query(DbContext contexto, int proveedorId, int comercialId, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var estadosDisponibles = contexto.Set<ProveedorEstado>().Where(x => x.Proveedor.ProveedorId == proveedorId && equipo.Contains(x.Comercial.ComercialId)).Select(x => x.Estado.EstadoId).ToList();
            var estado = estadosDisponibles.Contains(2) ? 2 : estadosDisponibles.Contains(3) ? 3 : estadosDisponibles.Contains(4) ? 4 : estadosDisponibles.Contains(5) ? 5 : estadosDisponibles.Contains(1) ? 1 : 0;
            var resultado =
                from prove in contexto.Set<Proveedor>()
                join est in contexto.Set<Estado>() on (estado == 0 ? prove.Estado.EstadoId : estado) equals est.EstadoId into ests
                from est in ests.DefaultIfEmpty()
                join fac in contexto.Set<FACACOP>() on prove.CUIT equals fac.CUIT into facs
                from fac in facs.DefaultIfEmpty()
                join rg in contexto.Set<SISA>().GroupBy(x => x.CUIT).Select(x => new { CUIT = x.Key, EstadoCUIT = x.FirstOrDefault().EstadoCuit }) on prove.CUIT equals rg.CUIT into rgs
                from rg in rgs.DefaultIfEmpty()
                join con in contexto.Set<ContactoComercial>() on prove.ProveedorId equals con.Proveedor.ProveedorId into cons
                from con in cons.DefaultIfEmpty()
                join pco in contexto.Set<ProveedorComercial>() on prove.ProveedorId equals pco.Proveedor.ProveedorId into pcos
                from pco in pcos.DefaultIfEmpty()
                join com in contexto.Set<Comercial>() on pco.Comercial.ComercialId equals com.ComercialId into coms
                from com in coms.DefaultIfEmpty()
                join pcaop in contexto.Set<ProveedorCanalOperacion>() on prove.ProveedorId equals pcaop.Proveedor.ProveedorId into pcaops
                from pcaop in pcaops.DefaultIfEmpty()
                join pd in contexto.Set<ProveedorDestinatario>() on prove.ProveedorId equals pd.Proveedor.ProveedorId into pds
                from pd in pds.DefaultIfEmpty()
                join pc in contexto.Set<ProveedorCondicion>() on prove.ProveedorId equals pc.Proveedor.ProveedorId into pcs
                from pc in pcs.DefaultIfEmpty()
                    //join pcom in contexto.Set<Proveedor>() on prove.ComisionistaId equals pcom.ProveedorId into pcoms
                    //from pcom in pcoms.DefaultIfEmpty()
                join estHome in contexto.Set<EstadoHome>() on prove.EstadoHomeId equals estHome.Id into estHomes
                from estHome in estHomes.DefaultIfEmpty()
                where prove.ProveedorId == proveedorId
                orderby pco.NroItem ascending
                select new BasicoProveedor()
                {
                    ProveedorId = prove.ProveedorId,
                    RazonSocial = prove.RazonSocial,
                    Alias = prove.Alias,
                    CUIT = prove.CUIT,
                    Estado = est.Descripcion,
                    Facacop = (fac.CUIT == null) ? 0 : 1,
                    RiesgoComercialSap = prove.RiesgoComercialSap,
                    EstadoCuit = rg != null ? rg.EstadoCUIT : 0,
                    Segmentacion = prove.Segmentacion.Descripcion,
                    GrupoSegmentacion = prove.Segmentacion.Grupo,
                    Email1 = con.Email1,
                    Email2 = con.Email2,
                    Email3 = con.Email3,
                    Email4 = null,
                    Telefono1 = con.Telefono1,
                    Telefono2 = con.Telefono2,
                    Telefono3 = con.Telefono3,
                    Telefono4 = null,
                    Observaciones = prove.Observaciones,
                    FechaUltimoContacto = prove.FechaUltimoContacto,
                    Nombres = com.Nombres,
                    Apellido = com.Apellido,
                    Direccion = prove.Direccion,
                    Provincia = prove.Localidad.Provincia.Nombre,
                    Localidad = prove.Localidad.Nombre,
                    CanalOperacion = pcaop.CanalOperacion.Descripcion,
                    Destinatario = pd.Destinatario.Descripcion,
                    Condicion = pc.Condicion.Descripcion,
                    AreaInfluencia = prove.AreaInfluencia.Descripcion,
                    Intermediario = prove.Intermediario,
                    Calificacion = prove.Calificacion,
                    GrupoDeCompras = com.GrupoDeCompras.Descripcion ?? "",
                    CodigoPostal = prove.CodigoPostal,
                    TipoTelefono1Id = con.TipoTelefono1Id,
                    TipoTelefono2Id = con.TipoTelefono2Id,
                    TipoTelefono3Id = con.TipoTelefono3Id,
                    TipoTelefono4Id = null,
                    NombreReferente = (con.Nombres == null && con.Apellido == null) ? "No Posee" : con.Nombres + " " + con.Apellido,
                    ClienteMOA = prove.ClienteMOA,
                    ProvinciaCompraNet = prove.LocalidadCompraNet.Provincia.Nombre ?? "",
                    LocalidadCompraNet = prove.LocalidadCompraNet.Nombre ?? "",
                    ClasificacionCompraNet = prove.ClasificacionCompraNet.Descripcion,
                    BoletoCompraNet = prove.BoletoCompraNet.Descripcion,
                    BolsaCompraNet = prove.BolsaCompraNet.Descripcion,
                    Consignatario = prove.Consignatario,
                    Comision = prove.ComisionPorcentaje,
                    PlanCanje = prove.PlanCanje,
                    RazonSocialComisionista = (prove.ComisionistaE == null) ? null : prove.ComisionistaE.RazonSocial,//(prove.ComisionistaId == null) ? "" : "Opera con comisionista.",
                    Comisionista = prove.Comisionista,
                    ComisionistaId = prove.ComisionistaId,
                    Deshabilitado = prove.Deshabilitado,
                    CuposConRiesgo = prove.CuposConRiesgo,
                    OperaConMATBA = prove.OperaConMATBA,
                    EstadoHomeId = (int)prove.EstadoHomeId,
                    EstadoHomeMensaje = prove.EstadoHomeMensaje,
                    EstadoHomeDescripcion = estHome.Descripcion
                };

            return resultado.ToList();
        }

        public virtual List<BasicoProveedor> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedorId, comercialId, equipo);
            }
        }
    }
}
