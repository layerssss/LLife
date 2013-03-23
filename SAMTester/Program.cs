using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.DirectoryServices;
namespace SAMTester
{
    class Program
    {
        static void Main(string[] args)
        {

            object obUsers = m_group.Invoke("Members");
            foreach (object ob in (IEnumerable)obUsers)
            {
                // Create object for each user.
                DirectoryEntry obUserEntry = new DirectoryEntry(ob);
                winuser = new WinUser(obUserEntry.Name);
                winuser.Description = obUserEntry.Properties["Description"].Value.ToString();
                winusercollection.Add(winuser);
            }
        }
    }
}
