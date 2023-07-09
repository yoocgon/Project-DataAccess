
using System.Text.Json.Nodes;
using System.Text.Json;
using Renci.SshNet;
using Microsoft.VisualBasic.Logging;
using System.Reflection.Metadata;
using System.Security.Policy;

namespace KCureDataAccess
{
    public class Controller
    {
        //
        Store store;
        //
        Observer observer;
        Config config;
        //
        CustomSftpClient sftpReqeustedDataClient;
        CustomSftpClient sftpInsertedDataClient;
        CustomSftpClient sftpInserteRequestDataClient;
        //
        CustomHttpClient httpKcureClient;
        //
        CustomDapperClient dapperClient;
        CustomPostgresClient postgresClient;
        //
        Test test = new Test();

        public Controller(Observer observer, Config config)
        {
            this.observer = observer;
            this.observer.Add(this);
            //
            this.config = config;
            //
            dapperClient = new CustomDapperClient(config);
            //
            // PrivateKeyFile? kcureSvr = new PrivateKeyFile("");
            // sftpDataReqeusted = new Sftp("100.100.100.1", 22, "centos", "", kcureSvr);
            // sftpDataImported = new Sftp("100.100.100.2", 22, "centos", "", kcureSvr);
            // httpKcureClient = new CustomHttpClient();
            //
            // test.test001(dapperClient);
        }

        public void Parse(string strJson)
        {
            var objJson = JsonSerializer.Deserialize<JsonObject>(strJson);
            if (objJson == null)
                return;
            //
            string? page = objJson["page"]?.GetValue<string>();
            string? action = objJson["action"]?.GetValue<string>();
            //
            if (action == "page")
            {
                if (page == "login")
                {
                    var objData = objJson["data"];
                    if (objData == null)
                        return;
                    //
                    string? id = objData["id"]?.GetValue<string>();
                    string? password = objData["password"]?.GetValue<string>();
                    //
                    Console.WriteLine("Debug>>> (id) : " + id);
                    Console.WriteLine("Debug>>> (password) : " + password);
                    //
                    if (id == null || password == null)
                        return;

                    store = new Store();
                    if (Login(id, password))
                    {
                        store.id = id;
                        observer.Send("formMain", "page", "02-index", null);
                    }
                }
                else
                {
                    string? dest = objJson["dest"]?.GetValue<string>();
                    if (dest == null)
                        return;

                    observer.Send("formMain", "page", dest, null);
                }
            }
            else if (action == "api")
            {
                var objData = objJson?["data"];
                if (objData == null)
                    return;
                //
                List<Dictionary<string, string>> listDicWhereCondition = new List<Dictionary<string, string>>();
                if (objData is JsonObject dataObject)
                {
                    foreach (var keyValuePair in dataObject)
                    {
                        Dictionary<string, string> dicWhereCondition = new Dictionary<string, string>();
                        string keyCamel = keyValuePair.Key;
                        string keySnake = CamelToSnake(keyCamel);
                        JsonNode? value = keyValuePair.Value;
                        string? strValue = value?.GetValue<string>();
                        if (strValue == null)
                            return;
                        //
                        Console.WriteLine($"\nDEBUG>>> (Key) {keySnake} (Value) {value}");
                        //
                        if (keyCamel == "importedDataExists" || keyCamel == "dataExportRequestExists" || keyCamel == "dataExportApprovalExistence")
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
                }
                else if (objData is JsonArray dataArray)
                {
                    Console.WriteLine("\nDEBUG>>> (exception) objData is a JsonArray");
                }
                //
                List<Dictionary<string, object>> listDicQueryResult = dapperClient.FilterSelect("tb_test", "*", listDicWhereCondition);
                List<Dictionary<string, object>> listDicTemp = SnakeKeyToCamelKey(listDicQueryResult);
                List<Dictionary<string, object>> listDicData = PostSelectFilter(listDicTemp);
                //
                // List<Dictionary<string, object>> listDicData = new List<Dictionary<string, object>>();
                // Dictionary<string, object> dicData1 = new Dictionary<string, object>();
                // dicData1.Add("id", "1");
                // dicData1.Add("researchSubject", "개발 테스트 연구");
                // dicData1.Add("department", "데이터스트림즈");
                // dicData1.Add("researcher", "유창곤");
                // dicData1.Add("applyStep", "1단계?");
                // dicData1.Add("applyStatus", "심사중");
                // dicData1.Add("importedDataExists", "Y");
                // dicData1.Add("dataExportRequestExists", "Y");
                // dicData1.Add("dataExportApprovalExistence", "Y");
                // dicData1.Add("dataUtilizationStartDate", "2023-06-01 00:00:00");
                // dicData1.Add("dataUtilizationEndDate", "2023-06-01 00:00:00");
                // listDicData.Add(dicData1);
                //
                // Dictionary<string, object> dicData2 = new Dictionary<string, object>();
                // dicData2.Add("id", "2");
                // dicData2.Add("researchSubject", "개발 테스트 연구");
                // dicData2.Add("department", "데이터스트림즈");
                // dicData2.Add("researcher", "유창곤");
                // dicData2.Add("applyStep", "1단계?");
                // dicData2.Add("applyStatus", "심사중");
                // dicData2.Add("importedDataExists", "Y");
                // dicData2.Add("dataExportRequestExists", "Y");
                // dicData2.Add("dataExportApprovalExistence", "Y");
                // dicData2.Add("dataUtilizationStartDate", "2023-06-01 00:00:00");
                // dicData2.Add("dataUtilizationEndDate", "2023-06-01 00:00:00");
                // listDicData.Add(dicData2);
                //
                observer.Send("formMain", "api", "", listDicData);
            }
        }


        public bool Login(string id, string password)
        {
            return true;
        }


        public void Listener(string target, string action, string message, dynamic data)
        {
            Console.WriteLine("\nDebug>>> Controller Listener");
            Console.WriteLine("(target) " + target);
            Console.WriteLine("(action) " + action);
            Console.WriteLine("(message) " + message);
        }

        public string CamelToSnake(string camelCase)
        {
            return string.Concat(camelCase.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }
            
        public List<Dictionary<string, object>> SnakeKeyToCamelKey(List<Dictionary<string, object>> listDicData)
        {
            List<Dictionary<string, object>> listDicCamelKey = new List<Dictionary<string, object>>();
            //
            if (listDicData != null)
            {
                foreach (var dic in listDicData)
                {
                    var newDic = new Dictionary<string, object>();
                    foreach (var key in dic.Keys)
                    {
                        var newKey = string.Concat(key.Split('_').Select((x, i) => i > 0 ? char.ToUpper(x[0]) + x.Substring(1) : x));
                        newDic.Add(newKey, dic[key]);
                    }
                    // listDicData[listDicData.IndexOf(dic)] = newDic;
                    listDicCamelKey.Add(newDic);
                }
            }
            //
            return listDicCamelKey;
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
    }
}
