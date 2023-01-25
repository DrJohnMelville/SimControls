namespace SimControls.SpbParser.ValueReaders;

public static class WithAssignmentImpl
{
    public static TRet WithAssignment<TRet, TArg>(this TRet ret, TArg value, out TArg target)
    {
        target = value;
        return ret;
    }
}