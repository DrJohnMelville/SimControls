using SimControls.Model;
using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class BoundedDoubleItemTest
    {
        private readonly DataItem<double> innerItem = new();

        private BoundedDoubleItem CreateSut(DoubleClampStratey? doubleClampStratey) =>
            new(
                innerItem, ConstantDataItem.FromValue(0d), ConstantDataItem.FromValue(10d),
                doubleClampStratey);

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
            var sut =CreateSut(BoundedDoubleItem.Clamping);
            sut.Value = set;
            Assert.Equal(result, innerItem.Value);
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
            var sut = CreateSut(BoundedDoubleItem.Rolling);
            sut.Value = set;
            Assert.Equal(result, innerItem.Value);
            Assert.Equal(result, sut.Value);
        }

        [Theory]
        [InlineData(10, 0)]
        [InlineData(9, 1)]
        [InlineData(7, 3)]
        [InlineData(5 ,5)]
        public void InvertedValueTest(double a, double b)
        {
            var sut = CreateSut(BoundedDoubleItem.Clamping);
            innerItem.Value = a;
            Assert.Equal(b, sut.InvertedValue);
            Assert.Equal(a, sut.Value);

            sut.InvertedValue = a;
            Assert.Equal(b, sut.Value);
            Assert.Equal(b, innerItem.Value);
            
        }
    }
}