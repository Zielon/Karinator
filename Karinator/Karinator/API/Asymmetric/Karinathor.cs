using System;
using System.Security.Cryptography;

namespace Karinator.API.Asymmetric
{
    public class Karinathor : AsymmetricAlgorithm
    {
        public override string SignatureAlgorithm { get; }
        public override string KeyExchangeAlgorithm { get; }

        public override void FromXmlString(string xmlString)
        {
            throw new NotImplementedException();
        }

        public override string ToXmlString(bool includePrivateParameters)
        {
            throw new NotImplementedException();
        }
    }
}
