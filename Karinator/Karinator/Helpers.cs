using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace Karinator
{
    public static class Helpers
    {
        public static void GetNodes(ObservableCollection<Node> collection)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            collection.Clear();

            if (openFileDialog.ShowDialog() == true)
                openFileDialog.FileNames.ToList().ForEach(e => collection.Add(new Node { FileName = Path.GetFileName(e), Path = e }));
        }

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
