using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace BuddySDK
{
    public class UserCollection : BuddyCollectionBase<User>
    {
        internal UserCollection(BuddyClient client)
            : base(null, client)
        {
        }

        public Task<SearchResult<User>> FindAsync(string userName = null, string email = null, string firstName = null, string lastName = null, BuddyGeoLocationRange locationRange = null, DateRange created = null, DateRange lastModified = null, int pageSize = 100, string pagingToken = null)
        {

            return base.FindAsync(userId: null,
                created: created,
                lastModified: lastModified,
                locationRange: locationRange,
                pagingToken: pagingToken,
                pageSize: pageSize,
                parameterCallback: (p) =>
                {
                    p["username"] = userName;
                    p["email"] = email;
                    p["firstName"] = firstName;
                    p["lastName"] = lastName;

                });

        }

        public Task<BuddyResult<User>> FindByIdentityAsync(string identityProviderName, string identityId)
        {
            return Task.Run<BuddyResult<User>>(() =>
            {
                var url = string.Format("{0}/identities/{1}/{2}", Path, Uri.EscapeDataString(identityProviderName), Uri.EscapeDataString(identityId));
                var r = Client.CallServiceMethod<string>("GET", url);
                return r.Result.Convert(userID => new User(userID, Client));
            });
        }
    }
}