// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBotCLU.EntityDetails;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
  public class PaidVacationEligibilityDialog : CancelAndHelpDialog
  {
    private const string confirmStepMsgText = "Are you a full time employee with at least one year of employment at Sisu?";

    public PaidVacationEligibilityDialog()
        : base(nameof(PaidVacationEligibilityDialog))
    {
      // AddDialog(new TextPrompt(nameof(TextPrompt)));
      AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
      // AddDialog(new WorkedYearsDialog(cluRecognizer));
      AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
      {
        ConfirmStepAsync,
        FinalStepAsync,
      }));

      // The initial child Dialog to run.
      InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var confirmationDetails = (ConfirmationDetails)stepContext.Options;
      var messageText = confirmStepMsgText;
      var promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);

      return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var confirmationDetails = (ConfirmationDetails)stepContext.Options;
      confirmationDetails.Confirmed = (bool)stepContext.Result;
      confirmationDetails.Intent = HumanResource.Intent._18_PaidVacationEligibility;
      return await stepContext.EndDialogAsync(confirmationDetails, cancellationToken);
    }
  }
}
