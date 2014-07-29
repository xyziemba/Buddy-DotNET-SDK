using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK
{
    [Flags]
    public enum BuddyClientFlags
    {
        None              =  0,
        AutoCrashReport =    0x00000002,
        AllowReinitialize =  0x00000004,
        Default = AutoCrashReport,
    }

    public class BuddyCreds
    {
        public string AppID { get; set; }
        public string AppKey { get; set; }
        public BuddyOptions Options { get; set; }

        public BuddyCreds(string appId, string appKey, BuddyOptions options)
        {
            AppID = appId;
            AppKey = appKey;
            Options = options;
        }
    }

    public static partial class Buddy
    {
        static IDictionary<string, BuddyClient> _clients = new Dictionary<string, BuddyClient>();
        static string _currentClientKey;

        private static string GetClientKey(BuddyCreds creds)
        {
            return string.Format("{0};{1};{2};{3}", creds.AppID, creds.AppKey, creds.Options.Flags, creds.Options.InstanceName);
        }

        internal static BuddyClient CurrentInstance
        {
            get
            {
                if (_currentClientKey == null || _clients[_currentClientKey] == null)
                {
                    throw new InvalidOperationException("Init must be called before accessing Instance.");
                }

                return _clients[_currentClientKey];
            }
            set
            {
                var clientKey = GetClientKey(new BuddyCreds(value.AppId, value.AppKey, value.Options));
                _currentClientKey = clientKey;

                _clients[clientKey] = value;
            }
        }

        public static Task<BuddyResult<T>> Get<T>(string path, object parameters = null)
        {
            return CurrentInstance.Get<T>(path, parameters);
        }
        public static Task<BuddyResult<T>> Post<T>(string path, object parameters = null)
        {
            return CurrentInstance.Post<T>(path, parameters);
        }
        public static Task<BuddyResult<T>> Put<T>(string path, object parameters = null)
        {
            return CurrentInstance.Put<T>(path, parameters);
        }
        public static Task<BuddyResult<T>> Patch<T>(string path, object parameters = null)
        {
            return CurrentInstance.Patch<T>(path, parameters);
        }
        public static Task<BuddyResult<T>> Delete<T>(string path, object parameters = null)
        {
            return CurrentInstance.Delete<T>(path, parameters);
        }

        [Obsolete("Call Buddy.Instance.[Get/Post/Put/Patch/Delete] instead")]
        public static Task<BuddyResult<IDictionary<string, object>>> CallServiceMethod(string verb, string path, object parameters = null)
        {
            return CurrentInstance.CallServiceMethod<IDictionary<string, object>>(verb, path, parameters);
        }

        [Obsolete("Call Buddy.Instance.[Get/Post/Put/Patch/Delete] instead")]
        public static Task<BuddyResult<T>> CallServiceMethod<T>(string verb, string path, object parameters = null)
        {
            return CurrentInstance.CallServiceMethod<T>(verb, path, parameters);
        }

        public static AuthenticatedUser CurrentUser
        {
            get
            {
                return CurrentInstance.User;
            }
        }

        public static void RunOnUiThread(Action a) {
            PlatformAccess.Current.InvokeOnUiThread (a);
        }

        #region Global Events
        public static event EventHandler AuthorizationLevelChanged {
            add {
                CurrentInstance.AuthorizationLevelChanged += value;
            }
            remove {
                CurrentInstance.AuthorizationLevelChanged -= value;
            }
        }

        public static event EventHandler AuthorizationNeedsUserLogin {
            add {
                CurrentInstance.AuthorizationNeedsUserLogin += value;
            }
            remove {
                CurrentInstance.AuthorizationNeedsUserLogin -= value;
            }
        }

        public static event EventHandler<ConnectivityLevelChangedArgs> ConnectivityLevelChanged {
            add {
                CurrentInstance.ConnectivityLevelChanged += value;
            }
            remove {
                CurrentInstance.ConnectivityLevelChanged -= value;
            }
        }

        public static event EventHandler<CurrentUserChangedEventArgs> CurrentUserChanged {
            add {
                CurrentInstance.CurrentUserChanged += value;
            }
            remove {
                CurrentInstance.CurrentUserChanged -= value;
            }
        }

        public static event EventHandler<ServiceExceptionEventArgs> ServiceException {
            add {
                CurrentInstance.ServiceException += value;
            }
            remove {
                CurrentInstance.ServiceException -= value;
            }
        }

        #endregion

        [Obsolete("Please use the BuddyOptions version of Init")]
        public static IBuddyClient Init(string appId, string appKey, BuddyClientFlags flags = PlatformAccess.DefaultFlags, 
            string instanceName = null, string appVersion = null)
        {
            var options = new BuddyOptions(flags,instanceName, appVersion);
            return Init(appId, appKey, options);
        }

        public static IBuddyClient Init(string appId, string appKey, BuddyOptions options = null)
        {
            if (options == null)
            {
                options = new BuddyOptions();
            }
            if (_currentClientKey != null && !options.Flags.HasFlag(BuddyClientFlags.AllowReinitialize))
            {
                throw new InvalidOperationException("Already initialized.");
            }

            CurrentInstance = new BuddyClient(appId, appKey, options);

            return CurrentInstance;
        }

        public static Task<BuddyResult<AuthenticatedUser>> CreateUserAsync(string username, string password, string firstName = null, string lastName = null, string email = null, UserGender? gender = null, DateTime? dateOfBirth = null, string tag = null) {
            return CurrentInstance.CreateUserAsync (username, password, firstName, lastName, email, gender, dateOfBirth, tag : tag);
        }

        public static Task<BuddyResult<AuthenticatedUser>> LoginUserAsync(string username, string password)
        {
            var t = CurrentInstance.LoginUserAsync(username, password);

            return t;
        }

        public static Task<BuddyResult<bool>> LogoutUserAsync ()
        {
            return CurrentInstance.LogoutUserAsync ();
        }

        public static Task<BuddyResult<SocialAuthenticatedUser>> SocialLoginUserAsync(string identityProviderName, string identityID, string identityAccessToken)
        {
            var t = CurrentInstance.SocialLoginUserAsync(identityProviderName, identityID, identityAccessToken);

            return t;
        }

        // 
        // Push Notifications
        //
        public  static Task<BuddyResult<Notification>> SendPushNotificationAsync(
            IEnumerable<string> recipientUserIds, 
            string title = null, string message = null, 
            int? counter = null, string payload = null, 
            IDictionary<string,object> osCustomData = null)
        {
            return CurrentInstance.SendPushNotificationAsync (
                recipientUserIds,
                title,
                message,
                counter,
                payload,
                osCustomData);
        }

        public static void SetPushToken(string token) {
            CurrentInstance.SetPushToken (token);
        }

        // 
        // Metrics
        //
        public static Task<BuddyResult<string>> RecordMetricAsync(string key, IDictionary<string, object> value = null, TimeSpan? timeout = null, DateTime? timeStamp = null)
        {
            return CurrentInstance.RecordMetricAsync(key, value, timeout, timeStamp);
        }

        public static Task<BuddyResult<TimeSpan?>> RecordTimedMetricEndAsync(string timedMetricId) {
            return CurrentInstance.RecordTimedMetricEndAsync (timedMetricId);
        }

        public static Task AddCrashReportAsync (Exception ex, string message = null)
        {
            return CurrentInstance.AddCrashReportAsync (ex, message);
        }

        public static BuddyGeoLocation LastLocation
        {
            get
            {
                return CurrentInstance.LastLocation;
            }
            
            set
            {
                CurrentInstance.LastLocation = value;
            }
        }
    }
}
