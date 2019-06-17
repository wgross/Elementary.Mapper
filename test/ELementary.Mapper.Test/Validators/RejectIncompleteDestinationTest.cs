using System.Linq;
using Xunit;

namespace Elementary.Mapper.Validators.Test
{
    public class RejectIncompleteDestinationTest
    {
        private class Source
        {
            public int Property { get; set; }
        }

        private class Destination
        {
        }

        [Fact]
        public void Find_missing_destination_property()
        {
            // ARRANGE

            var properties = new SelectValueTypeAndStringPropertiesOfIdenticalName().SelectProperties<Source, Destination>();

            // ACT

            var result = new RejectIncompleteDestination().Validate<Source, Destination>(properties).ToArray();

            // ASSERT

            Assert.Single(result);
            Assert.Equal($"{ typeof(Destination).Name} is missing property { nameof(Source.Property)}", result.Single().Reason);
        }
    }
}