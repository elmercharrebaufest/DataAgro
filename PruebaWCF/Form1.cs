using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Molinos.DataAgro.ServiceClient;
using Molinos.DataAgro.ServiceClient.DataAgroServicesProxy;

namespace PruebaWCF
{
    public partial class Form1 : Form
    {
        //---------------------------------------------------------
        //  Variables Privadas
        //---------------------------------------------------------

        private ServiceHelper mobjServiceHelper;

        private string mstrErrorMessage;

        //---------------------------------------------------------
        //  Constructor
        //---------------------------------------------------------

        public Form1()
        {
            InitializeComponent();
        }

        //---------------------------------------------------------
        //  Control de Eventos
        //---------------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
            mobjServiceHelper = new ServiceHelper();

            Renovar();
        }
                

        private void Form1_Shown(object sender, EventArgs e)
        {
            MostrarError(); 
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var id = ((DestinatarioIni)listBox1.SelectedItem).DestinatarioId;

                var oResult = mobjServiceHelper.TraerDestinatario(id);

                if (oResult.EntityErrors != null && oResult.EntityErrors.ListaErrores.Length > 0)
                {
                    MessageBox.Show(oResult.EntityErrors.ListaErrores[0].Message, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    this.textBox1.Text = oResult.Destinatario.Descripcion; 
                }
            }
        }


        private void butAgregar_Click(object sender, EventArgs e)
        {
            this.listBox1.SelectedIndex = -1;
             
            this.textBox1.Text = "";
            this.textBox1.Focus();   
        }


        private void butGrabar_Click(object sender, EventArgs e)
        {
            


            


            //var oDestinatario = new Destinatario();

            //if (listBox1.SelectedItem == null)
            //{
            //    oDestinatario.ObjectState = 0;
            //}
            //else
            //{
            //    oDestinatario.ObjectState = 2;
            //    oDestinatario.DestinatarioId = ((DestinatarioIni)listBox1.SelectedItem).DestinatarioId;
            //}

            //oDestinatario.Descripcion = this.textBox1.Text;

            //var oEntityErrors = mobjServiceHelper.GrabarDestinatario(oDestinatario);

            //if (oEntityErrors != null && oEntityErrors.ListaErrores.Length > 0)
            //{
            //    var mensaje = "";

            //    foreach (ErrorMessage oError in oEntityErrors.ListaErrores)
            //    {
            //        mensaje += oError.Message + Environment.NewLine;
            //    }

            //    MessageBox.Show(mensaje, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            //else
            //{
            //    Renovar();
            //    MostrarError();
            //}

        }

        private void butEliminar_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                var id = ((DestinatarioIni)listBox1.SelectedItem).DestinatarioId;

                var oEntityErrors = mobjServiceHelper.EliminarDestinatario(id); 

                if (oEntityErrors != null && oEntityErrors.ListaErrores.Length > 0)
                {
                    MessageBox.Show(oEntityErrors.ListaErrores[0].Message, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Renovar();
                    MostrarError(); 
                }
            }
        }
        
        //---------------------------------------------------------
        //  Metodos Privados
        //---------------------------------------------------------

        private void Renovar()
        {
            mstrErrorMessage = "";

            var oResult = mobjServiceHelper.TraerTodoDestinatario();

            if (oResult.EntityErrors != null && oResult.EntityErrors.ListaErrores.Length > 0)
            {
                mstrErrorMessage = oResult.EntityErrors.ListaErrores[0].Message;
            }
            else
            {
                this.listBox1.ValueMember = "DestinatarioId";
                this.listBox1.DisplayMember = "Descripcion";
                this.listBox1.DataSource = oResult.ResultIniDestinatario.Destinatario;
            }
        }


        private void MostrarError()
        {
            if (mstrErrorMessage.Length > 0)
            {
                MessageBox.Show(mstrErrorMessage, "Error !!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

    }


}
