using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class ConstantDataItemTest
    {
        [Fact]
        public void HasConstantValue()
        {
            Assert.Equal(214, ConstantDataItem.FromValue(214).Value);
        }
    }
}