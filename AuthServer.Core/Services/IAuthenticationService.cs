using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //login işlemleri pass- username alcaz doğruysa token döncez.
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);
        Task<Response<TokenDto>> CreateTokenByRefresh(string refreshToken);
        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);//refresh token ı null yapmak için.
        //token çalınsa bir ömrü var kısadır. ama resfresh token çalınırsa bunu devreye sokarız.
        // üyelik sistemi olmadan client a token verelim.
        Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto);
    }
}
