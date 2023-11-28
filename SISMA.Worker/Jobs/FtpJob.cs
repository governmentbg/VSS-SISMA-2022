// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Quartz;
using SISMA.Worker.Contracts;

namespace SISMA.Worker.Jobs
{
    /// <summary>
    /// Изтегляне данни за каталози от SFTP
    /// </summary>
    [DisallowConcurrentExecution]
    internal class FtpJob : BaseJob
    {
        private readonly IFtpWorker ftpWorker;
        public FtpJob(
            IFtpWorker _ftpWorker,
            ILogger<FtpJob> _logger)
        {
            ftpWorker = _ftpWorker;
            logger = _logger;
        }
        protected override async Task DoJob(IJobExecutionContext context)
        {
            await ftpWorker.Process();
        }
    }
}
