namespace SharepointConsoleApp.Interfaces
{
    public interface IResult
    {
        bool HasError();
        string GetError();
        string GetDetailedError();
    }
}
