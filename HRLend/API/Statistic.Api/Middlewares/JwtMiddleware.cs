using StatisticApi.Settings;
using StatisticApi.Utils;
using Microsoft.Extensions.Options;

namespace StatisticApi.Authorization
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSetting _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSetting> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = jwtUtils.ValidateJwtToken(token);

            if (user != null)
                context.Items["User"] = user;

            //if (userId != null)
            //{
            //    //attach user to context on successful jwt validation
            //    if (context.Session.Keys.Contains("User"))
            //    {
            //        context.Items["User"] = context.Session.Get<User>("User");
            //    }
            //    else
            //    {
            //        var user = userRepository.GetUser(userId.Value);
            //        context.Items["User"] = user;
            //        context.Session.Set<User>("User", user);
            //    }
            //}

            await _next(context);
        }
    }
}
