using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Bazcrypt;

namespace OTcpCoreComponents
{
    public class OTcpServerCommands : OTcpCommands
    {

        private Dictionary<string, FunctionHandler> CommandList = new Dictionary<string, FunctionHandler>();
        public void AddCommand(string commandName, FunctionHandler.ObjInObjOutHandlerDelegate responseFunction)
        {

            if (commandName != null && responseFunction != null)
            {
                var cmdHash = HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes(commandName), HashAlgorithmName.SHA256);
                var fh = new FunctionHandler(cmdHash, commandName, FunctionHandlerType.ObjInObjOut, responseFunction);
                CommandList.Add(commandName, fh);
                CommandHashMap.Add(commandName, cmdHash);
            }
        }
        public void AddCommand(string commandName, FunctionHandler.ObjInVoidOutHandlerDelegate responseFunction)
        {

            if (commandName != null && responseFunction != null)
            {
                var cmdHash = HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes(commandName), HashAlgorithmName.SHA256);
                var fh = new FunctionHandler(cmdHash, commandName, FunctionHandlerType.ObjInVoidOut, responseFunction);
                CommandList.Add(commandName, fh);
                CommandHashMap.Add(commandName, cmdHash);
            }
        }
        public void AddCommand(string commandName, FunctionHandler.VoidInObjOutHandlerDelegate responseFunction)
        {

            if (commandName != null && responseFunction != null)
            {
                var cmdHash = HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes(commandName), HashAlgorithmName.SHA256);
                var fh = new FunctionHandler(cmdHash, commandName, FunctionHandlerType.VoidInObjOut, responseFunction);
                CommandList.Add(commandName, fh);
                CommandHashMap.Add(commandName, cmdHash);
            }
        }
        public void AddCommand(string commandName, FunctionHandler.VoidInVoidOutHandlerDelegate responseFunction)
        {

            if (commandName != null && responseFunction != null)
            {
                var cmdHash = HashAlgoProvider.HashBytes(Encoding.UTF8.GetBytes(commandName), HashAlgorithmName.SHA256);
                var fh = new FunctionHandler(cmdHash, commandName, FunctionHandlerType.VoidInVoidOut, responseFunction);
                CommandList.Add(commandName, fh);
                CommandHashMap.Add(commandName, cmdHash);
            }
        }

        public FunctionHandler GetHandler(string command)
        {
            if (CommandList.ContainsKey(command))
            {
                return CommandList[command];
            }
            else
            {
                return null;
            }
        }


    }
}
