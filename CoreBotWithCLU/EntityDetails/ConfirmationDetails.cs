// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.BotBuilderSamples;

namespace CoreBotCLU.EntityDetails
{
    public class ConfirmationDetails
    {
        public bool Confirmed { get; set; }

        public HumanResource.Intent Intent { get; set; }
    }
}
