using System;
using System.Globalization;
using System.Threading.Tasks;

namespace BuddySDK
{
    public class UserCollection : BuddyCollectionBase<User>
    {
        internal UserCollection(BuddyClient client)
            : base(null, client)
        {
        }

        public Task<SearchResult<User>> FindAsync(string userName = null, string email = null, string firstName = null, string lastName = null,
            BuddyGeoLocationRange locationRange = null, DateRange created = null, DateRange lastModified = null, UserGender? gender = null,
            DateRange dateOfBirthRange = null, string userListId = null, int pageSize = 100, string pagingToken = null)
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
                    p["gender"] = gender;
                    p["dobRange"] = dateOfBirthRange;
                    p["userListId"] = userListId;
                });
        }

        public Task<BuddyResult<User>> FindByIdentityAsync(string identityProviderName, string identityId)
        {
            var url = string.Format(CultureInfo.InvariantCulture, "{0}/identities/{1}/{2}", Path, Uri.EscapeDataString(identityProviderName), Uri.EscapeDataString(identityId));
            return Client.Get<string>(url).WrapResult<string, User>(r => new User(r.Value, Client));
        }
    }
}