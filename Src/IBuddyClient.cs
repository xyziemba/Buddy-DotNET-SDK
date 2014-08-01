using System;
using System.Threading.Tasks;
using BuddySDK.Models;
using System.Collections.Generic;

namespace BuddySDK
{
    public interface IBuddyClient : IRestProvider
    {
      
        Task<User> GetCurrentUserAsync (bool reload = false);

        Task<BuddyResult<SocialNetworkUser>> SocialLoginUserAsync(string identityProviderName, string identityID, string identityAccessToken);

        void SetPushToken(string token);

        BuddyGeoLocation LastLocation { get; set; }

        event EventHandler<ServiceExceptionEventArgs> ServiceException;

        event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

        void RecordNotificationReceived<T>(T args);

        Task<BuddyResult<bool>> LogoutUserAsync();

        Task<BuddyResult<User>> LoginUserAsync(string username, string password);

        Task<BuddyResult<User>> CreateUserAsync(string username, string password, string firstName, string lastName, string email, User.UserGender? gender, DateTime? dateOfBirth, string tag);

        event EventHandler AuthorizationNeedsUserLogin;

        event EventHandler AuthorizationLevelChanged;

        Task<BuddyResult<BuddySDK.BuddyClient.Metric>> RecordMetricAsync(string key, IDictionary<string, object> value, TimeSpan? timeout, DateTime? timeStamp);
    }
}

