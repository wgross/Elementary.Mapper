using Elementary.Mapper;
using Xunit;

namespace ELementary.Mapper.Test
{
    public class MapperTest
    {
        private class Source
        {
            public int Integer { get; set; }
        }

        private class Destination
        {
            public int Integer { get; set; }
        }

        [Fact]
        public void Map_value_properties_and_strings()
        {
            // ARRANGE

            var src = new Source
            {
                Integer = 1
            };

            var dst = new Destination();

            var mapper = new MapperBuilder().MapStrict<Source, Destination>().Build();

            // ACT

            var result = mapper(src, dst);

            // ASSERT

            Assert.Equal(1, result.Integer);
        }
    }
}