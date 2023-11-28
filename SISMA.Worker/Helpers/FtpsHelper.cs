// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using WinSCP;

namespace SISMA.Worker.Helper
{
    public class FtpsHelper
    {
        private readonly ILogger logger;

        public string FtpPathCombine(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/");
        }

        public FtpsHelper(ILogger _logger)
        {
            logger = _logger;
        }

        public class FtpsFileModel
        {
            public string FileName { get; set; }
            public long FileSize { get; set; }
            public byte[] Content { get; set; }
        }

        public class FtpsFileResult
        {
            public bool Succeeded { get; set; }
            public string NewFileName { get; set; }

            public string ErrorMessage { get; set; }
        }

        string ConnectionType;
        string HostName;
        int Port;
        string UserName;
        string Password;
        string SSHcertificate;
        string WinSCPPath;

        SessionOptions sessionOption = null;

        public void Init(string type, string host, int port, string username = null, string password = null, string sshKey = null, string winscpPath = null)
        {
            ConnectionType = type;
            HostName = host;
            Port = port;
            if (!string.IsNullOrEmpty(username))
            {
                UserName = username;
                Password = password;
            }
            SSHcertificate = sshKey;
            WinSCPPath = winscpPath;
        }

        void createSessionOptions()
        {
            switch (ConnectionType)
            {
                case "ftp":
                    sessionOption = new SessionOptions()
                    {
                        Protocol = Protocol.Ftp,
                        HostName = HostName,
                        UserName = UserName,
                        Password = Password,
                    };
                    break;
                case "ftps":
                    sessionOption = new SessionOptions()
                    {
                        Protocol = Protocol.Ftp,
                        FtpSecure = FtpSecure.Explicit,
                        HostName = HostName,
                        UserName = UserName,
                        Password = Password,
                    };
                    break;
                case "sftp":
                    sessionOption = new SessionOptions()
                    {
                        Protocol = Protocol.Sftp,
                        HostName = HostName,
                        PortNumber = Port,
                        UserName = UserName,
                        Password = Password,
                        SshHostKeyFingerprint = SSHcertificate,
                        Timeout = TimeSpan.FromMinutes(2)
                    };
                    break;
            }
        }
        private void configureSession(Session session)
        {
            if (!string.IsNullOrEmpty(WinSCPPath))
            {
                session.ExecutablePath = WinSCPPath;
            }
        }

        public string[] List(string path)
        {
            createSessionOptions();

            using (Session session = new Session())
            {
                configureSession(session);
                try
                {
                    session.Open(sessionOption);
                    var rdi = session.ListDirectory(path);
                    var files = rdi.Files
                        .Where(x => !x.IsParentDirectory && !x.IsThisDirectory && !x.IsDirectory)
                        .Select(x => x.Name).ToArray();

                    session.Close();
                    return files;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public FtpsFileModel Download(string path)
        {
            createSessionOptions();

            using (Session session = new Session())
            {
                configureSession(session);

                try
                {
                    session.Open(sessionOption);
                    var fileStream = session.GetFile(path);
                    var result = new FtpsFileModel()
                    {
                        FileName = System.IO.Path.GetFileName(path)
                    };

                    using (MemoryStream ms = new MemoryStream())
                    {
                        fileStream.CopyTo(ms);
                        result.Content = ms.ToArray();
                        result.FileSize = result.Content.Length;

                    }
                    session.Close();

                    return result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


        }

        public FtpsFileResult Upload(string path, byte[] fileContent)
        {
            createSessionOptions();

            var result = new FtpsFileResult()
            {
                Succeeded = false
            };

            using (Session session = new Session())
            {
                configureSession(session);

                try
                {
                    session.Open(sessionOption);

                    using (MemoryStream fileStream = new MemoryStream(fileContent))
                    {
                        fileStream.Write(fileContent, 0, fileContent.Length);

                        session.PutFile(fileStream, path);
                        result.Succeeded = true;
                    }
                    session.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            return result;
        }

        public FtpsFileResult Delete(string path)
        {
            createSessionOptions();

            using (Session session = new Session())
            {
                configureSession(session);

                session.Open(sessionOption);
                var removeResponse = session.RemoveFile(path);

                var result = new FtpsFileResult()
                {
                    Succeeded = removeResponse.Error == null,
                    ErrorMessage = removeResponse?.Error?.Message
                };
                return result;
            }
        }


    }
}
