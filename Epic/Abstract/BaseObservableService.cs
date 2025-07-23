using System.Reactive.Disposables;
using Epic.Models;
using Microsoft.Extensions.Hosting;

namespace Epic.Abstract;

public abstract class BaseObservableService<T> : IObservable<Message<T>>, IHostedService
{
    private readonly HashSet<IObserver<Message<T>>> _observers = [];

    public abstract Task StartAsync(CancellationToken cancellationToken);
    public abstract Task StopAsync(CancellationToken cancellationToken);

    public IDisposable Subscribe(IObserver<Message<T>> observer)
    {
        _observers.Add(observer);

        return Disposable.Create(() => _observers.Remove(observer));
    }

    protected void PublishToAll(Message<T> message)
    {
        foreach (var observer in _observers)
        { 
            observer.OnNext(message);
        }
    }
}
