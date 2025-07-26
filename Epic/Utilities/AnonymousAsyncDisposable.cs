
namespace Epic.Utilities;

public class AnonymousAsyncDisposable(Func<ValueTask> disposeAsync) : IAsyncDisposable
{
    private readonly Func<ValueTask> _disposeAsync = disposeAsync ?? throw new ArgumentNullException(nameof(disposeAsync));
    private bool _isDisposed = false;

    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        if (_isDisposed) return ValueTask.CompletedTask;

        _isDisposed = true;
        return _disposeAsync();
    }
}
