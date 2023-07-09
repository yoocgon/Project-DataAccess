using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCureDataAccess
{
    public class Test
    {
        public void test001(CustomDapperClient client) {
            try
            {
                string query = "SELECT * AS TABLE_NAME FROM PG_STAT_USER_TABLES";
                client.Select(query, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
