using GreenStar.Core.TurnEngine;

namespace GreenStar.Core
{
    public static class TurnManagerExtensions
    {
        public static TurnManagerBuilder AddActors(this TurnManagerBuilder self, IActorBuilder actorBuilders)
        {
            foreach (var actor in actorBuilders.Create())
            {
                self.AddActor(actor);
            }

            return self;
        }
    }
}