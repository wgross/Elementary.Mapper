using System;

namespace Elementary.Mapper
{
    public interface IMapper<S, D>
    {
        D Map(S s, D d);
    }

    public class MapperBuilder
    {
        private class Mapper<S, D> : IMapper<S, D>
        {
            public D Map(S s, D d)
            {
                return d;
            }
        }

        public class Config<S, D>
        {
            public Func<S, D, D> Build()
            {
                return (s, d) => d;
            }
        }

        public Config<S, D> MapStrict<S, D>()
        {
            return new Config<S, D>();
        }
    }
}