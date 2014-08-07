using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Album : ModelBase
	{
        public Album()
            : base()
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

        [Newtonsoft.Json.JsonProperty("items")]
        public IEnumerable<object> Items { get; set; }
       
	}
}