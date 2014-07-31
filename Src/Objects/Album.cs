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
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
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