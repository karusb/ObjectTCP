using System;
using System.Threading.Tasks;
namespace Bazcrypt
{

    public class BazCryptBase
    {
        public int currentGen;
        public int currentTarget;

        public byte[] BazCryptKey(byte[] password,int generations,int algorithm)
        {
            currentTarget = generations;

            // Define Bitsets to hold message and CA world

            System.Collections.BitArray bitK = new System.Collections.BitArray(password);


            // EVOLUTION
            for (int curgen = 0; curgen < generations; ++curgen)
            {
                currentGen = curgen;
                bitK = AlgoSelect(bitK, algorithm);
            }
            return ToByteArray(bitK);

        }
        public byte[] DISABLED_BazCrypt(byte[] message,byte[] key,int algorithm)
        {
            System.Collections.BitArray bitM = new System.Collections.BitArray(message);
            System.Collections.BitArray bitK = new System.Collections.BitArray(key);
            System.Collections.BitArray bitO = new System.Collections.BitArray(message.Length,false);
            for (int i = 0; i < bitM.Length; i++)
            {
                if (i >= bitK.Length)
                {
                    if (i % bitK.Length == 0)
                    {
                        bitK = AlgoSelect(bitK, algorithm);
                    }
                }


                bitO[i] = bitM[i] ^ bitK[i%bitK.Length]; // XOR KEY WITH THE MESSAGE

                                                  
            }
            return ToByteArray(bitO);
        }
        public byte[] BazCrypt(byte[] message, byte[] key,int algorithm,int bitres)
        {
            System.Collections.BitArray[] bitM = ResolutionArraySolver.CreateResolutionArray(new System.Collections.BitArray(message), bitres);
            System.Collections.BitArray[] bitK = ResolutionArraySolver.CreateResolutionArray(new System.Collections.BitArray(key), bitres);
            System.Collections.BitArray[] bitOr = ResolutionArraySolver.CreateResolutionArray(new System.Collections.BitArray(message), bitres);
            System.Collections.BitArray bitO = new System.Collections.BitArray(message.Length, false);

            for(int i = 0; i < bitM.Length;++i)
            {
                bitK[i%bitK.Length] = AlgoSelect(bitK[i%bitK.Length], algorithm);
                //bitM[i] = AlgoSelect(bitM[i], algorithm);
                for (int j = 0; j < bitres; ++j)
                {
                    var xor = bitM[i].Get(j) ^ bitK[i%bitK.Length].Get(j);
                    bitOr[i].Set(j, xor);
                }
            }
            bitO = ResolutionArraySolver.UndoResolutionArray(bitOr);
            return ToByteArray(bitO);
        }
        public static byte[] ToByteArray(System.Collections.BitArray bits)
        {

            const int BYTE = 8;
            int length = (bits.Length / BYTE) + ((bits.Length % BYTE == 0) ? 0 : 1);
            var bytes = new byte[length];
            //   Parallel.For(0, bits.Length, i =>
            //{
            //    int bitIndex = i % BYTE;
            //    int byteIndex = i / BYTE;

            //    int mask = (bits[i] ? 1 : 0) << bitIndex;
            //    bytes[byteIndex] |= (byte)mask;
            //});
            for (int i = 0; i < bits.Length; i++)
            {

                int bitIndex = i % BYTE;
                int byteIndex = i / BYTE;

                int mask = (bits[i] ? 1 : 0) << bitIndex;
                bytes[byteIndex] |= (byte)mask;

            }//for

            return bytes;
        }
        // EVOLVE FUNCTIONS
        // Inputs MUST be a bitset of 20000 bits
        // Nbytes is the message size in bytes which determines the boundaries of evolution within the world
private static System.Collections.BitArray evolve39318(System.Collections.BitArray s)
{

    // VERY REPETITIVE RESULTS?
    // RULE 39318 4N

    int i = 0, nbits = 0;
            nbits = s.Length;

    System.Collections.BitArray t = new System.Collections.BitArray(nbits);

    t[0] = s[0] ^ s[1] ^ (s[nbits - 2] | s[nbits - 1]);
    t[1] = s[1] ^ s[2] ^ (s[nbits - 1] | s[0]);
    t[nbits - 1] = s[nbits - 1] ^ s[0] ^ (s[nbits - 3]| s[nbits - 2]);

    for (int j = 2; j < nbits-1; ++j)
    {

            t[j] = s[j] ^ s[j + 1] ^ (s[j - 2] | s[j - 1]);
    }

    return t;
}
private static System.Collections.BitArray evolve57630z(System.Collections.BitArray s) //ZERO BOUNDARY EVOLUTION
{
            int i = 0, nbits = 0;
            nbits = s.Length;

            System.Collections.BitArray t = new System.Collections.BitArray(nbits);

            // RULE 57630 4N
    t[0] = false ^ false ^ (s[0] | s[1]);
    t[1] = false ^ s[0] ^ (s[1] | s[2]);
    t[nbits - 1]= s[nbits - 3]^ s[nbits - 2] ^ (s[nbits - 1] | false);
            Parallel.For(2, nbits - 1, j =>
              {
                  t[j] = s[j - 2] ^ s[j - 1] ^ (s[j] | s[j + 1]);
              });
            //for (int j = 2; j < nbits - 1; ++j)
            //{

            //    t[j] = s[j - 2] ^ s[j - 1] ^ (s[j] | s[j + 1]);
            //}

            return t;

}
private static System.Collections.BitArray evolve57630b(System.Collections.BitArray s) //CYCLIC BOUNDARY EVOLUTION
{
            int i = 0, nbits = 0;
            nbits = s.Length;

            System.Collections.BitArray t = new System.Collections.BitArray(nbits);

            // RULE 57630 4N
    t[0] = s[nbits - 2] ^ s[nbits - 1] ^ (s[0] | s[1]);
    t[1] = s[nbits - 1] ^ s[0] ^ (s[1] | s[2]);
    t[nbits - 1] = s[nbits - 3] ^ s[nbits - 2] ^ (s[nbits - 1] | s[0]);
    for (int j = 2; j < nbits-1; ++j)
    {

            t[j] = s[j - 2] ^ s[j - 1] ^ (s[j] | s[j + 1]);
    }
            return t;

}
        private static System.Collections.BitArray AlgoSelect(System.Collections.BitArray input, int algorithm)
        {
            switch (algorithm)
            {
                case 0:

                    input = evolve57630b(input);
                    break;
                case 1:

                    input = evolve57630z(input);
                    break;
                case 2:

                    input = evolve39318(input);
                    break;
                default:

                    input = evolve57630b(input);
                    break;
            }
            return input;
        }

    }
}
