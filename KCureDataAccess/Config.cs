using Microsoft.VisualBasic.Logging;
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
        public string dapperConnStr { get; set; }

        public Config()
        {
            webRoot = @"D:/workspaces/vs/ProjectDataAccess/KCureDataAccess/web/";
            // webRoot = @"C:/Users/User/Workspaces/visualStudio/ProjectDataAccess/KCureDataAccess/web/";
            //
            // dapperConnStr = "Server=127.0.0.1;Port=5433;User Id=gony;Password=asdf;Database=kcure;";
            dapperConnStr = "Server=127.0.0.1;Port=5432;User Id=gony;Password=asdf;Database=kcure;";
            // dapperConnStr = "Server=127.0.0.1;Port=5432;User Id=postgres;Database=postgres;";
        }
    }
}
