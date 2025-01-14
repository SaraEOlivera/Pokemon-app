using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using Negocio;
using Utilidades;

namespace Primera_lectura_DB
{
    public partial class frmPokemons : Form        
    {
        //prop privada
        private List<Pokemon> listaPokemon;

        public frmPokemons()
        {
            InitializeComponent();
        }

        private void frmPokemons_Load(object sender, EventArgs e)
        {
            cargar();
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
            habilitarBotones();
            btnVolver.Visible = false;
        }

        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPokemons.CurrentRow != null) 
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                cargarImagen(seleccionado.UrlImagen);
            }
            habilitarBotones();
        }
        private void cargar() 
        {
            PokemonDatos datos = new PokemonDatos();
            try
            {
                listaPokemon = datos.listar();
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumnas();
                cargarImagen(listaPokemon[0].UrlImagen);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumnas() 
        {
            dgvPokemons.Columns["UrlImagen"].Visible = false;
            dgvPokemons.Columns["Id"].Visible = false;
        }

        private void cargarImagen(string imagen) 
        {
            try
            {
                pbxPokemon.Load(imagen);

            }
            catch (Exception ex)
            {
                pbxPokemon.Load("https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png");

            }
        }

        private void dgvPokemons_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            FrmAltaPokemon alta = new FrmAltaPokemon();
            alta.ShowDialog();
            cargar();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvPokemons.CurrentRow == null || dgvPokemons.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Seleccioná un Pókemon para modificar");
                return;
            }
            Pokemon seleccionado;
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

            FrmAltaPokemon modificar = new FrmAltaPokemon(seleccionado);
            modificar.ShowDialog();
            cargar();
        }

        private void btnEliminacionFisica_Click(object sender, EventArgs e)
        {
            eliminar();
            habilitarBotones();

        }

        private void btnEliminacionLogica_Click(object sender, EventArgs e)
        {
            eliminar(true);
            habilitarBotones();

        }

        private void eliminar(bool logico = false) 
        {
            PokemonDatos datos = new PokemonDatos();
            Pokemon seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminar este pokemon?", "Eliminar Pokemon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;
                    if (logico)
                        datos.eliminarLogico(seleccionado.Id);
                    else
                        datos.eliminar(seleccionado.Id);
                    cargar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            //validar campo
            if (cboCampo.SelectedIndex < 0) 
            {
                MessageBox.Show("Seleccione una opción en Campo");
                return true;
            }
            //validar criterio
            if (cboCriterio.SelectedIndex < 0) 
            {
                MessageBox.Show("Seleccione una opción en Criterio");
                return true;
            }
            //validar filtro
            if (cboCampo.SelectedItem.ToString() == "Número") 
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))
                {
                    MessageBox.Show("Debe completar el campo filtro con un número");
                    return true;
                }

                if (!(Validaciones.soloNumeros(txtFiltroAvanzado.Text))) 
                {
                    MessageBox.Show("Debe ingresar solo números");
                    return true;
                }
                //validarEstadoBtns();
            }

            return false;
        }

    
        private void btnFiltroAvanzado_Click(object sender, EventArgs e)
        {
            PokemonDatos datos = new PokemonDatos();
            try
            {
                if (validarFiltro())
                    return;

                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;
                dgvPokemons.DataSource = datos.filtrar(campo, criterio, filtro);
                btnVolver.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            habilitarBotones();
        }

        private void txtFiltroRapido_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;
            string filtro = txtFiltroRapido.Text;

            if (filtro.Length >= 3)
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));
            else
                listaFiltrada = listaPokemon;

            dgvPokemons.DataSource = null;
            dgvPokemons.DataSource = listaFiltrada;
            btnVolver.Visible = true;

            ocultarColumnas();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();
            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            }
            else 
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con ");
                cboCriterio.Items.Add("Termina con ");
                cboCriterio.Items.Add("Contiene ");

            }
        }

        private void txtFiltroAvanzado_TextChanged(object sender, EventArgs e)
        {

        }

        private void habilitarBotones() 
        {

            if (dgvPokemons.Rows.Count == 0)
            {
                btnModificar.Enabled = false;
                btnEliminacionLogica.Enabled = false;
                btnEliminacionFisica.Enabled = false;
            }
            else 
            {
                btnModificar.Enabled = true;
                btnEliminacionLogica.Enabled = true;
                btnEliminacionFisica.Enabled = true;
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            cargar();
            btnVolver.Visible = false;
        }
    }

}
