using Isidore.MagicMirror.Users.Models;
using System;

namespace Isidore.MagicMirror.Widgets.Models
{
    public class Widget : BaseMongoObject
    {
        public string Name { get; set; }

        public string Template { get; set; }

        public string Style { get; set; }

        public DateTime CreationDate { get; set; }

        public long AuthorId { get; set; }

        public dynamic[] Models { get; set; }

        public Widget[] ChildWidgets { get; set; }
    }
}
