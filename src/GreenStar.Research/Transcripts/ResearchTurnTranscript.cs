using System;
using System.Collections.Generic;
using System.Linq;

using GreenStar.Resources;
using GreenStar.Research;
using GreenStar.TurnEngine;
using System.Threading.Tasks;

namespace GreenStar.Transcripts;

public class ResearchTurnTranscript : TurnTranscript
{
    private readonly ResearchProgressEngine _progressEngine;
    private readonly IPlayerTechnologyStateLoader _stateLoader;

    public ResearchTurnTranscript(ResearchProgressEngine progressEngine, IPlayerTechnologyStateLoader stateLoader)
    {
        _progressEngine = progressEngine;
        _stateLoader = stateLoader;
    }

    public override async Task ExecuteAsync(Context context)
    {
        var allPlayers = context.PlayerContext.GetAllPlayers();

        foreach (var player in allPlayers)
        {
            // detect invested money of player
            var bills = IntermediateData["Billing"] as Dictionary<Guid, Invoice> ?? throw new InvalidOperationException("cannot run research funding without billing activated");
            if (bills.TryGetValue(player.Id, out var invoice))
            {
                // get tree of player
                var state = await _stateLoader.LoadAsync(player.Id);

                // ensure that the budget is complete and thresholds are set
                state = _progressEngine.AdjustBudgetAndDetermineThresholds(state);

                // detect invested money of player
                var technologyInvest = invoice.Total * (state.CurrentIncomePercentage / 100);

                if (technologyInvest > ResourceAmount.Empty)
                {
                    // execute invest and retrieve level ups
                    (state, var levelUps, var remainingBudget) = _progressEngine.InvestInTechnology(state, technologyInvest);
                    // set tech tree of player
                    await _stateLoader.SaveAsync(player.Id, state);

                    // bill to player including refund remaning budget
                    var invoiceItem = ((technologyInvest - remainingBudget) * -1) with
                    {
                        Name = "Research Funding"
                    };
                    invoice.Items.Add(invoiceItem);

                    // execute level ups
                    foreach (var up in levelUps)
                    {
                        await ExecuteLevelUpAsync(context, player, up);
                    }
                }
                else
                {
                    context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn, text: "Oh no, you have no fundings for research");
                }
            }
        }
    }

    public static async Task ExecuteLevelUpAsync(Context context, Player player, TechnologyLevelUp up)
    {
        // set player capable level
        if (!player.Capable.CapabilityNames.Any(n => n == up.Technology.Name))
        {
            player.Capable.AddCapability(up.Technology.Name);
        }

        player.Capable.Of(up.Technology.Name, up.Progress.CurrentLevel);

        // if the reached technology level has a special annotation
        var lastAnnotatedLevel = up.Technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == up.Progress.CurrentLevel);

        var techName = up.Technology.DisplayName;

        if (lastAnnotatedLevel is not null)
        {
            techName = lastAnnotatedLevel.DisplayName + " " + up.Technology.DisplayName;
        }

        // send messages
        var text = string.Format("You now have {0} Technology ({1})", techName, up.Progress.CurrentLevel);
        context.PlayerContext.SendMessageToPlayer(player.Id, context.TurnContext.Turn, text: text);

        // ... execute level up events in between the last and the current level
        var levelUpEvents = Enumerable
            .Range(up.PreviousProgress.CurrentLevel + 1, up.Progress.CurrentLevel)
            .Select(level => up.Technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == level)?.Event ?? up.Technology.LevelUpEvent)
            .Where(e => e is not null);

        foreach (var levelUpEvent in levelUpEvents)
        {
            if (levelUpEvent is not null)
            {
                await context.TurnContext.ExecuteEventAsync(context, player, levelUpEvent.Type, levelUpEvent.Arguments, levelUpEvent.Text);
            }
        }
    }
}