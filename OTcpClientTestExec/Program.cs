using System;
using System.Collections.Generic;
using OTcpClientCore;
using OTcpCommonClassTest;
using OTcpCoreComponents;


namespace TcpClientTestExec
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            OTcpClientCommands cmds = new OTcpClientCommands();
            cmds.AddCommand("osman");
            OTcpClient client = new OTcpClient(cmds,"127.0.0.1",12345,TransferType.JSON);
            DataClassConcept dcsend = new DataClassConcept();
            client.SetBufferSizes(int.MaxValue, int.MaxValue);
            dcsend.number = 11;
            dcsend.str = "2012";
            dcsend.strList.Add("kantikniet");
            client.Send("osman", dcsend);
            var msg = client.Receive();
            DataClassConcept dc = (DataClassConcept)msg.Message;
           
            Console.WriteLine(dc.number);
            Console.WriteLine(dc.str);
            Console.WriteLine(dc.strList[0]);
            client.Send("osman", dcsend);
            var msg2 = client.Receive();
            DataClassConcept dc2 = (DataClassConcept)msg2.Message;
            Console.WriteLine(dc2.number);
            Console.WriteLine(dc2.str);
            Console.WriteLine(dc2.strList[0]);
            client.Close();
        }
    }
}
