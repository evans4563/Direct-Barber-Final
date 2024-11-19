using Microsoft.EntityFrameworkCore;
using Direct_Barber.Models;

namespace Direct_Barber.Servicios.Contrato
{
    public interface IUsuarioService
    {
        Task<Usuario> GetUsuario(string correo, string contrasena); //Devolver un usuario.
        Task<Usuario> GetUsuarioPorCorreo(string correo); //Método para recuperar el usuario autenticado (para navegación dentro del sistema)
        Task<Usuario> SaveUsuario(Usuario modelo); //Guardar un usuario.
        Task<List<Rol>> GetRoles(); // Método para obtener roles        
        Task<Usuario> GetUsuarioById(int id); // Método para obtener un usuario por ID
        Task<Usuario> UpdateUsuario(Usuario usuario); // Método para actualizar el usuario
        Task<bool> UsuarioExists(int id); // Método para verificar si el usuario existe
        Task<int> ObtenerUsuarioId();
    }
}
