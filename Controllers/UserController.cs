using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrandElementApi.Interfaces;
using GrandElementApi.Models;
using GrandElementApi.Responses;
using GrandElementApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GrandElementApi.Controllers
{
    [Route("[controller]/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<ApiResponse> Login(User userReq) {
            try
            {
                var user = await _userService.GetUserAsync(userReq.Login, userReq.Password);
                var guid = await _userService.MakeSessionAsync(user.Id);
                return new DataResponse<Authorization>() { Code = ApiResponse.OK, Success = true, Data=new Authorization() { Token = guid, UserId=user.Id, Name=user.Name } };
            }
            catch (ArgumentException e)
            {
                _logger.LogInformation(e.Message);
                return ApiResponse.DefaultError(e);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return ApiResponse.DefaultError();
            }
        }
    }
}