using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;

namespace SimControls.NetworkCommon.NetworkVariableBinders
{
    public class NetworkVariableSynchronizer: IAsyncDisposable
    {
        private readonly IBinaryObjectPipeReader source;
        private readonly IBinaryObjectPipeWriter destination;
        private readonly Dictionary<ushort, NetworkVariableEntry> monitoredVariables = new();

        public NetworkVariableSynchronizer(IBinaryObjectPipeReader source, IBinaryObjectPipeWriter destination)
        {
            this.source = source;
            this.destination = destination;
            RunMessageLoop();
        }

        public NetworkVariableEntry RegisterVariable(DataItem variable)
        {
            var entry = NetworkVariableEntry.Create(variable, destination); 
            monitoredVariables.Add(variable.UniqueIndex, entry);
            return entry;
        }


        private async void RunMessageLoop()
        {
            await foreach (var item in source.Read())
            {
                if (item is TerminateConnection) return;
                HandleReceivedObject(item);
            }
        }

        private void HandleReceivedObject(object item)
        {
            switch (item)
            {
                #warning -- make an interface to join these two cases.
                case DoubleValueRecord dvr:
                    monitoredVariables[dvr.Index].AcceptWireFormat(dvr);
                    break;
                case ByteValueRecord bvr:
                    monitoredVariables[bvr.Index].AcceptWireFormat(bvr);
                    break;
                default:
                    HandleOtherMessage(item);
                    break;
            }
        }

        protected  virtual void HandleOtherMessage(object message)
        {
       }

        public ValueTask DisposeAsync() => destination.Write(new TerminateConnection()).AsValueTask();
    }
}