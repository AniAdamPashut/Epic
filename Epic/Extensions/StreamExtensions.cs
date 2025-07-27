using System.Reactive.Linq;
using Epic.Abstract.Functions;
using Epic.Abstract.Functions.Async;
using Epic.Models;

namespace Epic.Extensions;

public static class StreamExtensions
{

    #region Map Functions 

    public static IObservable<Message<T1>> Map<T, T1>(this IObservable<Message<T>> observable, Func<T, T1> map)
    {
        return observable.Select(x => new Message<T1>(map(x.Value), x.Context));
    }

    public static IObservable<Message<T1>> Map<T, T1>(this IObservable<Message<T>> observable, IMapFunction<T, T1> mapFunction)
    {
        return observable.Map(mapFunction.Map);
    }

    public static IObservable<Message<T1>> MapAsync<T, T1>(this IObservable<Message<T>> observable, Func<T, Task<T1>> map)
    {
        return observable.SelectMany(async x => new Message<T1>(await map(x.Value), x.Context));
    }


    public static IObservable<Message<T1>> MapAsync<T, T1>(this IObservable<Message<T>> observable, IAsyncMapFunction<T, T1> asyncMapFunction)
    {
        return observable.MapAsync(asyncMapFunction.MapAsync);
    }

    #endregion

    #region Flat Map Functions

    public static IObservable<Message<T1>> FlatMap<T, T1>(this IObservable<Message<T>> observable, Func<T, IList<T1>> map)
    {
        return observable.SelectMany(msg => msg.BindContexts(map));
    }

    public static IObservable<Message<T1>> FlatMap<T, T1>(this IObservable<Message<T>> observable, IFlatMapFunction<T, T1> flatMapFunction)
    {
        return observable.FlatMap(flatMapFunction.FlatMap);
    }


    public static IObservable<Message<T1>> FlatMapAsync<T, T1>(this IObservable<Message<T>> observable, Func<T, Task<IList<T1>>> map)
    {
        return observable.SelectMany(async msg => await msg.BindContextsAsync(map)).SelectMany(it => it);
    }

    public static IObservable<Message<T1>> FlatMapAsync<T, T1>(this IObservable<Message<T>> observable, IAsyncFlatMapFunction<T, T1> asyncFlatMapFunction)
    {
        return observable.FlatMapAsync(asyncFlatMapFunction.FlatMapAsync);
    }

    #endregion

    public static IObservable<IGroupedObservable<TKey, Message<T>>> KeyBy<T, TKey>(this IObservable<Message<T>> observable, Func<T, TKey> getKey)
    {
        return observable.GroupBy(x => getKey(x.Value));
    }

    #region Filter

    public static IObservable<Message<T>> Filter<T>(this IObservable<Message<T>> observable, Predicate<T> shouldFilter, string reason)
    {
        return observable.Where(x =>
        {
            if (!shouldFilter(x.Value))
                return true;

            x.Filter(reason);
            return false;
        });
    }

    public static IObservable<Message<T>> Filter<T>(this IObservable<Message<T>> observable, IFilterFunction<T> filterFunction)
    {
        return observable.Filter(filterFunction.ShouldFilter, filterFunction.Reason);
    }

        public static IObservable<Message<T>> FilterAsync<T>(this IObservable<Message<T>> observable, Func<T, Task<bool>> shouldFilter, string reason)
    {

        return observable
            .SelectMany(async msg => new { Message = msg, ShouldFilter = await shouldFilter(msg.Value) })
            .Where(item =>
            {
                if (!item.ShouldFilter)
                    return true;

                item.Message.Filter(reason);
                return false;
            })
            .Select(item => item.Message);
    }

    public static IObservable<Message<T>> FilterAsync<T>(this IObservable<Message<T>> observable, IAsyncFilterFunction<T> filterFunction)
    {
        return observable.FilterAsync(filterFunction.ShouldFilterAsync, filterFunction.Reason);
    }

    #endregion

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
