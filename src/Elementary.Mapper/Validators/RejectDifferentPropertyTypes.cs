using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Mapper.Validators
{
    public class RejectDifferentPropertyTypes : IMapperValidator
    {
        public IEnumerable<Violation> Validate<S, D>(IEnumerable<(PropertyInfo src, PropertyInfo dst)> propertyPairs) => propertyPairs
            .Where(pp => pp.dst != null)
            .Where(pp => !pp.src.PropertyType.Equals(pp.dst.PropertyType))
            .Select(pp => new Violation($"{typeof(S).Name}.{pp.src.Name} type ({pp.src.PropertyType}) is different from {typeof(D).Name}.{pp.dst.Name}: {pp.dst.PropertyType}"));
    }
}