using System.Collections.Generic;
using System.Reflection;

namespace Elementary.Mapper.Validators
{
    public sealed class Violation
    {
        public Violation(string reason)
        {
            this.Reason = reason;
        }

        public string Reason { get; }
    }

    public interface IMapperValidator
    {
        IEnumerable<Violation> Validate<S, D>(IEnumerable<(PropertyInfo src, PropertyInfo dst)> propertyPairs);
    }
}