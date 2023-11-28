// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NPOI.SS.Util;
using SISMA.Worker.Contracts;
using SISMA.Worker.Helper;
using SISMA.Worker.Helpers;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace SISMA.Worker.Services
{
    public class FtpWorker : IFtpWorker
    {
        private readonly SshHelper ssh;
        private readonly ILogger<FtpWorker> logger;
        private readonly bool renameWithTimestamp = true;
        private readonly IExcelFileProccess process;

        private string[] RootPaths;
        private string UploadedDir;
        public FtpWorker(IConfiguration config,
            ILogger<FtpWorker> _logger,
            IExcelFileProccess _process)
        {
            logger = _logger;
            process = _process;
            ssh = new SshHelper(logger);
            RootPaths = config.GetSection("FTP:Paths").Get<string[]>();
            UploadedDir = config.GetValue<string>("FTP:UploadedDir");

            ssh.Init(config.GetValue<string>("FTP:Host"),
                     config.GetValue<int>("FTP:Port"),
                     config.GetValue<string>("FTP:UserName"),
                     config.GetValue<string>("FTP:Password"),
                     config.GetValue<int>("FTP:TimeoutInSec",60));

            renameWithTimestamp = config.GetValue<bool>("FTP:RenameWithTimestamp");
        }
        public async Task<int> Process()
        {
            var result = 0;
            try
            {
                foreach (var path in RootPaths)
                {
                    result += await processOnePath(path);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Process");
            }
            return result;
        }

        Func<string, bool> filterFiles = x => x.StartsWith("sisma", StringComparison.InvariantCultureIgnoreCase);

        async Task<int> processOnePath(string rootDir)
        {
            var result = 0;


            //
            ////var fileName = "sisma-4-01-2022-02.xlsx";
            //var fileName = "sisma-4-05-2022-02.xlsx";
            //var fs = File.OpenRead(fileName);
            //byte[] bytes = new byte[fs.Length];
            //fs.Read(bytes, 0, (int)fs.Length);
            //var res = await process.ProcessAsync(fileName, bytes);


            var filesList = ssh.List(rootDir);
            foreach (var fileName in filesList)
            {
                var fileContent = ssh.Download(ssh.FtpPathCombine(rootDir, fileName));
                if (fileContent != null)
                {
                    bool isOk = await process.ProcessAsync(fileName, fileContent.Content);
                    if (isOk)
                    {
                        if (!string.IsNullOrEmpty(UploadedDir))
                        {
                            var uplDir = ssh.FtpPathCombine(rootDir, UploadedDir);
                            var uplFile = fileName;
                            if (renameWithTimestamp)
                            {
                                uplFile = $"{DateTime.Now:yyMMdd-HHmmss-}{fileName}";
                            }
                            ssh.Upload(ssh.FtpPathCombine(uplDir, uplFile), fileContent.Content);
                            ssh.Delete(ssh.FtpPathCombine(rootDir, fileName));
                        }
                        else
                        {
                            ssh.Delete(ssh.FtpPathCombine(rootDir, fileName));
                        }
                    }
                    result++;
                }
            }
            //logger.LogInformation($"Process started {DateTime.Now}: Time: {sw.Elapsed.TotalSeconds:N0} sec");
            return result;
        }
    }
}
