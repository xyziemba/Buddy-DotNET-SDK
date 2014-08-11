using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BuddySDK
{
    static class BuddyUtils
    {
        internal static Task<T2> WrapTask<T1, T2>(this Task<T1> mainTask, Func<Task<T1>, T2> mapper)
        {
            TaskCompletionSource<T2> tcs = new TaskCompletionSource<T2>();

            mainTask.ContinueWith((t1) =>
            {
                if (t1.Exception != null)
                {
                    tcs.SetException(t1.Exception);
                }
                else
                {
                    var t2 = mapper(t1);
                    tcs.SetResult(t2);
                }
            });

            return tcs.Task;

        }

        public static string ToHex(byte[] bytes)
        {
            char[] c = new char[bytes.Length * 2];

            byte b;

            for (int bx = 0, cx = 0; bx < bytes.Length; ++bx, ++cx)
            {
                b = ((byte)(bytes[bx] >> 4));
                c[cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);

                b = ((byte)(bytes[bx] & 0x0F));
                c[++cx] = (char)(b > 9 ? b + 0x37 + 0x20 : b + 0x30);
            }

            return new string(c);
        }

        public static string SignString(string key, string stringToSign)
        {
            using (var hasher = new HMACSHA256(Encoding.UTF8.GetBytes(key)))
            {
                return ToHex(hasher.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));
            }
        }

        internal static Task<BuddyResult<T2>> WrapResult<T1, T2>(this Task<BuddyResult<T1>> mainTask, Func<BuddyResult<T1>, T2> mapper, Func<BuddyResult<T1>, T2, BuddyResult<T2>> converter = null)
        {
            Func<BuddyResult<T1>, T2, BuddyResult<T2>> defaultConverter = (br1, bt2) => br1.Convert(x1 => bt2);
            converter = converter ?? defaultConverter;

            return WrapTask<BuddyResult<T1>, BuddyResult<T2>>(mainTask, (t1) =>
            {

                return converter(t1.Result,  mapper(t1.Result));
            });
        }
    }
}
