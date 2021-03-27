using SimControls.Model;
using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class TwoStepDoubleItemTest
    {
        private readonly DataItem<double> innerItem = new();
        private readonly TwoStepDoubleItem sut;

        public TwoStepDoubleItemTest()
        {
            sut = new TwoStepDoubleItem(
                innerItem, 
                ConstantDataItem.FromValue(0.0), ConstantDataItem.FromValue(100d),
                ConstantDataItem.FromValue(2.0), ConstantDataItem.FromValue(10.0));
        }

        [Fact]
        public void IncrementTest()
        {
            sut.BigIncrement();
            Assert.Equal(10.0, sut.Value);
            sut.BigIncrement();
            Assert.Equal(20, sut.Value);
        }

        [Fact]
        public void DecrementTest()
        {
            sut.Value = 50;
            sut.BigDecrement();
            Assert.Equal(40, sut.Value);
            
        }
    }
}