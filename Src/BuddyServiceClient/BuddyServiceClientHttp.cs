using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BuddySDK.BuddyServiceClient
{
     internal class BuddyServiceClientHttp :BuddyServiceClientBase
     {
         public bool LoggingEnabled { get; set; }

         protected override string ClientName
         {
			get{
        		return "DotNet";
			}
         }

         private string AppID;
         private string SharedSecret;
         private string sdkVersion;
         protected override string ClientVersion
         {
             get { return  sdkVersion; }
         }
        private string SdkVersion {
            get {
                return String.Format(CultureInfo.InvariantCulture, "{0};{1}", ClientName, ClientVersion); 
            }
        }

        internal enum HttpRequestType
        {
            HttpGet,
            HttpPostJson,
            HttpPostMultipartForm
        }

      
        public override bool IsLocal
        {
            get
            {
                return ServiceRoot.Contains("localhost");
            }
        }

        private string MakeStringToSign(string verb, string path)
        {
            if (path == null || verb == null)
            {
                return null;
            }

            string fullPath = path;
            if (!fullPath.StartsWith("/"))
            {
                fullPath = "/" + fullPath;
            }

            return string.Format("{0}\n{1}\n{2}", verb.ToUpper(), AppID, fullPath);
        }

        private string GenerateSignatureForRequest(string verb, string path)
        {
            string stringToSign = MakeStringToSign(verb, path);
            if(stringToSign==null)
            {
                return null;
            }

            return PlatformAccess.Current.SignString(SharedSecret,stringToSign);
        }

        public BuddyServiceClientHttp(string root,string appID,string sharedSecret)
        {
            SharedSecret = sharedSecret;
            AppID = appID;

            if (String.IsNullOrEmpty(root)) throw new ArgumentNullException("root");
            if (root.EndsWith("/"))
            {
                root = root.Substring(0, root.Length - 1);
            }
            ServiceRoot = root;

#if WINDOWS_PHONE
             var  versionAttrs = Assembly.GetExecutingAssembly().GetCustomAttributes(false);
#else
            var versionAttrs = typeof(BuddyServiceClientHttp).GetTypeInfo().Assembly.GetCustomAttributes();
#endif
            var attr = versionAttrs.OfType<AssemblyFileVersionAttribute>().First();

            var sdkVersion = "Version=" + attr.Version;

            this.sdkVersion = sdkVersion;
            LoggingEnabled = true;
        }

        public virtual void LogRequest(string method, string url, string body)
        {
            if (LoggingEnabled)
            {
                Debug.WriteLine("{0}: {1}", method, url);
                if (body != null)
                {
                    Debug.WriteLine(body);
                }
            }
        }

        public virtual void LogResponse(string method, string body, TimeSpan time, HttpWebResponse response = null)
        {
            if (LoggingEnabled){
                Debug.WriteLine("{1}: {0} ({2:0.0}ms)", response == null ? "(null)" : response.StatusCode.ToString(), method, time.TotalMilliseconds);
                if (body != null)
                {
                    Debug.WriteLine(body);
                 
                }
            }
        }

        public virtual void LogMessage(string message) {
            if (LoggingEnabled){
                Debug.WriteLine(message);
               
            }
        }

        private void StartRequest() {

             BuddySDK.PlatformAccess.Current.ShowActivity = true;
        }

        private void EndRequest() {

            BuddySDK.PlatformAccess.Current.ShowActivity = false;
        }

        protected override Task<BuddyCallResult<T>> CallMethodAsyncCore<T>(
             string verb,
             string path,
             object parameters,
             bool skipAuth)
        {
            // TODO: Refactor to use async/await instead of
            // TCS/callbacks.
            var tcs = new TaskCompletionSource<BuddyCallResult<T>>();

            DateTime start = DateTime.Now;

            StartRequest();

            Action<Exception, BuddyCallResult<T>> finishMethodCall = (ex, bcr) =>
            {
                EndRequest();
                if (ex == null)
                {
                    tcs.TrySetResult(bcr);
                    return;
                }

                WebException webEx = ex as WebException;
                HttpWebResponse response = null;
                var err = "UnknownServiceError";

                bcr.Message = ex.ToString();
                if (webEx != null)
                {
                    err = "InternetConnectionError";

                    if (webEx.Response != null)
                    {
                        response = (HttpWebResponse)webEx.Response;
                        bcr.Message = response.StatusDescription;

                        bcr.StatusCode = (int)response.StatusCode;
                        if (bcr.StatusCode >= 400)
                        {
                            err = "UnknownServiceError";
                        }
                    }
                    else
                    {
                        bcr.Message = webEx.Status.ToString();
                    }

                }

                bcr.Error = err;
                LogResponse(verb + " " + path, bcr.Message, DateTime.Now.Subtract(start), response);


                OnServiceException(ex);
                tcs.TrySetResult(bcr);
            };

            var d = ParametersToDictionary(parameters);
            MakeRequest<T>(verb, path, d, async (ex, response) =>
            {
                var bcr = new BuddyCallResult<T>();

                var isFile = typeof(BuddyFile).Equals(typeof(T));


                if (response == null && ex != null && ex is WebException)
                {
                    response = (HttpWebResponse)((WebException)ex).Response;
                }

                if ((response == null) && ex != null)
                {
                    finishMethodCall(ex, bcr);
                    return;
                }
                else if (response != null)
                {
                    bcr.StatusCode = (int)response.StatusCode;
                    if (!isFile || (bcr.StatusCode >= 400 && response.ContentType.Contains("application/json")))
                    {
                        string body = null;
                        try
                        {
                            using (var responseStream = response.GetResponseStream())
                            {
                                body = await new StreamReader(responseStream).ReadToEndAsync();
                            }
                        }
                        catch (Exception rex)
                        {
                            finishMethodCall(rex, bcr);
                            return;
                        }

                        LogResponse(MethodName(verb, path), body, DateTime.Now.Subtract(start), response);

                        //json parse
                        try
                        {
                            var envelope = JsonConvert.DeserializeObject<JsonEnvelope<T>>(body, new JsonSerializerSettings
                            {
                                DateTimeZoneHandling = DateTimeZoneHandling.Local
                            });

                            if (envelope == null)
                            {
                                // fall through
                            }
                            else if (envelope.error != null)
                            {
                                bcr.Error = envelope.error;
                                bcr.ErrorNumber = envelope.errorNumber;
                                bcr.Message = envelope.message;
                            }
                            else
                            {
                                // special case dictionary.
                                if (typeof(IDictionary<string, object>).IsAssignableFrom(typeof(T)))
                                {
                                    object obj = envelope.result;
                                    IDictionary<string, object> d2 = (IDictionary<string, object>)obj;
                                    obj = (obj == null) ? new Dictionary<string, object>(DotNetDeltas.InvariantComparer(true)) : new Dictionary<string, object>(d2, DotNetDeltas.InvariantComparer(true));
                                    envelope.result = (T)obj;
                                }
                                bcr.Result = envelope.result;
                            }

                            if (envelope != null)
                            {
                                bcr.RequestID = envelope.request_id;
                            }
                        }
                        catch (Exception pex)
                        {
                            LogMessage(pex.ToString());
                            bcr.Error = "BadJsonResponse";
                            bcr.Message = "Couldn't parse JSON: \r\n" + body;
                        }
                    }
                    else
                    {


                        bcr = new BuddyCallResult<T>();

                        if (bcr.StatusCode < 400)
                        {
                            var file = new BuddyFile(response.GetResponseStream(), null, response.ContentType);
                            bcr.Result = (T)(object)file;
                        }
                        bcr.StatusCode = (int)response.StatusCode;
                    }


                    try
                    {
                        finishMethodCall(null, bcr);
                    }
                    catch (Exception ex3)
                    {
                        finishMethodCall(ex3, bcr);
                    }

                }

            }, skipAuth);

            return tcs.Task;
        }

        private const int EncodeChunk = 32000;

        private static string EscapeDataString(string value)
        {
            StringBuilder encoded = new StringBuilder(value.Length);

            var pos = 0;

            // encode the string in 32K chunks.
            while (pos < value.Length)
            {
                var len = Math.Min(EncodeChunk, value.Length - pos);
                var encodedPart = value.Substring(pos, len);
                encodedPart = Uri.EscapeDataString(encodedPart);
                encoded.Append(encodedPart);
                pos += EncodeChunk;
            }
            return encoded.ToString();
        }


        private string GetUrlEncodedParameters(IDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null) continue;

                string val = null;
                if (kvp.Value is BuddyFile)
                {
                    val = Convert.ToBase64String(((BuddyFile)kvp.Value).Bytes);
                }
                else
                {
                    val = GetGlobalizedString(kvp.Value);
                }
                val = EscapeDataString(val);

                sb.AppendFormat(CultureInfo.InvariantCulture, "{2}{0}={1}", kvp.Key, val, isFirst ? "" : "&");
                isFirst = false;
            }
            return sb.ToString();
        }

        private const int TimeoutMilliseconds = 30000;
        private static string MethodName(string verb, string path)
        {
            return verb + " " + path;
        }

        private async void MakeRequest<T>(string verb, string path, IDictionary<string, object> parameters, Action<Exception, HttpWebResponse> callback, bool skipAuth)
        {
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            // get the token before generating the request url, as there may be a new ServiceRoot
            var token = skipAuth ? null : await Client.GetAccessToken();

            var url = String.Format(CultureInfo.InvariantCulture, "{0}{1}", ServiceRoot, path);
            var requestType = HttpRequestType.HttpPostJson;
            IEnumerable<KeyValuePair<string, object>> files = null;
         
            switch (verb.ToUpperInvariant())
            {
            case "GET":
                // For redirects (specifically Azure Blob), if our authentication header is in there
                // it'll deny us access.  So for that case, we just need to add the access token to the parameters
                // collection so it doesn't get added as a header.
                //
                if (typeof(T) == typeof(BuddyFile) && !parameters.ContainsKey("accessToken"))
                {
                    parameters["accessToken"] = token;
                    token = null;
                }
                url += "?" + GetUrlEncodedParameters(parameters);
                requestType = HttpRequestType.HttpGet;
                break;

            default:
                // do we have any file parameters.
                //
                files = from p in parameters where p.Value is BuddyFile select p;

                if (files.Any())
                {
                    // remove the files from the main array.
                    requestType = HttpRequestType.HttpPostMultipartForm;
                    var newParameters = new Dictionary<string, object>();
                    foreach (var fileKvp in files)
                    {
                        newParameters.Add(fileKvp.Key, fileKvp.Value);
                    }

                    foreach (var kvp in newParameters)
                    {
                        parameters.Remove(kvp.Key);
                    }

                    // get json for the remainder and make it into a file
                    var json = JsonConvert.SerializeObject(parameters, Formatting.None);

                    var jsonFile = new BuddyFile(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)), "body", "application/json");
                  
                    newParameters.Add(jsonFile.Name, jsonFile);
                    parameters = newParameters;
                }

                break;
            }


            HttpWebRequest wr = null;

            try
            {
                wr = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (Exception ex)
            {
                callback(ex, null);
                return;
            }

            wr.Headers["BuddyPlatformSDK"] = SdkVersion;

            if (token != null && (parameters == null || !parameters.ContainsKey("accessToken")))
            {
                if (SharedSecret != null)
                {
                    string requestSig = GenerateSignatureForRequest(verb, path);
                    if (requestSig != null)
                    {
                        wr.Headers["Authorization"] = String.Format("Buddy {0} {1}", token, requestSig);
                    }
                }
                else
                {
                    wr.Headers["Authorization"] = String.Format("Buddy {0}", token);
                }
            }

            wr.Method = verb;

            Action getResponse = () =>
            {
                try
                {
                    int requestStatus = -1;
                    LogRequest(MethodName(verb,path) , url, null);
                    wr.BeginGetResponse((async2) =>
                    {   
                        try
                        {
                            lock (wr)
                            {
                                if (requestStatus == 1)
                                {
                                    throw new WebException("Request timed out.", WebExceptionStatus.RequestCanceled);
                                }
                                else
                                {
                                    HttpWebResponse response = (HttpWebResponse)wr.EndGetResponse(async2);
                                    callback(null, response);
                                }
                            }
                        }
                        catch (WebException ex)
                        {
                           callback(ex, null);
                        }
                    }, null);

                    // spin up a timer to check for timeout.  not all platforms
                    // support proper threadpool wait.
                    //
                    Action timeoutHandler = () =>
                    {
                        lock (wr)
                        {
                            if (requestStatus == -1)
                            {
                                requestStatus = 1;
                                wr.Abort();
                            }
                        }
                    };
#if WINDOWS_APP_MODEL
                    TimeSpan delay = TimeSpan.FromMilliseconds(TimeoutMilliseconds);

                    Windows.System.Threading.ThreadPoolTimer.CreateTimer(
                        (source) =>
                        {
                            timeoutHandler();
                        }, delay);
         
#else
                    new System.Threading.Timer((state) =>
                    {
                       timeoutHandler();
                    }, null, TimeoutMilliseconds, System.Threading.Timeout.Infinite);
#endif

                }
                catch (WebException wex)
                {
                    LogResponse(MethodName(verb, path), wex.ToString(), TimeSpan.Zero);
                    callback(wex, null);
                }
            };
            
            try
            {
                if (HttpRequestType.HttpGet == requestType)
                {
                   getResponse();
                }
                else
                {
                    wr.BeginGetRequestStream((async) =>
                    {
                        try
                        {
                            using (var rs = wr.EndGetRequestStream(async))
                            {
                                switch (requestType)
                                {
                                    case HttpRequestType.HttpPostJson:
                                        wr.ContentType = "application/json";

                                        parameters = ConvertUnspecifiedDateTimes(parameters);

                                        var json = JsonConvert.SerializeObject(parameters, Formatting.None, new JsonSerializerSettings
                                        {
                                            DateTimeZoneHandling = DateTimeZoneHandling.Utc
                                        });
                                        byte[] jsonbytes = System.Text.Encoding.UTF8.GetBytes(json);

                                        rs.Write(jsonbytes, 0, jsonbytes.Length);
                                        LogRequest(MethodName(verb, path),url, json);
                                        break;
                                    case HttpRequestType.HttpPostMultipartForm:
                                        HttpPostMultipart(wr, rs, parameters);
                                        break;
                                }
                                rs.Flush();
                            }

                            getResponse();

                        }
                        catch (WebException wex)
                        {
                            LogResponse(MethodName(verb, path), wex.ToString(), TimeSpan.Zero);
                               
                            callback(wex, null);
                        }


                    }, null);
                }
            }
            catch (WebException wex)
            {
                LogResponse(MethodName(verb, path), wex.ToString(), TimeSpan.Zero);
                               
                callback(wex, null);
            }

        }

        // assume unspecified DateTime.Kind is always local
        private IDictionary<string, object> ConvertUnspecifiedDateTimes(IDictionary<string, object> parameters)
        {
            parameters = parameters.Select(parameter =>
            {
                if (parameter.Value is DateTime && ((DateTime)parameter.Value).Kind == DateTimeKind.Unspecified)
                {
                    parameter = new KeyValuePair<string,object>(parameter.Key, DateTime.SpecifyKind((DateTime)parameter.Value, DateTimeKind.Local));
                }

                return parameter;
            }).ToDictionary(x => x.Key, x => x.Value);

            return parameters;
        }

        private static string GetGlobalizedString(object value)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}", value);
        }

        private static void HttpPostMultipart(HttpWebRequest wr, Stream requestStream, IDictionary<string, object> nvc)
        {
            var files = new List<Tuple<string,BuddyFile>>();
            
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");

            wr.ContentType = "multipart/form-data; boundary=" + boundary;

            var boundaryWritten = false;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (var kvp in nvc)
            {
                if (kvp.Value == null) continue;

                if (kvp.Value is BuddyFile)
                {
                    files.Add(new Tuple<string,BuddyFile>(kvp.Key, (BuddyFile)kvp.Value));
                    continue;
                }

                boundaryWritten = WriteBoundary(boundaryWritten, boundary, requestStream);

                var globalizedString = GetGlobalizedString(kvp.Value);

                string formitem = string.Format(CultureInfo.InvariantCulture, formdataTemplate, kvp.Key, globalizedString);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                requestStream.Write(formitembytes, 0, formitembytes.Length);
            }

            for (var i = files.Count-1; i >=0; i--)
            {
                var file = files[i].Item2;
                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(CultureInfo.InvariantCulture, headerTemplate, files[i].Item1, file.Name, file.ContentType);
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                boundaryWritten = WriteBoundary(boundaryWritten, boundary, requestStream);
                requestStream.Write(headerbytes, 0, headerbytes.Length);
                requestStream.Write(file.Bytes, 0, (int)file.Bytes.Length);
             }

            byte[] trailer = System.Text.Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            requestStream.Write(trailer, 0, trailer.Length);
        }

        private static bool WriteBoundary(bool boundaryAlreadyWritten, string boundary, Stream stream)
        {
            if (boundaryAlreadyWritten)
            {
                var crlf = Encoding.UTF8.GetBytes("\r\n");
                stream.Write(crlf, 0, crlf.Length);
            }

            var partBoundary = "--" + boundary + "\r\n";
            var partBoundaryBytes = Encoding.UTF8.GetBytes(partBoundary);
            stream.Write(partBoundaryBytes, 0, partBoundaryBytes.Length);

            return true;
        }
     }
}
