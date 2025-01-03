﻿using System;
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

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            //esta variable nuevoPokemon ya no es necesaria. Se reemplaza por el atributo
            //Pokemon nuevoPokemon = new Pokemon();
            PokemonDatos datos = new PokemonDatos();
            try
            {
                if (pokemon == null)
                    pokemon = new Pokemon();

                //carga del objeto
                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;
                pokemon.UrlImagen=txtUrlImagen.Text;
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
     }
 }
