﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvertApi.Models;

namespace WebAdvert.Web.ServiceClients
{
    public interface IAdvertApiClient
    {
        Task<AdvertResponse> CreateAsync(CreateAdvertModel model);
        Task<bool> ConfirmAsync(ConfirmAdvertRequest model);
    }
}
