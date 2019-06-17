using Elementary.Mapper.Validators;
using System.Linq;
using Xunit;

namespace Elementary.Mapper.Test.Validators
{
    public class RejectDifferentPropertyTypesTest
    {
        private class Source
        {
            public int Different { get; set; }

            public int Assignable { get; set; }

            public int NoDestination { get; set; }
        }

        private class Destination
        {
            public string Different { get; set; }

            public long Assignable { get; set; }
        }

        [Fact]
        public void Find_different_property_types()
        {
            // ARRANGE

            var properties = new SelectValueTypeAndStringPropertiesOfIdenticalName().SelectProperties<Source, Destination>();

            // ACT

            var result = new RejectDifferentPropertyTypes().Validate<Source, Destination>(properties).ToArray();

            // ASSERT
            // collect vilolations and ignore missing destinations

            Assert.Equal(2, result.Count());
            Assert.Equal($"{typeof(Source).Name}.{nameof(Source.Different)} type (System.Int32) is different from {typeof(Destination).Name}.{nameof(Destination.Different)}: System.String", result.First().Reason);
            Assert.Equal($"{typeof(Source).Name}.{nameof(Source.Assignable)} type (System.Int32) is different from {typeof(Destination).Name}.{nameof(Destination.Assignable)}: System.Int64", result.Last().Reason);
        }
    }
}