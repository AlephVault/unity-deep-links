using System;
using System.Text.RegularExpressions;
using AlephVault.Unity.DeepLinks.Types;
using AlephVault.Unity.DeepLinks.Types.Routing;
using UnityEngine;

namespace AlephVault.Unity.DeepLinks
{
    namespace Samples
    {
        public class SampleDeepLinksRouterAware : MonoBehaviour
        {
            private Router router;

            private class HttpDeepLinkModel : DeepLinkModel
            {
                public readonly string Authority;
                public readonly string Path;

                public HttpDeepLinkModel(string authority, string path)
                {
                    Authority = authority;
                    Path = path;
                }
                
                public override string ToString()
                {
                    return $"https://{Authority}{Path}";
                }
            }

            private class YouTubeDeepLinkModel : DeepLinkModel
            {
                public readonly string Path;

                public YouTubeDeepLinkModel(string path)
                {
                    Path = path;
                }

                public override string ToString()
                {
                    return $"https://www.youtube.com{Path}";
                }
            }

            private class FtpDeepLinkModel : DeepLinkModel
            {
                public readonly string Authority;
                public readonly string Path;
                
                public FtpDeepLinkModel(string authority, string path)
                {
                    Authority = authority;
                    Path = path;
                }
                
                public override string ToString()
                {
                    return $"ftp://{Authority}{Path}";
                }
            }

            private void Awake()
            {
                router = new Router();
                router.AddParsingRule("youtube")
                    .MatchingScheme(new Regex(@"^https?$"))
                    .MatchingAuthority(new Regex(@"^(www\.)?youtube\.com$"))
                    .BuildingDeepLinkModelAs(result => new YouTubeDeepLinkModel(result.PathMatches[0][0]));
                router.OnDeepLink<YouTubeDeepLinkModel>(deepLinkModel =>
                {
                    Debug.Log($"YouTube deep link: {router.ExportDeepLink(deepLinkModel)}");
                });
                router.AddExportRule<YouTubeDeepLinkModel>(deepLinkModel => deepLinkModel.ToString());

                router.AddParsingRule("http")
                    .MatchingScheme(new Regex(@"^https?$"))
                    .BuildingDeepLinkModelAs(result => new HttpDeepLinkModel(
                        result.AuthorityMatches[0][0], result.PathMatches[0][0]
                    ));
                router.OnDeepLink<HttpDeepLinkModel>(deepLinkModel =>
                {
                    Debug.Log($"HTTP deep link: {router.ExportDeepLink(deepLinkModel)}");
                });
                router.AddExportRule<HttpDeepLinkModel>(deepLinkModel => deepLinkModel.ToString());

                router.AddParsingRule("ftp")
                    .MatchingScheme(new Regex(@"^ftp?$"))
                    .BuildingDeepLinkModelAs(result => new HttpDeepLinkModel(
                        result.AuthorityMatches[0][0], result.PathMatches[0][0]
                    ));
                router.OnDeepLink<HttpDeepLinkModel>(deepLinkModel =>
                {
                    Debug.Log($"HTTP deep link: {router.ExportDeepLink(deepLinkModel)}");
                });
                router.AddExportRule<FtpDeepLinkModel>(deepLinkModel => deepLinkModel.ToString());
            }

            void Start()
            {
                router.ProcessDeepLink(new Uri("http://youtube.com"));
                router.ProcessDeepLink(new Uri("http://youtube.com/sample-path"));
                router.ProcessDeepLink(new Uri("http://google.com/sample-path"));
                router.ProcessDeepLink(new Uri("https://google.com/sample-path"));
                // This one will NOT be captured.
                router.ProcessDeepLink(new Uri("samba://google.com/sample-path"));
                router.ProcessDeepLink(new Uri("ftp://google.com"));
                router.ProcessDeepLink(new Uri("ftp://google.com/sample-path"));
            }
        }
    }
}
