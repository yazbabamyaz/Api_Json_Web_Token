using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {

        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;//identity ile 3 manager gelir.user-signin-role-
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshToeknService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshToeknService;
        }

        public async Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto)); 
            var user=await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Response<TokenDto>.Fail("Email or Password is wrong.", 400,true);
            if (!await _userManager.CheckPasswordAsync(user,loginDto.Password))
            {
                return Response<TokenDto>.Fail("Email or Password is wrong.", 400, true);
            }
            //artık token üretebiliriz.
            var token = _tokenService.CreateToken(user);
            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();//db de daha önceden kayıtlı mı refresh token var mı yok sa kaydetcez.Daha önceden bu user a refresh token vermişmiyiz.
            if (userRefreshToken==null)
            {
                //db ye kaydet-memory e kaydet altta savechanges etcez
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code=token.RefreshToken, Expiration=token.RefreshTokenExpiration});
            }
            else
            {
                //güncelle
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expiration = token.RefreshTokenExpiration;
            }
            await _unitOfWork.CommitAsync();//db ye yansıttık
            return Response<TokenDto>.Success(token, 200);
        }

        public Response<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.Id == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (client==null)
            {
                //db de böyle bir client yoksa
                return  Response<ClientTokenDto>.Fail("ClientId or ClientSecret not found.",404,true);
            }
            var token = _tokenService.CreateTokenByClient(client);
            return Response<ClientTokenDto>.Success(token, 200);
        }


        //yukarıdaki 2 metotta normal kullanıcı için ve kullanıcı olmayan clientlar için token oluşturduk.
        //ŞİMDİ İSE REFRESHTOKEN İLE BERABER TOKEN ALCAZ.
        public async Task<Response<TokenDto>> CreateTokenByRefresh(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken == null)
                return Response<TokenDto>.Fail("Refresh token not found", 404,true);

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);
            if (user==null)
            {
                return Response<TokenDto>.Fail("User id not found.", 404, true); 
            }
            var tokenDto = _tokenService.CreateToken(user);
            //yeni token oluşturduğuma göre bunun içinde hem refresh hem de access token var. yani yeni bir refreshtoken geldi.
            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expiration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();
            return Response<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            //refreshtoken ı db de null yapcaz(olur da kullanıcı logout olursa)
            var existRefreshToken=await _userRefreshTokenService.Where(x=>x.Code== refreshToken).SingleOrDefaultAsync();
            if (existRefreshToken==null)
            {
                return Response<NoDataDto>.Fail("Refresh token not found",404,true);
            }
            _userRefreshTokenService.Remove(existRefreshToken);
           
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);//200 alırsa silme başarılı
        }
    }
}
