using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Actividad2_ActividadDocentes.Helpers;
using Actividad2_ActividadDocentes.Models;
using Actividad2_ActividadDocentes.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Actividad2_ActividadDocentes.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult IniciarSesion()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IniciarSesion(Director d)
        {
            escuelaContext ec = new escuelaContext();
            Repository<Director> rd = new Repository<Director>(ec);
            var dir = ec.Director.FirstOrDefault(x => x.Clave == d.Clave);
            try
            {
                if (dir != null && dir.Contrasena == d.Contrasena)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, "Usuario" + dir.Nombre));
                    info.Add(new Claim("Clave", dir.Clave.ToString()));
                    info.Add(new Claim(ClaimTypes.Role, "Director"));
                    info.Add(new Claim("Nombre", dir.Nombre));

                    var ClaimsIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var ClaimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsPrincipal, new AuthenticationProperties
                    { IsPersistent = true });
                    return RedirectToAction("IngresoExitoso");
                }
                else
                {
                    ModelState.AddModelError("", "La clave o la contraseña no son correctos");
                    return View(d);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(d);
            }
        }
        [AllowAnonymous]
        public IActionResult IniciarSesion2()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> IniciarSesion2(Profesor p)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.GetProfesoresByClave(p.Clave);
            try
            {
                if (prof != null && prof.Contrasena == HashHelper.ObtenerHash(p.Contrasena))
                {
                    if (prof.Activo == 1)
                    {
                        List<Claim> info = new List<Claim>();
                        info.Add(new Claim(ClaimTypes.Name, "Usuario" + prof.Nombre));
                        info.Add(new Claim("Clave", prof.Clave.ToString()));
                        info.Add(new Claim(ClaimTypes.Role, "Profesor"));
                        info.Add(new Claim("Nombre", prof.Nombre));
                        info.Add(new Claim("Id", prof.Id.ToString()));

                        var ClaimsIdentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                        var ClaimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, ClaimsPrincipal);
                        return RedirectToAction("IngresoExitoso", prof.Clave);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Su cuenta no se encuentra activa, debe de comunicarlo al director para que la active");
                        return View(p);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "La clave o la contraseña no son correctos");
                    return View(p);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(p);
            }
        }
        [Authorize(Roles = "Director, Profesor")]
        public IActionResult IngresoExitoso(int clave)
        {
            return View();
        }
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "Director")]
        public IActionResult VerProfesores()
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var ListaP = pr.GetAll();
            return View(ListaP);
        }
        [Authorize(Roles = "Director")]
        public IActionResult AgregarProfesores()
        {
            return View();
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult AgregarProfesores(Profesor p)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            try
            {
                var verify = pr.GetProfesoresByClave(p.Clave);
                if (verify != null)
                {
                    ModelState.AddModelError("", "Ya existe un profesor con esta clave");
                    return View(p);
                }
                else 
                {
                    p.Activo = 1;
                    p.Contrasena = HashHelper.ObtenerHash(p.Contrasena);
                    pr.Insert(p);
                    return RedirectToAction("VerProfesores");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(p);
            }
        }
        [Authorize(Roles = "Director")]
        public IActionResult EditarProfesores(int id)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.Get(id);
            if (prof == null)
            {
                return RedirectToAction("VerProfesores");
            }
            return View(prof);
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult EditarProfesores(Profesor p)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.Get(p.Id);
            try
            {
                if (prof != null) 
                {
                    prof.Nombre = p.Nombre;
                    pr.Update(prof);
                }
                return RedirectToAction("VerProfesores");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(prof);
            }
        }
        [Authorize(Roles = "Director")]
        public IActionResult CambiarContraProf(int id)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.Get(id);
            if (prof == null)
            {
                return RedirectToAction("VerProfesores");
            }
            return View(prof);
        }
        [Authorize(Roles = "Director")]
        [HttpPost]
        public IActionResult CambiarContraProf(Profesor p, string contraseña1, string contraseña2)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.Get(p.Id);
            try
            {
                if (prof != null)
                {
                    if (contraseña1 == prof.Contrasena)
                    {
                        ModelState.AddModelError("", "La nueva contraseña no puedo ser igual a la actual.");
                        return View(prof);
                    }
                    else
                    {
                        if (contraseña1 == contraseña2)
                        {
                            prof.Contrasena = contraseña1;
                            prof.Contrasena = HashHelper.ObtenerHash(contraseña1);
                            pr.Update(prof);
                        }
                        else
                        {
                            ModelState.AddModelError("", "Las contraseñas que agrego no son iguales.");
                            return View(prof);
                        }
                    }
                }
                return RedirectToAction("VerProfesores");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(prof);
            }
        }
        [HttpPost]
        public IActionResult DesactivarProfesor(Profesor p)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.Get(p.Id);
            if (prof != null && prof.Activo == 1) 
            {
                prof.Activo = 0;
                pr.Update(prof);
            }
            else 
            {
                prof.Activo = 1;
                pr.Update(prof);
            }
            return RedirectToAction("VerProfesores");
        }
        [Authorize(Roles = "Director, Profesor")]
        public IActionResult VerAlumnos(int id)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            var prof = pr.GetAlumnosById(id);
            if (prof != null)
            {
                return View(prof);
            }
            else
                return RedirectToAction("IngresoExitoso");
        }
        [Authorize(Roles = "Director, Profesor")]
        public IActionResult AgregarAlumno(int id)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            AlumnoViewModel avm = new AlumnoViewModel();
            avm.Profesor = pr.Get(id);
            return View(avm);
        }
        [Authorize(Roles = "Director, Profesor")]
        [HttpPost]
        public IActionResult AgregarAlumno(AlumnoViewModel avm)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            AlumnoRepository ar = new AlumnoRepository(ec);
            try
            {
                var IdProf = pr.GetProfesoresByClave(avm.Profesor.Clave).Id;
                avm.Alumno.IdProfesor = IdProf;
                ar.Insert(avm.Alumno);
                return RedirectToAction("IngresoExitoso");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(avm);
            }
        }
        [Authorize(Roles = "Director, Profesor")]
        public IActionResult EditarAlumno(int id)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            AlumnoRepository ar = new AlumnoRepository(ec);
            AlumnoViewModel avm = new AlumnoViewModel();
            avm.Alumno = ar.Get(id);
            avm.Profesores = pr.GetAll();
            if (avm.Alumno != null)
            {
                if (User.IsInRole("Profesor"))
                {
                    avm.Profesor = pr.Get(avm.Alumno.IdProfesor);
                    if (User.Claims.FirstOrDefault(x => x.Type == "Clave").Value == avm.Profesor.Clave.ToString())
                    {
                        return View(avm);
                    }
                    else
                    {
                        return RedirectToAction("VerAlumnos");
                    }
                }
                else return View(avm);
            }
            else return RedirectToAction("VerAlumnos");
        }
        [Authorize(Roles = "Director, Profesor")]
        [HttpPost]
        public IActionResult EditarAlumno(AlumnoViewModel avm)
        {
            escuelaContext ec = new escuelaContext();
            ProfesorRepository pr = new ProfesorRepository(ec);
            AlumnoRepository ar = new AlumnoRepository(ec);
            try
            {
                var alum = ar.Get(avm.Alumno.Id);
                if (alum != null)
                {
                    alum.Nombre = avm.Alumno.Nombre;
                    if (User.IsInRole("Director"))
                    {
                        alum.IdProfesor = avm.Alumno.IdProfesor;
                    }
                    ar.Update(alum);
                    return RedirectToAction("IngresoExitoso");
                }
                else
                {
                    ModelState.AddModelError("","El alumno que esta buscando no se encuentra registrado");
                    avm.Profesor = pr.Get(avm.Alumno.IdProfesor);
                    avm.Profesores = pr.GetAll();
                    return View(avm);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                avm.Profesor = pr.Get(avm.Alumno.IdProfesor);
                avm.Profesores = pr.GetAll();
                return View(avm);
            }
        }
        [Authorize(Roles = "Director, Profesor")]
        [HttpPost]
        public IActionResult ElimiarAlumno(Alumno a)
        {
            escuelaContext ec = new escuelaContext();
            AlumnoRepository ar = new AlumnoRepository(ec);
            var alum = ar.Get(a.Id);
            if (alum != null) 
            {
                ar.Delete(alum);
            }
            else
            {
                ModelState.AddModelError("", "El alumno que esta tratando de eliminar no se encuentra");
            }
            return RedirectToAction("VerAlumnos");
        }
    }
}
