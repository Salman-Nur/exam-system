namespace ExamSystem.Web.Exceptions;

public class SearchRequestInvalidException : Exception
{
    public SearchRequestInvalidException(string message) : base(message)
    {

    }

    public SearchRequestInvalidException(IEnumerable<string> errors)
        : base(errors.Aggregate((x, y) => $"{x}, {y}"))
    {

    }
}
