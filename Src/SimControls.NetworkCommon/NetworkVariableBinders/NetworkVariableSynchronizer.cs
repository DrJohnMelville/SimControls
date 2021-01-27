using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.INPC;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;

namespace SimControls.NetworkCommon.NetworkVariableBinders
{
    public abstract class NetworkVariableEntry
    {
        private readonly IBinaryObjectPipeWriter writer;

        public static NetworkVariableEntry Create(DataItem variable, IBinaryObjectPipeWriter writer) =>
            variable switch
            {
                ReadOnlyDataItem<double> dbl => new DoubleEntry(dbl, writer),
                _ => throw new InvalidOperationException("Unknown Data Type")
            };

        public NetworkVariableEntry(DataItem variable, IBinaryObjectPipeWriter writer)
        {
            this.writer = writer;
            variable.WhenMemberChanges("Value", TrySendValue);
        }

        private bool writeSuppressed;

        public void TrySendValue()
        {
            if (writeSuppressed) return;
            GC.KeepAlive(writer.Write(ToWireFormat()));
        }

        public abstract ICanWriteToPipe ToWireFormat();

        public void AcceptWireFormat(ICanWriteToPipe value)
        {
            writeSuppressed = true;
            try
            {
                ParseWireFormat(value);
            }
            finally
            {
                writeSuppressed = false;
            }
        }

        protected abstract void ParseWireFormat(ICanWriteToPipe value);

        private sealed class DoubleEntry : NetworkVariableEntry
        {
            private readonly ReadOnlyDataItem<double> variable;

            public DoubleEntry(ReadOnlyDataItem<double> variable,
                IBinaryObjectPipeWriter writer) : base(variable, writer)
            {
                this.variable = variable;
            }

            public override ICanWriteToPipe ToWireFormat() => 
                new DoubleValueRecord(variable.UniqueIndex, variable.Value);

            protected override void ParseWireFormat(ICanWriteToPipe value)
            {
                writeSuppressed = true;
                variable.TryUpdateFromSimulator(((DoubleValueRecord) value).Value);
                writeSuppressed = false;
            }
        }
    }

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
                switch (item)
                {
                    case DoubleValueRecord dvr:
                        monitoredVariables[dvr.Index].AcceptWireFormat(dvr);
                        break;
                    case TerminateConnection: return;
                    default:
                        HandleOtherMessage(item);
                        break;
                }
            }
        }

        protected  virtual void HandleOtherMessage(object message)
        {
       }

        public ValueTask DisposeAsync() => destination.Write(new TerminateConnection()).AsValueTask();
    }
}