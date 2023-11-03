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
                /// <summary>
                ///   This type lets the user determine what to
                ///   do with an incoming deep link.
                /// </summary>
                public enum DeepLinkProcessType
                {
                    /// <summary>
                    ///   Defer stands for some sort of "taking
                    ///   note" of the deep link (only THE LAST
                    ///   ONE will be kept) to be processed at
                    ///   a later time, manually.
                    /// </summary>
                    Defer,
                    /// <summary>
                    ///   Discard means two things: discard the
                    ///   current, incoming, deep link, and also
                    ///   discard whatever is deferred.
                    /// </summary>
                    Discard,
                    /// <summary>
                    ///   Immediate means processing the deep link
                    ///   as soon as it arrives. Whatever is deferred
                    ///   before, is also discarded.
                    /// </summary>
                    Immediate
                }
                
                // The router to use.
                private Router router;
                
                // The initial absolute uri.
                private string initialDeepLink;
                
                // The deferred uri. While it might be similar to the
                // initial uri, this one is actually deferred from the
                // user's choice, even for the initial deep link that
                // was stored on Awake. The user might want to trigger
                // this deep link later. New, arriving, deep links will
                // override this value, so it is a good idea to process
                // this as soon as possible, or otherwise not fear that
                // this value is lost.
                private string deferredDeepLink;

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
                    switch (GetProcessType())
                    {
                        case DeepLinkProcessType.Defer:
                            deferredDeepLink = link;
                            break;
                        case DeepLinkProcessType.Immediate:
                            deferredDeepLink = link;
                            ProcessCurrentDeepLink();
                            break;
                        default:
                            deferredDeepLink = null;
                            break;
                    }
                }

                /// <summary>
                ///   Processes the currently deferred deep link.
                ///   Intended for the user to invoke it when they
                ///   feel is the best moment.
                /// </summary>
                public void ProcessCurrentDeepLink()
                {
                    if (deferredDeepLink == null) return;
                    
                    try
                    {
                        string link = deferredDeepLink;
                        deferredDeepLink = null;
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

                /// <summary>
                ///   Gets the current processing type.
                /// </summary>
                protected virtual DeepLinkProcessType GetProcessType()
                {
                    return DeepLinkProcessType.Defer;
                }
            }
        }
    }
}
