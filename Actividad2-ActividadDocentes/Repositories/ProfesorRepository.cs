using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2_ActividadDocentes.Models;
using Microsoft.EntityFrameworkCore;

namespace Actividad2_ActividadDocentes.Repositories
{
    public class ProfesorRepository:Repository<Profesor>
    {
        public ProfesorRepository(escuelaContext ec):base(ec)
        {

        }
        public Profesor GetProfesoresByClave(int clave)
        {
            return context.Profesor.FirstOrDefault(x => x.Clave == clave);
        }
        public Profesor GetAlumnosById(int id)
        {
            return context.Profesor.Include(x => x.Alumno).FirstOrDefault(x => x.Id == id);
        }
        public override bool Validar(Profesor entidad)
        {
            if (string.IsNullOrWhiteSpace(entidad.Nombre))
                throw new Exception("Escriba el nombre del profesor");
            if (string.IsNullOrWhiteSpace(entidad.Contrasena))
                throw new Exception("Escriba la contraseña del profesor");
            if (entidad.Contrasena.Length < 8)
                throw new Exception("La contraseña debe ser de al menos 8 caracteres");
            return true;
        }
    }
}
