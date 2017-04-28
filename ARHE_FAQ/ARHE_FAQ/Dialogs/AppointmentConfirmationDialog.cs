using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ARHE_FAQ.Model;
using Microsoft.Bot.Builder.Dialogs;

namespace ARHE_FAQ.Dialogs
{
    [Serializable]
    public class AppointmentConfirmationDialog : IDialog<object>
    {
        private readonly AppointmentModel _appointment;
        public AppointmentConfirmationDialog(AppointmentModel appointment)
        {
            _appointment = appointment;
        }
        public Task StartAsync(IDialogContext context)
        {
            var appointmentText = string.Format("You currently have a {0} appointment scheduled for {1} with {2}. Would you like to confirm your appointment?",
                    _appointment.Subject,
                    _appointment.AppointmentStartTime.ToString("f", CultureInfo.CurrentCulture),
                    _appointment.AgentName);

            PromptDialog.Confirm(
                context: context,
                resume: ConfirmPromptAsync,
                prompt: appointmentText,
                retry: "I didn't understand. Please try again.");

            return Task.CompletedTask;
        }

        private async Task ConfirmPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            bool confirmAppointment = await result;

            if (confirmAppointment)
            {
                //Logic to update appointment Confirmation
                PromptDialog.Confirm(
                    context: context,
                    resume: ResumeOrEndPromptAsync,
                    prompt: "Your appointment has been confirmed.  Is there anything else that I can help you with?",
                    retry: "I didn't understand. Please try again.");
            }
            else
            {
                //Logic to update appointment Confirmation
                PromptDialog.Confirm(
                    context: context,
                    resume: ResumeOrEndPromptAsync,
                    prompt: "Ok, is there anything else that I can help you with?",
                    retry: "I didn't understand. Please try again.");
            }
        }
        private async Task ResumeOrEndPromptAsync(IDialogContext context, IAwaitable<bool> result)
        {
            object isMoreHelpNeeded = await result;

            context.Done(isMoreHelpNeeded);
        }
    }
}