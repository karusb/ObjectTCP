using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Bazcrypt;

namespace OTcpCoreComponents
{
    public class OTcpCommands
    {

        public Dictionary<string, byte[]> CommandHashMap = new Dictionary<string, byte[]>();

        protected void AddCommandHash(string command)
        {
            var cmdHash = HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes(command), HashAlgorithmName.SHA256);
            CommandHashMap.Add(command,cmdHash);
        }

        public byte[] GetHashOfCommand(string command)
        {
            return CommandHashMap[command];
        }
        public string GetCommandFromHash(byte[] commandHash)
        {
            foreach (var commandPair in CommandHashMap)
            {
                if (commandHash.SequenceEqual(commandPair.Value))
                {
                    return commandPair.Key;
                }
            }

            return null;
        }
    }
}
