using System;
using System.Collections.Generic;

namespace Actividad2_ActividadDocentes.Models
{
    public partial class Director
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public int Clave { get; set; }
    }
}
