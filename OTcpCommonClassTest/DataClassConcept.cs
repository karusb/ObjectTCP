using System;
using System.Collections.Generic;

namespace OTcpCommonClassTest
{
    [Serializable]
    public class DataClassConcept
    {
        public string str;
        public int number;
        public List<string> strList = new List<string>();
    }
}
