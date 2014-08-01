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
using BuddySDK.Models;

namespace BuddySDK
{
   

	public interface IRestProvider
	{
        Task<BuddyResult<T>> GetAsync<T>(string path, object parameters = null);
        Task<BuddyResult<T>> PostAsync<T>(string path, object parameters = null);
        Task<BuddyResult<T>> PutAsync<T>(string path, object parameters = null);
        Task<BuddyResult<T>> PatchAsync<T>(string path, object parameters = null);
        Task<BuddyResult<T>> DeleteAsync<T>(string path, object parameters = null);
 	}
}
