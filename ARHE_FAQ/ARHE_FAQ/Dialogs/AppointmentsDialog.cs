using System;
using System.Globalization;
using System.Threading.Tasks;
using ARHE_FAQ.Controllers;
using ARHE_FAQ.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class AppointmentsDialog : IDialog<object>
    {
        private const string TryAgain = " Is there anything else I can help you with today?";
        public Task StartAsync(IDialogContext context)
        {
            //Create Choice Prompt and display FAQ Data and question to continue?
            PromptDialog.Confirm(
                context: context,
                resume: ResumeAndPromptCustomerAsync,
                prompt: "Ok. You asked about appointments, are you a current customer?",
                retry: "I didn't understand. Please try again.");

            return Task.CompletedTask;
        }
        private async Task ResumeAndPromptCustomerAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isCurrentCustomer = await result;

            if (isCurrentCustomer)
            {
                PromptDialog.Text(
                    context: context,
                    resume: GetCustomerAppointmentInfo,
                    prompt: "Ok, can you please provide your Customer Id?",
                    retry: "I didn't understand. Please try again.");
            }
            else
            {
                await context.PostAsync("Please call our service center to schedule an appointment at (555)555-5555.");
                context.Done(false);
            }
        }

        private async Task GetCustomerAppointmentInfo(IDialogContext context, IAwaitable<string> result)
        {
            string customerId = await result;
            var customerAppointment = new ServiceRequestController().GetAppointmentInformation(customerId);

            switch (customerAppointment.IsConfirmed)
            {
                case true:
                    context.Done(GetAppointmentConfirmationInformation(customerAppointment));
                    break;
                case false:
                    context.Call(new AppointmentConfirmationDialog(customerAppointment), BackFromConfirmationPrompt);
                    break;
                default:
                    break;
            }
        }

        private async Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
        }
        private string GetAppointmentConfirmationInformation(AppointmentModel appointment)
        {
            string appointmentText = string.Format("You currently have a {0} appointment scheduled for {1} with {2}.  Your appointment was confirmed on {3}",
                appointment.Subject,
                appointment.AppointmentStartTime.ToString("f", CultureInfo.CurrentCulture),
                appointment.AgentName,
                appointment.AppointmentConfirmationDate.ToString("d", CultureInfo.CurrentCulture));

            return appointmentText;
        }
        private async Task BackFromConfirmationPrompt(IDialogContext context, IAwaitable<object> result)
        {
            object isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
        }
    }
}