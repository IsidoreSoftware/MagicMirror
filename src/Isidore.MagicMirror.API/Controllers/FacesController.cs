using Nancy;

namespace Isidore.MagicMirror.API.Controllers
{
    public class FacesController : NancyModule
    {
        public FacesController()
        {
            Get("/", args => $"Hello World, it's Nancy on .NET Core. {args}");
        }
    }
}
