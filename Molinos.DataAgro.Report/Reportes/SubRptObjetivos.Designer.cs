namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptObjetivos
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptObjetivos));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.textBox31 = new DataDynamics.ActiveReports.TextBox();
            this.textBox32 = new DataDynamics.ActiveReports.TextBox();
            this.label33 = new DataDynamics.ActiveReports.Label();
            this.label34 = new DataDynamics.ActiveReports.Label();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.textBox31)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox32)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label33)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label34)).BeginInit();
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
            this.textBox31,
            this.textBox32,
            this.label33,
            this.label34});
            this.detail.Height = 0.4724161F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // textBox31
            // 
            this.textBox31.DataField = "MaterialObjetivo";
            this.textBox31.Height = 0.2F;
            this.textBox31.Left = 1.114F;
            this.textBox31.Name = "textBox31";
            this.textBox31.Text = null;
            this.textBox31.Top = 0.075F;
            this.textBox31.Width = 2.219F;
            // 
            // textBox32
            // 
            this.textBox32.DataField = "ToneladaObjetivo";
            this.textBox32.Height = 0.2F;
            this.textBox32.Left = 4.97F;
            this.textBox32.Name = "textBox32";
            this.textBox32.Text = null;
            this.textBox32.Top = 0.07499959F;
            this.textBox32.Width = 2.021F;
            // 
            // label33
            // 
            this.label33.Height = 0.2F;
            this.label33.HyperLink = null;
            this.label33.Left = 3.902F;
            this.label33.Name = "label33";
            this.label33.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label33.Text = "Toneladas:";
            this.label33.Top = 0.07499959F;
            this.label33.Width = 0.96F;
            // 
            // label34
            // 
            this.label34.Height = 0.2819888F;
            this.label34.HyperLink = null;
            this.label34.Left = 0.1370001F;
            this.label34.Name = "label34";
            this.label34.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label34.Text = "Material:";
            this.label34.Top = 0.07499959F;
            this.label34.Width = 0.845F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptObjetivos
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
            ((System.ComponentModel.ISupportInitialize)(this.textBox31)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox32)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label33)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label34)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.TextBox textBox31;
        private DataDynamics.ActiveReports.TextBox textBox32;
        private DataDynamics.ActiveReports.Label label33;
        private DataDynamics.ActiveReports.Label label34;
    }
}


