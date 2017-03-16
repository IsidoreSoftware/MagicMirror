using Isidore.MagicMirror.Widgets.Contract;
using Nancy;

namespace Isidore.MagicMirror.Widgets.API.Controller
{
    public class WidgetsController : NancyModule
    {
        private IWidgetService _widgetService;

        public WidgetsController(IWidgetService widgetService) : base("/widgets")
        {
            this._widgetService = widgetService;
            SetUpRoutes();
        }

        private void SetUpRoutes()
        {
            Get("/{id}", async (_, ctx) =>
            {
                if (_["id"] != null)
                    return await _widgetService.GetByIdAsync(_["id"]);
                else
                    return HttpStatusCode.BadRequest;
            });
        }
    }
}
