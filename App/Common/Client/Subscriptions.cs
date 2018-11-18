namespace Connector.Common.Client
{
    /// <summary>
    /// Handles functionality for subscriptions to remote users. 
    /// This includes subscribing, checking subscriptions for new 
    /// timeline content, and unsubscribing.
    /// </summary>
    public static class Subscriptions
    {

        #region "Subscribe"
        /// <summary>
        /// Subscribe to a user within the Connector network
        /// </summary>
        /// <param name="username">local username making request to subscribe</param>
        /// <param name="url">remote URL to connect to (requires HTTPS)</param>
        /// <param name="remoteUsername">remote username to subscribe to</param>
        public static void Subscribe(int username, string remoteUrl, string remoteUsername)
        {
            //connect to remote address via HTTPS

            //request to subscribe to specified username

            //retrieve secret key from request

            //save secret key in new subscription record in database
        }

        /// <summary>
        /// Check to see if subscription request was accepted by remote user
        /// </summary>
        /// <param name="username">local username making request to subscribe</param>
        /// <param name="remoteUrl">remote URL to connect to (requires HTTPS)</param>
        /// <param name="remoteUsername">remote username to subscribe to</param>
        /// <returns></returns>
        public static bool IsSubscribed(int username, string remoteUrl, string remoteUsername)
        {
            //connect to remote address via HTTPS

            //query if user is subscribed

            return false;
        }

        #endregion

        #region "Timeline"

        #endregion
    }
}
