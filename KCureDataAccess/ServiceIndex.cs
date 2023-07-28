using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace KCureDataAccess
{
    public class ServiceIndex
    {
        public CustomDapperClient DapperClient { get; set; }
        public CustomLibrary Library { get; set; }

        public List<Dictionary<string, object>> GetUsers(JsonObject dataObject, Store store)
        {
            List<Dictionary<string, string>> listDicWhereCondition = new List<Dictionary<string, string>>();
            //
            foreach (var keyValuePair in dataObject)
            {
                Dictionary<string, string> dicWhereCondition = new Dictionary<string, string>();
                string keyCamel = keyValuePair.Key;
                string keySnake = Library.CamelToSnake(keyCamel);
                JsonNode? value = keyValuePair.Value;
                string? strValue = value?.GetValue<string>();
                if (strValue == null)
                    return null;
                //
                Console.WriteLine($"\nDEBUG>>> (Key) {keySnake} (Value) {value}");
                //
                if (keyCamel == "researcher")
                {
                    strValue = store.id;
                    dicWhereCondition.Add("type", "text");
                    dicWhereCondition.Add("expression", $" AND {keySnake} = @{keySnake}");
                }
                else if (keyCamel == "importedDataExists" || keyCamel == "dataExportRequestExists" || keyCamel == "dataExportApprovalExistence")
                {
                    dicWhereCondition.Add("type", "bool");
                    dicWhereCondition.Add("expression", $" AND {keySnake} = @{keySnake}");
                }
                else if (keyCamel == "dataUtilizationStartDate")
                {
                    dicWhereCondition.Add("type", "timestamp");
                    dicWhereCondition.Add("expression", $" AND {keySnake} >= @{keySnake}");
                    if (strValue == "1970-1-1")
                        strValue = "-";
                }
                else if (keyCamel == "dataUtilizationEndDate")
                {
                    dicWhereCondition.Add("type", "timestamp");
                    dicWhereCondition.Add("expression", $" AND {keySnake} <= @{keySnake}");
                    if (strValue == "1970-1-1")
                        strValue = "-";
                }
                else
                {
                    dicWhereCondition.Add("type", "text");
                    dicWhereCondition.Add("expression", $" AND {keySnake} = @{keySnake}");
                }
                //
                dicWhereCondition.Add("variable", keySnake);
                dicWhereCondition.Add("value", strValue);
                dicWhereCondition.Add("exception", "-");
                //
                listDicWhereCondition.Add(dicWhereCondition);
            }
            //
            List<Dictionary<string, object>> listDicQueryResult = DapperClient.FilterSelect("tb_test", "*", listDicWhereCondition);
            List<Dictionary<string, object>> listDicTemp = Library.SnakeKeyToCamelKey(listDicQueryResult);
            List<Dictionary<string, object>> listDicData = DapperClient.PostSelectFilter(listDicTemp);
            //
            return listDicData;
        }
    }
}
