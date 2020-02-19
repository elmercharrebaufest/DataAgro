namespace Molinos.DataAgro.Report
{
    /// <summary>
    /// Summary description for RptContacto.
    /// </summary>
    partial class RptAgendaActividad
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RptAgendaActividad));
            this.pageHeader = new DataDynamics.ActiveReports.PageHeader();
            this.Line1 = new DataDynamics.ActiveReports.Line();
            this.Line2 = new DataDynamics.ActiveReports.Line();
            this.Label2 = new DataDynamics.ActiveReports.Label();
            this.Label3 = new DataDynamics.ActiveReports.Label();
            this.label7 = new DataDynamics.ActiveReports.Label();
            this.label8 = new DataDynamics.ActiveReports.Label();
            this.label1 = new DataDynamics.ActiveReports.Label();
            this.label9 = new DataDynamics.ActiveReports.Label();
            this.label10 = new DataDynamics.ActiveReports.Label();
            this.picture1 = new DataDynamics.ActiveReports.Picture();
            this.reportInfo1 = new DataDynamics.ActiveReports.ReportInfo();
            this.detail = new DataDynamics.ActiveReports.Detail();
            this.txtCalificacion = new DataDynamics.ActiveReports.TextBox();
            this.txtRazonSocial = new DataDynamics.ActiveReports.TextBox();
            this.textBoxTelefono = new DataDynamics.ActiveReports.TextBox();
            this.textBoxUltimoContacto = new DataDynamics.ActiveReports.TextBox();
            this.line3 = new DataDynamics.ActiveReports.Line();
            this.textBoxOperable = new DataDynamics.ActiveReports.TextBox();
            this.textBoxCondicion = new DataDynamics.ActiveReports.TextBox();
            this.pageFooter = new DataDynamics.ActiveReports.PageFooter();
            this.Page = new DataDynamics.ActiveReports.ReportInfo();
            ((System.ComponentModel.ISupportInitialize)(this.Label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfo1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCalificacion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTelefono)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxUltimoContacto)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxOperable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCondicion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Page)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Controls.AddRange(new DataDynamics.ActiveReports.ARControl[] {
            this.Line1,
            this.Line2,
            this.Label2,
            this.Label3,
            this.label7,
            this.label8,
            this.label1,
            this.label9,
            this.label10,
            this.picture1,
            this.reportInfo1});
            this.pageHeader.Height = 1.629833F;
            this.pageHeader.Name = "pageHeader";
            // 
            // Line1
            // 
            this.Line1.Height = 0F;
            this.Line1.Left = 0F;
            this.Line1.LineColor = System.Drawing.Color.DarkGreen;
            this.Line1.LineWeight = 1F;
            this.Line1.Name = "Line1";
            this.Line1.Top = 1.544F;
            this.Line1.Width = 12.989F;
            this.Line1.X1 = 0F;
            this.Line1.X2 = 12.989F;
            this.Line1.Y1 = 1.544F;
            this.Line1.Y2 = 1.544F;
            // 
            // Line2
            // 
            this.Line2.Height = 0F;
            this.Line2.Left = 0F;
            this.Line2.LineColor = System.Drawing.Color.DarkGreen;
            this.Line2.LineWeight = 1F;
            this.Line2.Name = "Line2";
            this.Line2.Top = 1.151F;
            this.Line2.Width = 12.989F;
            this.Line2.X1 = 0F;
            this.Line2.X2 = 12.989F;
            this.Line2.Y1 = 1.151F;
            this.Line2.Y2 = 1.151F;
            // 
            // Label2
            // 
            this.Label2.Height = 0.2F;
            this.Label2.HyperLink = null;
            this.Label2.Left = 2.47F;
            this.Label2.Name = "Label2";
            this.Label2.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.Label2.Text = "Detalle de Actividad";
            this.Label2.Top = 1.243F;
            this.Label2.Width = 2.219F;
            // 
            // Label3
            // 
            this.Label3.Height = 0.2F;
            this.Label3.HyperLink = null;
            this.Label3.Left = 0.114F;
            this.Label3.Name = "Label3";
            this.Label3.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.Label3.Text = "Proveedor";
            this.Label3.Top = 1.243F;
            this.Label3.Width = 2.188F;
            // 
            // label7
            // 
            this.label7.Height = 0.2F;
            this.label7.HyperLink = null;
            this.label7.Left = 4.896F;
            this.label7.Name = "label7";
            this.label7.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label7.Text = "Tipo de Actividad";
            this.label7.Top = 1.243F;
            this.label7.Width = 1.479F;
            // 
            // label8
            // 
            this.label8.Height = 0.2F;
            this.label8.HyperLink = null;
            this.label8.Left = 6.77F;
            this.label8.Name = "label8";
            this.label8.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label8.Text = "Fecha Recordatorio";
            this.label8.Top = 1.243F;
            this.label8.Width = 1.655012F;
            // 
            // label1
            // 
            this.label1.Height = 0.387F;
            this.label1.HyperLink = null;
            this.label1.Left = 5.269F;
            this.label1.Name = "label1";
            this.label1.Style = "color: DarkGreen; font-family: Calibri; font-size: 20.25pt; font-style: italic; f" +
    "ont-weight: bold; ddo-char-set: 0";
            this.label1.Text = "Agenda de Actividades";
            this.label1.Top = 0.12F;
            this.label1.Width = 2.801001F;
            // 
            // label9
            // 
            this.label9.Height = 0.2F;
            this.label9.HyperLink = null;
            this.label9.Left = 10.6F;
            this.label9.Name = "label9";
            this.label9.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label9.Text = "Contacto Comercial";
            this.label9.Top = 1.243F;
            this.label9.Width = 1.74F;
            // 
            // label10
            // 
            this.label10.Height = 0.2F;
            this.label10.HyperLink = null;
            this.label10.Left = 8.749F;
            this.label10.Name = "label10";
            this.label10.Style = "font-size: 12pt; font-weight: bold; ddo-char-set: 0";
            this.label10.Text = "Comercial";
            this.label10.Top = 1.243F;
            this.label10.Width = 1.604F;
            // 
            // picture1
            // 
            this.picture1.Height = 1.039F;
            this.picture1.ImageData = ((System.IO.Stream)(resources.GetObject("picture1.ImageData")));
            this.picture1.Left = 0.196F;
            this.picture1.Name = "picture1";
            this.picture1.Top = 0F;
            this.picture1.Width = 2.051F;
            // 
            // reportInfo1
            // 
            this.reportInfo1.FormatString = "{RunDateTime:M/d/yyyy h:mm}";
            this.reportInfo1.Height = 0.2F;
            this.reportInfo1.Left = 11.531F;
            this.reportInfo1.Name = "reportInfo1";
            this.reportInfo1.Style = "";
            this.reportInfo1.Top = 0F;
            this.reportInfo1.Width = 2.01F;
            // 
            // detail
            // 
            this.detail.ColumnSpacing = 0F;
            this.detail.Controls.AddRange(new DataDynamics.ActiveReports.ARControl[] {
            this.txtCalificacion,
            this.txtRazonSocial,
            this.textBoxTelefono,
            this.textBoxUltimoContacto,
            this.line3,
            this.textBoxOperable,
            this.textBoxCondicion});
            this.detail.Height = 0.462F;
            this.detail.Name = "detail";
            // 
            // txtCalificacion
            // 
            this.txtCalificacion.DataField = "Detalle";
            this.txtCalificacion.Height = 0.2F;
            this.txtCalificacion.Left = 2.47F;
            this.txtCalificacion.Name = "txtCalificacion";
            this.txtCalificacion.Text = null;
            this.txtCalificacion.Top = 0.062F;
            this.txtCalificacion.Width = 2.219F;
            // 
            // txtRazonSocial
            // 
            this.txtRazonSocial.DataField = "RazonSocial";
            this.txtRazonSocial.Height = 0.2F;
            this.txtRazonSocial.Left = 0.114F;
            this.txtRazonSocial.Name = "txtRazonSocial";
            this.txtRazonSocial.Text = null;
            this.txtRazonSocial.Top = 0.062F;
            this.txtRazonSocial.Width = 2.188F;
            // 
            // textBoxTelefono
            // 
            this.textBoxTelefono.DataField = "TipoDeAcividad";
            this.textBoxTelefono.Height = 0.2F;
            this.textBoxTelefono.Left = 4.896F;
            this.textBoxTelefono.Name = "textBoxTelefono";
            this.textBoxTelefono.Text = null;
            this.textBoxTelefono.Top = 0.062F;
            this.textBoxTelefono.Width = 1.479F;
            // 
            // textBoxUltimoContacto
            // 
            this.textBoxUltimoContacto.DataField = "FechaHoraRecordatorio";
            this.textBoxUltimoContacto.Height = 0.2F;
            this.textBoxUltimoContacto.Left = 6.77F;
            this.textBoxUltimoContacto.Name = "textBoxUltimoContacto";
            this.textBoxUltimoContacto.Text = null;
            this.textBoxUltimoContacto.Top = 0.062F;
            this.textBoxUltimoContacto.Width = 1.655F;
            // 
            // line3
            // 
            this.line3.Height = 0.0009999871F;
            this.line3.Left = 0.03F;
            this.line3.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(100)))), ((int)(((byte)(0)))));
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.349F;
            this.line3.Width = 12.959F;
            this.line3.X1 = 0.03F;
            this.line3.X2 = 12.989F;
            this.line3.Y1 = 0.35F;
            this.line3.Y2 = 0.349F;
            // 
            // textBoxOperable
            // 
            this.textBoxOperable.DataField = "Apellido";
            this.textBoxOperable.Height = 0.2F;
            this.textBoxOperable.Left = 8.749001F;
            this.textBoxOperable.Name = "textBoxOperable";
            this.textBoxOperable.Style = "text-align: left";
            this.textBoxOperable.Text = null;
            this.textBoxOperable.Top = 0.062F;
            this.textBoxOperable.Width = 1.604F;
            // 
            // textBoxCondicion
            // 
            this.textBoxCondicion.DataField = "NombreContacto";
            this.textBoxCondicion.Height = 0.2F;
            this.textBoxCondicion.Left = 10.6F;
            this.textBoxCondicion.Name = "textBoxCondicion";
            this.textBoxCondicion.Text = null;
            this.textBoxCondicion.Top = 0.062F;
            this.textBoxCondicion.Width = 1.74F;
            // 
            // pageFooter
            // 
            this.pageFooter.Controls.AddRange(new DataDynamics.ActiveReports.ARControl[] {
            this.Page});
            this.pageFooter.Height = 1.125F;
            this.pageFooter.Name = "pageFooter";
            // 
            // Page
            // 
            this.Page.FormatString = "Page {PageNumber} of {PageCount}";
            this.Page.Height = 0.2F;
            this.Page.Left = 11.531F;
            this.Page.Name = "Page";
            this.Page.Style = "";
            this.Page.Top = 0F;
            this.Page.Width = 1.582999F;
            // 
            // RptAgendaActividad
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 13.24933F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.Label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.reportInfo1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCalificacion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRazonSocial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxTelefono)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxUltimoContacto)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxOperable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxCondicion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Page)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private DataDynamics.ActiveReports.Line Line1;
        private DataDynamics.ActiveReports.Line Line2;
        private DataDynamics.ActiveReports.Label Label2;
        private DataDynamics.ActiveReports.Label Label3;
        private DataDynamics.ActiveReports.TextBox txtCalificacion;
        private DataDynamics.ActiveReports.TextBox txtRazonSocial;
        private DataDynamics.ActiveReports.Label label7;
        private DataDynamics.ActiveReports.Label label8;
        private DataDynamics.ActiveReports.TextBox textBoxTelefono;
        private DataDynamics.ActiveReports.TextBox textBoxUltimoContacto;
        private DataDynamics.ActiveReports.Label label1;
        private DataDynamics.ActiveReports.Line line3;
        private DataDynamics.ActiveReports.ReportInfo Page;
        private DataDynamics.ActiveReports.Label label9;
        private DataDynamics.ActiveReports.Label label10;
        private DataDynamics.ActiveReports.TextBox textBoxOperable;
        private DataDynamics.ActiveReports.TextBox textBoxCondicion;
        private DataDynamics.ActiveReports.Picture picture1;
        private DataDynamics.ActiveReports.ReportInfo reportInfo1;
    }
}


