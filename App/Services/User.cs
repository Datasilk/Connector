using Microsoft.AspNetCore.Http;

namespace Connector.Services
{
    public class User : Service
    {
        public string homePath = "dashboard"; //user home path used to redirect after user log in success

        public User(HttpContext context) : base(context) { }
        public string Authenticate(string username, string password)
        {
            var encrypted = Query.Users.GetKey(username);
            if (!DecryptPassword(username, password, encrypted)) { return Error(); }
            {
                //password verified by Bcrypt
                var user = Query.Users.AuthenticateUser(username, encrypted);
                if (user != null)
                {
                    User.LogIn(user.userId, user.username, "", user.datecreated, "", 1);
                    User.Save(true);
                    return homePath;
                }
            }
            return Error();
        }

        public string SaveUserPassword(string password)
        {
            if (Server.resetPass == true)
            {
                var update = false; //security check
                var username = "";
                var adminId = 1;
                if (Server.resetPass == true)
                {
                    //securely change admin password
                    //get admin email address from database
                    username = Query.Users.GetUserName(adminId);
                    if (username != "" && username != null) { update = true; }
                }
                if (update == true)
                {
                    Query.Users.UpdatePassword(adminId, EncryptPassword(username, password));
                    Server.resetPass = false;
                }
                return Success();
            }
            return Error();
        }

        public string CreateUserAccount(string username, string password)
        {
            if (Server.hasAdmin == false && Server.environment == Server.Environment.development)
            {
                Query.Users.CreateUser(new Query.Models.User()
                {
                    username = username,
                    password = EncryptPassword(username, password)
                });
                Server.hasAdmin = true;
                Server.resetPass = false;
                return "success";
            }
            return Error();
        }

        public void LogOut()
        {
            User.LogOut();
        }

        public string EncryptPassword(string username, string password)
        {
            var bCrypt = new BCrypt.Net.BCrypt();
            return BCrypt.Net.BCrypt.HashPassword(username + Server.salt + password, Server.bcrypt_workfactor);
        }

        public bool DecryptPassword(string username, string password, string key)
        {
            return BCrypt.Net.BCrypt.Verify(username + Server.salt + password, key);
        }
    }
}