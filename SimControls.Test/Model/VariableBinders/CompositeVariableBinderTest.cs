using Moq;
using SimControls.Model;
using SimControls.Model.VariableBinders;
using Xunit;

namespace SimControls.Test.Model.VariableBinders
{
    public class CompositeVariableBinderTest
    {
        private readonly Mock<ISimVariableBinder> binderA = new();
        private readonly Mock<ISimVariableBinder> binderB = new();
        private readonly DataItem<double> item = new();
        private readonly SimEventTrigger eventTrigger = new SimEventTrigger("xxx_yyy");
        private readonly CompositeVariableBinder sut = new();

        [Fact]
        public void ForwardSingleEvent()
        {
            sut.AddBinder(binderA.Object);
            sut.BindEventToSimulator(eventTrigger);
            binderA.Verify(i=>i.BindEventToSimulator(eventTrigger), Times.Once);
            binderA.VerifyNoOtherCalls();
            binderB.VerifyNoOtherCalls();
        }
        [Fact]
        public void ForwardTwoEvents()
        {
            sut.AddBinder(binderA.Object);
            sut.AddBinder(binderB.Object);
            sut.BindEventToSimulator(eventTrigger);
            binderA.Verify(i=>i.BindEventToSimulator(eventTrigger), Times.Once);
            binderB.Verify(i=>i.BindEventToSimulator(eventTrigger), Times.Once);
            binderA.VerifyNoOtherCalls();
            binderB.VerifyNoOtherCalls();
        }
        [Fact]
        public void RecallEvent()
        {
            sut.AddBinder(binderA.Object);
            sut.BindEventToSimulator(eventTrigger);
            sut.AddBinder(binderB.Object);
            binderA.Verify(i=>i.BindEventToSimulator(eventTrigger), Times.Once);
            binderB.Verify(i=>i.BindEventToSimulator(eventTrigger), Times.Once);
            binderA.VerifyNoOtherCalls();
            binderB.VerifyNoOtherCalls();
        }

        [Fact]
        public void BindSingleVariable()
        {
            sut.AddBinder(binderA.Object);
            sut.AddBinder(binderB.Object);
            sut.BindVariableToSimulator("Name", "Unit", "SType", item);
            binderA.Verify(i=>i.BindVariableToSimulator("Name", "Unit", "SType", item));
            binderB.Verify(i=>i.BindVariableToSimulator("Name", "Unit", "SType", item));
            binderA.VerifyNoOtherCalls();
            binderB.VerifyNoOtherCalls();
        }

        [Fact]
        public void RecallVariable()
        {
            sut.AddBinder(binderA.Object);
            sut.BindVariableToSimulator("Name", "Unit", "SType", item);
            sut.AddBinder(binderB.Object);
            binderA.Verify(i=>i.BindVariableToSimulator("Name", "Unit", "SType", item));
            binderB.Verify(i=>i.BindVariableToSimulator("Name", "Unit", "SType", item));
            binderA.VerifyNoOtherCalls();
            binderB.VerifyNoOtherCalls();
        }
    }
}