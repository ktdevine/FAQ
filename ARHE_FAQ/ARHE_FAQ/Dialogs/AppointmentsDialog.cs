using System;
using System.Threading.Tasks;
using ARHE_FAQ.Controllers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class AppointmentsDialog : IDialog<object>
    {
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

            PromptDialog.Confirm(
                context: context,
                resume: ResumeOrEndPromptAsync,
                prompt: customerAppointment + " Is there anything else I can help you with today?",
                retry: "I didn't understand. Please try again.");
        }

        private async Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
        }
    }
}