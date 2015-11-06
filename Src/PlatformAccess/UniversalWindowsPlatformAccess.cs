using System;
using System.Text.RegularExpressions;
using Windows.Networking.PushNotifications;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace BuddySDK
{
    internal partial class BuddyClient
    {
        public void RecordNotificationReceived<T>(T args)
        {
            var pushArgs = args as PushNotificationReceivedEventArgs;

            var id = GetPushContent(pushArgs);

            if (!String.IsNullOrEmpty(id))
            {
                var match = Regex.Match(id, PlatformAccess.BuddyPushKey + "=(?<id>[^;\">]+)");
                if (match.Success)
                {
                    PlatformAccess.Current.OnNotificationReceived(match.Groups["id"].Value);
                }
            }
        }

        private string GetPushContent(PushNotificationReceivedEventArgs pushArgs)
        {
            switch (pushArgs.NotificationType)
            {
                case PushNotificationType.Badge:
                    return pushArgs.BadgeNotification.Content.GetXml();
                case PushNotificationType.Raw:
                    return pushArgs.RawNotification.Content;
                case PushNotificationType.Tile:
                case PushNotificationType.TileFlyout:
                    return pushArgs.TileNotification.Content.GetXml();
                case PushNotificationType.Toast:
                    return pushArgs.ToastNotification.Content.GetXml();
                default:
                    return "";
            }
        }
    }

    public abstract partial class PlatformAccess
    {
        public const BuddyClientFlags DefaultFlags = BuddyClientFlags.Default;

        private static PlatformAccess CreatePlatformAccess()
        {
            return new UniversalWindowsPlatformAccess();
        }
    }

    internal class UniversalWindowsPlatformAccess : WindowsPlatformBase
    {
        public override string Platform
        {
            get
            {
                return Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            }
        }

        public override string DeviceUniqueId
        {
            get
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.System.Profile.HardwareIdentification", "GetPackageSpecificToken"))
                {
                    var packageToken = HardwareIdentification.GetPackageSpecificToken(null);

                    var dataReader = DataReader.FromBuffer(packageToken.Id);

                    var bytes = new byte[packageToken.Id.Length];
                    dataReader.ReadBytes(bytes);

                    var id = BitConverter.ToString(bytes).Replace("-", "");

                    return id;
                }
                else
                {
                    return base.DeviceUniqueId;
                }
            }
        }
    }
}