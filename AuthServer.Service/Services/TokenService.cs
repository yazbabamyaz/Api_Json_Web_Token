using AuthServer.Core.Configuration;
using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
//using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _customTokenOption;

        //CustomTokenOption bunu direk geçmicez.IOptions interface ile yapcaz.
        public TokenService(UserManager<UserApp> userManager,IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _customTokenOption = options.Value;
            
        }
        private string CreateRefreshToken()
        {
            //random token üretecek
            //return Guid.NewGuid().ToString();//bu da olabilirdi.
            var numberByte = new Byte[32];//32 bytelık değer üretcem
            using var rnd = RandomNumberGenerator.Create();//random değer üretir.
            //bytelarını al numberbyte a aktar.
            rnd.GetBytes(numberByte);
            return Convert.ToBase64String(numberByte);  //base64 ile encode et stringe dönüştür byte ı
        }

        //Claim oluşturcaz.USER İÇİN ÜYELİK SİSTEMİ GEREKTİREN BİR TOKEN OLUŞTURMAK İÇİN CLAİMLERİ BU METOTLA OLUŞTURCAZ.
        private IEnumerable<Claim> GetClaims(UserApp userApp,List<String> audiences)
        {
            var userList=new List<Claim>
            {
                //kullanıcıyla alakalı claimler
                //payload da olacak bilgileri claim olarak ekliyoruz.
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),     
                
                new Claim(JwtRegisteredClaimNames.Jti ,userApp.Email),
                //new Claim("email",userApp.Email),//böyle de olur.
                new Claim(ClaimTypes.Name,userApp.UserName),
               
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())  //random değer üretir zorunlu değil ama gerekebilir.
            };
            //bir de tokenın kendisi ile alakalı claimler vardı audience vs.
            //foreach ile dönme gibi
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud,x)));//istek yaptığımızda token aud isine bakılcak kendisine istek yapılmaya uygun mu diye vs
        return userList;
        }

        //clientlar için claim oluşturma-ÜYELİK SİSTEMİ GEREKTİRMEYEN BİR TOKEN OLUŞTURMAK İSTEDİĞİMİZDE- CLAİMLERİ BU METOTLA OLUŞTURCAM.
        private IEnumerable<Claim> GetClaimsByClient(Client client)
        {
            var claims = new List<Claim>();
            claims.AddRange(client.Audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
            //token kimin için?? hangi clientid  için
            new Claim(JwtRegisteredClaimNames.Sub, client.Id.ToString());
            return claims;
        }


        //SIRA TOKEN OLUŞTURMADA
        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration=DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.RefreshTokenExpiration);
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);//tokenı imzalayacak key de hazır.
            //şimdi imzamızı oluşturalım.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            //token ı oluşturalım:
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _customTokenOption.Audience),
                signingCredentials: signingCredentials
                );

            var handler=new JwtSecurityTokenHandler();
            var token=handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return tokenDto;
        }

        public ClientTokenDto CreateTokenByClient(Client client)
        {


            var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
           
            var securityKey = SignService.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);//tokenı imzalayacak key de hazır.
            //şimdi imzamızı oluşturalım.
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            //token ı oluşturalım:
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: GetClaimsByClient(client),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new ClientTokenDto
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                
               
            };
            return tokenDto;



        }
    }
}
