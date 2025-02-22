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

public record ActorClickEventArgs(Guid ActorId, SelectionRequest? SelectionRequest = null);
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
    public EventCallback<ActorClickEventArgs> OnActorClick { get; set; }

    private IDisposable? _disposable = null;

    private TurnManager _game;
    private SkiaSharpRenderer _renderer;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _renderer = new SkiaSharpRenderer();

        _disposable = _turnService.TurnCompleted.Subscribe(_ =>
        {
            x.Invalidate();
        });
    }
    protected override Task OnParametersSetAsync()
    {
        if (GameId != Guid.Empty)
        {
            _game = GameHolder.Games[GameId];
        }

        return base.OnParametersSetAsync();
    }


    private SKGLView x;
    private ElementReference y;
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
                _renderer.InitViewPort(GameId);

                _init = true;
            }

            _renderer.MapGame(GameId, args.Surface.Canvas);

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


            var actorContext = GreenStar.AppService.GameHolder.Games[GameId].Actors as IActorContext;

            var x1 = (long)_lastClick.Value.X - 5;
            var x2 = (long)_lastClick.Value.X + 5;
            var y1 = (long)_lastClick.Value.Y - 5;
            var y2 = (long)_lastClick.Value.Y + 5;


            var foundActors = actorContext.GetActors<Actor, Locatable>(trait =>
            {
                var other = trait.GetPosition(actorContext);
                var screenOther = _renderer.MapAlgorithm.MapCoordinateToPoint(other);

                return (screenOther.X > x1 && screenOther.X < x2 && screenOther.Y > y1 && screenOther.Y < y2);
            });

            if (foundActors.FirstOrDefault() is Actor p)
            {
                var item = new SelectedItem();
                item.Trait<Locatable>().SetPosition(p.Id);

                actorContext.AddActor(item);

                if (_mapService?.SelectionRequest is var selectionRequest and not null)
                {
                    if (selectionRequest.AcceptableMatch is not null && selectionRequest.AcceptableMatch(p))
                    {
                        var ea = new ActorClickEventArgs(p.Id, selectionRequest);

                        if (selectionRequest.IsPrimarySelection)
                        {
                            await OnActorClick.InvokeAsync(ea);
                        }

                        await (_mapService?.SelectedAsync(ea) ?? Task.CompletedTask);
                    }
                }
                else
                {
                    var ea = new ActorClickEventArgs(p.Id);
                    await OnActorClick.InvokeAsync(ea);
                }
            }
        }

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

        x.Invalidate();
    }

    public void Dispose()
    {
        _disposable?.Dispose();
    }
}