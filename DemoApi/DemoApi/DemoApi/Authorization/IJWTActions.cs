using DemoApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Authorization
{
    public interface IJWTActions
    {
        public string GenerateToken(User user);
        public string ValidateToken(string token);
    }
}
