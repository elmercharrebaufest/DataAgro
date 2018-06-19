using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Domain;

namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    ///  

    public partial class RptInformeComercial : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------
        
        public List<InformeComercialAcopiadores> CapProduccion = new List<InformeComercialAcopiadores>();
        public List<InformeComercialAcopiadores> CapAlmacenamiento = new List<InformeComercialAcopiadores>();
     

        public RptInformeComercial()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
          
        }

        private void detail_Format(object sender, EventArgs e)
        {           
            this.SubRptAcopiadores.Report = new SubRptAcopiadores();
            SubRptAcopiadores.Report.DataSource = this.CapProduccion;
            if (this.CapProduccion.Count == 0)
            {
                this.label51.Visible = false;
                this.label52.Visible = false;
                this.label53.Visible = false;
                this.label54.Visible = false;
                this.label55.Visible = false;
                this.label56.Visible = false;
                this.label57.Visible = false;
                this.textBox18.Value = "No posee";
            }
            this.SubRptCapacidadAlm.Report = new SubRptCapacidadAlm();
            SubRptCapacidadAlm.Report.DataSource = this.CapAlmacenamiento;
            if (this.CapAlmacenamiento.Count == 0)
            {
                this.label18.Visible = false;
                this.label42.Visible = false;
                this.label43.Visible = false;
                this.label44.Visible = false;
                this.label45.Visible = false;
                this.textBox3.Value = "No posee";
            }
            this.SubRptFirma.Report = new SubRptFirma();         

        }

        private void pageHeader_Format(object sender, EventArgs e)
        {
          
        }
    }
}


