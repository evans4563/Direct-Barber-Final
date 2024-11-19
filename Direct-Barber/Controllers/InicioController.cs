 using Microsoft.AspNetCore.Mvc;

//Trabajar con los servicos creados.
using Direct_Barber.Models;
using Direct_Barber.Recursos;
using Direct_Barber.Servicios.Contrato;

//Trabajar con la autenticación por cookies.
using System.Security.Claims; 
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

//Imagen
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Direct_Barber.Servicios.Implementacion;

using Microsoft.AspNetCore.Mvc.Rendering;

namespace Direct_Barber.Controllers
{
    public class InicioController : Controller
    {
        //Utilizar el servicio.
        private readonly IUsuarioService _usuarioServicio;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InicioController(IUsuarioService usuarioServicio, IWebHostEnvironment webHostEnvironment)
        {
            _usuarioServicio = usuarioServicio;
            _webHostEnvironment = webHostEnvironment;
        }


        [HttpGet]
        public async Task<IActionResult> Registrarse()
        {
            // Obtener lista de roles de forma asíncrona
            var roles = await _usuarioServicio.GetRoles();

            // Pasar los roles a la vista, utilizando los nombres correctos de las propiedades
            ViewBag.Roles = new SelectList(roles, "Id", "Nombre");

            ViewData["Mensaje"] = null; // Inicializar con null
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(Usuario modelo, IFormFile foto)
        {
            // Verificar si el archivo está llegando
            if (foto == null || foto.Length == 0)
            {
                // Mostrar un mensaje si no se seleccionó una imagen
                ViewData["Mensaje"] = "No se ha seleccionado una imagen o la imagen está vacía.";
                return View(modelo);
            }

            // Invocar las imágenes
            string uFileName = Utilidades.UploadedFile(foto, _webHostEnvironment);      
            modelo.Foto = uFileName;

            // Encriptar la contraseña
            modelo.Contrasena = Utilidades.EncriptarContra(modelo.Contrasena);

            // Guardar el usuario
            Usuario usuario_creado = await _usuarioServicio.SaveUsuario(modelo);

            if (usuario_creado.Id > 0)
                return RedirectToAction("IniciarSesion", "Inicio");

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }




        public IActionResult IniciarSesion()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string Correo, string Contrasena)
        {
            // Al usuario encontrado se le encripta la contraseña.
            Usuario usuario_encontrado = await _usuarioServicio.GetUsuario(Correo, Utilidades.EncriptarContra(Contrasena));

            // Validación. Si no se encuentra el usuario, mostrar un mensaje.
            if (usuario_encontrado == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            // Invocar la imagen
            HttpContext.Session.SetString("Foto", usuario_encontrado.Foto ?? "usuario.png");

            // Crear una lista de claims con información relevante del usuario
            List<Claim> claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario_encontrado.Nombre),
        new Claim(ClaimTypes.Role, usuario_encontrado.Rol.Nombre), // Suponiendo que Rol tiene un campo Nombre
        new Claim(ClaimTypes.Email, usuario_encontrado.Correo),
        new Claim(ClaimTypes.NameIdentifier, usuario_encontrado.Id.ToString()) // Agregar el Id del cliente como reclamo
    };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
            );

            // Verificar el rol del usuario para redirigir
            if (usuario_encontrado.Rol.Id == 1)
            {
                return RedirectToAction("Index", "Home"); // Redirigir a una vista de cliente
            }
            else if (usuario_encontrado.Rol.Id == 2)
            {
                return RedirectToAction("Index", "Home"); // Redirigir a una vista de barbero
            }

            // Redirigir por defecto si no se cumple ninguna condición de rol
            return RedirectToAction("Index", "Home");
        }
    }
}
