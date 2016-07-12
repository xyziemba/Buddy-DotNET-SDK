using Nito.AsyncEx;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BuddySDK
{
    internal partial class BuddyClient
    {
        public void RecordNotificationReceived<T>(T args)
        {
        }
    }

    public abstract partial class PlatformAccess
    {
        public const BuddyClientFlags DefaultFlags = BuddyClientFlags.AllowReinitialize;

        static PlatformAccess CreatePlatformAccess()
        {
            return new DotNetPlatformAccess();
        }
    }

    internal class DotNetPlatformAccess : DotNetPlatformAccessBase
    {
        public override string Platform
        {
            get { return ".NET Core"; }
        }

        private readonly AsyncLazy<string> model = new AsyncLazy<string>(() =>
        {
            return (string)null;
        });

        public override AsyncLazy<string> Model
        {
            get { return model; }
        }

        private readonly AsyncLazy<string> osVersion = new AsyncLazy<string>(() =>
        {
            return "Unavailable on .NET Core";
        });

        public override AsyncLazy<string> OSVersion
        {
            get
            {
                return osVersion;
            }
        }

        public override bool SupportsFlags(BuddyClientFlags flags)
        {
            return (flags & (BuddyClientFlags.AutoCrashReport | BuddyClientFlags.AllowReinitialize)) == flags;
        }

        public override bool IsEmulator
        {
            get
            {
                return false;
            }
        }

        protected override Assembly EntryAssembly
        {
            get { return Assembly.GetEntryAssembly(); }
        }

        public override string AppVersion
        {
            get
            {
                if (EntryAssembly != null)
                {
                    var attr = EntryAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
                    return attr.Version;
                }

                return "1.0";
            }
        }

        public override ConnectivityLevel ConnectionType
        {
            get
            {
                return ConnectivityLevel.Connected;
            }
        }

        private Dictionary<string, string> _settings = new Dictionary<string,string>();

        public override void ClearUserSetting(string str)
        {
            _settings.Remove(str);
        }

        public override void SetUserSetting(string key, string value, DateTime? expires = null)
        {
            _settings[key] = value;
        }

        public override string GetUserSetting(string key)
        {
            string setting;
            return _settings.TryGetValue(key, out setting) ? setting : null;
        }
        
        public override string GetConfigSetting(string key)
        {
            return null;
        }
    }
}
