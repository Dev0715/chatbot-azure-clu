// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Microsoft.BotBuilderSamples.Clu;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
  /// <summary>
  /// An <see cref="IRecognizerConvert"/> implementation that provides helper methods and properties to interact with
  /// the CLU recognizer results.
  /// </summary>
  public class HumanResource : IRecognizerConvert
  {
    public enum Intent
    {
      _2_WhenEligibleForSickTime,
      _3_PaidSickTimeForPerDiemEmployee,
      _4_PaidSickTimeForPartTimeEmployee,
      _5_SickTimePeriod,
      _6_SickTimeRollOver,
      _7_RestSickTime,
      _8_UseSickTimeWhenImmediateFamilySick,
      _9_FutureSickTimeUsability,
      _10_VacationTimeAsPartTimeEmployee,
      _11_VacationTimeAsPerDiemEmployee,
      _12_VacationPeriod, // Layer - Worked Years
      _13_RestVacation, // Layer - Worked Years
      _14_WhenToRequestVacationTime,
      _15_VacationTimeGuaranteed,
      _16_CanTakeOneHourVacation,
      _17_CanUseVacationTimeForAbsence,
      _18_PaidVacationEligibility,  // Layer - Confirmation
      _19_VacationTimeAfterQuit,
      _20_VacationTimeAfterFired,
      _21_HolidayOnVacation,
      _22_AccruedVacationTimeAfterAbsence,
      _23_ManagerResponsibilityUponVacationRequest,
      _24_CanManagerDenyVacationRequest,
      _25_HowToScheduleVacationTime,
      _26_CanVacationTimeCoverTardiness,
      _27_VacationPolicy,
      _28_VacationInSmallIncrements,
      _29_CanHaveAnotherJob,
      _30_ImproperJobs, // Follow-up
      _31_SexuallyHarassed,
      _32_WhatShouldDo, // Follow-up
      _33_ProfessionalBoundaries,
      _34_WhereToFindStandardsConduct,
      _35_DrugPolicy,
      _36_SisuHaveRandomDrugTest,
      _37_WhatCauseRandomTest, // Follow-up
      _38_WhenTestedForDrugs,
      _39_AnyOtherTimes, // Follow-up
      _40_WhatHappensIfDrugTestFailed,
      _41_HowToReportAnIssue,
      _42_HowToFileGrievance, // Follow-up
      _43_WhatIfHavingIssueWithDirectSupervisor,
      _44_WhatIfNotOkWithDecision, // Follow-up
      _45_SisuHavePerformanceReview,
      _46_PerDiemEmployeeGetReview,
      _47_PartTimeEmployeeGetReview,
      _48_ReviewBasedOnWhat,
      _49_IsThereProbationaryPeriod,
      _50_WhenEligibleForBenefits,
      _51_CanTerminatedWithoutCause,
      _52_WhenReceiveSalaryIncrease,
      _53_ReceiveSalaryIncreaseAfterProbationaryPeriod,
      _54_GuaranteedSalaryIncreaseAfterFirstYear,
      _55_RatingCategoriesForReview,
      _56_PerformanceElements,
      _57_ThingsCauseFired,
      _58_HowManyInfractionsBeforeFired,
      _59_WhatCauseFiredImmediately,
      _60_FiredAfterLateTwice,
      _61_InfractionsWarrantCounseling,
      _62_WhatIfRefuseDoingSthInJD,
      _63_HowMuchNoticeForQuit,
      _64_PaidAfterFired,
      _65_WhatRequiredToTurnInAfterQuit,
      None
    }

    public string Text { get; set; }

    public string AlteredText { get; set; }

    public Dictionary<Intent, IntentScore> Intents { get; set; }

    public CluEntities Entities { get; set; }

    public IDictionary<string, object> Properties { get; set; }

    public void Convert(dynamic result)
    {
      var jsonResult = JsonConvert.SerializeObject(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
      var app = JsonConvert.DeserializeObject<HumanResource>(jsonResult);

      Text = app.Text;
      AlteredText = app.AlteredText;
      Intents = app.Intents;
      Entities = app.Entities;
      Properties = app.Properties;
    }

    public (Intent intent, double score) GetTopIntent()
    {
      var maxIntent = Intent.None;
      var max = 0.0;
      foreach (var entry in Intents)
      {
        Console.WriteLine($"{entry.Key}: {entry.Value.Score}");
        if (entry.Value.Score > 0.7 && entry.Value.Score > max)
        {
          maxIntent = entry.Key;
          max = entry.Value.Score.Value;
        }
      }

      return (maxIntent, max);
    }

    public class CluEntities
    {
      public CluEntity[] Entities;

      public CluEntity[] GetWorkedYearsList() => Entities.Where(e => e.Category == "workedYears").ToArray();

      public string GetWorkedYears() => GetWorkedYearsList().FirstOrDefault()?.Text;
    }
  }
}
