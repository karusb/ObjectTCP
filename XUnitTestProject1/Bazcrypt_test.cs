using System;
using Xunit;
using OTcpCoreComponents;
using System.Text;

namespace Tests
{
    public class Bazcrypt_test
    {
        [Fact]
        public void EncryptionDecryption_GoodWeather()
        {
            Bazcrypt.BazCryptBase bazCrypt = new Bazcrypt.BazCryptBase();
            string msg = "some type of message that will be carried";
            byte[] msgb = Encoding.UTF8.GetBytes(msg);
            string pw = "somePasswordIwillUse";
            byte[] pwb = Encoding.UTF8.GetBytes(pw);
            int gen = 1234;
            int algo = 0;
            int res = 8;
            byte[] key = bazCrypt.BazCryptKey(pwb, gen, algo);
            byte[] enc = bazCrypt.BazCrypt(msgb, key, algo,res);

            byte[] enc2 = bazCrypt.BazCrypt(enc, key, algo,res);
            var output = Encoding.UTF8.GetString(enc2);
            Console.WriteLine(msg);
            Console.WriteLine(enc2);
            Console.WriteLine(output);
            Assert.Equal(msg, output);
        }
        [Fact]
        public void Resolution_GoodWeather()
        {
            System.Collections.BitArray bits128 = new System.Collections.BitArray(128,true);

            var res = Bazcrypt.ResolutionArraySolver.CreateResolutionArray(bits128, 8);
            var unres = Bazcrypt.ResolutionArraySolver.UndoResolutionArray(res);
            Assert.Equal(16, res.Length);
            Assert.Equal(bits128.Length, unres.Length);
            Assert.Equal(bits128,unres);
        }
    }
}
