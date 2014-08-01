using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.Models
{
    public class Location : ModelBase
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("description")]
        public string Description
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("address1")]
        public string Address1
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("address2")]
        public string Address2
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("city")]
        public string City
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("region")]
        public string Region
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("country")]
        public string Country
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("postalCode")]
        public string PostalCode
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("fax")]
        public string FaxNumber
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("phone")]
        public string PhoneNumber
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("website")]
        public Uri Website
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("category")]
        public string Category
        {
            get;
            set;
        }

        [Newtonsoft.Json.JsonProperty("distanceFromSearch")]
        public double Distance
        {
            get;
            set;
        }

    }
}
