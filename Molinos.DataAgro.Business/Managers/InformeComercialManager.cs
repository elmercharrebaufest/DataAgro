using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace Molinos.DataAgro.Business
{
    public class InformeComercialManager : IInformeComercialManager
    {
        private IRepositorio repositorio;
        private readonly ILogger logger;
        private IMailManager mailManager;
        private IHttpContextManager httpContextManager;

        public InformeComercialManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public List<InformeComercialMaterialDisponible> TraerInformeComercial(int ProveedorId)
        {
            return repositorio.SelStore<InformeComercialMaterialDisponible>("DataAgro_InformeComercial_Traer", 0, ProveedorId);

        }

        public List<InformeGeneradoList> TraerInformeComercialGenerado(int ProveedorId)
        {
            var query = repositorio.SelStore<InformeGeneradoList>("DataAgro_InformeComercial_TraerInformesGenerado", 0, ProveedorId);
            query.ForEach(x => x.Materiales = x.Materiales.Replace("|", @"<br>"));
            return query;
        }

        public InformeResult GrabarInformeComercial(ParamInformeComercial informe, int IdActiveDirectory, List<NuevoProduccion> nuevosCampos,
            List<NuevoAcopio> nuevosAcopios, ContactoComercial contactoComercial, string direccion, string codigoPostal, int? localidadId)
        {
            informe.OrigenDA = true;
            var oEntityErrors = new InformeResult();
            InformeComercial inf = new InformeComercial();
            // si de MOA Operaciones generan un nuevo informe para un proveedor nuevo
            if (nuevosCampos != null || nuevosAcopios != null)
            {
                informe.OrigenDA = false;
                var materiales = repositorio.Listar<Material, int>(x => x.MaterialId);
                var campañas = repositorio.Listar<Campaña, int>(x => x.CampañaId);
                var localidades = repositorio.Listar<Localidad, int>(x => x.LocalidadId);

                if (nuevosCampos != null)
                {
                    if (nuevosCampos.Any(a => !materiales.Contains(a.MaterialId)))
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage("No se pudo encontrar el material seleccionado"));
                    }
                    if (nuevosCampos.Any(a => !campañas.Contains(a.CampañaId)))
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage("No se pudo encontrar la campaña seleccionada"));
                    }
                    if (nuevosCampos.Any(a => !localidades.Contains(a.LocalidadId)))
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage("No se pudo encontrar la Localidad seleccionada"));
                    }
                }
                if (nuevosAcopios != null)
                {
                    if (nuevosAcopios.Any(a => !campañas.Contains(a.CampañaId)))
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage("No se pudo encontrar la campaña seleccionada"));
                    }
                    if (nuevosAcopios.Any(a => !localidades.Contains(a.LocalidadId)))
                    {
                        oEntityErrors.Errores.Add(new ErrorMessage("No se pudo encontrar la Localidad seleccionada"));
                    }
                }
                if (string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(codigoPostal) || localidadId == null)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("No completo la direccion"));
                }
                if (string.IsNullOrWhiteSpace(codigoPostal))
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("No completo el codigo postal"));
                }
                if (localidadId == null)
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("No completo la localidad"));
                }
                if (contactoComercial == null || string.IsNullOrWhiteSpace(contactoComercial.Apellido) || string.IsNullOrWhiteSpace(contactoComercial.Nombres)
                    || string.IsNullOrWhiteSpace(contactoComercial.Puesto) || string.IsNullOrWhiteSpace(contactoComercial.Telefono1))
                {
                    oEntityErrors.Errores.Add(new ErrorMessage("No completo los campos de Contacto"));
                }
                if (oEntityErrors.HayError)
                {
                    return oEntityErrors;
                }
                else
                {
                    var camposMaterial = repositorio.Listar<CampoMaterial>(x => x.Campo.ProveedorId == informe.ProveedorId && x.CampañaId == informe.CampañaId);
                    var campos = repositorio.Listar<CampoMaterial, Campo>(x => x.Campo, x => x.Campo.ProveedorId == informe.ProveedorId && x.CampañaId == informe.CampañaId);
                    repositorio.RemoverTodos(camposMaterial);
                    repositorio.RemoverTodos(campos);
                    if (nuevosCampos != null)
                    {
                        foreach (var item in nuevosCampos)
                        {
                            Campo campo = new Campo
                            {
                                LocalidadId = item.LocalidadId,
                                ArrendaPropia = item.ArrendaPropia ? 1 : 0,
                                HabilitadoSojaSustentable = false,
                                ProveedorId = informe.ProveedorId,
                                NroItem = 1,
                                KMZfile = null,
                                KMZnombre = null
                            };
                            CampoMaterial campoMaterial = new CampoMaterial
                            {
                                Campo = campo,
                                CampañaId = item.CampañaId,
                                Hectareas = item.Hectareas,
                                MaterialId = item.MaterialId,
                                Toneladas = item.Toneladas,
                                NroItem = 1
                            };
                            repositorio.Agregar(campoMaterial);
                        }
                    }
                    var acopiosCampaña = repositorio.Listar<AcopioCampaña>(x => x.Acopio.ProveedorId == informe.ProveedorId && x.CampañaId == informe.CampañaId);
                    var acopiosMaterial = repositorio.Listar<AcopioMaterial>(x => x.Acopio.ProveedorId == informe.ProveedorId && x.CampañaId == informe.CampañaId);
                    var acopios = repositorio.Listar<AcopioCampaña, Acopio>(x => x.Acopio, x => x.Acopio.ProveedorId == informe.ProveedorId && x.CampañaId == informe.CampañaId);
                    repositorio.RemoverTodos(acopiosCampaña);
                    repositorio.RemoverTodos(acopiosMaterial);
                    repositorio.RemoverTodos(acopios);
                    if (nuevosAcopios != null)
                    {
                        foreach (var item in nuevosAcopios)
                        {
                            Acopio acopio = new Acopio
                            {
                                LocalidadId = item.LocalidadId,
                                ProveedorId = informe.ProveedorId,
                                NroItem = 1,
                                KMZfile = null,
                                KMZnombre = null
                            };
                            AcopioCampaña acopioCampaña = new AcopioCampaña
                            {
                                Acopio = acopio,
                                CampañaId = item.CampañaId,
                                HasArrendadas = !item.ArrendaPropia,
                                Toneladas = item.Toneladas,
                                NroItem = 1
                            };
                            repositorio.Agregar(acopioCampaña);
                        }
                    }
                    var oProveedor = repositorio.Obtener<Proveedor>(informe.ProveedorId);
                    oProveedor.LocalidadId = localidadId;
                    oProveedor.Direccion = direccion;
                    oProveedor.CodigoPostal = codigoPostal;
                    oProveedor.Telefono1 = contactoComercial.Telefono1;
                    oProveedor.Email1 = string.IsNullOrWhiteSpace(oProveedor.Email1) ? contactoComercial.Email1 : oProveedor.Email1;

                    contactoComercial.ProveedorId = oProveedor.ProveedorId;
                    var contactos = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == informe.ProveedorId);
                    //repositorio.RemoverTodos(contactos);
                    if (contactos.Count > 0)
                    {
                        foreach (var item in contactos.Where(a => a.Email1 != contactoComercial.Email1))
                        {
                            item.EsPrincipal = false;
                        }
                        foreach (var item in contactos.Where(a => a.Email1 == contactoComercial.Email1))
                        {
                            item.EsPrincipal = true;
                            item.Apellido = contactoComercial.Apellido;
                            item.Cargo = contactoComercial.Cargo;
                            item.Nombres = contactoComercial.Nombres;
                            item.Telefono1 = string.IsNullOrWhiteSpace(contactoComercial.Telefono1) ? item.Telefono1 : contactoComercial.Telefono1;
                        }
                    }
                    else
                    {
                        contactoComercial.EsPrincipal = true;
                        repositorio.Agregar(contactoComercial);
                    }

                    repositorio.GuardarCambios();
                }
            }

            if (informe.InformeComercialId > 0)
            {
                var producciones = repositorio.Listar<InformeComercialProduccion>(x => x.InformeComercial.InformeComercialId == informe.InformeComercialId);

                if (producciones != null)
                {
                    foreach (var produccion in producciones)
                    {
                        repositorio.Remover(produccion);

                    }

                }

                var almacenes = repositorio.Listar<InformeComercialAlmacenamiento>(x => x.InformeComercial.InformeComercialId == informe.InformeComercialId);
                if (almacenes != null)
                {
                    foreach (var almacen in almacenes)
                    {
                        repositorio.Remover(almacen);

                    }
                }

                inf = repositorio.Obtener<InformeComercial>(x => x.InformeComercialId == informe.InformeComercialId);
            }

            // 1 - Tiene que grabar en informe Comercial
            #region Informe Comercial

            inf.ProveedorId = informe.ProveedorId;
            inf.EstadoId = (int)EnumEstadoInforme.Generado;
            inf.CampañaId = informe.InformeComercialId == 0 || informe.InformeComercialId == null ? informe.CampañaId : inf.Campaña.CampañaId;
            informe.CampañaId = inf.CampañaId ?? inf.Campaña.CampañaId;
            inf.FechaAlta = DateTime.Now;
            inf.Comercial = repositorio.Obtener<Comercial>(IdActiveDirectory);
            inf.EmplRelDep = informe.EmplRelDep;

            inf.EmplRelDepCant = informe.EmplRelDepCant;
            inf.Rodados = informe.Rodados;
            inf.RodadosOtros = informe.RodadosOtros;
            inf.Chacra = informe.Chacra;
            inf.AntigActividad = informe.AntigActividad;
            inf.ChacraOtros = informe.ChacraOtros;

            inf.ActuacionProd = informe.ActuacionProd;
            inf.ClienteAnt = informe.ClienteAnt;
            inf.Comentarios = informe.Comentarios;
            inf.DomicilioReal = informe.Domicilio;
            inf.OrigenDA = informe.OrigenDA;

            if (inf.InformeComercialId == 0)
            {
                inf = repositorio.Agregar(inf);
            }
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            oEntityErrors.InformeId = inf.InformeComercialId;
            #endregion

            // 2 - Tiene que grabar en informe Comercial Produccion
            foreach (var mat in informe.Materiales)
            {
                var produccion = repositorio.ListarConsulta(new TraerInformeComercialProduccion(informe.ProveedorId, informe.CampañaId, mat.MaterialId, inf));

                foreach (var produ in produccion)
                {
                    var informeProduccion = new InformeComercialProduccion()
                    {
                        InformeComercial = produ.InformeComercial,
                        Hectareas = produ.Hectareas != null ? produ.Hectareas : 0,
                        Localidad = produ.Localidad,
                        Material = produ.Material,
                        Propio = produ.Propio,
                        Alquilado = produ.Alquilado,
                        Toneladas = produ.Toneladas != null ? produ.Toneladas : 0
                    };
                    repositorio.Agregar(informeProduccion);
                }
            }
            // 3 - Tiene que grabar en informe Comercial Almacenamiento

            var almacenamiento = repositorio.ListarConsulta(new TraerInformeComercialAlmacenamiento(informe.ProveedorId, informe.CampañaId, inf));

            foreach (var produ in almacenamiento)
            {
                var informeAlmacenamiento = new InformeComercialAlmacenamiento()
                {
                    InformeComercial = produ.InformeComercial,
                    Localidad = produ.Localidad,
                    Toneladas = produ.Toneladas != null ? produ.Toneladas : 0,
                    Propia = !produ.Propia,
                    Alquilada = !produ.Alquilada
                };
                repositorio.Agregar(informeAlmacenamiento);
            }

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            oEntityErrors.InformeId = inf.InformeComercialId;
            return oEntityErrors;
        }

        public RptInformeComercialInfo GenerarInformeComercial(ParamInformeComercial informe, int InformeId)
        {
            var datos = new RptInformeComercialInfo();
            var oProveedor = repositorio.Obtener<Proveedor>(informe.ProveedorId);

            datos.RazonSocial = oProveedor.RazonSocial;
            datos.CUIT = oProveedor.CUIT;
            datos.DomLegal = oProveedor.Direccion;
            datos.Telefono = oProveedor.Telefono1;
            datos.Email = oProveedor.Email1;

            var grupoDeCompras = repositorio.Obtener<ProveedorComercial, int>(x => x.ProveedorId == informe.ProveedorId, x => x.Comercial.GrupoDeCompras.Id);

            switch (grupoDeCompras)
            {
                case (int)AreasDeCompras.CORREDORBSAS:
                    datos.CorrBsAs = "X";
                    break;
                case (int)AreasDeCompras.CORREDORROSARIO:
                    datos.CorrRos = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORCENTRO:
                    datos.OrIntCentro = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORNORTE:
                    datos.OrIntNorte = "X";
                    break;
                case (int)AreasDeCompras.ORIGINTERIORSUR:
                    datos.OrIntSur = "X";
                    break;
            }

            var contactos = repositorio.Listar<ContactoComercial, RptContactosInfo>(x =>
                new RptContactosInfo()
                {
                    Nombres = x.Nombres + " " + x.Apellido,
                    Cargo = x.Puesto,
                    Telefono1 = x.Telefono1,
                    Email1 = x.Email1,
                    EsPrincipal = x.EsPrincipal == true ? "X" : ""
                }, x => x.ProveedorId == informe.ProveedorId, 2, "EsPrincipal", DirOrden.Desc);

            if (contactos.Count >= 1)
            {
                datos.ContactoNombre1 = contactos[0].Nombres + " " + contactos[0].Apellido;
                datos.ContactoCargo1 = contactos[0].Cargo;
                datos.ContactoTelefono1 = contactos[0].Telefono1;
                datos.ContactoMail1 = contactos[0].Email1;
            }

            if (contactos.Count == 2)
            {
                datos.ContactoNombre2 = contactos[1].Nombres + " " + contactos[1].Apellido;
                datos.ContactoCargo2 = contactos[1].Cargo;
                datos.ContactoTelefono2 = contactos[1].Telefono1;
                datos.ContactoMail2 = contactos[1].Email1;
            }

            switch (oProveedor.Segmentacion.Grupo)
            {
                case "Acopiadores":
                    datos.Acopiadores = "X";
                    break;
                case "Canjeadores":
                    datos.Canjeadores = "X";
                    break;
                case "Corredores":
                    datos.Corredores = "X";
                    break;
                case "Exportadores":
                    datos.Exportadores = "X";
                    break;
                case "Grandes Cuentas":
                    datos.GrandesCuentas = "X";
                    break;
                case "Productores":
                    datos.Productores = "X";
                    break;
            }

            datos.Campaña = informe.Campaña;

            var ProduccionInfo = new InformeComercialAcopiadores();
            var AlmacenamientoInfo = new InformeComercialAcopiadores();

            var oRptProduccionInfo = repositorio.Listar<InformeComercialProduccion, InformeComercialAcopiadores>(x => new InformeComercialAcopiadores()
            {
                Grano = x.Material.Descripcion,
                Localidad = x.Localidad.Nombre,
                Provincia = x.Localidad.Provincia.Nombre,
                Toneladas = (int)x.Toneladas,
                Superficie = (int)x.Hectareas,
                Alquilado = (bool)x.Alquilado ? "X" : String.Empty,
                Propio = (bool)x.Propio ? "X" : String.Empty
            }, x => x.InformeComercial.InformeComercialId == InformeId);

            //// Maiz
            var TonMaiz = repositorio.ObtenerConsultaEscalar(new ObtenerToneladasPorMaterial(InformeId, 1));
            //// Soja
            var TonSoja = repositorio.ObtenerConsultaEscalar(new ObtenerToneladasPorMaterial(InformeId, 3));

            //// Trigo
            var TonTrigo = repositorio.ObtenerConsultaEscalar(new ObtenerToneladasPorMaterial(InformeId, 2));

            //// Girasol
            var TonGira = repositorio.ObtenerConsultaEscalar(new ObtenerToneladasPorMaterial(InformeId, 4));

            //// Girasol Alto
            var TonGiraAlto = repositorio.ObtenerConsultaEscalar(new ObtenerToneladasPorMaterial(InformeId, 5));

            datos.ToneladasTodo = "Maiz " + TonMaiz.ToString("N2") + " Tn. / Soja " + TonSoja.ToString("N2") + " Tn. / Trigo " + TonTrigo.ToString("N2") +
                " Tn. / Girasol " + TonGira.ToString("N2") + " Tn. / Girasol A. O. " + TonGiraAlto.ToString("N2") + " Tn. ";

            var oRptAlmacenamientoInfo = repositorio.Listar<InformeComercialAlmacenamiento, InformeComercialAcopiadores>(x => new InformeComercialAcopiadores()
            {
                Localidad = x.Localidad.Nombre,
                Provincia = x.Localidad.Provincia.Nombre,
                Toneladas = (int)x.Toneladas,
                Alquilado = (bool)x.Alquilada ? "X" : String.Empty,
                Propio = (bool)x.Propia ? "X" : String.Empty
            }, x => x.InformeComercialId == InformeId);

            datos.Antiguedad = informe.AntigActividad;

            if (informe.EmplRelDep)
            {
                datos.RelacionDependenciaSi = "X";
                datos.RelacionDependenciaCant = informe.EmplRelDepCant;
            }
            else
            {
                datos.RelacionDependenciaNo = "X";
            }

            switch (informe.Rodados)
            {
                case (int)EnumRodados.EquipamientoPropio:
                    datos.RodadosEquipamientoPropio = "X";
                    break;
                case (int)EnumRodados.Alquilado:
                    datos.RodadosAlquilado = "X";
                    break;
                case (int)EnumRodados.PropioAlquilado:
                    datos.RodadosPropioYAlquilado = "X";
                    break;
                case (int)EnumRodados.Otros:
                    datos.RodadosOtros = "X";
                    datos.RodadosOtrosTexto = informe.RodadosOtros;
                    break;
            }

            switch (informe.Chacra)
            {
                case (int)EnumChacra.PersonalPropio:
                    datos.ServicioPersonalPropio = "X";
                    break;
                case (int)EnumChacra.PersonalContratado:
                    datos.ServicioPersonalContratado = "X";
                    break;
                case (int)EnumChacra.PersonalPropioContratado:
                    datos.ServicioPropioYContratado = "X";
                    break;
                case (int)EnumChacra.Otros:
                    datos.ServicioOtros = "X";
                    datos.ServicioOtrosTexto = informe.ChacraOtros;
                    break;
            }

            datos.Antiguedad = informe.AntigActividad;

            datos.ActuacionProduccion = informe.ActuacionProd;

            datos.ClientesAnteriores = informe.ClienteAnt;

            datos.DomReal = informe.Domicilio;

            datos.Comentarios = informe.Comentarios;

            datos.CapProduccion = oRptProduccionInfo;

            datos.CapAlmacenaje = oRptAlmacenamientoInfo;

            return datos;
        }

        public ParamInformeComercial ReimprimirInformeComercial(int InformeId)
        {
            var oParanInforme = new ParamInformeComercial();

            try
            {

                var oInformeComercial = repositorio.Obtener<InformeComercial>(x => x.InformeComercialId == InformeId);

                oParanInforme.InformeComercialId = InformeId;
                oParanInforme.CampañaId = (int)oInformeComercial.Campaña.CampañaId;
                oParanInforme.ProveedorId = oInformeComercial.Proveedor.ProveedorId;
                oParanInforme.Campaña = oInformeComercial.Campaña.Descripcion;
                oParanInforme.AntigActividad = oInformeComercial.AntigActividad;
                oParanInforme.ActuacionProd = oInformeComercial.ActuacionProd;
                oParanInforme.ClienteAnt = oInformeComercial.ClienteAnt;
                oParanInforme.Domicilio = oInformeComercial.DomicilioReal;
                oParanInforme.Comentarios = oInformeComercial.Comentarios;

                if (oInformeComercial.Chacra != null)
                {
                    oParanInforme.Chacra = oInformeComercial.Chacra;
                    oParanInforme.ChacraOtros = oInformeComercial.ChacraOtros;
                }

                if (oInformeComercial.Rodados != null)
                {
                    oParanInforme.Rodados = oInformeComercial.Rodados;
                    oParanInforme.RodadosOtros = oInformeComercial.RodadosOtros;
                }

                if ((bool)oInformeComercial.EmplRelDep)
                {
                    oParanInforme.EmplRelDep = true;
                    oParanInforme.EmplRelDepCant = oInformeComercial.EmplRelDepCant;
                }
                else
                {
                    oParanInforme.EmplRelDep = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return oParanInforme;

        }

        public List<MaterialesModificacionInforme> TraerInformeMateriales(int InformeId)
        {
            return repositorio.SelStore<MaterialesModificacionInforme>("DataAgro_InformeComercial_TraerMaterialesAModificar", 0, InformeId);
        }

        public List<ReportesList> ListarReportes(ParamReportesIC oParam, List<int> equipo)
        {
            return repositorio.SelStore<ReportesList>("DataAgro_InformeComercial_Reporte", 0, oParam.Cuit, oParam.ComercialID, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParam.MaterialID, oParam.EstadoId);
        }

        public DataSourceResult TraerInformesFiltrados(DataSourceRequest filtro)
        {
            var result = repositorio.ObtenerConsultaEscalar(new TraerInformesSinFiltro());
            var resultado = result.AsQueryable<InformeList>().ToDataSourceResult<InformeList>(filtro);
            return resultado;
        }

        public List<InformeList> TraerInformesGenerados()
        {
            //var list = repositorio.SelStore<InformeList>("DataAgro_InformeComercial_TraerExcelGeneracion", 0);
            //list.ForEach(x =>
            //{
            //    if (x.Materiales != null) x.Materiales = x.Materiales.Replace("|", @"<br>");
            //});
            //return list;

            var informeComercial = repositorio.Listar<InformeComercial, InformeList>(x => new InformeList()
            {
                InformeComercialId = x.InformeComercialId,
                Cuit = x.Proveedor.CUIT,
                RazonSocial = x.Proveedor.RazonSocial,
                Campaña = x.Campaña.Descripcion,
                //Materiales = "",
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Seleccionado = false
            });

            foreach (var infCom in informeComercial)
            {
                int infC = infCom.InformeComercialId;

                var materiales = repositorio.Listar<InformeComercialProduccion, String>(x =>
                    x.Material.Descripcion
                , x => x.InformeComercial.InformeComercialId == infC);

                String material = "";
                int cantMat = materiales.Count();
                int contMat = 0;

                foreach (var mat in materiales)
                {
                    contMat += 1;
                    material = contMat == cantMat ? material + mat : material + mat + @"<br>";
                }
                //infCom.Materiales = material;
            }
            return informeComercial;
        }

        public void GuardarFechaDescargaInformeComercial(List<int> ids)
        {
            try
            {
                List<InformeComercial> oInformesComercial = repositorio.Listar<InformeComercial>(x => ids.Contains(x.InformeComercialId));

                foreach (var oInformeComercial in oInformesComercial)
                {
                    oInformeComercial.FechaDescarga = DateTime.Now;
                }

                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public List<ResultCapacidadProductiva> TraerCapacidadProductiva(string informes)
        {
            return repositorio.SelStore<ResultCapacidadProductiva>("DataAgro_InformeComercial_TraerCapacidadProductiva", 0, informes);
        }

        public int GrabarCapacidadProductiva(string informes)
        {
            var val = repositorio.SelStore<Result>("DataAgro_InformeComercial_GrabarCapacidadProductiva", 1, informes);
            return val.FirstOrDefault().Res;
        }

        public Resultado EliminarInformes(int InformeId)
        {
            var error = new Resultado();

            var produccion = repositorio.Listar<InformeComercialProduccion>(x => x.InformeComercialId == InformeId);
            repositorio.RemoverTodos(produccion);

            var almacenamiento = repositorio.Listar<InformeComercialAlmacenamiento>(x => x.InformeComercialId == InformeId);
            repositorio.RemoverTodos(almacenamiento);

            var informe = repositorio.Obtener<InformeComercial>(x => x.InformeComercialId == InformeId);
            if (informe != null)
            {
                repositorio.Remover(informe);
            }
            repositorio.GuardarCambios();

            return error;
        }


        public Resultado RespuestaDeSapCapacidadProductiva(string cuit, string Material, string Respuesta)
        {
            var error = new Resultado();

            try
            {
                var proveedorId = repositorio.Obtener<Proveedor, int>(x => x.CUIT == cuit, x => x.ProveedorId);

                var Mat = repositorio.Obtener<Material, CampaniaMaterialSAP>(x => x.Codigo == Material, z => new CampaniaMaterialSAP { MaterialId = z.MaterialId, CampaniaId = z.CampañaId });

                var oInformeComercialProduccionSave = repositorio.Obtener<InformeComercialProduccion>(x => x.InformeComercial.EstadoId == (int)EnumEstadoInforme.Enviado && x.InformeComercial.ProveedorId == proveedorId
                    && x.InformeComercial.CampañaId == Mat.CampaniaId && x.MaterialId == Mat.MaterialId);

                if (oInformeComercialProduccionSave == null)
                {
                    error.Errores.Add(new ErrorMessage($"No se encontro un informe comercial produccion con proveedor {proveedorId}, estado {(int)EnumEstadoInforme.Enviado}, campania {Mat.CampaniaId}, material {Mat.MaterialId}"));
                    return error;
                }
                if (Respuesta.Length > 0)
                {
                    oInformeComercialProduccionSave.RtaOkSap = false;
                    oInformeComercialProduccionSave.MensajeSap = Respuesta;
                }
                else
                {
                    oInformeComercialProduccionSave.RtaOkSap = true;
                    oInformeComercialProduccionSave.MensajeSap = string.Empty;
                }

                var list = repositorio.Obtener<InformeComercialProduccion>(x => x.InformeComercialId == oInformeComercialProduccionSave.InformeComercialId && (x.RtaOkSap == null || x.RtaOkSap == false));

                if (list == null)
                {
                    oInformeComercialProduccionSave.InformeComercial.EstadoId = (int)EnumEstadoInforme.Confirmado;
                }
                repositorio.GuardarCambios();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return error;
        }

        public void EnviarMailInformeComercial(string identificador)
        {
            var pdf = repositorio.Obtener<Reportes, ReportesDto>(x => x.Identificador == identificador, x => new ReportesDto { Identificador = x.Identificador, Contenido = x.Contenido, FileName = x.FileName });
            var razonSocial = pdf.FileName.Substring(pdf.FileName.IndexOf('-') + 1);
            razonSocial = razonSocial.Remove(razonSocial.Length - 4);
            try
            {
                var context = new DataAgroDbContext();
                repositorio = new RepositorioEF(context);
                mailManager = new MailManager(logger, repositorio);
                httpContextManager = new HttpContextManager();
                var pathLogo = httpContextManager.ObtenerPathLogoMail();

                LinkedResource res = new LinkedResource(pathLogo)
                {
                    ContentId = Guid.NewGuid().ToString()
                };

                string htmlBody = "";
                htmlBody += "En el presente mail se adjunta un nuevo informe comercial generado para " + razonSocial + ". <br />";

                htmlBody += "<br /> <br />  Saludos Cordiales," +
                    " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                    @"<img src='cid:" + res.ContentId + @"'/>" +
                    "<br /> <br /> www.molinosagro.com.ar";
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                alternateView.LinkedResources.Add(res);

                var mail = ConfigurationManager.AppSettings["EmailInformeComercial"].ToString().Split(';').ToList();
                
                var asunto = "Nuevo Informe Comercial:" + razonSocial;
                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                {
                    asunto = "Mail Pruebas - Nuevo Informe Comercial:" + razonSocial;
                }
                mailManager.EnviarMail(mail, asunto, "", null, alternateView, pdf.Contenido, "Informe Comercial"+razonSocial+".pdf");

            }
            catch (Exception e)
            {
                logger.Error("Error al enviar mail en EnviarMailInformeComercial", e.Message);
            }
        }

    }

    public class Result
    {
        public int Res { get; set; }
    }
}
