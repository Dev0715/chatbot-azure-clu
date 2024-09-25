// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class VacationPeriodDialog : CancelAndHelpDialog
    {
        //private const string DestinationStepMsgText = "Where would you like to travel to?";
        private const string WorkedYearsStepMsgText = "How long have you worked at Sisu?";

        public VacationPeriodDialog()
            : base(nameof(VacationPeriodDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new DateResolverDialog());
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                //DestinationStepAsync,
                WorkedYearsStepAsync,
                //TravelDateStepAsync,
                //ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        //private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (WorkedYearsDetails)stepContext.Options;

        //    if (bookingDetails.Destination == null)
        //    {
        //        var promptMessage = MessageFactory.Text(DestinationStepMsgText, DestinationStepMsgText, InputHints.ExpectingInput);
        //        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.Destination, cancellationToken);
        //}

        private async Task<DialogTurnResult> WorkedYearsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (WorkedYearsDetails)stepContext.Options;

            //bookingDetails.Destination = (string)stepContext.Result;

            if (bookingDetails.Years == null)
            {
                var promptMessage = MessageFactory.Text(WorkedYearsStepMsgText, WorkedYearsStepMsgText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(bookingDetails.Years, cancellationToken);
        }

        //private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (WorkedYearsDetails)stepContext.Options;

        //    bookingDetails.Years = (string)stepContext.Result;

        //    if (bookingDetails.TravelDate == null || IsAmbiguous(bookingDetails.TravelDate))
        //    {
        //        return await stepContext.BeginDialogAsync(nameof(DateResolverDialog), bookingDetails.TravelDate, cancellationToken);
        //    }

        //    return await stepContext.NextAsync(bookingDetails.TravelDate, cancellationToken);
        //}

        //private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        //{
        //    var bookingDetails = (WorkedYearsDetails)stepContext.Options;

        //    bookingDetails.Years = (string)stepContext.Result;

        //    // var messageText = $"Please confirm, I have you traveling to: {bookingDetails.Destination} from: {bookingDetails.Years} on: {bookingDetails.TravelDate}. Is this correct?";
        //    var messageText = $"Please confirm, You have worked at Sisu for {bookingDetails.Years} years. Is this correct?";
        //    var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

        //    return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        //}

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var bookingDetails = (WorkedYearsDetails)stepContext.Options;

            bookingDetails.Years = (string)stepContext.Result;

            // var messageText = $"Please confirm, I have you traveling to: {bookingDetails.Destination} from: {bookingDetails.Years} on: {bookingDetails.TravelDate}. Is this correct?";
            // var messageText = $"Please confirm, You have worked at Sisu for {bookingDetails.Years} years. Is this correct?";
            // var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

            // return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);

            //if ((bool)stepContext.Result)
            //{
            //    var bookingDetails = (WorkedYearsDetails)stepContext.Options;

                return await stepContext.EndDialogAsync(bookingDetails, cancellationToken);
            //}

            //return await stepContext.EndDialogAsync(null, cancellationToken);
        }

        private static bool IsAmbiguous(string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return !timexProperty.Types.Contains(Constants.TimexTypes.Definite);
        }
    }
}
