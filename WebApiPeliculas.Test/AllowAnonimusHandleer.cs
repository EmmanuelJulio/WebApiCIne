using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiPeliculas.Test
{
    public class AllowAnonimusHandleer : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach(var requeriment in context.PendingRequirements.ToList())
            {
                context.Succeed(requeriment);
            }
            return Task.CompletedTask;
        }
    }
}
