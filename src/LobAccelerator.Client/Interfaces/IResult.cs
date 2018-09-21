namespace LobAccelerator.Client.Interfaces
{
    public interface IResult
    {
        bool HasError();
        string GetError();
        string GetDetailedError();
    }
}
