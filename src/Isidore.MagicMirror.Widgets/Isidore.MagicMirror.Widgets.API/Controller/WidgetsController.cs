using Nancy;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.Widgets.API.Controller
{
    public class WidgetsController : NancyModule
    {
        public WidgetsController():base("/widgets")
        {

                Get("/{id}", async (_, ctx)=>
                {
                    return await Task.FromResult(_["id"]);
                });
        }
    }
}
