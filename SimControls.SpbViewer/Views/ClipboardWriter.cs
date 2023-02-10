using System.Windows;

namespace SimControls.SpbViewer.Views;

public interface IClipboardWriter
{
    void WriteTextString(string text);
}
public class ClipboardWriter : IClipboardWriter
{
    public void WriteTextString(string text)
    {
        Clipboard.SetText(text);
    }
}