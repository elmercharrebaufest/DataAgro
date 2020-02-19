
namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptCapacidadAlm
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptCapacidadAlm));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.textBox7 = new DataDynamics.ActiveReports.TextBox();
            this.textBox1 = new DataDynamics.ActiveReports.TextBox();
            this.textBox9 = new DataDynamics.ActiveReports.TextBox();
            this.textBox10 = new DataDynamics.ActiveReports.TextBox();
            this.textBox2 = new DataDynamics.ActiveReports.TextBox();
            this.line2 = new DataDynamics.ActiveReports.Line();
            this.line1 = new DataDynamics.ActiveReports.Line();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.textBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).BeginInit();
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
            this.textBox7,
            this.textBox1,
            this.textBox9,
            this.textBox10,
            this.textBox2,
            this.line2,
            this.line1});
            this.detail.Height = 0.2159166F;
            this.detail.KeepTogether = true;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            this.detail.BeforePrint += new System.EventHandler(this.Detail_BeforePrint);
            // 
            // textBox7
            // 
            this.textBox7.DataField = "Toneladas";
            this.textBox7.Height = 0.205F;
            this.textBox7.Left = 0F;
            this.textBox7.Name = "textBox7";
            this.textBox7.Style = "text-align: center";
            this.textBox7.Text = null;
            this.textBox7.Top = 0F;
            this.textBox7.Width = 1.006F;
            // 
            // textBox1
            // 
            this.textBox1.CanShrink = true;
            this.textBox1.DataField = "Localidad";
            this.textBox1.Height = 0.205F;
            this.textBox1.Left = 1.006F;
            this.textBox1.Name = "textBox1";
            this.textBox1.Style = "text-align: center";
            this.textBox1.Text = null;
            this.textBox1.Top = 0F;
            this.textBox1.Width = 3.018F;
            // 
            // textBox9
            // 
            this.textBox9.DataField = "Propio";
            this.textBox9.Height = 0.205F;
            this.textBox9.Left = 6.764F;
            this.textBox9.Name = "textBox9";
            this.textBox9.Style = "text-align: center";
            this.textBox9.Text = null;
            this.textBox9.Top = 0F;
            this.textBox9.Width = 0.599F;
            // 
            // textBox10
            // 
            this.textBox10.CountNullValues = true;
            this.textBox10.DataField = "Alquilado";
            this.textBox10.Height = 0.205F;
            this.textBox10.Left = 7.363F;
            this.textBox10.Name = "textBox10";
            this.textBox10.Style = "text-align: center";
            this.textBox10.Text = null;
            this.textBox10.Top = 0F;
            this.textBox10.Width = 0.7199997F;
            // 
            // textBox2
            // 
            this.textBox2.DataField = "Provincia";
            this.textBox2.Height = 0.205F;
            this.textBox2.Left = 4.024F;
            this.textBox2.Name = "textBox2";
            this.textBox2.Style = "text-align: center";
            this.textBox2.Text = null;
            this.textBox2.Top = 0F;
            this.textBox2.Width = 2.74F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.215F;
            this.line2.Width = 8.083F;
            this.line2.X1 = 0F;
            this.line2.X2 = 8.083F;
            this.line2.Y1 = 0.215F;
            this.line2.Y2 = 0.215F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0F;
            this.line1.Width = 8.083F;
            this.line1.X1 = 0F;
            this.line1.X2 = 8.083F;
            this.line1.Y1 = 0F;
            this.line1.Y2 = 0F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptCapacidadAlm
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.083F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.textBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.TextBox textBox7;
        private DataDynamics.ActiveReports.TextBox textBox1;
        private DataDynamics.ActiveReports.TextBox textBox9;
        private DataDynamics.ActiveReports.TextBox textBox10;
        private DataDynamics.ActiveReports.TextBox textBox2;
        private DataDynamics.ActiveReports.Line line2;
        private DataDynamics.ActiveReports.Line line1;
    }
}


