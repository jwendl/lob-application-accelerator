using LobAccelerator.Library.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LobAccelerator.Library.Interfaces
{
    interface IValidator
    {
        bool Validate(TeamsInput ti);
    }
}
