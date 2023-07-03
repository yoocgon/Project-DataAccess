
using Npgsql;
using Renci.SshNet;
using System.Transactions;

namespace KCureDataAccess
{
    public class CustomPostgresClient
    {
        private string host;
        private int port;
        private string user;
        private string password;
        private string database;
        private NpgsqlConnection conn;

        public CustomPostgresClient(string host, int port, string user, string password, string database)
        {
            string strConn = string.Format(
                "HOST={0};PORT={1};USERNAME={2};PASSWORD={3};DATABASE={4}",
                host, port, user, password, database);
            //
            using (this.conn = new NpgsqlConnection(strConn))
            {
                this.conn.Open();
            }
        }

        //query = "INSERT INTO 테이블명 VALUES(@A, @B, @C, @D)"
        public void Insert(string query, Dictionary<string, string> dicParam)
        {
            NpgsqlTransaction transaction = conn.BeginTransaction();
            //
            try
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = query;
                    foreach (var item in dicParam)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }
                    command.ExecuteNonQuery();
                }
                //
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                //AppendText(txtDB, "에러입니다");
            }
            finally 
            { 
                // conn.Close(); 
            }
        }

        // query = "SELECT * FROM 테이블 명 ORDER BY  컬럼명 DESC "
        public void Select(string query, Dictionary<string, string> dicParam, string[] aryColumn)
        {
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            //
            try
            {
                using (var command = new NpgsqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = query;
                    foreach (var item in dicParam)
                    {
                        command.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var row = new Dictionary<string, string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                string columnValue = reader[i].ToString();
                                row[columnName] = columnValue;
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //AppendText(txtDB, "에러입니다");
            }
        }
    }
}
