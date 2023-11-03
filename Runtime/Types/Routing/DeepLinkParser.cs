using System;
using System.Collections.Generic;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   The deep link parser creates <see cref="DeepLinkModel" />
            ///   instances from a given URI.
            /// </summary>
            public class DeepLinkParser
            {
                // The rules.
                private List<Rule> rules = new List<Rule>();

                /// <summary>
                ///   Adds, and returns, a new rule.
                /// </summary>
                /// <param name="name">The name for the new rule</param>
                /// <returns>The rule</returns>
                public Rule AddRule(string name)
                {
                    Rule rule = new Rule(name);
                    rules.Add(rule);
                    return rule;
                }

                /// <summary>
                ///   Tries to parse a URI, according to the specified rules,
                ///   into one of the available deep links. If no match is done,
                ///   then this method returns null.
                /// </summary>
                /// <param name="uri">The uri to parse</param>
                /// <returns>The deep link, or null if no match was done</returns>
                public DeepLinkModel Parse(Uri uri)
                {
                    foreach (Rule rule in rules)
                    {
                        DeepLinkModel deepLinkModel = rule.Match(uri);
                        if (deepLinkModel != null) return deepLinkModel;
                    }

                    return null;
                }
            }
        }
    }
}
