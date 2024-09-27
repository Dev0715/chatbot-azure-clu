using Microsoft.BotBuilderSamples;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Collections.Generic;

namespace CoreBotCLU
{
  public static class Constants
  {
    public static readonly Dictionary<HumanResource.Intent, string> AnswersForIntent = new Dictionary<HumanResource.Intent, string>
        {
            { HumanResource.Intent._1_WhenStartGettingPaidSickTime, "Accumulation of sick leave will start at the first day of employment; however, sick leave pay will not be granted until the completion of the 90-day probationary period." },
            { HumanResource.Intent._2_WhenEligibleForSickTime,  "Accumulation of sick leave will start at the first day of employment; however, sick leave pay will not be granted until the completion of the 90-day probationary period." },
            { HumanResource.Intent._3_PaidSickTimeForPerDiemEmployee,  "It is the policy of SISU Healthcare Solutions to provide paid sick leave for employees who work a minimum of 40 hours per week." },
            { HumanResource.Intent._4_PaidSickTimeForPartTimeEmployee, "It is the policy of SISU Healthcare Solutions to provide paid sick leave for employees who work a minimum of 40 hours per week." },
            { HumanResource.Intent._5_SickTimePeriod, "Full-time employees who work 40 or more hours per week will receive sick leave at a rate of 40 hours per year. Sick time is loaded into each employee’s sick time bank on January 1st of each year." },
            { HumanResource.Intent._6_SickTimeRollOver, "Sick leave will not be transferred from year to year. Employees on sick leave will not accumulate additional sick leave or vacation pay during the time they are sick. Unused sick leave will be forfeited upon termination." },
            { HumanResource.Intent._7_RestSickTime, "Sick leave will not be transferred from year to year. Employees on sick leave will not accumulate additional sick leave or vacation pay during the time they are sick. Unused sick leave will be forfeited upon termination." },
            { HumanResource.Intent._8_UseSickTimeWhenImmediateFamilySick, "Employees may use sick leave for personal or immediate family illness or accident. The immediate family is defined as spouse, children, mother, and father. Employees will be paid for the actual amount of time they are absent from work, provided it is accrued." },
            { HumanResource.Intent._9_FutureSickTimeUsability, "Sick leave cannot be borrowed in advance. Management may request that an employee provide physician documentation of illness/condition, whether due to the employee's illness or illness of an immediate family member."},
            { HumanResource.Intent._10_VacationTimeAsPartTimeEmployee, "Paid vacation time is for full-time employees during periods of active, full-time, employment." },
            { HumanResource.Intent._11_VacationTimeAsPerDiemEmployee, "Paid vacation time is for full-time employees during periods of active, full-time, employment." },
            { HumanResource.Intent._14_WhenToRequestVacationTime, "Vacation time must be scheduled in writing to the employee’s immediate manager/ supervisor at least one week in advance." },
            { HumanResource.Intent._15_VacationTimeGuaranteed, "The use and duration of vacation time is subject to supervisory approval.  The Company will make every effort to accommodate requests to schedule vacation time but reserves the right to prioritize requests based on business needs, the seniority of the employees requesting vacation time and the good judgment of the supervisor/ manager." },
            { HumanResource.Intent._16_CanTakeOneHourVacation, "If you have completed at least one year of employment, vacation time may be taken in increments as small as one hour. However, vacation time may not be used to compensate employees for tardiness, unexcused absences or for sick time." },
            { HumanResource.Intent._17_CanUseVacationTimeForAbsence, "vacation time may not be used to compensate employees for tardiness, unexcused absences or for sick time." },
            { HumanResource.Intent._19_VacationTimeAfterQuit, "Employees who resign or are terminated, will not be eligible for a payout of vacation time. Vacation days will not be considered as time worked for purposes of calculating overtime. Vacation time cannot be used during a termination notice." },
            { HumanResource.Intent._20_VacationTimeAfterFired, "Employees who resign or are terminated, will not be eligible for a payout of vacation time. Vacation days will not be considered as time worked for purposes of calculating overtime. Vacation time cannot be used during a termination notice." },
            { HumanResource.Intent._21_HolidayOnVacation, "If a holiday occurs during employee’s vacation period, holiday pay will be earned at employee’s regular rate of pay and no vacation time will be used for that day." },
            { HumanResource.Intent._22_AccruedVacationTimeAfterAbsence, "Paid vacation time does not accumulate during an employee’s personal leave of absence or periods of administrative leave." },
            { HumanResource.Intent._23_ManagerResponsibilityUponVacationRequest, "A: Managers are responsible for receiving vacation requests, reviewing and approving or denying them, providing documented confirmation, and monitoring vacation time. They must also inform Payroll of any changes or revisions to approved vacation plans." },
            { HumanResource.Intent._24_CanManagerDenyVacationRequest, "Yes, vacation requests are subject to supervisory approval, and the company reserves the right to prioritize based on business needs and seniority. If a mutually acceptable time cannot be found, the company may decide when you will take your vacation." },
            { HumanResource.Intent._25_HowToScheduleVacationTime, "Vacation requests must be submitted in writing to your immediate manager or supervisor at least one week in advance. Approval of vacation time is subject to the supervisor’s discretion and business needs." },
            { HumanResource.Intent._26_CanVacationTimeCoverTardiness, "No, vacation time cannot be used to compensate for tardiness, unexcused absences, or sick time." },
            { HumanResource.Intent._27_VacationPolicy, "Sisu Healthcare Solutions provides paid vacation for full-time employees who work a minimum of 40 hours per week. Employees earn vacation from their first day of employment, but vacation cannot be used during the probationary period or any extensions. Vacation scheduling requires mutual agreement between employees and their supervisors." },
            { HumanResource.Intent._28_VacationInSmallIncrements, "Yes, vacation can be taken in increments as small as one hour, but cannot be used for tardiness or sick time." },
            { HumanResource.Intent.None, "Sorry, I am not ready for that question." }
        };
  }
}
