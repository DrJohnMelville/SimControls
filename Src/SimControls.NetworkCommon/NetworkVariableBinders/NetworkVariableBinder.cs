using System;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;

namespace SimControls.NetworkCommon.NetworkVariableBinders
{
    public class NetworkVariableBinder: ISimVariableBinder
    {
        private readonly IBinaryObjectPipeReader source;
        private readonly IBinaryObjectPipeWriter destination;

        public NetworkVariableBinder(IBinaryObjectPipeReader source, IBinaryObjectPipeWriter destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public void BindVariableToSimulator<T>(string name, string unit, string simType,
            ReadOnlyDataItem<T> variable)
        {
            GC.KeepAlive(destination.Write(new BindingRequest(variable.UniqueIndex)));
        }

        public void BindEventToSimulator(SimEventTrigger simEvent)
        {
            throw new System.NotImplementedException();
        }
   
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
            if (req.Index != NextIndex())
                throw new InvalidOperationException();
            var variable = varCache.VariableByNumber(req.Index);
            var entry = RegisterVariable(variable);
            entry.TrySendValue();
        }
    }
}