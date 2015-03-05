using BuddySDK.BuddyServiceClient;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Picture : BinaryModelBase
    {
        public class SizeInfo {
            [Newtonsoft.Json.JsonProperty("w")]
            public int  W{get;set;}

            [Newtonsoft.Json.JsonProperty("h")]
            public int  H{get;set;}
        }

        [Newtonsoft.Json.JsonProperty("caption")]
        public string Caption
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("title")]
        public string Title
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("size")]
        public SizeInfo Size
        {
            get;
            set;
        }


       

    }
}
