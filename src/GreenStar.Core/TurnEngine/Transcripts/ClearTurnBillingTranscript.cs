using System;
using System.Collections.Generic;

using GreenStar.Core.Resources;

namespace GreenStar.Core.TurnEngine.Transcripts;

/// <summary>
/// Clear the bills from the turn to the resources of the user
/// </summary>
public class ClearTurnBillingTranscript : TurnTranscript
{
    /// <summary>
    /// Clear the bills from the turn to the resources of the user
    /// </summary>
    public override void Execute(Context context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if (context.PlayerContext == null)
        {
            throw new System.InvalidOperationException("context.PlayerContext is not set");
        }
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
    }
}
