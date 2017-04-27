using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class ClaimsDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            PromptDialog.Confirm(
               context: context,
               resume: ResumeAndPromptCustomerAsync,
               prompt: "Ok. You asked about claims, are you a current customer?",
               retry: "I didn't understand. Please try again.");
        }

        private async Task ResumeAndPromptCustomerAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isCurrentCustomer = await result;            
            //context.Done(GetCustomerClaimInfo(isCurrentCustomer));

            PromptDialog.Confirm(
               context: context,
               resume: ResumeOrEndPromptAsync,
               prompt: "Here is what I found " + GetCustomerClaimInfo(isCurrentCustomer) + " Is there anything else I can help you with today",
               retry: "I didn't understand. Please try again.");
        }

        private async Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
           
        }

        public string GetCustomerClaimInfo(bool isCurrentCustomer)
        {
            if (isCurrentCustomer)
            {
                return "Please go to www.arhe.com/current-customer-claims for more info";
            } else
            {
                return "Please go to www.arhe.com/new-customer-claims for more info";
            }
        }

       
    }
}