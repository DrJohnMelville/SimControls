using System.Runtime.InteropServices;
using Moq;
using SimControls.Model;
using SimControls.Model.CompositeElements;
using Xunit;

namespace SimControls.Test.Model
{
    public class DoubleWithSetEventTest
    {
        private readonly Mock<IReadOnlyDataItem<double>> source = new();
        private readonly Mock<ISimEventTrigger> setter = new();
        private readonly DoubleWithSetEvent sut;

        public DoubleWithSetEventTest()
        {
            sut = new DoubleWithSetEvent(source.Object, setter.Object);
        }

        [Theory]
        [InlineData(0.0)]
        [InlineData(1.0)]
        public void ReadValue(double value)
        {
            source.SetupGet(i => i.Value).Returns(value);
            Assert.Equal(value, sut.Value);
            
        }

        [Fact]
        public void SetDoubleValue()
        {
            sut.Value = 15;
            setter.Verify(i=>i.Fire(15), Times.Once);
        }
    }
}