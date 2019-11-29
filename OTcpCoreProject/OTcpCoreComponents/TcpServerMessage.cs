using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace OTcpCoreComponents
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class OTcpServerMessage
    {
        public OTcpServerHeader Header;
        public object Message;
        public OTcpServerMessage(OTcpServerHeader header, object message)
        {
            Header = header;
            Message = message;
        }
        public OTcpServerMessage()
        {

        }
    }
}
