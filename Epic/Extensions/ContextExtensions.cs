using Epic.Exceptions;
using Epic.Models;
using Epic.Models.Contexts;

namespace Epic.Extensions;

public static class ContextExtensions
{
    public static TContext GetContext<TContext>(this MessageStatus status)
        where TContext : class
    {
        if (status.Context is TContext context)
            return context;
        

        if (status.Context is SharedContext sharedContext)
        {
            if (sharedContext.InnerContext is TContext anotherChanceAtTheContext)
                return anotherChanceAtTheContext;
        }

        throw new UnknownContextException<TContext>(status.Context.GetType());
    }
}