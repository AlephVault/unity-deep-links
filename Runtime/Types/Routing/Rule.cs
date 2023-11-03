using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   An intermediate class used to match the current deep link.
            /// </summary>
            public class Rule
            {
                /// <summary>
                ///   A debug-only name for this rule.
                /// </summary>
                public readonly string Name;
                    
                /// <summary>
                ///   The function that builds a deep link from a match result.
                /// </summary>
                public Func<RuleMatchResult, DeepLinkModel> DeepLinkModelBuilder { get; private set; }

                /// <summary>
                ///   The matcher for the scheme.
                /// </summary>
                public Matcher SchemeMatcher { get; private set; } = new Matcher(new Regex(".*"));
                    
                /// <summary>
                ///   The matcher for the authority.
                /// </summary>
                public Matcher AuthorityMatcher { get; private set; } = new Matcher(new Regex(".*"));
                    
                /// <summary>
                ///   The matcher
                /// </summary>
                public Matcher PathMatcher { get; private set; } = new Matcher(new Regex(".*"));

                /// <summary>
                ///   Builds the rule with a debug name.
                /// </summary>
                /// <param name="name">The debug name for this rule</param>
                public Rule(string name)
                {
                    Name = name;
                }
                    
                /// <summary>
                ///   Sets to match the scheme to an exact value.
                /// </summary>
                /// <param name="value">The value to match against</param>
                /// <exception cref="ArgumentException">The value is null or empty</exception>
                public Rule MatchingScheme(string value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("The target value must be set");
                    SchemeMatcher = new Matcher(value.Trim());
                    return this;
                }

                /// <summary>
                ///   Sets to match the scheme to a pattern.
                /// </summary>
                /// <param name="value">The pattern to match against</param>
                /// <exception cref="ArgumentException">The pattern is null</exception>
                public Rule MatchingScheme(Regex value)
                {
                    if (value == null)
                        throw new ArgumentException("The target value must be set");
                    SchemeMatcher = new Matcher(value);
                    return this;
                }

                /// <summary>
                ///   Sets to match the authority to an exact value.
                /// </summary>
                /// <param name="value">The value to match against</param>
                /// <exception cref="ArgumentException">The value is null or empty</exception>
                public Rule MatchingAuthority(string value)
                {
                    if (value == null)
                        throw new ArgumentException("The target value must be set");
                    AuthorityMatcher = new Matcher(value.Trim());
                    return this;
                }

                /// <summary>
                ///   Sets to match the authority to a pattern.
                /// </summary>
                /// <param name="value">The pattern to match against</param>
                /// <exception cref="ArgumentException">The pattern is null</exception>
                public Rule MatchingAuthority(Regex value)
                {
                    if (value == null)
                        throw new ArgumentException("The target value must be set");
                    AuthorityMatcher = new Matcher(value);
                    return this;
                }
                    
                    
                /// <summary>
                ///   Sets to match the path to an exact value.
                /// </summary>
                /// <param name="value">The value to match against</param>
                /// <exception cref="ArgumentException">The value is null or empty</exception>
                public Rule MatchingPath(string value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                        throw new ArgumentException("The target value must be set");
                    PathMatcher = new Matcher(value.Trim());
                    return this;
                }

                /// <summary>
                ///   Sets to match the path to a pattern.
                /// </summary>
                /// <param name="value">The pattern to match against</param>
                /// <exception cref="ArgumentException">The pattern is null</exception>
                public Rule MatchingPath(Regex value)
                {
                    if (value == null)
                        throw new ArgumentException("The target value must be set");
                    PathMatcher = new Matcher(value);
                    return this;
                }

                /// <summary>
                ///   Sets the builder function.
                /// </summary>
                /// <param name="builder">The builder function</param>
                /// <exception cref="ArgumentException">The builder function must not be null</exception>
                public Rule BuildingDeepLinkModelAs(Func<RuleMatchResult, DeepLinkModel> builder)
                {
                    DeepLinkModelBuilder = builder ?? throw new ArgumentException("The builder function must be set");
                    return this;
                }

                private DeepLinkModel MatchUri(Uri uri)
                {
                    if (SchemeMatcher == null || AuthorityMatcher == null || PathMatcher == null ||
                        DeepLinkModelBuilder == null)
                    {
                        Debug.LogWarning($"The matcher \"{Name}\" will be ignored since " +
                                         "it is not completely configured");
                        return null;
                    }

                    Matches scheme = SchemeMatcher.Match(uri.Scheme);
                    Matches authority = AuthorityMatcher.Match(uri.Authority);
                    Matches path = PathMatcher.Match(uri.AbsolutePath);
                    string query = uri.Query;

                    if (scheme != null && authority != null && path != null)
                    {
                        return DeepLinkModelBuilder(new RuleMatchResult(uri, scheme, authority, path, query));
                    }

                    return null;
                }

                /// <summary>
                ///   Performs a match against a given URI. It returns either a successful
                ///   match, or null if it did not match anything.
                /// </summary>
                /// <param name="uri">The URI to match against</param>
                /// <returns>The match result, or null</returns>
                public DeepLinkModel Match(Uri uri)
                {
                    try
                    {
                        return MatchUri(uri);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning($"An exception occurred while parsing using rule \"{Name}\"");
                        Debug.LogException(e);
                        return null;
                    }
                }
            }
        }
    }
}
