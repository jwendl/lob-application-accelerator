using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Models.Common
{
    public class Result
    {
        public object Value { get; set; }
        public bool HasError { get; set; } = false;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
