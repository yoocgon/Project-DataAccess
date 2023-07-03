using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCureDataAccess
{
    public class Config
    {
        public string webRoot { get; set; }


        public Config()
        {
            webRoot = @"D:/workspaces/vs/ProjectDataAccess/KCureDataAccess/web/";
        }
    }
}
