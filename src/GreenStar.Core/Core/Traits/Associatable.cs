using System;
using GreenStar.Core.Persistence;

namespace GreenStar.Core.Traits
{
    public class Associatable : Trait
    {
        public Guid PlayerId { get; set; }

        public string Name { get; set; }

        public override void Load(IPersistenceReader reader)
        {
            PlayerId = reader.Read<Guid>(nameof(PlayerId));
            Name = reader.Read<string>(nameof(Name));
        }

        public override void Persist(IPersistenceWriter writer)
        {
            writer.Write(nameof(PlayerId), PlayerId);
            writer.Write(nameof(Name), Name);
        }

        public bool IsOwnedByAnyPlayer()
            => !IsOwnedByPlayer(Guid.Empty);
        public bool IsOwnedByPlayer(Guid playerId)
            => PlayerId == playerId;
    }
}