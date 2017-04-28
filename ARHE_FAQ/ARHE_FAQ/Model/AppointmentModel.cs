using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ARHE_FAQ.Model
{
    public class AppointmentModel
    {
        public string CustomerId { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public string Subject { get; set; }
        public string AgentName { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime AppointmentConfirmationDate { get; set; }
    }
}