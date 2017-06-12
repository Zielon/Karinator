using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Karinator.API;
using Microsoft.Win32;

namespace Karinator.Helpers
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
    }
}
