using System;
using System.Collections.Generic;
using System.Text;

namespace Bazcrypt
{
    public class ResolutionArraySolver
    {
        public static System.Collections.BitArray[] CreateResolutionArray(System.Collections.BitArray array, int bitres)
        {
            if (bitres < 4) throw new SystemException();
            int lenMod = (array.Length % bitres);
            int addition = 0;
            if (lenMod != 0) throw new SystemException();

            int outSize = array.Length / bitres + addition;
            System.Collections.BitArray[] outArray = new System.Collections.BitArray[outSize];
            for (int i = 0; i < outSize; ++i)
            {
                outArray[i] = new System.Collections.BitArray(bitres);
                for (int res = 0; res < bitres; ++res)
                {
                    outArray[i].Set(res, array.Get(i * bitres + res));
                }
            }
            return outArray;
        }
        public static System.Collections.BitArray UndoResolutionArray(System.Collections.BitArray[] bitArrays)
        {
            int bitres = bitArrays[0].Length;
            System.Collections.BitArray outArray = new System.Collections.BitArray(bitArrays.Length * bitres);

            for (int i = 0; i < bitArrays.Length; ++i)
            {
                for (int j = 0; j < bitres; j++)
                {
                    outArray.Set(i * bitres + j, bitArrays[i].Get(j));
                }
            }
            return outArray;
        }
    }
}
