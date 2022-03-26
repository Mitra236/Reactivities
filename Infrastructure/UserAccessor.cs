using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Security
{
    public class UserAccesor : IUserAccessor
    {
        public readonly IHttpContextAccessor _httpContextAccesor;
        public UserAccesor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccesor = httpContextAccessor;
            
        }

        public string GetUsername()
        {
            return _httpContextAccesor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }
    }
}
