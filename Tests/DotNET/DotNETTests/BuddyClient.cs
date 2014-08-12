using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using BuddySDK;
using BuddySDK.BuddyServiceClient;


namespace BuddySDK.DotNetTests
{
    //TODO write some cool network-isolated tests here
    public class BuddyClient
    {
		private class  NetworkIsolatedBuddyClient : BuddyClient {
            private readonly BuddySDK.BuddyServiceClient.IRemoteMethodProvider _remoteMethodProvider;
            public NetworkIsolatedBuddyClient(string appid, string appkey, IRemoteMethodProvider remoteMethodProvider, BuddyOptions options)
            {
                _remoteMethodProvider = remoteMethodProvider;
            }


            protected Task<IRemoteMethodProvider> GetService ()
            {
                return Task.FromResult(_remoteMethodProvider);
            }
        }

        public BuddyClient ()
        {
        }
    }
}

