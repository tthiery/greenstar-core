using System;
using System.Collections.Generic;

using GreenStar.Resources;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

/// <summary>
/// Creates for each player a billd
/// </summary>
public class CreateTurnBillingTurnTranscript : TurnTranscript
{
    /// <summary>
    /// Creates for each player a billd
    /// </summary>
    public override void Execute(Context context)
    {
        if (this.IntermediateData == null)
        {
            throw new System.InvalidOperationException("CreateTurnBilling.IntermediateData is not set");
        }

        var bills = new Dictionary<Guid, Invoice>();

        foreach (var player in context.PlayerContext.GetAllPlayers())
        {
            bills.Add(player.Id, new Invoice() { Name = "Turn Bill" });
        }

        IntermediateData.Add("Billing", bills);
    }
}
