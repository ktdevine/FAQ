using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Script.Serialization;
using ARHE_FAQ.Model;

namespace ARHE_FAQ.Controllers
{
    public class ServiceRequestController : ApiController
    {
        [HttpPost]
        public string GetPremiumReimbursement(string requestString)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath(@"~\\Data\\PremiumReimbursement.json");
            string json = File.ReadAllText(path);
            JavaScriptSerializer js = new JavaScriptSerializer();
            ReimbursementModel[] responses = js.Deserialize<ReimbursementModel[]>(json);

            var response = responses.FirstOrDefault(x => x.CustomerId == requestString);
            string premiumAmount = string.Empty;

            if (response != null)
            {
                premiumAmount = string.Format("Your premium amount of {0} has {1}", response.PremiumAmount.ToString("C", CultureInfo.CurrentCulture),
                    response.IsProcessed ? "been processed" : "yet to be processed");
            }
            else
            {
                premiumAmount = string.Format("We're sorry, we are unable to find your premium information.");
            }

            return premiumAmount;
        }
        [HttpPost]
        public string GetAppointmentInformation(string requestString)
        {
            var path = System.Web.HttpContext.Current.Server.MapPath(@"~\\Data\\AppointmentDetail.json");
            string json = File.ReadAllText(path);
            JavaScriptSerializer js = new JavaScriptSerializer();
            AppointmentModel[] responses = js.Deserialize<AppointmentModel[]>(json);

            var response = responses.FirstOrDefault(x => x.CustomerId == requestString);
            string appointmentInformation = string.Empty;

            if (response != null)
            {
                appointmentInformation = string.Format("You currently have a {0} appointment scheduled for {1} with {2}. {3}{3}", 
                    response.Subject,
                    response.AppointmentStartTime.ToString("f", CultureInfo.CurrentCulture),
                    response.AgentName,
                    Environment.NewLine);

                appointmentInformation = appointmentInformation + (
                    response.IsConfirmed ? string.Format("Your Appointment was confirmed on {0}", response.AppointmentConfirmationDate.ToString("d", CultureInfo.CurrentCulture)) : string.Format("Your appointment has not been confirmed yet.  Would you like to confirm?"));
            }
            else
            {
                appointmentInformation = string.Format("You currently do not have a scheduled appointment, Please call our service center to schedule one today");
            }

            return appointmentInformation;
        }
    }
}
