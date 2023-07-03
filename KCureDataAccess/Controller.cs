
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
