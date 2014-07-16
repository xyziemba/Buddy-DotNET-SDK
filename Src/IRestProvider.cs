using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Globalization;
using BuddySDK.BuddyServiceClient;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BuddySDK
{
	public interface IRestProvider
	{
        Task<BuddyResult<T>> Get<T>(string path, object parameters = null , bool allowThrow = false);
        Task<BuddyResult<T>> Post<T>(string path, object parameters = null, bool allowThrow = false);
        Task<BuddyResult<T>> Put<T>(string path, object parameters = null, bool allowThrow = false);
        Task<BuddyResult<T>> Patch<T>(string path, object parameters = null, bool allowThrow = false);
        Task<BuddyResult<T>> Delete<T>(string path, object parameters = null, bool allowThrow = false);
 	}
}
