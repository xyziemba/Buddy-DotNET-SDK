using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BuddySDK
{
    public abstract class BuddyCollectionBase<T> where T: BuddyBase
    {
        private BuddyClient _client;
        private string _path;

        protected BuddyClient Client {
            get {
                return _client ?? Buddy.Instance;
            }
        }

        protected virtual string Path
        {
            get
            {
                if (_path == null)
                {
                    var attr = PlatformAccess.GetCustomAttribute<BuddyObjectPathAttribute>(typeof(T));
                    if (attr != null)
                    {
                        _path = attr.Path;
                    }
                }
                return _path;
            }
        }

        protected BuddyCollectionBase(string path = null, BuddyClient client = null)
        {
            this._path = path;
            this._client = client;
        }


        protected async Task<BuddyResult<T>> AddAsyncCore(T item)
        {
            var r = await item.SaveAsync ();
           

            return r.Convert (b => item);
        }

        protected Task<SearchResult<T>> FindAsync(
            string userId = null,
            BuddyGeoLocationRange locationRange = null, 
            DateRange created = null, 
            DateRange lastModified = null, 
            int pageSize = 100, 
            string pagingToken = null, 
            Action<IDictionary<string, object>> parameterCallback = null)
        {
           
                    IDictionary<string,object> obj = new Dictionary<string, object>(DotNetDeltas.InvariantComparer(true)){
                        {"ownerID", userId},
                        {"created", created},
                        {"lastModified", lastModified},
                        {"locationRange", locationRange}
                    };

                    if (parameterCallback != null)
                    {
                        parameterCallback(obj);
                    }

                    if (pagingToken == null)
                    {
                        obj["pagingToken"] = string.Format("{0};0", pageSize);
                    }
                    else
                    {
                        obj.Clear();
                        obj["pagingToken"] = pagingToken;
                    }

                    return Client.CallServiceMethod<SearchResult<IDictionary<string, object>>>("GET",
                                Path, obj).Result;
                            ).WrapTask<BuddyResult<SearchResult<IDictionary<string,object>>>, SearchResult<T>>(r1 =>
                            {
                                var r = r1.Result;
                                var sr = new SearchResult<T>();

                                sr.Error = r.Error;
                                sr.RequestID = r.RequestID;

                                if (r.IsSuccess)
                                {

                                    sr.NextToken = r.Value.NextToken;
                                    sr.PreviousToken = r.Value.PreviousToken;
                                    sr.CurrentToken = r.Value.CurrentToken;

                                    var items = new ObservableCollection<T>();
                                    foreach (var d in r.Value.PageResults)
                                    {
                                        var parameters = new List<object>();
                                        if (typeof(T).GetConstructor(new Type[] { typeof(string), typeof(BuddyClient) }) != null)
                                        {
                                            parameters.Add(Path);
                                        }
                                        parameters.Add(Client);
                                        T item = (T)Activator.CreateInstance(typeof(T), parameters.ToArray());
                                        item.Update(d);
                                        items.Add(item);
                                    }
                                    sr.PageResults = items;
                                }
                                return sr;
                            });
            
           
        }
    }
}