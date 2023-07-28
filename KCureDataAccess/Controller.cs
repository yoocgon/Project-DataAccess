
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
        CustomLibrary library;
        //
        Test test = new Test();
        //
        ServiceIndex serviceIndex;

        public Controller(Observer observer, Config config)
        {
            this.observer = observer;
            this.observer.Add(this);
            //
            this.config = config;
            this.library = new CustomLibrary(); 
            //
            this.dapperClient = new CustomDapperClient(config);
            //
            this.serviceIndex = new ServiceIndex();
            this.serviceIndex.Library = this.library;
            this.serviceIndex.DapperClient = this.dapperClient;
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
                    //
                    store = new Store();
                    if (Login(id, password))
                    {
                        // store.id = id;
                        observer.Send("formMain", "page", "02-index", null);
                    }
                }
                else
                {
                    string? dest = objJson["dest"]?.GetValue<string>();
                    if (dest == null)
                        return;
                    //
                    observer.Send("formMain", "page", dest, null);
                }
            }
            else if (action == "hello")
            {
                observer.Send("formMain", "hello", store.id, null);
            }
            else if (action == "ftp-status")
            {
                observer.Send("formMain", "ftp-status", CheckFtpStatus(), null);
            }
            else if (action == "api")
            {
                var objData = objJson?["data"];
                if (objData == null)
                    return;
                //
                List<Dictionary<string, object>> listDicData = serviceIndex.GetUsers((JsonObject) objData, store);
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

        public string CheckFtpStatus()
        {
            return "ok";
        }

        public void Listener(string target, string action, string message, dynamic data)
        {
            Console.WriteLine("\nDebug>>> Controller Listener");
            Console.WriteLine("(target) " + target);
            Console.WriteLine("(action) " + action);
            Console.WriteLine("(message) " + message);
        }
    }
}
