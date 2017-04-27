using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class ReimbursementsDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as IMessageActivity;

            // TODO: Get Reimubursement FAQ
            //Create Choice Prompt and display FAQ Data and question to continue?
            //Create method to handle the question (yes or no)
            //if yes....then conext.done - which will go back to root dialog
            // if now ... then prompt user "we are sorry it has been noted" context.done

            context.Wait(MessageReceivedAsync);
        }
    }
}