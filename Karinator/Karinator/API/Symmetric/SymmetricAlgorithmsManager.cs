﻿using System.Collections.Generic;
using System.Security.Cryptography;
using Karinator.API.Symmetric.Enums;

namespace Karinator.API.Symmetric
{
    public static class SymmetricAlgorithmsManager
    {
        public static readonly Dictionary<Algorithm, SymmetricAlgorithm> Algorithms = new Dictionary<Algorithm, SymmetricAlgorithm>();

        static SymmetricAlgorithmsManager()
        {
            Algorithms[Algorithm.Aes] = Aes.Create();
            Algorithms[Algorithm.DES] = DES.Create();
            Algorithms[Algorithm.RC2] = RC2.Create();
            Algorithms[Algorithm.Rijndael] = Rijndael.Create();
            Algorithms[Algorithm.TripleDES] = TripleDES.Create();
        }

        public static Keys GenerateKeys(Algorithm algorithm)
        {
            var algo = Algorithms[algorithm];
            algo.GenerateIV();
            algo.GenerateKey();
            return new Keys(algo.Key, algo.IV);
        }

        public static void SetKeys(Keys keys, Algorithm algorithm)
        {
            var algo = Algorithms[algorithm];
            algo.IV = keys.GetIVFromString();
            algo.Key = keys.GetKeyFromString();
        }
    }
}
