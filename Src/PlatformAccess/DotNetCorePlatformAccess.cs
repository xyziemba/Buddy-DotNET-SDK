﻿using Nito.AsyncEx;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
            var osVersionProperty = typeof(Environment).GetRuntimeProperty("OSVersion");
            object osVersion = osVersionProperty.GetValue(null, null);
            var versionStringProperty = osVersion.GetType().GetRuntimeProperty("VersionString");
            var versionString = (string)versionStringProperty.GetValue(osVersion, null);
            return versionString;
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

        
        private class DotNetIsoStore : IsolatedStorageSettings
        {
            protected override IsolatedStorageFile GetIsolatedStorageFile()
            {
                return IsolatedStorageFile.GetUserStoreForApplication();
            }

            protected override string ExecutionBinDir
            {
                get { return null; }
            }
        }

        private IsolatedStorageSettings _settings = new DotNetIsoStore();
        

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
        
    }
}
