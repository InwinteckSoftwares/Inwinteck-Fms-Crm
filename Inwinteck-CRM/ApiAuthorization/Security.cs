using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Inwinteck_CRM
{
    public class Security
    {
        
        public static bool SourceLogin(string username, string password)
        {
            if (username == "sourcesupport_user@#24" && password == "PGoiGJ#!&fXhzSgB0V9T7pTe9")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}