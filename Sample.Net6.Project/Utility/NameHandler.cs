using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sample.Net6.ExceptionService;

namespace Sample.Net6.Project.Utility
{
    public class NameHandler : AuthorizationHandler<NameRequirement>
    {
        private readonly IUserService _userService;

        public NameHandler(IUserService userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NameRequirement requirement)
        {
            // 此處進行驗證
            
            if (!context.User.Claims.Any())
            {
                return Task.CompletedTask;
            }

            string userId = context.User.Claims.First(c => c.Type == "UserId").Value;
            string name = context.User.Claims.First(c => c.Type == "Name").Value;

            if (_userService.Validate(userId, name))
            {
                context.Succeed(requirement); // 驗證通過
            }

            return Task.CompletedTask;
        }
    }
}
