using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;
using System.Security.Claims;

namespace Direct_Barber.Controllers
{
    public class SolicitudsBarberoController : Controller
    {
        private readonly DirectBarber1Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public SolicitudsBarberoController(DirectBarber1Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: SolicitudsBarbero
        public async Task<IActionResult> Index()
        {
            // Obtener el ID del usuario autenticado
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out var userId))
            {
                // Manejar el caso donde no se pueda obtener el ID del usuario
                return Unauthorized(); // Redirige a una página de error o login
            }

            // Filtrar solicitudes donde IdBarbero esté vacío
            var solicitudesSinBarbero = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.TipoServicioNavigation) // Incluir la relación con TipoSer
                .Where(s => s.IdBarbero == null)
                .ToListAsync();

            // Filtrar las solicitudes donde el usuario autenticado sea el barbero
            var solicitudesDelBarbero = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.TipoServicioNavigation) // Incluir la relación con TipoSer
                .Where(s => s.IdBarbero == userId)
                .ToListAsync();

            // Crear un ViewModel para enviar ambas listas a la vista
            var viewModel = new SolicitudIndexViewModel
            {
                SolicitudesSinBarbero = solicitudesSinBarbero,
                SolicitudesDelBarbero = solicitudesDelBarbero
            };

            return View(viewModel);
        }


        public async Task<IActionResult> Agenda()
        {
            // Obtener el ID del usuario autenticado
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out var userId))
            {
                // Manejar el caso donde no se pueda obtener el ID del usuario
                return Unauthorized(); // Redirige a una página de error o login
            }

            // Filtrar solicitudes donde IdBarbero esté vacío
            var solicitudesSinBarbero = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.TipoServicioNavigation) // Incluir la relación con TipoSer
                .Where(s => s.IdBarbero == null)
                .ToListAsync();

            // Filtrar las solicitudes donde el usuario autenticado sea el barbero
            var solicitudesDelBarbero = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.TipoServicioNavigation) // Incluir la relación con TipoSer
                .Where(s => s.IdEstado == 1 &&  s.IdBarbero == userId)
                .ToListAsync();
            
            // Filtrar las solicitudes completadas - HISTORIALS
            var solicitudesCompletadas = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.TipoServicioNavigation) // Incluir la relación con TipoSer
                .Where(s => s.IdEstado == 2 && s.IdBarbero == userId) // Asegurarse de que solo se obtengan las solicitudes completadas del barbero autenticado
                .ToListAsync();


            // Crear un ViewModel para enviar ambas listas a la vista
            var viewModel = new SolicitudIndexViewModel
            {
                SolicitudesSinBarbero = solicitudesSinBarbero,
                SolicitudesDelBarbero = solicitudesDelBarbero,
                SolicitudesCompletadas = solicitudesCompletadas // Asegúrate de que tu ViewModel tenga esta propiedad
            };


            return View(viewModel);
        }


        // GET: SolicitudsBarbero/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdEstadoNavigation)
                .Include(s => s.TipoServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitud == null)
            {
                return NotFound();
            }

            return View(solicitud);
        }

        // GET: SolicitudsBarbero/Create
        public IActionResult Create()
        {
            ViewData["IdBarbero"] = new SelectList(_context.Usuarios, "Id", "Contrasena");
            ViewData["IdCliente"] = new SelectList(_context.Usuarios, "Id", "Contrasena");
            ViewData["IdEstado"] = new SelectList(_context.EstadoSolicituds, "Id", "Id");
            ViewData["TipoServicio"] = new SelectList(_context.TipoSers, "Id", "Nombre");
            return View();
        }

        // POST: SolicitudsBarbero/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdSolicitud,IdEstado,IdCliente,IdBarbero,PrecioServicio,TipoServicio,Dirección,Fecha,Descripcion")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                _context.Add(solicitud);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdBarbero"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdBarbero);
            ViewData["IdCliente"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdCliente);
            ViewData["IdEstado"] = new SelectList(_context.EstadoSolicituds, "Id", "Id", solicitud.IdEstado);
            ViewData["TipoServicio"] = new SelectList(_context.TipoSers, "Id", "Nombre", solicitud.TipoServicio);
            return View(solicitud);
        }

        // GET: SolicitudsBarbero/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _context.Solicituds.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }
            ViewData["IdBarbero"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdBarbero);
            ViewData["IdCliente"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdCliente);
            ViewData["IdEstado"] = new SelectList(_context.EstadoSolicituds, "Id", "Id", solicitud.IdEstado);
            ViewData["TipoServicio"] = new SelectList(_context.TipoSers, "Id", "Nombre", solicitud.TipoServicio);
            return View(solicitud);
        }

        // POST: SolicitudsBarbero/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdSolicitud,IdEstado,IdCliente,IdBarbero,PrecioServicio,TipoServicio,Dirección,Fecha,Descripcion")] Solicitud solicitud)
        {
            if (id != solicitud.IdSolicitud)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(solicitud);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SolicitudExists(solicitud.IdSolicitud))
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
            ViewData["IdBarbero"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdBarbero);
            ViewData["IdCliente"] = new SelectList(_context.Usuarios, "Id", "Contrasena", solicitud.IdCliente);
            ViewData["IdEstado"] = new SelectList(_context.EstadoSolicituds, "Id", "Id", solicitud.IdEstado);
            ViewData["TipoServicio"] = new SelectList(_context.TipoSers, "Id", "Nombre", solicitud.TipoServicio);
            return View(solicitud);
        }

        // GET: SolicitudsBarbero/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var solicitud = await _context.Solicituds
                .Include(s => s.IdBarberoNavigation)
                .Include(s => s.IdClienteNavigation)
                .Include(s => s.IdEstadoNavigation)
                .Include(s => s.TipoServicioNavigation)
                .FirstOrDefaultAsync(m => m.IdSolicitud == id);
            if (solicitud == null)
            {
                return NotFound();
            }

            return View(solicitud);
        }


        // POST: Solicituds/Accept
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aceptar(int id)
        {
            // Obtener el ID del usuario autenticado
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(); // Redirige a una página de error o login
            }

            // Buscar la solicitud con el ID dado
            var solicitud = await _context.Solicituds.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Actualizar el campo IdBarbero con el ID del usuario autenticado
            solicitud.IdBarbero = userId;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Agenda), new { id = solicitud.IdSolicitud });
        }



        // POST: Solicituds/CancelarServicio
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarServicio(int id)
        {
            // Obtener el ID del usuario autenticado
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(); // Redirige a una página de error o login
            }

            // Buscar la solicitud con el ID dado
            var solicitud = await _context.Solicituds.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Reiniciar el campo IdBarbero a null
            solicitud.IdBarbero = null;
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Agenda));
        }


        private bool SolicitudExists(int id)
        {
            return _context.Solicituds.Any(e => e.IdSolicitud == id);
        }


        // POST: SolicitudsBarbero/Completar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Completar(int id)
        {
            // Obtener el ID del usuario autenticado
            var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized(); // Redirige a una página de error o login
            }

            // Buscar la solicitud con el ID dado
            var solicitud = await _context.Solicituds.FindAsync(id);
            if (solicitud == null)
            {
                return NotFound();
            }

            // Actualizar el campo IdEstado a 2
            solicitud.IdEstado = 2; // Suponiendo que 2 es el estado de "Completada"
            _context.Update(solicitud);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Agenda)); // Redirigir a la acción que muestra las solicitudes
        }

    }
}
