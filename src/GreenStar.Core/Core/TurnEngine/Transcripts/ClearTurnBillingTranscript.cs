using System.Collections.Generic;
using GreenStar.Core.Resources;
using System;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    /// <summary>
    /// Clear the bills from the turn to the resources of the user
    /// </summary>
    public class ClearTurnBillingTranscript : TurnTranscript
    {
        /// <summary>
        /// Clear the bills from the turn to the resources of the user
        /// </summary>
        public override void Execute()
        {
            if (this.Game == null)
            {
                throw new System.InvalidOperationException("ClearTurnBilling.Game is not set");
            }
            if (this.Game.Players == null)
            {
                throw new System.InvalidOperationException("ClearTurnBilling.Game.Players is not set");
            }
            if (this.IntermediateData == null)
            {
                throw new System.InvalidOperationException("ClearTurnBilling.IntermediateData is not set");
            }

            var bills = IntermediateData["Billing"] as Dictionary<Guid, Invoice>;

            if (bills != null)
            {
                foreach (var player in Game.Players)
                {
                    var turnBill = bills[player.Id];

                    if (turnBill != null)
                    {
                        player.Resources = player.Resources + turnBill.Total;

                        PlayerContext.SendMessageToPlayer(player.Id,
                            type: "Invoice",
                            text: $"You had a revenue of {turnBill.Total}",
                            data: turnBill
                        );
                    }
                }
            }
        }
    }
}
