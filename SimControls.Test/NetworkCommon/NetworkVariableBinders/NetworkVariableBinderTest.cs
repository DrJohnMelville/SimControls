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
        private readonly BoolItem servertBoolValue = new BoolItem() {UniqueIndex = 10};
        private readonly BoolItem clientBoolValue = new BoolItem() {UniqueIndex = 10};
        private readonly SimEventTrigger serverEvent = new SimEventTrigger("XXYYY");
        private readonly SimEventTrigger clientEvent = new SimEventTrigger("XXYYY");
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
            varCache.Setup(i => i.GetVariable<int, BoolItem>
                (It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 10))
                .Returns(servertBoolValue);
            varCache.Setup(i => i.GetEvent("XXYYY")).Returns(serverEvent);
            
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
        public async Task ClientEventTransmitsToServerAsync()
        {
            int fired = 0;
            var tcs = new TaskCompletionSource();
            serverEvent.RegisterEffector((_,data)=>
            {
                fired++;
                Assert.Equal(1234u, data);
                tcs.SetResult();
            });
            client.BindEventToSimulator(clientEvent);
            clientEvent.Fire(1234);
            await tcs.Task;
            Assert.Equal(1, fired);
            
        }

        [Fact]
        public void SyncDoubleToServer()
        {
            serverDoubleValue.Value = 1234;
            client.BindVariableToSimulator("name", "unit", "double", clientDoubleValue);

            while (clientDoubleValue.Value < 1233) ; // do nothing
            Assert.Equal(1234, clientDoubleValue.Value);
            clientDoubleValue.Value = 12;
            while (serverDoubleValue.Value > 13) ;//do nothing
            Assert.Equal(12, serverDoubleValue.Value);
        }
        [Fact]
        public void SyncBoolToServer()
        {
            servertBoolValue.BoolValue = true;
            client.BindVariableToSimulator("name", "unit", "double", clientBoolValue);

            while (!clientBoolValue.BoolValue) ; // do nothing
            Assert.True(clientBoolValue.BoolValue);
            clientBoolValue.BoolValue = false;
            while (servertBoolValue.BoolValue) ;//do nothing
            Assert.False(servertBoolValue.BoolValue);
        }

        public Task InitializeAsync() => Task.CompletedTask;

        public async Task DisposeAsync()
        {
            await server.DisposeAsync();
            await client.DisposeAsync();
        }
    }
}