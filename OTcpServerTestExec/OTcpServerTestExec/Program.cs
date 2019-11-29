using System;
using System.Collections.Generic;
using OTcpCommonClassTest;
using OTcpCoreComponents;
using OTcpServerCore;
namespace TcpServerTestExec
{

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            OTcpServerCommands cmds = new OTcpServerCommands();
            cmds.AddCommand("osman", keke);
            OTcpServer server = new OTcpServer();
            server.StartServer(cmds,"127.0.0.1", 12345, TransferType.JSON);
            //server.AddCommand("osman", keke);
            //Non responding void functions
            //server.AddCommand("nene", nene);
            // Non responding data functions
            // Non responding basic data types
            //Basic data types
            //server.AddCommand("dede", integerRT);
            //
            server.Listen();
        }
        private static DataClassConcept keke(object obj)
        {
            var inp = (DataClassConcept)obj;
            Console.WriteLine(inp.number);
            Console.WriteLine(inp.str);
            Console.WriteLine(inp.strList[0]);
            var dc = new DataClassConcept();
            dc.str = "1";
            dc.number = 2;
            dc.strList.Add("3");
            return dc;
        }
        private void nene()
        {

        }
        private void dede(object obj)
        {

        }
        private int integerRT(int i)
        {
            return 0;
        }
    }
}
