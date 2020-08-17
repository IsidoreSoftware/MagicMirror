using System;
using Isidore.MagicMirror.DAL.MongoDB;

namespace Isidore.MagicMirror.Widgets.Models
{
    public class Widget : BaseMongoObject
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public string Style { get; set; }

        public TimeSpan ModelRefreshInterval { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid AuthorId { get; set; }

        public dynamic[] Models { get; set; }

        public Widget[] ChildWidgets { get; set; }
    }
}
