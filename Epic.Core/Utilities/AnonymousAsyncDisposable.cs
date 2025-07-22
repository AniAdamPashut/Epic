
namespace Epic.Core.Utilities;

public class AnonymousAsyncDisposable : IAsyncDisposable
{
    private readonly Func<ValueTask> _disposeAsync;
    private bool _isDisposed = false;

    public AnonymousAsyncDisposable(Func<ValueTask> disposeAsync)
    {
        _disposeAsync = disposeAsync ?? throw new ArgumentNullException(nameof(disposeAsync));
    }

    public ValueTask DisposeAsync()
    {
        if (_isDisposed) return ValueTask.CompletedTask;

        _isDisposed = true;
        return _disposeAsync();
    }
}
