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
        public List<Dictionary<string, object>> FilterSelect(
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

    }
}
