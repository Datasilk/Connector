using System.Collections.Generic;

namespace Connector.Common.Server
{
    /// <summary>
    /// Handles functionality for subscribers to a local user.
    /// This includes receiving a request to subscribe, approving &
    /// declining subscription requests, and sending encrypted
    /// timeline content to remote users who share the same private key
    /// </summary>
    public static class Subscribers
    {
        #region "Subscribe"
        /// <summary>
        /// Receive a request from a remote user to subscribe to a local user
        /// </summary>
        /// <param name="username">the user to subscribe to</param>
        /// <param name="remoteUsername">the remote user requesting a subscription</param>
        public static string Subscribe(string username, string remoteUsername)
        {
            //get userId based on username

            //generate private key
            string key = "";

            //save subscription request to database

            return key;
        }

        /// <summary>
        /// Approve a new request from a remote user to subscribe to a local user
        /// </summary>
        /// <param name="userId">user being subscribed to</param>
        /// <param name="subscriberId"></param>
        public static void Approve(int userId, int subscriberId)
        {
            //update database to set subscription status to approved
        }

        /// <summary>
        /// Decline a new request from a remote user to subscribe to a local user
        /// </summary>
        /// <param name="userId">user being subscribed to</param>
        /// <param name="subscriberId"></param>
        public static void Decline(int userId, int subscriberId)
        {
            //update database to set subscription status to declined
        }

        /// <summary>
        /// Determines whether or not a local user approved a subscription request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="remoteUsername"></param>
        /// <returns></returns>
        public static Models.Subscribers.IsSubscribed IsSubscribed(string username, string remoteUsername)
        {
            return Models.Subscribers.IsSubscribed.undecided;
        }

        /// <summary>
        /// Determines whether or not a local user approved a subscription request
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public static Models.Subscribers.IsSubscribed IsSubscribed(int subscriberId)
        {
            return Models.Subscribers.IsSubscribed.undecided;
        }
        #endregion

        #region "Subscribers"
        /// <summary>
        /// Get a list of subscribers for a particular local user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="includeNewRequests">include new requests</param>
        /// <param name="includeApproved">include approved subscribers</param>
        /// <param name="includeDeclined">include declined subscribers</param>
        /// <returns></returns>
        public static List<Query.Models.Subscriber> GetList(int userId, bool includeNewRequests = false, bool includeApproved = true, bool includeDeclined = false)
        {
            var results = new List<Query.Models.Subscriber>();

            return results;
        }

        /// <summary>
        /// Get detailed information about a subscriber
        /// </summary>
        /// <param name="userId">local user who is being subscribed to</param>
        /// <param name="username">remote username who has a subscription</param>
        /// <returns></returns>
        public static Query.Models.Subscriber GetDetails(int userId, string remoteUsername)
        {
            //get subscriberId from userId & remoteUsername
            var subscriberId = 0;
            return GetDetails(subscriberId);
        }

        /// <summary>
        /// Get detailed information about a subscriber
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public static Query.Models.Subscriber GetDetails(int subscriberId)
        {
            //get subscriber details from database
            return new Query.Models.Subscriber();
        }
        #endregion
    }
}
