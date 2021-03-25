using Moq;
using SimControls.Debuggers;
using SimControls.Model;
using SimControls.Model.VariableBinders;
using Xunit;
using ZXing.QrCode.Internal;

namespace SimControls.Test.Debuggers
{
    public class DebuggerViewModelTest
    {
        private readonly Mock<ICompositeVariableBinder> rootBinder = new();
        private readonly DebuggerViewModel sut;

        public DebuggerViewModelTest()
        {
            sut = new DebuggerViewModel(rootBinder.Object);
        }

        [Fact]
        public void RegistersWithRootBinder()
        {
            rootBinder.Verify(i=>i.AddBinder(sut), Times.Once);
            rootBinder.VerifyNoOtherCalls();
        }

        [Fact]
        public void RegisterVariable()
        {
            var item = new ReadOnlyDataItem<double>();
            sut.BindVariableToSimulator("Name", "Unit", "SimType", item);
            Assert.Single(sut.Variables);
            var firstRow = sut.Variables[0];
            Assert.Equal("Name", firstRow.Name);
            Assert.Equal("Unit", firstRow.Unit);
            Assert.Equal("SimType", firstRow.SimType);
            Assert.Equal(item, firstRow.Item);
        }

        [Fact]
        public void RegisterEvent()
        {
            var sEvent = new SimEventTrigger("EvName");
            sut.BindEventToSimulator(sEvent);
            Assert.Single(sut.Events);
            Assert.Equal("EvName", sut.Events[0].SimulatorEventName);
            
        }
    }
}