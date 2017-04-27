using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Linq;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        //Properties used in prompts
        public string name;
        public FAQTypes faqtype;
        public bool IsCustomerRequired = false;

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
                options: Enum.GetValues(typeof(FAQTypes)).Cast<FAQTypes>().ToArray(),
                prompt: $"Thank you {name}. Which of these areas do you need help with today?",
                retry: "I didn't understand. Please try again."
                );
            
        }

        private async Task ResumeAndPromptDecisionAsync(IDialogContext context, IAwaitable<FAQTypes> result)
        {
            faqtype = await result;

            //We have the FAQ Type - need to determine if they are a customer or not.
            switch (faqtype.ToString())
            {
                case "Status":
                    context.Call(new StatusDialog(), ResumeAfterOptionDialog);
                    break;
                case "Claims":
                    context.Call(new ClaimsDialog(), ResumeAfterOptionDialog);
                    break;
                case "Reimbursements":
                    context.Call(new ReimbursementsDialog(), ResumeAfterOptionDialog);
                    break;
                case "Appointments":
                    context.Call(new AppointmentsDialog(), ResumeAfterOptionDialog);
                    break;
                default:
                    break;
            }

        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.BackFromDialogPromptAsync);
            }
        }

        private async Task BackFromDialogPromptAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            PromptDialog.Confirm(
                context: context,
                resume: ResumeOrEndPromptAsync,
                prompt: "Is there anything else I can help you with today",
                retry: "I didn't understand. Please try again.");
        }

        private Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            throw new NotImplementedException();
        }



        #region ENUMS for Choices
        public enum FAQTypes
        {
            Status,
            Claims,
            Reimbursements,
            Appointments
        }
        #endregion

        
    }
}