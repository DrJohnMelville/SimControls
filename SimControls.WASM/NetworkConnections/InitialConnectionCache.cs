using System;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using Melville.P2P.Raw.Matchmaker;
using SimControls.Model;

namespace SimControls.WASM.NetworkConnections
{
    public interface IInitialConnectionCache
    {
        Task WaitForConnection();
    }

    public class InitialConnectionCache: ISimVariableBinder, IInitialConnectionCache
    {
        private ISimVariableBinder? innerBinder;

        private readonly Func<BinaryObjectPipeReader, BinaryObjectPipeWriter, ISimVariableBinder>
            binderFactory;

        public InitialConnectionCache(Func<BinaryObjectPipeReader, BinaryObjectPipeWriter, ISimVariableBinder> binderFactory)
        {
            this.binderFactory = binderFactory;
        }

        public void BindVariableToSimulator<T>(
            string name, string unit, string simType, ReadOnlyDataItem<T> variable) => 
            innerBinder?.BindVariableToSimulator(name, unit, simType, variable);

        public void BindEventToSimulator(SimEventTrigger simEvent) =>
            innerBinder?.BindEventToSimulator(simEvent);

        public Task WaitForConnection()
        {
            return new TaskCompletionSource().Task; 
//            var (reader, writer) = await client.Connect();
//            innerBinder = binderFactory(reader, writer);
        }
    }
}