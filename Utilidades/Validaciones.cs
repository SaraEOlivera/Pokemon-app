using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilidades
{
    public static class Validaciones
    {
        public static bool soloNumeros(string cadena)
        {
            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                    return false;
            }
            return true;
        }

        public static bool soloLetras(string cadena)
        {
            if (string.IsNullOrWhiteSpace(cadena)) 
            {
                return false;
            }
            foreach (char caracter in cadena)
            {
                if (!(char.IsLetter(caracter) || char.IsWhiteSpace(caracter)))
                    return false;
            }
            return true;
        }
    }
}
