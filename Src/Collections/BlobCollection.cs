using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    public class BlobCollection : BuddyCollectionBase<Blob>
    {
        internal BlobCollection(BuddyClient client)
            : base(null, client)
        {
        }

        internal static Task<BuddyResult<Blob>> AddAsync(BuddyClient client, string friendlyName, Stream blobData, string contentType, BuddyGeoLocation location = null,
            BuddyPermissions readPermissions = BuddyPermissions.Default, BuddyPermissions writePermissions = BuddyPermissions.Default)
        {
            var c = new Blob(null, client)
            {
                FriendlyName = friendlyName,
                Location = location,
                Data = new BuddyServiceClient.BuddyFile()
                    {
                        ContentType = contentType,
                        Data = blobData,
                        Name = "data"
                    },
                ReadPermissions = readPermissions,
                WritePermissions = writePermissions
            };

            var t = c.SaveAsync();

            return t.WrapResult<bool, Blob>(r => r.IsSuccess ? c : null);
        }

        public Task<BuddyResult<Blob>> AddAsync(string friendlyName, Stream blobData, string contentType, BuddyGeoLocation location = null,
            BuddyPermissions readPermissions = BuddyPermissions.Default, BuddyPermissions writePermissions = BuddyPermissions.Default)
        {
            return BlobCollection.AddAsync(this.Client, friendlyName, blobData, contentType, location,
                readPermissions, writePermissions);
        }

        public Task<SearchResult<Blob>> FindAsync(string friendlyName = null, string contentType = null, string ownerUserId = null, BuddyGeoLocationRange locationRange = null, DateRange created = null, DateRange lastModified = null, int pageSize = 100, string pagingToken = null)
        {
            return base.FindAsync(userId: ownerUserId,
                created: created,
                lastModified: lastModified,
                locationRange: locationRange,
                pagingToken: pagingToken,
                pageSize: pageSize,
                parameterCallback: (p) =>
                {
                    p["friendlyName"] = friendlyName;
                    p["contentType"] = contentType;
                });
        }
    }
}
