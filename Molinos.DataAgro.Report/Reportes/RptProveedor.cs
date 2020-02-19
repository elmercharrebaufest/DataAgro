using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    /// 






    public partial class RptProveedor : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public List<RptContactosInfo> Contactos = new List<RptContactosInfo>();
        public List<RptProduccionInfo> Producciones = new List<RptProduccionInfo>();
        public List<RptAlmacenamientoInfo> Almacenamientos = new List<RptAlmacenamientoInfo>();
        public List<RptObjetivosInfo> Objetivos = new List<RptObjetivosInfo>();

        public RptProveedor()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
          
        }

        private void detail_Format(object sender, EventArgs e)
        {

            this.SubRptContactos.Report = new SubRptContactos();
            SubRptContactos.Report.DataSource = this.Contactos;
            if (this.Contactos.Count == 0)
            {
                this.textBox1.Text = "No posee";
            }
            this.SubRptProduccion.Report = new SubRptProduccion();
            SubRptProduccion.Report.DataSource = this.Producciones;
            if (this.Producciones.Count == 0)
            {
                this.textBox2.Text = "No posee";
            }
            this.SubRptAlmacenamiento.Report = new SubRptAlmacenamiento();
            SubRptAlmacenamiento.Report.DataSource = this.Almacenamientos;
            if (this.Almacenamientos.Count == 0)
            {
                this.textBox3.Text = "No posee";
            }
            this.SubRptObjetivos.Report = new SubRptObjetivos();
            SubRptObjetivos.Report.DataSource = this.Objetivos;
            if (this.Objetivos.Count == 0)
            {
                this.textBox4.Text = "No posee";
            }

        }

        private void pageHeader_Format(object sender, EventArgs e)
        {
          
        }
    }
}


