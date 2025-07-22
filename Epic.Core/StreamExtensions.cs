using System.Reactive.Linq;
using Epic.Core.Models;

namespace Epic.Core;

public static class StreamExtensions
{
    public static IObservable<Message<T1>> Map<T, T1>(this IObservable<Message<T>> observable, Func<T, T1> map)
    {
        return observable.Select(x => new Message<T1>(map(x.Value), x._taskCompletionSource));
    }

    public static IObservable<Message<T>> Filter<T>(this IObservable<Message<T>> observable, Predicate<T> filter, string reason)
    {
        return observable.Where(x =>
        {
            if (!filter(x.Value))
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
}
