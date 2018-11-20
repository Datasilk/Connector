using System.Collections.Generic;

namespace Query
{
    public static class Users
    {

        public static int CreateUser(Models.User user)
        {
            return Sql.ExecuteScalar<int>(
                "User_Create",
                new Dictionary<string, object>()
                {
                    {"name", user.name },
                    {"email", user.email },
                    {"username", user.username },
                    {"password", user.password }
                }
            );
        }

        public static Models.User AuthenticateUser(string username, string password)
        {
            var list = Sql.Populate<Models.User>("User_Authenticate",
                new Dictionary<string, object>()
                {
                    {"username", username },
                    {"password", password }
                }
            );
            if (list.Count > 0) { return list[0]; }
            return null;
        }

        public static void UpdatePassword(int userId, string password)
        {
            Sql.ExecuteNonQuery("User_UpdatePassword",
                new Dictionary<string, object>()
                {
                    {"userId", userId },
                    {"password", password }
                }
            );
        }

        public static Models.User GetDetails(int userId)
        {
            return Sql.ExecuteScalar<Models.User>("User_GetDetails",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
        }

        public static string GetPassword(string username)
        {
            return Sql.ExecuteScalar<string>("User_GetPassword",
                new Dictionary<string, object>()
                {
                    {"username", username }
                }
            );
        }

        public static bool Exist()
        {
            return Sql.ExecuteScalar<int>("Users_Exist") == 1;
        }
    }
}
