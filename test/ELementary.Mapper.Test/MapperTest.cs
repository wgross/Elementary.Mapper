using Elementary.Mapper;
using Elementary.Mapper.Validators;
using System.Linq;
using Xunit;

namespace ELementary.Mapper.Test
{
    public class MapperTest
    {
        private class Source
        {
            public int Integer { get; set; }
            public string String { get; set; }
        }

        private class Destination
        {
            public int Integer { get; set; }
            public string String { get; set; }
        }

        private class InvalidDestination
        {
            public long Integer { get; set; }
        }

        [Fact]
        public void MapBuilder_value_properties_and_strings()
        {
            // ARRANGE

            var src = new Source
            {
                Integer = 1,
                String = "text"
            };

            var mapper = new MapperBuilder().Map<Source, Destination>().Build();

            // ACT

            var result = (Destination)mapper.Map(src, new Destination());

            // ASSERT

            Assert.Equal(1, result.Integer);
            Assert.Equal("text", result.String);
        }

        [Fact]
        public void MapBuilder_maps_value_properties()
        {
            // ARRANGE

            var mapperCfg = new MapperBuilder().Map<Source, Destination>();

            // ACT

            var result = mapperCfg.PropertySelector;

            // ASSERT

            Assert.IsType<SelectValueTypeAndStringPropertiesOfIdenticalName>(result);
        }

        [Fact]
        public void MapBuilder_rejects_missing_destination_properties()
        {
            // ARRANGE

            var mapperCfg = new MapperBuilder().Map<Source, Destination>();

            // ACT

            var result = mapperCfg.Validators.Single(v => v.GetType().Equals(typeof(RejectIncompleteDestination)));

            // ASSERT

            Assert.NotNull(result);
        }

        [Fact]
        public void MapBuilder_rejects_different_property_types()
        {
            // ARRANGE

            var mapperCfg = new MapperBuilder().Map<Source, Destination>();

            // ACT

            var result = mapperCfg.Validators.Single(v => v.GetType().Equals(typeof(RejectDifferentPropertyTypes)));

            // ASSERT

            Assert.NotNull(result);
        }

        [Fact]
        public void MapBuilder_throws_violations()
        {
            // ARRANGE

            var mapperCfg = new MapperBuilder().Map<Source, InvalidDestination>();

            // ACT

            var result = Assert.Throws<InvalidMapperConfigException>(() => mapperCfg.Build());

            // ASSERT

            Assert.NotNull(result);
            Assert.Equal("Source.Integer type (System.Int32) is different from InvalidDestination.Integer: System.Int64;InvalidDestination is missing property String",
                result.Message);
        }

        [Fact]
        public void MapBuilder_build_IMapper_instance()
        {
            // ACT

            var result = new MapperBuilder().Map<Source, Destination>().Build();

            // ASSERT

            Assert.IsType<MapperBuilder.Mapper<Source, Destination>>(result);
        }
    }
}