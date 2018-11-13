using System.Collections.Generic;

namespace Query
{
    public static class Subscribers
    {

        public static int CreateSubscriber(Models.Subscriber subscriber)
        {
            return Sql.ExecuteScalar<int>(
                "Subscriber_Create",
                new Dictionary<string, object>()
                {
                    {"username", subscriber.username },
                    {"password", subscriber.password }
                }
            );
        }

        public static Models.Subscriber AuthenticateSubscriber(string username, string password)
        {
            var list = Sql.Populate<Models.Subscriber>("Subscriber_Authenticate",
                new Dictionary<string, object>()
                {
                    {"username", username },
                    {"password", password }
                }
            );
            if (list.Count > 0) { return list[0]; }
            return null;
        }

        public static void UpdatePassword(int subscriberId, string password)
        {
            Sql.ExecuteNonQuery("Subscriber_UpdatePassword",
                new Dictionary<string, object>()
                {
                    {"subscriberId", subscriberId },
                    {"password", password }
                }
            );
        }

        public static string GetUserName(int subscriberId)
        {
            return Sql.ExecuteScalar<string>("Subscriber_GetUserName",
                new Dictionary<string, object>()
                {
                    {"subscriberId", subscriberId }
                }
            );
        }

        public static string GetKey(string username)
        {
            return Sql.ExecuteScalar<string>("Subscriber_GetKey",
                new Dictionary<string, object>()
                {
                    {"username", username }
                }
            );
        }
    }
}
