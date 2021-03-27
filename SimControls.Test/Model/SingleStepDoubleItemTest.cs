using SimControls.Model;
using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class SingleStepDoubleItemTest
    {
        private readonly DataItem<double> innerItem = new();
        private readonly SingleStepDoubleItem sut;

        public SingleStepDoubleItemTest()
        {
            sut = new SingleStepDoubleItem(
                innerItem, ConstantDataItem.FromValue(0.0), ConstantDataItem.FromValue(100d),
                ConstantDataItem.FromValue(2.0));
        }

        [Fact]
        public void IncrementTest()
        {
            sut.Increment();
            Assert.Equal(2.0, sut.Value);
            sut.Increment();
            Assert.Equal(4.0, sut.Value);
        }

        [Fact]
        public void DecrementTest()
        {
            sut.Value = 50;
            sut.Decrement();
            Assert.Equal(48, sut.Value);
            
        }
    }
}