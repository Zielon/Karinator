using System;
using System.IO;
using System.Linq;

namespace Karinator.Helpers
{
    public static class ExtenstionMethods
    {
        public static byte[] GetKey(this string key)
        {
            return key.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray();
        }

        public static string GetKey(this byte[] bytes)
        {
            return string.Join("-", bytes);
        }

        public static string ToOutputPath(this string node, string ending)
        {
            Func<string, string> fun = s => {
                var path = s.Split(Path.DirectorySeparatorChar);
                var output = path.Take(path.Length - 1).ToList();
                output.Add($"{Path.GetFileNameWithoutExtension(s)}{ending}{Path.GetExtension(s)}");
                return string.Join(Path.DirectorySeparatorChar.ToString(), output);
            };

            return fun(node);
        }
    }
}
