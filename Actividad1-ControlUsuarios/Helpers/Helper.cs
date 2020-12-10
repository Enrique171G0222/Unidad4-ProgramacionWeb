using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad1_ControlUsuarios.Helpers
{
    public static class Helper
    {
        public static int GetCode()
        {
            Random r = new Random();
            int code1 = r.Next(100, 1000);
            int code2 = r.Next(100, 1000);
            return (code1 + code2);
        }
    }
}
