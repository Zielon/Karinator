using System;

namespace Karinator.Enums
{
    [Flags]
    public enum Algorithm
    {
        Aes = 1,
        DES = 2,
        RC2 = 4,
        Rijndael = 8,
        TripleDES = 16
    }
}
