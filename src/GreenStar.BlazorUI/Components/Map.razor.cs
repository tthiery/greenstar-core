using System.Threading.Tasks;

using GreenStar.AppService;
using GreenStar.Renderer;
using GreenStar.Traits;
using GreenStar.TurnEngine;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using SkiaSharp;
using SkiaSharp.Views.Blazor;

namespace GreenStar.BlazorUI.Components;

public record ActorClickEventArgs(Guid ActorId, Guid[] OtherActorIds, SelectionRequest? SelectionRequest = null);
public record SelectionRequest(string Color, bool IsPrimarySelection = false, string CorrelationId = "", Predicate<Actor>? AcceptableMatch = null);

public class MapService
{
    public SelectionRequest? SelectionRequest { get; set; } = null;

    public async Task SelectedAsync(ActorClickEventArgs selectedActor)
    {
        await (OnNewSelection?.Invoke(selectedActor) ?? Task.CompletedTask);

        SelectionRequest = null;
    }
    public void RequestSelection(SelectionRequest request)
    {
        SelectionRequest = request;
    }
    public event Func<ActorClickEventArgs, Task>? OnNewSelection;
}

public partial class Map : IDisposable
{
    [Parameter]
    public Guid GameId { get; set; } = Guid.Empty;
    [Parameter]
    public Guid PlayerId { get; set; } = Guid.Empty;
    [Inject]
    public IActorService ActorService { get; set; } = default!;
    [Inject]
    public IPlayerService PlayerService { get; set; } = default;

    [Parameter]
    public EventCallback<ActorClickEventArgs> OnActorClick { get; set; }

    private IDisposable? _disposableTurnServiceSubscription = null;
    private IDisposable? _disposableCommandServiceSubscription = null;

    private SkiaSharpRenderer _renderer = default!;
    private IEnumerable<Actor> _knownActors = [];

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _renderer = new SkiaSharpRenderer();

        _disposableTurnServiceSubscription = _turnService.TurnCompleted.Subscribe(_ =>
        {
            _knownActors = ActorService.GetAllKnownActors(GameId, PlayerId);
            System.Diagnostics.Debug.Assert(OperatingSystem.IsBrowser());
            x.Invalidate();
        });
        _disposableCommandServiceSubscription = _commandService.CommandCompleted.Subscribe(_ =>
        {
            _knownActors = ActorService.GetAllKnownActors(GameId, PlayerId);
            System.Diagnostics.Debug.Assert(OperatingSystem.IsBrowser());
            x.Invalidate();
        });
    }
    protected override Task OnParametersSetAsync()
    {
        _knownActors = ActorService.GetAllKnownActors(GameId, PlayerId);
        return base.OnParametersSetAsync();
    }


    private SKGLView x = default!;
    private bool _init = false;
    private bool _debug = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await jsRuntime.InvokeAsync<string>("mapUtils.addDefaultPreventingHandler", null);
    }

    void OnPaintSurface(SKPaintGLSurfaceEventArgs args)
    {
        var canvas = args.Surface.Canvas;
        var imageInfo = args.Info;

        _renderer.MapAlgorithm.SetViewPortSize(imageInfo.Width, imageInfo.Height);

        if (GameId != Guid.Empty)
        {
            if (_init == false)
            {
                _renderer.InitViewPort(ActorService.GetAllActors(GameId, PlayerId));

                _init = true;
            }

            _renderer.MapGame(_knownActors, PlayerService.GetAllPlayers(GameId), args.Surface.Canvas);

            if (_lastClick is not null && _debug)
            {
                _renderer.MapAlgorithm.DrawDebugAtPoint(canvas, _lastClick.Value);
                _debug = false;
            }

            if (_pointerLocation is not null)
            {
                if (_mapService?.SelectionRequest is var selectionRequest and not null)
                {
                    canvas.DrawCircle(_pointerLocation.Value, 20, new SKPaint { Color = SKColor.Parse(selectionRequest.Color), Style = SKPaintStyle.Stroke });
                }
            }
        }
    }

    private SKPoint? _lastClick;

    void OnWheelAsync(WheelEventArgs ea)
    {
        if (ea.DeltaY > 0)
        {
            _renderer.MapAlgorithm.ZoomOut((float)ea.OffsetX, (float)ea.OffsetY);
        }
        else if (ea.DeltaY < 0)
        {
            _renderer.MapAlgorithm.ZoomIn((float)ea.OffsetX, (float)ea.OffsetY);
        }
        System.Diagnostics.Debug.Assert(OperatingSystem.IsBrowser());
        x.Invalidate();
    }
    async Task OnClick(MouseEventArgs args)
    {
        _lastClick = null;

        if (args.AltKey)
        {
            var moveToPoint = new SKPoint((float)args.OffsetX, (float)args.OffsetY);
            var moveToCoordinate = _renderer.MapAlgorithm.MapPointToCoordinate(moveToPoint);
            var centerCoordinate = _renderer.MapAlgorithm.MapPointToCoordinate(new SKPoint(_renderer.MapAlgorithm.ViewPort.Width / 2, _renderer.MapAlgorithm.ViewPort.Height / 2));
            var delta = centerCoordinate - moveToCoordinate;

            _renderer.MapAlgorithm.MoveViewPortByLogicalVector(delta);

        }
        else if (args.CtrlKey)
        {
            _debug = true;
            _lastClick = new SKPoint((float)args.OffsetX, (float)args.OffsetY);
        }
        else
        {
            _lastClick = new SKPoint((float)args.OffsetX, (float)args.OffsetY);

            if (ActorService is not null)
            {
                var x1 = _lastClick.Value.X - 5;
                var x2 = _lastClick.Value.X + 5;
                var y1 = _lastClick.Value.Y - 5;
                var y2 = _lastClick.Value.Y + 5;

                var topLeft = _renderer.MapAlgorithm.MapPointToCoordinate(new SKPoint(x1, y1));
                var bottomRight = _renderer.MapAlgorithm.MapPointToCoordinate(new SKPoint(x2, y2));

                var foundActors = ActorService.GetKnownActorInRectangle(GameId, PlayerId, topLeft, bottomRight);

                // TODO: What to prefer here? a locatable with own position or a locatable with host location? What if multiple are there hiding in the UI behind each other?
                if (foundActors.FirstOrDefault() is Actor p)
                {
                    var otherIds = foundActors.Where(a => a.Id != p.Id).Select(a => a.Id).ToArray();
                    if (_mapService?.SelectionRequest is var selectionRequest and not null)
                    {
                        if (selectionRequest.AcceptableMatch is not null && selectionRequest.AcceptableMatch(p))
                        {
                            var ea = new ActorClickEventArgs(p.Id, otherIds, selectionRequest);

                            if (selectionRequest.IsPrimarySelection)
                            {
                                await OnActorClick.InvokeAsync(ea);
                            }

                            await (_mapService?.SelectedAsync(ea) ?? Task.CompletedTask);
                        }
                    }
                    else
                    {
                        var ea = new ActorClickEventArgs(p.Id, otherIds);
                        await OnActorClick.InvokeAsync(ea);
                    }
                }
            }
        }

        System.Diagnostics.Debug.Assert(OperatingSystem.IsBrowser());
        x.Invalidate();
    }

    private SKPoint? _pointerDown;
    private SKPoint? _pointerLocation;

    private void OnPointer(PointerEventArgs args)
    {
        if (args.Type == "pointerdown")
        {
            _pointerDown = new SKPoint((float)args.OffsetX, (float)args.OffsetY);
        }
        else if (args.Type == "pointerup")
        {
            _pointerDown = null;
        }
        else if (args.Type == "pointerleave")
        {
            _pointerDown = null;
            _pointerLocation = null;
        }
        else if (args.Type == "pointermove")
        {
            _pointerLocation = new SKPoint((float)args.OffsetX, (float)args.OffsetY);

            if (_pointerDown is not null)
            {
                var moveToPoint = _pointerLocation.Value;
                var moveToCoordinate = _renderer.MapAlgorithm.MapPointToCoordinate(moveToPoint);
                var lastClickCoordinate = _renderer.MapAlgorithm.MapPointToCoordinate(_pointerDown.Value);
                var delta = moveToCoordinate - lastClickCoordinate;

                _renderer.MapAlgorithm.MoveViewPortByLogicalVector(delta);

                _pointerDown = moveToPoint;
            }
        }

        System.Diagnostics.Debug.Assert(OperatingSystem.IsBrowser());
        x.Invalidate();
    }

    public void Dispose()
    {
        _disposableTurnServiceSubscription?.Dispose();
        _disposableCommandServiceSubscription?.Dispose();
    }
}