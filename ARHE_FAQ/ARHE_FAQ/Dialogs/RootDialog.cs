﻿using System;
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

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(InitialPromptMessageAsync);

        }

        private async Task InitialPromptMessageAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            var activity = await result;

            //Retrieving user name from userdata...this will return IF this is the users 2nd time
            string u;
            bool isUname = context.UserData.TryGetValue("username", out u);

            switch (activity.Type)
            {
                
                case ActivityTypes.ConversationUpdate:


                    string prompt = isUname ? $"Hello {u}, welcome back. Ready to start?" : "Hello, I am the ARHE Bot.I am here to help. Can I have your name?";
                        
                    //This is called when the bot starts
                    PromptDialog.Text(
                        context: context,
                        resume: ResumeAndPromptFAQAsync,
                        prompt: prompt,
                        retry: "I didn't understand. Please try again.");                
                    break;
                case ActivityTypes.Message:
                    //All the messages come here.  In case the connection does not connect. 
                    //Restarting dialogs                   
                    PromptDialog.Text(
                       context: context,
                       resume: ResumeAndPromptFAQAsync,
                       prompt: $"Hello, looks like you are back {u}. Ready to start?",
                       retry: "I didn't understand. Please try again.");
                    break;
                default:
                    context.Wait(InitialPromptMessageAsync);
                    break;
            }
            
        }

        private async Task ResumeAndPromptFAQAsync(IDialogContext context, IAwaitable<string> result)
        {
            name = await result;
            //Handle if there is a username present ... otherwise ignore for now
            string u;
            if (!context.UserData.TryGetValue("username", out u))
            {
                context.UserData.SetValue("username", name);
            } else
            {
                name = u;
            }     
            
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
                await RestartPromptFAQAsync(context);
            }
            else
            {
                await context.PostAsync("Thank you and have a great day");
            }
            
        }

        private async Task RestartPromptFAQAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context: context,
                resume: ResumeAndPromptDecisionAsync,
                options: new List<string>() { FAQStatus, FAQClaims, FAQAppointments, FAQReimbursements },
                prompt: "Which of these areas do you need help with today?",
                retry: "I didn't understand. Please try again."
                );
        }


    }
}