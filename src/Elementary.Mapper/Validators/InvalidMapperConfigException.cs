using System;
using System.Collections.Generic;
using System.Linq;

namespace Elementary.Mapper.Validators
{
    public class InvalidMapperConfigException : Exception
    {
        public InvalidMapperConfigException(IEnumerable<Violation> violations)
            : base(string.Join(";", violations.Select(v => v.Reason)))
        {
        }
    }
}