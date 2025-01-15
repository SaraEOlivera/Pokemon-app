using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;
using System.Diagnostics.Eventing.Reader;

namespace Negocio
{
    public class PokemonDatos
    {
        public List<Pokemon> listar() 
        {
            List<Pokemon> lista = new List<Pokemon>();
            //crear objeto  que permite conexion:
            SqlConnection conexion = new SqlConnection();
            //crear objeto que permite realizar acciones:
            SqlCommand comando = new SqlCommand();
            //crear objeto que permite albergar la lectura del set de datos:
            SqlDataReader lector; 
            try
            {
                conexion.ConnectionString = "server =  .\\SQLEXPRESS; database = POKEDEX_DB; integrated security = true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo and D.Id=P.IdDebilidad AND P.Activo = 1";
                comando.Connection = conexion;

                conexion.Open();
                lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    Pokemon auxiliar = new Pokemon();

                    auxiliar.Id = (int)lector["Id"];
                    auxiliar.Numero = lector.GetInt32(0);
                    //auxiliar.Nombre = lector.GetString(1);
                    auxiliar.Nombre =(string) lector["Nombre"];
                    auxiliar.Descripcion = (string)lector["Descripcion"];

                    if (!(lector["UrlImagen"] is DBNull))
                        auxiliar.UrlImagen = (string)lector["UrlImagen"];

                    auxiliar.Tipo = new Elementos();
                    auxiliar.Tipo.Id = (int)lector["IdTipo"];
                    auxiliar.Tipo.Descripcion = (string)lector["Tipo"];
                    auxiliar.Debilidad = new Elementos();
                    auxiliar.Debilidad.Id = (int)lector["IdDebilidad"];
                    auxiliar.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(auxiliar);
                }
                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            {
                conexion.Close();
            }
        }

        public void agregar(Pokemon nuevo) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("Insert into POKEMONS(Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, urlImagen)Values(@Numero, @Nombre, @Descripcion, 1, @IdTipo, @IdDebilidad, @urlImagen)");
                datos.setearParametro("@Numero", nuevo.Numero);
                datos.setearParametro("@Nombre", nuevo.Nombre);
                datos.setearParametro("@Descripcion", nuevo.Descripcion);
                datos.setearParametro("@IdTipo", nuevo.Tipo.Id);
                datos.setearParametro("@IdDebilidad", nuevo.Debilidad.Id);
                datos.setearParametro("@urlImagen", nuevo.UrlImagen);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            {
                datos.cerrarConnexion();
            }
        }

        public void modificar(Pokemon poke) 
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @descripcion, UrlImagen = @urlImagen, IdTipo = @idTipo, IdDebilidad = @idDebilidad where Id = @id;");

                //agregar parametros:
                datos.setearParametro("@numero", poke.Numero);
                datos.setearParametro("@nombre", poke.Nombre);
                datos.setearParametro("@descripcion", poke.Descripcion);
                datos.setearParametro("@urlImagen", poke.UrlImagen);
                datos.setearParametro("@idTipo", poke.Tipo.Id);
                datos.setearParametro("@idDebilidad", poke.Debilidad.Id);
                datos.setearParametro("@id", poke.Id);

                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally 
            { 
                datos.cerrarConnexion(); 
            }
        }

        public void eliminar(int id) 
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("Delete From POKEMONS Where Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void eliminarLogico(int id) 
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("Update POKEMONS Set Activo = 0 where Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List <Pokemon> filtrar(string campo, string criterio, string filtro)
        {
            List<Pokemon> lista = new List<Pokemon>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "Select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion as Tipo, D.Descripcion as Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where E.Id = P.IdTipo and D.Id=P.IdDebilidad AND P.Activo = 1 AND ";

                if (campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero >" + filtro;
                            break;
                        case "Menor a":
                            consulta += "Numero <" + filtro;
                            break;
                        default:
                            consulta += "Numero =" + filtro;
                            break;

                    }
                }
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like ' " + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "Nombre like ' %" + filtro + " ' ";
                            break;
                        default:
                            consulta += "Nombre like '%" + filtro + "%' ";
                            break;
                    }
                }
                else 
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descripcion like ' " + filtro + "%' ";
                            break;
                        case "Termina con":
                            consulta += "P.Descripcion like ' %" + filtro + " ' ";
                            break;
                        default:
                            consulta += "P.Descripcion like '%" + filtro + "%' ";
                            break;
                    }
                }
                datos.setearConsulta(consulta);
                datos.ejecutarConsulta();

                while (datos.Lector.Read())
                {
                    Pokemon auxiliar = new Pokemon();

                    auxiliar.Id = (int)datos.Lector["Id"];
                    auxiliar.Numero = datos.Lector.GetInt32(0);
                    //auxiliar.Nombre = lector.GetString(1);
                    auxiliar.Nombre = (string)datos.Lector["Nombre"];
                    auxiliar.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["UrlImagen"] is DBNull))
                        auxiliar.UrlImagen = (string)datos.Lector["UrlImagen"];

                    auxiliar.Tipo = new Elementos();
                    auxiliar.Tipo.Id = (int)datos.Lector["IdTipo"];
                    auxiliar.Tipo.Descripcion = (string)datos.Lector["Tipo"];
                    auxiliar.Debilidad = new Elementos();
                    auxiliar.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    auxiliar.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

                    lista.Add(auxiliar);
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
