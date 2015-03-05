using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuddySDK.BuddyServiceClient;
using Newtonsoft.Json;
using System.IO;

namespace BuddySDK.Models
{
    public class Blob : BinaryModelBase
    {
        [Newtonsoft.Json.JsonProperty("friendlyName")]
        public string FriendlyName
        {
            get;
            set;
        }


      

    }
}
