namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptProduccion
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptProduccion));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            this.textBox18 = new DataDynamics.ActiveReports.TextBox();
            this.textBox19 = new DataDynamics.ActiveReports.TextBox();
            this.textBox20 = new DataDynamics.ActiveReports.TextBox();
            this.textBox21 = new DataDynamics.ActiveReports.TextBox();
            this.textBox22 = new DataDynamics.ActiveReports.TextBox();
            this.label4 = new DataDynamics.ActiveReports.Label();
            this.label10 = new DataDynamics.ActiveReports.Label();
            this.label17 = new DataDynamics.ActiveReports.Label();
            this.label26 = new DataDynamics.ActiveReports.Label();
            this.label27 = new DataDynamics.ActiveReports.Label();
            ((System.ComponentModel.ISupportInitialize)(this.textBox18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox22)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label26)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label27)).BeginInit();
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
            this.textBox18,
            this.textBox19,
            this.textBox20,
            this.textBox21,
            this.textBox22,
            this.label4,
            this.label10,
            this.label17,
            this.label26,
            this.label27});
            this.detail.Height = 0.816166F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // textBox18
            // 
            this.textBox18.DataField = "Campaña";
            this.textBox18.Height = 0.2F;
            this.textBox18.Left = 1.182F;
            this.textBox18.Name = "textBox18";
            this.textBox18.Text = null;
            this.textBox18.Top = 0.09100001F;
            this.textBox18.Width = 2.438F;
            // 
            // textBox19
            // 
            this.textBox19.DataField = "Material";
            this.textBox19.Height = 0.2F;
            this.textBox19.Left = 4.949F;
            this.textBox19.Name = "textBox19";
            this.textBox19.Text = null;
            this.textBox19.Top = 0.09100001F;
            this.textBox19.Width = 2.374F;
            // 
            // textBox20
            // 
            this.textBox20.DataField = "ProvinciaProd";
            this.textBox20.Height = 0.2F;
            this.textBox20.Left = 8.686999F;
            this.textBox20.Name = "textBox20";
            this.textBox20.Text = null;
            this.textBox20.Top = 0.09100001F;
            this.textBox20.Width = 1.865001F;
            // 
            // textBox21
            // 
            this.textBox21.DataField = "Toneladas";
            this.textBox21.Height = 0.2F;
            this.textBox21.Left = 1.238F;
            this.textBox21.Name = "textBox21";
            this.textBox21.Text = null;
            this.textBox21.Top = 0.4719995F;
            this.textBox21.Width = 2.382F;
            // 
            // textBox22
            // 
            this.textBox22.DataField = "ArrendadoPropio";
            this.textBox22.Height = 0.2F;
            this.textBox22.Left = 5.678F;
            this.textBox22.Name = "textBox22";
            this.textBox22.Text = null;
            this.textBox22.Top = 0.4719995F;
            this.textBox22.Width = 2.322F;
            // 
            // label4
            // 
            this.label4.Height = 0.282F;
            this.label4.HyperLink = null;
            this.label4.Left = 0.1160001F;
            this.label4.Name = "label4";
            this.label4.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label4.Text = "Campaña:";
            this.label4.Top = 0.09100001F;
            this.label4.Width = 0.918F;
            // 
            // label10
            // 
            this.label10.Height = 0.2819888F;
            this.label10.HyperLink = null;
            this.label10.Left = 3.918F;
            this.label10.Name = "label10";
            this.label10.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label10.Text = "Material:";
            this.label10.Top = 0.09100001F;
            this.label10.Width = 0.845F;
            // 
            // label17
            // 
            this.label17.Height = 0.2F;
            this.label17.HyperLink = null;
            this.label17.Left = 0.1150001F;
            this.label17.Name = "label17";
            this.label17.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label17.Text = "Toneladas:";
            this.label17.Top = 0.4719995F;
            this.label17.Width = 0.9780049F;
            // 
            // label26
            // 
            this.label26.Height = 0.262F;
            this.label26.HyperLink = null;
            this.label26.Left = 3.918F;
            this.label26.Name = "label26";
            this.label26.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label26.Text = "Arrendado/ propio:";
            this.label26.Top = 0.4719995F;
            this.label26.Width = 1.541F;
            // 
            // label27
            // 
            this.label27.Height = 0.2F;
            this.label27.HyperLink = null;
            this.label27.Left = 7.541F;
            this.label27.Name = "label27";
            this.label27.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label27.Text = "Provincia:";
            this.label27.Top = 0.09100001F;
            this.label27.Width = 1.052F;
            // 
            // SubRptProduccion
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
            ((System.ComponentModel.ISupportInitialize)(this.textBox18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox22)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label26)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label27)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.TextBox textBox18;
        private DataDynamics.ActiveReports.TextBox textBox19;
        private DataDynamics.ActiveReports.TextBox textBox20;
        private DataDynamics.ActiveReports.TextBox textBox21;
        private DataDynamics.ActiveReports.TextBox textBox22;
        private DataDynamics.ActiveReports.Label label4;
        private DataDynamics.ActiveReports.Label label10;
        private DataDynamics.ActiveReports.Label label17;
        private DataDynamics.ActiveReports.Label label26;
        private DataDynamics.ActiveReports.Label label27;
    }
}


