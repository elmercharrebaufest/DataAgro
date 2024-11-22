using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report.Clases
{
    public class LstProveedor
    {
        //-----------------------------------------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------------------------------------

        private IReportesManager reportesManager;

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public LstProveedor(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------

        public string GenerarListado(StoredPorProveedorResult oParam)
        {
            var oRptProveedor = new RptProveedor();

            var oDatos = new List<RptProveedorInfo>();
            var dato = new RptProveedorInfo();

            var oRptContactosInfo = new List<RptContactosInfo>();
            var oRptProduccionInfo = new List<RptProduccionInfo>();
            var oRptAlmacenamientoInfo = new List<RptAlmacenamientoInfo>();
            var oRptObjetivosInfo = new List<RptObjetivosInfo>();

            var ContactosInfo = new RptContactosInfo();
            var ProduccionInfo = new RptProduccionInfo();
            var AlmacenamientoInfo = new RptAlmacenamientoInfo();
            var ObjetivosInfo = new RptObjetivosInfo();


            //Datos Basicos                 
            dato.CUIT = oParam.BasicoProveedorTraerPorProveedores[0].CUIT;
            dato.RazonSocial = oParam.BasicoProveedorTraerPorProveedores[0].RazonSocial;
            dato.Estado = oParam.BasicoProveedorTraerPorProveedores[0].Estado;
            dato.NombreReferente = string.IsNullOrEmpty(oParam.BasicoProveedorTraerPorProveedores[0].NombreReferente) ? "No posee" : oParam.BasicoProveedorTraerPorProveedores[0].NombreReferente;
            dato.AreaInfluencia = string.IsNullOrEmpty(oParam.BasicoProveedorTraerPorProveedores[0].AreaInfluencia) ? "No posee" : oParam.BasicoProveedorTraerPorProveedores[0].AreaInfluencia;
            dato.Calificacion = oParam.BasicoProveedorTraerPorProveedores[0].Calificacion == null ? "No posee" : oParam.BasicoProveedorTraerPorProveedores[0].Calificacion.ToString();
            dato.Segmentacion = oParam.BasicoProveedorTraerPorProveedores[0].Segmentacion;
            dato.Email1P = string.IsNullOrEmpty(oParam.BasicoProveedorTraerPorProveedores[0].Email1) ? "No posee" : oParam.BasicoProveedorTraerPorProveedores[0].Email1 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Email2 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Email3 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Email4;
            dato.Telefono1P = string.IsNullOrEmpty(oParam.BasicoProveedorTraerPorProveedores[0].Telefono1) ? "No posee" : oParam.BasicoProveedorTraerPorProveedores[0].Telefono1 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Telefono2 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Telefono3 + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Telefono4;


            dato.CodigoPostal = string.IsNullOrEmpty(oParam.DatosContacto.CodigoPostal) ? "No posee" : oParam.DatosContacto.CodigoPostal;
            dato.Intermediario = string.IsNullOrEmpty(oParam.DatosContacto.Intermediario) ? "No posee" : oParam.DatosContacto.Intermediario;
            dato.Direccion = string.IsNullOrEmpty(oParam.DatosContacto.Direccion) ? "No posee" : oParam.DatosContacto.Direccion + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Localidad + "   " + oParam.BasicoProveedorTraerPorProveedores[0].Provincia;

            if (oParam.CanalesDeOperacion.Count == 0)
            {
                dato.CanalOperacion = "No posee";
            }
            for (int j = 0; j < oParam.CanalesDeOperacion.Count; j++)
            {
                dato.CanalOperacion += oParam.CanalesDeOperacion[j].Descripcion + "   ";
            }
            if (oParam.ProveedorDestinatario.Count == 0)
            {
                dato.Destinatario = "No posee";
            }
            for (int k = 0; k < oParam.ProveedorDestinatario.Count; k++)
            {
                dato.Destinatario += oParam.ProveedorDestinatario[k].Descripcion + "   ";
            }
            if (oParam.ProveedorCondicion.Count == 0)
            {
                dato.Condicion = "No posee";
            }
            for (int z = 0; z < oParam.ProveedorCondicion.Count; z++)
            {
                dato.Condicion += oParam.ProveedorCondicion[z].Descripcion + "   ";
            }

            oDatos.Add(dato);

            for (int i = 0; i < oParam.ContactosComercialesTraerPorProveedores.Count; i++)
            {
                ContactosInfo.Apellido = oParam.ContactosComercialesTraerPorProveedores[i].Apellido + " " + oParam.ContactosComercialesTraerPorProveedores[i].Nombres;
                ContactosInfo.Cargo = oParam.ContactosComercialesTraerPorProveedores[i].Cargo;
                ContactosInfo.Email1 = oParam.ContactosComercialesTraerPorProveedores[i].Email1 + "   " + oParam.ContactosComercialesTraerPorProveedores[i].Email2 + "   " + oParam.ContactosComercialesTraerPorProveedores[i].Email3;
                ContactosInfo.Interes = oParam.ContactosComercialesTraerPorProveedores[i].Interes;
                ContactosInfo.Telefono1 = oParam.ContactosComercialesTraerPorProveedores[i].Telefono1 + "   " + oParam.ContactosComercialesTraerPorProveedores[i].Telefono2 + "   " + oParam.ContactosComercialesTraerPorProveedores[i].Telefono3;

                oRptContactosInfo.Add(ContactosInfo);
            }

            List<int?> campañas = oParam.CampoProduccionAcopioPorProveedores.GroupBy(x => x.CampañaId).OrderByDescending(x => x.Key).Take(2).Select(x => x.Key).ToList();
            var listCampañas = oParam.CampoProduccionAcopioPorProveedores.Where(x => campañas.Contains(x.CampañaId)).OrderByDescending(x => x.CampañaId);
            /*if (oParam.CampoProduccionAcopioPorProveedores.Count >= 2)
            {*/
            foreach (var campaña in listCampañas)
            {
                ProduccionInfo = new RptProduccionInfo();
                //int cant = oParam.CampoProduccionAcopioPorProveedores.Count;

                ProduccionInfo.ProvinciaProd = campaña.Provincia + "; " + campaña.Localidad; ;
                ProduccionInfo.LocalidadProd = campaña.Localidad;
                ProduccionInfo.ArrendadoPropio = ((campaña.ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
                ProduccionInfo.Material = (string.IsNullOrEmpty(campaña.Material) ? "No posee" : campaña.Material);
                ProduccionInfo.Toneladas = (campaña.Toneladas != null ? campaña.Toneladas : 0);
                ProduccionInfo.Campaña = (string.IsNullOrEmpty(campaña.Campaña) ? "No posee" : campaña.Campaña);
                oRptProduccionInfo.Add(ProduccionInfo);
            }
            /*ProduccionInfo = new RptProduccionInfo();
            ProduccionInfo.ProvinciaProd = oParam.CampoProduccionAcopioPorProveedores[cant - 2].Provincia + "; " + oParam.CampoProduccionAcopioPorProveedores[cant - 2].Localidad;
            ProduccionInfo.LocalidadProd = oParam.CampoProduccionAcopioPorProveedores[cant - 2].Localidad;
            ProduccionInfo.ArrendadoPropio = ((oParam.CampoProduccionAcopioPorProveedores[cant - 2].ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
            ProduccionInfo.Material = oParam.CampoProduccionAcopioPorProveedores[cant - 2].Material;
            ProduccionInfo.Toneladas = oParam.CampoProduccionAcopioPorProveedores[cant - 2].Toneladas;
            ProduccionInfo.Campaña = oParam.CampoProduccionAcopioPorProveedores[cant - 2].Campaña;
            oRptProduccionInfo.Add(ProduccionInfo);*/

            /* }
             else if (oParam.CampoProduccionAcopioPorProveedores.Count == 1)
             {
                 ProduccionInfo.ProvinciaProd = oParam.CampoProduccionAcopioPorProveedores[0].Provincia + "; " + oParam.CampoProduccionAcopioPorProveedores[0].Localidad; ;
                 ProduccionInfo.LocalidadProd = oParam.CampoProduccionAcopioPorProveedores[0].Localidad;
                 ProduccionInfo.ArrendadoPropio = ((oParam.CampoProduccionAcopioPorProveedores[0].ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
                 ProduccionInfo.Material = oParam.CampoProduccionAcopioPorProveedores[0].Material;
                 ProduccionInfo.Toneladas = oParam.CampoProduccionAcopioPorProveedores[0].Toneladas;
                 ProduccionInfo.Campaña = oParam.CampoProduccionAcopioPorProveedores[0].Campaña;

                 oRptProduccionInfo.Add(ProduccionInfo);
             }
             */


            List<int?> acopios = oParam.Acopio.GroupBy(x => x.CampañaId).OrderByDescending(x => x.Key).Take(2).Select(x => x.Key).ToList();
            var listAcopios = oParam.Acopio.Where(x => campañas.Contains(x.CampañaId)).OrderByDescending(x => x.CampañaId);
            /*if (oParam.Acopio.Count >= 2)
            {*/

            foreach (var campaña in listAcopios)
            {
                AlmacenamientoInfo = new RptAlmacenamientoInfo();
                //int cant = oParam.Acopio.Count;

                AlmacenamientoInfo.ProvinciaAlm = campaña.Provincia;
                AlmacenamientoInfo.LocalidadAlm = campaña.Localidad;
                AlmacenamientoInfo.ArrendadoPropioAlm = ((campaña.ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
                AlmacenamientoInfo.MaterialAlm = (string.IsNullOrEmpty(campaña.Material) ? "No posee" : campaña.Material);
                AlmacenamientoInfo.ToneladasAlm = (campaña.Toneladas != null ? campaña.Toneladas : 0);
                AlmacenamientoInfo.CampañaAlm = (string.IsNullOrEmpty(campaña.Campaña) ? "No posee" : campaña.Campaña);
                oRptAlmacenamientoInfo.Add(AlmacenamientoInfo);
            }
            /*AlmacenamientoInfo = new RptAlmacenamientoInfo();
            AlmacenamientoInfo.ProvinciaAlm = oParam.Acopio[cant - 2].Provincia;
            AlmacenamientoInfo.LocalidadAlm = oParam.Acopio[cant - 2].Localidad;
            AlmacenamientoInfo.ArrendadoPropioAlm = ((oParam.Acopio[cant - 2].ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
            AlmacenamientoInfo.MaterialAlm = oParam.Acopio[cant - 2].Material;
            AlmacenamientoInfo.ToneladasAlm = oParam.Acopio[cant - 2].Toneladas;
            AlmacenamientoInfo.CampañaAlm = oParam.Acopio[cant - 2].Campaña;
            oRptAlmacenamientoInfo.Add(AlmacenamientoInfo);*/
            /*}
            else if (oParam.Acopio.Count == 1)
            {
                AlmacenamientoInfo = new RptAlmacenamientoInfo();
                AlmacenamientoInfo.ProvinciaAlm = oParam.Acopio[0].Provincia;
                AlmacenamientoInfo.LocalidadAlm = oParam.Acopio[0].Localidad;
                AlmacenamientoInfo.ArrendadoPropioAlm = ((oParam.Acopio[0].ArrendadoPropio) == true) ? "Es Propio" : "Es Alquilado";
                AlmacenamientoInfo.MaterialAlm = oParam.Acopio[0].Material;
                AlmacenamientoInfo.ToneladasAlm = oParam.Acopio[0].Toneladas;
                AlmacenamientoInfo.CampañaAlm = oParam.Acopio[0].Campaña;

                oRptAlmacenamientoInfo.Add(AlmacenamientoInfo);
            }*/

            for (int i = 0; i < oParam.ObjetivosTraerPorProveedorId.Count; i++)
            {
                ObjetivosInfo = new RptObjetivosInfo();

                ObjetivosInfo.MaterialObjetivo = oParam.ObjetivosTraerPorProveedorId[i].Material;
                ObjetivosInfo.ToneladaObjetivo = oParam.ObjetivosTraerPorProveedorId[i].ToneladasObjetivos;

                oRptObjetivosInfo.Add(ObjetivosInfo);
            }

            oRptProveedor.Contactos = oRptContactosInfo;

            oRptProveedor.Producciones = oRptProduccionInfo;

            oRptProveedor.Almacenamientos = oRptAlmacenamientoInfo;

            oRptProveedor.Objetivos = oRptObjetivosInfo;


            oRptProveedor.DataSource = oDatos;

            oRptProveedor.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptProveedor.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Proveedor.pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


    }
}
