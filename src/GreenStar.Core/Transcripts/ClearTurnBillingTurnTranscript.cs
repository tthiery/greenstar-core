using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GreenStar.Resources;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

/// <summary>
/// Clear the bills from the turn to the resources of the user
/// </summary>
public class ClearTurnBillingTurnTranscript : TurnTranscript
{
    /// <summary>
    /// Clear the bills from the turn to the resources of the user
    /// </summary>
    public override Task ExecuteAsync(Context context)
    {
        if (this.IntermediateData == null)
        {
            throw new System.InvalidOperationException("ClearTurnBilling.IntermediateData is not set");
        }

        var bills = IntermediateData["Billing"] as Dictionary<Guid, Invoice>;

        if (bills != null)
        {
            foreach (var player in context.PlayerContext.GetAllPlayers())
            {
                var turnBill = bills[player.Id];

                if (turnBill != null)
                {
                    player.Resources = player.Resources + turnBill.Total;

                    context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn,
                        type: "Invoice",
                        text: $"You had a revenue of {turnBill.Total}",
                        data: turnBill
                    );
                }
            }
        }

        return Task.CompletedTask;
    }
}
