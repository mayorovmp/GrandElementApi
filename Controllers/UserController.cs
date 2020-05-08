using System;
using System.Threading.Tasks;
using AutoMapper;
using GrandElementApi.DTOs;
using GrandElementApi.Interfaces;
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
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger, IUserService userService, IMapper mapper)
        {
            _logger = logger;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthorizationDTO>> Login(UserLoginDTO userReq) {
            try
            {
                var user = await _userService.GetUserAsync(userReq.Login, userReq.Password);
                var guid = await _userService.MakeSessionAsync(user.Id);

                return new AuthorizationDTO() { Token = guid, UserId = user.Id, Name = user.Name };
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
                return Problem(e.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<UserDTO>> Get([FromHeader]Guid authorization)
        {
            try
            {
                var res = await _userService.GetUserAsync(authorization);
                return Ok(_mapper.Map<UserDTO>(res));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Problem();
            }
        }
    }
}