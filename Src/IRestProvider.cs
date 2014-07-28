using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using BuddySDK.BuddyServiceClient;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BuddySDK
{
    public interface IBuddyClient
    {
        Task<BuddyResult<T>> CallServiceMethod<T>(string verb, string path, object parameters = null, bool allowThrow = false);

        Task<BuddyResult<T>> Get<T>(string path, object parameters = null);

        Task<BuddyResult<T>> Post<T>(string path, object parameters = null);

        Task<BuddyResult<T>> Put<T>(string path, object parameters = null);

        Task<BuddyResult<T>> Patch<T>(string path, object parameters = null);

        Task<BuddyResult<T>> Delete<T>(string path, object parameters = null);

        AuthenticatedUser User { get; }

        Task<BuddyResult<SocialAuthenticatedUser>> SocialLoginUserAsync(string identityProviderName, string identityID, string identityAccessToken);

        void SetPushToken(string token);

        BuddyGeoLocation LastLocation { get; set; }

        event EventHandler<ServiceExceptionEventArgs> ServiceException;

        event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged;

        event EventHandler<ConnectivityLevelChangedArgs> ConnectivityLevelChanged;

        Task<BuddyResult<Notification>> SendPushNotificationAsync(IEnumerable<string> recipientUserIds, string title, string message, int? counter, string payload, IDictionary<string, object> osCustomData);

        Task<BuddyResult<TimeSpan?>> RecordTimedMetricEndAsync(string timedMetricId);

        Task<BuddyResult<bool>> LogoutUserAsync();

        Task<BuddyResult<AuthenticatedUser>> LoginUserAsync(string username, string password);

        Task<BuddyResult<AuthenticatedUser>> CreateUserAsync(string username, string password, string firstName, string lastName, string email, UserGender? gender, DateTime? dateOfBirth, string tag);

        event EventHandler AuthorizationNeedsUserLogin;

        event EventHandler AuthorizationLevelChanged;

        Task<BuddyResult<bool>> AddCrashReportAsync(Exception ex, string message);

        Task<BuddyResult<string>> RecordMetricAsync(string key, IDictionary<string, object> value, TimeSpan? timeout, DateTime? timeStamp);
    }

	public interface IRestProvider
	{
        Task<BuddyResult<T>> Get<T>(string path, object parameters = null);
        Task<BuddyResult<T>> Post<T>(string path, object parameters = null);
        Task<BuddyResult<T>> Put<T>(string path, object parameters = null);
        Task<BuddyResult<T>> Patch<T>(string path, object parameters = null);
        Task<BuddyResult<T>> Delete<T>(string path, object parameters = null);
 	}
}
