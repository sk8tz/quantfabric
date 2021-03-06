﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Material.Infrastructure.Static;

namespace Material.Contracts
{
    public interface ISMSAdapter
    {
        //Action<IEnumerable<Tuple<DateTimeOffset, JObject>>> Handler { get; set; }

        Task<IEnumerable<SMSMessage>> GetAllSMS(DateTime filterDate);
    }
}
