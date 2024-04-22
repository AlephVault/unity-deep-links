using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   This is the list of matches from either the
            ///   only string or a regular expression.
            /// </summary>
            public class Matches : ReadOnlyCollection<IReadOnlyList<string>>
            {
                public Matches(string exact) : base(new List<IReadOnlyList<string>>{ new List<string>{ exact }}) {}

                // Builds the matches.
                private static List<IReadOnlyList<string>> BuildMatches(Match match)
                {
                    List<IReadOnlyList<string>> allMatches = new List<IReadOnlyList<string>>();

                    foreach(Group group in match.Groups)
                    {
                        allMatches.Add(group.Captures.Select(capture => capture.Value).ToList());
                    }

                    return allMatches;
                }

                public Matches(Match match) : base(BuildMatches(match)) {}
            }
        }
    }
}
