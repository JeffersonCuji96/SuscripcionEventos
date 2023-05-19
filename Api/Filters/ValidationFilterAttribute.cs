using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Filters
{
    public class ValidationFilterAttribute: IActionFilter
    {
        /// <summary>
        /// Filtro de acción
        /// </summary>
        /// <remarks>
        /// Verificar si el modelo utilizado en la acción es válido. El context que se pasa 
        /// como parámetro contiene información sobre la acción que se va a ejecutar y su estado, 
        /// incluyendo el estado del modelo.
        /// </remarks>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
