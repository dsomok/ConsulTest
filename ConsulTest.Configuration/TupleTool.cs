using System;
using System.Collections.Generic;
using System.Text;

namespace ConsulTest.Configuration
{
    public class TupleTool
    {
        public static (string, string) GetTuple()
        {
            return ("Key", "Value");
        }
    }
}
