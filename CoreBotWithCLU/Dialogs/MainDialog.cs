﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly HumanResourceRecognizer _cluRecognizer;
        protected readonly ILogger Logger;

        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialog(HumanResourceRecognizer cluRecognizer, VacationPeriodDialog vacationPeriodDialog, ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _cluRecognizer = cluRecognizer;
            Logger = logger;

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(vacationPeriodDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_cluRecognizer.IsConfigured)
            {
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("NOTE: CLU is not configured. To enable all capabilities, add 'CluProjectName', 'CluDeploymentName', 'CluAPIKey' and 'CluAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.
            var messageText = stepContext.Options?.ToString() ?? "What can I help you with today?\n\nAsk anything about Human Resource policy of SISU Healthcare Solutions.";
            var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_cluRecognizer.IsConfigured)
            {
                // CLU is not configured, we just run the BookingDialog path with an empty BookingDetailsInstance.
                return await stepContext.BeginDialogAsync(nameof(VacationPeriodDialog), new WorkedYearsDetails(), cancellationToken);
            }

            // Call CLU and gather any potential booking details. (Note the TurnContext has the response to the prompt.)
            var cluResult = await _cluRecognizer.RecognizeAsync<HumanResource>(stepContext.Context, cancellationToken);
            switch (cluResult.GetTopIntent().intent)
            {
                case HumanResource.Intent.VacationPeriod:
                    // Initialize BookingDetails with any entities we may have found in the response.
                    var bookingDetails = new WorkedYearsDetails()
                    {
                        // Destination = cluResult.Entities.GetToCity(),
                        Years = cluResult.Entities.GetWorkedYears(),
                        // TravelDate = cluResult.Entities.GetFlightDate(),
                    };

                    // Run the BookingDialog giving it whatever details we have from the CLU call, it will fill out the remainder.
                    return await stepContext.BeginDialogAsync(nameof(VacationPeriodDialog), bookingDetails, cancellationToken);

                default:
                    // Catch all for unhandled intents
                    // var didntUnderstandMessageText = $"Sorry, I didn't get that. Please try asking in a different way (intent was {cluResult.GetTopIntent().intent})";
                    var didntUnderstandMessageText = "Sorry, I am not ready for that question.";
                    var didntUnderstandMessage = MessageFactory.Text(didntUnderstandMessageText, didntUnderstandMessageText, InputHints.IgnoringInput);
                    await stepContext.Context.SendActivityAsync(didntUnderstandMessage, cancellationToken);
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // the Result here will be null.
            if (stepContext.Result is WorkedYearsDetails result)
            {
                // Now we have all the booking details call the booking service.
                string years = result.Years;
                int vacationWeeks = 2;
                try {
                    vacationWeeks = int.Parse(years.Split(" ")[0]) > 3 ? 3 : 2;
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
                
                var messageText = $"{vacationWeeks} weeks of vacation for eligible employees";
                var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            // Restart the main dialog with a different message the second time around
            var promptMessage = "What other question do you have?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}
