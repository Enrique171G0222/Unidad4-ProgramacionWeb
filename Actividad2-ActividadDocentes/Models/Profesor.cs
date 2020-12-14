using System;
using System.Collections.Generic;

namespace Actividad2_ActividadDocentes.Models
{
    public partial class Profesor
    {
        public Profesor()
        {
            Alumno = new HashSet<Alumno>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Contrasena { get; set; }
        public ulong? Activo { get; set; }
        public int Clave { get; set; }

        public virtual ICollection<Alumno> Alumno { get; set; }
    }
}
