﻿using Microsoft.AspNetCore.Http;

namespace Connector.Services
{
    public class User : Service
    {
        public User(HttpContext context) : base(context) { }

        public string Authenticate(string username, string password)
        {
            var server = global::Server.Instance;
            var user = Common.Client.Users.Authenticate(username, password, server.salt);
            if (user != null)
            {
                User.LogIn(user.userId, user.email, user.name, user.datecreated, user.username, 1, user.photo);
                User.Save(true);
                return "dashboard";
            }
            return Error();
        }

        public string CreateUserAccount(string name, string email, string username, string password)
        {
            var server = global::Server.Instance;
            Common.Client.Users.Create(name, email, username, password, server.salt, server.bcrypt_workfactor);
            if(Server.hasAdmin == false)
            {
                Server.hasAdmin = true;
            }
            return "success";
        }

        public void LogOut()
        {
            User.LogOut();
        }
    }
}