using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace Actividad1_ControlUsuarios.Helpers
{
    public class HashHelper
    {
        public static string GetHash(string cadena)
        {
            var alg = SHA256.Create();
            byte[] codificar = Encoding.UTF8.GetBytes(cadena);
            byte[] hash = alg.ComputeHash(codificar);
            string c = "";
            foreach (var item in hash)
            {
                c += item.ToString("x2");
            }
            return c;
        }
    }
}
