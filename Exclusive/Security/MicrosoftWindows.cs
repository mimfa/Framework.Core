using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.Security
{
   public class MicrosoftWindows
    {
        public static bool IsAuthenticated(string usr, string pwd)
        {
            bool authenticated = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(Environment.SystemDirectory, usr, pwd);
                object nativeObject = entry.NativeObject;
                authenticated = true;
            }
            catch { }

            return authenticated;
        }
        public static bool IsValidActiveDirectoryUser(string activeDirectoryServerDomain, string username, string password)
        {
            try
            {
                DirectoryEntry de = new DirectoryEntry("LDAP://" + activeDirectoryServerDomain, username + "@" + activeDirectoryServerDomain, password, AuthenticationTypes.Secure);
                DirectorySearcher ds = new DirectorySearcher(de);
                ds.FindOne();
                return true;
            }
            catch //(Exception ex)
            {
                return false;
            }
        }
    }
}
