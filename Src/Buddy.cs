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

    public static partial class Buddy
    {
        static IDictionary<string, BuddyClient> _clients = new Dictionary<string, BuddyClient>();
        static Tuple<string, string, BuddyClientFlags, string> _creds;
        static string _currentClientKey;

        private static string GetClientKey(string appId, string appKey, BuddyClientFlags flags, string name)
        {
            return string.Format("{0};{1};{2};{3}", appId, appKey, flags, name);
        }

        internal static BuddyClient CurrentInstance
        {
            get
            {
                if (_creds == null)
                {
                    throw new InvalidOperationException("Init must be called before accessing Instance.");
                }
                if (_currentClientKey == null)
                {
                    _currentClientKey = GetClientKey(_creds.Item1, _creds.Item2, _creds.Item3, _creds.Item4);
                    if (!_clients.ContainsKey(_currentClientKey) || _clients[_currentClientKey] == null)
                    {
                        _clients[_currentClientKey] = new BuddyClient(_creds.Item1, _creds.Item2, _creds.Item3, instanceName: _creds.Item4);
                    }
                }
                return _clients[_currentClientKey];
            }
        }

        public static Task<BuddyResult<T>> Get<T>(string path, object parameters = null, bool allowThrow = false)
        {
            return CurrentInstance.Get<T>(path, parameters, allowThrow);
        }
        public static Task<BuddyResult<T>> Post<T>(string path, object parameters = null, bool allowThrow = false)
        {
            return CurrentInstance.Post<T>(path, parameters, allowThrow);
        }
        public static Task<BuddyResult<T>> Put<T>(string path, object parameters = null, bool allowThrow = false)
        {
            return CurrentInstance.Put<T>(path, parameters, allowThrow);
        }
        public static Task<BuddyResult<T>> Patch<T>(string path, object parameters = null, bool allowThrow = false)
        {
            return CurrentInstance.Patch<T>(path, parameters, allowThrow);
        }
        public static Task<BuddyResult<T>> Delete<T>(string path, object parameters = null, bool allowThrow = false)
        {
            return CurrentInstance.Delete<T>(path, parameters, allowThrow);
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
        
        public static BuddyClient Init(string appId, string appKey, BuddyClientFlags flags = PlatformAccess.DefaultFlags, string instanceName = null)
        {
            if (_creds != null && !flags.HasFlag(BuddyClientFlags.AllowReinitialize))
            {
                throw new InvalidOperationException("Already initialized.");
            }
            _creds = new Tuple<string, string, BuddyClientFlags, string>(appId, appKey, flags, instanceName);

            _currentClientKey = null;

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

        public static Task<bool> UpdateDeviceAsync(string devicePushToken = null, bool? isProduction = true)
        {
            return CurrentInstance.UpdateDeviceAsync(devicePushToken, isProduction);
        }

        public static ConnectivityLevel ConnectivityLevel { get {
            return CurrentInstance.ConnectivityLevel;
        } }

        public static Task<BuddyResult<string>> PingAsync()
        {
            return CurrentInstance.PingAsync();
        }

        public static Task<BuddyResult<bool>> RequestPasswordResetAsync(string userName, string subject, string body)
        {
            return CurrentInstance.RequestPasswordResetAsync(userName, subject, body);
        }

        public static Task<BuddyResult<bool>> ResetPasswordAsync(string userName, string resetCode, string newPassword)
        {
            return CurrentInstance.ResetPasswordAsync(userName, resetCode, newPassword);
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

        #region Collections
        public static Metadata AppMetadata
        {
            get
            {
                return CurrentInstance.AppMetadata;
            }
        }

        public static CheckinCollection Checkins
        {
            get
            {
                return CurrentInstance.Checkins;
            }
        }

        public static LocationCollection Locations
        {
            get
            {
                return CurrentInstance.Locations;
            }
        }

        public static MessageCollection Messages
        {
            get
            {
                return CurrentInstance.Messages;
            }
        }

        public static PictureCollection Pictures
        {
            get
            {
                return CurrentInstance.Pictures;
            }
        }

        public static AlbumCollection Albums
        {
            get
            {
                return CurrentInstance.Albums;
            }
        }

        public static UserCollection Users
        {
            get
            {
                return CurrentInstance.Users;
            }
        }

        public static UserListCollection UserLists
        {
            get
            {
                return CurrentInstance.UserLists;
            }
        }

        #endregion
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
