using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Extensions
{
    public static class CustomTokenAuth
    {
        public static void AddCustomTokenAuth(this IServiceCollection services,CustomTokenOption tokenOptions)
        {


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;//2 şemayı birbirine bağladık.
                                                                                        //farklı üyelik sistemleri varsa bayiler için normal kullanıcılar için bunlar sema biz default belirttik 1 TANE VAR. ŞİMDİ DOĞRULAMA NASIL OLCAK COOKİE-JWT APİ OLD. DOLAYI JWT YAPCAZ.
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
            {
                
                opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidIssuer = tokenOptions.Issuer,
                    ValidAudience = tokenOptions.Audience[0],//ilk ine bakmak doğrulamak için yeterli
                    IssuerSigningKey = SignService.GetSymmetricSecurityKey(tokenOptions.SecurityKey),

                    ValidateIssuerSigningKey = true,//önemli mutlaka bir imzası olmalı.benim gönderdiğim issuer mu
                    ValidateAudience = true,
                    ValidateIssuer = true,//bunu da doğrula...
                    ValidateLifetime = true,//ömrünü kontrol et

                    //ÇOK KULLANILMAZ AMA BİLELİM.
                    ClockSkew = TimeSpan.Zero,//1 saaat ömür varsa default olarak  5 dk da kendi ekler. neden: farklı zaman aralıklarındaki serverlara kurabilirsin arada farkı minimize etmek için.ONU İPTAL EDİYORUZ GİBİ 2 SERVER ZAMANINI DA EŞİTLEDİK. FARK SIFIR.
                };
            }
);



        }
    }
}
