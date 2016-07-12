#if WINDOWS_APP_MODEL

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BuddySDK
{
    internal static class DotNetDeltas
    {
        public static PropertyInfo GetProperty(this System.Type t, string name)
        {
            return t.GetRuntimeProperty(name);
        }

        public static ConstructorInfo GetConstructor(this System.Type t, params Type[] paramTypes)
        {
            return t.GetTypeInfo().DeclaredConstructors.Where(c => c.GetParameters().Count().Equals(paramTypes.Count()))
                .FirstOrDefault(c => c.GetParameters().All(p => p.ParameterType.IsAssignableFrom(paramTypes.ElementAt(p.Position))));
        }

        public static T GetCustomAttribute<T>(this System.Reflection.PropertyInfo pi) where T : System.Attribute
        {
            return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(pi);
        }

        public static T GetCustomAttribute<T>(this System.Type t) where T : System.Attribute
        {
            return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(t.GetTypeInfo());
        }

        public static int CurrentThreadId
        {
            get
            {
                return Environment.CurrentManagedThreadId;
            }
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type t)
        {
            return t.GetRuntimeProperties();
        }

        public static bool IsAssignableFrom(this Type t, Type other)
        {
            return t.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }

        public static bool IsInstanceOfType(this Type t, object obj)
        {
            if (obj == null) return false;
            return IsAssignableFrom(t, obj.GetType());
        }

        public static void Sleep(int ms)
        {
            Task.Delay(ms).Wait();
        }

        public static StringComparer InvariantComparer(bool ignoreCase = false)
        {
            if (ignoreCase)
            {
                return StringComparer.OrdinalIgnoreCase;
            }
            else
            {
                return StringComparer.Ordinal;
            }
        }

        public class ExceptionEventArgs
        {
            public Exception Exception { get; set; }
            public string Message { get; set; }
            public bool IsHandled { get; set; }
        }

        public static event EventHandler<ExceptionEventArgs> UnhandledException;

        static DotNetDeltas()
        {
            Application.Current.UnhandledException += (s, args) =>
            {
                var a = new ExceptionEventArgs
                {
                    Exception = args.Exception,
                    Message = args.Message
                };
                if (UnhandledException != null)
                {
                    UnhandledException(null, a);
                }
                args.Handled = a.IsHandled;
            };
        }

        public static string SignString(string key, string message)
        {
            MacAlgorithmProvider macAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm("HMAC_SHA256");
            var binaryMessage = CryptographicBuffer.ConvertStringToBinary(message, BinaryStringEncoding.Utf8);
            var binaryKeyMaterial = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            var hmacKey = macAlgorithmProvider.CreateKey(binaryKeyMaterial);
            var binarySignedMessage = CryptographicEngine.Sign(hmacKey, binaryMessage);
            var signedMessage = CryptographicBuffer.EncodeToHexString(binarySignedMessage);
            return signedMessage;
        }
    }
}

#else

using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BuddySDK
{
    internal static class DotNetDeltas
    {
        public static T GetCustomAttribute<T>(this System.Reflection.PropertyInfo pi) where T : System.Attribute
        {
            return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(pi);
        }

        public static T GetCustomAttribute<T>(this System.Type t) where T : System.Attribute
        {
            return System.Reflection.CustomAttributeExtensions.GetCustomAttribute<T>(t.GetTypeInfo());
        }

        public static System.Collections.Generic.IEnumerable<PropertyInfo> GetProperties(this System.Type t)
        {
            return t.GetProperties();
        }

        public static bool IsAssignableFrom(this System.Type t, System.Type other)
        {
            return t.IsAssignableFrom(other);
        }

        public static bool IsInstanceOfType(this System.Type t, object obj)
        {
            return t.IsInstanceOfType(obj);
        }

        public static void Sleep(int ms)
        {
            System.Threading.Thread.Sleep(ms);
        }

        public static int CurrentThreadId
        {
            get
            {
                return System.Threading.Thread.CurrentThread.ManagedThreadId;
            }
        }

        public static System.StringComparer InvariantComparer(bool ignoreCase = false)
        {
#if NETCORE
            //TODO: InvariantComparer should become available in .NET Core 1.1.
            if (ignoreCase)
            {
                return System.StringComparer.Ordinal;
            }
            else
            {
                return System.StringComparer.OrdinalIgnoreCase;
            }
#else
            if (ignoreCase)
            {
                return System.StringComparer.InvariantCulture;
            }
            else
            {
                return System.StringComparer.InvariantCultureIgnoreCase;
            }
#endif
        }

        public class ExceptionEventArgs
        {
            public System.Exception Exception { get; set; }
            public string Message { get; set; }
            public bool IsHandled { get; set; }
        }

        public static event System.EventHandler<ExceptionEventArgs> UnhandledException;

        /*static DotNetDeltas()
        {
            System.AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var a = new ExceptionEventArgs
                {
                    Exception = args.ExceptionObject as System.Exception
                };

                if (UnhandledException != null)
                {
                    UnhandledException(null, a);
                }

            };
        }*/

        public static string SignString(string key, string stringToSign)
        {
            using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                return BuddyUtils.ToHex(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            }
        }
    }
}

#endif
