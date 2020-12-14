using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Actividad2_ActividadDocentes.Helpers
{
    public class HashHelper
    {
        public static string ObtenerHash(string cadena)
        {
            var alg = SHA256.Create();
            byte[] Cod = System.Text.Encoding.UTF8.GetBytes(cadena);
            byte[] Hash = alg.ComputeHash(Cod);
            string c = "";
            foreach (var item in Cod)
            {
                c += item.ToString("x2");
            }
            return c;
        }
    }
}
