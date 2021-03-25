using System;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.Model.VariableBinders;
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
            simEvent.RegisterEffector(
                (_,v)=>destination.Write(new FireEventRecord(simEvent.SimulatorEventName, v)));
        }

        public ValueTask DisposeAsync() => synchronizer.DisposeAsync();
    }

    public interface INetworkVariableServer
    {
    }

    public class NetworkVariableServer: NetworkVariableSynchronizer, INetworkVariableServer
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
            switch (message)
            {
                case BindingRequest req:
                    RegisterRemoteVariable(req);
                    break;
                case FireEventRecord fer:
                    varCache.GetEvent(fer.EventName).Fire(fer.Value);
                    break;
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