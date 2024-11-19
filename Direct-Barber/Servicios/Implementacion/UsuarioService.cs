using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;
using Direct_Barber.Servicios.Contrato;
using Direct_Barber.Recursos;

namespace Direct_Barber.Servicios.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly DirectBarber1Context _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioService(DirectBarber1Context context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Usuario> GetUsuario(string correo, string contrasena)
        {
            return await _context.Usuarios
           .Include(u => u.Rol)
           .Where(u => u.Correo == correo && u.Contrasena == contrasena)
           .FirstOrDefaultAsync();
        }
        public async Task<Usuario> GetUsuarioPorCorreo(string correo)
        {
            return await _context.Usuarios
                .Include(u => u.Rol)
                .Where(u => u.Correo == correo) // Solo comparar el correo
                .FirstOrDefaultAsync();
        }

        public async Task<Usuario> SaveUsuario(Usuario modelo)
        {
            _context.Usuarios.Add(modelo);
            await _context.SaveChangesAsync();
            return modelo;
        }

        public async Task<List<Rol>> GetRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        // Método para obtener un usuario por su ID
        public async Task<Usuario> GetUsuarioById(int id)
        {
            return await _context.Usuarios
                .Include(u => u.Rol) // Cargar el rol si es necesario
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Método para actualizar el usuario
        public async Task<Usuario> UpdateUsuario(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // Método para verificar si el usuario existe por su ID
        public async Task<bool> UsuarioExists(int id)
        {
            return await _context.Usuarios.AnyAsync(u => u.Id == id);
        }

        public async Task<int> ObtenerUsuarioId()
        {
            var correo = _httpContextAccessor.HttpContext.User.Identity.Name; // Obtener el correo del usuario autenticado.
            var usuario = await GetUsuarioPorCorreo(correo); // Utiliza el método existente para obtener el usuario por correo.
            return usuario != null ? usuario.Id : 0; // Retorna el ID del usuario o 0 si no se encuentra.
        }

    }
}
