﻿using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<UserApp> _userManager;
        //constructor da DI nesnesi olarak geçelim.
        public UserService(UserManager<UserApp> userManager)
        {
            _userManager= userManager;
        }
        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
           var user= new UserApp { Email= createUserDto.Email,UserName=createUserDto.UserName };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);
            //aynı kayıtları tekrar kaydetme ihtimaline karşı:
            //identity kütüphanesinden geliyor.
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x=>x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);//birden fazla hata olabilir o yüzden errrordto nun parametrelerinden list olanı kullandık.cLİENT IN GÖNDERDİĞİ DATA DA PROBLEM VAR.
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userName)
        {
            var user =await _userManager.FindByNameAsync(userName);
            if (user==null)
            {
                return Response<UserAppDto>.Fail("Username not found.",404, true);
            }
            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }
    }
}
