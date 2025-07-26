namespace Epic.Exceptions;

public class UnknownContextException<TIntended>(Type Unindented) 
    : ArgumentException($"Unindented type {Unindented.FullName} cannot be converted into {nameof(TIntended)}");
