using Epic.Abstract.Functions;

namespace Epic.Playground.Filters;

public class AAFilterFunction : IFilterFunction<string>
{
    public string Reason => "The string starts with the letters 'aa'";

    public bool ShouldFilter(string data) => data.StartsWith("aa");
}
