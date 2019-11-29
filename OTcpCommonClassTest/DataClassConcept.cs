using System;
using System.Collections.Generic;

namespace OTcpCommonClassTest
{
    [Serializable]
    public class DataClassConcept : object
    {
        public string str;
        public int number;
        public List<string> strList = new List<string>();
    }
}
