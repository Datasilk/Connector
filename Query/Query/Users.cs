﻿using System.Collections.Generic;

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

        public static string GetUserName(int userId)
        {
            return Sql.ExecuteScalar<string>("User_GetUserName",
                new Dictionary<string, object>()
                {
                    {"userId", userId }
                }
            );
        }

        public static string GetKey(string username)
        {
            return Sql.ExecuteScalar<string>("User_GetKey",
                new Dictionary<string, object>()
                {
                    {"username", username }
                }
            );
        }
    }
}
