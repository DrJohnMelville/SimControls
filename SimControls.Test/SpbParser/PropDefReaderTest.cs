using System.Threading.Tasks;
using SimControls.SbpViewer.DefaultPropDefs;
using SimControls.SbpViewer.PropertyAndSetDeclarations;
using SimControls.SbpViewer.ValueReaders;
using Xunit;

namespace SimControls.Test.SpbParser;

public class PropDefReaderTest
{
    [Fact]
    public async Task ReadPropertyXmlAsync()
    {
        var src = DefaultPropertyLibrary.GetDefaultStream("propflightplan.xml");
        var reg = await PropertyRegistryParser.ParseAsync(src!);
        var fpset = reg.Get("AB92F550-AE71-49a9-A8D2-4432FC710E41") as SetDecl;
        Assert.Equal("FlightPlan", fpset!.Name);
        Assert.Equal("Flight Plan", fpset.Description);

        var alt2 = reg.Get("FED18A5B-EAAA-406E-92A0-42F4F7A56731") as PropertyDecl;
        Assert.Equal("Alt2FP", alt2!.Name);
        Assert.Equal("0", alt2.Default);
        Assert.Equal("", alt2.Description);
        Assert.Equal(ValueReaderFactory.Create("LONG"), alt2.Parser);

        var appType = reg.Get("5AF36CA5-E86E-43F1-86FD-F9CC092FDA09") as PropertyDecl;
        Assert.Equal("ApproachTypeFP", appType!.Name);
        Assert.Equal("", appType.Default);
        Assert.Equal("APPROACH_TYPE", appType.Description);
        var parser = appType.Parser as EnumParser;

        var values = new string[]
        {
            "GPS (0)",
            "VOR (1)",
            "NDB (2)",
            "ILS (3)",
            "LOCALIZER (4)",
            "SDF (5)",
            "LDA (6)",
            "VORDME (7)",
            "NDBDME (8)",
            "RNAV (9)",
            "LOCALIZER_BACK_COURSE (10)"
        };
        for (int i = 0; i < values.Length; i++)

        {
            Assert.Equal(values[i], parser!.StringFromValue(i));
        }
    }
    [Fact]
    public async Task ReadAllPropAsync()
    {
        Assert.Equal(2890, (await DefaultPropertyLibrary.GlobalRegistryAsync()).ItemCount );
    }
}