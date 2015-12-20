using Microsoft.Phone.Info;
using Microsoft.Phone.Notification;
using Nito.AsyncEx;
using System;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace BuddySDK
{
    internal partial class BuddyClient
    {
        private WindowsPhonePlatformAccess WpPlatformAccess
        {
            get
            {
                var pa = (WindowsPhonePlatformAccess)BuddySDK.PlatformAccess.Current;
                return pa;
            }
        }

        public void RecordNotificationReceived<T>(T args) 
        {
            var context = args as NavigationContext;
            string value;
            if (context != null && context.QueryString.TryGetValue(PlatformAccess.BuddyPushKey, out value))
            {
                PlatformAccess.Current.OnNotificationReceived(value);
            }
        }

        private HttpNotificationChannel _channel;

        public HttpNotificationChannel PushNotificationChannel
        {
            get
            {
                return _channel;
            }
            set
            {
                if (_channel != value)
                {
                    _channel.ChannelUriUpdated -= ChannelUriUpdated;
                    _channel = null;
                }
                _channel = value;

                if (_channel != null)
                {
                    PlatformAccess.Current.SetPushToken(_channel.ChannelUri.AbsoluteUri);
                    _channel.ChannelUriUpdated += ChannelUriUpdated;
                }
            }
        }

        void ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {
            PlatformAccess.Current.SetPushToken(e.ChannelUri.AbsoluteUri);
        }
    }

    public abstract partial class PlatformAccess
    {
        public const BuddyClientFlags DefaultFlags = BuddyClientFlags.Default;

        private static WindowsPhonePlatformAccess CreatePlatformAccess()
        {
            return new WindowsPhonePlatformAccess();
        }
    }

    internal class WindowsPhonePlatformAccess : DotNetPlatformAccessBase
    {
        public override string Platform
        {
            get { 
                return "WindowsPhone"; 
            }
        }

        private readonly AsyncLazy<string> model = new AsyncLazy<string>(() =>
        {
            return DeviceStatus.DeviceName;
        });

        public override AsyncLazy<string> Model
        {
            get
            {
                return model;
            }
        }

        public override string ApplicationID
        {
            get
            {
                var xml = XElement.Load("WMAppManifest.xml");
                var prodId = (from app in xml.Descendants("App")
                              select app.Attribute("ProductID").Value).FirstOrDefault();
                if (string.IsNullOrEmpty(prodId)) return string.Empty;
                return new Guid(prodId).ToString();
            }
        }

        public override string DeviceUniqueId
        {
            get
            {
                try
                {
                    byte[] id = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");

                    if (id != null)
                    {
                        return Convert.ToBase64String(id);
                    }
                }
                catch
                {
                    System.Diagnostics.Debug.WriteLine("You must enable the ID_CAP_IDENTITY_DEVICE capability in WMAppManifest.xml to enable retrieval of DeviceExtendedProperties' DeviceUniqueId.");
                }
                return base.DeviceUniqueId;
            }
        }

        protected override Assembly EntryAssembly
        {
            get
            {
                  return Assembly.GetCallingAssembly();
            }
        }

        private readonly AsyncLazy<string> osVersion = new AsyncLazy<string>(() =>
        {
            return Environment.OSVersion.Version.ToString();
        });

        public override AsyncLazy<string> OSVersion
        {
            get
            {
                return osVersion;
            }
        }

        public override bool IsEmulator
        {
            get
            {
                return Microsoft.Devices.Environment.DeviceType == Microsoft.Devices.DeviceType.Emulator;
            }
        }

        public override ConnectivityLevel ConnectionType
        {
            get
            {
                return ConnectivityLevel.Connected;
            }
        }

        private class WindowsPhoneIsoStore : IsolatedStorageSettings
        {
            protected override IsolatedStorageFile GetIsolatedStorageFile()
            {
                return IsolatedStorageFile.GetUserStoreForApplication();
            }

            protected override string ExecutionBinDir { get { return null; } }
        }

        private IsolatedStorageSettings _settings = new WindowsPhoneIsoStore();

        public override void ClearUserSetting(string str)
        {
            _settings.ClearUserSetting(str);
        }

        public override void SetUserSetting(string key, string value, DateTime? expires = null)
        {
            _settings.SetUserSetting(key, value, expires);
        }

        public override string GetUserSetting(string key)
        {
            return _settings.GetUserSetting(key);
        }

        public override string GetConfigSetting(string key)
        {
            var appSettings = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;

            return appSettings.Contains(key) ? appSettings[key] as string : null;
        }

        public override bool SupportsFlags(BuddyClientFlags flags)
        {
            return (flags & (BuddyClientFlags.AutoCrashReport | BuddyClientFlags.AllowReinitialize)) == flags;
        }
    }
}