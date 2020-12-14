using System;
using System.Collections.Generic;

namespace Actividad2_ActividadDocentes.Models
{
    public partial class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string NoControl { get; set; }
        public int? IdProfesor { get; set; }

        public virtual Profesor IdProfesorNavigation { get; set; }
    }
}
