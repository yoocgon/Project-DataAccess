
using System.Text.Json.Nodes;
using System.Text.Json;
using Renci.SshNet;

namespace KCureDataAccess
{
    public class Controller
    {
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
            if (page == "login")
            {
                if(action == "page")
                {
                    var objData = objJson["data"];
                    string id = (string)objData["id"];
                    string password = (string)objData["password"];
                    Console.WriteLine("Debug>>> (id) : " + id);
                    Console.WriteLine("Debug>>> (password) : " + password);
                    observer.Send("formMain", "page", "index", null);
                }
            }
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
