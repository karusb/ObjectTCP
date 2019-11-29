using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Bazcrypt
{
    public static class HashAlgoProvider
    {
        
        static public byte[] HashBytes(byte[] input, HashAlgorithmName strAlgoName)
        {
            
            HashAlgorithm algo = HashAlgorithm.Create(strAlgoName.Name);
            var outBytes = algo.ComputeHash(input);
            // Verify that the hash length equals the length specified for the algorithm.
            if (outBytes.Length*8 != algo.HashSize)
            {
                throw new Exception("There was an error creating the hash");
            }
            return outBytes;
        }
    }
}
