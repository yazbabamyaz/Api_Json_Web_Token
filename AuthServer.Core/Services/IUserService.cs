using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //üye kayıt et. Repository oluşturmadım direkt service oluşturdum. zaten hazır metotlar ıdentity den gelece
    public interface IUserService
    {
        //3 büyük class gelecek zaten identity den usermanager- rolemanager-signinmanager geliyor.
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
    }
}
