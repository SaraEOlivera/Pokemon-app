using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Negocio;
using dominio;
using Utilidades;

namespace Primera_lectura_DB
{
    public partial class frmDetalle : Form
    {
        private Pokemon seleccionado;
        public frmDetalle()
        {
            InitializeComponent();
        }

        public frmDetalle(Pokemon pkm)
        {
            InitializeComponent();
            this.seleccionado = pkm;
        }

        private void frmDetalle_Load(object sender, EventArgs e)
        {
            txtNumeroDetalle.Text = seleccionado.Numero.ToString();
            txtNombreDetalle.Text = seleccionado.Nombre;
            txtDescripcionDetalle.Text = seleccionado.Descripcion;
            txtTipoDetalle.Text = seleccionado.Tipo.Descripcion;
            txtDebilidadDetalle.Text = seleccionado.Debilidad.Descripcion;


            if (!(string.IsNullOrEmpty(seleccionado.UrlImagen))) 
            {
                try
                {
                    pboPokemonDetalle.Load(seleccionado.UrlImagen);
                }
                catch (Exception)
                {

                    pboPokemonDetalle.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");

                }
            }


        }

        private void btnVolverDetalle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
