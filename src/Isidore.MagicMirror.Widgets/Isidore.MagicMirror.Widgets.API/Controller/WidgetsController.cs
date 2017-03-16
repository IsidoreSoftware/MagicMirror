using Nancy;
using System.Threading.Tasks;

namespace Isidore.MagicMirror.Widgets.API.Controller
{
    public class WidgetsController : NancyModule
    {
        public WidgetsController():base("/widgets")
        {

                Get("/{id}",  (_, ctx)=>
                {
                    return Task.FromResult(_["id"]);
                });
        }
    }
}
