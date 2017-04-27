using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;
using System.Collections.Generic;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //Properties used in prompts
        public string name;
        public bool IsCustomerRequired = false;

        private const string FAQStatus = "Status";
        private const string FAQAppointments = "Appointments";
        private const string FAQReimbursements = "Reimbursements";
        private const string FAQClaims = "Claims";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(InitialPromptMessageAsync);

            return Task.CompletedTask;
        }

        private async Task InitialPromptMessageAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            //Recived the initial text from the user. What do we want to do?
            PromptDialog.Text(
                context: context,
                resume: ResumeAndPromptFAQAsync,
                prompt: "Hello, I am the ARHE Bot. I am here to help. Can I please have your name?",
                retry: "I didn't understand. Please try again.");
            
        }

        private async Task ResumeAndPromptFAQAsync(IDialogContext context, IAwaitable<string> result)
        {
            name = await result;

            PromptDialog.Choice(
                context: context,
                resume: ResumeAndPromptDecisionAsync,
                options: new List<string>() { FAQStatus, FAQClaims, FAQAppointments, FAQReimbursements },
                prompt: $"Thank you {name}. Which of these areas do you need help with today?",
                retry: "I didn't understand. Please try again."                
                );
            
        }

        private async Task ResumeAndPromptDecisionAsync(IDialogContext context, IAwaitable<string> result)
        {
            var message = await result;
                       
            //We have the FAQ Type - need to determine if they are a customer or not.
            switch (message)
            {
                case "Status":
                    context.Call(new StatusDialog(), BackFromDialogPrompt);
                    break;
                case "Claims":
                    context.Call(new ClaimsDialog(), BackFromDialogPrompt);
                    break;
                case "Reimbursements":
                    context.Call(new ReimbursementsDialog(), BackFromDialogPrompt);
                    break;
                case "Appointments":
                    context.Call(new AppointmentsDialog(), BackFromDialogPrompt);
                    break;
                default:
                    break;
            }

        }

        private async Task BackFromDialogPrompt(IDialogContext context, IAwaitable<object> result)
        {
            //is more help needed?
            var isMoreHelpNeeded = await result;

            if ((bool)isMoreHelpNeeded)
            {
                context.Wait(InitialPromptMessageAsync);
            }
            else
            {
                await context.PostAsync("Thank you and have a great day");
            }
            
        }
        
        
    }
}