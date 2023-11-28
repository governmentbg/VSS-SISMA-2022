﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SISMA.Infrastructure.Contracts
{
    public interface IUserContext
    {
        string UserId { get; }
        string Email { get; }
        string LogName { get; }
        string FullName { get; }
        bool IsUserInRole(string role);
    }
}
