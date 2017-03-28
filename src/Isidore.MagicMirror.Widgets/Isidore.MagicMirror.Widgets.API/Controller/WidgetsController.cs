using Isidore.MagicMirror.Widgets.Contract;
using Isidore.MagicMirror.Widgets.Models;
using Nancy;
using System;

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
                var id = (string)_.id;

                if (id != null)
                {
                    var widgets = new[]{new Widget()
                    {
                        AuthorId = 1,
                        CreationDate = DateTime.Now,
                        Id = "1",
                        Name = "Clock",
                        Template = "<div>Sum 2+5 =  {{2+5}}</div>",
                        Style = "{\"color\":\"red\" }"
                    }};
                    return widgets;

                    var widget = await _widgetService.GetByIdAsync(id);
                    if (widget == null)
                        return HttpStatusCode.NotFound;

                    return widget;
                }
                else
                    return HttpStatusCode.BadRequest;
            });
        }
    }
}
