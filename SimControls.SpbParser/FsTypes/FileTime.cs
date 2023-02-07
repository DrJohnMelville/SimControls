using System;

namespace SimControls.SpbParser.FsTypes;

#pragma warning disable 0649
public readonly struct FileTime
{
    // this class is a total guess that Spb filetime records are just the filetime structure
    //from minwinbase written directly out to a file.
    public readonly UInt32 LowFileTime;
    public readonly UInt32 HighFileTime;
}