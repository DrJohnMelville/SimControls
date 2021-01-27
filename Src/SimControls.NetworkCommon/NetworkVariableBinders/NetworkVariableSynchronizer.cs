using System;
using System.Collections.Generic;
using Melville.INPC;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.Model;
using SimControls.NetworkCommon.DataClasses;

namespace SimControls.NetworkCommon.NetworkVariableBinders
{
    public abstract class NetworkVariableEntry
    {
        private readonly IBinaryObjectPipeWriter writer;

        public static NetworkVariableEntry Create(DataItem variable, IBinaryObjectPipeWriter writer, ushort index) =>
            variable switch
            {
                ReadOnlyDataItem<double> dbl => new DoubleEntry(dbl, writer, index),
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
            private readonly ushort index;
            private readonly ReadOnlyDataItem<double> variable;

            public DoubleEntry(ReadOnlyDataItem<double> variable,
                IBinaryObjectPipeWriter writer, ushort index) : base(variable, writer)
            {
                this.variable = variable;
                this.index = index;
            }

            public override ICanWriteToPipe ToWireFormat() => new DoubleValueRecord(index, variable.Value);

            protected override void ParseWireFormat(ICanWriteToPipe value)
            {
                writeSuppressed = true;
                variable.TryUpdateFromSimulator(((DoubleValueRecord) value).Value);
                writeSuppressed = false;
            }
        }
    }

    public class NetworkVariableSynchronizer
    {
        private readonly IBinaryObjectPipeReader source;
        private readonly IBinaryObjectPipeWriter destination;
        private readonly List<NetworkVariableEntry> monitoredVariables = new();
        protected ushort NextIndex() => (ushort)monitoredVariables.Count;

        public NetworkVariableSynchronizer(IBinaryObjectPipeReader source, IBinaryObjectPipeWriter destination)
        {
            this.source = source;
            this.destination = destination;
        }

        public NetworkVariableEntry RegisterVariable(DataItem variable)
        {
            var entry = NetworkVariableEntry.Create(variable, destination, NextIndex()); 
            monitoredVariables.Add(entry);
            return entry;
        }


        public async void RunMessageLoop()
        {
            await foreach (var item in source.Read())
            {
                switch (item)
                {
                    case DoubleValueRecord dvr:
                        monitoredVariables[dvr.Index].AcceptWireFormat(dvr);
                        break;
                    default:
                        HandleOtherMessage(item);
                        break;
                }
            }
        }

        protected  virtual void HandleOtherMessage(object message)
        {
       }
    }
}