﻿using ChatApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel emailModel);
    }
}
