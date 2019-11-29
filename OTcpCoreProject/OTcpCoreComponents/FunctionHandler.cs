using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace OTcpCoreComponents
{
    public class FunctionHandler
    {
        public string Command;
        public byte[] CommandHash;
        public FunctionHandlerType HandlerType;
        public readonly bool ExpectResponse;
        public delegate object ObjInObjOutHandlerDelegate(object obj);
        public delegate void ObjInVoidOutHandlerDelegate(object obj);
        public delegate object VoidInObjOutHandlerDelegate();
        public delegate void VoidInVoidOutHandlerDelegate();

        //public delegate TU ObjInObjOutHandlerDelegate<TU>(TU obj);
        //public delegate void ObjInVoidOutHandlerDelegate<TU>(TU obj);
        //public delegate TU VoidInObjOutHandlerDelegate<TU>();
        //public delegate void VoidInVoidOutHandlerDelegate();

        public ObjInObjOutHandlerDelegate ObjInObjOutHandler;
        public ObjInVoidOutHandlerDelegate ObjInVoidOutHandler;
        public VoidInObjOutHandlerDelegate VoidInObjOutHandler;
        public VoidInVoidOutHandlerDelegate VoidInVoidOutHandler;

        //public T Handler;




        public FunctionHandler(byte[] commandHash, string command, FunctionHandlerType type, ObjInObjOutHandlerDelegate handler)
        {
            Command = command;
            CommandHash = commandHash;
            HandlerType = type;
            ObjInObjOutHandler = handler;
            ExpectResponse = true;
        }
        public FunctionHandler(byte[] commandHash, string command, FunctionHandlerType type, ObjInVoidOutHandlerDelegate handler)
        {
            Command = command;
            CommandHash = commandHash;
            HandlerType = type;
            ObjInVoidOutHandler = handler;
            ExpectResponse = false;
        }
        public FunctionHandler(byte[] commandHash, string command, FunctionHandlerType type, VoidInObjOutHandlerDelegate handler)
        {
            Command = command;
            CommandHash = commandHash;
            HandlerType = type;
            VoidInObjOutHandler = handler;
            ExpectResponse = true;
        }
        public FunctionHandler(byte[] commandHash, string command, FunctionHandlerType type, VoidInVoidOutHandlerDelegate handler)
        {
            Command = command;
            CommandHash = commandHash;
            HandlerType = type;
            VoidInVoidOutHandler = handler;
            ExpectResponse = false;
        }
        public object CallDuplex(object obj)
        {
            return ObjInObjOutHandler(obj);
        }

        public void CallArg(object obj)
        {
            ObjInVoidOutHandler(obj);
        }

        public void Call()
        {
            VoidInVoidOutHandler();
        }

        public object CallExpect()
        {
            return VoidInObjOutHandler();
        }
    }

}
