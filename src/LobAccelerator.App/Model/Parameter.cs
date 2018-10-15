using Microsoft.WindowsAzure.Storage.Table;

namespace LobAccelerator.App.Models
{
    public class Parameter
        : TableEntity
    {
        public string Value { get; set; }
    }
}
