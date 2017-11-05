using System;
using System.Collections.Generic;
using GreenStar.Core.Resources;

namespace GreenStar.Core.TurnEngine.Transcripts
{
    /// <summary>
    /// Creates for each player a billd
    /// </summary>
    public class CreateTurnBillingTranscript : TurnTranscript
    {
        /// <summary>
        /// Creates for each player a billd
        /// </summary>
        public override void Execute(Context context)
        {
            if (this.Game == null)
            {
                throw new System.InvalidOperationException("CreateTurnBilling.Game is not set");
            }
            if (this.Game.Players == null)
            {
                throw new System.InvalidOperationException("CreateTurnBilling.Game.Players is not set");
            }
            if (this.IntermediateData == null)
            {
                throw new System.InvalidOperationException("CreateTurnBilling.IntermediateData is not set");
            }

            var bills = new Dictionary<Guid, Invoice>();

            foreach (var player in Game.Players)
            {
                bills.Add(player.Id, new Invoice() { Name = "Turn Bill" });
            }

            IntermediateData.Add("Billing", bills);
        }
    }
}
