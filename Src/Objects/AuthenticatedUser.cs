using System;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using BuddySDK.BuddyServiceClient;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;


namespace BuddySDK
{
    public class AuthenticatedUser : User
    {
        /// <summary>
        /// Gets the unique user token that is the secret used to log-in this user. Each user has a unique ID, a secret user token and a user/pass combination.
        /// </summary>
        public string AccessToken
        {
            get
            {
                return GetValueOrDefault<string>("AccessToken");
            }
            protected set
            {
                SetValue<string>("AccessToken", value);
            }
        }

        internal AuthenticatedUser(string id, string accessToken)
            : base(id)
        {
            this.AccessToken = accessToken;
        }

        public override string ToString()
        {
            return base.ToString() + ", Email: " + this.Email;
        }
    }
}
