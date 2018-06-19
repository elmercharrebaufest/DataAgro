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
    public partial class SubRptAlmacenamiento : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public SubRptAlmacenamiento()
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


