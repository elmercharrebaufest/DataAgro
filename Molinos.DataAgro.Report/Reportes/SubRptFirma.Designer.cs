namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptFirma
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptFirma));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.label44 = new DataDynamics.ActiveReports.Label();
            this.label42 = new DataDynamics.ActiveReports.Label();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.label44)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label42)).BeginInit();
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
            this.label44,
            this.label42});
            this.detail.Height = 0.8448334F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // label44
            // 
            this.label44.Height = 0.826F;
            this.label44.HyperLink = null;
            this.label44.Left = 0F;
            this.label44.Name = "label44";
            this.label44.Style = "font-size: 9.75pt; font-weight: normal; white-space: nowrap; ddo-char-set: 0; ddo" +
    "-font-vertical: none";
            this.label44.Text = "             VENDEDOR\r\n\r\nFirma:__________________\r\n\r\nAclaración:______________";
            this.label44.Top = 0F;
            this.label44.Width = 2.293F;
            // 
            // label42
            // 
            this.label42.Height = 0.826F;
            this.label42.HyperLink = null;
            this.label42.Left = 3.915F;
            this.label42.Name = "label42";
            this.label42.Style = "color: Black; font-size: 9.75pt; font-weight: normal; white-space: nowrap; ddo-ch" +
    "ar-set: 0";
            this.label42.Text = "             COMPRADOR\r\n\r\nFirma:__________________\r\n\r\nAclaración:______________";
            this.label42.Top = 0F;
            this.label42.Width = 2.293F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptFirma
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 6.270417F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.label44)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label42)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.Label label44;
        private DataDynamics.ActiveReports.Label label42;
    }
}


