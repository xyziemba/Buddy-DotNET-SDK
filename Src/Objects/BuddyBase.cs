using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BuddySDK
{
    public abstract class BuddyBase
    {
        [JsonProperty("id")]
        public string ID
        {
            get;
            private set;
        }

        [JsonProperty("location")]
        public virtual BuddyGeoLocation Location { get; set; }

        [JsonProperty("readPermissions")]
        public BuddyPermissions ReadPermissions
        {
            get;
            set;
        }

        [JsonProperty("writePermissions")]
        public BuddyPermissions WritePermissions
        {
            get;
            set;
        }

        [JsonProperty("created")]
        public DateTime Created
        {
            get;
            set;
        }

        [JsonProperty("lastModified")]
        public DateTime LastModified
        {
            get;
            set;
        }

        [JsonProperty("tag")]
        public string Tag
        {
            get;
            set;
        }

        protected BuddyBase()
        {
        }

        protected BuddyBase(string id)
        {
            ID = id;
        }
    }

    // From http://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim m_semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> m_releaser;

        public AsyncLock()
        {
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                        m_releaser :
                        wait.ContinueWith((_, state) => (IDisposable)state,
                            m_releaser.Result, CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }
    }
}
