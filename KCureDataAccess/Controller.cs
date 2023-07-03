
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
            string strTpye = (string)objJson["type"];
            //
            if (strTpye == "data")
            {
                string strDataType = (string)objJson["dataType"];
                var objData = objJson["data"];
            }
            else if (strTpye == "page")
            {
                string page = (string)objJson["data"];
                if (page == "test")
                {
                    observer.Send("page", "test", "form1", null);
                }
            }
        }

        public void Listen(string type, string message, dynamic data)
        {
            Console.WriteLine("");
        }
    }
}
