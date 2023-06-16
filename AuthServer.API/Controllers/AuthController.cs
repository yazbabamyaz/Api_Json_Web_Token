using AuthServer.Core.DTOs;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : CustomBaseController    {
        //Api katmanı service katmanıyla haberleşecek
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken(LoginDto loginDto)
        {
            //Üye olduğum bilgilerle token istiyorum.Bana token üretiyor.
            var result=await _authenticationService.CreateTokenAsync(loginDto);
            return ActionResultInstance(result);
        }

        [HttpPost]
        public  IActionResult CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var result =  _authenticationService.CreateTokenByClient(clientLoginDto);
            return ActionResultInstance(result);
        }

        
        [HttpPost]//string alma genelde güvenlik vs..
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenDto refreshTokenDto)
        {
            var result = await _authenticationService.RevokeRefreshToken(refreshTokenDto.Token);
            
            return ActionResultInstance(result);
        }

        [HttpPost]
        public async  Task<IActionResult> CreateTokenByRefrehToken(RefreshTokenDto refreshTokenDto)
        {
            //Elimde refresh token varsa-yani daha önceden token aldıysam- onu parametre olarak veriyorum ve bana token üretiyor.
            //service katmanında AuthenticationService te metot sonuna Token yazmamış olabilirim.
            var result = await _authenticationService.CreateTokenByRefresh(refreshTokenDto.Token);
            return ActionResultInstance(result);
        }
    }
}
