// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using CoreBotCLU;
using CoreBotCLU.EntityDetails;
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
    public MainDialog(HumanResourceRecognizer cluRecognizer, VacationPeriodDialog vacationPeriodDialog, RestVacationDialog restVacationDialog, ILogger<MainDialog> logger)
        : base(nameof(MainDialog))
    {
      _cluRecognizer = cluRecognizer;
      Logger = logger;

      AddDialog(new TextPrompt(nameof(TextPrompt)));
      AddDialog(vacationPeriodDialog);
      AddDialog(restVacationDialog);
      AddDialog(new PaidVacationEligibilityDialog());
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
      var cluResult = await _cluRecognizer.RecognizeAsync<HumanResource>(stepContext.Context, cancellationToken);
      HumanResource.Intent topIntent = cluResult.GetTopIntent().intent;
      switch (topIntent)
      {
        case HumanResource.Intent._12_VacationPeriod:
          {
            Console.WriteLine("-----> case HumanResource.Intent._12_VacationPeriod:");
            var workedYearsDetails = new WorkedYearsDetails()
            {
              Years = cluResult.Entities.GetWorkedYears(),
            };
            return await stepContext.BeginDialogAsync(nameof(VacationPeriodDialog), workedYearsDetails, cancellationToken);
          }
        case HumanResource.Intent._13_RestVacation:
          {
            Console.WriteLine("-----> case HumanResource.Intent._13_RestVacation:");
            var workedYearsDetails = new WorkedYearsDetails()
            {
              Years = cluResult.Entities.GetWorkedYears(),
            };
            return await stepContext.BeginDialogAsync(nameof(RestVacationDialog), workedYearsDetails, cancellationToken);
          }
        case HumanResource.Intent._18_PaidVacationEligibility:
          {
            Console.WriteLine("-----> case HumanResource.Intent._18_PaidVacationEligibility:");
            var confirmationDetails = new ConfirmationDetails()
            {
              Confirmed = false,
            };
            return await stepContext.BeginDialogAsync(nameof(PaidVacationEligibilityDialog), confirmationDetails, cancellationToken);
          }
        default:
          // Catch all for unhandled intents
          var defaultMessageText = Constants.AnswersForIntent[topIntent];
          var defaultMessage = MessageFactory.Text(defaultMessageText, defaultMessageText, InputHints.IgnoringInput);
          await stepContext.Context.SendActivityAsync(defaultMessage, cancellationToken);
          break;
      }

      return await stepContext.NextAsync(null, cancellationToken);
    }

    private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
    {
      // the Result here will be null.
      if (stepContext.Result is WorkedYearsDetails workedYearsDetailsResult)
      {
        bool bSenior = false;
        string messageText = "";
        if (int.TryParse(workedYearsDetailsResult.Years, out int number) && number > 3)
        {
          bSenior = true;
        }

        if (workedYearsDetailsResult.Intent == HumanResource.Intent._12_VacationPeriod)
        {
          messageText = $"{(bSenior ? 3 : 2)} weeks of vacation for eligible employees.";
        }
        else if (workedYearsDetailsResult.Intent == HumanResource.Intent._13_RestVacation)
        {
          messageText = bSenior
            ? "Employees who have completed years 4 - 5+ may carry over a maximum of 40 hours to the next year with a cap of 4 total weeks of vacation in any given year. Any vacation exceeding 4 weeks will be forfeited by the employee."
            : "Employees who have completed years 1 - 4 may carry over a maximum of 40 hours to the next year with a cap of 3 total weeks of vacation in any given year. Any vacation exceeding 3 weeks will be forfeited by the employee.";
        }

        var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
        await stepContext.Context.SendActivityAsync(message, cancellationToken);
      }
      else if (stepContext.Result is ConfirmationDetails confirmationDetailsResult)
      {
        string messageText = "I am sorry, it isn't eligible.";
        if (confirmationDetailsResult.Confirmed)
        {
          messageText = "It is the policy of SISU Healthcare Solutions to provide vacation for full-time employees who work a minimum of 40 hours per week. Paid vacation time is for full-time employees during periods of active, full-time, employment.  Paid vacation time does not accumulate during an employee’s personal leave of absence or periods of administrative leave. Employees will earn vacation time from their first day of employment but are not eligible to use the accrued time during the probation period to include any extensions to the probation.";
        }

        var message = MessageFactory.Text(messageText, messageText, InputHints.IgnoringInput);
        await stepContext.Context.SendActivityAsync(message, cancellationToken);
      }

      // Restart the main dialog with a different message the second time around
      var promptMessage = "What other question do you have?";
      return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
    }
  }
}
