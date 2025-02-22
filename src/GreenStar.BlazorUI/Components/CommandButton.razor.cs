using GreenStar.AppService.Actor;
using GreenStar.Traits;

using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

using Icons = Microsoft.FluentUI.AspNetCore.Components.Icons;

namespace GreenStar.BlazorUI.Components;

public partial class CommandButton
{
    [Inject]
    protected IToastService ToastService { get; set; } = default!;
    [Inject]
    protected MapService MapService { get; set; } = default!;
    [Inject]
    protected ICommandService CommandDomainService { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }
    [Parameter]
    public Guid PlayerId { get; set; }
    [Parameter]
    public Guid ActorId { get; set; }
    [Parameter]
    public Command Command { get; set; } = default!;

    public bool InSelection { get; set; } = false;

    public Icon IconEnd
        => InSelection
            ? new Icons.Regular.Size20.VideoPlayPause()
            : new Icons.Regular.Size20.Play();

    public async Task OnClickAsync()
    {
        // Cancel it
        if (InSelection == true)
        {
            InSelection = false;
        }
        // if it is an argument free command, execute it straight
        else if (Command.Arguments.Where(a => string.IsNullOrWhiteSpace(a.Value)).Count() == 0)
        {
            InSelection = false;

            await CommandDomainService.ExecuteCommandAsync(GameId, PlayerId, Command);
        }
        // if there are arguments, request them from the map
        else
        {
            InSelection = true;

            // TODO: Extend map to select more than one item.
            foreach (var argument in Command.Arguments.Where(a => string.IsNullOrWhiteSpace(a.Value)))
            {
                ToastService.ShowInfo($"Please select '{argument.Name}' on the map!");

                MapService.RequestSelection(new SelectionRequest("FF0000", false, ActorId.ToString() + "/" + Command.Id + "/" + argument.Name, actor => argument.DataType switch
                {
                    CommandArgumentDataType.LocatableAndHospitableReference => actor.TryGetTrait(out Locatable? _) && actor.TryGetTrait(out Hospitality? _),
                    CommandArgumentDataType.ActorReference => true,
                    _ => false,
                }));
            }
        }
    }

    public async Task OnNewSelection(ActorClickEventArgs actorClickedEventArgs)
    {
        if (InSelection)
        {
            var correlation = actorClickedEventArgs.SelectionRequest.CorrelationId.Split('/');

            if (Command.ActorId == Guid.Parse(correlation[0]) && Command.Id == correlation[1])
            {
                Command = Command with
                {
                    Arguments = Command.Arguments.Select(a => a.Name == correlation[2] ? a with { Value = actorClickedEventArgs.ActorId.ToString() } : a).ToArray(),
                };

                InSelection = false;

                ToastService.ClearAll();

                if (Command.Arguments.Where(a => string.IsNullOrWhiteSpace(a.Value)).Count() == 0)
                {
                    await CommandDomainService.ExecuteCommandAsync(GameId, PlayerId, Command);
                }
            }

            StateHasChanged();
        }
    }

    protected override void OnInitialized()
    {
        MapService.OnNewSelection += OnNewSelection;
    }

    public void Dispose()
    {
        MapService.OnNewSelection -= OnNewSelection;
    }
}