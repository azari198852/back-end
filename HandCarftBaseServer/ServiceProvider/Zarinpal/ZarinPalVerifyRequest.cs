﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HandCarftBaseServer.ServiceProvider.ZarinPal
{
    public class ZarinPalVerifyRequest
    {
        public string merchant_id { set; get; }
        public int amount { set; get; }
        public string authority { set; get; }
    }
}
