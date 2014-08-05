using System;

namespace BuddySDK
{
    public class UserListItem
    {

        public enum UserListItemType
        {
            User = 0,
            UserList = 1
        }

        [Newtonsoft.Json.JsonProperty("id")]
        public string ID { get; set; }


        [Newtonsoft.Json.JsonProperty("itemType")]
        public UserListItemType  ItemType {
            get;set;
        }
    }
}

