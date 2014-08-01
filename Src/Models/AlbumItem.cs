using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
   
    
    public class AlbumItem : ModelBase
	{

        public enum AlbumItemType
        {
            Picture,
            Video
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
            set;
        }
	}   
}