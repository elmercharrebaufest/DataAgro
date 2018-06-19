using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    public partial class SubRptProduccion : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public SubRptProduccion()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();           
           
        }

        private void detail_Format(object sender, EventArgs e)
        {

        }
    }
}


