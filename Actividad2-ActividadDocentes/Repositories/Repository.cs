using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad2_ActividadDocentes.Models;

namespace Actividad2_ActividadDocentes.Repositories
{
    public class Repository<T> where T:class
    {
        public escuelaContext context { get; set; }
        public Repository(escuelaContext ec)
        {
            context = ec;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return context.Set<T>();
        }
        public T Get(object id)
        {
            return context.Find<T>(id);
        }
        public virtual bool Validar(T entidad)
        {
            return true;
        }
        public virtual void Insert(T entidad)
        {
            if (Validar(entidad))
            {
                context.Add(entidad);
                context.SaveChanges();
            }
        }
        public virtual void Update(T entidad)
        {
            if (Validar(entidad))
            {
                context.Update<T>(entidad);
                context.SaveChanges();
            }
        }
        public virtual void Delete(T entidad)
        {
            context.Remove<T>(entidad);
            context.SaveChanges();
        }
    }
}
