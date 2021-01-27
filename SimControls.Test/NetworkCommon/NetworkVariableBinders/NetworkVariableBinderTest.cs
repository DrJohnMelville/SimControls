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
    public class NetworkVariableBinderTest: IAsyncLifetime
    {
        private readonly DataItem<double> serverDoubleValue = new DataItem<double>() {UniqueIndex = 11};
        private readonly DataItem<double> clientDoubleValue = new DataItem<double>() {UniqueIndex = 11};
        private readonly Mock<IVariableCache> varCache = new();
        private readonly Pipe clientToServer = new();
        private readonly Pipe serverToClient = new();
        private readonly NetworkVariableBinder client;
        private readonly NetworkVariableServer server;

        public NetworkVariableBinderTest()
        {
            varCache.Setup(i => i.GetVariable<double, DataItem<double>>
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 11))
                .Returns(serverDoubleValue);
            
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
        public async Task ReadValueFromServer()
        {
            serverDoubleValue.Value = 1234;
            client.BindVariableToSimulator("name", "unit", "double", clientDoubleValue);

            while (clientDoubleValue.Value < 1233) ; // do nothing
            Assert.Equal(1234, clientDoubleValue.Value);
            clientDoubleValue.Value = 12;
            while (serverDoubleValue.Value > 13) ;//do nothing
            Assert.Equal(12, serverDoubleValue.Value);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await server.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}