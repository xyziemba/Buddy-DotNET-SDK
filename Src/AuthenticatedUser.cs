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

        internal AuthenticatedUser(string id, string accessToken, BuddyClient client)
            : base(id, client)
        {
            this.AccessToken = accessToken;
        }

        public override string ToString()
        {
            return base.ToString() + ", Email: " + this.Email;
        }


        public Task<BuddyResult<bool>> AddIdentityAsync(string identityProviderName, string identityID)
        {
            return AddRemoveIdentityCoreAsync(() => Client.Post<string>("/users/me/identities/" + Uri.EscapeDataString(identityProviderName), new
            {
                IdentityID = identityID
            }));
        }

        public Task<BuddyResult<bool>> RemoveIdentityAsync(string identityProviderName, string identityID)
        {
            return AddRemoveIdentityCoreAsync(() => Client.Delete<string>("/users/me/identities/" + Uri.EscapeDataString(identityProviderName), new { IdentityID = identityID }));
        }

        private Task<BuddyResult<bool>> AddRemoveIdentityCoreAsync(Func<Task<BuddyResult<string>>> serviceMethod)
        {
            return serviceMethod().WrapResult<string, bool>((r1) => r1.IsSuccess);
        }

        public Task<BuddyResult<IEnumerable<string>>> GetIdentitiesAsync(string identityProviderName = null)
        {
            return Task.Run<BuddyResult<IEnumerable<string>>>(async () =>
            {
                var encodedIdentityProviderName = string.IsNullOrEmpty(identityProviderName) ? "" : Uri.EscapeDataString(identityProviderName);

                var r = await Client.Get<IEnumerable<Newtonsoft.Json.Linq.JObject>>("/users/me/identities/" + encodedIdentityProviderName);

                return r.Convert<IEnumerable<string>>(jObjects => jObjects.Select(jObject => jObject.Value<string>("identityProviderID")));
            });
        }
    }
}
