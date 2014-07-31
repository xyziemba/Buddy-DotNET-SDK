using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK
{
    [BuddyObjectPath("/albums")]
	public class Album : BuddyBase
	{
        internal Album()
            : base()
        {
        }

		public Album(string id)
			: base(id)
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
	}
}