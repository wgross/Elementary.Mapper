using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Mapper
{
    public sealed class SelectValueTypeAndStringPropertiesOfIdenticalName : IMapperPropertySelector
    {
        public IEnumerable<(PropertyInfo src, PropertyInfo dst)> SelectProperties<S, D>()
        {
            return typeof(S)
                .GetProperties()
                // exclude reference types except strings
                .Where(p => p.PropertyType.IsValueType || p.PropertyType.Equals(typeof(string)))
                .Select(p => (src: p, dst: typeof(D).GetProperty(p.Name)));
                // exclude not matching property types
                //.Where(pp => pp.src.PropertyType.Equals(pp.dst?.PropertyType));
        }
    }

    public interface IMapperPropertySelector
    {
        IEnumerable<(PropertyInfo src, PropertyInfo dst)> SelectProperties<S, D>();
    }
}