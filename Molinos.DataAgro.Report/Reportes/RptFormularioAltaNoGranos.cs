using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    ///  

    public partial class RptFormularioAltaNoGranos : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------
        
       
        public RptFormularioAltaNoGranos()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
          
        }

        private void detail_Format(object sender, EventArgs e)
        {           
            

        }

        private void pageHeader_Format(object sender, EventArgs e)
        {
          
        }
    }
}


