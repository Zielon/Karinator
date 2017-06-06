using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace Karinator
{

    public class Node : INotifyPropertyChanged
    {
        private int _progress;

        public string File { get; set; }
        public string Path { get; set; }
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; PropertyChanged(this, new PropertyChangedEventArgs("Progress")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    public partial class MainWindow : Window
    {
        ObservableCollection<Node> nodes = new ObservableCollection<Node>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            nodes.Clear();

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    nodes.Add(new Node
                    {
                        File = System.IO.Path.GetFileName(filename),
                        Path = filename
                    });
            }

            listView.ItemsSource = nodes;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (nodes.Count > 0)
            {
                Aes crypto = Aes.Create();

                crypto.GenerateIV();
                crypto.GenerateKey();

                vectorText.Text += string.Join("-", crypto.IV);
                passwordBox.Text += string.Join("-", crypto.Key);

                buttonEncrypt.IsEnabled = false;
                buttonDecrypt.IsEnabled = false;

                List<Task> tasks = new List<Task>();

                foreach (var item in nodes)
                {
                    var output = new List<string>(item.Path.Split('.'));
                    output[output.Count - 2] = output[output.Count - 2] + "_C.";
                    tasks.Add(EncryptFile(item.Path, string.Join("", output), crypto, item));
                }

                Task.Factory.ContinueWhenAll(tasks.ToArray(),
                    t => {
                        buttonDecrypt.IsEnabled = true;
                        buttonEncrypt.IsEnabled = true;
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private Task EncryptFile(string inputFile, string outputFile, SymmetricAlgorithm encryptor, Node node)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    string cryptFile = outputFile;
                    using (FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create))
                    using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                    using (CryptoStream cs = new CryptoStream(fsCrypt, encryptor.CreateEncryptor(encryptor.Key, encryptor.IV), CryptoStreamMode.Write))
                    {
                        var incrementation = fsIn.Length / 100;

                        if (fsIn.Length < 100)
                            node.Progress = 100;

                        int data; int i = 1;
                        while ((data = fsIn.ReadByte()) != -1)
                        {
                            if (incrementation > 0 && i % incrementation == 0)
                                node.Progress++;
                            cs.WriteByte((byte)data);
                            i++;
                        }

                        node.Progress = 100;
                    }
                }
                catch (Exception e){
                    MessageBox.Show("Encryption failed! " + e.Message, "Error");
                }
            }, TaskCreationOptions.LongRunning );
        }

        private Task DecryptFile(string inputFile, string outputFile, SymmetricAlgorithm decryptor, Node node)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                    using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                    using (CryptoStream cs = new CryptoStream(fsCrypt, decryptor.CreateDecryptor(decryptor.Key, decryptor.IV), CryptoStreamMode.Read))
                    {
                        var incrementation = fsCrypt.Length / 100;

                        if (fsCrypt.Length < 100)
                            node.Progress = 100;

                        int data; int i = 1;
                        while ((data = cs.ReadByte()) != -1)
                        {
                            if (incrementation > 0 && i % incrementation == 0)
                                node.Progress++;
                            fsOut.WriteByte((byte)data);
                            i++;
                        }

                        node.Progress = 100;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Decryption failed! " + e.Message, "Error");
                }
            }, TaskCreationOptions.LongRunning );
        }

        private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
        {

            if (nodes.Count > 0)
            {
                Aes crypto = Aes.Create();

                string key = this.passwordBox.Text;
                string vector = this.vectorText.Text;

                if (!(key.Length > 0) || !(vector.Length > 0)) return;

                var keyArray = key.Split(new[] { '-'}, StringSplitOptions.RemoveEmptyEntries).Select(i => byte.Parse(i)).ToArray();
                var vectorArray = vector.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries).Select(i => byte.Parse(i)).ToArray();

                crypto.IV = vectorArray;
                crypto.Key = keyArray;

                buttonEncrypt.IsEnabled = false;
                buttonDecrypt.IsEnabled = false;

                List<Task> tasks = new List<Task>();

                foreach (var item in nodes)
                {
                    var output = new List<string>(item.Path.Split('.'));
                    output[output.Count - 2] = output[output.Count - 2] + "_Ready.";
                    tasks.Add(DecryptFile(item.Path, string.Join("", output), crypto, item));
                }

                Task.Factory.ContinueWhenAll(tasks.ToArray(),
                    t => {
                        buttonDecrypt.IsEnabled = true;
                        buttonEncrypt.IsEnabled = true;
                    },
                    CancellationToken.None,
                    TaskContinuationOptions.None,
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
