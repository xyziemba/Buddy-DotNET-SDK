using Nito.AsyncEx;
using System;
using System.Reflection;
using System.Threading;

namespace BuddySDK
{
    internal abstract class DotNetPlatformAccessBase : PlatformAccess
    {
        public override string Platform
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override AsyncLazy<string> Model
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string DeviceUniqueId
        {
            get
            {
                var uniqueId = GetUserSetting("UniqueId");
                if (uniqueId == null)
                {
                    uniqueId = Guid.NewGuid().ToString();
                    SetUserSetting("UniqueId", uniqueId);
                }
                return uniqueId;
            }
        }

        public override AsyncLazy<string> OSVersion
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool IsEmulator
        {
            get
            {
                return false;
            }
        }

        public override string ApplicationID
        {
            get
            {
                return EntryAssembly.FullName;
            }
        }

        protected abstract Assembly EntryAssembly { get; }

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

        public override string GetConfigSetting(string key)
        {
            throw new NotImplementedException();
        }

        protected override void InvokeOnUiThreadCore(Action a)
        {
            var context = SynchronizationContext.Current;

            if (context != null)
            {
                context.Post((s) => { a(); }, null);
            }
            else
            {
                a();
            }
        }
    }
}