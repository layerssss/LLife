using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCmd
{
    public class CmdLineHandlerException:Exception
    {
        public CmdLineHandlerException(string message,params object[] objs):base(string.Format(message,objs))
        {
        }
        public override string Message
        {
            get
            {
                return base.Message;
            }
        }
    }
}
