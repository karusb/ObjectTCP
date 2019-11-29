using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Bazcrypt;

namespace OTcpCoreComponents
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class OTcpServerHeader
    {

        public Guid Token = Guid.Empty;
        public byte[] CommandHash;
        public long DataSize = 0;
        public string ExpectType = new string(' ', TypeStringLength);
        public static int TypeStringLength = 64;
        public static int HeaderSize = ObjectBinarySerialization.ObjectToByteArray(new OTcpServerHeader(HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes("OTcp"), HashAlgorithmName.SHA256), 102,"")).Length;




        // Creates totally empty header
        public OTcpServerHeader(byte[] command,long dataSize, string expectType,Guid token)
        {
            this.CommandHash = command;
            this.DataSize = dataSize;
            this.Token = token;
            TryExpectTypeSet(expectType);

        }
        public OTcpServerHeader(byte[] command, long dataSize,string expectType)
        {
            this.CommandHash = command;
            this.DataSize = dataSize;
            TryExpectTypeSet(expectType);
        }
        private void TryExpectTypeSet(string expectType)
        {
            if (expectType.Length < TypeStringLength)
            {
                this.ExpectType = expectType;
                string padString = new string(' ', TypeStringLength - expectType.Length);
                this.ExpectType += padString;
            }
            else if (expectType.Length == TypeStringLength)
            {
                this.ExpectType = expectType;
            }
            else if (expectType.Length > TypeStringLength)
            {
                throw new Exception("Header TypeStringSize is limiting header creation.");
            }
        }
    }
}
