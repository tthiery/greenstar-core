using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace GreenStar.Persistence;

public record DehydratedActor(Guid Id, string ActorType, DehydratedTrait[] Traits);
public record DehydratedTrait(string TraitType, DehydratedTraitProperty[] Properties);
public record DehydratedTraitProperty(string Name, string Value);

public class FileSystemPersistence : IPersistence
{
    public async Task PersistActorsAsync(IActorContext actorContext)
    {
        var list = PersistActorContext(actorContext);

        using var fileStream = new FileStream("save.json", FileMode.Create, FileAccess.Write);

        var jsonWriter = new Utf8JsonWriter(fileStream);

        jsonWriter.WriteStartArray();

        foreach (var a in list)
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

        await jsonWriter.FlushAsync();

        fileStream.Flush();
        fileStream.Close();
    }
    public IEnumerable<DehydratedActor> PersistActorContext(IActorContext actorContext)
    {
        List<DehydratedActor> dataToPersist = new();
        foreach (var actor in actorContext.AsQueryable())
        {
            var id = actor.Id;
            var actorType = actor.GetType().FullName ?? throw new NotSupportedException("invalid type of actor");
            var traitsToPersist = new List<DehydratedTrait>();

            foreach (var trait in actor.Traits)
            {
                var traitType = trait.GetType().FullName ?? throw new NotSupportedException("invalid type of trait");

                var propertyWriter = new PropertyWriter();
                trait.Persist(propertyWriter);

                if (propertyWriter.IsUsed)
                {
                    traitsToPersist.Add(new DehydratedTrait(traitType, propertyWriter.Result));
                }
            }

            dataToPersist.Add(new DehydratedActor(id, actorType, traitsToPersist.ToArray()));
        }

        return dataToPersist;
    }
}

public class PropertyWriter : IPersistenceWriter
{
    private List<DehydratedTraitProperty> _list = new();

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
            _ => throw new NotSupportedException("not supported data type in dehydration"),
        };

    public DehydratedTraitProperty[] Result
        => _list.ToArray();
}