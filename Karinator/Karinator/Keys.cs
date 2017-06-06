using System;

namespace Karinator
{
    public class Keys
    {
        private readonly byte[] IV;
        private readonly string IV_str;

        private readonly byte[] Key;
        private readonly string Key_str;

        public Keys(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public Keys(string key, string iv)
        {
            Key_str = key;
            IV_str = iv;
        }

        public byte[] GetIVFromString()
        {
            if (IV_str == null) throw new NullReferenceException();

            return IV_str.GetKey();
        }

        public byte[] GetKeyFromString()
        {
            if (Key_str == null) throw new NullReferenceException();

            return Key_str.GetKey();
        }

        public string GetIV()
        {
            return IV.GetKey();
        }

        public string GetKey()
        {
            return Key.GetKey();
        }
    }
}
