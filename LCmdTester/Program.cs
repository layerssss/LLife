using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCmdTester
{
    class Program
    {
        static void Main()
        {
            while (true)
            {
                var type = typeof(DummyDll.Class1);
                var instance = new DummyDll.Class1();
                LCmd.CmdLineHandler.SafeHandleArgsLine(type.GetMethod("hello"), instance, Console.ReadLine());
            }
        }
    }
}
