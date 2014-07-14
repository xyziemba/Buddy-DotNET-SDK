using BuddySDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BuddySDK.BuddyServiceClient
{
	public interface IRemoteMethodProvider
	{
        Task<BuddyCallResult<T>> CallMethodAsync<T> (string verb, string path, object parameters);
        void CallMethodAsync<T> (string verb, string path, object parameters, Action<BuddyCallResult<T>> callback);
	}
}
