using System;
using System.Linq;
using System.Linq.Expressions;

namespace GreenStar.Core;

public static class QueryableExtensions
{
    public static IQueryable<A> With<A, T>(this IQueryable<A> self, Func<T, bool> predicate) where A : Actor where T : Trait
        => self.Where(actor => actor.HasTrait<T>() && predicate(actor.Trait<T>()));

    public static A? TakeOneRandom<A>(this IQueryable<A> self) where A : Actor
    {
        var rnd = new Random();

        return self.OrderBy(a => rnd.Next()).FirstOrDefault();
    }
}
