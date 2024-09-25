// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace Microsoft.BotBuilderSamples.Dialogs
{
  public class WorkedYearsDialog : CancelAndHelpDialog
  {
    private readonly HumanResourceRecognizer _cluRecognizer;

    private const string PromptMsgText = "How long have you worked at Sisu?";
    private const string RePromptMsgText = "I am sorry, could you please enter the number of years you have worked here?";

    public WorkedYearsDialog(HumanResourceRecognizer cluRecognizer)
        : base(nameof(WorkedYearsDialog))
    {
      _cluRecognizer = cluRecognizer;

      AddDialog(new TextPrompt(nameof(TextPrompt), TextPromptValidator));
      AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
      {
                InitialStepAsync,
                FinalStepAsync,
      }));

      // The initial child Dialog to run.
      InitialDialogId = nameof(WaterfallDialog);
    }

    private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var years = (string)stepContext.Options;

      var promptMessage = MessageFactory.Text(PromptMsgText, PromptMsgText, InputHints.ExpectingInput);
      var rePromptMessage = MessageFactory.Text(RePromptMsgText, RePromptMsgText, InputHints.ExpectingInput);

      if (years == null)
      {
        return await stepContext.PromptAsync(nameof(TextPrompt),
            new PromptOptions
            {
              Prompt = promptMessage,
              RetryPrompt = rePromptMessage,
            }, cancellationToken);
      }

      return await stepContext.NextAsync(years, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      var years = ((string)stepContext.Result);
      return await stepContext.EndDialogAsync(years, cancellationToken);
    }

    private static Task<bool> TextPromptValidator(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
    {
      if (promptContext.Recognized.Succeeded)
      {
        // Need to check if input is valid
        return Task.FromResult(true);
      }

      return Task.FromResult(false);
    }
  }
}
