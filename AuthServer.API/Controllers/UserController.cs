using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            //yeni kullanıcı kaydı
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        [Authorize]//mutlaka bir token ister ki girsin
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            //gelen token dan user vs alacaz
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.Identity.Name));
            //istekteki token içinden name claimini buluyor.
        }
    }
}
