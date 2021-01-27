using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Melville.P2P.Raw.BinaryObjectPipes;
using SimControls.NetworkCommon.DataClasses;
using Xunit;

namespace SimControls.Test.NetworkCommon.DataClasses
{
    public class DataClassSerialization
    {
        private readonly SimObjectDictionary dict = new();
        private readonly BinaryObjectPipeWriter writer;
        private readonly BinaryObjectPipeReader reader;

        public DataClassSerialization()
        {
            var pipe = new Pipe();
            writer = new BinaryObjectPipeWriter(pipe.Writer, dict);
            reader = new BinaryObjectPipeReader(pipe.Reader, dict);
        }

        private async Task TestSerialization<T>(T obj) where T:ICanWriteToPipe
        {
            await writer.Write(obj);
            var copy = await reader.Read().FirstAsync();
            Assert.Equal(obj, (T)copy);
        }

        [Fact]
        public Task SerializeBindingRequest() =>
            TestSerialization(new BindingRequest("Var", "Unit", "type", 2342));
    }
}