using System.Threading.Tasks;

namespace SimControls.Model
{
    public static class ValueTaskExtension
    {
        public static ValueTask AsValueTask<T>(this ValueTask<T> valueTask)
        {
            if (valueTask.IsCompletedSuccessfully)
            {
                valueTask.GetAwaiter().GetResult();
                return default;
            }

            return new ValueTask(valueTask.AsTask());
        }
    }
}