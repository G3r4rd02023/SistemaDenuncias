using Microsoft.AspNetCore.Mvc.Rendering;

namespace UMA_SYSTEM.Frontend.Services
{
    public interface IServicioLista
    {
        Task<IEnumerable<SelectListItem>> GetListaEstados();

        Task<IEnumerable<SelectListItem>> GetListaTipos();

        Task<IEnumerable<SelectListItem>> GetListaRoles();

        Task<string> ObtenerCodigo();

    }
}
