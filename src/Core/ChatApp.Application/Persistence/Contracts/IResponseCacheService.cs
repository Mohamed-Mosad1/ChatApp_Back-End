﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Application.Persistence.Contracts
{
    public interface IResponseCacheService
    {
        // Cache Data
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan expireTime);

        // Get Cached Data
        Task<string?> GetCachedResponse(string cacheKey);
    }
}