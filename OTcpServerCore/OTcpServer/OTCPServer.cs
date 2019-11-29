using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Bazcrypt;
using OTcpCoreComponents;



namespace OTcpServerCore
{
    public class OTcpServer
    {
        private static TcpListener listener { get; set; }
        private static bool accept { get; set; } = false;
        public delegate object FunctionHandlerDelegate(object obj);

        private OTcpServerCommands Commands;
        //private Dictionary<int, Tuple<string, FunctionHandlerDelegate>> CommandList = new Dictionary<int, Tuple<string, FunctionHandlerDelegate>>();
        //private Dictionary<int, FunctionHandler> CommandList = new Dictionary<int, FunctionHandler>();
        //public static List<string> CommandList = new System.Collections.Generic.List<string>();
        private Dictionary<Guid, TcpClient> commandQueue = new System.Collections.Generic.Dictionary<Guid, TcpClient>();
        private Dictionary<Guid, Tuple<string, object>> issuedCommands = new Dictionary<Guid, Tuple<string, object>>();
        public delegate void CommandEventHandler(Guid id, string command);
        public static event CommandEventHandler CommandEvent;
        private int commandCount;
        public TransferType Type;
        public void StartServer(OTcpServerCommands commands,string address, int port, TransferType transferType)
        {
            IPAddress _IPaddress = IPAddress.Parse(address);
            listener = new TcpListener(_IPaddress, port);
            this.Type = transferType;
            this.Commands = commands;
            listener.Start();
            accept = true;


            Console.WriteLine($"OTcp Server started. Listening to TCP clients at {address}:{port}");
        }
        public void Listen()
        {
            if (listener != null && accept)
            {

                // Continue listening.  
                while (true)
                {
                    Console.WriteLine("Waiting for client...");
                    var client = listener.AcceptTcpClient(); // Get the client  
                    Console.WriteLine("Client connected. Waiting for data.");
                    Thread clientHandlerThread = new Thread(new ParameterizedThreadStart(HandleClient));
                    Thread queueHandlerThread = new Thread(new ThreadStart(HandleResponseQueue));
                    clientHandlerThread.Start(client);
                    queueHandlerThread.Start();

                }
            }
        }
        public void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            client.ReceiveBufferSize = int.MaxValue;
            client.SendBufferSize = int.MaxValue;
            while (client.Connected)
            {
                while (client.Available <= 0) ;
                //byte[] data = Encoding.ASCII.GetBytes("Send next data: [enter 'quit' to terminate] ");
                //client.GetStream().Write(data, 0, data.Length);
                OTcpServerMessage receivedMessage = Receive(ref client);
                if (receivedMessage != null)
                {
                    Guid commandId = new Guid();
                    string commandString = Commands.GetCommandFromHash(receivedMessage.Header.CommandHash);

                    commandQueue.Add(commandId, client);
                    issuedCommands.Add(commandId, new Tuple<string, object>(commandString, receivedMessage.Message));
                    CommandEvent?.Invoke(commandId, commandString);
                }
            }
        }

        //public void AddCommand(string commandName, FunctionHandler.ObjInObjOutHandlerDelegate responseFunction)
        //{
        //    if (commandName != null && responseFunction != null)
        //    {
                
        //        var fh  = new FunctionHandler(commandCount,commandName,FunctionHandlerType.ObjInObjOut,responseFunction);
        //        CommandList.Add(commandCount, fh);
        //        commandCount++;
        //    }
        //}
        //public void AddCommand(string commandName, FunctionHandler.ObjInVoidOutHandlerDelegate responseFunction)
        //{
        //    if (commandName != null && responseFunction != null)
        //    {

        //        var fh = new FunctionHandler(commandCount, commandName, FunctionHandlerType.ObjInVoidOut, responseFunction);
        //        CommandList.Add(commandCount, fh);
        //        commandCount++;
        //    }
        //}
        //public void AddCommand(string commandName, FunctionHandler.VoidInObjOutHandlerDelegate responseFunction)
        //{
        //    if (commandName != null && responseFunction != null)
        //    {

        //        var fh = new FunctionHandler(commandCount, commandName, FunctionHandlerType.VoidInObjOut, responseFunction);
        //        CommandList.Add(commandCount, fh);
        //        commandCount++;
        //    }
        //}
        //public void AddCommand(string commandName, FunctionHandler.VoidInVoidOutHandlerDelegate responseFunction)
        //{
        //    if (commandName != null && responseFunction != null)
        //    {

        //        var fh = new FunctionHandler(commandCount, commandName, FunctionHandlerType.VoidInVoidOut, responseFunction);
        //        CommandList.Add(commandCount, fh);
        //        commandCount++;
        //    }
        //}
        private OTcpServerMessage ReceiveBinary(ref TcpClient client)
        {
            // Get Header
            var receivedHeader = ReceiveHeader(ref client);
            var convertedType = StringTypeToTypeConverter.GetTypeFrom(receivedHeader.ExpectType);
            // Once Header Received + Receive Data
            byte[] databuffer = new byte[receivedHeader.DataSize];


            try
            {
                client.GetStream().Read(databuffer, 0, databuffer.Length);
                object dataobject = Convert.ChangeType(ObjectBinarySerialization.ByteArrayToObject(databuffer), convertedType);
                return new OTcpServerMessage(receivedHeader, dataobject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

        }
        private OTcpServerMessage ReceiveJSON(ref TcpClient client)
        {

            // Once Header Received + Receive Data
            var receivedHeader = ReceiveHeader(ref client);
            var convertedType = StringTypeToTypeConverter.GetTypeFrom(receivedHeader.ExpectType);
            byte[] databuffer = new byte[receivedHeader.DataSize];
            try
            {
                client.GetStream().Read(databuffer, 0, databuffer.Length);

                object dataobject = Convert.ChangeType(JsonConvert.DeserializeObject(Encoding.UTF8.GetString(databuffer), convertedType), convertedType);
                return new OTcpServerMessage(receivedHeader, dataobject);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        private OTcpServerMessage Receive(ref TcpClient client)
        {
            OTcpServerMessage receivedMessage;
            switch (Type)
            {
                case TransferType.Binary:
                    receivedMessage = ReceiveBinary(ref client);
                    break;
                case TransferType.JSON:
                    receivedMessage = ReceiveJSON(ref client);
                    break;
                default:
                    receivedMessage = null;
                    break;
            }

            return receivedMessage;
        }
        private OTcpServerHeader ReceiveHeader(ref TcpClient client)
        {
            // Get Header
            var headerByteSize = OTcpServerHeader.HeaderSize;
            byte[] headerbuffer = new byte[headerByteSize];
            //client.Client.RemoteEndPoint
            try
            {
                client.GetStream().Read(headerbuffer, 0, headerbuffer.Length);
                OTcpServerHeader receivedHeader = (OTcpServerHeader)ObjectBinarySerialization.ByteArrayToObject(headerbuffer);
                receivedHeader.ExpectType = receivedHeader.ExpectType.Split(' ')[0];
                return receivedHeader;
            }
            catch (Exception e)
            {
                Console.WriteLine("Header Receive Error" + e.Message );
                throw e;
            }

        }
        public void HandleResponseQueue()
        {
            while (true)
            {
                if (issuedCommands.Count > 0)
                {
                    foreach (var commandPair in commandQueue)
                    {
                        var fHandler = Commands.GetHandler(issuedCommands[commandPair.Key].Item1);
                        var client = commandPair.Value;
                        var command = issuedCommands[commandPair.Key].Item1;
                        object data = issuedCommands[commandPair.Key].Item2;
                        object message;
                        switch (fHandler.HandlerType)
                        {
                            case FunctionHandlerType.ObjInObjOut:

                                message = fHandler.ObjInObjOutHandler(data);
                                Send(ref client, message, command, Guid.Empty);
                                break;
                            case FunctionHandlerType.ObjInVoidOut:
                                fHandler.ObjInVoidOutHandler(data);
                                break;
                            case FunctionHandlerType.VoidInObjOut:
                                message = fHandler.VoidInObjOutHandler();
                                Send(ref client, message, command, Guid.Empty);
                                break;
                            case FunctionHandlerType.VoidInVoidOut:
                                fHandler.VoidInVoidOutHandler();
                                break;
                        }
                        issuedCommands.Remove(commandPair.Key);
                    }
                    commandQueue.Clear();
                }
            }

        }

        private OTcpServerHeader SendHeader(ref TcpClient client, int messageSize, string command,string expectType, Guid token)
        {
            var header = new OTcpServerHeader(Commands.GetHashOfCommand(command),messageSize,expectType,token);
            var headerBytes = ObjectBinarySerialization.ObjectToByteArray(header);
            client.GetStream().Write(headerBytes, 0, headerBytes.Length);
            return header;
        }
        private void SendBinary<T>(ref TcpClient client, T message,string command, Guid token)
        {
            try
            {
                var messageBytes = ObjectBinarySerialization.ObjectToByteArray(message);
                var messageSize = messageBytes.Length;
                SendHeader(ref client, messageSize, command, message.GetType().FullName, token);
                // token handle
                client.GetStream().Write(messageBytes, 0, messageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        private void SendJSON<T>(ref TcpClient client, T message, string command, Guid token)
        {
            try
            {
                var messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                var messageSize = messageBytes.Length;
                SendHeader(ref client, messageSize, command, message.GetType().FullName, token);
                // token handle
                client.GetStream().Write(messageBytes, 0, messageSize);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        private void Send<T>(ref TcpClient client, T message, string command, Guid token)
        {
            try
            {
                switch (this.Type)
                {
                    case TransferType.Binary:
                        SendBinary(ref client, message, command, token);
                        break;
                    case TransferType.JSON:
                        SendJSON(ref client, message, command, token);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

   
}
