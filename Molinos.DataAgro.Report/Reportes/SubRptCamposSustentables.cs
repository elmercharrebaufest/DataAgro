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
    public partial class SubRptCamposSustentables : DataDynamics.ActiveReports.ActiveReport
    {
        //--------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------

        public SubRptCamposSustentables()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

        }

        private void detail_Format(object sender, EventArgs e)
        {

        }

        private void Detail_BeforePrint(object sender, EventArgs e)
        {

            this.textBox1.Height = this.detail.Height;
            this.textBox1.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox1.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox2.Height = this.textBox1.Height;
            this.textBox2.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox2.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox3.Height = this.textBox1.Height;
            this.textBox3.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox3.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox4.Height = this.textBox1.Height;
            this.textBox4.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox4.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox5.Height = this.textBox1.Height;
            this.textBox5.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox5.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox7.Height = this.textBox1.Height;
            this.textBox7.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox7.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox9.Height = this.textBox1.Height;
            this.textBox9.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox9.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

            this.textBox10.Height = this.textBox1.Height;
            this.textBox10.Border.LeftColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.textBox10.Border.LeftStyle = DataDynamics.ActiveReports.BorderLineStyle.Solid;

        }

    }
}


