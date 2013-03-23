using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
namespace LCmd
{
    public class CmdLineHandler
    {
        private MethodInfo method;
        CmdLineHandler()
        {
        }
        CmdLineHandler(MethodInfo m)
        {
            this.method = m;
        }
        public static bool ShowParameters = false;
        void handleArgs(object instance,string argsLine)
        {
            {
                var f = Assembly.GetEntryAssembly().Location.Replace(".exe", ".conf");
                if (File.Exists(f))
                {
                    argsLine = "-conf \"" + f.Replace("\\","\\\\") + "\" " + argsLine;
                }
            }
            {
                var f = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".conf");
                if (File.Exists(f))
                {
                    argsLine = "-conf \"" + f.Replace("\\", "\\\\") + "\" " + argsLine;
                }
            }
            var pDic = this.parseLine(argsLine);
            var pars = new List<object>();
            foreach (var par in this.method.GetParameters())
            {
                object obj = par.DefaultValue;
                if (pDic.ContainsKey(par.Name))
                {
                    try
                    {
                        obj = Convert.ChangeType(pDic[par.Name], par.ParameterType);
                    }
                    catch
                    {
                        throw (new CmdLineHandlerException(string.Format("Connot convert '{0}'({1}) into a '{2}'", pDic[par.Name], par.Name, par.ParameterType.ToString())));
                    }
                }
                pars.Add(obj);
            }
            if (pDic.ContainsKey("message"))
            {
                Console.WriteLine(pDic["message"]);
            }
            else
            {
                if (ShowParameters)
                {
                    Console.WriteLine("LCmd calling:" + this.method.Name + "({");
                    for (var i = 0; i < this.method.GetParameters().Length; i++)
                    {
                        var p = this.method.GetParameters()[i];
                        Console.WriteLine("\t" + p.Name + ":" + pars[i] ?? "null");
                    }
                    Console.WriteLine("});");
                }
            }
            try
            {
                this.method.Invoke(instance, pars.ToArray());
            }
            catch (TargetInvocationException ex)
            {
                throw (ex.InnerException);
            }
        }
        Dictionary<string, string> parseLine(string argsLine)
        {
            try
            {
                string[] args;
                {
                    var l = new List<string>();
                    var str = "";
                    var s = 0;
                    for (int i = 0; i < argsLine.Length; i++)
                    {
                        switch (argsLine[i])
                        {
                            case '"':
                                switch (s)
                                {
                                    case 0:
                                        str = "";
                                        s = 2;
                                        break;
                                    case 2:
                                        s = 0;
                                        l.Add(str);
                                        break;
                                    case 3:
                                        str += '"';
                                        s = 2;
                                        break;
                                    default:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                break;
                            case ' ':
                                switch (s)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        l.Add(str);
                                        s = 0;
                                        break;
                                    case 2:
                                        s = 2;
                                        str += ' ';
                                        break;
                                    default:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                break;
                            case '\t':
                                switch (s)
                                {
                                    case 0:
                                        break;
                                    case 1:
                                        l.Add(str);
                                        s = 0;
                                        break;
                                    case 2:
                                        s = 2;
                                        str += ' ';
                                        break;
                                    default:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                break;
                            case '\\':
                                switch (s)
                                {
                                    case 0:
                                        str = "\\";
                                        s = 1;
                                        break;
                                    case 1:
                                        str += '\\';
                                        break;
                                    case 2:
                                        s = 3;
                                        break;
                                    case 3:
                                        str += '\\';
                                        s = 2;
                                        break;
                                    default:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                break;
                            case 'n':
                                switch (s)
                                {
                                    case 0:
                                        str = "n";
                                        s = 1;
                                        break;
                                    case 1:
                                        str += 'n';
                                        break;
                                    case 2:
                                        str += 'n';
                                        break;
                                    case 3:
                                        str += "\r\n";
                                        s = 2;
                                        break;
                                    default:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                break;
                            default:
                                switch (s)
                                {
                                    case 0:
                                        s = 1;
                                        str = "";
                                        break;
                                    case 3:
                                        throw (new CmdLineHandlerException("Error parsing command.{0} {1}", s, argsLine[i]));
                                }
                                str += argsLine[i];
                                break;
                        }
                    }
                    if (s == 1)
                    {
                        l.Add(str);
                        s = 0;
                    }
                    if (s != 0)
                    {
                        throw (new CmdLineHandlerException("Error parsing command:unexpected end."));
                    }
                    args = l.ToArray();
                }
                if (args.Length % 2 != 0)
                {
                    throw (new CmdLineHandlerException("Error parsing command:unpaired args({0}).", argsLine));
                }
                var pDic = new Dictionary<string, string>();
                for (var i = 0; i < args.Length; i += 2)
                {
                    if (args[i][0] != '-')
                    {
                        throw (new CmdLineHandlerException("Error parsing command at '" + args[i] + "'"));
                    }
                    if (args[i] == "-conf")
                    {
                        var newDic = this.parseLine(System.IO.File.ReadAllText(args[i + 1],Encoding.Default).Replace("\r\n", " "));
                        foreach (var key in newDic.Keys)
                        {
                            pDic[key] = newDic[key];
                        }
                    }
                    else
                    {
                        pDic[args[i].Substring(1)] = args[i + 1];
                    }
                }
                return pDic;
            }
            catch (CmdLineHandlerException ex)
            {
                throw (new CmdLineHandlerException("Failed to call argLine:{0}\r\n{1}", argsLine, ex.Message));
            }
        }
        public static void HandleArgsLine(MethodInfo m, object instance, string argsLine)
        {
            var cmd = new CmdLineHandler(m);
            cmd.handleArgs(instance, argsLine);
        }
        public static bool SafeHandleArgsLine(MethodInfo m, object instance, string argsLine)
        {
            var cmd = new CmdLineHandler(m);
            try
            {
                cmd.handleArgs(instance, argsLine);
            }
            catch(CmdLineHandlerException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public static bool SafeHandleArgs(MethodInfo m, object instance, string[] args)
        {
            var argsLine = args.Any() ? args.Aggregate((str1, str2) => str1 + " " + str2) : "";
            return CmdLineHandler.SafeHandleArgsLine(m, instance, argsLine);
        }
    }
}
