using System;
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
                ReadOnlyBoolItem bi=> new BooleanEntry(bi, writer),
                BoolItem bi=> new BooleanEntry(bi, writer),
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
        public abstract void FromWireFormat(ICanWriteToPipe value);

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

        protected void ParseWireFormat(ICanWriteToPipe value)
        {
            writeSuppressed = true;
            FromWireFormat(value);
            writeSuppressed = false;
        }

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
            public override void FromWireFormat(ICanWriteToPipe value)
            {
                variable.TryUpdateFromSimulator(((DoubleValueRecord) value).Value);
            }
        }
        private sealed class BooleanEntry : NetworkVariableEntry
        {
            private readonly DataItem<int> variable;

            public BooleanEntry(DataItem<int> variable,
                IBinaryObjectPipeWriter writer) : base(variable, writer)
            {
                this.variable = variable;
            }
            public override ICanWriteToPipe ToWireFormat() => 
                new ByteValueRecord(variable.UniqueIndex, (byte)variable.Value);
            public override void FromWireFormat(ICanWriteToPipe value)
            {
                variable.TryUpdateFromSimulator(((ByteValueRecord)value).Value);
            }
        }
    }
}