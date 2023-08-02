using Npgsql;
using Dapper;

namespace KCureDataAccess
{
    public class CustomDapperClient
    {
        private Config config;

        public CustomDapperClient(Config config) { 
            this.config = config;
        }

        //
        //string query = "" +
        //    "SELECT * " +
        //    "FROM tb_test " +
        //    "WHERE department = @department " +
        //    "AND researcher = @researcher" +
        //    "AND researcher = @applyStep" +
        //    "AND researcher = @applyStatus" +
        //    "AND researcher = @importedDataExists" +
        //    "AND researcher = @dataExportRequestExists" +
        //    "AND researcher = @dataExportApprovalExistence";
        //
        //var parameter = new
        //{
        //    department = department,
        //    researcher = researcher,
        //    applyStep = applyStep,
        //    applyStatus = applyStatus,
        //    importedDataExists = importedDataExists,
        //    dataExportRequestExists = dataExportRequestExists,
        //    dataExportApprovalExistence = dataExportApprovalExistence
        //};
        //
        // List<Dictionary<string, object>> listDicData = dapperClient.Select(query, parameter);
        public List<Dictionary<string, object>> Select(String query, object parameters)
        {
            using (var connection = new NpgsqlConnection(config.dapperConnStr))
            {
                connection.Open();
                dynamic result;
                if(parameters == null)
                { 
                    result = connection.Query<dynamic>(query);
                }
                else
                {
                    result = connection.Query(query, parameters).ToList();
                }
                //
                List<Dictionary<string, object>> listResult = new List<Dictionary<string, object>>();
                foreach (var row in result)
                {
                    var dictionary = new Dictionary<string, object>();
                    foreach (var column in row)
                    {
                        dictionary.Add(column.Key, column.Value);
                    }
                    listResult.Add(dictionary);
                }
                //
                Console.WriteLine("\nDebug>>> (query result) ");
                foreach (var row in listResult)
                {
                    Console.WriteLine("\nDebug>>> (row) ");
                    foreach (var kvp in row)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                }
                //
                connection.Close();
                //
                return listResult;
            }
        }

        // 쿼리식, 값, contition 
        public List<Dictionary<string, object>> SelectUsingFilter(
            string table, 
            string conditionSelect,
            List<Dictionary<string, string>> conditionsWhere) {
            //
            using (var connection = new NpgsqlConnection(config.dapperConnStr))
            {
                connection.Open();
                //
                DynamicParameters dynamicParameters = new DynamicParameters();
                //
                string strWhere = "";
                foreach(Dictionary<string, string> dicCondition in conditionsWhere)
                {
                    if (dicCondition["value"] == dicCondition["exception"])
                        continue;
                    //
                    if (dicCondition["type"] == "text")
                    {
                        strWhere += dicCondition["expression"];
                        dynamicParameters.Add("@" + dicCondition["variable"], dicCondition["value"], System.Data.DbType.String);
                    }
                    else if (dicCondition["type"] == "number")
                    {
                        strWhere += dicCondition["expression"];
                        dynamicParameters.Add("@" + dicCondition["variable"], dicCondition["value"], System.Data.DbType.Int64);
                    }
                    else if (dicCondition["type"] == "bool")
                    {
                        strWhere += dicCondition["expression"];
                        object objValue = false;
                        if (dicCondition["value"] == "Y")
                            objValue = true;
                        //
                        dynamicParameters.Add("@" + dicCondition["variable"], objValue, System.Data.DbType.Boolean);
                    }
                    else if (dicCondition["type"] == "timestamp")
                    {
                        strWhere += dicCondition["expression"];
                        DateTime dateTime = DateTime.Parse(dicCondition["value"]);
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        dynamicParameters.Add("@" + dicCondition["variable"], dateTime, System.Data.DbType.DateTime);
                    }
                }
                //
                string query = $"SELECT {conditionSelect} FROM {table} WHERE 1=1 {strWhere}";
                Console.WriteLine("\nDebug>>> ({0}) {1}", "query", query);
                //
                dynamic result = connection.Query(query, dynamicParameters).ToList();
                List<Dictionary<string, object>> listResult = new List<Dictionary<string, object>>();
                foreach (var row in result)
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (var column in row)
                    {
                        dictionary.Add(column.Key, column.Value);
                    }
                    listResult.Add(dictionary);
                }
                //
                Console.WriteLine("\nDebug>>> (query result) ");
                foreach (var row in listResult)
                {
                    Console.WriteLine("\nDebug>>> (row) ");
                    foreach (var kvp in row)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                }
                //
                connection.Close();
                //
                return listResult;
            }
        }

        public List<Dictionary<string, object>> PostSelectFilter(List<Dictionary<string, object>> listDicData)
        {
            List<Dictionary<string, object>> listDicPostData = new List<Dictionary<string, object>>();
            foreach (var dic in listDicData)
            {
                var newDic = new Dictionary<string, object>();
                foreach (var item in dic)
                {
                    string newValue = "";
                    if (item.Value == null)
                        newValue = "";
                    else if (item.Value.GetType().Name == "Boolean" && (bool)item.Value)
                        newValue = "Y";
                    else if (item.Value.GetType().Name == "Boolean" && !(bool)item.Value)
                        newValue = "N";
                    else
                        newValue = item.Value.ToString();
                    //
                    newDic.Add(item.Key, newValue);
                }
                //
                listDicPostData.Add(newDic);
            }
            //
            return listDicPostData;
        }

        // 쿼리식, 값, contition 
        public List<Dictionary<string, object>> GetPostgresTableColumnInfo(string schema, string table)
        {
            //
            //string query = $"""
            //    SELECT b.table_name, b.column_name, b.data_type, a.column_comment 
            //    FROM (
            //     (SELECT TABLE_NAME, column_name, data_type
            //     FROM INFORMATION_SCHEMA.COLUMNS
            //     WHERE TABLE_CATALOG = '{schema}'
            //       AND TABLE_NAME = '{table}'
            //       AND table_schema = 'public'
            //     ORDER BY ORDINAL_POSITION) as b 
            //    left JOIN
            //     (SELECT PS.RELNAME AS TABLE_NAME, PA.ATTNAME AS COLUMN_NAME, PD.DESCRIPTION AS COLUMN_COMMENT
            //      FROM PG_STAT_ALL_TABLES PS, PG_DESCRIPTION PD, PG_ATTRIBUTE PA
            //      WHERE PS.SCHEMANAME = (SELECT SCHEMANAME FROM PG_STAT_USER_TABLES WHERE RELNAME = '{table}')
            //       AND PS.RELNAME = '{table}'
            //       AND PS.RELID = PD.OBJOID
            //       AND PD.OBJSUBID <> 0
            //       AND PD.OBJOID = PA.ATTRELID
            //       AND PD.OBJSUBID = PA.ATTNUM
            //     ORDER BY PS.RELNAME, PD.OBJSUBID) as a
            //    ON a.column_name = b.column_name)""";
            //
            string query = @"
                SELECT b.table_name, b.column_name, b.data_type, a.column_comment 
                FROM (
                 (SELECT TABLE_NAME, column_name, data_type
                 FROM INFORMATION_SCHEMA.COLUMNS
                 WHERE TABLE_CATALOG = '{schema}'
                   AND TABLE_NAME = '{table}'
                   AND table_schema = 'public'
                 ORDER BY ORDINAL_POSITION) as b 
                left JOIN
                 (SELECT PS.RELNAME AS TABLE_NAME, PA.ATTNAME AS COLUMN_NAME, PD.DESCRIPTION AS COLUMN_COMMENT
                  FROM PG_STAT_ALL_TABLES PS, PG_DESCRIPTION PD, PG_ATTRIBUTE PA
                  WHERE PS.SCHEMANAME = (SELECT SCHEMANAME FROM PG_STAT_USER_TABLES WHERE RELNAME = '{table}')
                   AND PS.RELNAME = '{table}'
                   AND PS.RELID = PD.OBJOID
                   AND PD.OBJSUBID <> 0
                   AND PD.OBJOID = PA.ATTRELID
                   AND PD.OBJSUBID = PA.ATTNUM
                 ORDER BY PS.RELNAME, PD.OBJSUBID) as a
                ON a.column_name = b.column_name)";
            //
            query = query.Replace("{schema}", schema);
            query = query.Replace("{table}", table);
            //
            using (var connection = new NpgsqlConnection(config.dapperConnStr))
            {
                connection.Open();
                dynamic result = connection.Query(query).ToList();
                List<Dictionary<string, object>> listResult = new List<Dictionary<string, object>>();
                foreach (var row in result)
                {
                    Dictionary<string, object> dictionary = new Dictionary<string, object>();
                    foreach (var column in row)
                    {
                        dictionary.Add(column.Key, column.Value);
                    }
                    listResult.Add(dictionary);
                }
                //
                Console.WriteLine("\nDebug>>> (query result) ");
                foreach (var row in listResult)
                {
                    Console.WriteLine("\nDebug>>> (row) ");
                    foreach (var kvp in row)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                }
                //
                connection.Close();
                //
                return listResult;
            }
        }

        //
        public void SetColumnFilterValues(List<Dictionary<string, object>> columns, string schema, string table)
        {
            if (columns == null)
                return;
            //
            foreach (var column in columns)
            {
                List<object> values = SelectDistinctColumns(schema, table, (string) column["column_name"]);
                column.Add("values", values);
            }
        }

        //
        public List<object> SelectDistinctColumns(string schema, string table, string column)
        {
            //
            string query = $"SELECT distinct {column} FROM {table}";
            //
            using (var connection = new NpgsqlConnection(config.dapperConnStr))
            {
                connection.Open();
                List<dynamic> result = connection.Query(query).ToList();
                List<object> listResult = new List<object>();
                foreach (var item in result)
                    listResult.Add(item);
                //
                connection.Close();
                return listResult;
            }
        }
    }
}
