using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public interface IParametroService
    {
        Task<string> ObtenerValor(string nombre);

    }
}
