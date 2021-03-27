using Moq;
using SimControls.Model;
using Xunit;
using SimControls.Model.CompositeElements;

namespace SimControls.Test.Model
{
    public class BoolWithToggleElementTest
    {
        private readonly Mock<IReadOnlyBoolItem> item = new();
        private readonly Mock<ISimEventTrigger> trigger = new();
        private readonly BoolWithToggleElement sut;

        public BoolWithToggleElementTest()
        {
            sut = new BoolWithToggleElement(item.Object, trigger.Object);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void GetValue(bool value)
        {
            item.SetupGet(i => i.BoolValue).Returns(value);
            Assert.Equal(value, sut.BoolValue);
        }
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void setValue(bool value)
        {
            item.SetupGet(i => i.BoolValue).Returns(value);
            sut.BoolValue = value;
            trigger.Verify(i => i.Fire(1U), Times.Never);
            sut.BoolValue = !value;
            trigger.Verify(i=>i.Fire(1U), Times.Once);
        }
        
        
    }
}