using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
namespace BatchesCaller
{
    class Program
    {
        static void Main(string[] args)
        {
            LCmd.CmdLineHandler.SafeHandleArgs(typeof(Program).GetMethod("Call"), null, args);
        }
        public static void Call(
            string root = ".\\",
            string prefix = "",
            string search="*.cmd",
            string tmpFilename="BatchesCallerTmp.cmd"
            )
        {
            var dic = new Dictionary<char, string>();
            foreach (var file in Directory.GetFiles(root,"*.cmd"))
            {
                var f=file.Substring(file.LastIndexOf('\\')+1);
                if (f.StartsWith(prefix))
                {
                    for (var c = f[prefix.Length]; ; c++)
                    {
                        if (!dic.ContainsKey(c))
                        {
                            var cc = char.Parse(c.ToString().ToLower());
                            dic.Add(cc, file);
                            Console.Write("{0}={1}\t", cc, f);
                            break;
                        }
                    }
                }
            }
            Console.WriteLine();
            var cmd = "@echo off\r\n";
            while (true)
            {
                try
                {
                    var str = Console.ReadLine();
                    foreach (var c in str)
                    {
                        if (c == ' ')
                        {
                            continue;
                        }
                        var cc = char.Parse(c.ToString().ToLower());
                        if (!dic.ContainsKey(cc))
                        {
                            throw (new Exception(cc + " is not avalable!(" + dic.Keys.Aggregate("", (str1, c2) => str1 +","+ c2)));
                        }
                        cmd += "call \"" + dic[cc] + "\"\r\n";
                    }
                    break;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
            }
            cmd += "echo BatchesCaller运行完毕\r\n";
            cmd += "pause\r\n";
            
            File.WriteAllText(tmpFilename, cmd,Encoding.Default);
            Process.Start(tmpFilename).WaitForExit();
            File.Delete(tmpFilename);
            
        }
    }
}
