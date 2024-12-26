/// <summary>
/// Atributo personalizado para autorizar a los usuarios según su nivel de acceso.
/// </summary>
/// <remarks>
/// Este atributo verifica si un usuario autenticado tiene un tipo de acceso válido 
/// especificado en los parámetros del constructor. Si el usuario no está autenticado 
/// o su tipo de acceso no coincide, se devuelve un resultado de autorización denegada.
/// </remarks>
/// <param name="accessTypes">Lista de tipos de acceso permitidos para esta acción o controlador.</param>
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;


namespace SysSoniaInventory.Task
{
    public class AccessLevelAuthorize : AuthorizeAttribute, IAuthorizationFilter
    {
            private readonly string[] _accessTypes;

        /// <summary>
        /// Constructor que inicializa los tipos de acceso permitidos.
        /// </summary>
        /// <param name="accessTypes">Lista de tipos de acceso permitidos.</param>
        public AccessLevelAuthorize(params string[] accessTypes)
            {
                _accessTypes = accessTypes;
            }

        /// <summary>
        /// Método de autorización que valida si el usuario tiene permiso para acceder al recurso.
        /// </summary>
        /// <param name="context">Contexto de la autorización.</param>
        public void OnAuthorization(AuthorizationFilterContext context)
            {
                var user = context.HttpContext.User;

                if (!user.Identity.IsAuthenticated)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var userAccessTipe = user.FindFirst("AccessTipe")?.Value;

                if (string.IsNullOrEmpty(userAccessTipe) || !_accessTypes.Contains(userAccessTipe))
                {
                    context.Result = new ForbidResult();
                }
            }
        
    }
}
