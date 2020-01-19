using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using GrandElementApi.Responses;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace GrandElementApi.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        IUserService _userService;

        public TokenMiddleware(RequestDelegate next, IUserService userService)
        {
            this._next = next;
            this._userService = userService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                await _next(context);
                return;
            }
            //если логинимся, то проверять авторизацию не надо
            if (context.Request.Path == "/user/login")
            {
                await _next(context);
                return;
            }

            var tokenHeaders = context.Request.Headers["Authorization"];
            if (tokenHeaders.Count > 0)
            {
                var guid = tokenHeaders.ToArray()[0];
                if (await _userService.IsValidTokenAsync(guid))
                {
                    await _next(context);
                    return;
                }
            }

            context.Response.StatusCode = 200;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var resp = new DataResponse<object>() { Success = false, Message = "Не авторизированный запрос", Code = DataResponse<object>.UNAUTHORIZED };

            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string jsonString = JsonConvert.SerializeObject(resp, Formatting.Indented, settings);
            await context.Response.WriteAsync(jsonString, Encoding.UTF8);
        }
    }
}
