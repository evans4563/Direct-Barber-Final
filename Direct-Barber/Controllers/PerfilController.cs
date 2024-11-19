using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Direct_Barber.Servicios.Contrato;
using Direct_Barber.Recursos;

namespace Direct_Barber.Controllers
{
    public class PerfilController : Controller
    {
        private readonly DirectBarber1Context _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUsuarioService _usuarioService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PerfilController(DirectBarber1Context context, IWebHostEnvironment webHostEnvironment, IUsuarioService usuarioService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _usuarioService = usuarioService;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Perfil
        [Authorize]
        public async Task<IActionResult> Index(Usuario modelo, IFormFile foto)
        {
            // Obtener el correo del usuario autenticado desde los claims
            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);

            // Obtener el usuario autenticado solo por su correo
            var usuario = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);


            if (usuario == null)
            {
                return NotFound(); // Manejar el caso donde no se encuentra el usuario
            }

            // Retornar la vista con el usuario encontrado
            return View(usuario);
        }



        // GET: Perfil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            // Condicional para adaptar la vista según el rol del usuario
            if (usuario.Rol.Nombre == "Barbero")
            {
                // Lógica específica para ver detalles del barbero (incluyendo reseñas, puntuación, etc.)
                ViewBag.EsBarbero = true;
            }
            else
            {
                ViewBag.EsBarbero = false;
            }

            return View(usuario);
        }
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obtener el usuario por su Id
            var usuario = await _usuarioService.GetUsuarioById(id);

            if (usuario == null)
            {
                return NotFound(); // Si no se encuentra el usuario, se retorna un 404
            }

            return View(usuario); // Enviar el modelo a la vista
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Correo,Direccion,Telefono,Apellido,FecNacimiento")] Usuario usuario, IFormFile foto)
        {
            // Verificar que el ID proporcionado coincida con el del usuario
            if (id != usuario.Id)
            {
                return NotFound();
            }

            // Obtener el usuario original desde la base de datos
            var usuarioExistente = await _usuarioService.GetUsuarioById(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            // Desacoplar la entidad existente para evitar conflictos de rastreo
            _context.Entry(usuarioExistente).State = EntityState.Detached;

            // Mantener los datos que no se están modificando
            usuario.Contrasena = usuarioExistente.Contrasena;
            usuario.Foto = usuarioExistente.Foto;
            usuario.Id_Rol = usuarioExistente.Id_Rol;
            usuario.Documento = usuarioExistente.Documento;
            usuario.FecRegistro = usuarioExistente.FecRegistro;

            // Verificar si se ha subido una nueva imagen
            if (foto != null)
            {
                string uFileName = Utilidades.UploadedFile(foto, _webHostEnvironment); // Usar método centralizado
                if (!string.IsNullOrEmpty(uFileName))
                {
                    usuario.Foto = uFileName; // Asignar el nuevo nombre de archivo
                }
            }
            else
            {
                usuario.Foto = usuarioExistente.Foto; // Mantener la foto original si no hay nueva
            }

            HttpContext.Session.SetString("Foto", usuario.Foto);


            // Remover las validaciones para los campos no editables
            ModelState.Remove("Contrasena");
            ModelState.Remove("Foto");
            ModelState.Remove("Id_Rol");
            ModelState.Remove("Documento");
            ModelState.Remove("Rol");
            ModelState.Remove("ImagenFile");
            ModelState.Remove("ResenasComoCliente");
            ModelState.Remove("ResenasComoBarbero");

            if (ModelState.IsValid)
            {
                try
                {
                    // Adjuntar el usuario actualizado y marcarlo como modificado
                    _context.Entry(usuario).State = EntityState.Modified;

                    // Guardar los cambios en la base de datos
                    await _usuarioService.UpdateUsuario(usuario);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _usuarioService.UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // GET: Perfil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Perfil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario != null)
            {
                // Eliminar usuario de la base de datos
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                // Limpiar la sesión del usuario
                HttpContext.Session.Clear(); // Eliminar toda la información de la sesión
            }

            // Redirigir al método IniciarSesion del controlador Inicio
            return RedirectToAction("IniciarSesion", "Inicio");
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }

        // -------------- ---- USUARIO OPUESTOS ---- --------------

        // Método para listar usuarios opuestos al rol actual
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ListarUsuariosOpuestos()
        {
            // Obtener el correo del usuario autenticado
            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);

            // Obtener el usuario autenticado
            var usuarioActual = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            if (usuarioActual == null)
            {
                return NotFound(); // Si no se encuentra el usuario autenticado
            }

            // Determinar el rol del usuario actual
            List<Usuario> usuariosOpuestos;

            if (usuarioActual.Rol.Nombre == "Cliente")
            {
                // Si el usuario es un cliente, obtener todos los barberos
                usuariosOpuestos = await _context.Usuarios
                    .Where(u => u.Rol.Nombre == "Barbero")
                    .ToListAsync();
            }
            else if (usuarioActual.Rol.Nombre == "Barbero")
            {
                // Si el usuario es un barbero, obtener todos los clientes
                usuariosOpuestos = await _context.Usuarios
                    .Where(u => u.Rol.Nombre == "Cliente")
                    .ToListAsync();
            }
            else
            {
                // En caso de que el rol no sea ni barbero ni cliente (puede manejarse según tu sistema)
                usuariosOpuestos = new List<Usuario>();
            }

            // Retornar la vista con la lista de usuarios opuestos
            return View(usuariosOpuestos);
        }

        // Método para ver el perfil de un usuario opuesto
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> VerPerfil(int id)
        {
            // Obtener el correo del usuario autenticado
            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);

            // Obtener el usuario autenticado
            var clienteActual = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            if (clienteActual == null)
            {
                return NotFound(); // Si el cliente no se encuentra
            }

            // Busca al barbero por ID y carga solo las reseñas donde él es el barbero
            var barbero = await _context.Usuarios
                .Include(u => u.ResenasComoBarbero) // Incluye las reseñas
                .ThenInclude(r => r.Cliente)        // También incluye el cliente que hizo cada reseña
                .FirstOrDefaultAsync(u => u.Id == id && u.Id_Rol == 2); // Asegúrate de que sea un barbero

            if (barbero == null)
            {
                return NotFound();
            }

            // Calcula el promedio de calificación del barbero en base a las reseñas que recibió como barbero
            var promedioCalificacion = barbero.ResenasComoBarbero.Any() ?
                Math.Round((decimal)barbero.ResenasComoBarbero.Average(r => r.Calificacion), 1) : 0m;

            // Prepara el modelo de la vista
            var modelo = new BarberoViewModel
            {
                NombreBarbero = barbero.Nombre,
                ApellidoBarbero = barbero.Apellido,
                Direccion = barbero.Direccion,
                Telefono = barbero.Telefono,
                Foto = barbero.Foto,
                Descripcion = barbero.Descripcion,
                PromedioCalificacion = promedioCalificacion,
                TotalResenas = barbero.ResenasComoBarbero.Count(),
                Resenas = barbero.ResenasComoBarbero.ToList(),  // Lista de reseñas con los clientes incluidos
                BarberoId = barbero.Id,
                ClienteActualId = clienteActual.Id // Pasar el ID del cliente autenticado
            };

            return View(modelo);
        }




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AgregarResena(int BarberoId, string contenido, int calificacion)
        {
            // Obtener el cliente que está dejando la reseña
            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);
            var cliente = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            if (cliente == null)
            {
                return NotFound(); // Si el cliente no se encuentra
            }

            // Crear la nueva reseña
            var nuevaResena = new Resena
            {
                Id_Cliente = cliente.Id,
                Id_Barbero = BarberoId,
                Contenido = contenido,
                Calificacion = calificacion,
                FechaPublicacion = DateTime.Now
            };

            // Agregar la reseña a la base de datos
            _context.Resenas.Add(nuevaResena);
            await _context.SaveChangesAsync();

            // Redirigir al perfil del barbero después de publicar la reseña
            return RedirectToAction("VerPerfil", new { id = BarberoId });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditarResena(int id)
        {
            // Buscar la reseña por su ID
            var resena = await _context.Resenas.Include(r => r.Cliente).FirstOrDefaultAsync(r => r.Id == id);

            if (resena == null)
            {
                return NotFound();
            }

            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);
            var cliente = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            // Verificar si el usuario actual es el propietario de la reseña
            if (resena.Id_Cliente != cliente.Id)
            {
                return Forbid();
            }

            return View(resena); // Retornar la vista de edición con el modelo Resena
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditarResena(int id, ResenaViewModel model)
        {
            var resena = await _context.Resenas.FindAsync(id);

            if (resena == null)
            {
                return NotFound();
            }

            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);
            var cliente = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            // Verificar si el usuario actual es el propietario de la reseña
            if (resena.Id_Cliente != cliente.Id)
            {
                return Forbid();
            }

            // Actualizar la reseña
            resena.Contenido = model.Contenido;
            resena.Calificacion = model.Calificacion;
            resena.FechaPublicacion = DateTime.Now;

            _context.Resenas.Update(resena);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerPerfil", new { id = resena.Id_Barbero });
        }



        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EliminarResena(int id)
        {
            var resena = await _context.Resenas.FindAsync(id);

            if (resena == null)
            {
                return NotFound();
            }

            var correoUsuario = User.FindFirstValue(ClaimTypes.Email);
            var cliente = await _usuarioService.GetUsuarioPorCorreo(correoUsuario);

            // Verificar si el usuario actual es el propietario de la reseña
            if (resena.Id_Cliente != cliente.Id)
            {
                return Forbid(); // No permitir si no es el dueño de la reseña
            }

            _context.Resenas.Remove(resena);
            await _context.SaveChangesAsync();

            return RedirectToAction("VerPerfil", new { id = resena.Id_Barbero });
        }

        // Acción para "Términos y Condiciones"
        [HttpGet]
        [Authorize]
        public IActionResult TerminosYCondiciones()
        {
            // Devuelve la vista de términos y condiciones
            return View();
        }

        // Acción para "Soporte"
        [HttpGet]
        [Authorize]
        public IActionResult Soporte()
        {
            // Devuelve la vista de soporte
            return View();
        }

        // Acción para "Ayuda"
        [HttpGet]
        [Authorize]
        public IActionResult Ayuda()
        {
            // Devuelve la vista de ayuda
            return View();
        }


    }
}