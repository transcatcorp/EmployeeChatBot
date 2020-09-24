using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace URMC.ActiveDirectory
{
    public class ActiveDirectoryOptions
    {
        public string URL { get; set; }
        public int Port { get; set; }
        public string DirectoryClasses { get; set; }
    }
}
