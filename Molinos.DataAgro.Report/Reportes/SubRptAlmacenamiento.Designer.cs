namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptAlmacenamiento
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptAlmacenamiento));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.textBox24 = new DataDynamics.ActiveReports.TextBox();
            this.textBox25 = new DataDynamics.ActiveReports.TextBox();
            this.textBox26 = new DataDynamics.ActiveReports.TextBox();
            this.textBox27 = new DataDynamics.ActiveReports.TextBox();
            this.textBox28 = new DataDynamics.ActiveReports.TextBox();
            this.label28 = new DataDynamics.ActiveReports.Label();
            this.label29 = new DataDynamics.ActiveReports.Label();
            this.label30 = new DataDynamics.ActiveReports.Label();
            this.label31 = new DataDynamics.ActiveReports.Label();
            this.label32 = new DataDynamics.ActiveReports.Label();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.textBox24)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox25)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox27)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label28)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label29)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label30)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label32)).BeginInit();
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
            this.textBox24,
            this.textBox25,
            this.textBox26,
            this.textBox27,
            this.textBox28,
            this.label28,
            this.label29,
            this.label30,
            this.label31,
            this.label32});
            this.detail.Height = 0.847416F;
            this.detail.Name = "detail";
            // 
            // textBox24
            // 
            this.textBox24.DataField = "CampañaAlm";
            this.textBox24.Height = 0.2F;
            this.textBox24.Left = 1.216F;
            this.textBox24.Name = "textBox24";
            this.textBox24.Text = null;
            this.textBox24.Top = 0.119F;
            this.textBox24.Width = 2.219F;
            // 
            // textBox25
            // 
            this.textBox25.DataField = "MaterialAlm";
            this.textBox25.Height = 0.2F;
            this.textBox25.Left = 4.97F;
            this.textBox25.Name = "textBox25";
            this.textBox25.Text = null;
            this.textBox25.Top = 0.119F;
            this.textBox25.Width = 2.281F;
            // 
            // textBox26
            // 
            this.textBox26.DataField = "ProvinciaAlm";
            this.textBox26.Height = 0.2F;
            this.textBox26.Left = 8.707999F;
            this.textBox26.Name = "textBox26";
            this.textBox26.Text = null;
            this.textBox26.Top = 0.119F;
            this.textBox26.Width = 1.865001F;
            // 
            // textBox27
            // 
            this.textBox27.DataField = "ToneladasAlm";
            this.textBox27.Height = 0.2F;
            this.textBox27.Left = 1.279F;
            this.textBox27.Name = "textBox27";
            this.textBox27.Text = null;
            this.textBox27.Top = 0.500001F;
            this.textBox27.Width = 2.156F;
            // 
            // textBox28
            // 
            this.textBox28.DataField = "ArrendadoPropioAlm";
            this.textBox28.Height = 0.2F;
            this.textBox28.Left = 5.699F;
            this.textBox28.Name = "textBox28";
            this.textBox28.Text = null;
            this.textBox28.Top = 0.500001F;
            this.textBox28.Width = 2.124F;
            // 
            // label28
            // 
            this.label28.Height = 0.2820002F;
            this.label28.HyperLink = null;
            this.label28.Left = 0.1370001F;
            this.label28.Name = "label28";
            this.label28.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label28.Text = "Campaña:";
            this.label28.Top = 0.119F;
            this.label28.Width = 0.918F;
            // 
            // label29
            // 
            this.label29.Height = 0.2819888F;
            this.label29.HyperLink = null;
            this.label29.Left = 3.902F;
            this.label29.Name = "label29";
            this.label29.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label29.Text = "Material:";
            this.label29.Top = 0.119F;
            this.label29.Width = 0.845F;
            // 
            // label30
            // 
            this.label30.Height = 0.2F;
            this.label30.HyperLink = null;
            this.label30.Left = 0.1430001F;
            this.label30.Name = "label30";
            this.label30.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label30.Text = "Toneladas:";
            this.label30.Top = 0.500001F;
            this.label30.Width = 0.9780042F;
            // 
            // label31
            // 
            this.label31.Height = 0.252F;
            this.label31.HyperLink = null;
            this.label31.Left = 3.939F;
            this.label31.Name = "label31";
            this.label31.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label31.Text = "Arrendado/propio:";
            this.label31.Top = 0.500001F;
            this.label31.Width = 1.541F;
            // 
            // label32
            // 
            this.label32.Height = 0.2F;
            this.label32.HyperLink = null;
            this.label32.Left = 7.562F;
            this.label32.Name = "label32";
            this.label32.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label32.Text = "Provincia:";
            this.label32.Top = 0.119F;
            this.label32.Width = 1.052F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptAlmacenamiento
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 12.52F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.textBox24)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox25)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label28)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label29)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label30)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.TextBox textBox24;
        private DataDynamics.ActiveReports.TextBox textBox25;
        private DataDynamics.ActiveReports.TextBox textBox26;
        private DataDynamics.ActiveReports.TextBox textBox27;
        private DataDynamics.ActiveReports.TextBox textBox28;
        private DataDynamics.ActiveReports.Label label28;
        private DataDynamics.ActiveReports.Label label29;
        private DataDynamics.ActiveReports.Label label30;
        private DataDynamics.ActiveReports.Label label31;
        private DataDynamics.ActiveReports.Label label32;
    }
}


