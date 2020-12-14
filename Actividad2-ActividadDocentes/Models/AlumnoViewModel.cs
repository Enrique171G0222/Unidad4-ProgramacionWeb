using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Actividad2_ActividadDocentes.Models
{
    public class AlumnoViewModel
    {
        public Alumno Alumno { get; set; }
        public Profesor Profesor { get; set; }
        public IEnumerable<Profesor> Profesores { get; set; }
    }
}
