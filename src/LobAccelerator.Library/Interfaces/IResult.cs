namespace LobAccelerator.Library.Interfaces
{
    public interface IResult
    {
        bool HasError();
        string GetError();
        string GetDetailedError();
    }
}
