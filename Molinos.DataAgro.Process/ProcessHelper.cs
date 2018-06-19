using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Mastersoft.Framework.DataRepository;
using System.Configuration;
using Molinos.DataAgro.Business;

namespace Molinos.DataAgro.Process
{
    public class ProcessHelper
    {
        private MSContext mobjMSContext = new MSContext();

        RG2300Manager mobjRG2300Manager = new RG2300Manager();

        FacacopManager mobjFacacopManager = new FacacopManager();

        EstadoProveedorManager mobjEstadoManager = new EstadoProveedorManager();

        ComprasManager mobjComprasManager = new ComprasManager();

        ProveedorManager mobjProveedorManager = new ProveedorManager();

        ComercialManager mobComercialManager = new ComercialManager();

        CampañaMaterialManager mobCampañaMaterialManager = new CampañaMaterialManager();


        public void ActualizarComprasAyer()
        {
            mobjComprasManager.Inicializar(Util.GetMSContext());
            mobjComprasManager.ActualizarComprasAyer();
        }

        public void ActualizarGrupoCompras(){
            mobComercialManager.Inicializar(Util.GetMSContext());
            mobCampañaMaterialManager.Inicializar(Util.GetMSContext());
            mobjProveedorManager.Inicializar(Util.GetMSContext(), mobComercialManager, mobCampañaMaterialManager);
        }



        public void ProcesarRg2300()
        {

            mobjRG2300Manager.Inicializar(Util.GetMSContext());

            var str = ConfigurationManager.AppSettings["Rg2300"];

            StreamReader objReader = new StreamReader(str.ToString(), System.Text.Encoding.Default); //aca... hay que ver si el archivo siempre viene como ansi, o como verga sea
            

            string sLine = "";
            ArrayList arrText = new ArrayList();

            List<RG2300> lista = new List<RG2300>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j == 0)
                {
                    j = 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Rg = sLine.Split(';');
                        RG2300 obj = new RG2300();

                        obj.CUIT = !String.IsNullOrEmpty(Rg[0]) ? Rg[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        obj.RazonSocial = !String.IsNullOrEmpty(Rg[1]) ? Rg[1].Replace("\"", String.Empty) : String.Empty;
                        obj.Categoria = !String.IsNullOrEmpty(Rg[2]) ? Rg[2].Replace("\"", String.Empty) : String.Empty;
                        obj.Situacion = !String.IsNullOrEmpty(Rg[3]) ? Rg[3].Replace("\"", String.Empty) : String.Empty;
                        obj.CBU = !String.IsNullOrEmpty(Rg[4]) ? Rg[4].Replace("\"", String.Empty) : String.Empty;
                        obj.FechaActCBU = !String.IsNullOrEmpty(Rg[5]) ? (DateTime?)DateTime.Parse(Rg[5]) : null;
                        obj.FechaPubInclusion = !String.IsNullOrEmpty(Rg[6]) ? (DateTime?)DateTime.Parse(Rg[6]) : null;
                        obj.FechaPubSuspension = !String.IsNullOrEmpty(Rg[7]) ? (DateTime?)DateTime.Parse(Rg[7]) : null;
                        obj.FechaLevSuspension = !String.IsNullOrEmpty(Rg[8]) ? (DateTime?)DateTime.Parse(Rg[8]) : null;
                        obj.FechaNotExclusion = !String.IsNullOrEmpty(Rg[9]) ? (DateTime?)DateTime.Parse(Rg[9]) : null;
                        obj.FechaActRegistro = !String.IsNullOrEmpty(Rg[10]) ? (DateTime?)DateTime.Parse(Rg[10]) : null;
                        obj.Observaciones = !String.IsNullOrEmpty(Rg[11]) ? Rg[11].Replace("\"", String.Empty) : String.Empty;
                        obj.FechaGeneracion = !String.IsNullOrEmpty(Rg[12]) ? (DateTime?)DateTime.Parse(Rg[12]) : null;

                        if (obj.CUIT.Trim() == "20066104185")
                        {
                            var a = 1;
                        }
                        lista.Add(obj);                        
                    }
                }
            }
            objReader.Close();

            try
            {
                int count;
                count = 1;
                int cantProcesar = 1000;
                bool procesar = true;
                var CantVecesRecorrer = lista.Count / 1000 + 1;

                while (count <= CantVecesRecorrer && procesar)
                {
                    Console.WriteLine("Entramos al while de afuera , count = " + count);
                    Console.WriteLine("El procesar es  = " + procesar);
                    procesar = mobjRG2300Manager.InsetarRG2300(lista, cantProcesar);
                    cantProcesar += 1000;
                    mobjRG2300Manager.Inicializar(Util.GetMSContext());
                    count += 1;
                }
                Console.WriteLine("Salimos del ultimo while");
            }
            catch (Exception EX)
            {
                throw;
            }
            //}


           // Console.ReadLine();

        }

        public void ProcesarFacacop()
        {
            mobjFacacopManager.Inicializar(Util.GetMSContext());

            var str = ConfigurationManager.AppSettings["Facacop"];

            StreamReader objReader = new StreamReader(str.ToString());

            string sLine = "";
            ArrayList arrText = new ArrayList();

            List<FACACOP> lista = new List<FACACOP>();

            var j = 0;

            while (sLine != null)
            {
                sLine = objReader.ReadLine();

                if (j < 3)
                {
                    j += 1;
                }
                else
                {
                    if (sLine != null)
                    {
                        var Fc = sLine.Split(',');
                        Fc = sLine.Split(new[] { ',' }, 4);
                        FACACOP obj = new FACACOP();

                        if (Fc[0] == "33710001249")
                        {

                        }
                        //ArrayList StringAux = new ArrayList();
                       

                        //var item1 = Fc[0];
                        //var item2 = Fc[1];
                        //var item3 = Fc[2];
                        //var iteam4 = Fc[3];

                        //StringAux = Fc.Where(x => x != Fc[0] && x != Fc[1] && x != Fc[2]).ToArray();

                        //string textoAux = StringAux.Join(",");

                        obj.CUIT = !String.IsNullOrEmpty(Fc[0]) ? Fc[0].Replace('"', '\0').Replace('\\', '\0') : String.Empty;
                        obj.Fecha1 = !String.IsNullOrEmpty(Fc[1]) ? DateTime.Parse(Fc[1]) : DateTime.Now;
                        obj.Fecha2 = !String.IsNullOrEmpty(Fc[2]) ? DateTime.Parse(Fc[2]) : DateTime.Now;
                        obj.ObservacionesEspeciales = !String.IsNullOrEmpty(Fc[3]) ? Fc[3] : String.Empty;

                        lista.Add(obj);
                    }
                }
            }
            objReader.Close();

            try
            {
                int count;
                count = 1;
                int cantProcesar = 1000;
                bool procesar = true;
                var CantVecesRecorrer = lista.Count / 1000 + 1;

                while (count <= CantVecesRecorrer && procesar)
                {
                    Console.WriteLine("Entramos al while de afuera , count = " + count);
                    Console.WriteLine("El procesar es  = " + procesar);
                    procesar = mobjFacacopManager.InsetarFacacop(lista, cantProcesar);
                    cantProcesar += 1000;
                    mobjRG2300Manager.Inicializar(Util.GetMSContext());
                    count += 1;
                }
                Console.WriteLine("Salimos del ultimo while");
            }
            catch (Exception ex)
            {
                throw;
            }

            //Console.ReadLine();
        }

        public void ActualizarEstadoDelProveedor()
        {
            mobjEstadoManager.Inicializar(Util.GetMSContext());
            mobjEstadoManager.ActualizarProveedores();
        }

        public void ActualizarComprasProveedor()
        {
            mobjComprasManager.Inicializar(Util.GetMSContext());
            mobjComprasManager.ActualizarCompras();
        }


    }
}
