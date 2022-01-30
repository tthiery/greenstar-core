using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Core.Resources;
using GreenStar.Events;
using GreenStar.Research;

namespace GreenStar.Core.TurnEngine.Transcripts;

public class ResearchTranscript : TurnTranscript
{
    private readonly ResearchProgressEngine _progressEngine;
    private readonly IPlayerTechnologyStateLoader _stateLoader;

    public ResearchTranscript(ResearchProgressEngine progressEngine, IPlayerTechnologyStateLoader stateLoader)
    {
        _progressEngine = progressEngine;
        _stateLoader = stateLoader;
    }

    public override void Execute(Context context)
    {
        var allPlayers = context.PlayerContext.GetAllPlayers();

        foreach (var player in allPlayers)
        {
            // detect invested money of player
            var bills = IntermediateData["Billing"] as Dictionary<Guid, Invoice> ?? throw new InvalidOperationException("cannot run research funding without billing activated");
            if (bills.TryGetValue(player.Id, out var invoice))
            {
                // get tree of player
                var state = _stateLoader.Load(player.Id);

                // detect invested money of player
                var technologyInvest = invoice.Total * (state.CurrentIncomePercentage / 100);

                if (technologyInvest > ResourceAmount.Empty)
                {

                    // execute invest and retrieve level ups
                    (state, var levelUps, var remainingBudget) = _progressEngine.InvestInTechnology(state, technologyInvest);
                    // set tech tree of player
                    _stateLoader.Save(player.Id, state);

                    // bill to player including refund remaning budget
                    var invoiceItem = ((technologyInvest - remainingBudget) * -1) with
                    {
                        Name = "Research Funding"
                    };
                    invoice.Items.Add(invoiceItem);


                    // execute level ups
                    foreach (var up in levelUps)
                    {
                        var annotatedLevel = up.Technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == up.Progress.CurrentLevel);

                        var techName = up.Technology.DisplayName;
                        var levelUpEvent = up.Technology.LevelUpEvent;

                        if (annotatedLevel is not null)
                        {
                            techName = annotatedLevel.DisplayName + " " + up.Technology.DisplayName;
                            levelUpEvent = annotatedLevel.Event ?? up.Technology.LevelUpEvent;
                        }

                        // send messages
                        var text = string.Format("You now have {0} Technology ({1})", techName, up.Progress.CurrentLevel);
                        context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn, text: text);

                        // ... execute level up events
                        if (levelUpEvent is not null)
                        {
                            EventExecutor.Execute(context, player, levelUpEvent.Type, levelUpEvent.Argument, levelUpEvent.Text);
                        }
                    }
                }
                else
                {
                    context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn, text: "Oh no, you have no fundings for research");
                }
            }
        }
    }
}