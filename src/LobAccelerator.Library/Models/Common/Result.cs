namespace LobAccelerator.Library.Models.Common
{
    public class Result<T>
    {
        public T Value { get; set; }
        public bool HasError { get; set; } = false;
        public string Error { get; set; } = string.Empty;
        public string DetailedError { get; set; } = string.Empty;
    }
}
