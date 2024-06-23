using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public interface IBitacoraService
    {
        Task<Bitacora> AgregarRegistro(int usuarioId, int objetoId, string accion, string descripcion);

    }

}
