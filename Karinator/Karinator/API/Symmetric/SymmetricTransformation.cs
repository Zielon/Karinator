using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;
using Karinator.API.Symmetric.Enums;
using Karinator.Helpers;

namespace Karinator.API.Symmetric
{
    public class SymmetricTransformation
    {
        private SymmetricAlgorithm _algorithm;
        private Func<byte[], byte[], ICryptoTransform> _transform;

        public List<Task> Transform(List<Node> nodes, CryptoStreamMode mode, Algorithm algorithm)
        {
            _algorithm = SymmetricAlgorithmsManager.Algorithms[algorithm];

            var tasks = new List<Task>();
            Func<string, Node, Task> fun;

            if (mode == CryptoStreamMode.Read)
            {
                fun = DecryptFile;
                _transform = _algorithm.CreateDecryptor;
            }
            else
            {
                fun = EncryptFile;
                _transform = _algorithm.CreateEncryptor;
            }

            nodes.ForEach(e => { tasks.Add(fun(e.Path, e)); });

            return tasks;
        }

        private Task EncryptFile(string inputFile, Node node)
        {
            return Perform(inputFile, inputFile.ToOutputPath("_C"), node, CryptoStreamMode.Write);
        }

        private Task DecryptFile(string inputFile, Node node)
        {
            return Perform(inputFile, inputFile.ToOutputPath("_D"), node, CryptoStreamMode.Read);
        }

        private Task Perform(string inputFile, string outputFile, Node node, CryptoStreamMode mode)
        {
            return Task.Factory.StartNew(
                () => {
                    try
                    {
                        using (FileStream fsOut = new FileStream(outputFile, FileMode.Create))
                        using (FileStream fsIn = new FileStream(inputFile, FileMode.Open))
                        {
                            int data, i = 1;
                            var inc = fsIn.Length / 100;
                            if (mode == CryptoStreamMode.Read)
                                using (CryptoStream cs = new CryptoStream(
                                    fsIn,
                                    _transform(_algorithm.Key, _algorithm.IV),
                                    mode))
                                {
                                    while ((data = cs.ReadByte()) != -1)
                                    {
                                        ChangeProgress(i, inc, node);
                                        fsOut.WriteByte((byte) data);
                                        i++;
                                    }
                                }
                            else
                                using (CryptoStream cs = new CryptoStream(
                                    fsOut,
                                    _transform(_algorithm.Key, _algorithm.IV),
                                    mode))
                                {
                                    while ((data = fsIn.ReadByte()) != -1)
                                    {
                                        ChangeProgress(i, inc, node);
                                        cs.WriteByte((byte) data);
                                        i++;
                                    }
                                }
                            node.Progress = 100;
                        }
                    }
                    catch (Exception e) { MessageBox.Show("Encryption failed! " + e.Message, "Error"); }
                });
        }

        private void ChangeProgress(int i, long incrementation, Node node)
        {
            if (incrementation > 0 && i % incrementation == 0) node.Progress++;
        }
    }
}
