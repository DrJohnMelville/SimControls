using System;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;

namespace SimControls.NetworkCommon.NetworkVariableBinders
{
    public class NetworkVariableBinder: ISimVariableBinder, IAsyncDisposable
    {
        private readonly IBinaryObjectPipeReader source;
        private readonly IBinaryObjectPipeWriter destination;
        private readonly NetworkVariableSynchronizer synchronizer;

        public NetworkVariableBinder(IBinaryObjectPipeReader source, IBinaryObjectPipeWriter destination)
        {
            this.source = source;
            this.destination = destination;
            synchronizer = new NetworkVariableSynchronizer(source, destination);
        }

        public void BindVariableToSimulator<T>(string name, string unit, string simType,
            ReadOnlyDataItem<T> variable)
        {
            synchronizer.RegisterVariable(variable);
            GC.KeepAlive(destination.Write(new BindingRequest(variable.UniqueIndex)));
        }

        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask DisposeAsync() => synchronizer.DisposeAsync();
    }

    public class NetworkVariableServer: NetworkVariableSynchronizer
    {
        private readonly IVariableCache varCache;

        public NetworkVariableServer(
            IVariableCache varCache, IBinaryObjectPipeReader source, IBinaryObjectPipeWriter destination):
            base(source, destination)
        {
            this.varCache = varCache;
        }

        protected override void HandleOtherMessage(object message)
        {
            if (message is BindingRequest req)
            {
                RegisterRemoteVariable(req);
            }
        }

        private void RegisterRemoteVariable(BindingRequest req)
        {
            var variable = varCache.VariableByNumber(req.Index);
            var entry = RegisterVariable(variable);
            entry.TrySendValue();
        }
    }
}