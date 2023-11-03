using System;
using UnityEngine;

namespace AlephVault.Unity.DeepLinks
{
    namespace Authoring
    {
        namespace Behaviours
        {
            using Types.Routing;

            /// <summary>
            ///   Processes the deep link when the app just starts
            ///   or when the app is running and receives a deep
            ///   link request externally.
            /// </summary>
            public abstract class DeepLinkAware : MonoBehaviour
            {
                // The router to use.
                private Router router;
                
                // The initial absolute uri.
                private string initialDeepLink;
                
                private void Awake()
                {
                    router = MakeRouter();
                    
                    // Deep links received while the app is running
                    // will be handled by ProcessDeepLink.
                    Application.deepLinkActivated += ProcessDeepLink;
                    
                    // Also the initial deep link used to launch the
                    // application will be handled by the same callback.
                    // It will be deferred to the Start event.
                    if (!string.IsNullOrWhiteSpace(Application.absoluteURL))
                    {
                        initialDeepLink = Application.absoluteURL;
                    }
                }

                private void Start()
                {
                    // On start, if there's any deferred event, it will
                    // be properly routed. The router callbacks are able
                    // to handle it immediately or somehow defer it even
                    // more (this is implementation-specific to the logic
                    // in the router).
                    if (initialDeepLink != null)
                    {
                        ProcessDeepLink(Application.absoluteURL);
                    }
                }

                // Processes the incoming deep link by using the
                // router. This is useful only for mobile devices.
                // It will have no effect on desktop apps. Also,
                // each mobile device platform will require their
                // own steps to configure the allowed deep links
                // properly.
                private void ProcessDeepLink(string link)
                {
                    try
                    {
                        router.ProcessDeepLink(new Uri(link));
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }

                /// <summary>
                ///   Makes and completely configured the internal
                ///   router that will process the deep link.
                /// </summary>
                /// <returns>The router</returns>
                protected abstract Router MakeRouter();
            }
        }
    }
}
