using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Helpers;

namespace Molinos.DataAgro.Entities
{
    public partial class NuevoProveedorDatos : IEntityKeyValid
    {
        //--------------------------------------------------------------------------------
        //   Implementacion de IEntityValid
        //--------------------------------------------------------------------------------

        public bool ValidateKey(List<ErrorMessage> oErrorMessages)
        {             

            return oErrorMessages.Count == 0;
        }


        public bool Validate(List<ErrorMessage> oErrorMessages)
        {
            for (int i = 0; i < this.Proveedor.basicos.emails.Count; i++)
            {
                if (!ValidacionMail.IsValidEmail(this.Proveedor.basicos.emails[i]))
                {
                    oErrorMessages.Add(new ErrorMessage("El formato del campo 'Email' " + i + " de los Datos Basicos es incorrecto", "Proveedor.Email"));
                }
            }

            if(this.Proveedor.contactocomercial != null)
            { 
            for (int i = 0; i < this.Proveedor.contactocomercial.Count; i++)
            {
                for (int j = 0; j < this.Proveedor.contactocomercial[i].emails.Count; j++)
                {
                    if (!ValidacionMail.IsValidEmail(this.Proveedor.contactocomercial[i].emails[j]))
                    {
                        oErrorMessages.Add(new ErrorMessage("El formato del campo 'Email' " + j + " del Contacto comercial es incorrecto", "Proveedor.Email"));
                    }
                }              
            }
            }
            if (string.IsNullOrEmpty(this.Proveedor.basicos.cuit))
            {
                oErrorMessages.Add(new ErrorMessage("El campo CUIT debe estar informado", "Proveedor.cuit"));
            }
            if (this.Proveedor.basicos.segmentacion == 0)
            {
                oErrorMessages.Add(new ErrorMessage("El campo Segmentacin debe estar informado", "Proveedor.segmentacion"));
            }
            if (string.IsNullOrEmpty(this.Proveedor.basicos.nomReferente))
            {
                oErrorMessages.Add(new ErrorMessage("El campo 'Nombre de Referente' debe estar informado", "Proveedor.nomReferente"));
            }
            for (int i = 0; i < this.Proveedor.basicos.telefonos.Count; i++)
            {
                if (!ValidacionMail.IsValidNumber(this.Proveedor.basicos.telefonos[i].telefono))
                {
                    if (i == 0)
                    {
                        oErrorMessages.Add(new ErrorMessage("El primer telefono debe ser numerico", "Proveedor.telefono"));
                    }
                    if (i == 1)
                    {
                        oErrorMessages.Add(new ErrorMessage("El segundo telefono debe ser numerico", "Proveedor.telefono"));
                    }
                    if (i == 2)
                    {
                        oErrorMessages.Add(new ErrorMessage("El tercer telefono debe ser numerico", "Proveedor.telefono"));
                    }
                    if (i == 3)
                    {
                        oErrorMessages.Add(new ErrorMessage("El cuarto telefono debe ser numerico", "Proveedor.telefono"));
                    }  
                }
            }
            for (int i = 0; i < this.Proveedor.basicos.telefonos.Count; i++)
            {
                if ((this.Proveedor.basicos.telefonos[i].telefono == null && this.Proveedor.basicos.telefonos[i].tipoTelefono != null) || (this.Proveedor.basicos.telefonos[i].tipoTelefono == null && this.Proveedor.basicos.telefonos[i].telefono != null))
                {
                    oErrorMessages.Add(new ErrorMessage("Para ingresar un telefono correctamente debe ingresar el tipo y el numero, o ninguno de los 2. Telefono: " + i + 1, "Proveedor.telefono"));
                }
            }
            if (!ValidacionMail.IsValidNumber(this.Proveedor.basicos.cuit))
            {
                oErrorMessages.Add(new ErrorMessage("El campo CUIT debe ser numerico", "Proveedor.cuit"));
            }

            return oErrorMessages.Count == 0;
        }
    }


    public class NuevoProveedor
    {
        public Basico basicos { get; set; }
        public Contacto contacto { get; set; }
        public Produccion produccion{ get; set; }
        public Almacenamiento almacenamiento { get; set; }
        public List<ContactosComercial> contactocomercial { get; set; }
        public int? ProveedorId { get; set; }
        
    }

    public class Basico
    {
        public string RazonSocial { get; set; }
        public string cuit { get; set; }
        public int nocliente { get; set; }
        public int segmentacion { get; set; }
        public List<string> emails { get; set; }
        public List<Telefono> telefonos { get; set; }       
        public string comentario { get; set; }
        public string nomReferente { get; set; }
        public int? calificacion { get; set; }
        public int? ProvinciaCompraNet { get; set; }
        public int? LocalidadCompraNet { get; set; }
        public int? ClasificacionCompraNet { get; set; }
        public int? BoletoCompraNet { get; set; }
        public int? BolsaCompraNet { get; set; }
    }

    public class Telefono
    {
        public int? tipoTelefono { get; set; }
        public string telefono { get; set; }       
    }

    public class Contacto
    {
        public string areaDeInfluencia { get; set; }
        public List<int> canalesOperacion { get; set; }
        public string direccion { get; set; }
        public string codpost { get; set; }
        public string comentarioContacto { get; set; }
        public List<int> entregaA { get; set; }
        public string intermediario { get; set; }
        public int? localidad { get; set; }
        public int? provincia { get; set; }
        public List<int> condPreferentes { get; set; }
    }

    public class Produccion
    {
        public List<CamposProduccion> CamposProduccion { get; set; }
        public double? TonsMaxAprobSojaSust { get; set; }
        public string habilitaoSojaSust { get; set; }
        public double? hasAprobSojaSust { get; set; }
        public double? volumenAnualTotalTns { get; set; }        
        public List<Objetivos> objetivos { get; set; }
        public List<Objetivos> eliminarobjetivos { get; set; }
        public int? CampoId { get; set; }

        public Produccion()
        {
            eliminarobjetivos = new List<Objetivos>();
        }
    }

    public class CamposProduccion
    {
        public string archivo { get; set; }
        public byte[] archivofile { get; set; }
        public string archivoFileResult { get; set; }
        public List<Granos> granos { get; set; }
        public List<Granos> eliminarproduccion { get; set; }
        public int item { get; set; }
        public int localidad { get; set; }
        public string localidadNom { get; set; }
        public int provincia { get; set; }
        public string provinciaNom { get; set; }
        public int? hectareas { get; set; }
        public string coordenadas { get; set; }
        public int? CampoId { get; set; }
    }

    public class Granos
    {
        public string grano { get; set; }
        public int granoId { get; set; }
        public int? hectareas { get; set; }
        public string hectareasNom { get; set; }
        public int? toneladas { get; set; }
        public int campañaId { get; set; }
    }

    public class Almacenamiento
    {
            public List<CamposAlmacenamiento> CamposAlmacenamiento { get; set; }
    }

    public class CamposAlmacenamiento
    {
        public int? CampoId { get; set; }
        public string archivo { get; set; }
        public byte[] archivofile { get; set; }
        public string archivoFileResult { get; set; }
        public List<GranosAlmacenamiento> granosAlmacenamiento { get; set; }
        public List<GranosAlmacenamiento> eliminargranoalmacenamiento { get; set; }
        public List<GranosAlmacenamientoGrano> granosAlmacenamientoGrano { get; set; }
        public List<GranosAlmacenamientoGrano> eliminargranoalmacenamientograno { get; set; }
        public int item { get; set; }
        public int localidad { get; set; }
        public string localidadNom { get; set; }
        public int provincia { get; set; }
        public string provinciaNom { get; set; }
        public string coordenadasAlmacenamiento { get; set; }
        //public int hectareasAlmacenamiento { get; set; }
    }

    public class GranosAlmacenamiento
    {

        public int campañaId { get; set; }
        public double? toneladasAlmacenamiento { get; set; }
        public bool? hasArrendadas { get; set; }
    }

    public class GranosAlmacenamientoGrano
    {
        public int granoId { get; set; }
        public int campañaId { get; set; }
        public double? toneladasAlmacenamiento { get; set; }
    }

    public class ContactosComercial
    {
        public int? contactoComercialId { get; set; }
        public string apellido { get; set; }
        public string cargo { get; set; }
        public List<string> emails { get; set; }
        public DateTime? fechaNacimiento { get; set; }
        public List<int> intereses { get; set; }
        public int? item { get; set; }
        public string nombre { get; set; }
        public string otrosIntereses { get; set; }
        public bool? principal { get; set; }
        public string puesto { get; set; }
        public List<Telefono> telefonos { get; set; }
    }

    public class Objetivos
    {
        public int granoId { get; set; }
        public string grano { get; set; }
        public int campañaId { get; set; }
        public string campaña { get; set; }
        public string toneladasObjetivo { get; set; }
    }

}
