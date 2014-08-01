using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK
{
    public enum AlbumItemType
    {
        Picture,
        Video
    }
    
    [BuddyObjectPath("/items")]
	public class AlbumItem : BuddyBase
	{
        internal AlbumItem()
            : base()
        {
        }

        [Newtonsoft.Json.JsonProperty("caption")]
        public string Caption
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("itemId")]
        public string ItemId
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("itemType")]
        public AlbumItemType ItemType
        {
            get;
            private set;
        }
	}   
}