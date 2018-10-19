using System.Collections.Generic;

namespace LobAccelerator.App.Model
{
    public class ValidationResult<T>
    {
        public bool IsValid { get; set; }

        public T Result { get; set; }

        public IEnumerable<string> ValidationMessages { get; set; }
    }
}
