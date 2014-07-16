using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    public abstract class BuddyMetadataBase
    {
        private BuddyClient _client;
        protected BuddyClient Client
        {
            get
            {
                return _client ?? Buddy.Instance;
            }
        }

        private string _metadataId;
        protected virtual string MetadataID
        {
            get
            {
                return _metadataId;
            }
            private set
            {
                _metadataId = value;
            }
        }

        protected BuddyMetadataBase(string id, BuddyClient client = null)
        {
            this._metadataId = id;
            
            this._client = client;
        }

        private void EnsureID()
        {
            if (MetadataID == null) throw new ArgumentNullException("An ID value is required to set metadata.");
        }

        private string GetMetadataPath(string key = null)
        {
            EnsureID();
            var id = MetadataID;

            var path = string.Format("/metadata/{0}", id);

            if (!string.IsNullOrEmpty(key))
            {
                path += string.Format("/{0}", key);
            }

            return path;
        }

      
        private Task<BuddyResult<bool>> SetMetadataCore(string key, object value, BuddyPermissions? visibility = null)
        {
            var callParams = new Dictionary<string, object>();
            callParams["value"] = value;
             

            if (visibility != null)
            {
                callParams["visibility"] = visibility;
            }

            return Client.Put<bool>(GetMetadataPath(key), callParams);
        }

        private Task<BuddyResult<bool>> SetMetadataCore(IDictionary<string, object> values, BuddyPermissions? visibility = null)
        {
            var callParams = new Dictionary<string, object>();
            callParams["values"] = values;

            if (visibility != null)
            {
                callParams["visibility"] = visibility;
            }

            return Client.Put<bool>(GetMetadataPath(), callParams);
        }

        public Task<BuddyResult<bool>> SetMetadataAsync(string key, object value, BuddyPermissions? visibilty = null)
        {
            return SetMetadataCore(key, value, visibilty);
        }

        public Task<BuddyResult<bool>> SetMetadataAsync(IDictionary<string, object> values, BuddyPermissions? visibility = null)
        {
            return SetMetadataCore(values, visibility);
        }

        public Task<BuddyResult<object>> GetMetadataValueAsync(string key, BuddyPermissions? visibility = null)
        {
            return Task.Run<BuddyResult<object>>(() =>
            {
                var t2 = GetMetadataItemAsync(key, visibility);

                return t2.Result.Convert<object>(i => i == null ? null : i.Value);
            });
        }

        public Task<BuddyResult<MetadataItem>> GetMetadataItemAsync(string key, BuddyPermissions? visibility = null)
        {
            var callParams = new Dictionary<string, object>();
            if (visibility != null)
            {
                callParams["visibility"] = visibility;
            }
            return Client.Get<MetadataItem>(GetMetadataPath(key), callParams);
        }

        public Task<BuddyResult<MetadataItem>> IncrementMetadataAsync(string key, double? delta = null, BuddyPermissions? visibility = null)
        {
            var path = GetMetadataPath(key) + "/increment";
            var callParams = new Dictionary<string, object>();
            callParams["delta"] = delta;
            if (visibility != null)
            {
                callParams["visibility"] = visibility;
            }

            var r = Client.Post<MetadataItem>(path,callParams);
            return r;
        }

        public Task<BuddyResult<bool>> DeleteMetadataAsync(string key, BuddyPermissions? visibility = null)
        {
            var callParams = new Dictionary<string, object>();
            if (visibility != null)
            {
                callParams["visibility"] = visibility;
            }

            var t = Client.Delete<bool>(GetMetadataPath(key), callParams);

            return t;
        }

        public Task<SearchResult<MetadataItem>> FindMetadataAsync(
            string key = null, 
            string keyPrefix = null,
            BuddyGeoLocationRange locationRange = null,
            DateRange created = null,
            DateRange lastModified = null,
            BuddyPermissions? visibility = null, string ownerUserId = null, int pageSize = 100, string pagingToken = null)
        {
            var obj = new Dictionary<string, object>(DotNetDeltas.InvariantComparer(true)){
                    {"created", created},
                    {"lastModified", lastModified},
                    {"locationRange", locationRange},
                    {"ownerID", ownerUserId},
                    {"key", key},
                    {"keyPrefix", keyPrefix}
                };

            if (visibility != null)
            {
                obj["visibility"] = visibility;
            }

            BuddyCollectionBase<BuddyBase>.InitializePaging(obj, pagingToken, pageSize);

            return Client.Get<SearchResult<MetadataItem>>(
                GetMetadataPath(), obj
            ).WrapTask<BuddyResult<SearchResult<MetadataItem>>, SearchResult<MetadataItem>>((r1) => r1.Result.Value);
        }
    }
}
