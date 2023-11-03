using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   A rule match result, to build a deep link from.
            /// </summary>
            public class RuleMatchResult
            {
                /// <summary>
                ///   The involved Uri.
                /// </summary>
                public Uri Uri;

                /// <summary>
                ///   The matches over the scheme. Typically, Group(0).Captures(0) matters.
                /// </summary>
                public readonly Matches SchemeMatches;

                /// <summary>
                ///   The matches over the authority. Typically, Group(0).Captures(0) matters.
                /// </summary>
                public readonly Matches AuthorityMatches;

                /// <summary>
                ///   The matches over the path. Typically, Group(0).Captures(0) matters.
                /// </summary>
                public readonly Matches PathMatches;

                /// <summary>
                ///   The querystring. It is completely optional.
                /// </summary>
                public readonly IReadOnlyDictionary<string, IReadOnlyCollection<string>> QueryString;

                public RuleMatchResult(Uri uri, Matches scheme, Matches authority, Matches path, string queryString)
                {
                    Uri = uri;
                    SchemeMatches = scheme;
                    AuthorityMatches = authority;
                    PathMatches = path;
                    queryString = queryString?.Trim();

                    if (!string.IsNullOrEmpty(queryString))
                    {
                        Dictionary<string, List<string>> elements = new Dictionary<string, List<string>>();
                        if (queryString.StartsWith("?")) queryString = queryString.Substring(1);

                        foreach (string arg in queryString.Split("&"))
                        {
                            if (string.IsNullOrEmpty(arg)) continue;
                            var parts = arg.Split("=", 2);
                            if (!elements.ContainsKey(parts[0])) elements[parts[0]] = new List<string>();
                            elements[parts[0]].Add(parts[1]);
                        }

                        Dictionary<string, IReadOnlyCollection<string>> readonlyElements =
                            new Dictionary<string, IReadOnlyCollection<string>>();

                        foreach (var pair in elements)
                        {
                            readonlyElements.Add(pair.Key, new ReadOnlyCollection<string>(pair.Value));
                        }

                        QueryString = new ReadOnlyDictionary<string, IReadOnlyCollection<string>>(readonlyElements);
                    }
                }
                
                /// <summary>
                ///   Gets a value from a parsed query string and
                ///   at a particular index (by default: 0).
                /// </summary>
                /// <param name="name">The parameter name</param>
                /// <param name="index">The index under that name (by default: 0)</param>
                /// <returns>The string</returns>
                public string GetQueryStringParameter(string name, int index = 0)
                {
                    return GetQueryStringParameters(name)?.ElementAt(index);
                }

                /// <summary>
                ///   Gets all the values from a parsed query string
                ///   for a given parameter name.
                /// </summary>
                /// <param name="name">The parameter name</param>
                /// <returns>The list of elements</returns>
                public IReadOnlyCollection<string> GetQueryStringParameters(string name)
                {
                    if (!QueryString.TryGetValue(name, out var toValue))
                    {
                        return null;
                    }

                    return toValue;
                }
            }
        }
    }
}
