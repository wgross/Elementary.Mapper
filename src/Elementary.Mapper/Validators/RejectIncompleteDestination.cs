using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elementary.Mapper.Validators
{
    public sealed class RejectIncompleteDestination : IMapperValidator
    {
        public IEnumerable<Violation> Validate<S, D>(IEnumerable<(PropertyInfo src, PropertyInfo dst)> propertyPairs) => propertyPairs
            .Where(pp => pp.src != null && pp.dst is null)
            .Select(pp => new Violation($"{typeof(D).Name} is missing property {pp.src.Name}"));
    }
}