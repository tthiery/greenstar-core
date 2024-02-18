using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

//TODO: in scale this is the worst possible implementation

using GreenStar.TurnEngine;

namespace GreenStar.Persistence;

public record DehydratedFile(Guid Id, string GameType, int Turn, IEnumerable<DehydratedPlayer> Players, IEnumerable<DehydratedActor> Actors);

public record DehydratedActor(Guid Id, string ActorType, DehydratedTrait[] Traits);
public record DehydratedPlayer(Guid Id, string PlayerType, int CompletedTurn, DehydratedTrait[] Traits);
public record DehydratedTrait(string TraitType, DehydratedTraitProperty[] Properties);
public record DehydratedTraitProperty(string Name, string Value);

public class FileSystemPersistence : IPersistence
{
    private readonly Guid _gameId;
    private readonly string _gameType;
    private readonly ActorTypeDictionary _actorTypeDictionary;

    public FileSystemPersistence(Guid gameId, string gameType, ActorTypeDictionary actorTypeDictionary)
    {
        _gameId = gameId;
        _gameType = gameType;
        _actorTypeDictionary = actorTypeDictionary;
    }

    public string FileName
        => $"save_core_{_gameType}_{_gameId}.json";

    public async Task PersistFullAsync(ITurnContext turnContext, IPlayerContext playerContext, IActorContext actorContext)
    {
        var playerStream = DehydratePlayerContext(playerContext);
        var actorStream = DehydrateActorContext(actorContext);

        var file = new DehydratedFile(Guid.Empty, string.Empty, turnContext.Turn, playerStream, actorStream);

        using var fileStream = new FileStream(FileName, FileMode.Create, FileAccess.Write);

        await JsonSerializer.SerializeAsync<DehydratedFile>(fileStream, file);

        fileStream.Flush();
        fileStream.Close();
    }

    public IEnumerable<DehydratedActor> DehydrateActorContext(IActorContext actorContext)
    {
        foreach (var actor in actorContext.AsQueryable())
        {
            var id = actor.Id;
            var actorType = actor.GetType().Name ?? throw new NotSupportedException("invalid type of actor");
            var traitsToPersist = DehydrateTraits(actor.Traits);

            yield return new DehydratedActor(id, actorType, traitsToPersist);
        }
    }

    public IEnumerable<DehydratedPlayer> DehydratePlayerContext(IPlayerContext playerContext)
    {
        foreach (var player in playerContext.GetAllPlayers())
        {
            var traitsToPersist = DehydrateTraits(new Trait[] { player.IdealConditions, player.Relatable, player.Resourceful, player.Capable });

            yield return new DehydratedPlayer(player.Id, player.GetType().FullName ?? throw new NotSupportedException("invalid type of player"), player.CompletedTurn, traitsToPersist);
        }
    }


    private static DehydratedTrait[] DehydrateTraits(IEnumerable<Trait> traits)
    {
        var traitsToPersist = new List<DehydratedTrait>();

        foreach (var trait in traits)
        {
            var traitType = trait.GetType().Name ?? throw new NotSupportedException("invalid type of trait");

            var propertyWriter = new PropertyWriter();
            trait.Persist(propertyWriter);

            if (propertyWriter.IsUsed)
            {
                traitsToPersist.Add(new DehydratedTrait(traitType, propertyWriter.Result));
            }
        }

        return traitsToPersist.ToArray();
    }

    public async Task<(int, IEnumerable<Player>, IEnumerable<Actor>)> LoadFullAsync()
    {
        using var fileStream = File.OpenRead(FileName);

        var file = await JsonSerializer.DeserializeAsync<DehydratedFile>(fileStream);

        fileStream?.Close();

        return (
            file.Turn,
            file.Players.Select(HydratePlayer),
            file.Actors.Select(HydrateActor)
        );
    }

    public Player HydratePlayer(DehydratedPlayer player)
    {
        var playerType = Type.GetType(player.PlayerType) ?? throw new InvalidOperationException($"Unknown Player Type {player.PlayerType}");

        var playerObj = Activator.CreateInstance(playerType) as Player ?? throw new InvalidOperationException($"Unable to instanciate {playerType.FullName}");
        playerObj.Id = player.Id;
        playerObj.CompletedTurn = player.CompletedTurn;

        HydrateTraits(null, [playerObj.IdealConditions, playerObj.Relatable, playerObj.Resourceful, playerObj.Capable], player.Traits);

        return playerObj;
    }
    public Actor HydrateActor(DehydratedActor actor)
    {
        if (_actorTypeDictionary.Actors.TryGetValue(actor.ActorType, out var actorType))
        {

            var actorObj = Activator.CreateInstance(actorType) as Actor ?? throw new InvalidOperationException($"Unable to instantiate {actorType.FullName}");
            actorObj.Id = actor.Id;

            HydrateTraits(actorObj, actorObj.Traits, actor.Traits);

            return actorObj;
        }
        else
        {
            throw new InvalidOperationException($"Unknown Actor Type {actor.ActorType}");
        }
    }

    private void HydrateTraits(Actor? actor, IEnumerable<Trait> existingTraits, DehydratedTrait[] traits)
    {
        foreach (var t in traits)
        {
            if (_actorTypeDictionary.Traits.TryGetValue(t.TraitType, out var traitType))
            {

                // try load auto-created with actor
                var trait = existingTraits.FirstOrDefault(to => to.GetType().Name == t.TraitType);

                // add dynammically applied ones
                bool newTrait = trait is null;
                trait ??= Activator.CreateInstance(traitType) as Trait;

                if (trait is not null)
                {
                    var propertyReader = new PropertyReader(t.Properties);
                    trait.Load(propertyReader);

                    if (newTrait && actor is not null)
                    {
                        actor.AddTrait(trait);
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"Unknown Trait Type {t.TraitType}");
            }

        }
    }
}

public class PropertyReader : IPersistenceReader
{
    private readonly DehydratedTraitProperty[] _properties;

    public PropertyReader(DehydratedTraitProperty[] properties)
    {
        _properties = properties;
    }

    public T Read<T>(string property)
    {
        var p = _properties.FirstOrDefault(i => i.Name == property);

        var t = typeof(T);

        object? result = (t, p?.Value) switch
        {
            (_, null) => default(T),
            ({ Name: "Boolean" }, var value) => bool.TryParse(value, out var b) ? b : false,
            ({ Name: "String" }, var value) => value,
            ({ Name: "Int16" }, var value) => short.TryParse(value, out var i16) ? i16 : 0,
            ({ Name: "Int32" }, var value) => int.TryParse(value, out var i32) ? i32 : 0,
            ({ Name: "Int64" }, var value) => long.TryParse(value, out var i64) ? i64 : 0L,
            ({ Name: "Single" }, var value) => float.TryParse(value, out var f32) ? f32 : 0f,
            ({ Name: "Double" }, var value) => double.TryParse(value, out var f64) ? f64 : 0d,
            ({ Name: "Guid" }, var value) => Guid.TryParse(value, out var guid) ? guid : Guid.Empty,
            ({ IsEnum: true }, var value) => Enum.TryParse(t, value, out var enumValue) ? enumValue : default(T),
            ({ Name: "IEnumerable`1", IsGenericType: true, GenericTypeArguments: [Type { Name: "Guid" }] }, var value) => value.Split(",").Select(s => Guid.TryParse(s, out var g) ? g : Guid.Empty).ToArray(),
            _ => throw new InvalidOperationException($"not supported type conversion: {t.Name}"),
        };

        if (result is null)
        {
            throw new InvalidOperationException("Invalid Value Persisted (resulted in null value)");
        }

        return (T)result;
    }

    public IEnumerable<string> ReadPropertyNames(string? prefix = null)
        => _properties.Select(p => p.Name);
}

public class PropertyWriter : IPersistenceWriter
{
    private readonly List<DehydratedTraitProperty> _list = new();

    public void Write<T>(string property, T value)
    {
        if (value is not null)
        {
            _list.Add(WriteToProperty(property, value));
        }
    }

    public bool IsUsed => _list.Count > 0;
    public DehydratedTraitProperty WriteToProperty(string property, object value)
        => value switch
        {
            bool b => new DehydratedTraitProperty(property, Convert.ToString(b)),
            string s => new DehydratedTraitProperty(property, s),
            int i => new DehydratedTraitProperty(property, Convert.ToString(i)),
            long l => new DehydratedTraitProperty(property, Convert.ToString(l)),
            float f => new DehydratedTraitProperty(property, Convert.ToString(f)),
            double d => new DehydratedTraitProperty(property, Convert.ToString(d)),
            Guid g => new DehydratedTraitProperty(property, g.ToString()),
            Enum e => new DehydratedTraitProperty(property, e.ToString()),
            IEnumerable<Guid> ga => new DehydratedTraitProperty(property, String.Join(",", ga.Select(i => i.ToString()))),
            _ => throw new NotSupportedException("not supported data type in dehydration"),
        };

    public DehydratedTraitProperty[] Result
        => _list.ToArray();
}