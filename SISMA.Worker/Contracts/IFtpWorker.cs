﻿// Copyright (C) Information Services. All Rights Reserved.
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SISMA.Worker.Contracts
{
    public interface IFtpWorker
    {
        Task<int> Process();
    }
}
