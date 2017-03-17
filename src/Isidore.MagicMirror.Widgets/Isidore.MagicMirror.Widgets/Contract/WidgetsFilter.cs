using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Widgets.Models;
using System;

namespace Isidore.MagicMirror.Widgets.Contract
{
    class WidgetsFilter : MongoDbFilter<Widget>
    {
        long Id { get; set; }
        String Name { get; set; }
    }
}
