
using System.Text.Json.Nodes;
using System.Text.Json;
using Renci.SshNet;
using Microsoft.VisualBasic.Logging;

namespace KCureDataAccess
{
    public class Controller
    {
        //
        Store store;
        //
        Observer observer;
        //
        CustomSftpClient sftpReqeustedDataClient;
        CustomSftpClient sftpInsertedDataClient;
        CustomSftpClient sftpInserteRequestDataClient;
        //
        CustomHttpClient httpKcureClient;
        //
        CustomPostgresClient postgresClient;

        public Controller(Observer observer) {
            this.observer = observer;
            this.observer.Add(this);
            ////
            //PrivateKeyFile? kcureSvr = new PrivateKeyFile("");
            //sftpDataReqeusted = new Sftp("100.100.100.1", 22, "centos", "", kcureSvr);
            //sftpDataImported = new Sftp("100.100.100.2", 22, "centos", "", kcureSvr);
            //
            httpKcureClient = new CustomHttpClient();
        }

        public void Parse(string strJson)
        {
            var objJson = JsonSerializer.Deserialize<JsonObject>(strJson);
            //
            string page = (string)objJson["page"];
            string action = (string)objJson["action"];
            //
           if(action == "page")
           {
                if (page == "login")
                {
                    var objData = objJson["data"];
                    string id = (string)objData["id"];
                    string password = (string)objData["password"];
                    Console.WriteLine("Debug>>> (id) : " + id);
                    Console.WriteLine("Debug>>> (password) : " + password);
                    //
                    store = new Store();
                    if (Login(id, password))
                    {
                        store.id = id;
                        observer.Send("formMain", "page", "02-index", null);
                    }
                    //
                }
                else
                {
                    string dest = (string)objJson["dest"];
                    observer.Send("formMain", "page", dest, null);
                }
            }
           else if(action == "api")
            {
                var objData = objJson["data"];
                string department = (string)objData["department"];
                Console.WriteLine("Debug>>> (department) : " + department);

                List<Dictionary<String, String>> listDicData = new List<Dictionary<String, String>>();
                Dictionary<String, String> dicData = new Dictionary<String, String>();
                dicData.Add("id", "1");
                dicData.Add("researchSubject", "개발 테스트 연구");
                dicData.Add("department", "데이터스트림즈");
                dicData.Add("researcher", "유창곤");
                dicData.Add("applyStep", "1단계?");
                dicData.Add("applyStatus", "심사중");
                dicData.Add("importedDataExists", "Y");
                dicData.Add("dataExportRequestExists", "Y");
                dicData.Add("dataExportApprovalExistence", "Y");
                dicData.Add("dataUtilizationStartDate", "2023-06-01 00:00:00");
                dicData.Add("dataUtilizationEndDate", "2023-06-01 00:00:00");
                listDicData.Add(dicData);

                Dictionary<String, String> dicData2 = new Dictionary<String, String>();
                dicData2.Add("id", "2");
                dicData2.Add("researchSubject", "개발 테스트 연구");
                dicData2.Add("department", "데이터스트림즈");
                dicData2.Add("researcher", "유창곤");
                dicData2.Add("applyStep", "1단계?");
                dicData2.Add("applyStatus", "심사중");
                dicData2.Add("importedDataExists", "Y");
                dicData2.Add("dataExportRequestExists", "Y");
                dicData2.Add("dataExportApprovalExistence", "Y");
                dicData2.Add("dataUtilizationStartDate", "2023-06-01 00:00:00");
                dicData2.Add("dataUtilizationEndDate", "2023-06-01 00:00:00");
                listDicData.Add(dicData2);

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
    }
}
