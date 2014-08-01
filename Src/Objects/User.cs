using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK
{
    /// <summary>
    /// Represents the gender of a user.
    /// </summary>
    public enum UserGender
    {
		Unknown,
		Male,
        Female
    }

  

    /// <summary>
    /// Represents a public user profile. Public user profiles are usually returned when looking at an AuthenticatedUser's friends or making a search with FindUser.
    /// <example>
    /// <code>
    ///    
    ///     var loggedInUser = await Buddy.LoginUserAsync("username", "password");
    /// </code>
    /// </example>
    /// </summary>
    /// 
    [BuddyObjectPath("/users")]
    [JsonObject(MemberSerialization.OptIn)]
    public class User : BuddyBase
    {

        [JsonProperty("firstName")]
        public string FirstName
        {
            get;
            set;
        }

        [JsonProperty("lastName")]
        public string LastName
        {
            get;
            set;
        }

        [JsonProperty("userName")]
        public string Username
        {
            get;
            set;

        }

        [JsonProperty("email")]
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the gender of the user.
        /// </summary>
        [JsonProperty("gender")]
        public UserGender? Gender
        {
            get;
            set;
        }

        /// <summary>
        /// Get the celebrityMode setting of the user.
        /// </summary>
        [JsonProperty("celbMode")]
        public bool CelebrityMode
        {
            get;
            set;
        }

        [JsonProperty("dateOfBirth")]
        public DateTime? DateOfBirth
        {
            get;
            set;
        }
      
        /// <summary>
        /// Gets the age of this user.
        /// </summary>
        public int? Age
        {
            get
            {
                var dob = this.DateOfBirth;

                if (dob != null)
                {
                    return (int)(DateTime.Now.Subtract (dob.Value).TotalDays / 365.25);
                }

                return null;
            }
        }

        [JsonProperty("profilePictureID")]
        public string ProfilePictureID
        {
            get;
            set;
        }

        [JsonProperty("profilePictureUrl")]
        public string ProfilePictureUrl
        {
            get;
            private set;
        }

        private Picture profilePicture;
        public Picture ProfilePicture
        {
            get
            {
                if (ProfilePictureID != null && (profilePicture == null || profilePicture.ID != ProfilePictureID || profilePicture.SignedUrl != ProfilePictureUrl))
                {
                    ProfilePicture = new Picture(ProfilePictureID, ProfilePictureUrl);
                }

                return profilePicture;
            }
            set
            {
                profilePicture = value;
                if (value != null)
                {
                    ProfilePictureID = value.ID;
                    ProfilePictureUrl = value.SignedUrl;
                }
            }
        }

        internal User(): base()
        {
        }

        public User(string id)
            : base(id)
        {
        }
    }
}