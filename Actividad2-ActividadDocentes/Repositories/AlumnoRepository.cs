using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2_ActividadDocentes.Models;
using Microsoft.EntityFrameworkCore;

namespace Actividad2_ActividadDocentes.Repositories
{
    public class AlumnoRepository:Repository<Alumno>
    {
        public AlumnoRepository(escuelaContext ec):base(ec)
        {

        }
        public Alumno GetAlumnosByNoControl(string noControl)
        {
            return context.Alumno.FirstOrDefault(x => x.NoControl.ToUpper() == noControl.ToUpper());
        }
        public override bool Validar(Alumno entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new Exception("Escriba el nombre del alumno");
            if (string.IsNullOrWhiteSpace(entidad.NoControl))
                throw new Exception("Escriba el numero de control del alumno");
            if (entidad.IdProfesor <= 0 || entidad.IdProfesor == null)
                throw new Exception("Asigne el maestro del alumno");
            return true;
        }
    }
}
