using Renci.SshNet;
using Renci.SshNet.Sftp;


namespace KCureDataAccess
{
    public class CustomSftpClient
    {
        public string host;
        public int port;
        public string user;
        public string password;
        public string pathSecretKey;
        public string dir;
        private Renci.SshNet.SftpClient client;


        public CustomSftpClient(string host, int port, string user, string password, string dir) { 
            this.host = host;
            this.port = port;
            this.user = user;
            this.password = password;
            this.dir = dir;
            this.client = new Renci.SshNet.SftpClient(host, 22, user, password);
            //
            client.KeepAliveInterval = TimeSpan.FromSeconds(60);
            client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
            client.OperationTimeout = TimeSpan.FromMinutes(180);
            client.Connect();
        }


        public CustomSftpClient(string host, int port, string user, string dir, params PrivateKeyFile[] keyFiles)
        {
            this.host = host;
            this.port = port;
            this.user = user;
            this.client = new Renci.SshNet.SftpClient(host, port, user, keyFiles);
            //
            client.KeepAliveInterval = TimeSpan.FromSeconds(60);
            client.ConnectionInfo.Timeout = TimeSpan.FromMinutes(180);
            client.OperationTimeout = TimeSpan.FromMinutes(180);
            client.Connect();
        }


        public List<SftpFile> GetFileList(string dir)
        {
            var files = new List<SftpFile>();
            IEnumerable<SftpFile> temps = client.ListDirectory(dir);
            foreach (SftpFile item in temps)
            {
                files.Add(item);
            }
            return files;
        }


        public void DownloadDir(string dirRemote, string dirLocal)
        {
            List<SftpFile> files = GetFileList(dirRemote);
            foreach(SftpFile file in files)
            {
                using (Stream fileStream = File.Create(file.FullName))
                {
                    string fileDownload = dirLocal + "/" + file;
                    client.DownloadFile(fileDownload, fileStream);
                }
            }
        }


        public void UploadDir(string dirLocal, string dirRemote)
        {
            IEnumerable<FileSystemInfo> infos = new DirectoryInfo(dirLocal).EnumerateFileSystemInfos();
            foreach (FileSystemInfo info in infos)
            {
                if (info.Attributes.HasFlag(FileAttributes.Directory))
                {
                    string subPath = dirRemote + "/" + info.Name;
                    if (!client.Exists(subPath))
                    {
                        client.CreateDirectory(subPath);
                    }

                    UploadDir(info.FullName, dirRemote + "/" + info.Name);
                }
                else
                {
                    using (Stream fileStream = new FileStream(info.FullName, FileMode.Open))
                    {
                        client.UploadFile(fileStream, dirRemote + "/" + info.Name);
                    }
                }
            }
        }


        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}
