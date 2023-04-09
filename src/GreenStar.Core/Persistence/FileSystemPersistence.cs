using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreenStar.Persistence;

public record DehydratedActor(Guid Id, string ActorType, DehydratedTrait[] Traits);
public record DehydratedPlayer(Guid Id, string PlayerType, int CompletedTurn, DehydratedTrait[] Traits);
public record DehydratedTrait(string TraitType, DehydratedTraitProperty[] Properties);
public record DehydratedTraitProperty(string Name, string Value);

public class FileSystemPersistence : IPersistence
{
    public async Task PersistFullAsync(ITurnContext turnContext, IPlayerContext playerContext, IActorContext actorContext)
    {
        var playerList = DehydratePlayerContext(playerContext);
        var actorList = DehydrateActorContext(actorContext);

        using var fileStream = new FileStream("save.json", FileMode.Create, FileAccess.Write);

        var jsonWriter = new Utf8JsonWriter(fileStream);

        jsonWriter.WriteStartObject();

        // turn
        jsonWriter.WritePropertyName("turn"u8);
        jsonWriter.WriteStartObject();
        jsonWriter.WriteNumber("turn"u8, turnContext.Turn);
        jsonWriter.WriteEndObject();

        // player context
        jsonWriter.WritePropertyName("players"u8);
        jsonWriter.WriteStartArray();
        foreach (var p in playerList)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("id"u8, p.Id);
            jsonWriter.WriteString("type"u8, p.PlayerType);
            jsonWriter.WriteNumber("completedTurn"u8, p.CompletedTurn);
            foreach (var t in p.Traits)
            {
                jsonWriter.WritePropertyName(t.TraitType);
                jsonWriter.WriteStartObject();
                foreach (var prop in t.Properties)
                {
                    jsonWriter.WriteString(prop.Name, prop.Value);
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndArray();

        // actors
        jsonWriter.WritePropertyName("actors"u8);
        jsonWriter.WriteStartArray();

        foreach (var a in actorList)
        {
            jsonWriter.WriteStartObject();
            jsonWriter.WriteString("id", a.Id);
            jsonWriter.WriteString("type", a.ActorType);
            foreach (var t in a.Traits)
            {
                jsonWriter.WritePropertyName(t.TraitType);
                jsonWriter.WriteStartObject();
                foreach (var p in t.Properties)
                {
                    jsonWriter.WriteString(p.Name, p.Value);
                }
                jsonWriter.WriteEndObject();
            }

            jsonWriter.WriteEndObject();
        }

        jsonWriter.WriteEndArray();


        // end
        jsonWriter.WriteEndObject();

        await jsonWriter.FlushAsync();

        fileStream.Flush();
        fileStream.Close();
    }
    public IEnumerable<DehydratedActor> DehydrateActorContext(IActorContext actorContext)
    {
        List<DehydratedActor> dataToPersist = new();
        foreach (var actor in actorContext.AsQueryable())
        {
            var id = actor.Id;
            var actorType = actor.GetType().FullName ?? throw new NotSupportedException("invalid type of actor");
            var traitsToPersist = DehydrateTraits(actor.Traits);

            dataToPersist.Add(new DehydratedActor(id, actorType, traitsToPersist));
        }

        return dataToPersist;
    }
    public IEnumerable<DehydratedPlayer> DehydratePlayerContext(IPlayerContext playerContext)
    {
        List<DehydratedPlayer> dataToPersist = new();
        foreach (var player in playerContext.GetAllPlayers())
        {
            var traitsToPersist = DehydrateTraits(new Trait[] { player.IdealConditions, player.Relatable, player.Resourceful, player.Capable });

            dataToPersist.Add(new DehydratedPlayer(player.Id, player.GetType().FullName ?? throw new NotSupportedException("invalid type of player"), player.CompletedTurn, traitsToPersist));
        }

        return dataToPersist;
    }

    private static DehydratedTrait[] DehydrateTraits(IEnumerable<Trait> traits)
    {
        var traitsToPersist = new List<DehydratedTrait>();

        foreach (var trait in traits)
        {
            var traitType = trait.GetType().FullName ?? throw new NotSupportedException("invalid type of trait");

            var propertyWriter = new PropertyWriter();
            trait.Persist(propertyWriter);

            if (propertyWriter.IsUsed)
            {
                traitsToPersist.Add(new DehydratedTrait(traitType, propertyWriter.Result));
            }
        }

        return traitsToPersist.ToArray();
    }
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