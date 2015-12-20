using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class MetadataItem
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public BuddyGeoLocation Location { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
