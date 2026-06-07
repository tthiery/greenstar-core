using GreenStar.AppService.Turn;

using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic;

namespace GreenStar.BlazorUI.Components.GamePanels;

public partial class HistoryPanel : IDisposable
{
    [Inject]
    protected ITurnService TurnService { get; set; } = default!;

    [Parameter]
    public Guid GameId { get; set; }
    [Parameter]
    public Guid PlayerId { get; set; }

    public GreenStar.AppService.Turn.Information? Information { get; set; } = default;

    private IDisposable? _disposable = null;

    protected override Task OnInitializedAsync()
    {
        Information = TurnService.Information(GameId, PlayerId);

        _disposable = TurnService.TurnCompleted.Subscribe(_ =>
        {
            Information = TurnService.Information(GameId, PlayerId);

            StateHasChanged();
        });

        return base.OnInitializedAsync();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}