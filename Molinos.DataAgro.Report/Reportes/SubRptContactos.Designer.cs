namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class SubRptContactos
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SubRptContactos));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.textBox1 = new DataDynamics.ActiveReports.TextBox();
            this.textBox5 = new DataDynamics.ActiveReports.TextBox();
            this.textBox4 = new DataDynamics.ActiveReports.TextBox();
            this.label5 = new DataDynamics.ActiveReports.Label();
            this.textBox3 = new DataDynamics.ActiveReports.TextBox();
            this.label12 = new DataDynamics.ActiveReports.Label();
            this.label13 = new DataDynamics.ActiveReports.Label();
            this.textBox2 = new DataDynamics.ActiveReports.TextBox();
            this.label15 = new DataDynamics.ActiveReports.Label();
            this.label11 = new DataDynamics.ActiveReports.Label();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label11)).BeginInit();
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
            this.textBox5,
            this.textBox4,
            this.label5,
            this.textBox3,
            this.label12,
            this.label13,
            this.textBox2,
            this.label15,
            this.label11});
            this.detail.Height = 0.6494993F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format_1);
            // 
            // textBox1
            // 
            this.textBox1.DataField = "Apellido";
            this.textBox1.Height = 0.2F;
            this.textBox1.Left = 1.243F;
            this.textBox1.Name = "textBox1";
            this.textBox1.Text = null;
            this.textBox1.Top = 0.063F;
            this.textBox1.Width = 2.519F;
            // 
            // textBox5
            // 
            this.textBox5.DataField = "Interes";
            this.textBox5.Height = 0.2F;
            this.textBox5.Left = 4.97F;
            this.textBox5.Name = "textBox5";
            this.textBox5.Text = null;
            this.textBox5.Top = 0.364F;
            this.textBox5.Width = 2.374002F;
            // 
            // textBox4
            // 
            this.textBox4.DataField = "Email1";
            this.textBox4.Height = 0.2F;
            this.textBox4.Left = 1.246002F;
            this.textBox4.Name = "textBox4";
            this.textBox4.Text = null;
            this.textBox4.Top = 0.364F;
            this.textBox4.Width = 2.516F;
            // 
            // label5
            // 
            this.label5.Height = 0.2F;
            this.label5.HyperLink = null;
            this.label5.Left = 0.137F;
            this.label5.Name = "label5";
            this.label5.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label5.Text = "Nombre:";
            this.label5.Top = 0.063F;
            this.label5.Width = 0.918F;
            // 
            // textBox3
            // 
            this.textBox3.DataField = "Telefono1";
            this.textBox3.Height = 0.2F;
            this.textBox3.Left = 8.788F;
            this.textBox3.Name = "textBox3";
            this.textBox3.Text = null;
            this.textBox3.Top = 0.063F;
            this.textBox3.Width = 1.73F;
            // 
            // label12
            // 
            this.label12.Height = 0.2819888F;
            this.label12.HyperLink = null;
            this.label12.Left = 3.94F;
            this.label12.Name = "label12";
            this.label12.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label12.Text = "Cargo:";
            this.label12.Top = 0.063F;
            this.label12.Width = 0.845F;
            // 
            // label13
            // 
            this.label13.Height = 0.2F;
            this.label13.HyperLink = null;
            this.label13.Left = 0.1370024F;
            this.label13.Name = "label13";
            this.label13.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label13.Text = "Emails:";
            this.label13.Top = 0.364F;
            this.label13.Width = 0.857F;
            // 
            // textBox2
            // 
            this.textBox2.DataField = "Cargo";
            this.textBox2.Height = 0.2F;
            this.textBox2.Left = 4.947999F;
            this.textBox2.Name = "textBox2";
            this.textBox2.Text = null;
            this.textBox2.Top = 0.063F;
            this.textBox2.Width = 2.396001F;
            // 
            // label15
            // 
            this.label15.Height = 0.2F;
            this.label15.HyperLink = null;
            this.label15.Left = 3.940002F;
            this.label15.Name = "label15";
            this.label15.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label15.Text = "Interés:";
            this.label15.Top = 0.364F;
            this.label15.Width = 0.896F;
            // 
            // label11
            // 
            this.label11.Height = 0.2F;
            this.label11.HyperLink = null;
            this.label11.Left = 7.569F;
            this.label11.Name = "label11";
            this.label11.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label11.Text = "Teléfonos:";
            this.label11.Top = 0.063F;
            this.label11.Width = 1.052F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // SubRptContactos
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
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.TextBox textBox1;
        private DataDynamics.ActiveReports.TextBox textBox5;
        private DataDynamics.ActiveReports.TextBox textBox4;
        private DataDynamics.ActiveReports.Label label5;
        private DataDynamics.ActiveReports.TextBox textBox3;
        private DataDynamics.ActiveReports.Label label12;
        private DataDynamics.ActiveReports.Label label13;
        private DataDynamics.ActiveReports.TextBox textBox2;
        private DataDynamics.ActiveReports.Label label15;
        private DataDynamics.ActiveReports.Label label11;
    }
}


