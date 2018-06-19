using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.DatosDelProveedor;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class EstadoProveedorManager : IEstadoProveedorManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------
        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;


        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void ActualizarProveedores()
        {
            try
            {
                
                var listaDeCuit = new List<Datos>();
                
                var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
                var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
                var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                
                /*
                var listProve = oProveedor
                            .Join(oProveedorComercial, x => x.ProveedorId, c => c.ProveedorId, (x, c) => new { P = x, PC = c })
                            .Join(oComercial, x => x.PC.ComercialId, c => c.ComercialId, (x, c) => new { x.P,x.PC, C = c })
                            .Select(x=> new Datos() {CUIT= x.P.CUIT,UsuarioDirectory = x.C.IdActiveDirectory } )
                            .Where (X=> X.CUIT == "30708104074")
                            .ToList();
*/

                var CUIT = oProveedor.Select(x => x.CUIT).ToList();
                var usuario = oComercial.Where(x=> x.ComercialId == 10).Select(x => x.IdActiveDirectory).ToList();


                var oProveedorEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().AsNoTracking();
                var ProveedorId = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
                //var ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                //var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();

                var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().ToList();
                

                /*var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().ToList();
                ProvEstados.ForEach(x => x.ObjectState = Constants.Object_Deleted);
                mobjUnitOfWork.SaveChanges();*/
                
                /*
                 foreach (var x in listProve)
                {
                    if (x.UsuarioDirectory.ToLower() == "NUNEZML".ToLower()) {
                    }*/
                    if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1")
                    {
                        string aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                        usuario.Add(aux);
                        //if (x.UsuarioDirectory.ToLower() == aux.ToLower())
                        //{
                        //    string aux2 = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                        //    listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = aux2 });
                        //}
                        //else
                        //{
                        //    listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = x.UsuarioDirectory });
                        //}

                    }
                /*    else{
                        listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = x.UsuarioDirectory });
                    }
                    

                }*/

                var list = new DatosProveedor().ObtenerDatosDeProveedorEstado(CUIT, usuario);

                if (list.Count > 0)
                {

                    Comercial comercial = null;
                    Estado Est = null;

                    foreach (var lista in list)
                    {
                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1")
                        {
                            string aux = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                            if (lista.USUARIO.ToLower() == aux.ToLower())
                            {
                                string auxNombreActual= ConfigurationManager.AppSettings["usuarioLaurastring"].ToString();
                                comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == auxNombreActual.ToLower());
                            }
                            else
                            {
                                comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                            }
                        }
                        else
                        {
                            comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                        }
                        

                        if (comercial != null)
                        {
                            Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                            var EstadoId = Est != null ? Est.EstadoId : 1;

                            var Actualizar = mobjUnitOfWork.SelStore<FakeClass>("DataAgro_ActualizarEstadoProveedor", comercial.ComercialId, lista.CUIT,
                                (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false), EstadoId);

                            var oResult = Actualizar.ToList();
                        }


                            //var oProveedors = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().FirstOrDefault(x => x.CUIT == lista.CUIT);

                            //if (oProveedors !=null )
                            //{ 
                            //    try
                            //    { 
                            //    id = id + 1;
                            //    ProveedorEstado pe = new ProveedorEstado();
                            //    var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                            //    pe.ComercialId = comercial.ComercialId;
                            //    var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                            //    pe.EstadoId = Est != null ? Est.EstadoId : 1;
                            //    pe.ObjectState = Constants.Object_Added;
                            //    pe.ProveedorEstadoId = id;
                            //    pe.ProveedorId = oProveedors.ProveedorId;

                            //    mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);

                            //    if (contad == 1000)
                            //    {
                            //        mobjUnitOfWork.SaveChanges();
                            //        contad = 0;
                            //    }
                            //    contad++;
                            //    }
                            //    catch(Exception ex)
                            //    {


                            //    }
                            //    }

                        }
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            Console.WriteLine("termino");

        }

        public void ActualizarProveedoresPorComercial(int ComercialId)
        {
            try
            {
                var listaDeCuit = new List<Datos>();
                var a = new List<string>();

                var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
                var oProveedorComercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
                var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();

                
                var listProve = mobjUnitOfWork.SelStore<Datos>("DataAgro_ActualizarComercialHome", ComercialId).ToList();
                var oComerciales = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
                var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();

                var ProvEstados = mobjUnitOfWork.Repository<ProveedorEstado>().Queryable().Where(x => x.ComercialId == ComercialId).ToList();
                ProvEstados.ForEach(x => x.ObjectState = Constants.Object_Deleted);

                foreach (var x in listProve)
                {
                    listaDeCuit.Add(new Datos() { CUIT = x.CUIT, UsuarioDirectory = x.UsuarioDirectory });
                }
                //listaDeCuit.Add(new Datos() { CUIT = oParam.basicos.cuit, UsuarioDirectory = usuarioPrueba });


                var list = new DatosProveedor().ObtenerDatosDeProveedor(listaDeCuit);

                if (list.Count > 0)
                {
                    foreach (var lista in list)
                    {


                        int id = mobjUnitOfWork.Repository<ProveedorEstado>()
                                .Queryable()
                                .AsNoTracking()
                                .Select(x => x.ProveedorEstadoId)
                                .DefaultIfEmpty(0)
                                .Max();

                        var oProveedors = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking().FirstOrDefault(x => x.CUIT == lista.CUIT);


                        ProveedorEstado pe = new ProveedorEstado();
                        var comercial = oComerciales.FirstOrDefault(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());
                        pe.ComercialId = comercial.ComercialId;
                        var Est = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault();
                        pe.EstadoId = Est != null ? Est.EstadoId : 1;
                        pe.ObjectState = Constants.Object_Added;
                        pe.ProveedorEstadoId = id + 1;
                        pe.ProveedorId = oProveedors.ProveedorId;

                        mobjUnitOfWork.Repository<ProveedorEstado>().SaveEntity(pe);



                    }

                    mobjUnitOfWork.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void ActualizarEstadoProveedor(List<Datos> listProve)
        {
            var cuit = "0";
            try
            {
                if (listProve.Count > 0)
                { 
                    var oEstados = mobjUnitOfWork.Repository<Estado>().Queryable().AsNoTracking().ToList();
                    if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
                    {
                        listProve.ForEach(x => x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"]);
                    }


                    //if (listProve.Any(x=> x.CUIT == "20043159381"))
                    //{
                    //    listProve = listProve.Where(x => x.CUIT == "20043159381").ToList();
                    //}
                    
                    var listaZMP = new List<ZMPES5140>();

                    listaZMP = new DatosProveedor().ObtenerDatosDeProveedor(listProve);

                    var lista = listaZMP.GroupBy(x => x.CUIT).ToList();

                    
                    
                    List<Proveedor> ListProveedor = new List<Proveedor>();
                    foreach (var i in lista)
                    {
                        try
                        {

                            if (i.Key == "30682519890")
                            {

                            }

                            var oprov = TraerComercial(i.Key);
                            if (oprov != null)
                            {
                                
                                
                                ListProveedor.Add(oprov);
                                var item = listaZMP.Where(x => x.CUIT == i.Key && x.STATUS.ToLower() == "operando" ).FirstOrDefault();
                                if (item == null)
                                {
                                    item = listaZMP.Where(x => x.CUIT == i.Key).FirstOrDefault();
                                }
                                oprov.ClienteMOA = (!String.IsNullOrEmpty(item.CLIENTE_MOA) ? true : false);
                                var Est = oEstados.Where(x => x.Descripcion.ToLower() == item.STATUS.ToLower()).FirstOrDefault();
                                oprov.EstadoId = Est != null ? Est.EstadoId : 1;

                                oprov.ObjectState = Constants.Object_Modified;
                                

                            }
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                    }
                    mobjUnitOfWork.Repository<Proveedor>().SaveRange(ListProveedor);
                    mobjUnitOfWork.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Proveedor TraerComercial(string CUIT)
        {
            var oProveedor = new Proveedor();

            oProveedor = mobjUnitOfWork.Repository<Proveedor>()
                                 .Queryable()
                                 .AsNoTracking()
                                 .Where(x => x.CUIT == CUIT)
                                 .SingleOrDefault();

            if (oProveedor != null)
            {
                oProveedor.ObjectState = Constants.Object_Modified;
            }

            return oProveedor;
        }

    }

    public class FakeClass
    {
        public int id { get; set; }
    }
}
