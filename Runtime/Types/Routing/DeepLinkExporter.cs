using System;
using System.Collections.Generic;

namespace AlephVault.Unity.DeepLinks
{
    namespace Types
    {
        namespace Routing
        {
            /// <summary>
            ///   Exports a deep link according to the type.
            /// </summary>
            public class DeepLinkExporter
            {
                private List<Tuple<Type, Func<DeepLinkModel, string>>> exportRules =
                    new List<Tuple<Type, Func<DeepLinkModel, string>>>();

                /// <summary>
                ///   Adds a new export rule.
                /// </summary>
                /// <param name="exporter">The exporter function</param>
                /// <returns>The rule</returns>
                public void AddRule<DeepLinkModelType>(Func<DeepLinkModelType, string> exporter) where DeepLinkModelType : DeepLinkModel
                {
                    if (exporter == null)
                    {
                        throw new ArgumentNullException(nameof(exporter));
                    }
                    
                    if (typeof(DeepLinkModelType).IsAbstract)
                    {
                        throw new ArgumentException("The deep link type must be concrete");
                    }
                    
                    exportRules.Add(new Tuple<Type, Func<DeepLinkModel, string>>(typeof(DeepLinkModelType), deepLinkModel =>
                    {
                        return exporter((DeepLinkModelType)deepLinkModel);
                    }));
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
                    foreach (var pair in exportRules)
                    {
                        if (pair.Item1.IsInstanceOfType(deepLinkModel))
                        {
                            return pair.Item2(deepLinkModel);
                        }
                    }

                    return "about:blank";
                }
            }
        }
    }
}