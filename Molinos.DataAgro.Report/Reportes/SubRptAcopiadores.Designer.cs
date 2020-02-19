namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptAcopiadores
    {
        private DataDynamics.ActiveReports.PageHeader pageHeader;
        private DataDynamics.ActiveReports.Detail detail;
        private DataDynamics.ActiveReports.PageFooter pageFooter;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #region ActiveReport Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptAcopiadores));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.textBox1 = new DataDynamics.ActiveReports.TextBox();
            this.textBox2 = new DataDynamics.ActiveReports.TextBox();
            this.textBox3 = new DataDynamics.ActiveReports.TextBox();
            this.textBox5 = new DataDynamics.ActiveReports.TextBox();
            this.textBox6 = new DataDynamics.ActiveReports.TextBox();
            this.textBox13 = new DataDynamics.ActiveReports.TextBox();
            this.textBox14 = new DataDynamics.ActiveReports.TextBox();
            this.line1 = new DataDynamics.ActiveReports.Line();
            this.line2 = new DataDynamics.ActiveReports.Line();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = 0F;
            this.pageHeader.Name = "pageHeader";
            // 
            // detail
            // 
            this.detail.ColumnSpacing = 0F;
            this.detail.Controls.AddRange(new DataDynamics.ActiveReports.ARControl[] {
            this.textBox1,
            this.textBox2,
            this.textBox3,
            this.textBox5,
            this.textBox6,
            this.textBox13,
            this.textBox14,
            this.line1,
            this.line2});
            this.detail.Height = 0.22025F;
            this.detail.KeepTogether = true;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            this.detail.BeforePrint += new System.EventHandler(this.Detail_BeforePrint);
            // 
            // textBox1
            // 
            this.textBox1.DataField = "Grano";
            this.textBox1.Height = 0.218F;
            this.textBox1.Left = 0F;
            this.textBox1.Name = "textBox1";
            this.textBox1.Style = "text-align: center";
            this.textBox1.CanGrow = true;
            this.textBox1.CanShrink = true;
            this.textBox1.Text = null;
            this.textBox1.Top = 0F;
            this.textBox1.Width = 1.006F;
            // 
            // textBox2
            // 
            this.textBox2.DataField = "Superficie";
            this.textBox2.Height = 0.218F;
            this.textBox2.Left = 1.006F;
            this.textBox2.Name = "textBox2";
            this.textBox2.Style = "text-align: center";
            this.textBox2.Text = null;
            this.textBox2.Top = 0F;
            this.textBox2.Width = 1.221F;
            // 
            // textBox3
            // 
            this.textBox3.DataField = "Toneladas";
            this.textBox3.Height = 0.218F;
            this.textBox3.Left = 2.227F;
            this.textBox3.Name = "textBox3";
            this.textBox3.Style = "text-align: center";
            this.textBox3.Text = null;
            this.textBox3.Top = 0.002F;
            this.textBox3.Width = 0.6840003F;
            // 
            // textBox5
            // 
            this.textBox5.DataField = "Localidad";
            this.textBox5.Height = 0.218F;
            this.textBox5.Left = 2.911F;
            this.textBox5.Name = "textBox5";
            this.textBox5.Style = "text-align: center";
            this.textBox5.Text = null;
            this.textBox5.Top = 0.002F;
            this.textBox5.Width = 1.941F;
            // 
            // textBox6
            // 
            this.textBox6.DataField = "Provincia";
            this.textBox6.Height = 0.218F;
            this.textBox6.Left = 4.852F;
            this.textBox6.Name = "textBox6";
            this.textBox6.Style = "text-align: center";
            this.textBox6.Text = null;
            this.textBox6.Top = 0F;
            this.textBox6.Width = 1.912F;
            // 
            // textBox13
            // 
            this.textBox13.DataField = "Propio";
            this.textBox13.Height = 0.218F;
            this.textBox13.Left = 6.764F;
            this.textBox13.Name = "textBox13";
            this.textBox13.Style = "text-align: center";
            this.textBox13.Text = null;
            this.textBox13.Top = 0F;
            this.textBox13.Width = 0.599F;
            // 
            // textBox14
            // 
            this.textBox14.DataField = "Alquilado";
            this.textBox14.Height = 0.218F;
            this.textBox14.Left = 7.363F;
            this.textBox14.Name = "textBox14";
            this.textBox14.Style = "text-align: center";
            this.textBox14.Text = null;
            this.textBox14.Top = 0.002F;
            this.textBox14.Width = 0.7199997F;
            // 
            // line1
            // 
            this.line1.Height = 0.002000004F;
            this.line1.Left = 0F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.228F;
            this.line1.Width = 8.083F;
            this.line1.X1 = 0F;
            this.line1.X2 = 8.083F;
            this.line1.Y1 = 0.23F;
            this.line1.Y2 = 0.228F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0F;
            this.line2.Width = 8.083F;
            this.line2.X1 = 0F;
            this.line2.X2 = 8.083F;
            this.line2.Y1 = 0F;
            this.line2.Y2 = 0F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptAcopiadores
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.046584F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion
        private DataDynamics.ActiveReports.TextBox textBox1;
        private DataDynamics.ActiveReports.TextBox textBox2;
        private DataDynamics.ActiveReports.TextBox textBox5;
        private DataDynamics.ActiveReports.TextBox textBox6;
        private DataDynamics.ActiveReports.TextBox textBox13;
        private DataDynamics.ActiveReports.TextBox textBox14;
        private DataDynamics.ActiveReports.Line line1;
        private DataDynamics.ActiveReports.Line line2;
        private DataDynamics.ActiveReports.TextBox textBox3;
    }
}


