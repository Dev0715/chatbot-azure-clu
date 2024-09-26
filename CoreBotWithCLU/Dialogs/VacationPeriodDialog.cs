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
    public class VacationPeriodDialog : CancelAndHelpDialog
  {
    private readonly HumanResourceRecognizer _cluRecognizer;

    private const string WorkedYearsStepMsgText = "How long have you worked at Sisu?";
    private const string RePromptMsgText = "I am sorry, could you please enter the number of years you have worked here?";

    public VacationPeriodDialog(HumanResourceRecognizer cluRecognizer)
        : base(nameof(VacationPeriodDialog))
    {
      _cluRecognizer = cluRecognizer;

      AddDialog(new TextPrompt(nameof(TextPrompt)));
      // AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
      // AddDialog(new WorkedYearsDialog(cluRecognizer));
      AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
      {
        WorkedYearsStepAsync,
        FinalStepAsync,
      }));

      // The initial child Dialog to run.
      InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> WorkedYearsStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var workedYearsDetails = (WorkedYearsDetails)stepContext.Options;

      if (workedYearsDetails.Years == null || workedYearsDetails.Years == "Error")
      {
        var messageTex = workedYearsDetails.Years == null ? WorkedYearsStepMsgText : RePromptMsgText;
        var promptMessage = MessageFactory.Text(messageTex, messageTex, InputHints.ExpectingInput);
        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        // return await stepContext.BeginDialogAsync(nameof(WorkedYearsDialog), workedYearsDetails.Years, cancellationToken);
      }

      return await stepContext.NextAsync(workedYearsDetails.Years, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var workedYearsDetails = (WorkedYearsDetails)stepContext.Options;

      var cluResult = await _cluRecognizer.RecognizeAsync<HumanResource>(stepContext.Context, cancellationToken);
      var Years = cluResult.Entities.GetWorkedYears();

      if (Years == null)
      {
        workedYearsDetails.Years = "Error";
        return await stepContext.ReplaceDialogAsync(InitialDialogId, workedYearsDetails, cancellationToken);
      }
      else
      {
        workedYearsDetails.Years = Years;
        workedYearsDetails.Intent = HumanResource.Intent.VacationPeriod;
        return await stepContext.EndDialogAsync(workedYearsDetails, cancellationToken);
      }

      // return await stepContext.EndDialogAsync(null, cancellationToken);
    }
  }
}
