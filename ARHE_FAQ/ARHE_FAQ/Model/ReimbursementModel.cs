using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARHE_FAQ.Model
{
    public class ReimbursementModel
    {
        public string CustomerId { get; set; }
        public bool IsProcessed { get; set; }
        public double PremiumAmount { get; set; }
    }
}