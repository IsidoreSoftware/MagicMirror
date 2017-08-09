using Isidore.MagicMirror.Infrastructure.Services;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace Isidore.MagicMirror.DAL.MongoDB
{
    public abstract class MongoDbFilter<T> : IFilter<T>
    {
        public string QueryString { get => FilterToJson(); }

        protected string FilterToJson()
        {
            var type = this.GetType();
            var conditions = new Dictionary<string, object>();
            foreach (var property in type.GetRuntimeProperties())
            {
                if (property.Name != nameof(QueryString))
                {
                    var value = property.GetValue(this);

                    if (value != null)
                    {
                        conditions.Add(property.Name, value);
                    }
                }
            }

            return JsonConvert.SerializeObject(conditions);
        }
    }
}
