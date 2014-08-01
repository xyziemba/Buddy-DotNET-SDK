using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace BuddySDK
{
    [BuddyObjectPath("/users/lists")]
    public class UserList : BuddyBase
    {
        internal UserList()
            : base()
        {
        }

        public UserList(string id)
            : base(id)
        {

        }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get;
            set;
        }

        public enum UserListItemType
        {
            User = 0,
            UserList = 1
        }

        public class UserListItem
        {
            public string ID { get; set; }
            public UserListItemType  ItemType {get;set;}

            internal UserListItem(IDictionary<string, object> d)
            {
                ID = d["id"] as string;

                UserListItemType itemType;

                if (Enum.TryParse<UserListItemType>(d["itemType"] as string, out itemType))
                {
                    ItemType = itemType;
                }
            }
        }
    }
}