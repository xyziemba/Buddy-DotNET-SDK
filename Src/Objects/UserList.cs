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
        internal UserList(BuddyClient client = null)
            : base(client)
        {
        }

        public UserList(string id, BuddyClient client = null)
            : base(id, client)
        {

        }

        [Newtonsoft.Json.JsonProperty("name")]
        public string Name
        {
            get
            {
                return GetValueOrDefault<string>("Name");
            }
            set
            {
                SetValue<string>("Name", value, checkIsProp: false);
            }
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

       
        public Task<BuddyResult<bool>> AddUserAsync(User user)
        {
            return Client.Put<bool>(GetObjectPath() + "/items/" + user.ID);
        }

        public Task<BuddyResult<bool>> RemoveUserAsync(User user)
        {
            return Client.Delete<bool>(GetObjectPath() + "/items/" + user.ID);
        }

        public Task<SearchResult<UserListItem>> GetUsersAsync(string pagingToken = null) {
		
            var path = GetObjectPath() + "/items";
            return Client.Get<SearchResult<IDictionary<string, object>>>(path, new
            {
                token = pagingToken
            }).WrapTask<BuddyResult<SearchResult<IDictionary<string,object>>>,SearchResult<UserListItem>>(t2 => {


                var r = t2.Result;

                var sr = new SearchResult<UserListItem>();

                if (r.IsSuccess) {
                    sr.NextToken = r.Value.NextToken;
                    sr.PreviousToken = r.Value.PreviousToken;
                    sr.CurrentToken = r.Value.CurrentToken;
                        
                    sr.PageResults = r.Value.PageResults.Select(i => new UserListItem(i));
                }
                else {
                    sr.Error = r.Error;
                    
                }
                return sr;
            });
        }
    }
}