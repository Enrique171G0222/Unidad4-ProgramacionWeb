using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad1_ControlUsuarios.Models;

namespace Actividad1_ControlUsuarios.Repositories
{
    public class Repository<T> where T : class
    {
        public cuentascontrolContext Context { get; set; }
        public Repository(cuentascontrolContext ccc)
        {
            Context = ccc;
        }
        public Usuario GetUsuarioByCorreo(string correo)
        {
            return Context.Usuario.FirstOrDefault(x => x.Correo.ToUpper() == correo.ToUpper());
        }
        public virtual void Insert(Usuario us)
        {
            if (validar(us))
            {
                Context.Add(us);
                Context.SaveChanges();
            }
        }
        public virtual void Update(Usuario us)
        {
            if (validar(us))
            {
                Context.Update(us);
                Context.SaveChanges();
            }
        }
        public virtual void Delete(T us)
        {
            Context.Remove(us);
            Context.SaveChanges();
        }
        public bool validar(Usuario us)
        {
            if (string.IsNullOrWhiteSpace(us.Nombre))
                throw new Exception("Escriba su nombre de usuario");
            if (string.IsNullOrWhiteSpace(us.Correo))
                throw new Exception("Escriba el correo electronico de su usuario");
            if (string.IsNullOrWhiteSpace(us.Contrasena))
                throw new Exception("Escriba la contraseña de su usuario");
            if (Context.Usuario.Any(x => x.Correo.ToUpper() == us.Correo.ToUpper() && x.Id != us.Id))
                throw new Exception("Ya existe un usuario registrado con ese correo electronico");
            return true;
        }

    }
}
