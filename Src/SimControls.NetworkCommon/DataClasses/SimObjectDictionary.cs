using Melville.P2P.Raw.BinaryObjectPipes;

namespace SimControls.NetworkCommon.DataClasses
{
    public class SimObjectDictionary : BinaryObjectDictionary
    {
        public SimObjectDictionary()
        {
            Register<BindingRequest>(BindingRequest.ReadFromPipe);
            Register<DoubleValueRecord>(DoubleValueRecord.ReadFromPipe);
        }
    }
}