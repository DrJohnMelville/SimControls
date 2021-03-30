using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimControls.NetworkServer
{
    public class AcceptEncodingParser
    {
        private static readonly Regex finder = new Regex("[a-zA-z]{2,}");
        private string storedQuery = "";
        private List<(string, string)> retlList = new List<(string, string)>();
        public IEnumerable<(string, string)> Extensions(string acceptEncodings)
        {
            if (!acceptEncodings.Equals(storedQuery))
            {
                storedQuery = acceptEncodings;
                RebuildRetList();
            }
            return retlList;
        }

        private void RebuildRetList()
        {
            retlList.Clear();
            foreach (var match in finder.Matches(storedQuery).OfType<Match>())
            {
                retlList.Add(CreateMapping(match));
            }
        }

        private (string, string) CreateMapping(Match i) =>
            i.Value switch
            {
                "gzip" => (".gz", "gzip"),
                var ext => ("." + ext, ext)
            };
    }
}