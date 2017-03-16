
using Isidore.MagicMirror.Infrastructure.Services;
using Isidore.MagicMirror.Widgets.Models;

namespace Isidore.MagicMirror.Widgets.Contract
{
    public interface IWidgetService: IDataService<Widget>, IAsyncDataService<Widget>
    {
    }
}
