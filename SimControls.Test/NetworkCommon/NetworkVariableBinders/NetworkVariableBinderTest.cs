using System.IO.Pipelines;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using Moq;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;
using SimControls.NetworkCommon.NetworkVariableBinders;
using Xunit;

namespace SimControls.Test.NetworkCommon.NetworkVariableBinders
{
    public class NetworkVariableBinderTest
    {
        private readonly DataItem<double> serverDoubleValue = new DataItem<double>();
        private readonly DataItem<double> clientDoubleValue = new DataItem<double>();
        private readonly Mock<IVariableCache> varCache = new();
        private readonly Pipe clientToServer = new();
        private readonly Pipe serverToClient = new();
        private readonly NetworkVariableBinder client;
        private readonly NetworkVariableServer server;

        public NetworkVariableBinderTest()
        {
            varCache.Setup(i => i.GetVariable<double, DataItem<double>>
                ("name", "unit", "double", It.IsAny<ushort>())).Returns(serverDoubleValue);
            
            var dict = new SimObjectDictionary();
            server = new (varCache.Object, 
                new BinaryObjectPipeReader(clientToServer.Reader, dict),
                new BinaryObjectPipeWriter(serverToClient.Writer, dict)
                );
            client = new( 
                new BinaryObjectPipeReader(serverToClient.Reader, dict), 
                new BinaryObjectPipeWriter(clientToServer.Writer, dict)
                );
        }

        [Fact]
        public void ReadValueFromServer()
        {
            serverDoubleValue.Value = 1234;
            
            client.BindVariableToSimulator("name", "unit", "double", clientDoubleValue);

            Assert.Equal(1234, clientDoubleValue.Value);
            
        }
    }
    public class NetworkVariableBinderTest2
    {
        private readonly Mock<IBinaryObjectPipeWriter> writer = new();
        private readonly Mock<IBinaryObjectPipeReader> reader = new();
        private readonly NetworkVariableBinder sut;

        public NetworkVariableBinderTest2()
        {
            sut = new NetworkVariableBinder(reader.Object, writer.Object);
        }

        [Fact]
        public void RegisterSendsToNetwork()
        {
            writer.Setup(i => i.Write(It.IsAny<BindingRequest>())).Returns((BindingRequest message) =>
            {
                var br = (BindingRequest) message;
                Assert.Equal(0, br.Index);
                return new ValueTask<FlushResult>(new FlushResult());
            });
            sut.BindVariableToSimulator("VarName", "Number", "SimType", new ReadOnlyBoolItem());
            writer.Verify(i=>i.Write(It.IsAny<ICanWriteToPipe>()));
        }
    }
}