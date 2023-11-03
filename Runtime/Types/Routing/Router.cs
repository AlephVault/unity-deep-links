using System;
using System.Collections.Generic;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   A router has an internal rule parser and processes
            /// </summary>
            public class Router
            {
                // The internal parser.
                private DeepLinkParser deepLinkParser = new DeepLinkParser();

                // The internal exporter.
                private DeepLinkExporter deepLinkExporter = new DeepLinkExporter();

                // The deep link processor callbacks.
                private List<Tuple<Type, Action<DeepLinkModel>>> onDeepLinkCallbacks = new List<Tuple<Type, Action<DeepLinkModel>>>();

                /// <summary>
                ///   Adds a new parsing rule.
                /// </summary>
                /// <param name="name">The debug-only name of the rule</param>
                /// <returns>The new rule</returns>
                public Rule AddParsingRule(string name)
                {
                    return deepLinkParser.AddRule(name);
                }

                /// <summary>
                ///   Adds a new export rule.
                /// </summary>
                /// <param name="exporter">The exporter function</param>
                /// <returns>The rule</returns>
                public void AddExportRule<DeepLinkModelType>(Func<DeepLinkModelType, string> exporter) where DeepLinkModelType : DeepLinkModel
                {
                    deepLinkExporter.AddRule<DeepLinkModelType>(exporter);
                }
                
                /// <summary>
                ///   Adds a handler to process deep links of certain type. Only
                ///   the first callback whose type can receive the type of the
                ///   parsed deep link will be executed, so avoid using the same
                ///   type twice, or using more general types at first. 
                /// </summary>
                /// <param name="callback">The callback to use to process an incoming deep link</param>
                /// <typeparam name="DeepLinkModelType">The deep link type</typeparam>
                /// <exception cref="ArgumentException">The deep link type is abstract</exception>
                /// <exception cref="ArgumentNullException">The callback is null</exception>
                public void OnDeepLink<DeepLinkModelType>(Action<DeepLinkModelType> callback) where DeepLinkModelType : DeepLinkModel
                {
                    if (callback == null)
                    {
                        throw new ArgumentNullException(nameof(callback));
                    }
                    
                    if (typeof(DeepLinkModelType).IsAbstract)
                    {
                        throw new ArgumentException("The deep link type must be concrete");
                    }
                    
                    onDeepLinkCallbacks.Add(new Tuple<Type, Action<DeepLinkModel>>(
                        typeof(DeepLinkModelType), deepLinkModel => callback((DeepLinkModelType)deepLinkModel)
                    ));
                }

                /// <summary>
                ///   Processes an incoming URI into a recognized deep link,
                ///   properly triggering its callback.
                /// </summary>
                /// <param name="uri">The uri to parse</param>
                public void ProcessDeepLink(Uri uri)
                {
                    DeepLinkModel parsed = deepLinkParser.Parse(uri);
                    if (parsed == null) return;

                    foreach (var pair in onDeepLinkCallbacks)
                    {
                        if (pair.Item1.IsInstanceOfType(parsed))
                        {
                            pair.Item2(parsed);
                            return;
                        }
                    }
                }

                /// <summary>
                ///   Tries to export a deep link, according to the specified rules,
                ///   for one of the available deep link types. If no match is done,
                ///   then this method returns "about:blank".
                /// </summary>
                /// <param name="deepLinkModel">The deep link to export</param>
                /// <returns>The string-dumped URI of that deep link, according to the rule</returns>
                public string ExportDeepLink(DeepLinkModel deepLinkModel)
                {
                    return deepLinkExporter.ExportDeepLink(deepLinkModel);
                }
            }
        }
    }
}