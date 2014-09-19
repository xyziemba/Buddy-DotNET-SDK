using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK.Models
{

  

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
    [JsonObject(MemberSerialization.OptIn)]
    public class User : ModelBase
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
            set;
        }
    }
}