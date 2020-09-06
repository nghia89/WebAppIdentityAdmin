using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WebAppIdentityAdmin.Services
{
    public class CheckAuthenticationMiddlewares
    {
        private readonly RequestDelegate _next;
        public CheckAuthenticationMiddlewares(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string authHeader = context.Request.Headers["cookie"];
            if (authHeader == null)
            {
                context.Request.Path = "/Account/SignIn";
            }
            else
            {
                Console.WriteLine("Đã đăng nhập.");
            }

            await _next(context);
        }
    }
}
