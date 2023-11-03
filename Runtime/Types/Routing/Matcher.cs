using System.Text.RegularExpressions;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   A matcher that can do an exact string match or
            ///   a regular expression match.
            /// </summary>
            public class Matcher
            {
                // The value to match against.
                private string exactValue;
            
                // The pattern to match against.
                private Regex pattern;

                public Matcher(string value)
                {
                    exactValue = value;
                }

                public Matcher(Regex value)
                {
                    pattern = value;
                }

                /// <summary>
                ///   Attempts to match the input value.
                /// </summary>
                /// <param name="value">The value to match</param>
                /// <returns>The found match(es)</returns>
                public Matches Match(string value)
                {
                    if (exactValue != null)
                    {
                        return exactValue == value ? new Matches(value) : null;
                    }
                    Match match = pattern.Match(value);
                    return match.Success ? new Matches(match) : null;
                }
            }
        }
    }
}
