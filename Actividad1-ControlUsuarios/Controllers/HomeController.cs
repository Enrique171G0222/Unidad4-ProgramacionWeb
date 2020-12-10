using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Actividad1_ControlUsuarios.Models;
using Actividad1_ControlUsuarios.Repositories;
using Actividad1_ControlUsuarios.Helpers;
using System.IO;
using System.Net.Mail;

namespace Actividad1_ControlUsuarios.Controllers
{
    public class HomeController : Controller
    {
        public IWebHostEnvironment Environment { get; set; }
        public HomeController(IWebHostEnvironment env)
        {
            Environment = env;
        }

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
        [AllowAnonymous]
        public async Task<IActionResult> IniciarSesion(Usuario user, bool recordar)
        {
            cuentascontrolContext ccc = new cuentascontrolContext();
            Repository<Usuario> repos = new Repository<Usuario>(ccc);
            var usuario = repos.GetUsuarioByCorreo(user.Correo);
            if (usuario != null && HashHelper.GetHash(user.Contrasena) == usuario.Contrasena)
            {
                if (usuario.Activo == 1)
                {
                    List<Claim> info = new List<Claim>();
                    info.Add(new Claim(ClaimTypes.Name, $"{usuario.Nombre}"));
                    info.Add(new Claim(ClaimTypes.Role, "UsuarioActivo"));
                    info.Add(new Claim("Nombre", usuario.Nombre));
                    info.Add(new Claim("Correo electronico", usuario.Correo));
                    var claimidentity = new ClaimsIdentity(info, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimprincipal = new ClaimsPrincipal(claimidentity);
                    if (recordar == true)
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties
                        { IsPersistent = true });
                    }
                    else
                    {
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimprincipal, new AuthenticationProperties
                        { IsPersistent = false });
                    }
                    return RedirectToAction("IngresoExitoso");
                }
                else
                {
                    ModelState.AddModelError("", "El correo electronico y/o la contraseña estan incorrectos");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "El usuario no se encuentra registrado");
                return View();
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult IngresoExitoso()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult RegistrarCuenta()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult RegistrarCuenta(Usuario user, string contraseña1, string contraseña2)
        {
            cuentascontrolContext ccc = new cuentascontrolContext();
            Repository<Usuario> repos = new Repository<Usuario>(ccc);
            try
            {
                if (ccc.Usuario.Any(x => x.Correo == user.Correo))
                {
                    ModelState.AddModelError("", "Ya hay un usuario con este correo");
                    return View(user);
                }
                else
                {
                    if (contraseña1 == contraseña2)
                    {
                        user.Contrasena = HashHelper.GetHash(contraseña1);
                        user.Codigo = Helper.GetCode();
                        user.Activo = 0;
                        repos.Insert(user);

                        MailMessage message = new MailMessage();
                        message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Disney+");
                        message.To.Add(user.Correo);
                        message.Subject = "Se ha enviado un correo de confirmacion";

                        string mensaje = System.IO.File.ReadAllText(Environment.WebRootPath + "/mensaje.html");
                        message.Body = mensaje.Replace("##Codigo##", user.Codigo.ToString());
                        message.IsBodyHtml = true;

                        SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                        client.EnableSsl = true;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                        client.Send(message);
                        return RedirectToAction("ActivarUsuario");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Las contraseñas no son iguales");
                        return View(user);
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(user);
            }
        }
        public IActionResult ActivarUsuario()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ActivarUsuario(int codigo)
        {
            cuentascontrolContext ccc = new cuentascontrolContext();
            UsuarioRepository repos = new UsuarioRepository(ccc);
            var usuario = ccc.Usuario.FirstOrDefault(x => x.Codigo == codigo);
            if (usuario != null && usuario.Activo == 0)
            {
                var c = usuario.Codigo;
                if (codigo == c)
                {
                    usuario.Activo = 1;
                    repos.Update(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "El código que introdujo esta equivocado.");
                    return View((object)codigo);
                }
            }
            else
            {
                ModelState.AddModelError("", "No se encontró el usuario.");
                return View((object)codigo);
            }
        }
        public IActionResult RecuperarContraseña()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RecuperarContraseña(string correo)
        {
            try
            {
                cuentascontrolContext ccc = new cuentascontrolContext();
                UsuarioRepository repos = new UsuarioRepository(ccc);
                var usuario = repos.GetUsuarioByCorreo(correo);
                if (usuario != null)
                {
                    var contra = Helper.GetCode();
                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("sistemascomputacionales7g@gmail.com", "Disney+");
                    message.To.Add(correo);
                    message.Subject = "Se ha enviado un correo que contiene la contraseña temporal";
                    message.Body = $"Contraseña temporal, esta contraseña dejara de ser util al cerrar sesion: {contra}";

                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("sistemascomputacionales7g@gmail.com", "sistemas7g");
                    client.Send(message);
                    usuario.Contrasena = HashHelper.GetHash(contra.ToString());
                    repos.Update(usuario);
                    return RedirectToAction("IniciarSesion");
                }
                else
                {
                    ModelState.AddModelError("", "No se encuentra ese correo registrado");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View((object)correo);
            }
        }
        [Authorize]
        public IActionResult CambiarContraseña()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult CambiarContraseña(string correo, string contraseña1, string contraseña2)
        {
            cuentascontrolContext ccc = new cuentascontrolContext();
            UsuarioRepository repos = new UsuarioRepository(ccc);
            var usuario = repos.GetUsuarioByCorreo(correo);
            try
            {
                if (contraseña1 == contraseña2)
                {
                    usuario.Contrasena = HashHelper.GetHash(contraseña1);
                    if (usuario.Contrasena == contraseña1)
                    {
                            ModelState.AddModelError("", "La contraseña nueva no puede ser igual a la que ya posees");
                            return View(contraseña1);
                    }
                    else
                    {
                        repos.Update(usuario);
                        return RedirectToAction("IngresoExitoso");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Escribiste diferentes contraseñas.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(contraseña1, contraseña2);
            }
        }

        [HttpPost]
        public IActionResult Eliminar(string correo)
        {
            cuentascontrolContext ccc = new cuentascontrolContext();
            UsuarioRepository repos = new UsuarioRepository(ccc);
            var usuario = repos.GetUsuarioByCorreo(correo);

            if (usuario != null)
            {
                HttpContext.SignOutAsync();
                repos.Delete(usuario);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "La eliminacion no ha sido posible.");
                return RedirectToAction("Acceso");
            }
        }
    }
}
