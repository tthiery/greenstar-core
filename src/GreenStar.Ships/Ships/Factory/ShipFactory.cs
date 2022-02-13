using System;
using System.Reflection;

using GreenStar.Resources;
using GreenStar.Traits;
using GreenStar.Research;
using System.Threading.Tasks;

namespace GreenStar.Ships.Factory;

public class ShipFactory
{
    private readonly IPlayerTechnologyStateLoader _playerTechnologyStateLoader;

    public ShipFactory(IPlayerTechnologyStateLoader playerTechnologyStateLoader)
    {
        _playerTechnologyStateLoader = playerTechnologyStateLoader;
    }

    public async Task<Blueprint?> GetBlueprintAsync(Guid playerId, string className)
    {
        var state = await _playerTechnologyStateLoader.LoadAsync(playerId);
        var technology = state.FindTechnologyByName(className);

        if (technology is not null)
        {
            var data = technology.TechnologyData ?? throw new InvalidOperationException("blueprints need technology data");
            var array = data.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var costs = new ResourceAmount(
                new ResourceAmountItem(ResourceConstants.Money, Convert.ToInt32(array[1])),
                new ResourceAmountItem(ResourceConstants.Metal, Convert.ToInt32(array[2]))
            );
            var capable = new Capable(new[] {
                ShipCapabilities.Range,
                ShipCapabilities.Speed,
                ShipCapabilities.Attack,
                ShipCapabilities.Defense,
                ShipCapabilities.Mini
            });
            capable.Of(ShipCapabilities.Range, Convert.ToInt32(array[3]));
            capable.Of(ShipCapabilities.Speed, Convert.ToInt32(array[4]));
            capable.Of(ShipCapabilities.Attack, Convert.ToInt32(array[5]));
            capable.Of(ShipCapabilities.Defense, Convert.ToInt32(array[6]));
            capable.Of(ShipCapabilities.Mini, Convert.ToInt32(array[7]));

            return new Blueprint(className, array[0], costs, capable);
        }
        else
        {
            return null;
        }
    }

    public async Task<Ship> CreateShipAsync(Guid playerId, string className, string name)
    {
        Ship result;

        var blueprint = await GetBlueprintAsync(playerId, className);

        if (blueprint != null)
        {
            var t = Assembly.GetExecutingAssembly().GetType($"GreenStar.Ships.{blueprint.ClassType}") ?? throw new InvalidOperationException("invalid blueprint (class)");
            var ship = Activator.CreateInstance(t) as Ship ?? throw new InvalidOperationException("invalid blueprint (wrong class)");

            foreach (var capabilityName in blueprint.Capable.CapabilityNames)
            {
                ship.Trait<Capable>().Of(capabilityName, blueprint.Capable.Of(capabilityName));
            }

            if (ship is VectorShip)
            {
                ship.Trait<VectorFlightCapable>().Fuel = ship.Trait<Capable>().Of(ShipCapabilities.Range);
            }

            result = ship;
        }
        else
        {
            throw new InvalidOperationException("unknown blueprint");
        }

        return result;
    }
}
