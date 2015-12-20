using System;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace BuddySDK
{
    internal partial class BuddyClient
    {
        public void RecordNotificationReceived<T>(T args)
        {
            var launchArgs = args as LaunchActivatedEventArgs;

            var id = launchArgs.Arguments;

            if (!String.IsNullOrEmpty(id))
            {
                var match = Regex.Match(id, PlatformAccess.BuddyPushKey + "=(?<id>[^;]+)");
                if (match.Success)
                {
                    PlatformAccess.Current.OnNotificationReceived(match.Groups["id"].Value);
                }
            }
        }

        private PushNotificationChannel _channel;

        public PushNotificationChannel PushNotificationChannel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (_channel != value)
                {
                    _channel = null;
                }
                _channel = value;

                if (_channel != null)
                {
                    PlatformAccess.Current.SetPushToken(_channel.Uri);
                }
            }
        }
    }

    public abstract partial class PlatformAccess
    {
        public const BuddyClientFlags DefaultFlags = BuddyClientFlags.Default;

        private static PlatformAccess CreatePlatformAccess()
        {
            return new WindowsPlatformAccess();
        }
    }

    internal class WindowsPlatformAccess : WindowsPlatformBase
    {

        public override string Platform
        {
            get
            {
#if WINDOWS_PHONE_APP
                return "WindowsPhone";
#else
                return "WindowsStore";
#endif
            }
        }

        public override string DeviceUniqueId
        {
            get
            {
                var packageToken = HardwareIdentification.GetPackageSpecificToken(null);

                var dataReader = DataReader.FromBuffer(packageToken.Id);

                var bytes = new byte[packageToken.Id.Length];
                dataReader.ReadBytes(bytes);

                var id = BitConverter.ToString(bytes).Replace("-", "");

                return id;
            }
        }
    }
}
