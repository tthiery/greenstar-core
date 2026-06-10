using System;
using System.Linq;
using System.Threading.Tasks;

using GreenStar.Traits;
using GreenStar.TurnEngine;

namespace GreenStar.Transcripts;

public record SetPrimarySelectionCommand(Guid ActorId)
    : Command("cmd-set-primary-selection", "Set Primary Selection", ActorId, []);

public class SetPrimarySelectionCommandTranscript : CommandTranscript<SetPrimarySelectionCommand>
{
    public SetPrimarySelectionCommandTranscript(SetPrimarySelectionCommand command)
        : base(command)
    { }

    public override Task ExecuteAsync(Context context, SetPrimarySelectionCommand command)
    {
        // remove old primary selection
        var previousSelectedItems = context.ActorContext.GetActors<SelectedItem, Locatable>().ToArray();
        foreach (var previousSelectedItem in previousSelectedItems)
        {
            context.ActorContext.RemoveActor(previousSelectedItem);
        }

        // add new primary selection
        var item = new SelectedItem();
        item.Trait<Locatable>().SetPosition(command.ActorId);
        context.ActorContext.AddActor(item);

        return Task.CompletedTask;
    }
}
