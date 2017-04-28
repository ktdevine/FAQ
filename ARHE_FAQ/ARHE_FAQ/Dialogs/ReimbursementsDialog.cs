using System;
using System.Threading.Tasks;
using ARHE_FAQ.Controllers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class ReimbursementsDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            //Create Choice Prompt and display FAQ Data and question to continue?
            PromptDialog.Confirm(
                context: context,
                resume: ResumeAndPromptCustomerAsync,
                prompt: "Ok. You asked about reimbursements, are you a current customer?",
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
                    resume: GetCustomerReimbursementInfo,
                    prompt: "Ok, can you please provide your Customer Id?",
                    retry: "I didn't understand. Please try again.");
            }
            else
            {
                await context.PostAsync("We do not store reimbursement information for non-customers.");
            }
        }

        private async Task GetCustomerReimbursementInfo(IDialogContext context, IAwaitable<string> result)
        {
            string customerId = await result;
            var customerReimbursement = new ServiceRequestController().GetPremiumReimbursement(customerId);

            PromptDialog.Confirm(
                context: context,
                resume: ResumeOrEndPromptAsync,
                prompt: customerReimbursement + " Is there anything else I can help you with today",
                retry: "I didn't understand. Please try again.");
        }

        private async Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
        }
    }
}