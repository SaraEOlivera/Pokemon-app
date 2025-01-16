using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using Negocio;
using Utilidades;

namespace Primera_lectura_DB
{
    public partial class FrmAltaPokemon : Form
    {
        //ATRIBUTO
        private Pokemon pokemon = null;

        private OpenFileDialog archivo = null;

        //definicion de la clase
        public FrmAltaPokemon()
        {
            InitializeComponent();
        }

        public FrmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;
            Text = "Modificar Pokemon";
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        //valido campos del form de alta:
        private bool validarCamposAlta()
        {

            if (string.IsNullOrEmpty(txtNumero.Text))
            {
                MessageBox.Show("El campo Número es obligatorio");
                return true;
            }
            
            if (!(Validaciones.soloNumeros(txtNumero.Text)))
            {
                MessageBox.Show("Debe ingresar solo números en el campo Número");
                return true;
            }
            
            if (string.IsNullOrEmpty(txtNombre.Text))
            {
                MessageBox.Show("El campo Nombre es obligatorio");
                return true;
            }

            if (!(Validaciones.soloLetras(txtNombre.Text)))
            {
                MessageBox.Show("Debe ingresar solo letras en el campo Nombre");
                return true;
            }

            return false;
        }

        private void btnAceptar_Click(object sender, EventArgs e)   
        {
            //esta variable nuevoPokemon ya no es necesaria. Se reemplaza por el atributo
            //Pokemon nuevoPokemon = new Pokemon();
            PokemonDatos datos = new PokemonDatos();
            string nombre = txtNombre.Text;
                string imagenUrl = txtUrlImagen.Text;
            int numero;

            try
            {
                if (validarCamposAlta())
                    return;

                numero = int.Parse(txtNumero.Text);
                if (!(validarRepetidos(numero, nombre, imagenUrl)))
                    return;

                if (pokemon == null)
                    pokemon = new Pokemon();

                //si el campo está vacio, si la url  o ruta local no es valida , asigna placeholder
                if (string.IsNullOrWhiteSpace(imagenUrl) || !(validarUrl(imagenUrl) || validarRutaLocal(imagenUrl)))
                    pokemon.UrlImagen = "https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png";
                else
                     pokemon.UrlImagen = txtUrlImagen.Text;








                if (!(validarRepetidos(numero, nombre, imagenUrl)))
                    return;

                //carga del objeto
                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;


                //capturar valores de los desplegables:
                pokemon.Tipo = (Elementos)cboTipo.SelectedItem;
                pokemon.Debilidad = (Elementos)cboDebilidad.SelectedItem;

                if (pokemon.Id != 0) 
                {
                    datos.modificar(pokemon);
                    MessageBox.Show("Datos modificados con éxito");
                }
                else
                {
                    datos.agregar(pokemon);
                    MessageBox.Show("Datos agregados con éxito");
                }

                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")))
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);


                Close();

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarRutaLocal(string rutaLocal)
        {
            return File.Exists(rutaLocal);
        }

        private bool validarUrl(string imagenUrl)
        {
            return Uri.TryCreate(imagenUrl, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private void FrmAltaPokemon_Load(object sender, EventArgs e)
        {
            ElementoNegocio elementoNegocio = new ElementoNegocio();
            try
            {
                cboTipo.DataSource = elementoNegocio.listar();
                //Clave y valor
                cboTipo.ValueMember = "Id";
                cboTipo.DisplayMember = "Descripcion";

                cboDebilidad.DataSource = elementoNegocio.listar();
                //Clave y valor
                cboDebilidad.ValueMember = "Id";
                cboDebilidad.DisplayMember = "Descripcion";


                if (pokemon != null) 
                {
                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;

                    cargarImagen(pokemon.UrlImagen);
                    //preseleccionar valores pkm que yo elija:
                    cboTipo.SelectedValue = pokemon.Tipo.Id;
                    cboDebilidad.SelectedValue = pokemon.Debilidad.Id;
                }

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {
            cargarImagen(txtUrlImagen.Text);
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

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            archivo = new OpenFileDialog();
            archivo.Filter = "jpg|*.jpg; |png|*.png";
            if (archivo.ShowDialog() == DialogResult.OK)
            { 
                txtUrlImagen.Text = archivo.FileName;
                cargarImagen(archivo.FileName);
                //File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);
            }
            //Confirmacion de actualizaciones GitHub
        }

        private bool validarRepetidos(int numero, string nombre, string imgUrl ) 
        {
            PokemonDatos datos = new PokemonDatos();
            List<Pokemon> listaPkms = datos.listar();
            string imgPlaceholder = "https://developers.elementor.com/docs/assets/img/elementor-placeholder-image.png";

            foreach (Pokemon pkm in listaPkms)
            {
                if (pokemon != null && !(string.IsNullOrEmpty(imgUrl)) && !(imgUrl.Trim().Equals(imgPlaceholder.Trim())) && pokemon.UrlImagen == imgUrl)
                {
                    MessageBox.Show("La imágen ya está registrada", "Alta Pokemon", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }

                if (pkm.Numero == numero) 
                {
                    MessageBox.Show("El número de este Pókemon ya está registrado", "Alta Pokemon", MessageBoxButtons.OK,MessageBoxIcon.Stop);
                    return false;
                }
                if (pkm.Nombre == nombre)
                {
                    MessageBox.Show("El nombre de este Pókemon ya está registrado", "Alta Pokemon", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }


            }
            return true;
        }



     }
 }
