using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    ///  

    public partial class RptCartaDePresentacion : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------
        
        public List<CartaDePresentacionAcopiadores> CapProduccion = new List<CartaDePresentacionAcopiadores>();
        public List<CartaDePresentacionAcopiadores> CapAlmacenamiento = new List<CartaDePresentacionAcopiadores>();
     

        public RptCartaDePresentacion()
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
                     

        }

        private void pageHeader_Format(object sender, EventArgs e)
        {
          
        }
    }
}


