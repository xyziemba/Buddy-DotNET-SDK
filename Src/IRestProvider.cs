using System.Threading.Tasks;

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
