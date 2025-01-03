using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace Negocio
{
    public class ElementoNegocio
    {
        public List<Elementos> listar() 
        {
            List<Elementos> lista = new List<Elementos>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("select Id, Descripcion from ELEMENTOS");
                datos.ejecutarConsulta();

                while (datos.Lector.Read())
                {
                    Elementos auxiliar = new Elementos();
                    auxiliar.Id = (int)datos.Lector["Id"];
                    auxiliar.Descripcion = (string)datos.Lector["Descripcion"];

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
                datos.cerrarConnexion();
            }   
        }
    }
}
