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
            var technologyInvest = new ResourceAmount(new ResourceAmountItem("Money", 100)); // TODO

            // get tree of player
            var state = _stateLoader.Load(player.Id);
            // execute invest and retrieve level ups
            (state, var levelUps) = _progressEngine.InvestInTechnology(context, state, technologyInvest);
            // set tech tree of player
            _stateLoader.Save(player.Id, state);

            // execute level ups
            foreach (var up in levelUps)
            {
                var annotatedLevel = up.Technology.AnnotatedLevels?.FirstOrDefault(al => al.Level == up.Progress.CurrentLevel);

                (string techName, TechnologyEvent? levelUpEvent) = (annotatedLevel, up.Technology) switch
                {
                    (null, var technology) => (technology.DisplayName, technology.LevelUpEvent),
                    (var al, var technology) => (al.DisplayName + " " + technology.DisplayName, al.Event ?? technology.LevelUpEvent),
                };

                // send messages
                // Your now have <nicknameRange Technology (<level>)
                var text = string.Format("You now have {0} Technology ({1})", techName, up.Progress.CurrentLevel);
                context.PlayerContext.SendMessageToPlayer(player.Id, year: context.TurnContext.Turn, text: text);

                // ... execute level up events
                if (levelUpEvent is not null)
                {
                    EventExecutor.Execute(context, player, levelUpEvent.Type, levelUpEvent.Argument, levelUpEvent.Text);
                }
            }
        }
    }
}