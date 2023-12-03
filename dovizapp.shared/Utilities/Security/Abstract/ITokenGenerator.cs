using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dovizapp.shared.Utilities.Security.Abstract
{
    public interface ITokenGenerator
    {
        string CreateToken(string username, string role);
        bool ValidateToken(string token);
    }
}