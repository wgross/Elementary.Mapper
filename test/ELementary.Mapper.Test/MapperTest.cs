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

            var mapper = new MapperBuilder().MapStrict<Source, Destination>().Build();

            // ACT

            var result = new Destination();
            mapper(src, result);

            // ASSERT

            Assert.Equal(1, result.Integer);
        }
    }
}