﻿using ChatApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Persistence.Contracts
{
    internal interface IMessageRepository : IGenericRepository<Message>
    {
    }
}
