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
    public partial class SubRptAcopiadores : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public SubRptAcopiadores()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();           
           
        }
        private void detail_Format(object sender, EventArgs e)
        {

        }


        private void Detail_BeforePrint(object sender, EventArgs e) {



            this.textBox1.CanGrow = true;
            this.textBox1.CanShrink = true;
            this.textBox1.Height = this.detail.Height;
            
            
            this.textBox2.Height = this.textBox1.Height;
            this.textBox2.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox2.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            
            this.textBox3.Height = this.textBox1.Height;
            this.textBox3.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox3.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            
            this.textBox5.Height = this.textBox1.Height;
            this.textBox5.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox5.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            
            this.textBox6.Height = this.textBox1.Height;
            this.textBox6.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox6.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            
            this.textBox13.Height = this.textBox1.Height;
            this.textBox13.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox13.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            
            this.textBox14.Height = this.textBox1.Height;
            this.textBox14.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox14.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;
            

        }
    }
}


