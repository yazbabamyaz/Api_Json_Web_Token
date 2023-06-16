using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Services
{
    public static class SignService//public gerek yok
    {
        //elektronik imza vs bunlar asimetrik imzalamalardır.publickey - privateky vs
        //simetrik sec. key dönecek.
        public static SecurityKey GetSymmetricSecurityKey(string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }

    }
}
