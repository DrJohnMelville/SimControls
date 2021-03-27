using SimControls.Model;
using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class BoundedDoubleItemTest
    {
        private readonly DataItem<double> innerItem = new();
        
        [Theory]
        [InlineData(-1,0)]
        [InlineData(0,0)]
        [InlineData(1,1)]
        [InlineData(5,5)]
        [InlineData(9,9)]
        [InlineData(10,10)]
        [InlineData(11,10)]
        [InlineData(11234,10)]
        public void ClampingValueTest(double set, double result)
        { 
            var sut = new BoundedDoubleItem(
                innerItem, ConstantDataItem.FromValue(0d), ConstantDataItem.FromValue(10d));
            sut.Value = set;
            Assert.Equal(result, sut.Value);
            
        }

        [Theory]
        [InlineData(-1,9)]
        [InlineData(0,0)]
        [InlineData(1,1)]
        [InlineData(5,5)]
        [InlineData(9,9)]
        [InlineData(10,0)]
        [InlineData(11,1)]
        [InlineData(15,5)]
        public void RollingValueTest(double set, double result)
        { 
            var sut = new BoundedDoubleItem(
                innerItem, ConstantDataItem.FromValue(0d), ConstantDataItem.FromValue(10d),
                BoundedDoubleItem.Rolling);
            sut.Value = set;
            Assert.Equal(result, sut.Value);
        }

    }
}