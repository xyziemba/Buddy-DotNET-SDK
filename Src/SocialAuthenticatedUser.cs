using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    public class SocialAuthenticatedUser : AuthenticatedUser
    {
        public bool IsNew { get; protected set; }

        internal SocialAuthenticatedUser(string id, string accessToken, bool isNew)
            : base(id, accessToken)
        {
            IsNew = isNew;
        }
    }
}
