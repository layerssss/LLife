using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
namespace LZip
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Write("Commands:");
                LCmd.CmdLineHandler.SafeHandleArgsLine(typeof(Program).GetMethod("Zip"), null, Console.ReadLine());
            }
            else
            {
                LCmd.CmdLineHandler.SafeHandleArgs(typeof(Program).GetMethod("Zip"), null, args);
            }
        }
        public static void Zip(string source,string target,bool compress=true,string password=null)
        {
            FastZip z = new FastZip();
            z.Password = password;
            target = string.Format(target, DateTime.Now);
            source = string.Format(source, DateTime.Now);
            if (compress)
            {
                System.IO.File.Delete(target);
                z.CreateZip(
                    target,
                    source, true, "", "");
            }
            else
            {
                z.ExtractZip(
                    source,
                    target, "");
            }
        }
    }
}
