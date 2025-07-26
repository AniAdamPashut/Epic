using System.Reactive.Linq;
using Epic.Models;
using Microsoft.Win32.SafeHandles;

namespace Epic.Extensions;

public static class StreamExtensions
{
    public static IObservable<Message<T1>> Map<T, T1>(this IObservable<Message<T>> observable, Func<T, T1> map)
    {
        return observable.Select(x => new Message<T1>(map(x.Value), x.Context));
    }

    public static IObservable<Message<T1>> FlatMap<T, T1>(this IObservable<Message<T>> observable, Func<T, IList<T1>> map)
    {
        return observable.SelectMany(x => x.BindContexts(map));
    }

    public static IObservable<IGroupedObservable<TKey, Message<T>>> KeyBy<T, TKey>(this IObservable<Message<T>> observable, Func<T, TKey> getKey)
    {
        return observable.GroupBy(x => getKey(x.Value));
    }

    public static IObservable<Message<T>> Filter<T>(this IObservable<Message<T>> observable, Predicate<T> shouldKeep, string reason)
    {
        return observable.Where(x =>
        {
            if (!shouldKeep(x.Value))
                return true;

            x.Filter(reason);
            return false;
        });
    }

    public static IDisposable Acknowledge<T>(this IObservable<Message<T>> observable, Action<T> consume)
    {
        return observable.Subscribe(x =>
        {
            consume(x.Value);
            x.Acknowledge();
        });
    }

    public static IDisposable Acknowledge<T, TKey>(this IObservable<IGroupedObservable<TKey, Message<T>>> observable, Action<T, TKey> consume)
    {
        return observable.Subscribe(group =>
        {
            group.Subscribe(msg =>
            {
                consume(msg.Value, group.Key);
                msg.Acknowledge();
            });
        });
    }
}
