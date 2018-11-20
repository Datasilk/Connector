using System;

namespace Connector.Common.Client
{
    /// <summary>
    /// Handles functionality for user accounts, including
    /// authenticating login attempts and updating profile
    /// information
    /// </summary>
    public static class Users
    {
        #region "Encryption"
        private static string EncryptPassword(string username, string password)
        {
            var server = global::Server.Instance;
            var bCrypt = new BCrypt.Net.BCrypt();
            return BCrypt.Net.BCrypt.HashPassword(username + server.salt + password, server.bcrypt_workfactor);
        }

        private static bool DecryptPassword(string username, string rawPassword, string encryptedPassword)
        {
            var server = global::Server.Instance;
            return BCrypt.Net.BCrypt.Verify(username + server.salt + rawPassword, encryptedPassword);
        }
        #endregion

        #region "Account"
        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static int Create(string name, string email, string username, string rawPassword)
        {
            var user = new Query.Models.User()
            {
                name = name,
                email = email,
                username = username,
                password = EncryptPassword(username, rawPassword)
            };
            return Query.Users.CreateUser(user);
        }

        /// <summary>
        /// Attempts to authenticate a user based on their username
        /// and raw password input
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rawPassword"></param>
        /// <returns></returns>
        public static Query.Models.User Authenticate(string username, string rawPassword)
        {
            var encrypted = Query.Users.GetPassword(username);
            if(DecryptPassword(username, rawPassword, encrypted))
            {
                return Query.Users.AuthenticateUser(username, encrypted);
            }
            return null;
            
        }
        #endregion

        #region "Profile"

        #endregion
    }
}
