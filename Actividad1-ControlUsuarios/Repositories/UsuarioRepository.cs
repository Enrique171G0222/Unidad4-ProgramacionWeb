using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Actividad1_ControlUsuarios.Models;

namespace Actividad1_ControlUsuarios.Repositories
{
    public class UsuarioRepository: Repository<Usuario>
    {
        public UsuarioRepository(cuentascontrolContext ccc) : base(ccc) { }
    }
}
