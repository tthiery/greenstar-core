using GreenStar.AppService.Turn;

using Microsoft.AspNetCore.Components;

namespace GreenStar.BlazorUI.Components.GamePanels;

public partial class GamePanel : IDisposable
{
    [Inject]
    protected ITurnService TurnService { get; set; } = default!;


    [Parameter]
    public Guid GameId { get; set; }
    [Parameter]
    public Guid PlayerId { get; set; }
    [Parameter]
    public Guid ActorId { get; set; }
    [Parameter]
    public Guid[] OtherFoundActorIds { get; set; }


    public void OnFinishTurn()
    {
        TurnService.Finish(GameId, PlayerId);
    }


    private IDisposable? _disposable = null;

    protected override Task OnInitializedAsync()
    {
        _disposable = TurnService.TurnCompleted.Subscribe(_ =>
        {
            StateHasChanged();
        });
        return base.OnInitializedAsync();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}