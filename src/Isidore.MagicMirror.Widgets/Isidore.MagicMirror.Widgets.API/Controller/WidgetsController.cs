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
                        AuthorId = Guid.NewGuid(),
                        CreationDate = DateTime.Now,
                        Id = Guid.NewGuid().ToString(),
                        Name = "Clock",
                        Template = "<div class=\"time-box\">"+
                                        "<div class=\"time\">{{ context.now | date:'shortTime'}}</div>"+
                                        "<div class=\"date\">{{ context.now | date:'EEEE'}}</div>"+
                                        "<div class=\"date\">{{ context.now | date:'mediumDate'}}</div>"+
                                    "</div>",
                        Style = ".time-box {  text-align: left; float: left; font-size: 6em; } " +
                        ".date {font-size: 1.5rem;}",
                        ModelRefreshInterval = TimeSpan.FromSeconds(1)
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
