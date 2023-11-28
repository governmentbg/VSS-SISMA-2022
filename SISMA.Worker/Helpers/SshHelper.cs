using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static SISMA.Worker.Helper.FtpsHelper;

namespace SISMA.Worker.Helpers
{
    public class SshHelper
    {
        string HostName;
        int Port;
        int TimeoutInSec;
        string UserName;
        string Password;
        private readonly ILogger logger;

        public SshHelper(ILogger logger)
        {
            this.logger = logger;
        }

        public string FtpPathCombine(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/");
        }

        public void Init(string host, int port, string username = null, string password = null,int timeoutInSec = 60)
        {
            HostName = host;
            Port = port;
            TimeoutInSec = timeoutInSec;
            if (!string.IsNullOrEmpty(username))
            {
                UserName = username;
                Password = password;
            }
        }

        void configureClient(SftpClient client)
        {
            client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(TimeoutInSec);
        }

        public string[] List(string path)
        {
            Func<string, bool> filterFiles = x => !x.StartsWith(".") &&
            (x.EndsWith("xml", StringComparison.InvariantCultureIgnoreCase) || x.EndsWith("xlsx", StringComparison.InvariantCultureIgnoreCase));
            using (var client = new SftpClient(HostName, Port, UserName, Password))
            {
                configureClient(client);
                client.Connect();
                var directoryListing = client.ListDirectory(path)
                                            .Where(x => x.IsRegularFile)
                                            .Select(x => x.Name)
                                            .Where(filterFiles)
                                            .OrderBy(x => x)
                                            .ToArray();
                client.Disconnect();
                return directoryListing;
            }
        }

        public FtpsFileModel Download(string path)
        {
            using (var client = new SftpClient(HostName, Port, UserName, Password))
            {
                configureClient(client);
                client.Connect();
                using (var ms = new MemoryStream())
                {


                    client.DownloadFile(path, ms);
                    client.Disconnect();
                    return new FtpsFileModel()
                    {
                        Content = ms.ToArray(),
                        FileSize = ms.Length
                    };
                }
            }
        }

        public FtpsFileResult Upload(string path, byte[] fileContent)
        {
            try
            {
                using (var client = new SftpClient(HostName, Port, UserName, Password))
                {
                    configureClient(client);
                    client.Connect();
                    using (var ms = new MemoryStream(fileContent))
                    {


                        client.UploadFile(ms, path);
                        client.Disconnect();
                        return new FtpsFileResult()
                        {
                            Succeeded = true
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                return new FtpsFileResult()
                {
                    Succeeded = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public FtpsFileResult Delete(string path)
        {
            try
            {
                using (var client = new SftpClient(HostName, Port, UserName, Password))
                {
                    configureClient(client);
                    client.Connect();

                    client.DeleteFile(path);
                    client.Disconnect();
                    return new FtpsFileResult()
                    {
                        Succeeded = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new FtpsFileResult()
                {
                    Succeeded = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
