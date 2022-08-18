﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterService.Contracts
{
    public interface ITweetReportService
    {
        Task<int> GetTotalTweetReceived();
        Task<IEnumerable<string>?> GetTopTenHashtags();
    }
}
