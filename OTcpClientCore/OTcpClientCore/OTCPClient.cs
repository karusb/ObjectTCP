using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using OTcpCoreComponents;
namespace OTcpClientCore
{
    public class OTcpClient
    {
        private TcpClient _currentClient;
        private NetworkStream _currentStream;
        public TransferType Type;
        private OTcpClientCommands Commands;
        private delegate void ReceiveEventDelegate(object obj);

        private event ReceiveEventDelegate ReceiveEvent;
        public OTcpClient(OTcpClientCommands commands,string ipAddr, int port,TransferType type)
        {
            // add watchdog for disconnect
            this.Type = type;
            Commands = commands;
            try
            {
                _currentClient = new TcpClient(ipAddr, port); // Create a new connection  
                _currentClient.ReceiveBufferSize = Int32.MaxValue; // Default to max buffer sizes
                _currentClient.SendBufferSize = Int32.MaxValue;
                _currentStream = _currentClient.GetStream();
            }
            catch
            {
                _currentStream = null;
            }
        }
        public void SetBufferSizes(int sendBuffer, int receiveBuffer)
        {
            _currentClient.ReceiveBufferSize = receiveBuffer;
            _currentClient.SendBufferSize = sendBuffer;
        }
        public void Send<T>(string command,T messageData)
        {

            switch(Type)
            {
                case TransferType.Binary:
                    SendBinary(command, messageData);
                    break;
                case TransferType.JSON:
                    SendJSON(command, messageData);
                    break;
            }
        }
        private void SendBinary<T>(string command,T messageData)
        {
            var data = ObjectBinarySerialization.ObjectToByteArray(messageData);
            var header = new OTcpServerHeader(Commands.GetHashOfCommand(command), data.LongLength, (messageData).GetType().FullName);
            int headerSize = OTcpServerHeader.HeaderSize;
            _currentStream.Write(ObjectBinarySerialization.ObjectToByteArray(header), 0, headerSize);
            _currentStream.Write(data, 0, data.Length); // Write the bytes  
        }
        private void SendJSON<T>(string command, T messageData)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(messageData));
                var header = new OTcpServerHeader(Commands.GetHashOfCommand(command), data.LongLength,(messageData).GetType().FullName);
                int headerSize = OTcpServerHeader.HeaderSize;
                _currentStream.Write(ObjectBinarySerialization.ObjectToByteArray(header), 0, headerSize);
                _currentStream.Write(data, 0, data.Length); // Write the bytes  
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
        public OTcpServerMessage Receive()
        {
            while (!_currentStream.DataAvailable) ;

            switch(this.Type)
            {
                case TransferType.Binary:
                    return ReceiveBinary(ref _currentClient);
                case TransferType.JSON:
                    return ReceiveJSON(ref _currentClient);
                default:
                    return null;
            }
        }
        private OTcpServerHeader ReceiveHeader(ref TcpClient client)
        {
            int headerSize = OTcpServerHeader.HeaderSize;
            byte[] headerBytes = new byte[headerSize]; // Clear the message
                                                       // Receive the stream of bytes  
            try
            {
                _currentStream.Read(headerBytes, 0, headerSize);
                OTcpServerHeader header = (OTcpServerHeader)ObjectBinarySerialization.ByteArrayToObject(headerBytes);
                header.ExpectType = header.ExpectType.Split(' ')[0];
                return header;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }           
        }
        private OTcpServerMessage ReceiveBinary(ref TcpClient client)
        {
            var header = ReceiveHeader(ref client);
            var receivedMessage = new OTcpServerMessage();
            if (header == null)
            {
                // LOG
                return null;
            }

            receivedMessage.Header = header;
            byte[] messageBytes = new byte[header.DataSize]; // Clear the message   
            // Receive the stream of bytes  
            client.GetStream().Read(messageBytes, 0, messageBytes.Length);
            receivedMessage.Message = ObjectBinarySerialization.ByteArrayToObject(messageBytes);
            return receivedMessage;
        }
        private OTcpServerMessage ReceiveJSON(ref TcpClient client)
        {
            var header = ReceiveHeader(ref client);
            var dtype = StringTypeToTypeConverter.Convert(header.ExpectType);
            var receivedMessage = new OTcpServerMessage();
            if (header == null)
            {
                // LOG
                return null;
            }
            receivedMessage.Header = header;
            byte[] messageBytes = new byte[header.DataSize]; // Clear the message   
            // Receive the stream of bytes  
            client.GetStream().Read(messageBytes, 0, messageBytes.Length);

            var convertedType = StringTypeToTypeConverter.GetTypeFrom(header.ExpectType);
            receivedMessage.Message = Convert.ChangeType(JsonConvert.DeserializeObject(Encoding.UTF8.GetString(messageBytes), convertedType), convertedType);
            return receivedMessage;
        }
        public bool Connected()
        {
            return _currentClient.Connected;
        }
        public void Close()
        {
            string quitCmd = "quit";
            _currentStream.Write(Encoding.ASCII.GetBytes(quitCmd.ToCharArray()), 0, quitCmd.Length);
            _currentStream.Dispose();
            _currentClient.Close();
        }
        public static byte[] sendMessage(byte[] messageBytes, int port, string ipAddr)
        {
            const int bytesize = 1024 * 1024;
            try // Try connecting and send the message bytes  
            {
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(ipAddr, port); // Create a new connection  
                NetworkStream stream = client.GetStream();

                stream.Write(messageBytes, 0, messageBytes.Length); // Write the bytes  
                Console.WriteLine("================================");
                Console.WriteLine("=   Connected to the server    =");
                Console.WriteLine("================================");
                Console.WriteLine("Waiting for response...");

                messageBytes = new byte[bytesize]; // Clear the message   

                // Receive the stream of bytes  
                stream.Read(messageBytes, 0, messageBytes.Length);

                // Clean up  
                stream.Dispose();
                client.Close();
            }
            catch (Exception e) // Catch exceptions  
            {
                Console.WriteLine(e.Message);
            }

            return messageBytes; // Return response  
        }

    }
}
