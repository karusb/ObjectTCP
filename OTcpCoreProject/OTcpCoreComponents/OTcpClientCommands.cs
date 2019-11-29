using System;
using System.Collections.Generic;
using System.Text;

namespace OTcpCoreComponents
{
    public class OTcpClientCommands : OTcpCommands
    {
        public void AddCommand(string command)
        {
            AddCommandHash(command);
        }
    }
}
