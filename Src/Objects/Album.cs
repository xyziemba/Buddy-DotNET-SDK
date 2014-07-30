using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK
{
    [BuddyObjectPath("/albums")]
	public class Album : BuddyBase
	{
        internal Album(BuddyClient client = null)
            : base(client)
        {
        }

		public Album(string id, BuddyClient client= null)
			: base(id, client)
		{
		}

		[Newtonsoft.Json.JsonProperty("caption")]
		public string Caption
		{
			get
			{
                return GetValueOrDefault<string>("Caption");
			}
			set
			{
                SetValue<string>("Caption", value, checkIsProp: false);
			}
		}

		[Newtonsoft.Json.JsonProperty("name")]
		public string Name
		{
			get
			{
				return GetValueOrDefault<string>("Name");
			}
			set
			{
				SetValue<string>("Name", value, checkIsProp: false);
			}
		}

		private IEnumerable<object> _items;
        public IEnumerable<object> Items
        {
            get
            {
                return _items;
            }
        }

        public async Task<BuddyResult<AlbumItem>> AddItemAsync(string itemId, string caption, BuddyGeoLocation location, string tag = null,
            BuddyPermissions readPermissions = BuddyPermissions.Default, BuddyPermissions writePermissions = BuddyPermissions.Default)
		{
			var c = new AlbumItem(this.GetObjectPath() + PlatformAccess.GetCustomAttribute<BuddyObjectPathAttribute>(typeof(AlbumItem)).Path, this.Client)
			{
				ItemId = itemId,
                Caption = caption,
				Location = location,
				Tag = tag,
                ReadPermissions = readPermissions,
                WritePermissions = writePermissions
			};

            var r = await c.SaveAsync();
					
            return r.Convert<AlbumItem>(b => c);
		}
	}
}