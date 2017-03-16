using System;
using Isidore.MagicMirror.DAL.MongoDB;
using Isidore.MagicMirror.Widgets.Models;
using MongoDB.Driver;
using Isidore.MagicMirror.Widgets.Contract;

namespace Isidore.MagicMirror.Widgets.Services
{
    public class WidgetService : MongoDataService<Widget>, IWidgetService
    {
        public WidgetService(IMongoDatabase database) : base(database, "widgets")
        {
        }

        protected override string EntityIdPropertyName => nameof(Widget.Id);
    }
}
