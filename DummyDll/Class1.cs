using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DummyDll
{
    public class Class1
    {
        public void hello(string m1,int m2,string p1="ddd",int p2=2,bool p3=false)
        {
            Console.WriteLine("{0}   {1}    {2}", p1, p2, p3);
        }
    }
}
