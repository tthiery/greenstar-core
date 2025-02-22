using GreenStar.AppService;
using GreenStar.AppService.Actor;
using GreenStar.AppService.Turn;
using GreenStar.Resources;
using GreenStar.Traits;

using Microsoft.AspNetCore.Components;

namespace GreenStar.BlazorUI.Components;

public partial class PropertyPanel : IDisposable
{
    [Inject]
    protected ICommandService CommandService { get; set; } = default!;
    [Inject]
    protected ITurnService TurnService { get; set; } = default!;
    public IEnumerable<Command> Commands
        => CommandService.GetAllCommands(GameId, PlayerId, ActorId);

    [Parameter]
    public Guid GameId { get; set; }
    [Parameter]
    public Guid PlayerId { get; set; }
    [Parameter]
    public Guid ActorId { get; set; }

    public Actor? Actor { get; set; } = null;

    public string Name
        => Actor?.TryGetTrait<Nameable>(out var nameable) ?? false
            ? nameable.Name
            : Actor?.GetType().Name ?? string.Empty;
    public ResourceAmount? Resources
        => Actor?.TryGetTrait<Resourceful>(out var resourceful) ?? false ? resourceful.Resources : ResourceAmount.Empty;
    public Populatable? Populatable
        => Actor?.TryGetTrait<Populatable>(out var populatable) ?? false ? populatable : null;
    public Associatable? Associatable
        => Actor?.TryGetTrait<Associatable>(out var associatable) ?? false ? associatable : null;
    public Hospitality? Hospitality
        => Actor?.TryGetTrait<Hospitality>(out var hospitality) ?? false ? hospitality : null;

    protected override void OnParametersSet()
    {
        LoadActor();
    }

    private void LoadActor()
    {
        if (ActorId != Guid.Empty)
        {
            if (GameHolder.Games.TryGetValue(GameId, out var game))
            {
                Actor = game.Actors.GetActor(ActorId);
            }
        }
    }

    private IDisposable? _disposable = null;

    protected override Task OnInitializedAsync()
    {
        _disposable = TurnService.TurnCompleted.Subscribe(_ =>
        {
            LoadActor();

            StateHasChanged();
        });
        return base.OnInitializedAsync();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}