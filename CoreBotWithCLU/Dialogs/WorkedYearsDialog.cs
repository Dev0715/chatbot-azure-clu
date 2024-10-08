﻿// Copyright (c) Microsoft Corporation. All rights reserved.
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
  public class WorkedYearsDialog : CancelAndHelpDialog
  {
    private readonly HumanResourceRecognizer _cluRecognizer;

    private const string ErrorYear = "ErrorYear";
    private const string WorkedYearsStepMsgText = "How long have you worked at Sisu?";
    private const string RePromptMsgText = "I am sorry, could you please enter the number of years you have worked here?";

    public WorkedYearsDialog(HumanResourceRecognizer cluRecognizer)
        : base(nameof(WorkedYearsDialog))
    {
      _cluRecognizer = cluRecognizer;

      AddDialog(new TextPrompt(nameof(TextPrompt)));
      // AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
      // AddDialog(new WorkedYearsValidatorDialog(cluRecognizer));
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

      if (workedYearsDetails.Years == null || workedYearsDetails.Years == ErrorYear)
      {
        var messageTex = workedYearsDetails.Years == null ? WorkedYearsStepMsgText : RePromptMsgText;
        var promptMessage = MessageFactory.Text(messageTex, messageTex, InputHints.ExpectingInput);
        return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        // return await stepContext.BeginDialogAsync(nameof(WorkedYearsValidatorDialog), workedYearsDetails.Years, cancellationToken);
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
        workedYearsDetails.Years = ErrorYear;
        return await stepContext.ReplaceDialogAsync(InitialDialogId, workedYearsDetails, cancellationToken);
      }
      else
      {
        workedYearsDetails.Years = Years;
        return await stepContext.EndDialogAsync(workedYearsDetails, cancellationToken);
      }

      // return await stepContext.EndDialogAsync(null, cancellationToken);
    }
  }
}
