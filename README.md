# ObjectTCP
ObjectTCP Library for .NET Platform written in .NET Core to support cross-platforms.
OBJECT TCP IS STILL AN ONGOING PROJECT USAGES, NAMES, METHODS ARE SUBJECT TO CHANGE

Allows you to send and receive .NET objects between OTCP Client and OTCP Server. This makes it easier to integrate other .NET applications with .NET Core. 

Currently supports JSON and Binary transfer modes. Binary mode is not cross platform ready.
JSON mode may not support all .NET generic data types.

# Convention
##**Your Object**
DataClassConcept class represent your own class to send, this class must be known to both Client and Server.
Add Serializable attribute to your class.
```c#
    [Serializable]
    public class DataClassConcept
    {
        public string str;
        public int number;
        public List<string> strList = new List<string>();
    }
```
##**Server**
```c#
using OTcpCoreComponents;
using OTcpServerCore;

            OTcpServerCommands cmds = new OTcpServerCommands();
            cmds.AddCommand("commandName", commandFunction);
            OTcpServer server = new OTcpServer();
            server.StartServer(cmds,"127.0.0.1", 12345, TransferType.JSON);
```

###**Define Response Function**

commandFunction return parameter represent the returned object to the client. Void will result in no response to the client.
commandFunction argument is always one parameter in the type of "object" and needs to be cast to the correct class before use.
```c#
        private static DataClassConcept commandFunction(object obj)
        {
            var inp = (DataClassConcept)obj;
            var dc = new DataClassConcept();
            dc.str = "1";
            dc.number = 2;
            dc.strList.Add("3");
            return dc;
        }
```
##**Client**

Returned message data type can be accessed in runtime via the Header. Object returned must be cast to the correct class.
```c#
using OTcpClientCore;
using OTcpCoreComponents;

            OTcpClientCommands cmds = new OTcpClientCommands();
            cmds.AddCommand("commandName");
            OTcpClient client = new OTcpClient(cmds,"127.0.0.1",12345,TransferType.JSON);
            DataClassConcept dcsend = new DataClassConcept();
            client.Send("commandName", dcsend);
            var msg = client.Receive();
            DataClassConcept dc = (DataClassConcept)msg.Message;
```
