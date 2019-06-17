using System.Linq;
using Xunit;

namespace Elementary.Mapper.Test
{
    public class SelectValueTypeAndStringPropertiesOfIdenticalNameTest
    {
        private class Source
        {
            public int Integer { get; set; }

            public string String { get; set; }

            public object ReferenceType { get; set; }

            public int WrongType { get; set; }

            public int WrongName { get; set; }
        }

        private class Destination
        {
            public int Integer { get; set; }

            public string String { get; set; }

            public object ReferenceType { get; set; }

            public long WrongType { get; set; }

            public int wrongName { get; set; }
        }

        [Fact]
        public void Select_ValueType_and_String_properties_of_identical_name_and_type_only()
        {
            // ARRANGE

            var selector = new SelectValueTypeAndStringPropertiesOfIdenticalName();

            // ACT

            var result = selector.SelectProperties<Source, Destination>().ToArray();

            // ASSERT
            // all properties wrere found except reference type. WronName has no destination

            Assert.Equal(4, result.Count());
            Assert.Equal(nameof(Source.Integer), result.ElementAt(0).src.Name);
            Assert.Equal(nameof(Source.Integer), result.ElementAt(0).dst.Name);
            Assert.Equal(nameof(Source.String), result.ElementAt(1).src.Name);
            Assert.Equal(nameof(Source.String), result.ElementAt(1).dst.Name);
            Assert.Equal(nameof(Source.WrongType), result.ElementAt(2).src.Name);
            Assert.Equal(nameof(Source.WrongType), result.ElementAt(2).dst.Name);
            Assert.Equal(nameof(Source.WrongName), result.ElementAt(3).src.Name);
            Assert.Null(result.ElementAt(3).dst);
        }
    }
}